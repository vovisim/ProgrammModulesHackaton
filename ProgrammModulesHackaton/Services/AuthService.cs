using ProgrammModulesHackaton.Helpers;
using ProgrammModulesHackaton.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammModulesHackaton.Services
{
    public class AuthService
    {
        private List<User> _users;

        public AuthService(List<User> users)
        {
            _users = users;
        }

        public User Authenticate(string username, string password)
        {
            foreach (var user in _users)
            {
                if (user.Username == username && PasswordHelper.VerifyPassword(password, user.PasswordHash))
                {
                    return user;
                }
            }
            return null;
        }
    }
}
