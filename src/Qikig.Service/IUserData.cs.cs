using FreeSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qikig.Service
{
    public interface IUserData : IBaseRepository<UserEntity, int>
    {
        Task<string> GetUserName(int id);
    }
}
