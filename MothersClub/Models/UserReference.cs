using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UserReferenceModule.Models;

namespace MothersClub.Models
{
    public class UserReference : Base
    {
        [Required]
        public int referenceUserId { get; set; }

        [Required]
        public int userId { get; set; }

        [Required]
        public Enums.ReferenceStatus status { get; set; }

        [Required, DefaultValue(0)]
        public decimal totalDiscount { get; set; }

        [ForeignKey("campaign")]
        public int campaignId { get; set; }

        public Campaign campaign { get; set; }

        [ForeignKey("userInvitation")]
        public int userInvitationId { get; set; }

        public UserInvitation userInvitation { get; set; }


        [InverseProperty("userReference")]
        public ICollection<UserReferenceOrder> orders { get; set; }
    }
}