namespace MovieManagementPanel.WebApp.Models
{
    public class UserLoginModel
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public bool IsRememberMe { get; set; }
    }
}
