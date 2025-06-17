using FreeSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qikig.Service
{
    /// <summary>
    /// 读取数据库
    /// </summary>
    public class UserData: BaseRepository<UserEntity, int>, IUserData
    {
        public UserData(IFreeSql fsql) : base(fsql)
        {
        }

        public async Task<string> GetUserName(int id)
        {
            var userinfo = await Where(s => s.Id == id).ToOneAsync();
            // Simulate fetching user name from a database or other data source
            return $"User{id}";
        }
    }
}
