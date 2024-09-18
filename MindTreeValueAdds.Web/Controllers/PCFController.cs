using System;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using MindTreeValueAdds.Web.Models;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using ClosedXML.Excel;
using System.Xml;

namespace MindTreeValueAdds.Web.Controllers
{
    public class PCFController : Controller
    {
        private static NLog.Logger nLogLogger = NLog.LogManager.GetCurrentClassLogger();
        private static LoginDtl _loginDtl = LoginDtl.Instance;
        readonly string globalPath = HostingEnvironment.MapPath("~/App_Data/PCF");
        readonly string globalPath_Input = HostingEnvironment.MapPath("~/App_Data/PCF/Input");
        readonly string globalPath_Output = HostingEnvironment.MapPath("~/App_Data/PCF/Output");

        readonly string SUCCESS = "success";
        readonly string FAIL = "failure";
        readonly int iLogger = 0;

        public PCFController()
        {
            iLogger = Convert.ToInt32(ConfigurationManager.AppSettings["Logger"]);
        }

        // GET: PCF
        public ActionResult Index()
        {
            string id_token = Convert.ToString(HttpContext.Request.Cookies["id_token"].Value);
            bool isRunningInLocalHost = Request.Url.Host == "localhost";

            if (!string.IsNullOrEmpty(id_token))
            {
                if (CheckLoggedUserTimeIsNotExpired(id_token, isRunningInLocalHost) == true)
                {
                    if (Session["GUID"] == null || Convert.ToString(Session["GUID"]) == string.Empty)
                        Session["GUID"] = Guid.NewGuid().ToString();

                    //bool CUBE_DB_Status = Common.CheckOracleDBConnection(ConfigurationManager.ConnectionStrings["CUBEDB"].ConnectionString);
                    string[] appSettings_Servers = ConfigurationManager.AppSettings["Server"].Split(',');

                    List<SelectListItem> Servers = new List<SelectListItem>();
                    foreach (string Server in appSettings_Servers)
                    {
                        if (Server.Trim() == "Prod")
                        {
                            Servers.Add(new SelectListItem
                            {
                                Text = Server.Trim(),
                                Value = Server.Trim()
                            });
                        }
                    }

                    // Delete all output folders created in past.
                    //foreach (var directory in Directory.GetDirectories(globalPath_Output)) { if (directory != globalPath_Output + DateTime.Today.ToString("yyyyMMdd")) { Directory.Delete(directory, true); } }

                    // Delete OLD Folders.
                    ViewBag.Server = Servers;
                    ViewBag.GUID = Convert.ToString(Session["GUID"]);
                    ViewBag.GUID = Guid.NewGuid();
                    return View();
                }
                else
                {
                    return Redirect(logoutURl());
                }
            }
            else
                return Redirect(logoutURl());
        }

