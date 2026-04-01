namespace API.DTOs
{
    public class UserDto // returned after register / login
    {
        public string Username { get; set; }   
        public string Token { get; set; }   
        public string PhotoUrl { get; set; } // main photo
    }
}