namespace Appel.SharpTemplate.ViewModels
{
    public class ResetPassword
    {
        public int Id { get; set; }
        public string EmailHash { get; set; }
        public string Password { get; set; }
        public string PasswordConfirmation { get; set; }
    }
}
