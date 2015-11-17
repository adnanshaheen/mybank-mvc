using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBankMVC15.Models
{
    public interface IMyAuthenticationService
    {
        bool ValidateUser(string userName, string password);
        string GetRolesForUser(string uname);
        bool SignIn(string userName, string password, bool createPersistentCookie);
        bool ChangePassword(string userName, string password, string newPassword);
        void SignOut();
    }
}
