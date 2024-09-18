using System;
using System.IO;
using System.Web;
using System.Data;
using System.Web.Mvc;
using ClosedXML.Excel;
using System.Web.Hosting;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Collections.Generic;
using MindTreeValueAdds.Web.Models;

namespace MindTreeValueAdds.Web.Controllers
{
    public class OOBBalanceReportController : Controller
    {
        private static NLog.Logger nLogLogger = NLog.LogManager.GetCurrentClassLogger();
        private static LoginDtl _loginDtl = LoginDtl.Instance;

        readonly string SUCCESS = "success";
        readonly string FAIL = "fail";
        readonly int iLogger = 0;

        readonly string FinalOOBReport_Format = "OOB EDW Summary ";
        readonly string FinalCoverageBDReport_Format = "Coverage Breakdown ";
        readonly string FinalTaxBDReport_Format = "Tax Breakdown ";
        readonly string FinalPolicyTerm_Format = "Policy Term ";
        readonly string FinalEDWAndADWData_Format = "EDW & ADW Data ";
        readonly string DotXLSX = ".xlsx";
        readonly string globalPath = HostingEnvironment.MapPath("~/App_Data/OOB Reports");
        readonly string globalPath_Output = HostingEnvironment.MapPath("~/App_Data/OOB Reports/Output");

        static DataSet outputForms = new DataSet();

        public OOBBalanceReportController()
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
                    if (Session["GUID"] == null || Convert.ToString(Session["GUID"]) == string.Empty)
                        Session["GUID"] = Guid.NewGuid().ToString();

                    List<SelectListItem> Servers = new List<SelectListItem>();
                    //bool CUBE_DB_Status = Common.CheckOracleDBConnection(ConfigurationManager.ConnectionStrings["CUBEDB"].ConnectionString);
                    //bool EDW_DB_Status = Common.CheckOracleDBConnection(ConfigurationManager.ConnectionStrings["EDWDB"].ConnectionString);

                    string[] appSettings_Servers = ConfigurationManager.AppSettings["Server"].Split(',');

