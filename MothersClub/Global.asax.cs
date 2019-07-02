using MothersClub.Models;
using MothersClub.Service;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MothersClub
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            InitService.InitDefaultCampaign();
        }
        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();
            if (exception == null) return;
            ActivityService.LogException(exception);
        }

        protected void Begin_Request()
        {
            #region Input Validation against XSS attacks
            //Validate only post methods with no form data. Because if form data available MVC validate it by itself. We exclude file uploads and validate only some specific controllers.
            if (Request.HttpMethod == "POST" && Request.Form.Count == 0 && Request.InputStream != null && Request.InputStream.Length > 0 && Request.Files != null && Request.Files.Count == 0 && !string.IsNullOrEmpty(Request.RawUrl))
            {
                string controller = string.Empty;
                var urlParts = Request.RawUrl.Split('/');
                if (urlParts.Length > 1)
                {
                    controller = urlParts[1];
                }
                Stream req = Request.InputStream;
                req.Seek(0, SeekOrigin.Begin);
                string json = new StreamReader(req).ReadToEnd();
                req.Position = 0;
                Newtonsoft.Json.Linq.JObject input = JsonConvert.DeserializeObject<dynamic>(json);
                if (input != null && input.HasValues)
                {
                    foreach (var item in input.Children())
                    {
                        var value = ((Newtonsoft.Json.Linq.JProperty)item).Value?.ToString() ?? string.Empty;
                        if (!string.IsNullOrEmpty(value) && !value.IsInputClear())
                        {

                            string message = @"{
                                                       ""status"":
                                                           {
                                                               ""code"": 500,
                                                               ""errorMessage"": ""A potentially dangerous value was detected from the client.""
                                                           },
                                                       ""body"": """"
                                                       }";

                            Context.Response.StatusCode = 500;
                            Context.Response.Write(message);
                            Context.Response.End();
                        }
                    }
                }
            }
            #endregion
        }
        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            try
            {
                var requestIpAddress = Request.ServerVariables["REMOTE_ADDR"].ToString();
                if (Session["CurrentUserId"] != null && Session["UserName"] != null)
                {
                    ActivityService.Log(HttpContext.Current.Request.RawUrl + " metodu çağırıldı.", requestIpAddress, (int?)Session["CurrentUserId"], Session["userName"].ToString());
                }
                if (HttpContext.Current.Session != null && HttpContext.Current.Session.SessionID != null && Request.ServerVariables["REMOTE_ADDR"] != null && HttpContext.Current.Session["RequestIp"] != null)
                {
                    var sessionIpAddress = HttpContext.Current.Session["RequestIp"].ToString();

                    if (!string.IsNullOrEmpty(requestIpAddress) && !string.IsNullOrEmpty(sessionIpAddress) && !sessionIpAddress.Equals(requestIpAddress, StringComparison.InvariantCulture))
                    {
                        ActivityService.Log("Session hijacking attempt", requestIpAddress);
                        Session.Clear();
                        Session.RemoveAll();
                        Session.Abandon();
                        if (HttpContext.Current.Response.Cookies["ASP.NET_SessionId"] != null)
                        {
                            HttpContext.Current.Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddDays(-1d);
                        }

                        return;
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
