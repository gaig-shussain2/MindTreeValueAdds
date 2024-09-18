using MindTreeValueAdds.Web.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MindTreeValueAdds.Web.Controllers
{
    public class ExtractSessionDataController : Controller
    {
        private static NLog.Logger nLogLogger = NLog.LogManager.GetCurrentClassLogger();
        private static LoginDtl _loginDtl = LoginDtl.Instance;
        readonly int iLogger = 0;

        static readonly string Success = "success";
        static readonly string Fail = "fail";

        public ExtractSessionDataController()
        {
            iLogger = Convert.ToInt32(ConfigurationManager.AppSettings["Logger"]);
        }

        public ActionResult Index()
        {
            string id_token = Convert.ToString(HttpContext.Request.Cookies["id_token"].Value);
            bool isRunningInLocalHost = Request.Url.Host == "localhost";

            if (!string.IsNullOrEmpty(id_token))
            {
                if (CheckLoggedUserTimeIsNotExpired(id_token, isRunningInLocalHost) == true)
                {
                    if (Session["GUID"] == null || Convert.ToString(Session["GUID"]) == string.Empty) { Session["GUID"] = Guid.NewGuid().ToString(); }

                    List<SelectListItem> Servers = new List<SelectListItem>();
                    string[] appSettings_Servers = ConfigurationManager.AppSettings["Server"].Split(',');

                    foreach (string Server in appSettings_Servers)
                    {
                        Servers.Add(new SelectListItem
                        {
                            Text = Server.Trim(),
                            Value = Server.Trim()
                        });
                    }

                    ViewBag.Server = Servers;
                    ViewBag.GUID = Convert.ToString(Session["GUID"]);
                    ViewBag.GUID = Guid.NewGuid();
                    return View();
                }
                else { return Redirect(logoutURl()); }
            }
            else { return Redirect(logoutURl()); }
        }

        public JsonResult GenerateReport(string Server, string OriginalSubmissionID, string SubmissionID, string VersionNumber)
        {
            if (!string.IsNullOrEmpty(Server) && !string.IsNullOrEmpty(OriginalSubmissionID) && !string.IsNullOrEmpty(SubmissionID) && !string.IsNullOrEmpty(VersionNumber))
            {
                nLogLogger.Info("--------------------------------------------------------------" + Environment.NewLine);
                nLogLogger.Info(" " + Session["LoginUserName"] + " " + " Converting DCXML into Excel Sheet process started: " + Server + ", " + OriginalSubmissionID + ", " + SubmissionID + ", " + VersionNumber);

                string DCTServerURL = Common.ReturnValidDUCKServerURL(Server);

                Tuple<string, string> GenerateReportRs = ExtractSessionData.UserDefineFunctions.GenerateReport(Server, OriginalSubmissionID, SubmissionID, VersionNumber, DCTServerURL);

                if (GenerateReportRs.Item1 != Success)
                {
                    nLogLogger.Info(" " + Session["LoginUserName"] + " " + " Request failed. Input Details : " + Server + ", " + OriginalSubmissionID + ", " + SubmissionID + ", " + VersionNumber);
                    Json(GenerateReportRs.Item2, JsonRequestBehavior.AllowGet);
                }

                nLogLogger.Info(" " + Session["LoginUserName"] + " " + " Request complited. Input Details : " + Server + ", " + OriginalSubmissionID + ", " + SubmissionID + ", " + VersionNumber);
                return Json(GenerateReportRs.Item2, JsonRequestBehavior.AllowGet);
            }
            else { return Json("Oops. Invalid/Insufficient Parametrs.", JsonRequestBehavior.AllowGet); }
        }

        public ActionResult Download(string FileName)
        {
            string FileNotFoundMessage = "Oops. File not found.";

            if (!string.IsNullOrEmpty(FileName))
            {
                if (System.IO.File.Exists(FileName)) { return File(FileName, "application/xml", "Final Output.xml"); }
                else { return Content(FileNotFoundMessage); }
            }
            else { return Content(FileNotFoundMessage); }

        }

        private bool CheckLoggedUserTimeIsNotExpired(string idToken, bool isRunningInLocalHost)
        {
            if (!string.IsNullOrEmpty(idToken))
            {
                SaveIdTokenInfoToSession(idToken);

                if (Session["id_token_Expire"] != null)
                {
                    DateTime TokenExpireDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Convert.ToInt32(Session["id_token_Expire"]));

                    if (TokenExpireDateTime > DateTime.UtcNow || isRunningInLocalHost)
                        return true;
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

            JObject id_token_obj = new JObject
                {
                    {"decoded_header", decodedHeader},
                    {"decoded_payload", decodedPayload}
                };

            Session["id_token"] = strIdToken;
            Session["id_token_json0"] = id_token_obj.GetValue("decoded_header").ToString();
            Session["id_token_json1"] = id_token_obj.GetValue("decoded_payload").ToString();
            Session["LG_Status"] = "OK";
            Session["id_token_Expire"] = JObject.Parse(decodedPayload).GetValue("exp").ToString();
            Session["LoginUserName"] = JObject.Parse(decodedPayload).GetValue("name").ToString();
            LoginDtl.Instance.idToken = strIdToken;
            //DateTime ExpireDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds((int)JObject.Parse(decodedPayload).GetValue("exp"));
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

        private string logoutURl()
        {
            string logout_endpoint = _loginDtl.GetLogout_Endpoint();
            string idtoken_hint = _loginDtl.idToken;
            string base_url = _loginDtl.GetBase_Url();

            Session.Abandon();
            string logout_request = $"{logout_endpoint}?id_token_hint={idtoken_hint}&post_logout_redirect_uri={HttpUtility.UrlEncode(base_url)}";
            return logout_request;
        }
    }
}