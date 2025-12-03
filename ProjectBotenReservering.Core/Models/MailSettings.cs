using System.ComponentModel.DataAnnotations;
namespace ProjectBotenReservering.Core.Models
{
    public class MailSettings
    {
        [Required]
        public string Server { get; set; }
        [Required]
        public int Port { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }

    }
}
