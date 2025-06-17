namespace Qikig.App.Extension.AppConfig
{
    public partial class AppConfigs
    {
        public ConnectionStrings ConnectionStrings { get; set; } = new ConnectionStrings();
        public RedisConfig Redis { get; set; } = new RedisConfig();
        public IdGenerate IdGenerate { get; set; } = new IdGenerate();
        public TokenPotions TokenPotions { get; set; } = new TokenPotions();
        public LogConfig LogConfig { get; set; } = new LogConfig();
        public SwaggerConfig SwaggerConfig { get; set; } = new SwaggerConfig();
        public AutofacConfig AutofacConfig { get; set; } = new AutofacConfig();
    }
    public class ConnectionStrings
    {
        public string SqlConnection { get; set; } = string.Empty;
        public string providerName { get; set; } = string.Empty;
    }
    public class RedisConfig
    {
        public string Configuration { get; set; } = string.Empty;
        public string InstanceName { get; set; } = string.Empty;
        public int Database { get; set; } = 0;
        public string RedisPass { get; set; } = string.Empty;
        public int RedisTimeout { get; set; }
    }
    public class IdGenerate
    { 
        public int WorkerId { get; set; }
        public int WorkerIdBitLength { get; set; }

    }
    public class AutofacConfig
    { 
        public string ServiceName { get; set; }
        public string RepositoryName { get; set; }
        public string ServiceNamespace { get; set; }
        public string RepositoryNamespace { get; set; }

        
    }
    public class LogConfig
    {
        public bool WriteMysql { get; set; }
        public string MysqlTableName { get; set; }
        public string LogName { get; set; }
        

    }
    public class SwaggerConfig
    {
        public string Xmlname { get; set; }

    }
    public class TokenPotions
    {

        /// <summary>
        /// 密钥
        /// </summary>
        public string? SecretKey { get; set; }

        /// <summary>
        /// 发行人
        /// </summary>
        public string? Issuer { get; set; }

        /// <summary>
        /// 拥护者
        /// </summary>
        public string? Audience { get; set; }

        /// <summary>
        /// 过期时间分钟
        /// </summary>
        public int ExpireMinutes { get; set; }
    }
}
