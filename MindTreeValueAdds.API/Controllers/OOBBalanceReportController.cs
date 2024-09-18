using System;
using System.Data;
using System.Web.Http;
using System.Configuration;
using System.Threading.Tasks;
using System.Collections.Generic;
using MindTreeValueAdds.API.Models;

namespace MindTreeValueAdds.API.Controllers
{
    [RoutePrefix("api/oobbalancereport")]
    public class OOBBalanceReportController : ApiController
    {
        #region Readonly Variables
        static readonly string errorLogFileName = "OOBBalanceReport";
        static readonly string messageforUI_UnAvailableDB = "Unablle to set connection through DB.";
        #endregion

        [HttpGet]
        [Route("checkdbconnection")]
        public IHttpActionResult CheckDBConnection(string DB)
        {
            if (string.IsNullOrEmpty(DB)) { return BadRequest(); }

            else if (new List<string> { "CUBE", "EDW" }.Contains(DB) == false) { return BadRequest(); }

            else
            {
                if (DB == "CUBE")
                {
                    if (Helppers.OOBBalanceReport.CheckOracleDBConnection(ConfigurationManager.ConnectionStrings["CUBEDB"].ConnectionString)) { return Ok(); }
                }
                else
                {
                    if (Helppers.OOBBalanceReport.CheckOracleDBConnection(ConfigurationManager.ConnectionStrings["EDWDB"].ConnectionString)) { return Ok(); }
                }

                return NotFound();
            }
        }

        #region CUBE DB
        [HttpGet]
        [Route("getcubemanageactivityscreendetails")]
        public IHttpActionResult GetCUBEManageActivityScreenDetails(string PolicyNumber)
        {
            if (Helppers.OOBBalanceReport.ValidatePolicyNumber(PolicyNumber) == false) { return BadRequest(); }

            string[] policyData = PolicyNumber.Split(' ');
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { "&POL_NO_CD", policyData[1] },
                { "&POL_MODULE_CD", policyData[2] },
                { "&POL_VERSION_CD", policyData[3] }
            };

            return ExicuteSQLScriptUsingFileName("FetchCubeManageActivityScreenDetails", Parameters, ConfigurationManager.ConnectionStrings["CUBEDB"].ConnectionString);
        }

        [HttpGet]
        [Route("getcubemanageactivityscreendetails")]
        public IHttpActionResult GetCUBEManageActivityScreenDetails(string PolicyNumber, string SubmissionID)
        {
            if (Helppers.OOBBalanceReport.ValidatePolicyNumber(PolicyNumber) == false) { return BadRequest(); }

            string[] policyData = PolicyNumber.Split(' ');
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { "&POL_NO_CD", policyData[1] },
                { "&POL_MODULE_CD", policyData[2] },
                { "&POL_VERSION_CD", policyData[3] },
                { "&SUBMISSION_ID", SubmissionID }
            };

