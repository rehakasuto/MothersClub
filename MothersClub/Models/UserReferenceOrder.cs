using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UserReferenceModule.Models;

namespace MothersClub.Models
{
    public class UserReferenceOrder : Base
    {
        [Required]
        public int orderId { get; set; }

        [ForeignKey("userReference")]
        public int userReferenceId { get; set; }
        public UserReference userReference { get; set; }
        [Required]
        public int campaignRuleId { get; set; }

        [Required]
        public decimal orderPrice { get; set; }

        [Required, DefaultValue(0)]
        public decimal deservedDiscount { get; set; }

        public Enums.OrderState orderState { get; set; }

        [Required]
        public DateTime activationDate { get; set; }
    }
}