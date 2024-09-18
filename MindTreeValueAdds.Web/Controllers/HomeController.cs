using System;
using System.Web;
using System.Xml;
using System.Web.Mvc;
using System.Web.Hosting;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Collections.Generic;
using MindTreeValueAdds.Web.Models;

namespace MindTreeValueAdds.Web.Controllers
{
    public class HomeController : Controller
    {
        private static readonly string listOfValueAddsForHomeScreenFilePath = HostingEnvironment.MapPath("~/App_Data/ListOfValueAddsForHomeScreen.xml");
        private static LoginDtl _loginDtl = LoginDtl.Instance;

        private static NLog.Logger nLogLogger = NLog.LogManager.GetCurrentClassLogger();

        public ActionResult Index()
        {
            //nLogLogger.Info("code from Home Index:" + Session["QueryString_Code"]);
            //nLogLogger.Info("Session [Errpr]" + Session["error"]);
            //nLogLogger.Info("Session [LG_Status]:" + Session["LG_Status"]);
            //nLogLogger.Info("idtoken:" + Convert.ToString(HttpContext.Request.Cookies["id_token"].Value));

            if (Session["error"] != null) { ViewData["error"] = Session["error"]; Session["error"] = null; }

            if (Session["LG_Status"] == null || (Session["LG_Status"] != null && Convert.ToString(Session["LG_Status"]) != "OK"))
            {
				if (string.IsNullOrEmpty(Convert.ToString(HttpContext.Request.Cookies["id_token"]))) { return RedirectToAction("Index", "Default"); }
				else
				{
					bool isRunningInLocalHost = Request.Url.Host == "localhost";
					if (CheckLoggedUserTimeIsNotExpired(Convert.ToString(HttpContext.Request.Cookies["id_token"].Value), isRunningInLocalHost) == false)
					{
						return RedirectToAction("Logout");
					}
				}				
            }

            int count = 0;
            XmlNodeList ValueAdds;
            XmlDocument xmlDocument = new XmlDocument();

            string AbsoluteUri = Request.Url.AbsoluteUri;
            string Server = ConfigurationManager.AppSettings["Server"].ToLower();
            List<Models.Home.HomeListOfValueAddsSctructure> homeListOfValueAddsSctructures = new List<Models.Home.HomeListOfValueAddsSctructure>();

            xmlDocument.Load(listOfValueAddsForHomeScreenFilePath);

            if (Request.Url.Host == "localhost")
            {
                // Show Under Development Value adds.
                ValueAdds = xmlDocument.SelectNodes("ValueAdds/ValueAdd");
            }
            else
            {
                if (Server == "prod") { ValueAdds = xmlDocument.SelectNodes("ValueAdds/ValueAdd[@prod='1']"); }
                else if (Server == "uat") { ValueAdds = xmlDocument.SelectNodes("ValueAdds/ValueAdd[@uat='1']"); }
                else { ValueAdds = xmlDocument.SelectNodes("ValueAdds/ValueAdd[@dev='1']"); }
            }

            foreach (XmlNode ValueAdd in ValueAdds)
            {
                if (ValueAdd.Attributes["name"].Value != null && ValueAdd.Attributes["category"].Value != null && ValueAdd.Attributes["url"].Value != null)
                {
                    count++;
                    Models.Home.HomeListOfValueAddsSctructure homeListOfValueAddsSctructure = new Models.Home.HomeListOfValueAddsSctructure
                    {
                        Number = count,
                        Name = ValueAdd.Attributes["name"].Value,
                        Category = ValueAdd.Attributes["category"].Value,
                        URL = ValueAdd.Attributes["url"].Value
                    };
                    homeListOfValueAddsSctructures.Add(homeListOfValueAddsSctructure);
                }
            }

            if (count == 0)
            {
                Models.Home.HomeListOfValueAddsSctructure homeListOfValueAddsSctructure = new Models.Home.HomeListOfValueAddsSctructure
                {
                    Number = 0,
                    Name = "Nothing to display.",
                    Category = "NA",
                    URL = "#"
                };
                homeListOfValueAddsSctructures.Add(homeListOfValueAddsSctructure);
            }

            ViewData["homeListOfValueAddsSctructures"] = homeListOfValueAddsSctructures;

            return View();
            //return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            Session["LG_Status"] = null;
            Session["UserID"] = null;

            string logout_endpoint = _loginDtl.GetLogout_Endpoint();
            string base_url = _loginDtl.GetBase_Url();
            string idtoken_hint;

            if (Session["id_token"] != null && !string.IsNullOrEmpty(Convert.ToString(Session["id_token"])))
                idtoken_hint = Convert.ToString(Session["id_token"]);
            else
                idtoken_hint = Convert.ToString(HttpContext.Request.Cookies["id_token"].Value);

            Session.Abandon();
            HttpContext.Response.Cookies.Clear();

            string logout_request = $"{logout_endpoint}?id_token_hint={idtoken_hint}&post_logout_redirect_uri={HttpUtility.UrlEncode(base_url)}";

            return Redirect(logout_request);

        }

        private bool CheckLoggedUserTimeIsNotExpired(string idToken, bool isRunningInLocalHost)
        {
            if (!string.IsNullOrEmpty(idToken))
            {
                SaveIdTokenInfoToSession(idToken);

                if (Session["id_token_Expire"] != null)
                {
                    DateTime TokenExpireDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Convert.ToInt32(Session["id_token_Expire"]));

                    if (TokenExpireDateTime > DateTime.UtcNow || isRunningInLocalHost) return true;
                    else return false;
                }
                else
                    return false;
            }
            else
                return false;
        }
        private void SaveIdTokenInfoToSession(string strIdToken)
        {


            string[] jwtParts = strIdToken.Split('.');

            string decodedHeader = SafeDecodeBase64(jwtParts[0]);
            string decodedPayload = SafeDecodeBase64(jwtParts[1]);

            JObject id_token_obj = new JObject
                {
                    {"decoded_header", decodedHeader},
                    {"decoded_payload", decodedPayload}
                };

            Session["id_token"] = strIdToken;
            Session["id_token_json0"] = Convert.ToString(id_token_obj.GetValue("decoded_header"));
            Session["id_token_json1"] = Convert.ToString(id_token_obj.GetValue("decoded_payload"));
            Session["LG_Status"] = "OK";
            Session["id_token_Expire"] = Convert.ToString(JObject.Parse(decodedPayload).GetValue("exp"));
            Session["LoginUserName"] = Convert.ToString(JObject.Parse(decodedPayload).GetValue("name"));

            //nLogLogger.Info("Payload : " + Convert.ToString(id_token_obj.GetValue("decoded_payload")));
            //nLogLogger.Info("LoginUserName : " + Convert.ToString(JObject.Parse(decodedPayload).GetValue("name")));
        }

        public String SafeDecodeBase64(String str)
        {
            return System.Text.Encoding.UTF8.GetString(
                getPaddedBase64String(str));
        }

        private byte[] getPaddedBase64String(string base64Url)
        {
            string padded = base64Url.Length % 4 == 0 ? base64Url : base64Url + "====".Substring(base64Url.Length % 4);
            string base64 = padded.Replace("_", "/").Replace("-", "+");
            return System.Convert.FromBase64String(base64);
        }

    }
}