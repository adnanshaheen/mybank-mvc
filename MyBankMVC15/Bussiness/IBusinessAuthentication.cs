﻿namespace MyBankMVC15.Business
{
    public interface IBusinessAuthentication
    {
        string GetRolesForUser(string uname);

        bool SignIn(string userName, string password, bool createPersistentCookie);

        bool ChangePassword(string userName, string password, string newPassword);

        void SignOut();

        bool ValidateUser(string userName, string password);
    }
}