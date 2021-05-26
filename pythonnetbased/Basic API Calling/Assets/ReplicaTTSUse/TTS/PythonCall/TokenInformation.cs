namespace TokenAuthorization
{
    public class TokenInformation
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }

        public TokenInformation(string access, string refresh)
        {
            access_token = access;
            refresh_token = refresh;
        }
    }
}