        public JsonResult GneeratePCF()
        {
            string[] validFileTypes = { ".xls", ".xlsx" };
            DataSet results = new DataSet();
            string Message = "Oops. Something went wrong.";
            string processStartTime = DateTime.Now.ToString("hh:mm:ss:fff:tt");

            try
            {
                if (Request.Files.Count > 0 && Request.Form["Server"] != null)
                {
                    string Server = Request.Form["Server"];
                    bool TriggerRqInTool = Convert.ToBoolean(Request.Form["TriggerRqInTool"]);

                    HttpFileCollectionBase files = Request.Files;

                    nLogLogger.Info("*******************************************************************" + Environment.NewLine);
                    nLogLogger.Info(" " + Session["LoginUserName"] + " PDF file generation started at " + processStartTime + ".");

                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        string path = string.Empty;

                        // Checking for Internet Explorer  
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            path = testfiles[testfiles.Length - 1];
                        }
                        else { path = file.FileName; }

                        // validate file extension
                        string extension = Path.GetExtension(path).ToLower();

                        if (validFileTypes.Contains(extension))
                        {
                            path = Path.Combine(globalPath_Input, path);
                            string connString = string.Empty;
                            file.SaveAs(path);

                            if (extension.Trim() == ".xls") { connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\""; }
                            else if (extension.Trim() == ".xlsx") { connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\""; }

                            // Read Excel.
                            DataTable dtUplodedData = new DataTable();
                            try { dtUplodedData = Common.ConvertXSLXtoDataTable(path, connString, "Page 1"); }
                            catch { return null; }

                            if (dtUplodedData.Rows.Count > 0)
                            {
                                int SLNO = 1;
                                DataTable finalTable_Data = new DataTable { TableName = "Sheet 1" };
                                List<string> Columns = new List<string> { "SL NO", "Incident No", "Policy Number", "Subsmission ID", "EAM URL", "Status" };
                                foreach (string column in Columns) { finalTable_Data.Columns.Add(column, typeof(string)); }

                                DataTable finalTable_RqAndRs = new DataTable { TableName = "Dev - Data" };
                                List<string> Columns2 = new List<string> { "SL NO", "Incident No", "BU Code", "Policy Number", "Original Submission ID", "Subsmission ID", "Transaction Type", "Version", "Azure File Name", "Request", "Responce", "RS-Status" };
                                foreach (string column in Columns2) { finalTable_RqAndRs.Columns.Add(column, typeof(string)); }

                                foreach (DataRow row in dtUplodedData.Rows)
                                {
                                    try
                                    {
                                        var ItemArray = row.ItemArray.Select(x => x.ToString().Trim()).ToArray();
                                        string incident = ItemArray[0];
                                        string shortDescription = ItemArray[1];
                                        string Description = ItemArray[2];

                                        if (Description.StartsWith("strMsgTitle") || (Description.Contains("Rejected Code: WS_INVOCATION_ERROR") && Description.Contains("Step: update ud ws ")))
                                        {
                                            string submission = string.Empty; string BU = string.Empty;

                                            BU = PCF.UserDefineFunctions.GetBUFromShortDescription(shortDescription);

                                            if (Description.StartsWith("strMsgTitle")) { submission = Description.Split('\n')[4].Split(':')[1].Trim(); }
                                            else { submission = Description.Split('\n')[7].Split(':')[1].Trim(); }


                                            if (!string.IsNullOrEmpty(submission))
                                            {
                                                string DCTServerURL = Common.ReturnValidDUCKServerURL(Server);
                                                var relatedSubmissionData = PCF.UserDefineFunctions.GetRelatedSubmissionData(submission, DCTServerURL, Server);

                                                XmlDocument PCFRs = new XmlDocument();
                                                string policyNumber = string.Empty; string OriginalSubmissionID = string.Empty;
                                                string TransactionType = string.Empty; string VersionNumber = string.Empty;
                                                string AzureFileName = string.Empty; string PCFRs_Status = string.Empty;

                                                if (!string.IsNullOrEmpty(relatedSubmissionData["policyNumber"]) &&
                                                    !string.IsNullOrEmpty(relatedSubmissionData["policyID"]) &&
                                                    !string.IsNullOrEmpty(relatedSubmissionData["OriginalSubmissionID"]) &&
                                                    !string.IsNullOrEmpty(relatedSubmissionData["TransactionType"]) &&
                                                    !string.IsNullOrEmpty(relatedSubmissionData["VersionNumber"]) &&
                                                    !string.IsNullOrEmpty(relatedSubmissionData["AzureFileName"]))
                                                {
                                                    policyNumber = relatedSubmissionData["policyNumber"];
                                                    OriginalSubmissionID = relatedSubmissionData["OriginalSubmissionID"];
                                                    TransactionType = relatedSubmissionData["TransactionType"];
                                                    VersionNumber = relatedSubmissionData["VersionNumber"];
                                                    AzureFileName = relatedSubmissionData["AzureFileName"];

                                                    string PCFRq = Common.BuildScriptToTriggerAttachPolicyFormRuleInDuck(OriginalSubmissionID, submission, VersionNumber, TransactionType);

                                                    if (!string.IsNullOrEmpty(PCFRq))
                                                    {
                                                        if (TriggerRqInTool)
                                                        {
                                                            PCFRs = Common.PostDCTRequestReturnResponse(PCFRq, DCTServerURL);
                                                            PCFRs_Status = PCFRs.SelectSingleNode("/server/responses/CustomServer.processRs/@status").InnerText;
                                                        }

                                                        finalTable_Data.Rows.Add(SLNO, incident, policyNumber, submission, "", PCFRs_Status);
                                                        finalTable_RqAndRs.Rows.Add(SLNO, incident, BU, policyNumber, OriginalSubmissionID, submission, TransactionType, VersionNumber, AzureFileName, PCFRq, PCFRs.OuterXml, PCFRs_Status);
                                                    }
                                                    else { finalTable_Data.Rows.Add(SLNO, incident, policyNumber, submission, "", FAIL + ": Unable to generate PCF request."); }
                                                }
                                                else { finalTable_Data.Rows.Add(SLNO, incident, policyNumber, submission, "", FAIL + ": Unable to fetch all mandatory policy details."); }
                                            }
                                            else { finalTable_Data.Rows.Add(SLNO, incident, "", submission, "", FAIL + ": Unable to fetch BU or Submission info from ticket description."); }
                                        }
                                        else { finalTable_Data.Rows.Add(SLNO, incident, "", "", "", FAIL + ": Invalid ticket description."); }

                                    }
                                    catch (Exception ex)
                                    {
                                        Message = "Exception: " + ex.Message;
                                        nLogLogger.Info(" " + Session["LoginUserName"] + " Exception oucred at " + processStartTime + " for submission Sl No: " + Convert.ToString(SLNO) + ", Exceltion message: " + ex.Message);
                                        nLogLogger.Info("*******************************************************************" + Environment.NewLine);
                                        Logger.logException(ex, "Exception oucred for " + Session["LoginUserName"] + " at " + processStartTime);
                                    }

                                    SLNO++;
                                }

                                if (finalTable_Data.Rows.Count > 0) { results.Tables.AddRange(new DataTable[] { finalTable_Data, finalTable_RqAndRs }); }
                                else { Message = "Unable to buil final data."; }
                            }
                            else { Message = "Unable to read uploaded data."; }
                        }
                        else { Message = "Invalid file type."; }
                    }
                }
                else { Message = "Missing mandatory data."; }
            }
            catch (Exception ex)
            {
                Message = "Exception: " + ex.Message;
                nLogLogger.Info(" " + Session["LoginUserName"] + " Exception oucred at " + processStartTime + ". " + ex.Message);
                nLogLogger.Info("*******************************************************************" + Environment.NewLine);
                Logger.logException(ex, "Exception oucred for " + Session["LoginUserName"] + " at " + processStartTime);
            }

