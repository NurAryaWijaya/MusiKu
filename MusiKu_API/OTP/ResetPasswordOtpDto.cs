namespace MusiKu_API.OTP
{
    public class ResetPasswordOtpDto
    {
        public string Email { get; set; }
        public string Code { get; set; }
        public string NewPassword { get; set; }
    }
}
