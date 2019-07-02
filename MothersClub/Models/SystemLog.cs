using System.ComponentModel.DataAnnotations;
using UserReferenceModule.Models;

namespace MothersClub.Models
{
    public class SystemLog : Base
    {
        [MaxLength(1000), Required]
        public string logMessage { get; set; }
        public int? userId { get; set; }
        [MaxLength(200)]
        public string userName { get; set; }
        [MaxLength(100)]
        public string requestIpAddress { get; set; }
    }
}