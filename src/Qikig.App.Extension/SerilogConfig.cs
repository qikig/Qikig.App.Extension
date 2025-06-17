using Microsoft.Extensions.DependencyInjection;
using Qikig.App.Extension.AppConfig;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MariaDB;
using Serilog.Sinks.MariaDB.Extensions;
using System.Net;

namespace Qikig.App.Extension
{
    /// <summary>
    /// Serilog for Config
    /// </summary>
    public static class SerilogConfig
    {
        /// <summary>
        /// 注册日志
        /// </summary>
        /// <param name="services"></param>
        /// <param name="appConfigs"></param>
        /// <returns></returns>
        public static IServiceCollection AddLog(this IServiceCollection services, AppConfigs appConfigs)
        {
            services.AddLogging(cfg =>
            {
                ConnectionStrings? connString = appConfigs.ConnectionStrings;
                var logconfig=  appConfigs.LogConfig;

                
                var filePath = Path.Combine(AppContext.BaseDirectory, logconfig.LogName);
                var opt = new MariaDBSinkOptions();
                opt.ExcludePropertiesWithDedicatedColumn = true; // 排除属性专用列
                opt.LogRecordsExpiration = TimeSpan.FromDays(30); // 设置日志记录过期时间
                opt.PropertiesToColumnsMapping = new Dictionary<string, string>
                {

                    { "Level", "level" },
                    { "Message", "message" },
                    { "Exception", "exception" },
                    { "Timestamp", "timestamp" },
                    { "Properties", "properties" },
                    { "IP", "ip" }

                };

                if (logconfig.WriteMysql)
                {
                    Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .WriteTo.Console()
                    .WriteTo.MariaDB(
                        connectionString: connString.SqlConnection,
                        tableName: logconfig.MysqlTableName,
                        autoCreateTable: false,
                         options: opt,
                         restrictedToMinimumLevel: LogEventLevel.Warning)
                     .WriteTo.File(path: filePath, rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: LogEventLevel.Debug, outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
                                            fileSizeLimitBytes: 1000000, rollOnFileSizeLimit: true, retainedFileCountLimit: 31, retainedFileTimeLimit: new TimeSpan(31, 0, 0, 0))
                     .Enrich.FromLogContext()
                     .Enrich.WithProperty("IP", GetIpAddress())
                    .CreateLogger();

                }
                else {
                    Log.Logger = new LoggerConfiguration()
                        .MinimumLevel.Information()
                        .WriteTo.Console()
                         .WriteTo.File(path: filePath, rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: LogEventLevel.Debug, outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
                                                fileSizeLimitBytes: 1000000, rollOnFileSizeLimit: true, retainedFileCountLimit: 31, retainedFileTimeLimit: new TimeSpan(31, 0, 0, 0))
                         .Enrich.FromLogContext()
                         .Enrich.WithProperty("IP", GetIpAddress())
                        .CreateLogger();
                }

                cfg.AddSerilog();
            });
            return services;
        }
        public static string GetIpAddress()
        {
            string ipAddress = "127.0.0.1";
            IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress ip in ips)
            {
                if (ip.AddressFamily.ToString().ToLower().Equals("internetwork"))
                {
                    ipAddress = ip.ToString();
                    return ipAddress;
                }
            }

            return ipAddress;
        }

    }
}
