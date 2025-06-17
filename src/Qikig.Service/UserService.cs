namespace Qikig.Service
{
    public class UserService: IUserService
    {
        private readonly IUserData _userData;
        public UserService(IUserData userData)
        {
            _userData = userData;
        }
        public string GetUserName(int id)
        {
           var info= _userData.GetUserName(id);
            return "User" + id.ToString();
        }
    }
}
