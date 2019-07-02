using System.ComponentModel.DataAnnotations;

namespace MothersClub.Models.ViewModel
{
    public class CampaignRuleViewModel
    {
        public int campaignId { get; set; }
        public int campaignRuleId { get; set; }
        [Required(ErrorMessage = "Lütfen kural adı giriniz."), MaxLength(50)]
        public string name { get; set; }
        [MaxLength(100)]
        public string description { get; set; }
        public Enums.RuleTypes ruleType { get; set; }

        [Required(ErrorMessage = "Lütfen en az kaç lira olacağını giriniz.")]
        public decimal atLeastPrice { get; set; }

        [Required(ErrorMessage = "Lütfen tutar giriniz.")]
        public int count { get; set; }
        [Required(ErrorMessage = "Lütfen sıra numarası giriniz.")]
        public int index { get; set; }

        [Required(ErrorMessage = "Lütfen kaç günde aktif olacağı bilgisini giriniz.")]
        public int activationDay { get; set; }
    }
}