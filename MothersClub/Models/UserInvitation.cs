using System.ComponentModel.DataAnnotations;
using UserReferenceModule.Models;

namespace MothersClub.Models
{
    public class UserInvitation : Base
    {
        [Required, MaxLength(10)]
        public string invitationCode { get; set; }

        [Required]
        public int referenceUserId { get; set; }

        [Required, MaxLength(100)]
        public string referenceUserName { get; set; }

        [Required, MaxLength(100)]
        public string mailAddress { get; set; }

        public Enums.InvitationStatus invitationStatus { get; set; }
    }
}