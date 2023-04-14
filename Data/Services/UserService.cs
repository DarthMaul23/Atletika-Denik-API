using System.Runtime.Serialization;
using System.Xml.Schema;
using System.Text.Json;
using Atletika_Denik_API.Data.ViewModels;
using Microsoft.EntityFrameworkCore;
using TrainingDefinition = Atletika_Denik_API.Data.ViewModels.TrainingDefinition;

namespace Atletika_Denik_API.Data.Services
{
    public class UserService
    {
        private Atletika_Denik_API.Data.Models.UserContext _userContext;

        public UserService(Atletika_Denik_API.Data.Models.UserContext context)
        {
            _userContext = context;
        }

        public Models.UserLoginResponse LoginUser(string userName, string userPassword)
        {
            using (var context = _userContext)
            {
                var user = context.Users.FirstOrDefault(u => u.userName == userName && u.userPassword == userPassword);
                if (user != null)
                {
                    return new Models.UserLoginResponse() { loggedin = true, token = Guid.NewGuid(), userName = userName, admin =  user.admin == 1 ? true : false};
                }
                else
                {
                    return new Models.UserLoginResponse() { loggedin = false, token = null, userName = null, admin =  false};
                }
            }
        }

        public Users GetUserByID(int userId)
        {
            return _userContext.Users.FirstOrDefault(x => x.id == userId);
        }

        public void CreateUser(Atletika_Denik_API.Data.Models.UsersCreate user, int trenerId)
        {
            using (var context = _userContext)
            {
                if (context.Users.FirstOrDefault(u => u.userName == user.userName) == null)
                {
                    context.Users.Add(new ViewModels.Users()
                    {
                        id = context.Users.Select(u => u.id).Max() + 1,
                        userName = user.userName,
                        userPassword = user.userPassword,
                        admin = user.admin
                    });
                    context.SaveChanges();
                }
            }
        }

        public void UpdateUser(Atletika_Denik_API.Data.Models.Users _user, int trenerId)
        {
            using (var context = _userContext)
            {
                var user = context.Users.FirstOrDefault(u => u.userName == _user.userName);
                if (user != null)
                {
                    user.userName = _user.userName;
                    user.userPassword = _user.userPassword;
                    context.Entry(user).State = EntityState.Modified;
                    context.Update(user);
                    context.SaveChanges();
                }
            }
        }

        public void DeleteUser(int userId, int trenerId)
        {
            using (var context = _userContext)
            {
                var user = context.Users.FirstOrDefault(u => u.id == userId);
                if (user != null)
                {
                    context.Entry(user).State = EntityState.Deleted;
                    context.Remove(user);
                    context.SaveChanges();
                }
            }
        }

        public List<Info> GetUserList(int trenerId)
        {

            List<Info> userList = new List<Info>();

            using (var context = _userContext)
            {
                var usersData = from user in _userContext.Users
                                join trener in _userContext.Asociace_Trener_Uzivatel on user.id equals trener.user_id
                                join info in _userContext.User_Info on user.id equals info.UserId
                                select new { userId = user.id, userInfo = info.Info };

                foreach (var item in usersData)
                {
                    var _info = JsonSerializer.Deserialize<ViewModels.Info>(item.userInfo);
                    userList.Add(new DataTransformation().GetInfoObject(item.userId, _info));
                }
                return userList;
            }
        }

        public List<Users> GetUsersForTrainer(string trenerName)
        {
            using (var context = _userContext)
            {
                var trener = context.Users.Where(x=> x.userName == trenerName).Select(x=> x.id).ToList().ElementAt(0);
                var users_id = context.Asociace_Trener_Uzivatel.Where(a => a.trener_id == trener)
                    .Select(x => x.user_id).ToList();
                return context.Users.Where(x => users_id.Contains(x.id)).ToList();
            }
        }

        public UserInfo GetUserInfoByID(int id)
        {
            using (var context = _userContext)
            {
                var _userInfo = context.User_Info.FirstOrDefault(x => x.UserId == id);
                var info = JsonSerializer.Deserialize<ViewModels.Info>(_userInfo.Info);
                ViewModels.UserInfo userInfo = new ViewModels.UserInfo()
                {
                    UserId = _userInfo.UserId,
                    Info = new DataTransformation().GetInfoObject(_userInfo.UserId, info)
                };
                return userInfo;
            }
        }

        public void CreateUserInfo(int _userId, Info _userInfo)
        {
            using (var context = _userContext)
            {
                if (context.User_Info.FirstOrDefault(u => u.UserId == _userId) == null)
                {
                    context.User_Info.Add(new User_Info()
                    {
                        UserId = _userId,
                        Info = JsonSerializer.Serialize(_userInfo)
                    });
                    context.SaveChanges();
                }
            }
        }

        public void UpdateUserInfo(int _userId, Info _userInfo)
        {
            using (var context = _userContext)
            {
                var userInfo = _userContext.User_Info.FirstOrDefault(u => u.UserId == _userId);
                userInfo.Info = JsonSerializer.Serialize(_userInfo);
                context.Entry(userInfo).State = EntityState.Modified;
                context.Update(userInfo);
                context.SaveChanges();
            }
        }

        public void DeleteUserInfo(int id)
        {
            using (var context = _userContext)
            {
                var userInfo = _userContext.User_Info.FirstOrDefault(u => u.UserId == id);
                context.Entry(userInfo).State = EntityState.Deleted;
                context.Remove(userInfo);
                context.SaveChanges();
            }
        }
    }
}