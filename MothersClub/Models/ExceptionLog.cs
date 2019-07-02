using System.ComponentModel.DataAnnotations;
using UserReferenceModule.Models;

namespace MothersClub.Models
{
    public class ExceptionLog : Base
    {
        [MaxLength(100), Required]
        public string function { get; set; }
        [MaxLength(100), Required]
        public string objectClass { get; set; }
        [Required]
        public string exceptionMessage { get; set; }
    }
}