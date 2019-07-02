using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UserReferenceModule.Models;

namespace MothersClub.Models
{
    public class CampaignRule : Base
    {
        [ForeignKey("campaign")]
        public int campaignId { get; set; }
        public Campaign campaign { get; set; }

        [Required, MaxLength(50)]
        public string name { get; set; }

        [MaxLength(100)]
        public string description { get; set; }

        public Enums.RuleTypes ruleType { get; set; }

        [Required, DefaultValue(0)]
        public decimal atLeastPrice { get; set; }

        [Required, DefaultValue(0)]
        public int count { get; set; }

        public int index { get; set; }

        [Required, DefaultValue(15)]
        public int activationDay { get; set; }
    }
}