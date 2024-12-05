namespace StreamMaster.Infrastructure.Authentication
{
    public class LoginResource
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string RememberMe { get; set; } = string.Empty;
    }
}
