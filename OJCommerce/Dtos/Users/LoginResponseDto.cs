namespace OJCommerce.Dtos.Users
{
    public class LoginResponseDto
    {
        public bool Success { get; set; }           // true if login succeeds
        public string Message { get; set; }         // "Login successful" or error message
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
