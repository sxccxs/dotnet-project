namespace Core.Settings
{
    public class AppSettings
    {
        public string SecretKey { get; set; }

        public int JwtExpirationTime { get; set; } // days

        public int TokensExpirationTime { get; set; } // days

        public string Domain { get; set; }

        public string AccountActivationEmailTemplate { get; set; }

        public string EmailChangeEmailTemplate { get; set; }

        public string RecordMessagesPath { get; set; }
    }
}
