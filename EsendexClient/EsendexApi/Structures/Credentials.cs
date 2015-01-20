namespace EsendexApi.Structures
{
    public class Credentials
    {
        public Credentials(string username, string password, string accountReference)
        {
            Username = username;
            Password = password;
            AccountReference = accountReference;
        }

        public string Username { get; private set; }
        public string Password { get; private set; }
        public string AccountReference { get; private set; }
    }
}