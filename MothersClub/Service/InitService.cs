using MothersClub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace MothersClub.Service
{
    public static class InitService
    {
        public static void InitDefaultCampaign()
        {
            try
            {
                if (WebConfigurationManager.AppSettings["EnableInitService"] != null && !Convert.ToBoolean(WebConfigurationManager.AppSettings["EnableInitService"]))
                {
                    return;
                }
                using (MCContext ctx = new MCContext())
                {
                    var isCampaignExists = ctx.campaigns.FirstOrDefault(x => x.isActive && x.rules.Count > 0) != null;
                    if (!isCampaignExists)
                    {
                        var newCampaign = ctx.campaigns.Add(new Campaign()
                        {
                            isActive = true,
                            name = "Referans Üyelere Yüzdelik İndirim Kampanyası"
                        });
                        var percentages = new int[] { 10, 8, 6, 4, 2 };
                        var index = 1;
                        foreach (var percentage in percentages)
                        {
                            ctx.campaignRules.Add(new CampaignRule()
                            {
                                atLeastPrice = 0,
                                activationDay = 15,
                                count = percentage,
                                description = "Referans yolu ile üye olan kullanıcının " + index + ". alışverişinde yüzde " + percentage + " indirim.",
                                index = index,
                                ruleType = Enums.RuleTypes.percentage,
                                name = "Yüzde " + percentage + " indirim",
                                campaign = newCampaign
                            });
                            index++;
                        }
                        ctx.SaveChanges();
                        ActivityService.Log("InitDefaultCampaign çağırıldı.");
                    }
                }
            }
            catch (Exception ex)
            {
                ActivityService.LogException(ex);
            }

        }
    }
}