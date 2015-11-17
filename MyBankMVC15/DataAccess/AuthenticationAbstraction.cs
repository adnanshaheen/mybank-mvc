namespace MyBankMVC15.Service
{
    public class AuthenticationAbstraction
    {
        IAuthenticationService _authService = null;

        public AuthenticationAbstraction(IAuthenticationService iauthService)
        {
            _authService = iauthService;
        }

        public AuthenticationAbstraction()
            : this(GenericFactory<AuthenticationService, IAuthenticationService>.CreateInstance())
        {

        }

        public string GetRolesForUser(string uname)
        {
            return _authService.GetRolesForUser(uname);
        }

        public bool SignIn(string userName, string password, bool createPersistentCookie)
        {
            return _authService.SignIn(userName, password, createPersistentCookie);
        }

        public bool ChangePassword(string userName, string password, string newPassword)
        {
            return _authService.ChangePassword(userName, password, newPassword);
        }

        public void SignOut()
        {
            _authService.SignOut();
        }

        public bool ValidateUser(string userName, string password)
        {
            return _authService.ValidateUser(userName, password);
        }
    }
}