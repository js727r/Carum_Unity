using System;

namespace Carum.Util
{
    [Serializable]
    public static class RequestDto
    {
        [Serializable]
        public class User
        {
            public string userId;
            public string password;

            public User(string userId, string password)
            {
                this.userId = userId;
                this.password = password;
            }
        }

        [Serializable]
        public class Token
        {
            public string accessToken;
            public string refreshToken;

            public Token(string accessToken, string refreshToken)
            {
                this.accessToken = accessToken;
                this.refreshToken = refreshToken;
            }
        }
    }
}