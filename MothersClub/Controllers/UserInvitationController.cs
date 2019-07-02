using MothersClub.Models;
using MothersClub.Service;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Configuration;
using System.Web.Mvc;

namespace MothersClub.Controllers
{
    public class UserInvitationController : BaseController
    {
        // GET: UserInvitation
        public ActionResult Index()
        {
            if (Session["IsAdmin"] != null && (bool)Session["IsAdmin"] == true)
            {
                return Redirect("/Home/Index");
            }
            else
            {
                if (Session["CurrentUserId"] == null)
                {
                    Response.Redirect("~/Account/Login");
                }
            }
            ViewBag.IsAdmin = Session["IsAdmin"];
            ViewBag.UserName = Session["UserName"];
            return View();
        }

        #region Rest
        [HttpGet]
        public ActionResult RetrieveUserInvitations()
        {
            try
            {
                var userId = Convert.ToInt32(Session["CurrentUserId"]);
                using (MCContext ctx = new MCContext())
                {
                    var invitations = ctx.userInvitations
                                        .Where(x => x.referenceUserId == userId)
                                        .OrderByDescending(x => x.id)
                                        .Select(x => new
                                        {
                                            x.mailAddress,
                                            x.invitationStatus,
                                            x.createdDate
                                        })
                                        .ToList();
                    return Json(invitations, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ActivityService.LogException(ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "İşlem sırasında bir hata oluştu.");
            }
        }
        [HttpPost]
        public ActionResult InviteUser(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Conflict, "E-posta alanı boş olmamalıdır.");
                }
                if (email.Equals(Session["UserName"].ToString(), StringComparison.InvariantCulture))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Conflict, "Kendinize davet gönderemezsiniz.");
                }

                var userId = Convert.ToInt32(Session["CurrentUserId"]);
                using (MCContext ctx = new MCContext())
                {
                    var invitations = ctx.userInvitations
                        .Where(x => x.invitationStatus == Enums.InvitationStatus.delivered &&
                                        x.referenceUserId == userId);
                    var invitation = invitations.FirstOrDefault(x =>
                                        x.mailAddress.Equals(email, StringComparison.InvariantCulture));
                    if (invitation != null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.Conflict, "E-posta adresini girmiş olduğunuz kişiye aktif davetiniz mevcuttur.");
                    }

                    if (WebConfigurationManager.AppSettings["UserInvitiationCount"] != null)
                    {
                        int limit = Convert.ToInt32(WebConfigurationManager.AppSettings["UserInvitiationCount"]);
                        if (invitations.Count() > limit)
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.Conflict, "Daha fazla davet isteği gönderemezsiniz.");
                        }
                    }
                    //todo: return bool IsEmailExists(string email)
                    if (true)
                    {
                        var code = Extensions.RandomString(10);
                        var state = SendMail(email, code);
                        ctx.userInvitations.Add(new UserInvitation()
                        {
                            invitationStatus = state,
                            referenceUserName = Session["UserName"].ToString(),
                            mailAddress = email,
                            referenceUserId = userId,
                            invitationCode = code
                        });
                        ctx.SaveChanges();
                        return Json(new { responseText = "Davet gönderildi." }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.Conflict, "Kullanıcı sistemde zaten kayıtlıdır.");
                    }
                }
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ActivityService.LogException(ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "İşlem sırasında bir hata oluştu.");
            }
        }
        #endregion

        #region Helpers
        private Enums.InvitationStatus SendMail(string email, string code)
        {
            try
            {
                if (WebConfigurationManager.AppSettings["SmtpHost"] == null || WebConfigurationManager.AppSettings["SmtpPort"] == null)
                {
                    return Enums.InvitationStatus.notDelivered;
                }

                using (SmtpClient smtpClient = new SmtpClient())
                {
                    var from = "rehakasuto@gmail.com";
                    var password = "yenireha1990";
                    var basicCredential = new NetworkCredential(from, password);
                    using (MailMessage message = new MailMessage())
                    {
                        MailAddress fromAddress = new MailAddress(from);

                        smtpClient.Host = WebConfigurationManager.AppSettings["SmtpHost"].ToString();
                        smtpClient.UseDefaultCredentials = false;
                        smtpClient.Credentials = basicCredential;
                        smtpClient.Port = Convert.ToInt32(WebConfigurationManager.AppSettings["SmtpPort"]);
                        smtpClient.EnableSsl = false;

                        message.From = fromAddress;
                        message.Subject = "this is a test email.";
                        // Set IsBodyHtml to true means you can send HTML email.
                        message.IsBodyHtml = true;
                        message.Body = "Click <a href='http://www.market.sleepy.com?invitationCode=" + code + "'>here</a> to accept invitation.";
                        message.To.Add(email);
                        smtpClient.Send(message);
                    }
                }
                return Enums.InvitationStatus.delivered;
            }
            catch (Exception ex)
            {
                ActivityService.LogException(ex);
                return Enums.InvitationStatus.notDelivered;
            }
        }
        #endregion
    }
}