            if (results.Tables.Count > 0)
            {
                string filePath = SaveFinalOutput(results);
                if (!string.IsNullOrEmpty(filePath))
                {
                    nLogLogger.Info(" " + Session["LoginUserName"] + " PCF file generation is completed successfully");
                    nLogLogger.Info("*******************************************************************" + Environment.NewLine);
                    return Json(SUCCESS + "==" + filePath, JsonRequestBehavior.AllowGet);
                }
            }
            else { Message = "Oops. No output to print."; }

            return Json(Message);
        }

        public JsonResult GetPCFData(string Submission, string Server)
        {
            string result = string.Empty;

            if (!string.IsNullOrEmpty(Submission) && !string.IsNullOrEmpty(Server))
            {
                var relatedSubmissionData = PCF.UserDefineFunctions.GetRelatedSubmissionData(Submission, Common.ReturnValidDUCKServerURL(Server), Server);
                if (!string.IsNullOrEmpty(relatedSubmissionData["policyNumber"]) &&
                    !string.IsNullOrEmpty(relatedSubmissionData["policyID"]) &&
                    !string.IsNullOrEmpty(relatedSubmissionData["OriginalSubmissionID"]) &&
                    !string.IsNullOrEmpty(relatedSubmissionData["TransactionType"]) &&
                    !string.IsNullOrEmpty(relatedSubmissionData["VersionNumber"]) &&
                    !string.IsNullOrEmpty(relatedSubmissionData["AzureFileName"]))
                {
                    string messageforUI = relatedSubmissionData["policyNumber"] + "==" + relatedSubmissionData["OriginalSubmissionID"] + "==" + relatedSubmissionData["TransactionType"]
                        + "==" + relatedSubmissionData["VersionNumber"] + "==" + relatedSubmissionData["AzureFileName"];

                    return Json(messageforUI);
                }
                return Json("Unable to fetch all required data.");
            }

            return Json("SubmissionID/Server is mandatory to proceed.");
        }

        public ActionResult DownloadReport(string filePath, string key)
        {

            if (filePath == "SampleInput") { return File(globalPath_Input + "Sample Input.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PCF Tool Input.xlsx"); }

            if (System.IO.File.Exists(filePath)) { return File(filePath, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Final Result.xlsx"); }
            return Content("Oops. File not found.");
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

        private String SafeDecodeBase64(String str)
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

        private string SaveFinalOutput(DataSet dataSet)
        {
            string filePath = string.Empty;

            try
            {
                string fileName = DateTime.Now.ToString("hh_mm_ss_fff_tt") + ".xlsx";
                string outputFilepath = globalPath_Output + "/" + DateTime.Today.ToString("yyyyMMdd");

                if (!Directory.Exists(outputFilepath)) { Directory.CreateDirectory(outputFilepath); }
                filePath = outputFilepath + "\\" + fileName;
                using (XLWorkbook workbook = new XLWorkbook())
                {
                    foreach (DataTable table in dataSet.Tables) { workbook.Worksheets.Add(table); }
                    workbook.SaveAs(filePath);
                }

                return filePath;
            }
            catch (Exception ex) { string messge = ex.Message; }

            return filePath;
        }
    }
}