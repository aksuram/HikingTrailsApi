namespace HikingTrailsApi.Application.Users
{
    public class UserRegistrationDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string RepeatPassword { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