            return ExicuteSQLScriptUsingFileName("FetchCubeManageActivityScreenDetailsForSubmission", Parameters, ConfigurationManager.ConnectionStrings["CUBEDB"].ConnectionString);
        }
        #endregion

        #region BDM DB
        [HttpGet]
        [Route("getpolicydatafrombdm")]
        public IHttpActionResult GetPolicyDataFromBDM(string PolicyNumber)
        {
            if (Helppers.OOBBalanceReport.ValidatePolicyNumber(PolicyNumber) == false) { return BadRequest(); }

            string[] policyData = PolicyNumber.Split(' ');
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { "&POL_NO_CD", policyData[1] },
                { "&POL_MODULE_CD", policyData[2] },
                { "&POL_VERSION_CD", policyData[3] }
            };

            return ExicuteSQLScriptUsingFileName("BDMScript", Parameters, ConfigurationManager.ConnectionStrings["EDWDB"].ConnectionString);
        }

        #endregion

        #region EDW DB
        [HttpGet]
        [Route("getpolicynontaxdatafromedw")]
        public IHttpActionResult GetPolicyNonTaxDataFromEDW(string PolicyNumber)
        {
            if (Helppers.OOBBalanceReport.ValidatePolicyNumber(PolicyNumber) == false) { return BadRequest(); }

            string[] policyData = PolicyNumber.Split(' ');
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { "&POL_NO_CD", policyData[1] },
                { "&POL_MODULE_CD", policyData[2] },
                { "&POL_VERSION_CD", policyData[3] }
            };

            return ExicuteSQLScriptUsingFileName("EDWNonTax", Parameters, ConfigurationManager.ConnectionStrings["EDWDB"].ConnectionString);
        }

        [HttpGet]
        [Route("getpolicytaxdatafromedw")]
        public IHttpActionResult GetPolicyTaxDataFromEDW(string PolicyNumber)
        {
            if (Helppers.OOBBalanceReport.ValidatePolicyNumber(PolicyNumber) == false) { return BadRequest(); }

            string[] policyData = PolicyNumber.Split(' ');
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { "&POL_NO_CD", policyData[1] },
                { "&POL_MODULE_CD", policyData[2] },
                { "&POL_VERSION_CD", policyData[3] }
            };

            return ExicuteSQLScriptUsingFileName("EDWTax", Parameters, ConfigurationManager.ConnectionStrings["EDWDB"].ConnectionString);
        }
        #endregion

        #region ADW DB
        [HttpGet]
        [Route("getpolicynontaxdatafromadw")]
        public IHttpActionResult GetPolicyNonTaxDataFromADW(string PolicyNumber)
        {
            if (Helppers.OOBBalanceReport.ValidatePolicyNumber(PolicyNumber) == false) { return BadRequest(); }

            string[] policyData = PolicyNumber.Split(' ');
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { "&POL_NO_CD", policyData[1] },
                { "&POL_MODULE_CD", policyData[2] },
                { "&POL_VERSION_CD", policyData[3] }
            };

            return ExicuteSQLScriptUsingFileName("ADWNonTax", Parameters, ConfigurationManager.ConnectionStrings["EDWDB"].ConnectionString);
        }

        [HttpGet]
        [Route("getpolicytaxdatafromadw")]
        public IHttpActionResult GetPolicyTaxDataFromADW(string PolicyNumber)
        {
            if (Helppers.OOBBalanceReport.ValidatePolicyNumber(PolicyNumber) == false) { return BadRequest(); }

            string[] policyData = PolicyNumber.Split(' ');
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { "&POL_NO_CD", policyData[1] },
                { "&POL_MODULE_CD", policyData[2] },
                { "&POL_VERSION_CD", policyData[3] }
            };

            return ExicuteSQLScriptUsingFileName("ADWTax", Parameters, ConfigurationManager.ConnectionStrings["EDWDB"].ConnectionString);
        }
        #endregion

        private IHttpActionResult ExicuteSQLScriptUsingFileName(string FileName, Dictionary<string, string> Parameters, string ConnectionStrings)
        {
            try
            {
                string SQLScript = OOBBalanceReport.GetSQLScript(FileName);
                if (string.IsNullOrEmpty(SQLScript)) { return InternalServerError(); }

                foreach (string Parameter in Parameters.Keys) { SQLScript = SQLScript.Replace(Parameter, Parameters[Parameter]); }

                // Check if DB is available to use or not.
                if (Helppers.OOBBalanceReport.CheckOracleDBConnection(ConnectionStrings) == false) { return BadRequest(messageforUI_UnAvailableDB); }

                DataTable ExicuteSQLScriptRs = Helppers.OOBBalanceReport.ExicuteSQLScriptAsync(ConnectionStrings, SQLScript);

                if (ExicuteSQLScriptRs == null) { return NotFound(); }
                else if (ExicuteSQLScriptRs.Rows.Count < 1) { return NotFound(); }

                return Ok(ExicuteSQLScriptRs);
            }
            catch (Exception ex)
            {
                Common.LogException(ex, errorLogFileName);
                return InternalServerError();
            }
        }
    }
}