                    foreach (string Server in appSettings_Servers)
                    {
                        Servers.Add(new SelectListItem
                        {
                            Text = Server.Trim(),
                            Value = Server.Trim()
                        });
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

        public ActionResult Manage()
        {
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
            return View();
        }

        public JsonResult CheckOracleDBConnections(string Server)
        {
            List<string> oracleDBsInUse = new List<string> { "CUBE", "EDW" };
            bool CUBE_DB_Status, EDW_DB_Status;
            if (Server == "INT")
            {
                CUBE_DB_Status = Common.CheckOracleDBConnection(ConfigurationManager.ConnectionStrings["CUBEINTDB"].ConnectionString);
                EDW_DB_Status = Common.CheckOracleDBConnection(ConfigurationManager.ConnectionStrings["EDWINTDB"].ConnectionString);
            }
            else
            {
                CUBE_DB_Status = Common.CheckOracleDBConnection(ConfigurationManager.ConnectionStrings["CUBEDB"].ConnectionString);
                EDW_DB_Status = Common.CheckOracleDBConnection(ConfigurationManager.ConnectionStrings["EDWDB"].ConnectionString);
            }

            if (CUBE_DB_Status && EDW_DB_Status) { return Json(SUCCESS, JsonRequestBehavior.AllowGet); }

            return Json("Oops. CUBE/EDW DB is not avaialable to use.", JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidatePolicy(string PolicyNumber, string Server)
        {
            string isPolicyValid = FAIL; string Message = "Oops. Something went wrong.";
            string[] policyData = PolicyNumber.Split(' ');

            if (policyData.Length == 4 || (PolicyNumber.EndsWith("X") && policyData.Length == 5))
            {
                bool CUBE_DB_Status = Common.CheckOracleDBConnection(ConfigurationManager.ConnectionStrings[Common.GetConnectionStringName(Server).Split(':')[0]].ConnectionString);
                bool EDW_DB_Status = Common.CheckOracleDBConnection(ConfigurationManager.ConnectionStrings[Common.GetConnectionStringName(Server).Split(':')[1]].ConnectionString);

                if (Common.ValidateMLPolicySymbol(PolicyNumber))
                {
                    if (CUBE_DB_Status)
                    {
                        if (OOBBalanceReport.UserDefineFunctions.VerifyPolicyInBDM(PolicyNumber, Server))
                        {
                            isPolicyValid = SUCCESS; Message = SUCCESS;
                        }
                        else { Message = "Oops. Policy is missing from BDM."; }
                    }
                    else { Message = "Oops. CUBE DB is not avaialable to use."; }
                }
                else
                {
                    if (CUBE_DB_Status && EDW_DB_Status)
                    {
                        if (OOBBalanceReport.UserDefineFunctions.VerifyPolicyInCUBE(PolicyNumber, Server))
                        {
                            if (OOBBalanceReport.UserDefineFunctions.VerifyPolicyInPolicyTerm(PolicyNumber, Server))
                            {
                                isPolicyValid = SUCCESS; Message = SUCCESS;
                            }
                            else { Message = "Oops. Policy is missing from EDW."; }
                        }
                        else { Message = "Oops. Policy is missing from CUBE."; }
                    }
                    else { Message = "Oops. CUBE/EDW DB is not avaialable to use."; }
                }
            }
            else { Message = "Oops. Invalid Policy."; }

            if (isPolicyValid == SUCCESS) { return Json(Message, JsonRequestBehavior.AllowGet); }
            else { return Json(Message, JsonRequestBehavior.AllowGet); }
        }

        public JsonResult ExtractOOBReport(string PolicyNumber, string Server, bool AdvanceOption, string SubmissionIDs, string key)
        {
            try
            {
                string[] FullPolicyData = PolicyNumber.Split(' ');

                if (FullPolicyData.Length == 4 || (PolicyNumber.EndsWith("X") && FullPolicyData.Length == 5))
                {
                    nLogLogger.Info("*******************************************************************" + Environment.NewLine);
                    nLogLogger.Info(" " + Session["LoginUserName"] + " OOB Balance Report Process is started. Input Details : " + Server + ", " + PolicyNumber + ", " + SubmissionIDs + ", " + ", " + key);

                    string DCTServerURL = Common.ReturnValidDUCKServerURL(Server);
                    DataSet FinalOutput = new DataSet();

                    string[] lstSubmissionIDs = SubmissionIDs.Trim().Split(',');
                    if (lstSubmissionIDs.Length > 0 && !string.IsNullOrEmpty(SubmissionIDs)) { FinalOutput = OOBBalanceReport.UserDefineFunctions.ExtractAndCompileData(DCTServerURL, Server, PolicyNumber, lstSubmissionIDs); }

                    else { FinalOutput = OOBBalanceReport.UserDefineFunctions.ExtractAndCompileData(DCTServerURL, Server, PolicyNumber); }

                    if (FinalOutput.Tables.Count > 0)
                    {
                        Session["GUID"] = key;
                        string filePath = SaveFinalOutput(FinalOutput);
                        if (!string.IsNullOrEmpty(filePath))
                        {
                            nLogLogger.Info(" " + Session["LoginUserName"] + " OOB Balance Report Process is completed successfully");
                            nLogLogger.Info("*******************************************************************" + Environment.NewLine);
                            return Json(SUCCESS + "==" + filePath, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                nLogLogger.Info(" " + Session["LoginUserName"] + " Exception oucred for " + PolicyNumber + " " + ex.Message);
                nLogLogger.Info("*******************************************************************" + Environment.NewLine);
                Logger.logException(ex, "Exception oucred for " + PolicyNumber);
            }

            return Json(PolicyNumber, JsonRequestBehavior.DenyGet);
        }

        public JsonResult ValidateSubmission(string PolicyNumber, string Submission, string Server)
        {
            try
            {
                string[] policyData = PolicyNumber.Split(' ');

                if (policyData.Length == 4)
                {
                    if (OOBBalanceReport.UserDefineFunctions.VerifyPolicyInEDW(PolicyNumber, Submission, Server))
                    {
                        if (OOBBalanceReport.UserDefineFunctions.VerifyPolicyInADW(PolicyNumber, Submission, Server))
                        {
                            return Json(SUCCESS, JsonRequestBehavior.AllowGet);
                        }
                        else { return Json("Oops. Policy not presnt in ADW.", JsonRequestBehavior.AllowGet); }
                    }
                    else { return Json("Oops. Policy not presnt in EDW.", JsonRequestBehavior.AllowGet); }
                }
                else { return Json("Oops. Invalid Policy #.", JsonRequestBehavior.AllowGet); }
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.DenyGet);
            }
        }

        public JsonResult ExtractCoverageBreakDOwnReport(string PolicyNumber, string Submission, string Server)
        {
            string[] policyData = PolicyNumber.Split(' ');

            if (policyData.Length == 4)
            {
                if (OOBBalanceReport.UserDefineFunctions.VerifyPolicyInEDW(PolicyNumber, Submission, Server))
                {
                    if (OOBBalanceReport.UserDefineFunctions.VerifyPolicyInADW(PolicyNumber, Submission, Server))
                    {
                        DataSet FinalOutput = OOBBalanceReport.UserDefineFunctions.ExtractCoverageBreakdown(PolicyNumber, Submission, Server);

                        if (FinalOutput.Tables.Count > 0)
                        {
                            Session["GUID"] = "";
                            string filePath = SaveFinalOutput(FinalOutput);
                            if (!string.IsNullOrEmpty(filePath)) { return Json(SUCCESS + "==" + filePath, JsonRequestBehavior.AllowGet); }
                        }

                        return Json("Oops. Nothing to disply.", JsonRequestBehavior.AllowGet);
                    }
                    else { return Json("Oops. Policy is not present in ADW.", JsonRequestBehavior.AllowGet); }
                }
                else { return Json("Oops. Policy is not present in EDW.", JsonRequestBehavior.AllowGet); }
            }
            else { return Json("Oops. Invalid Policy #.", JsonRequestBehavior.AllowGet); }
        }

        public JsonResult ExtractTaxBreakDownReport(string PolicyNumber, string Server)
        {
            string[] policyData = PolicyNumber.Split(' ');

            if (policyData.Length == 4)
            {
                if (OOBBalanceReport.UserDefineFunctions.VerifyPolicyInEDW(PolicyNumber, string.Empty, Server))
                {
                    if (OOBBalanceReport.UserDefineFunctions.VerifyPolicyInADW(PolicyNumber, string.Empty, Server))
                    {
                        DataSet FinalOutput = OOBBalanceReport.UserDefineFunctions.ExtractTaxBreakdown(PolicyNumber, Server);

                        if (FinalOutput.Tables.Count > 0)
                        {
                            Session["GUID"] = "";
                            string filePath = SaveFinalOutput(FinalOutput);
                            if (!string.IsNullOrEmpty(filePath)) { return Json(SUCCESS + "==" + filePath, JsonRequestBehavior.AllowGet); }
                        }

                        return Json("Oops. Nothing to disply.", JsonRequestBehavior.AllowGet);
                    }
                    else { return Json("Oops. Policy is not present in ADW.", JsonRequestBehavior.AllowGet); }
                }
                else { return Json("Oops. Policy is not present in EDW.", JsonRequestBehavior.AllowGet); }
            }
            else { return Json("Oops. Invalid Policy #.", JsonRequestBehavior.AllowGet); }
        }

        public JsonResult DownloadPolicyTerm(string PolicyNumber, string Server)
        {
            string[] policyData = PolicyNumber.Split(' ');

            if (policyData.Length == 4)
            {
                if (OOBBalanceReport.UserDefineFunctions.VerifyPolicyInPolicyTerm(PolicyNumber, Server))
                {
                    DataSet FinalOutput = OOBBalanceReport.UserDefineFunctions.ExtractPolicyTermTable(PolicyNumber, Server);

                    if (FinalOutput.Tables.Count > 0)
                    {
                        Session["GUID"] = "";
                        string filePath = SaveFinalOutput(FinalOutput);
                        if (!string.IsNullOrEmpty(filePath)) { return Json(SUCCESS + "==" + filePath, JsonRequestBehavior.AllowGet); }
                    }

                    return Json("Oops. Nothing to disply.", JsonRequestBehavior.AllowGet);
                }
                else { return Json("Oops. Policy is not present in policy term table.", JsonRequestBehavior.AllowGet); }
            }
            else { return Json("Oops. Invalid Policy #.", JsonRequestBehavior.AllowGet); }
        }

        public JsonResult DownloadEDWAndADWData(string PolicyNumber, string Server, string SubmissionIDs)
        {
            string[] policyData = PolicyNumber.Split(' ');

            if (policyData.Length == 4)
            {
                if (OOBBalanceReport.UserDefineFunctions.VerifyPolicyInEDW(PolicyNumber, Server))
                {
                    if (OOBBalanceReport.UserDefineFunctions.VerifyPolicyInADW(PolicyNumber, Server))
                    {
                        string[] lstSubmissionIDs = SubmissionIDs.Trim().Split(',');

                        if (lstSubmissionIDs.Length > 0)
                        {
                            DataSet FinalOutput = OOBBalanceReport.UserDefineFunctions.DownloadEDWAndADWData(PolicyNumber, Server, lstSubmissionIDs);

                            if (FinalOutput.Tables.Count > 0)
                            {
                                Session["GUID"] = "";
                                string filePath = SaveFinalOutput(FinalOutput);
                                if (!string.IsNullOrEmpty(filePath)) { return Json(SUCCESS + "==" + filePath, JsonRequestBehavior.AllowGet); }
                            }

                            return Json("Oops. Nothing to disply.", JsonRequestBehavior.AllowGet);
                        }
                        else { return Json("Oops. Invalid/Incorrect Submission ID's.", JsonRequestBehavior.AllowGet); }
                    }
                    else { return Json("Oops. Policy is not present in ADW.", JsonRequestBehavior.AllowGet); }
                }
                else { return Json("Oops. Policy is not present in EDW.", JsonRequestBehavior.AllowGet); }
            }
            else { return Json("Oops. Invalid Policy #.", JsonRequestBehavior.AllowGet); }
        }

        public JsonResult ValidateBDMPremiums(string PolicyNumber, string Submission, string Server)
        {
            try
            {
                string[] policyData = PolicyNumber.Split(' ');

                if (policyData.Length == 4)
                {
                    DataTable result = OOBBalanceReport.UserDefineFunctions.ValidateBDMPremiums(PolicyNumber, "'" + Submission + "'", Server);
                    if (result.Rows.Count > 0)
                    {
                        string TOT_APRPP_PREM_AMT = Convert.ToString(result.Rows[0]["TOT_APRPP_PREM_AMT"]);
                        string TOT_APRP_PRM_EXCLD_TX_SRCHRG = Convert.ToString(result.Rows[0]["TOT_APRP_PRM_EXCLD_TX_SRCHRG"]);
                        string TOT_APRP_PREM_TX_SRCHRG_AMT = Convert.ToString(result.Rows[0]["TOT_APRP_PREM_TX_SRCHRG_AMT"]);

                        return Json(TOT_APRPP_PREM_AMT + "==" + TOT_APRP_PRM_EXCLD_TX_SRCHRG + "==" + TOT_APRP_PREM_TX_SRCHRG_AMT, JsonRequestBehavior.AllowGet);
                    }
                    else { return Json("Oops. Policy/Submission not presnt in BDM.", JsonRequestBehavior.AllowGet); }
                }
                else { return Json("Oops. Invalid Policy #.", JsonRequestBehavior.AllowGet); }
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.DenyGet);
            }
        }

        public void DownloadReport(string CallingName, string CallingType, string key)
        {
            try
            {
                if (Session[CallingName] != null && !string.IsNullOrEmpty(CallingName) && !string.IsNullOrEmpty(CallingType) && Convert.ToString(Session["GUID"]) == key)
                {
                    string fileName = string.Empty;
                    DataSet FinalOutput = (DataSet)Session[CallingName];

                    if (FinalOutput.Tables.Count > 0 && (CallingType == "OOBReport" || CallingType == "CoverageBDForPolicy" || CallingType == "CoverageBDForSubmission" || CallingType == "TaxBD" || CallingType == "PolicyTerm" || CallingType == "EDWAndADWData"))
                    {
                        if (CallingType == "OOBReport") { fileName = FinalOOBReport_Format + CallingName + DotXLSX; }
                        else if (CallingType == "CoverageBDForPolicy" || CallingType == "CoverageBDForSubmission") { fileName = FinalCoverageBDReport_Format + CallingName + DotXLSX; }
                        else if (CallingType == "TaxBD") { fileName = FinalTaxBDReport_Format + CallingName + DotXLSX; }
                        else if (CallingType == "PolicyTerm") { fileName = FinalPolicyTerm_Format + CallingName + DotXLSX; }
                        else if (CallingType == "EDWAndADWData") { fileName = FinalEDWAndADWData_Format + CallingName + DotXLSX; }
                        else { fileName = "Invalid or Missing Calling Type" + CallingName + DotXLSX; }

                        using (XLWorkbook workbook = new XLWorkbook())
                        {
                            foreach (DataTable dataTable in FinalOutput.Tables)
                            {
                                workbook.Worksheets.Add(dataTable);
                                Response.Clear();
                                Response.Buffer = true;
                                Response.Charset = "";
                                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                                Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
                            }
                            using (MemoryStream myStream = new MemoryStream())
                            {
                                workbook.SaveAs(myStream);
                                myStream.WriteTo(Response.OutputStream);
                                Response.Flush();
                                Response.End();
                            }
                        }
                    }
                }
            }
            catch { }
        }

        public ActionResult DownloadReportNew(string CallingName, string CallingType, string filePath, string key)
        {
            if (System.IO.File.Exists(filePath))
            {
                string fileName = string.Empty;
                if (CallingType == "OOBReport") { fileName = FinalOOBReport_Format + CallingName + DotXLSX; }
                else if (CallingType == "TaxBD") { fileName = FinalTaxBDReport_Format + CallingName + DotXLSX; }
                else if (CallingType == "PolicyTerm") { fileName = FinalPolicyTerm_Format + CallingName + DotXLSX; }
                else if (CallingType == "EDWAndADWData") { fileName = FinalEDWAndADWData_Format + CallingName + DotXLSX; }
                else if (CallingType == "CoverageBDForPolicy" || CallingType == "CoverageBDForSubmission") { fileName = FinalCoverageBDReport_Format + CallingName + DotXLSX; }

                if (!string.IsNullOrEmpty(fileName)) { return File(filePath, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName); }
            }

            return Content("Oops. File not found.");
        }

        private string SaveFinalOutput(DataSet dataSet)
        {
            string filePath = string.Empty;

            try
            {
                string outputFilepath = globalPath_Output + "/" + DateTime.Today.ToString("yyyyMMdd");
                if (!Directory.Exists(outputFilepath)) { Directory.CreateDirectory(outputFilepath); }

                filePath = outputFilepath + "\\" + DateTime.Now.ToString("hh_mm_ss_fff_tt") + DotXLSX;
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

        private string logoutURl()
        {
            string logout_endpoint = _loginDtl.GetLogout_Endpoint();
            string idtoken_hint = _loginDtl.idToken;
            string base_url = _loginDtl.GetBase_Url();

            Session.Abandon();
            string logout_request = $"{logout_endpoint}?id_token_hint={idtoken_hint}&post_logout_redirect_uri={HttpUtility.UrlEncode(base_url)}";
            return logout_request;
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
    }
}