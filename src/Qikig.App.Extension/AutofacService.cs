using FreeRedis;
using FreeSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Qikig.App.Extension.AppConfig;
using System.Text;
using Yitter.IdGenerator;

namespace Qikig.App.Extension
{
    /// <summary>
    /// 统一 service 注入
    /// </summary>
    public static class AutofacService
    {
        /// <summary>
        /// 全注册Service log、freesl,freeredis,Auth jwt
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddQikigService(this IServiceCollection services, IConfiguration configuration)
        {
            var appConfig = configuration.Get<AppConfigs>();
            if (appConfig == null)
            {
                throw new ArgumentNullException(nameof(appConfig), "appConfig is null.");
            }
            services.AddLog(appConfig);
            services.AddAutofacService(configuration);
            services.AddFreeSql(appConfig);
            services.AddRedisCache(appConfig);
            services.AddAuthentication(appConfig);

            return services;
        }
        

        /// <summary>
        /// 全注册Host Autofac
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public static IHostBuilder AddQikigHost(this IHostBuilder host, IConfiguration configuration)
        {
            host.Register(configuration);
            return host;
        }

        /// <summary>
        /// 注册业务组件
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        private static IServiceCollection AddAutofacService(this IServiceCollection services, IConfiguration configuration)
        {

            #region 必要组件
            var swconfig = configuration.Get<AppConfigs>();
            services.AddMemoryCache();
            services.AddDistributedMemoryCache();
            services.AddHttpContextAccessor();
            //防伪造
            services.AddAntiforgery(options =>
            {
                // Set Cookie properties using CookieBuilder properties†.
                options.FormFieldName = "AntiforgeryFieldname";
                options.HeaderName = "X-CSRF-TOKEN-HEADERNAME";
                options.SuppressXFrameOptionsHeader = false;
            });

            //跨域
            services.AddCors(policy =>
            {
                policy.AddPolicy("CorsPolicy", opt => opt
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
                .WithExposedHeaders("X-Pagination")
                );

            });


            //注入Swagger
            services.AddSwaggerGen(options =>
            {
                options.AddAuthenticationHeader(swconfig);
            });

            var idGenerate = configuration.Get<AppConfigs>()?.IdGenerate;
            if (idGenerate != null)
            {
                var idGeneratorOptions = new IdGeneratorOptions((ushort)idGenerate.WorkerId) { WorkerIdBitLength = (byte)idGenerate.WorkerIdBitLength };
                //配置雪花Id算法
                YitIdHelper.SetIdGenerator(idGeneratorOptions);
            }
            #endregion

            return services;
        }
        #region database、redis、jwt

        /// <summary>
        /// 注册 freesql
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        private static IServiceCollection AddFreeSql(this IServiceCollection services, AppConfigs appConfigs)
        {
            ConnectionStrings? connectionString = appConfigs.ConnectionStrings;
            Func<IServiceProvider, IFreeSql> fsqlFactory = x =>
            {
                IFreeSql fsql = new FreeSqlBuilder()
                    .UseConnectionString(DataType.MySql, connectionString.SqlConnection)
#if DEBUG
                    .UseMonitorCommand(cmd => Console.WriteLine(cmd.CommandText)) //打印 SQL
#endif
                    .Build();


                return fsql;
            };

            // FreeSql单例注入
            services.AddSingleton(fsqlFactory);
            services.AddScoped<UnitOfWorkManager>();
            services.AddFreeRepository();

            return services;
        }
        /// <summary>
        /// 注册redis
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        private static IServiceCollection AddRedisCache(this IServiceCollection services,
                AppConfigs appConfigs)
        {
            RedisConfig? redisconfig = appConfigs.Redis;
            if (redisconfig == null)
            {
                throw new ArgumentNullException(nameof(redisconfig), "redisconfig is null.");
            }
            //free Redis
            var redisOptions = new ConnectionStringBuilder();
            redisOptions.Password = redisconfig.RedisPass;
            redisOptions.Database = redisconfig.Database;
            redisOptions.Host = redisconfig.Configuration;

            services.AddSingleton(c => new FreeRedisService(redisOptions));
            return services;
        }

        /// <summary>
        /// 注册 jwt
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        private static IServiceCollection AddAuthentication(this IServiceCollection services,
                AppConfigs appConfigs)
        {
            TokenPotions? tokenOptions = appConfigs.TokenPotions;
            if (tokenOptions == null)
            {
                throw new ArgumentNullException(nameof(tokenOptions), "tokenOptions is null.");
            }
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,//是否在令牌期间验证签发者
                    ValidateAudience = true,//是否验证接收者
                    ValidateLifetime = true,//是否验证失效时间
                    ValidateIssuerSigningKey = true,//是否验证签名
                    ValidIssuer = tokenOptions.Issuer,//签发者, 签发token的人
                    ValidAudience = tokenOptions.Audience,//接收者
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.SecretKey!))//密钥
                };
            });
            
            return services;
        }

        #endregion
    }
}
