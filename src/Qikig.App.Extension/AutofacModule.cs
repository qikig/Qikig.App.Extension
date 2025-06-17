using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Qikig.App.Extension.AppConfig;

namespace Qikig.App.Extension
{
    /// <summary>
    /// 自动注册
    /// </summary>
    public static class AutofacConfigure //Autofac配置类
    {
        public static IHostBuilder Register(this IHostBuilder host, IConfiguration configuration)
        {
            var appConfig = configuration.Get<AppConfigs>().AutofacConfig;
            host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>(builder =>
                {
                    if (!string.IsNullOrWhiteSpace(appConfig.ServiceNamespace))
                    {
                        var AppServices = System.Reflection.Assembly.Load(appConfig.ServiceNamespace);
                        builder.RegisterAssemblyTypes(AppServices).Where(t => t.Name.EndsWith(appConfig.ServiceName)).AsImplementedInterfaces().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies)
                        .InstancePerLifetimeScope();
                    }
                    if (!string.IsNullOrWhiteSpace(appConfig.RepositoryNamespace))
                    {
                        var RepServices = System.Reflection.Assembly.Load(appConfig.RepositoryNamespace);
                        builder.RegisterAssemblyTypes(RepServices).Where(t => t.Name.EndsWith(appConfig.ServiceName)).AsImplementedInterfaces().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies)
                        .InstancePerLifetimeScope();
                    }
                    
                });
            return host;
        }
    }
}
