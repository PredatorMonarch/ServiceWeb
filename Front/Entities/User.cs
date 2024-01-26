namespace Front.Entities
{
    public class UserUpdateModel(string username, string email, string pass)
    {
        public  string Pass { get; set; } = pass;
        public  string Username { get; set; } = username;
        public  string Email { get; set; } = email;
    }
    public class UserLoginModel(string login, string pass)
    {
        public string Login { get; set; } = login;
        public string Pass { get; set; } = pass;
    }
    
    public class UserLogin(string login, string pass, bool rememberMe = false)
    {
        public string Login { get; set; } = login;
        public string Pass { get; set; } = pass;
        public bool RememberMe { get; set; } = rememberMe;
    }
    public class UserDTO
    {
        public string Id { get; set; } 
        public string Username { get; set; } 
        public string Email { get; set; }
    }

    public class JWTAndUser
    {
        public required string Token { get; set; }
        public required UserDTO User { get; set; }
    }
}
