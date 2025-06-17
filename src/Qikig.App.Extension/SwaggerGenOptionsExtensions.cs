using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Qikig.App.Extension.AppConfig;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Qikig.App.Extension
{
    public static class SwaggerGenOptionsExtensions
    {
        /// <summary>
        /// 为swagger增加Authentication报文头
        /// </summary>
        /// <param name="option"></param>
        public static void AddAuthenticationHeader(this SwaggerGenOptions options, AppConfigs appConfigs)
        {
            SwaggerConfig? cconfig = appConfigs.SwaggerConfig;
            options.UseInlineDefinitionsForEnums();
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference=new OpenApiReference{Id="Bearer",Type=ReferenceType.SecurityScheme},
                        },
                        Array.Empty<string>()
                    }
                });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "不需要 添加Bearer",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer"
            });

            var xmlFile = $"Qikig.App.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, cconfig.Xmlname?? xmlFile);
            options.IncludeXmlComments(xmlPath);
        }
    }
}
