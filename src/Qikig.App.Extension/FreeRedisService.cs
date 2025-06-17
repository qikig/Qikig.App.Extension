using Newtonsoft.Json;
using FreeRedis;

namespace Qikig.App.Extension
{
    /// <summary>
    /// FreeRedis Redis 帮助类
    /// </summary>
    public class FreeRedisService
    {
        
        private static RedisClient db;
        private static readonly object locker = new object();
        ConnectionStringBuilder _redisOptions;
        /// <summary>
        ///     懒加载Redis客户端
        /// </summary>
        private static readonly Lazy<RedisClient> redisClientLazy = new(() =>
        {
            var r = db;
            r.Serialize = obj => JsonConvert.SerializeObject(obj);
            r.Deserialize = (json, type) => JsonConvert.DeserializeObject(json, type);
            // r.Notice += (s,e) => Console.WriteLine(e.Log);
            return r;
        });
        /// <summary>
        ///     获取Client实例
        /// </summary>
        public RedisClient Instance
        {
            get
            {
                if (InitRedisClient(_redisOptions)) return redisClientLazy.Value;

                throw new NullReferenceException("Redis不可用");
            }
        }
        /// <summary>
        /// 初始化配置
        /// </summary>
        private FreeRedisOption _freeOption;
        /// <summary>
        ///     初始化Redis
        /// </summary>
        /// <returns></returns>
        private bool InitRedisClient(ConnectionStringBuilder redisOptions)
        {
            _freeOption = new FreeRedisOption();
            _freeOption.UseClientSideCache = false;
            _redisOptions = redisOptions;
            if (db == null)
                lock (locker)
                {
                    if (db == null)
                    {
                        db = new RedisClient(_redisOptions);
                        //设置客户端缓存
                        if (_freeOption.UseClientSideCache)
                            db.UseClientSideCaching(new ClientSideCachingOptions
                            {
                                Capacity = 0, //本地缓存的容量，0不限制
                                KeyFilter = o => o.StartsWith("ClientCache:"), //过滤哪些键能被本地缓存
                                CheckExpired = (key, dt) =>
                                    DateTime.Now.Subtract(dt) > TimeSpan.FromSeconds(86400) //检查长期未使用的缓存
                            });
                    }
                }

            return true;
        }
        // private static IDatabase db = null;
        public FreeRedisService(ConnectionStringBuilder redisOptions)
        {
            InitRedisClient(redisOptions);
        }
    }

    public class FreeRedisOption
    {
        public bool UseClientSideCache { get; set; } = false;
    }
}
