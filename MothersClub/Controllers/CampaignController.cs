using MothersClub.Models;
using MothersClub.Models.ViewModel;
using MothersClub.Service;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MothersClub.Controllers
{
    public class CampaignController : BaseController
    {
        public ActionResult Index()
        {
            if (Session["IsAdmin"] != null && (bool)Session["IsAdmin"] == false)
            {
                return Redirect("/Home/Index");
            }
            ViewBag.IsAdmin = Session["IsAdmin"];
            ViewBag.UserName = Session["UserName"];
            return View();
        }

        [HttpPost]
        public ActionResult UpdateCampaignRule(CampaignRuleViewModel model)
        {
            try
            {
                using (MCContext ctx = new MCContext())
                {
                    var rule = ctx.campaignRules.FirstOrDefault(x => x.id == model.campaignRuleId);
                    if (rule == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Böyle bir veri mevcut değildir.");
                    }
                    var campaignRule = ctx.campaignRules
                                        .FirstOrDefault(x => x.campaignId == model.campaignId &&
                                                        x.campaign.isActive &&
                                                        x.name.Equals(model.name, StringComparison.InvariantCulture) &&
                                                        x.id != model.campaignRuleId);
                    if (campaignRule != null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.Conflict, "Kural sistemde zaten mevcut.");
                    }

                    var enableToUpdate = ctx.userReferenceOrders.FirstOrDefault(x => x.campaignRuleId == model.campaignRuleId && x.userReference.campaign.isActive && x.userReference.status != Enums.ReferenceStatus.passive) == null;
                    if (!enableToUpdate)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.Conflict, "Güncellemek istediğiniz kural aktif bir siparişe atandığı için güncelleyemezsiniz.");
                    }
                    var indexExists = ctx.campaignRules
                                        .FirstOrDefault(x => x.index == model.index &&
                                                        x.campaign.isActive &&
                                                        x.campaignId == model.campaignId &&
                                                        x.id != model.campaignRuleId) != null;
                    if (indexExists)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.Conflict, "Girdiğiniz sırada kural zaten mevcut. Lütfen farklı bir sıra numarası ile tekrar deneyiniz.");
                    }
                    rule.index = model.index;
                    rule.name = model.name;
                    rule.modifiedDate = DateTime.Now;
                    rule.ruleType = model.ruleType;
                    rule.description = model.description;
                    rule.count = model.count;
                    rule.atLeastPrice = model.atLeastPrice;
                    rule.activationDay = model.activationDay;
                    ctx.SaveChanges();
                    return Json(new { responseText = "Kural başarıyla güncellendi." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ActivityService.LogException(ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "İşlem sırasında bir hata oluştu.");
            }
        }

        [HttpPost]
        public ActionResult DeleteCampaignRule(int campaignRuleId)
        {
            try
            {
                using (MCContext ctx = new MCContext())
                {
                    var rule = ctx.campaignRules.FirstOrDefault(x => x.id == campaignRuleId);
                    if (rule == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Böyle bir veri mevcut değildir.");
                    }

                    var enableToDelete = ctx.userReferenceOrders.FirstOrDefault(x => x.campaignRuleId == campaignRuleId && x.userReference.campaign.isActive && x.userReference.status != Enums.ReferenceStatus.passive) == null;
                    if (!enableToDelete)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.Conflict, "Silmek istediğiniz kural aktif bir siparişe atandığı için silemezsiniz.");
                    }
                    ctx.campaignRules.Remove(rule);
                    ctx.SaveChanges();
                    return Json(new { responseText = "Kural başarıyla silindi." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ActivityService.LogException(ex);
                return Json(new { message = "İşlem sırasında bir hata oluştu.", state = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult AddCampaignRule(CampaignRuleViewModel model)
        {
            try
            {
                using (MCContext ctx = new MCContext())
                {
                    var campaignRule = ctx.campaignRules
                                        .FirstOrDefault(x => x.campaignId == model.campaignId &&
                                                        x.campaign.isActive &&
                                                        x.name.Equals(model.name, StringComparison.InvariantCulture));
                    if (campaignRule != null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.Conflict, "Kural sistemde zaten mevcut.");

                    }
                    var indexExists = ctx.campaignRules.FirstOrDefault(x => x.index == model.index && x.campaign.isActive && x.campaignId == model.campaignId) != null;
                    if (indexExists)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.Conflict, "Girdiğiniz sırada kural zaten mevcut. Lütfen farklı bir sıra numarası ile tekrar deneyiniz.");
                    }
                    ctx.campaignRules.Add(new CampaignRule()
                    {
                        activationDay = model.activationDay,
                        atLeastPrice = model.atLeastPrice,
                        count = model.count,
                        description = model.description,
                        index = model.index,
                        name = model.name,
                        ruleType = model.ruleType,
                        campaignId = model.campaignId
                    });
                    ctx.SaveChanges();
                    return Json(new { responseText = "Kural başarıyla kaydedildi." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ActivityService.LogException(ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "İşlem sırasında bir hata oluştu.");
            }
        }

        [HttpGet]
        public ActionResult RetrieveCampaign()
        {
            try
            {
                using (MCContext ctx = new MCContext())
                {
                    var campaign = ctx.campaigns.FirstOrDefault(x => x.isActive);
                    if (campaign != null)
                    {
                        return Json(campaign, JsonRequestBehavior.AllowGet);
                    }
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ActivityService.LogException(ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "İşlem sırasında bir hata oluştu.");
            }
        }
        [HttpGet]
        public ActionResult RetrieveCampaignRules(int campaignId)
        {
            try
            {
                using (MCContext ctx = new MCContext())
                {
                    var campaignRules = ctx.campaignRules
                                            .Where(x => x.campaignId == campaignId)
                                            .Select(x => new
                                            {
                                                x.id,
                                                x.activationDay,
                                                atLeastPrice = x.atLeastPrice + " TL",
                                                x.count,
                                                x.description,
                                                x.index,
                                                x.name,
                                                ruleType = x.ruleType == Enums.RuleTypes.percentage ? "%" : "TL"
                                            })
                                            .OrderBy(x => x.index)
                                            .ToList();
                    if (campaignRules.Any())
                    {
                        return Json(campaignRules, JsonRequestBehavior.AllowGet);
                    }
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ActivityService.LogException(ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "İşlem sırasında bir hata oluştu.");
            }
        }
        [HttpGet]
        public ActionResult RetrieveCampaignRule(int campaignId, int index)
        {
            try
            {
                using (MCContext ctx = new MCContext())
                {
                    var rule = ctx.campaignRules.FirstOrDefault(x => x.campaignId == campaignId && x.campaign.isActive && x.index == index);
                    if (rule == null)
                    {
                        return Json(new { }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(rule, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ActivityService.LogException(ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "İşlem sırasında bir hata oluştu.");
            }
        }

        [HttpGet]
        public JsonResult RetrieveOptions()
        {
            var lst = Enum.GetValues(typeof(Enums.RuleTypes)).Cast<Enums.RuleTypes>().Select(x => new
            {
                value = (int)x,
                text = x.ToString()
            });
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
    }
}