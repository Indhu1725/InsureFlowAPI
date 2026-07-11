namespace InsureFlowAPI.DTOs.Authentication
{
    public class LoginResponseDto
    {
        public string JWTToken { get; set; }

        public string TokenType { get; set; } = "Bearer";

        public string UserEmail { get; set; }

        public string UserRole { get; set; }

        public DateTime TokenExpiry { get; set; }
    }
}
