namespace MusiKu_API.OTP
{
    public static class OtpMemoryStore
    {
        public static Dictionary<string, (string code, DateTime expiration)> OtpCodes = new();
    }

}
