using System;
using System.Web;
using System.Web.Mvc;
using MindTreeValueAdds.Web.Models;
using Newtonsoft.Json.Linq;

namespace MindTreeValueAdds.Web.Controllers
{
    public class DefaultController : Controller
    {
        private static NLog.Logger nLogLogger = NLog.LogManager.GetCurrentClassLogger();
        private static LoginDtl _loginDtl = LoginDtl.Instance;

        public ActionResult Index()
        {
            ViewBag.LoginRs = null;
            string responseString = string.Empty;

            bool isRunningInLocalHost = Request.Url.Host == "localhost";

            if (!string.IsNullOrEmpty(Request.QueryString["code"]) || isRunningInLocalHost)
            {
                try
                {
                    ViewData["code"] = Convert.ToString(Request.QueryString["code"]);
                    Session["QueryString_Code"] = Convert.ToString(Request.QueryString["code"]);


                    if (isRunningInLocalHost) { responseString = _loginDtl.GetToken(Convert.ToString(Request.QueryString["code"]), isRunningInLocalHost); }
                    else { responseString = _loginDtl.GetToken(Convert.ToString(Request.QueryString["code"])); }


                    if (CheckLoggedUserTimeIsNotExpired(responseString, isRunningInLocalHost) == false) { return RedirectToAction("Logout", "Home"); }

                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    Session["error"] = ex.Message;
                    nLogLogger.Info("Catch Section : Redirect to Login from Default Index");
                    return Redirect("");
                }
            }
            else
            {
                Session["LG_Status"] = null;
                nLogLogger.Info("Redirect to Login from Default Index");
                return Redirect(_loginDtl.GetLoginReqUrl());
            }
        }

        [HttpPost]
        public ActionResult Index(string UserID, string Password)
        {
			//nLogLogger.Info("This is the call from Default controller with UserID & Password");
            return View();
        }

        private bool CheckLoggedUserTimeIsNotExpired(string strResponse, bool isRunningInLocalHost)
        {
            JObject jsonObj = JObject.Parse(strResponse);

            Session["access_token"] = jsonObj.GetValue("access_token");
            Session["refresh_token"] = jsonObj.GetValue("refresh_token");
            Session["scope"] = jsonObj.GetValue("scope");

            string idToken = Convert.ToString(jsonObj.GetValue("id_token"));

            if (!string.IsNullOrEmpty(idToken))
            {
                SaveIdTokenInfoToSession(idToken);

                if (Session["id_token_Expire"] != null)
                {
                    DateTime TokenExpireDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Convert.ToInt32(Session["id_token_Expire"]));

                    if (TokenExpireDateTime > DateTime.UtcNow || isRunningInLocalHost)
                    {
                        HttpContext.Response.Cookies.Add(new HttpCookie("id_token", Convert.ToString(Session["id_token"]))
                        {
                            HttpOnly = true,
                            Secure = true,
                        });

                        nLogLogger.Info("Session-Payload : " + Session["id_token_json1"]);
                        nLogLogger.Info("Session-LoginUserName : " + Session["LoginUserName"]);
                        nLogLogger.Info("_______________________________________________________________" + Environment.NewLine);

                        LoginDtl.Instance.idToken = Session["id_token"].ToString();
                        return true;
                    }
                    else
                        return false;
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

            JObject id_token_obj = new JObject { { "decoded_header", decodedHeader }, { "decoded_payload", decodedPayload } };

            Session["id_token"] = strIdToken;
            Session["id_token_json0"] = id_token_obj.GetValue("decoded_header").ToString();
            Session["id_token_json1"] = id_token_obj.GetValue("decoded_payload").ToString();
            Session["LG_Status"] = "OK";
            Session["id_token_Expire"] = JObject.Parse(decodedPayload).GetValue("exp").ToString();
            Session["LoginUserName"] = JObject.Parse(decodedPayload).GetValue("name").ToString();
            Session.Timeout = 60;
            DateTime ExpireDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Convert.ToInt32(JObject.Parse(decodedPayload).GetValue("exp")));
            //Session.Timeout = Convert.ToInt32((ExpireDateTime - DateTime.Now).TotalMinutes / 6);

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

        public DateTime ConvertFromUnixTimestamp(int timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }
    }
}