using ThomsonReuters.Eikon.Toolkit.Interfaces;

namespace ThomsonReuters.Eikon.SystemTestApp
{
    internal class Security
    {
        internal bool AllowedInAtAll { get; private set; }
        //internal string Email { get; private set; }

        internal Security(IAppServerServices services)
        {
            AllowedInAtAll = false;
            var uuid = services.UserContext.UUID;

            // Only allow user who had uuid
            if (!string.IsNullOrWhiteSpace(uuid))
            {
                AllowedInAtAll = true;
            }
        }

        /*static private string GetUserEmail(IAppServerServices services)
        {
            // to be changed to use UIS
            string email = string.Empty;
            //string uuid = services.UserContext.UUID;
            try
            {
                // This can throw an exception if there are problems with the AAA service or it's address in our config files
                email = services.UserContext.EmailAddress;
            }
            catch (Exception)
            {
                services.Logger.LogError("Failed getting user's email address from AAA");
            }

            return email;
        }*/
    }
}
