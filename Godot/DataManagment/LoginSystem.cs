using System.Collections.Generic;

namespace ArTiX.DataManagment
{
    public static class LoginSystem
    {
        // Use only properties and not fields because JsonSerializer does not serialize fields by default.
        private class LoginInfos
        {
            public Dictionary<string, string> UsernamePasswordDict {  get; set; }
        }

        private const string ACCOUNTS_ALREADY_EXISTS = "An account with this username already exists.";
        private const string MISSING_USERNAME_OR_PASSWORD = "There is no username or password, please add one.";
        private const string WRONG_INFOS_OR_MISSING_ACCOUNT = "Wrong username or password. Try creating an account.";

        private const string ACCOUNT_CREATED = "Account created successfully !";
        private const string CONNEXION_SUCCESSFUL = "Connexion was successful !";

        private const string LOGIN_FILE_PATH = "user://Json/";
        private const string LOGIN_FILE_NAME = "LoginInfos.json";

        /// <summary>
        ///
        /// </summary>
        /// <param name="inputUsername"></param>
        /// <param name="inputPassword"></param>
        /// <param name="outputMessage">Can be used as an error or a validation message.</param>
        /// <returns></returns>
        public static bool Connexion(in string inputUsername, in string inputPassword, out string outputMessage)
        {
            if (!AreInputDatasValid(inputUsername, inputPassword))
            {
                outputMessage = MISSING_USERNAME_OR_PASSWORD;
                return false;
            }

            if (TryGetExistigLoginInfos(out LoginInfos loginInfos))
            {
                if (loginInfos.UsernamePasswordDict.TryGetValue(inputUsername, out string password) && password == inputPassword)
                {
                    outputMessage = CONNEXION_SUCCESSFUL;
                    return true;
                }

                outputMessage = WRONG_INFOS_OR_MISSING_ACCOUNT;
                return false;
            }

            outputMessage = WRONG_INFOS_OR_MISSING_ACCOUNT;
            return false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="inputUsername"></param>
        /// <param name="inputPassword"></param>
        /// <param name="outputMessage">Can be used as an error or a validation message.</param>
        /// <returns></returns>
        public static bool CreateAccount(in string inputUsername, in string inputPassword, out string outputMessage)
        {
            if (AreInputDatasValid(inputUsername, inputPassword))
            {
                if (TryGetExistigLoginInfos(out LoginInfos loginInfos))
                {
                    if (loginInfos.UsernamePasswordDict.ContainsKey(inputUsername))
                    {
                        outputMessage = ACCOUNTS_ALREADY_EXISTS;
                        return false;
                    }

                    loginInfos.UsernamePasswordDict.Add(inputUsername, inputPassword);
                }
                else
                {
                    loginInfos = new LoginInfos
                    {
                        UsernamePasswordDict = new Dictionary<string, string>
                        {
                            { inputUsername, inputPassword }
                        }
                    };
                }

                JsonReader.WriteData(LOGIN_FILE_PATH, LOGIN_FILE_NAME, loginInfos);
                outputMessage = ACCOUNT_CREATED;
                return true;
            }

            outputMessage = MISSING_USERNAME_OR_PASSWORD;
            return false;
        }

        private static bool AreInputDatasValid(in string inputUsername, in string inputPassword) => inputUsername.Length > 0 || inputPassword.Length > 0;

        private static bool TryGetExistigLoginInfos(out LoginInfos loginInfos)
        {
            loginInfos = JsonReader.LoadData<LoginInfos>(LOGIN_FILE_PATH + LOGIN_FILE_NAME);
            return loginInfos != null;
        }
    }
}

