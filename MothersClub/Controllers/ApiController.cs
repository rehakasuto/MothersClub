using MothersClub.Models;
using MothersClub.Models.ViewModel;
using MothersClub.Service;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MothersClub.Controllers
{
    public class ApiController : Controller
    {
        #region CreateUserReference
        [HttpPost]
        public ActionResult CreateUserReference(UserReferenceViewModel data)
        {
            try
            {
                if (string.IsNullOrEmpty(data.invitationCode))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Davet kodu boş olamaz.");
                }

                using (MCContext ctx = new MCContext())
                {
                    var invitation = ctx.userInvitations.FirstOrDefault(x => x.invitationCode.Equals(data.invitationCode) && x.invitationStatus == Enums.InvitationStatus.delivered);
                    if (invitation == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Davet bulunmamaktadır.");
                    }
                    var invitationCodeExists = ctx.userReferences.FirstOrDefault(x => x.userInvitationId == invitation.id) != null;
                    if (invitationCodeExists)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.Conflict, "Davet kodu ile zaten referans kaydı vardır.");
                    }
                    if (data.userId == invitation.referenceUserId)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.Conflict, "Kendine referans olamazsın.");
                    }
                    var activeCampaign = ctx.campaignRules.FirstOrDefault(x => x.campaign.isActive);
                    if (activeCampaign == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Aktif kampanya bulunmamaktadır.");
                    }
                    var userReference = ctx.userReferences.FirstOrDefault(x => x.userId == data.userId);
                    if (userReference != null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.Conflict, "Bu kullanıcı zaten başka bir referansın daveti ile sisteme kayıt edilmiştir");
                    }
                    ctx.userReferences.Add(new UserReference()
                    {
                        campaignId = activeCampaign.id,
                        referenceUserId = invitation.referenceUserId,
                        status = Enums.ReferenceStatus.pending,
                        totalDiscount = 0,
                        userId = data.userId,
                        userInvitationId = invitation.id
                    });
                    ctx.SaveChanges();
                    return new HttpStatusCodeResult(HttpStatusCode.OK, "Referans bağlantısı başarıyla kurulmuştur.");
                }
            }
            catch (Exception ex)
            {
                ActivityService.LogException(ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "İşlem sırasında bir hata oluştu.");
            }
        }
        #endregion

        #region CreateUserReferenceOrder
        [HttpPost]
        public ActionResult CreateUserReferenceOrder(UserReferenceOrderViewModel data)
        {
            try
            {
                using (MCContext ctx = new MCContext())
                {
                    var userReference = ctx.userReferences
                                            .AsQueryable()
                                            .Include(x => x.campaign)
                                            .Include(x => x.campaign.rules)
                                            .Include(x => x.orders)
                                            .FirstOrDefault(x => x.userId == data.userId);
                    if (userReference == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Referans bulunmamaktadır.");
                    }
                    if (userReference.status != Enums.ReferenceStatus.passive)
                    {
                        var activeRule = new CampaignRule();
                        if (userReference.orders.Any())
                        {
                            var usedRules = userReference.orders.Select(x => x.campaignRuleId).ToList();
                            var existingRules = ctx.campaignRules.Where(x => usedRules.Contains(x.id)).ToList();
                            activeRule = userReference.campaign.rules.Except(existingRules).OrderBy(x => x.index).FirstOrDefault();
                        }
                        else
                        {
                            activeRule = userReference.campaign.rules.OrderBy(x => x.index).FirstOrDefault();
                        }
                        if (activeRule == null)
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Kampanya kuralı bulunmamaktadır.");
                        }
                        if (data.orderPrice < activeRule.atLeastPrice)
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.Conflict, "Sipariş tutarı en az harcama tutarının altındadır.");
                        }
                        var deservedDiscount = activeRule.ruleType == Enums.RuleTypes.amount ? activeRule.count : (data.orderPrice * activeRule.count) / 100;

                        if (ctx.userReferenceOrders.FirstOrDefault(x => x.orderId == data.orderId) == null)
                        {
                            ctx.userReferenceOrders.Add(new UserReferenceOrder()
                            {
                                activationDate = data.orderDate.AddDays(activeRule.activationDay),
                                orderId = data.orderId,
                                orderPrice = data.orderPrice,
                                userReferenceId = userReference.id,
                                orderState = Enums.OrderState.paid,
                                deservedDiscount = deservedDiscount,
                                campaignRuleId = activeRule.id
                            });
                            ctx.SaveChanges();
                            return new HttpStatusCodeResult(HttpStatusCode.OK, "Sipariş kaydı oluşturulmuştur.");
                        }
                        else
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.Conflict, "Sipariş kaydı mevcuttur.");
                        }
                    }
                    else
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.Conflict, "Referans bağlantısı aktif olmadığı için sisteme kayıt atılmamaktadır.");
                    }
                }
            }
            catch (Exception ex)
            {
                ActivityService.LogException(ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "İşlem sırasında bir hata oluştu.");
            }
        }
        #endregion

        #region CancelUserReferenceOrder
        [HttpPost]
        public ActionResult CancelUserReferenceOrder(CancelOrderViewModel data)
        {
            try
            {
                using (MCContext ctx = new MCContext())
                {
                    var userReference = ctx.userReferences.FirstOrDefault(x => x.userId == data.userId);
                    if (userReference == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Referans bulunmamaktadır.");
                    }
                    var order = ctx.userReferenceOrders.FirstOrDefault(x => x.orderId == data.orderId);
                    if (order == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Sipariş bulunmamaktadır.");
                    }
                    if (order.activationDate < DateTime.Now)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Sipariş iptal edilemez.");
                    }
                    order.orderState = Enums.OrderState.canceled;
                    ctx.SaveChanges();

                    return new HttpStatusCodeResult(HttpStatusCode.OK, "Sipariş iptal edilmiştir.");
                }
            }
            catch (Exception ex)
            {
                ActivityService.LogException(ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "İşlem sırasında bir hata oluştu.");
            }
        }
        #endregion

        [HttpGet]
        public ActionResult RetrieveUserDeservedAmount(int userId)
        {
            try
            {
                using (MCContext ctx = new MCContext())
                {
                    decimal totalDeservedAmount = 0;
                    var datas = ctx.userReferences.Where(x => x.referenceUserId == userId && x.status == Enums.ReferenceStatus.active).Select(x => x.totalDiscount);
                    foreach (var item in datas)
                    {
                        if (item != 0)
                        {
                            totalDeservedAmount += item;
                        }
                    }
                    return Json(new { totalDeservedAmount }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ActivityService.LogException(ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "İşlem sırasında bir hata oluştu.");
            }
        }
    }
}