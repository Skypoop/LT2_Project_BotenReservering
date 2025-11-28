namespace ProjectBotenReservering.Core.Models
{
    public class MailSettings
    {
        public required string Server { get; set; }
        public int Port { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }

    }
}
