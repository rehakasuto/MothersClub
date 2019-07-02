using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UserReferenceModule.Models;

namespace MothersClub.Models
{
    public class Campaign : Base
    {
        [MaxLength(50)]
        public string name { get; set; }

        [Required, DefaultValue(false)]
        public bool isActive { get; set; }

        [InverseProperty("campaign")]
        public ICollection<CampaignRule> rules { get; set; }

        [InverseProperty("campaign")]
        public ICollection<UserReference> references { get; set; }
    }
}