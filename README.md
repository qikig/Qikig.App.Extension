# Qikig.App.Extension
# Qikig.App.Extension，一键配置web项目
# 开箱即用的web集成

## 配置 redis log sql 写的接口注册，一个引用轻松使用
# 1、在安装 NuGet Qikig.App
# 2、Program 注册
```csharp
var configuration = builder.Configuration;
builder.Services.AddQikigService(configuration);
builder.Host.AddQikigHost(configuration);
```
# 3、复制appsettings.json 修改自己对应的 配置完事。
# 写的接口servie 不需要注册，配置文件配置类名的结尾，自动注册
# 比如 IUserService UserService
# 之前需要 写好多 注册，builder.Services.AddScoped<IUserService, UserService>();
# 现在只需要在配置文件配置 AutofacConfig下
```json
# "ServiceNamespace": "Qikig.Service", //dll 名称
# "ServiceName": "Service",//文件后缀带Service就可以
```
# redis 使用
# 构造注入
# private readonly RedisClient _redisClient;
# _redisClient.Instance //初始化
# 使用
# _redisCache.GetMyKey("test");
# mysql
# 新建创储
# 继承:BaseRepository<表名, int>
# 然后查询 await Where(s=>s.Id==id).ToOneAsync();
