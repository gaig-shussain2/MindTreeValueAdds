using System;
using System.IO;
using System.Xml;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml.XPath;
using System.Web.Hosting;
using System.Configuration;
using System.Collections.Generic;

namespace MindTreeValueAdds.Web.Models
{
    public class OOBBalanceReport
    {
        static readonly string Success = "success";
        static readonly string DBScriptsPath = "~/App_Data/DB Scripts/OOB/";
        static readonly string DBScriptsPath_Duck = "~/App_Data/DB Scripts/OOB/ForDuckPolicy/";
        //static readonly string DBScriptsPath_Other = "~/App_Data/DB Scripts/OOB/ForOtherPolicy/";
        //static readonly string className = "OOBBalanceReport";

        public class UserDefineFunctions
        {
            static string GetSQLScript(string Type)
            {
                switch (Type)
                {
                    case "FetchCubeManageActivityScreenDetails":
                        return File.ReadAllText(HostingEnvironment.MapPath(DBScriptsPath + "FetchCubeManageActivityScreenDetails.txt"));

                    case "FetchCubeManageActivityScreenDetailsForSubmission":
                        return File.ReadAllText(HostingEnvironment.MapPath(DBScriptsPath + "FetchCubeManageActivityScreenDetailsForSubmission.txt"));

                    case "BDM":
                        return File.ReadAllText(HostingEnvironment.MapPath(DBScriptsPath_Duck + "BDMNew.txt"));

                    case "BDMBySubmission":
                        return File.ReadAllText(HostingEnvironment.MapPath(DBScriptsPath_Duck + "BDMNewBySubmission.txt"));

                    case "EDWNonTax":
                        return "select * from qdm.VW_TRANS_PREMIUM_CLSMDMKEY where POL_NO_CD in ('&POL_NO_CD') and POL_MODULE_CD in ('&POL_MODULE_CD') and POL_VERSION_CD in ('&POL_VERSION_CD')";

                    case "EDWNonTaxForSubmission":
                        return "select * from qdm.VW_TRANS_PREMIUM_CLSMDMKEY where POL_NO_CD in ('&POL_NO_CD') and POL_MODULE_CD in ('&POL_MODULE_CD') and POL_VERSION_CD in ('&POL_VERSION_CD') and SUBM_ID in ('&SUBM_ID')";

                    case "EDWTax":
                        return "select * from qdm.VW_TRANS_NON_PREMIUM where POL_NO_CD in ('&POL_NO_CD') and POL_MODULE_CD in ('&POL_MODULE_CD') and POL_VERSION_CD in ('&POL_VERSION_CD')";

                    case "EDWTaxForSubmission":
                        return "select * from qdm.VW_TRANS_NON_PREMIUM where POL_NO_CD in ('&POL_NO_CD') and POL_MODULE_CD in ('&POL_MODULE_CD') and POL_VERSION_CD in (&POL_VERSION_CD) and SUBM_ID in ('&SUBM_ID')";

                    case "ADWNonTax":
                        return File.ReadAllText(HostingEnvironment.MapPath(DBScriptsPath_Duck + "ADWNonTax.txt"));

                    case "ADWNonTaxForSubmission":
                        return File.ReadAllText(HostingEnvironment.MapPath(DBScriptsPath_Duck + "ADWNonTaxForSubmission.txt"));

                    case "ADWTax":
                        return File.ReadAllText(HostingEnvironment.MapPath(DBScriptsPath_Duck + "ADWTax.txt"));

                    case "ADWTaxForSubmission":
                        return File.ReadAllText(HostingEnvironment.MapPath(DBScriptsPath_Duck + "ADWTaxForSubmission.txt"));

                    case "PolicyTerm":
                        return File.ReadAllText(HostingEnvironment.MapPath(DBScriptsPath_Duck + "PolicyTerm.txt"));

                    case "EDWNonTaxForSubmission-Total":
                        return "SELECT sum(PREM_AMT) Total from qdm.VW_TRANS_PREMIUM_CLSMDMKEY where POL_NO_CD in ('&POL_NO_CD') and POL_MODULE_CD in ('&POL_MODULE_CD') and POL_VERSION_CD in ('&POL_VERSION_CD') and SUBM_ID in ('&SUBM_ID') and PRCPREM_CATGCD = 'APRP' and PREM_AMT != 0";

                    case "EDWTaxForSubmission-Total":
                        return "SELECT sum(NON_PREM_AMT) Total from qdm.VW_TRANS_NON_PREMIUM where POL_NO_CD in ('&POL_NO_CD') and POL_MODULE_CD in ('&POL_MODULE_CD') and POL_VERSION_CD in ('&POL_VERSION_CD') and SUBM_ID in ('&SUBM_ID') and PRCPREM_CATGCD = 'APRP' and PREM_AMT != 0";

                    case "ADWNonTaxForSubmission-Total":
                        return "SELECT sum (A.PREMIUM_CHANGE_AMOUNT) Total FROM ADW.DIVISIONAL_PREMIUMS A INNER JOIN EDW.EDW_TRANS T on T.EDW_TRANS_ID=A.EDW_TRANS_ID INNER JOIN qdm.VW_TRANS_POLTERM PT on A.EDW_TRANS_ID=PT.EDW_TRANS_ID WHERE PT.POL_NO_CD IN ('&POL_NO_CD') AND PT.POL_MODULE_CD IN ('&POL_MODULE_CD') AND PT.POL_VERSION_CD IN ('&POL_VERSION_CD') AND A.SUBM_ID IN ('&SUBM_ID') AND CHARGE_CD IS NULL";

                    case "ADWTaxForSubmission-Total":
                        return "SELECT sum (A.PREMIUM_CHANGE_AMOUNT) Total FROM ADW.DIVISIONAL_PREMIUMS A INNER JOIN EDW.EDW_TRANS T on T.EDW_TRANS_ID=A.EDW_TRANS_ID INNER JOIN qdm.VW_TRANS_POLTERM PT on A.EDW_TRANS_ID=PT.EDW_TRANS_ID WHERE PT.POL_NO_CD IN ('&POL_NO_CD') AND PT.POL_MODULE_CD IN ('&POL_MODULE_CD') AND PT.POL_VERSION_CD IN ('&POL_VERSION_CD') AND A.SUBM_ID IN ('&SUBM_ID') AND CHARGE_CD IS NOT NULL";

                    case "EDWNonTaxForBigPolicy":
                        return File.ReadAllText(HostingEnvironment.MapPath(DBScriptsPath + "EDWUpdated/EDWNonTaxForBigPolicy.txt"));

                    case "ADWNonTaxForBigPolicy":
                        return File.ReadAllText(HostingEnvironment.MapPath(DBScriptsPath + "EDWUpdated/ADWNonTaxForBigPolicy.txt"));

                    case "ADWTaxForBigPolicy":
                        return File.ReadAllText(HostingEnvironment.MapPath(DBScriptsPath + "EDWUpdated/ADWTaxForBigPolicy.txt"));

                    default:
                        return "";
                }
            }

            internal static bool VerifyPolicyInCUBE(string policyNumber, string server, string submission = "")
            {
                string[] policyData = policyNumber.Split(' ');
                string SQLScript = string.Empty;

                if (!string.IsNullOrEmpty(submission))
                {
                    SQLScript = GetSQLScript("FetchCubeManageActivityScreenDetailsForSubmission");
                    SQLScript = SQLScript.Replace("&POL_NO_CD", policyData[1]).Replace("&POL_MODULE_CD", policyData[2]).Replace("&POL_VERSION_CD", policyData[3]).Replace("&SUBMISSION_ID", submission);
                }
                else
                {
                    SQLScript = GetSQLScript("FetchCubeManageActivityScreenDetails");
                    SQLScript = SQLScript.Replace("&POL_NO_CD", policyData[1]).Replace("&POL_MODULE_CD", policyData[2]).Replace("&POL_VERSION_CD", policyData[3]);
                }

				DataTable OracleDBResults = Common.ExicuteOracleDBQuery(SQLScript, ConfigurationManager.ConnectionStrings[Common.GetConnectionStringName(server).Split(':')[0]].ConnectionString);

                if (OracleDBResults.Rows.Count > 0) { return true; }

                return false;
            }

            internal static bool VerifyPolicyInPolicyTerm(string policyNumber, string server)
            {
                string[] policyData = policyNumber.Split(' ');

                string SQLScript = GetSQLScript("PolicyTerm") + " AND rownum < 2";
                SQLScript = SQLScript.Replace("&POL_NO_CD", policyData[1]).Replace("&POL_MODULE_CD", policyData[2]).Replace("&POL_VERSION_CD", policyData[3]);

				DataTable OracleDBResults = Common.ExicuteOracleDBQuery(SQLScript, ConfigurationManager.ConnectionStrings[Common.GetConnectionStringName(server).Split(':')[1]].ConnectionString);

                if (OracleDBResults.Rows.Count > 0) { return true; }
                return false;
            }

			internal static bool VerifyPolicyInBDM(string policyNumber, string server)
			{
				string[] policyData = policyNumber.Split(' ');

                string SQLScript = GetSQLScript("BDM");
                SQLScript = SQLScript.Replace("&POL_NO_CD", policyData[1]).Replace("&POL_MODULE_CD", policyData[2]).Replace("&POL_VERSION_CD", policyData[3]).Replace("AND rownum<5000", "AND rownum<2");

				DataTable OracleDBResults = Common.ExicuteOracleDBQuery(SQLScript, ConfigurationManager.ConnectionStrings[Common.GetConnectionStringName(server).Split(':')[1]].ConnectionString);

                if (OracleDBResults.Rows.Count > 0) { return true; }

                return false;
            }

            internal static bool VerifyPolicyAndSubmissionInBDM(string policyNumber, string submission, string server)
            {
                string[] policyData = policyNumber.Split(' ');

                string SQLScript = GetSQLScript("BDMBySubmission");
                SQLScript = SQLScript.Replace("&POL_NO_CD", policyData[1]).Replace("&POL_MODULE_CD", policyData[2]).Replace("&POL_VERSION_CD", policyData[3]).Replace("AND rownum<5000", "AND rownum<2").Replace("'&SOURCE_SUBMISSION_ID'", submission);

                DataTable OracleDBResults = Common.ExicuteOracleDBQuery(SQLScript, ConfigurationManager.ConnectionStrings[Common.GetConnectionStringName(server).Split(':')[1]].ConnectionString);

                if (OracleDBResults.Rows.Count > 0) { return true; }

                return false;
            }

			internal static bool VerifyPolicyInEDW(string policyNumber, string server)
			{
				string[] policyData = policyNumber.Split(' ');
				string SQLScript = SQLScript = "select * from qdm.VW_TRANS_PREMIUM_CLSMDMKEY where POL_NO_CD in ('" + policyData[1] + "') and POL_MODULE_CD in ('" + policyData[2] + "') and POL_VERSION_CD in ('" + policyData[3] + "') and rownum<2";

				DataTable result = Common.ExicuteOracleDBQuery(SQLScript, ConfigurationManager.ConnectionStrings[Common.GetConnectionStringName(server).Split(':')[1]].ConnectionString);

                if (result.Rows.Count > 0) { return true; }
                return false;
            }

			internal static bool VerifyPolicyInEDW(string policyNumber, string submission, string server)
            {
				if (string.IsNullOrEmpty(submission)) { return VerifyPolicyInEDW(policyNumber, server); };

                string[] policyData = policyNumber.Split(' ');
                string SQLScript = SQLScript = "select * from qdm.VW_TRANS_PREMIUM_CLSMDMKEY where POL_NO_CD in ('" + policyData[1] + "') and POL_MODULE_CD in ('" + policyData[2] + "') and POL_VERSION_CD in ('" + policyData[3] + "') and subm_id in ('" + submission + "') and rownum<2";

				DataTable result = Common.ExicuteOracleDBQuery(SQLScript, ConfigurationManager.ConnectionStrings[Common.GetConnectionStringName(server).Split(':')[1]].ConnectionString);

                if (result.Rows.Count > 0) { return true; }
                return false;
            }

			internal static bool VerifyPolicyInADW(string policyNumber, string server)
            {
                string[] policyData = policyNumber.Split(' ');

                string SQLScript = GetSQLScript("ADWNonTax");
                SQLScript = SQLScript.Replace("&POL_NO_CD", policyData[1]).Replace("&POL_MODULE_CD", policyData[2]).Replace("&POL_VERSION_CD", policyData[3]).Replace("AND CHARGE_CD IS NULL", "AND CHARGE_CD IS NULL AND rownum<2");

				DataTable result = Common.ExicuteOracleDBQuery(SQLScript, ConfigurationManager.ConnectionStrings[Common.GetConnectionStringName(server).Split(':')[1]].ConnectionString);

                if (result.Rows.Count > 0) { return true; }
                return false;
            }

			internal static bool VerifyPolicyInADW(string policyNumber, string submission, string server)
			{
				if (string.IsNullOrEmpty(submission)) { return VerifyPolicyInADW(policyNumber, server); };

                string[] policyData = policyNumber.Split(' ');

                string SQLScript = GetSQLScript("ADWNonTaxForSubmission");
                SQLScript = SQLScript.Replace("&POL_NO_CD", policyData[1]).Replace("&POL_MODULE_CD", policyData[2]).Replace("&POL_VERSION_CD", policyData[3]).Replace("'&SUBM_ID'", submission).Replace("AND CHARGE_CD IS NULL", "AND CHARGE_CD IS NULL AND rownum<2");

				DataTable result = Common.ExicuteOracleDBQuery(SQLScript, ConfigurationManager.ConnectionStrings[Common.GetConnectionStringName(server).Split(':')[1]].ConnectionString);

                if (result.Rows.Count > 0) { return true; }
                return false;
            }

            internal static DataSet ExtractAndCompileData(string DCTServerURL, string Server, string PolicyNumber)
            {
                DataSet results = new DataSet();

                if (!string.IsNullOrEmpty(DCTServerURL) && !string.IsNullOrEmpty(Server) && !string.IsNullOrEmpty(PolicyNumber))
                {
                    #region Define Static Data
                    bool isPolicyProcessingSystemIsDuck = false;
                    bool isPolicyTooBig = false;
                    bool isMLPolicy = false;
                    int TransactionCountToConsiderAsBig = Convert.ToInt32(ConfigurationManager.AppSettings["TransactionCountToConsiderAsBig"]);

                    DataTable DUCK = new DataTable(); DataTable CUBE = new DataTable(); DataTable PolicyTerm = new DataTable(); DataTable BDM = new DataTable();
                    DataTable EDW = new DataTable(); DataTable EDWTax = new DataTable(); DataTable ADW = new DataTable(); DataTable ADWTax = new DataTable();
                    DataTable PolicyTotal = new DataTable(); DataTable SubmissionTotal = new DataTable();

                    XmlDocument xDoc_CUBE = new XmlDocument();
                    XmlDocument xDoc_PolicyTerm = new XmlDocument();
                    XmlDocument xDoc_BDM = new XmlDocument();
                    XmlDocument xDoc_EDW = new XmlDocument();
                    XmlDocument xDoc_EDWTax = new XmlDocument();
                    XmlDocument xDoc_ADW = new XmlDocument();
                    XmlDocument xDoc_ADWTax = new XmlDocument();
                    XmlDocument xDoc_SubmissionTotal = new XmlDocument();
                    #endregion

                    if (PolicyNumber.EndsWith("X"))
                    {
                        isPolicyTooBig = true;
                        PolicyNumber = PolicyNumber.TrimEnd('X').Trim();
                    }

                    // Check if policy is present in DUCK or Not.
                    XmlDocument loadPolicyRs = Common.LoadPolicyRq(DCTServerURL, PolicyNumber);
                    if (loadPolicyRs.SelectSingleNode("/server/responses/OnlineData.loadPolicyRs/@status").Value == Success) { isPolicyProcessingSystemIsDuck = true; }

                    // Check for ML policy.
                    if (Common.ValidateMLPolicySymbol(PolicyNumber) && isPolicyProcessingSystemIsDuck) { isMLPolicy = true; }

                    if (isPolicyProcessingSystemIsDuck) { DUCK = ExctractPolicyDataFromDuck(PolicyNumber, DCTServerURL); }
                    DUCK.TableName = "Duck";

					CUBE = ExctractPolicyDataFromCube(PolicyNumber, Server);
                    CUBE.TableName = "Cube";

					BDM = ExctractPolicyDataFromBDM(PolicyNumber, Server);
                    BDM.TableName = "BDM";

                    if (isMLPolicy == false)
                    {
						PolicyTerm = ExctractPolicyDataFromPolicyTerm(PolicyNumber, Server);
                        PolicyTerm.TableName = "Policy Term";

                        if (isPolicyTooBig == false && PolicyTerm.Rows.Count >= TransactionCountToConsiderAsBig) { isPolicyTooBig = true; }

                        if (isPolicyTooBig)
                        {
							EDW = ExctractPolicyDataFromEDWNonTaxForBigPolicy(PolicyNumber, Server);
							ADW = ExctractPolicyDataFromADWNonTaxForBigPolicy(PolicyNumber, Server);
							ADWTax = ExctractPolicyDataFromADWTaxForBigPolicy(PolicyNumber, Server);

                            // Temp.
							EDWTax = ExctractPolicyDataFromEDWTax(PolicyNumber, Server);
                        }
                        else
                        {
							EDW = ExctractPolicyDataFromEDWNonTax(PolicyNumber, Server);
							EDWTax = ExctractPolicyDataFromEDWTax(PolicyNumber, Server);
							ADW = ExctractPolicyDataFromADWNonTax(PolicyNumber, Server);
							ADWTax = ExctractPolicyDataFromADWTax(PolicyNumber, Server);
                        }
                    }

                    if (CUBE.Rows.Count > 0) { xDoc_CUBE = Common.ConvertDataTableToXML(CUBE); }
                    if (PolicyTerm.Rows.Count > 0) { xDoc_PolicyTerm = Common.ConvertDataTableToXML(PolicyTerm); }
                    if (BDM.Rows.Count > 0) { xDoc_BDM = Common.ConvertDataTableToXML(BDM); }
                    if (EDW.Rows.Count > 0) { xDoc_EDW = Common.ConvertDataTableToXML(EDW); }
                    if (EDWTax.Rows.Count > 0) { xDoc_EDWTax = Common.ConvertDataTableToXML(EDWTax); }
                    if (ADW.Rows.Count > 0) { xDoc_ADW = Common.ConvertDataTableToXML(ADW); }
                    if (ADWTax.Rows.Count > 0) { xDoc_ADWTax = Common.ConvertDataTableToXML(ADWTax); }

                    if (isMLPolicy)
                    {
                        SubmissionTotal = BuildSubmissionTotalForMLPolicy(xDoc_CUBE, xDoc_BDM);
                        PolicyTotal = BuildPolicyTotalForMLPolicy(DCTServerURL, PolicyNumber, isPolicyProcessingSystemIsDuck, xDoc_CUBE, xDoc_BDM);
                    }
                    else
                    {
                        if (isPolicyTooBig)
                        {
                            SubmissionTotal = BuildSubmissionTotalForBigPolicy(PolicyNumber, xDoc_CUBE, xDoc_PolicyTerm, xDoc_BDM, xDoc_EDW, xDoc_EDWTax, xDoc_ADW, xDoc_ADWTax);

                            if (SubmissionTotal.Rows.Count > 0) { xDoc_SubmissionTotal = Common.ConvertDataTableToXML(SubmissionTotal); }
                            PolicyTotal = BuildPolicyTotalForBigPolicy(PolicyNumber, isPolicyProcessingSystemIsDuck, DCTServerURL, xDoc_CUBE, xDoc_BDM, xDoc_SubmissionTotal);
                        }
                        else
                        {
                            SubmissionTotal = BuildSubmissionTotal(DCTServerURL, PolicyNumber, xDoc_CUBE, xDoc_PolicyTerm, xDoc_BDM, xDoc_EDW, xDoc_EDWTax, xDoc_ADW, xDoc_ADWTax);
                            PolicyTotal = BuildPolicyTotal(DCTServerURL, PolicyNumber, isPolicyProcessingSystemIsDuck, xDoc_CUBE, xDoc_BDM, xDoc_EDW, xDoc_EDWTax, xDoc_ADW, xDoc_ADWTax);
                        }
                    }

                    SubmissionTotal.TableName = "Submission Total";
                    PolicyTotal.TableName = "Policy Total";

                    if (isMLPolicy)
                    {
                        results.Tables.AddRange(new DataTable[] { PolicyTotal, SubmissionTotal, CUBE, DUCK, BDM });
                    }
                    else
                    {
                        if (EDWTax.Rows.Count == 0 && ADWTax.Rows.Count == 0)
                        {
                            results.Tables.AddRange(new DataTable[] { PolicyTotal, SubmissionTotal, CUBE, DUCK, PolicyTerm, BDM, EDW, ADW });
                        }
                        else { results.Tables.AddRange(new DataTable[] { PolicyTotal, SubmissionTotal, CUBE, DUCK, PolicyTerm, BDM, EDW, EDWTax, ADW, ADWTax }); }
                    }
                }

                return results;
            }

            internal static DataSet ExtractAndCompileData(string DCTServerURL, string Server, string PolicyNumber, string[] SubmissionIDs)
            {
                DataSet results = new DataSet();

                if (!string.IsNullOrEmpty(DCTServerURL) && !string.IsNullOrEmpty(Server) && !string.IsNullOrEmpty(PolicyNumber))
                {
                    #region Define Static Data
                    DataTable CUBE = new DataTable(); DataTable PolicyTerm = new DataTable(); DataTable BDM = new DataTable(); DataTable SubmissionTotal = new DataTable();
                    DataTable EDW = new DataTable(); DataTable EDWTax = new DataTable(); DataTable ADW = new DataTable(); DataTable ADWTax = new DataTable();
                    #endregion

                    #region Clean Submission List
                    string Submissions = string.Empty;
                    foreach (string Submission in SubmissionIDs) { Submissions += "'" + Submission.Trim() + "',"; }
                    Submissions = Submissions.TrimEnd(',');
                    #endregion

					CUBE = ExctractPolicyDataFromCube(PolicyNumber, Submissions, Server);
					CUBE.TableName = "Cube";

					PolicyTerm = ExctractPolicyDataFromPolicyTerm(PolicyNumber, Submissions, Server);
					PolicyTerm.TableName = "Policy Term";

					BDM = ExctractPolicyDataFromBDM(PolicyNumber, Submissions, Server);
					BDM.TableName = "BDM";

					EDW = ExctractPolicyDataFromEDWNonTax(PolicyNumber, Submissions, Server);
					EDW.TableName = "EDW";

					EDWTax = ExctractPolicyDataFromEDWTax(PolicyNumber, Submissions, Server);
					EDWTax.TableName = "EDW Tax";

					ADW = ExctractPolicyDataFromADWNonTax(PolicyNumber, Submissions, Server);
					ADW.TableName = "ADW";

					ADWTax = ExctractPolicyDataFromADWTax(PolicyNumber, Submissions, Server);
					ADWTax.TableName = "ADW Tax";

                    XmlDocument xDoc_CUBE = Common.ConvertDataTableToXML(CUBE);
                    XmlDocument xDoc_PolicyTerm = Common.ConvertDataTableToXML(PolicyTerm);
                    XmlDocument xDoc_BDM = Common.ConvertDataTableToXML(BDM);
                    XmlDocument xDoc_EDW = Common.ConvertDataTableToXML(EDW);
                    XmlDocument xDoc_EDWTax = Common.ConvertDataTableToXML(EDWTax);
                    XmlDocument xDoc_ADW = Common.ConvertDataTableToXML(ADW);
                    XmlDocument xDoc_ADWTax = Common.ConvertDataTableToXML(ADWTax);

                    SubmissionTotal = BuildSubmissionTotal(DCTServerURL, PolicyNumber, xDoc_CUBE, xDoc_PolicyTerm, xDoc_BDM, xDoc_EDW, xDoc_EDWTax, xDoc_ADW, xDoc_ADWTax, SubmissionIDs);
                    SubmissionTotal.TableName = "Submission Total";

                    if (EDWTax.Rows.Count == 0 && ADWTax.Rows.Count == 0)
                    {
                        results.Tables.AddRange(new DataTable[] { SubmissionTotal, CUBE, PolicyTerm, BDM, EDW, ADW });
                    }
                    else { results.Tables.AddRange(new DataTable[] { SubmissionTotal, CUBE, PolicyTerm, BDM, EDW, EDWTax, ADW, ADWTax }); }
                }

                return results;
            }

            internal static DataSet DownloadEDWAndADWData(string PolicyNumber, string server, string[] SubmissionIDs)
            {
                DataSet results = new DataSet();

                #region Clean Submission List
                string Submissions = string.Empty;
                foreach (string Submission in SubmissionIDs) { Submissions += "'" + Submission.Trim() + "',"; }
                Submissions = Submissions.TrimEnd(',');
                #endregion

                if (!string.IsNullOrEmpty(Submissions))
                {
					DataTable EDW = ExctractPolicyDataFromEDWNonTax(PolicyNumber, Submissions, server);
					EDW.TableName = "EDW";

					DataTable EDWTax = ExctractPolicyDataFromEDWTax(PolicyNumber, Submissions, server);
					EDWTax.TableName = "EDW Tax";

					DataTable ADW = ExctractPolicyDataFromADWNonTax(PolicyNumber, Submissions, server);
					ADW.TableName = "ADW";

					DataTable ADWTax = ExctractPolicyDataFromADWTax(PolicyNumber, Submissions, server);
					ADWTax.TableName = "ADW Tax";

                    XmlDocument xDoc_EDW = Common.ConvertDataTableToXML(EDW);
                    XmlDocument xDoc_EDWTax = Common.ConvertDataTableToXML(EDWTax);
                    XmlDocument xDoc_ADW = Common.ConvertDataTableToXML(ADW);
                    XmlDocument xDoc_ADWTax = Common.ConvertDataTableToXML(ADWTax);

                    DataTable SubmissionTotal = BuildEDWAndADWTotal(xDoc_EDW, xDoc_EDWTax, xDoc_ADW, xDoc_ADWTax, SubmissionIDs);
                    SubmissionTotal.TableName = "Submission Total";

                    if (EDWTax.Rows.Count == 0 && ADWTax.Rows.Count == 0) { results.Tables.AddRange(new DataTable[] { SubmissionTotal, EDW, ADW }); }
                    else { results.Tables.AddRange(new DataTable[] { SubmissionTotal, EDW, EDWTax, ADW, ADWTax }); }
                }

                return results;
            }

            internal static DataSet ExtractCoverageBreakdown(string policyNumber, string submission, string server)
            {
                DataSet results = new DataSet();
                string CoverageBD = "Coverage BD";

                if (!string.IsNullOrEmpty(submission))
                {
					DataTable _temp = BuildCoverageBreakDownForSubmission(policyNumber, submission, server);
                    _temp.TableName = CoverageBD;
                    results.Tables.AddRange(new DataTable[] { _temp });
                }
                else
                {
					DataTable _temp = BuildCoverageBreakDownForPolicy(policyNumber, server);
                    _temp.TableName = CoverageBD;
                    results.Tables.AddRange(new DataTable[] { _temp });
                }

                return results;
            }

            internal static DataSet ExtractTaxBreakdown(string policyNumber, string server)
            {
                DataSet results = new DataSet();

                DataTable _temp = new DataTable();

				DataTable EDW = ExctractPolicyDataFromEDWNonTax(policyNumber, server);
				DataTable ADW = ExctractPolicyDataFromADWNonTax(policyNumber, server);

                if (EDW.Rows.Count > 0 && ADW.Rows.Count > 0)
                {
                    XmlDocument xDoc_EDW = Common.ConvertDataTableToXML_New(EDW);
                    XmlDocument xDoc_ADW = Common.ConvertDataTableToXML_New(ADW);

                    if (xDoc_EDW.InnerXml !="" && xDoc_ADW.InnerXml != "")
                    {
                        List<string> Columns = new List<string> { "Submission" };
                        List<string> CHARGE_CD_List = xDoc_EDW.SelectNodes("/DocumentElement/EDW_x0020_Tax/CHARGE_CD").Cast<XmlNode>().Select(c => c.InnerText).Distinct().ToList();

                        foreach (string CHARGE_CD in CHARGE_CD_List) { Columns.Add(CHARGE_CD + " in EDW"); Columns.Add(CHARGE_CD + " in ADW"); }
                        foreach (string Col in Columns) { _temp.Columns.Add(Col, typeof(string)); }

                        HashSet<string> Submissions = GetSubmissionIDsFromPolicyTerm(policyNumber, server);

                        foreach (string Submission in Submissions)
                        {
                            List<string> rowData = new List<string> { Submission };
                            foreach (string CHARGE_CD in CHARGE_CD_List)
                            {
                                try
                                {
                                    string CHARGE_CD_Data_EDW = string.Empty; string CHARGE_CD_Data_ADW = string.Empty;
                                    CHARGE_CD_Data_EDW = Convert.ToString(xDoc_EDW.SelectSingleNode("/DocumentElement/EDW_x0020_Tax[PRCNONPREM_CATGCD='APRP'][CHARGE_CD='" + CHARGE_CD + "'][SUBM_ID='" + Submission + "']/NON_PREM_AMT").InnerXml);
                                    CHARGE_CD_Data_ADW = GetSumFromXMLDocument(xDoc_ADW.OuterXml, "sum(/DocumentElement/ADW_x0020_Tax[CHARGE_CD='" + CHARGE_CD + "'][SUBM_ID='" + Submission + "']/PREMIUM_CHANGE_AMOUNT)");
                                    rowData.Add(CHARGE_CD_Data_EDW);
                                    rowData.Add(CHARGE_CD_Data_ADW);
                                }
                                catch { }
                            }
                            _temp.Rows.Add(rowData.ToArray());
                        }
                    }                 
                }

                results.Tables.AddRange(new DataTable[] { _temp });

                return results;
            }

            internal static DataSet ExtractPolicyTermTable(string policyNumber, string server)
            {
				DataTable PolicyTerm = ExctractPolicyDataFromPolicyTerm(policyNumber, server);
                PolicyTerm.TableName = "Policy Term";

                DataSet results = new DataSet();
                if (PolicyTerm.Rows.Count > 0) { results.Tables.AddRange(new DataTable[] { PolicyTerm }); }

                return results;
            }

			internal static DataTable ValidateBDMPremiums(string policyNumber, string submission, string server)
			{
				return ExctractPolicyDataFromBDM(policyNumber, submission, server);
			}

            // Functions to fetch data.
            private static DataTable ExctractPolicyDataFromDuck(string PolicyNumber, string DCTServerURL)
            {
                DataTable result = new DataTable();

                StringBuilder listTransactionsRq = new StringBuilder();
                listTransactionsRq.Append("<OnlineData.loadPolicyRq policyNumber=\"" + PolicyNumber + "\" />");
                listTransactionsRq.Append("<TransACT.listTransactionsRq />");
                XmlDocument listTransactionsRs = Common.PostDCTRequestReturnResponse(listTransactionsRq.ToString(), DCTServerURL);

                string status = listTransactionsRs.SelectSingleNode("/server/responses/TransACT.listTransactionsRs/@status").Value;
                if (status == Success)
                {
                    string[] Columns = { "Submission", "History ID", "Term Premium", "Charge Premium", "Activity", "Status", "Transection Date" };
                    foreach (string Col in Columns) { result.Columns.Add(Col, typeof(string)); }

                    XmlNodeList transactions = listTransactionsRs.SelectNodes("/server/responses/TransACT.listTransactionsRs/transactions/transaction");
                    if (transactions.Count > 0)
                    {
                        for (int i = (transactions.Count - 1); i >= 0; i--)
                        {
                            string HistoryID = transactions[i].SelectSingleNode("HistoryID").InnerText;
                            string SubmissionID = transactions[i].SelectSingleNode("SubmissionID").InnerText;
                            string Type = transactions[i].SelectSingleNode("TypeCaption").InnerText;
                            string TransectionDate = transactions[i].SelectSingleNode("EffectiveDate").InnerText;
                            string TPremium = transactions[i].SelectSingleNode("TermPremium").InnerText;
                            string CPremium = transactions[i].SelectSingleNode("Charge").InnerText;
                            string Status = transactions[i].SelectSingleNode("Status").InnerText;
                            result.Rows.Add(SubmissionID, HistoryID, TPremium, CPremium, Type, Status, TransectionDate);
                        }
                    }
                }

                return result;
            }

			private static DataTable ExctractPolicyDataFromCube(string PolicyNumber, string Server)
            {
                string[] policyData = PolicyNumber.Split(' ');

                string SQLScript = GetSQLScript("FetchCubeManageActivityScreenDetails");
                SQLScript = SQLScript.Replace("&POL_NO_CD", policyData[1]).Replace("&POL_MODULE_CD", policyData[2]).Replace("&POL_VERSION_CD", policyData[3]);

                return Common.ExicuteOracleDBQuery(SQLScript, ConfigurationManager.ConnectionStrings[Common.GetConnectionStringName(Server).Split(':')[0]].ConnectionString);
            }

			private static DataTable ExctractPolicyDataFromCube(string PolicyNumber, string Submission, string Server)
            {
                string[] policyData = PolicyNumber.Split(' ');

                string SQLScript = GetSQLScript("FetchCubeManageActivityScreenDetailsForSubmission");
                SQLScript = SQLScript.Replace("&POL_NO_CD", policyData[1]).Replace("&POL_MODULE_CD", policyData[2]).Replace("&POL_VERSION_CD", policyData[3]).Replace("'&SUBMISSION_ID'", Submission);

				return Common.ExicuteOracleDBQuery(SQLScript, ConfigurationManager.ConnectionStrings[Common.GetConnectionStringName(Server).Split(':')[0]].ConnectionString);
            }

			private static DataTable ExctractPolicyDataFromPolicyTerm(string PolicyNumber, string Server)
			{
				string[] policyData = PolicyNumber.Split(' ');

                string SQLScript = GetSQLScript("PolicyTerm");
                SQLScript = SQLScript.Replace("&POL_NO_CD", policyData[1]).Replace("&POL_MODULE_CD", policyData[2]).Replace("&POL_VERSION_CD", policyData[3]);

				return Common.ExicuteOracleDBQuery(SQLScript, ConfigurationManager.ConnectionStrings[Common.GetConnectionStringName(Server).Split(':')[1]].ConnectionString);
			}

			private static DataTable ExctractPolicyDataFromPolicyTerm(string PolicyNumber, string Submission, string Server)
			{
				string[] policyData = PolicyNumber.Split(' ');

                string SQLScript = GetSQLScript("PolicyTerm") + " AND SUBM_ID IN (" + Submission + ")";
                SQLScript = SQLScript.Replace("&POL_NO_CD", policyData[1]).Replace("&POL_MODULE_CD", policyData[2]).Replace("&POL_VERSION_CD", policyData[3]);

				return Common.ExicuteOracleDBQuery(SQLScript, ConfigurationManager.ConnectionStrings[Common.GetConnectionStringName(Server).Split(':')[1]].ConnectionString);
			}

			private static DataTable ExctractPolicyDataFromBDM(string PolicyNumber, string Server)
			{
				string[] policyData = PolicyNumber.Split(' ');

                string SQLScript = GetSQLScript("BDM");
                SQLScript = SQLScript.Replace("&POL_NO_CD", policyData[1]).Replace("&POL_MODULE_CD", policyData[2]).Replace("&POL_VERSION_CD", policyData[3]);

				return Common.ExicuteOracleDBQuery(SQLScript, ConfigurationManager.ConnectionStrings[Common.GetConnectionStringName(Server).Split(':')[1]].ConnectionString);
			}

			private static DataTable ExctractPolicyDataFromBDM(string PolicyNumber, string Submission, string Server)
			{
				string[] policyData = PolicyNumber.Split(' ');

                string SQLScript = GetSQLScript("BDMBySubmission");
                SQLScript = SQLScript.Replace("&POL_NO_CD", policyData[1]).Replace("&POL_MODULE_CD", policyData[2]).Replace("&POL_VERSION_CD", policyData[3]).Replace("'&SOURCE_SUBMISSION_ID'", Submission);

				return Common.ExicuteOracleDBQuery(SQLScript, ConfigurationManager.ConnectionStrings[Common.GetConnectionStringName(Server).Split(':')[1]].ConnectionString);
			}

			private static DataTable ExctractPolicyDataFromEDWNonTax(string PolicyNumber, string Server)
			{
				string[] policyData = PolicyNumber.Split(' ');

                string SQLScript = GetSQLScript("EDWNonTax");
                SQLScript = SQLScript.Replace("&POL_NO_CD", policyData[1]).Replace("&POL_MODULE_CD", policyData[2]).Replace("&POL_VERSION_CD", policyData[3]);

				DataTable result = Common.ExicuteOracleDBQuery(SQLScript, ConfigurationManager.ConnectionStrings[Common.GetConnectionStringName(Server).Split(':')[1]].ConnectionString);
				result.TableName = "EDW";

                return result;
            }

			private static DataTable ExctractPolicyDataFromEDWNonTax(string PolicyNumber, string Submission, string Server)
			{
				string[] policyData = PolicyNumber.Split(' ');

                string SQLScript = GetSQLScript("EDWNonTaxForSubmission");
                SQLScript = SQLScript.Replace("&POL_NO_CD", policyData[1]).Replace("&POL_MODULE_CD", policyData[2]).Replace("&POL_VERSION_CD", policyData[3]).Replace("'&SUBM_ID'", Submission);

				DataTable result = Common.ExicuteOracleDBQuery(SQLScript, ConfigurationManager.ConnectionStrings[Common.GetConnectionStringName(Server).Split(':')[1]].ConnectionString);
				result.TableName = "EDW";

                return result;
            }

			private static DataTable ExctractPolicyDataFromEDWTax(string PolicyNumber, string Server)
			{
				string[] policyData = PolicyNumber.Split(' ');

                string SQLScript = GetSQLScript("EDWTax");
                SQLScript = SQLScript.Replace("&POL_NO_CD", policyData[1]).Replace("&POL_MODULE_CD", policyData[2]).Replace("&POL_VERSION_CD", policyData[3]);

				DataTable result = Common.ExicuteOracleDBQuery(SQLScript, ConfigurationManager.ConnectionStrings[Common.GetConnectionStringName(Server).Split(':')[1]].ConnectionString);
				result.TableName = "EDW Tax";

                return result;
            }

			private static DataTable ExctractPolicyDataFromEDWTax(string PolicyNumber, string Submission, string Server)
			{
				string[] policyData = PolicyNumber.Split(' ');

                string SQLScript = GetSQLScript("EDWTaxForSubmission");
                SQLScript = SQLScript.Replace("&POL_NO_CD", policyData[1]).Replace("&POL_MODULE_CD", policyData[2]).Replace("&POL_VERSION_CD", policyData[3]).Replace("'&SUBM_ID'", Submission);

				DataTable result = Common.ExicuteOracleDBQuery(SQLScript, ConfigurationManager.ConnectionStrings[Common.GetConnectionStringName(Server).Split(':')[1]].ConnectionString);
				result.TableName = "EDW Tax";

                return result;
            }

			private static DataTable ExctractPolicyDataFromADWNonTax(string PolicyNumber, string Server)
			{
				string[] policyData = PolicyNumber.Split(' ');

                string SQLScript = GetSQLScript("ADWNonTax");
                SQLScript = SQLScript.Replace("&POL_NO_CD", policyData[1]).Replace("&POL_MODULE_CD", policyData[2]).Replace("&POL_VERSION_CD", policyData[3]);

				DataTable result = Common.ExicuteOracleDBQuery(SQLScript, ConfigurationManager.ConnectionStrings[Common.GetConnectionStringName(Server).Split(':')[1]].ConnectionString);
				result.TableName = "ADW";

                return result;
            }

			private static DataTable ExctractPolicyDataFromADWNonTax(string PolicyNumber, string Submission, string Server)
			{
				string[] policyData = PolicyNumber.Split(' ');

                string SQLScript = GetSQLScript("ADWNonTaxForSubmission");
                SQLScript = SQLScript.Replace("&POL_NO_CD", policyData[1]).Replace("&POL_MODULE_CD", policyData[2]).Replace("&POL_VERSION_CD", policyData[3]).Replace("'&SUBM_ID'", Submission);

				DataTable result = Common.ExicuteOracleDBQuery(SQLScript, ConfigurationManager.ConnectionStrings[Common.GetConnectionStringName(Server).Split(':')[1]].ConnectionString);
				result.TableName = "ADW";

                return result;
            }

			private static DataTable ExctractPolicyDataFromADWTax(string PolicyNumber, string Server)
			{
				string[] policyData = PolicyNumber.Split(' ');

                string SQLScript = GetSQLScript("ADWTax");
                SQLScript = SQLScript.Replace("&POL_NO_CD", policyData[1]).Replace("&POL_MODULE_CD", policyData[2]).Replace("&POL_VERSION_CD", policyData[3]);

				DataTable result = Common.ExicuteOracleDBQuery(SQLScript, ConfigurationManager.ConnectionStrings[Common.GetConnectionStringName(Server).Split(':')[1]].ConnectionString);
				result.TableName = "ADW Tax";

                return result;
            }

			private static DataTable ExctractPolicyDataFromADWTax(string PolicyNumber, string Submission, string Server)
			{
				string[] policyData = PolicyNumber.Split(' ');

                string SQLScript = GetSQLScript("ADWTaxForSubmission");
                SQLScript = SQLScript.Replace("&POL_NO_CD", policyData[1]).Replace("&POL_MODULE_CD", policyData[2]).Replace("&POL_VERSION_CD", policyData[3]).Replace("'&SUBM_ID'", Submission);

				DataTable result = Common.ExicuteOracleDBQuery(SQLScript, ConfigurationManager.ConnectionStrings[Common.GetConnectionStringName(Server).Split(':')[1]].ConnectionString);
				result.TableName = "ADW Tax";

                return result;
            }

            // Functions related to Policy Data Fetch
            private static DataTable BuildSubmissionTotal(string DCTServerURL, string PolicyNumber, XmlDocument xDoc_CUBE, XmlDocument xDoc_PolicyTerm, XmlDocument xDoc_BDM, XmlDocument xDoc_EDW, XmlDocument xDoc_EDWTax, XmlDocument xDoc_ADW, XmlDocument xDoc_ADWTax, string[] SubmissionIDs = null)
            {
                DataTable result = new DataTable();
                HashSet<string> submissionIDs = new HashSet<string>();

                string[] Columns = { "Submission", "Cube", "BDM Non Tax", "BDM  Tax", "EDW Non Tax", "EDW  Tax", "EDW Waive Premium", "ADW Non Tax", "ADW  Tax", "ADW Non Tax Comm.", "ADW Tax Comm." };
                foreach (string Col in Columns) { result.Columns.Add(Col, typeof(string)); }

                if (SubmissionIDs != null) { if (SubmissionIDs.Length > 0) { submissionIDs = SubmissionIDs.ToHashSet(); } }
                else
                {
                    foreach (XmlElement xmlElement in xDoc_BDM.SelectNodes("/DocumentElement/BDM/SOURCE_SUBMISSION_ID")) { submissionIDs.Add(xmlElement.InnerText); }
                    foreach (XmlElement xmlElement in xDoc_PolicyTerm.SelectNodes("/DocumentElement/Policy_x0020_Term/SUBM_ID")) { submissionIDs.Add(xmlElement.InnerText); }
                }

                foreach (var Submission in submissionIDs)
                {
                    try
                    {
                        string CubePremiumPerSubmission = string.Empty;
                        if (!string.IsNullOrEmpty(xDoc_CUBE.OuterXml)) { CubePremiumPerSubmission = GetSumFromXMLDocument(xDoc_CUBE.OuterXml, "sum(/DocumentElement/Cube[SUBMISSION_ID='" + Submission + "']/CUBEAPRP)"); }

                        string BDMNonTaxPremiumPerSubmission = string.Empty; string BDMTaxPremiumPerSubmission = string.Empty;
                        if (!string.IsNullOrEmpty(xDoc_BDM.OuterXml))
                        {
                            if (xDoc_BDM.SelectNodes("/DocumentElement/BDM[SOURCE_SUBMISSION_ID='" + Submission + "']/TOT_APRP_PRM_EXCLD_TX_SRCHRG").Count > 0)
                            {
                                BDMNonTaxPremiumPerSubmission = GetSumFromXMLDocument(xDoc_BDM.OuterXml, "sum(/DocumentElement/BDM[SOURCE_SUBMISSION_ID='" + Submission + "']/TOT_APRP_PRM_EXCLD_TX_SRCHRG)");
                                BDMTaxPremiumPerSubmission = GetSumFromXMLDocument(xDoc_BDM.OuterXml, "sum(/DocumentElement/BDM[SOURCE_SUBMISSION_ID='" + Submission + "']/TOT_APRP_PREM_TX_SRCHRG_AMT)");
                            }
                        }

                        string EDWNonTaxPremiumPerSubmission = string.Empty; string EDWTaxPremiumPerSubmission = string.Empty; string EDWWaivedPremiumPerSubmission = string.Empty;
                        if (xDoc_EDW.SelectNodes("/DocumentElement/EDW[SUBM_ID='" + Submission + "'][PRCPREM_CATGCD='APRP'][CVRG_CD!='WAIVEPREM']/PREM_AMT").Count > 0)
                        {
                            EDWNonTaxPremiumPerSubmission = GetSumFromXMLDocument(xDoc_EDW.OuterXml, "sum(/DocumentElement/EDW[SUBM_ID='" + Submission + "'][PRCPREM_CATGCD='APRP'][CVRG_CD!='WAIVEPREM']/PREM_AMT)");
                            EDWTaxPremiumPerSubmission = GetSumFromXMLDocument(xDoc_EDWTax.OuterXml, "sum(/DocumentElement/EDW_x0020_Tax[SUBM_ID='" + Submission + "'][PRCNONPREM_CATGCD='APRP']/NON_PREM_AMT)");
                            EDWWaivedPremiumPerSubmission = GetSumFromXMLDocument(xDoc_EDW.OuterXml, "sum(/DocumentElement/EDW[SUBM_ID='" + Submission + "'][PRCPREM_CATGCD='APRP'][CVRG_CD='WAIVEPREM']/PREM_AMT)");
                        }

                        string ADWNonTaxPremiumPerSubmission = string.Empty; string ADWTaxPremiumPerSubmission = string.Empty; string ADWNonTaxComissionPerSubmission = string.Empty; string ADWTaxComissionPerSubmission = string.Empty;
                        if (xDoc_ADW.SelectNodes("/DocumentElement/ADW[SUBM_ID='" + Submission + "']/PREMIUM_CHANGE_AMOUNT").Count > 0)
                        {
                            ADWNonTaxPremiumPerSubmission = GetSumFromXMLDocument(xDoc_ADW.OuterXml, "sum(/DocumentElement/ADW[SUBM_ID='" + Submission + "']/PREMIUM_CHANGE_AMOUNT)");
                            ADWTaxPremiumPerSubmission = GetSumFromXMLDocument(xDoc_ADWTax.OuterXml, "sum(/DocumentElement/ADW_x0020_Tax[SUBM_ID='" + Submission + "']/PREMIUM_CHANGE_AMOUNT)");
                            ADWNonTaxComissionPerSubmission = GetSumFromXMLDocument(xDoc_ADW.OuterXml, "sum(/DocumentElement/ADW[SUBM_ID='" + Submission + "']/COMMISS_AMT)");
                            ADWTaxComissionPerSubmission = GetSumFromXMLDocument(xDoc_ADWTax.OuterXml, "sum(/DocumentElement/ADW_x0020_Tax[SUBM_ID='" + Submission + "']/COMMISS_AMT)");

                        }

                        result.Rows.Add(Submission, CubePremiumPerSubmission, BDMNonTaxPremiumPerSubmission, BDMTaxPremiumPerSubmission, EDWNonTaxPremiumPerSubmission, EDWTaxPremiumPerSubmission, EDWWaivedPremiumPerSubmission, ADWNonTaxPremiumPerSubmission, ADWTaxPremiumPerSubmission, ADWNonTaxComissionPerSubmission, ADWTaxComissionPerSubmission);
                    }
                    catch { }
                }

                return result;
            }

            private static DataTable BuildEDWAndADWTotal(XmlDocument xDoc_EDW, XmlDocument xDoc_EDWTax, XmlDocument xDoc_ADW, XmlDocument xDoc_ADWTax, string[] SubmissionIDs)
            {
                DataTable result = new DataTable();
                HashSet<string> submissionIDs = new HashSet<string>();

                string[] Columns = { "Submission", "EDW Non Tax", "EDW  Tax", "EDW Waive Premium", "ADW Non Tax", "ADW  Tax", "ADW Non Tax Comm.", "ADW Tax Comm." };
                foreach (string Col in Columns) { result.Columns.Add(Col, typeof(string)); }

                if (SubmissionIDs.Length > 0) { submissionIDs = SubmissionIDs.ToHashSet(); }

                foreach (var Submission in submissionIDs)
                {
                    try
                    {
                        string EDWNonTaxPremiumPerSubmission = string.Empty; string EDWTaxPremiumPerSubmission = string.Empty; string EDWWaivedPremiumPerSubmission = string.Empty;
                        if (xDoc_EDW.SelectNodes("/DocumentElement/EDW[SUBM_ID='" + Submission + "'][PRCPREM_CATGCD='APRP'][CVRG_CD!='WAIVEPREM']/PREM_AMT").Count > 0)
                        {
                            EDWNonTaxPremiumPerSubmission = GetSumFromXMLDocument(xDoc_EDW.OuterXml, "sum(/DocumentElement/EDW[SUBM_ID='" + Submission + "'][PRCPREM_CATGCD='APRP'][CVRG_CD!='WAIVEPREM']/PREM_AMT)");
                            EDWTaxPremiumPerSubmission = GetSumFromXMLDocument(xDoc_EDWTax.OuterXml, "sum(/DocumentElement/EDW_x0020_Tax[SUBM_ID='" + Submission + "'][PRCNONPREM_CATGCD='APRP']/NON_PREM_AMT)");
                            EDWWaivedPremiumPerSubmission = GetSumFromXMLDocument(xDoc_EDW.OuterXml, "sum(/DocumentElement/EDW[SUBM_ID='" + Submission + "'][PRCPREM_CATGCD='APRP'][CVRG_CD='WAIVEPREM']/PREM_AMT)");
                        }

                        string ADWNonTaxPremiumPerSubmission = string.Empty; string ADWTaxPremiumPerSubmission = string.Empty; string ADWNonTaxComissionPerSubmission = string.Empty; string ADWTaxComissionPerSubmission = string.Empty;
                        if (xDoc_ADW.SelectNodes("/DocumentElement/ADW[SUBM_ID='" + Submission + "']/PREMIUM_CHANGE_AMOUNT").Count > 0)
                        {
                            ADWNonTaxPremiumPerSubmission = GetSumFromXMLDocument(xDoc_ADW.OuterXml, "sum(/DocumentElement/ADW[SUBM_ID='" + Submission + "']/PREMIUM_CHANGE_AMOUNT)");
                            ADWTaxPremiumPerSubmission = GetSumFromXMLDocument(xDoc_ADWTax.OuterXml, "sum(/DocumentElement/ADW_x0020_Tax[SUBM_ID='" + Submission + "']/PREMIUM_CHANGE_AMOUNT)");
                            ADWNonTaxComissionPerSubmission = GetSumFromXMLDocument(xDoc_ADW.OuterXml, "sum(/DocumentElement/ADW[SUBM_ID='" + Submission + "']/COMMISS_AMT)");
                            ADWTaxComissionPerSubmission = GetSumFromXMLDocument(xDoc_ADWTax.OuterXml, "sum(/DocumentElement/ADW_x0020_Tax[SUBM_ID='" + Submission + "']/COMMISS_AMT)");

                        }

                        result.Rows.Add(Submission, EDWNonTaxPremiumPerSubmission, EDWTaxPremiumPerSubmission, EDWWaivedPremiumPerSubmission, ADWNonTaxPremiumPerSubmission, ADWTaxPremiumPerSubmission, ADWNonTaxComissionPerSubmission, ADWTaxComissionPerSubmission);
                    }
                    catch { }
                }

                return result;
            }

            private static DataTable BuildPolicyTotal(string DCTServerURL, string PolicyNumber, bool isPolicyProcessingSystemIsDuck, XmlDocument xDoc_CUBE, XmlDocument xDoc_BDM, XmlDocument xDoc_EDW, XmlDocument xDoc_EDWTax, XmlDocument xDoc_ADW, XmlDocument xDoc_ADWTax)
            {
                DataTable result = new DataTable();

                try
                {
                    string[] Columns = { "Policy", "Cube", "Duck Non Tax", "Duck Tax", "BDM Non Tax", "BDM  Tax", "EDW Non Tax", "EDW  Tax", "ADW Non Tax", "ADW  Tax", "ADW Non Tax Comm.", "ADW Tax Comm." };
                    foreach (string Col in Columns) { result.Columns.Add(Col, typeof(string)); }

                    // Cube
                    string CubeAPRPPremiumSum = string.Empty;
                    CubeAPRPPremiumSum = GetSumFromXMLDocument(xDoc_CUBE.OuterXml, "sum(/DocumentElement/Cube/CUBEAPRP)");

                    // Duck
                    string PremiumWrittenNonTax = "NA"; string TaxesSurchargesValues = "NA"; string DuckPremiumWritten = "NA";

                    if (isPolicyProcessingSystemIsDuck)
                    {
                        DuckPremiumWritten = GetValueFromPolicy(DCTServerURL, PolicyNumber, "//data/policy/PremiumWritten");
                        TaxesSurchargesValues = GetValueFromPolicy(DCTServerURL, PolicyNumber, "//data/policy/line/TaxesSurchargesValues/@written");
                        if (!string.IsNullOrEmpty(DuckPremiumWritten) && !string.IsNullOrEmpty(TaxesSurchargesValues))
                        {
                            float nonTaxPremium = float.Parse(DuckPremiumWritten) - float.Parse(TaxesSurchargesValues);
                            PremiumWrittenNonTax = Convert.ToString(nonTaxPremium);
                        }
                    }

                    // BDM
                    string BDMNonTaxTotal = string.Empty; string BDMTaxTotal = string.Empty;
                    BDMNonTaxTotal = GetSumFromXMLDocument(xDoc_BDM.OuterXml, "sum(/DocumentElement/BDM/TOT_APRP_PRM_EXCLD_TX_SRCHRG)");
                    BDMTaxTotal = GetSumFromXMLDocument(xDoc_BDM.OuterXml, "sum(/DocumentElement/BDM/TOT_APRP_PREM_TX_SRCHRG_AMT)");

                    // EDW
                    string EDWNonTaxTotal = string.Empty; string EDWTaxTotal = string.Empty;
                    EDWNonTaxTotal = GetSumFromXMLDocument(xDoc_EDW.OuterXml, "sum(/DocumentElement/EDW[PRCPREM_CATGCD='APRP']/PREM_AMT)");
                    EDWTaxTotal = GetSumFromXMLDocument(xDoc_EDWTax.OuterXml, "sum(/DocumentElement/EDW_x0020_Tax[PRCNONPREM_CATGCD='APRP']/NON_PREM_AMT)");

                    // ADW
                    string ADWNonTaxTotal = string.Empty; string ADWTaxTotal = string.Empty; string ADWNonTaxComission = string.Empty; string ADWTaxComission = string.Empty;
                    ADWNonTaxTotal = GetSumFromXMLDocument(xDoc_ADW.OuterXml, "sum(/DocumentElement/ADW/PREMIUM_CHANGE_AMOUNT)");
                    ADWTaxTotal = GetSumFromXMLDocument(xDoc_ADWTax.OuterXml, "sum(/DocumentElement/ADW_x0020_Tax/PREMIUM_CHANGE_AMOUNT)");
                    ADWNonTaxComission = GetSumFromXMLDocument(xDoc_ADW.OuterXml, "sum(/DocumentElement/ADW/COMMISS_AMT)");
                    ADWTaxComission = GetSumFromXMLDocument(xDoc_ADWTax.OuterXml, "sum(/DocumentElement/ADW_x0020_Tax/COMMISS_AMT)");

                    // Policy Total
                    string BDMPolicyTotal = string.Empty; string EDWPolicyTotal = string.Empty; string ADWPolicyTotal = string.Empty; string EDWComissionTotal = string.Empty;
                    try
                    {
                        BDMPolicyTotal = Convert.ToString(float.Parse(BDMNonTaxTotal) + float.Parse(BDMTaxTotal));
                        EDWPolicyTotal = Convert.ToString(float.Parse(EDWNonTaxTotal) + float.Parse(EDWTaxTotal));
                        ADWPolicyTotal = Convert.ToString(float.Parse(ADWNonTaxTotal) + float.Parse(ADWTaxTotal));
                        EDWComissionTotal = Convert.ToString(float.Parse(ADWNonTaxComission) + float.Parse(ADWTaxComission));
                    }
                    catch { }

                    result.Rows.Add(PolicyNumber, CubeAPRPPremiumSum, PremiumWrittenNonTax, TaxesSurchargesValues, BDMNonTaxTotal, BDMTaxTotal, EDWNonTaxTotal, EDWTaxTotal, ADWNonTaxTotal, ADWTaxTotal, ADWNonTaxComission, ADWTaxComission);
                    result.Rows.Add("", CubeAPRPPremiumSum, "", DuckPremiumWritten, "", BDMPolicyTotal, "", EDWPolicyTotal, "", ADWPolicyTotal, "", EDWComissionTotal);
                }
                catch { }

                return result;
            }

            // Functions related to Big Policy Data Fetch
			private static DataTable ExctractPolicyDataFromEDWNonTaxForBigPolicy(string PolicyNumber, string Server)
            {
                DataTable result = new DataTable();
                string[] policyData = PolicyNumber.Split(' ');

                string SQLScript = GetSQLScript("EDWNonTaxForBigPolicy");
                SQLScript = SQLScript.Replace("&POL_NO_CD", policyData[1]).Replace("&POL_MODULE_CD", policyData[2]).Replace("&POL_VERSION_CD", policyData[3]);

				result = Common.ExicuteOracleDBQuery(SQLScript, ConfigurationManager.ConnectionStrings[Common.GetConnectionStringName(Server).Split(':')[1]].ConnectionString);
				result.TableName = "EDW";

                return result;
            }

			private static DataTable ExctractPolicyDataFromADWNonTaxForBigPolicy(string PolicyNumber, string Server)
			{
				DataTable result = new DataTable();
				string[] policyData = PolicyNumber.Split(' ');

                string SQLScript = GetSQLScript("ADWNonTaxForBigPolicy");
                SQLScript = SQLScript.Replace("&POL_NO_CD", policyData[1]).Replace("&POL_MODULE_CD", policyData[2]).Replace("&POL_VERSION_CD", policyData[3]);

				result = Common.ExicuteOracleDBQuery(SQLScript, ConfigurationManager.ConnectionStrings[Common.GetConnectionStringName(Server).Split(':')[1]].ConnectionString);
				result.TableName = "ADW";

                return result;
            }

			private static DataTable ExctractPolicyDataFromADWTaxForBigPolicy(string PolicyNumber, string Server)
			{
				DataTable result = new DataTable();
				string[] policyData = PolicyNumber.Split(' ');

                string SQLScript = GetSQLScript("ADWTaxForBigPolicy");
                SQLScript = SQLScript.Replace("&POL_NO_CD", policyData[1]).Replace("&POL_MODULE_CD", policyData[2]).Replace("&POL_VERSION_CD", policyData[3]);

				result = Common.ExicuteOracleDBQuery(SQLScript, ConfigurationManager.ConnectionStrings[Common.GetConnectionStringName(Server).Split(':')[1]].ConnectionString);
				result.TableName = "ADW Tax";

                return result;
            }

            private static DataTable BuildSubmissionTotalForBigPolicy(string PolicyNumber, XmlDocument xDoc_CUBE, XmlDocument xDoc_PolicyTerm, XmlDocument xDoc_BDM, XmlDocument xDoc_EDW, XmlDocument xDoc_EDWTax, XmlDocument xDoc_ADW, XmlDocument xDoc_ADWTax)
            {
                DataTable result = new DataTable();
                HashSet<string> submissionIDs = new HashSet<string>();
                string[] policyData = PolicyNumber.Split(' ');

                string[] Columns = { "Submission", "Cube", "BDM Non Tax", "BDM  Tax", "EDW Non Tax", "EDW  Tax", "ADW Non Tax", "ADW  Tax" };
                foreach (string Col in Columns) { result.Columns.Add(Col, typeof(string)); }

                foreach (XmlElement xmlElement in xDoc_BDM.SelectNodes("/DocumentElement/BDM/SOURCE_SUBMISSION_ID")) { submissionIDs.Add(xmlElement.InnerText); }
                foreach (XmlElement xmlElement in xDoc_PolicyTerm.SelectNodes("/DocumentElement/Policy_x0020_Term/SUBM_ID")) { submissionIDs.Add(xmlElement.InnerText); }

                foreach (var Submission in submissionIDs)
                {
                    try
                    {
                        string CubePremiumPerSubmission = string.Empty;
                        if (!string.IsNullOrEmpty(xDoc_CUBE.OuterXml))
                        {
                            if (xDoc_CUBE.SelectNodes("/DocumentElement/Cube[SUBMISSION_ID='" + Submission + "']/CUBEAPRP").Count > 0)
                            {
                                CubePremiumPerSubmission = GetSumFromXMLDocument(xDoc_CUBE.OuterXml, "sum(/DocumentElement/Cube[SUBMISSION_ID='" + Submission + "']/CUBEAPRP)");
                            }
                        }

                        string BDMNonTaxPremiumPerSubmission = string.Empty; string BDMTaxPremiumPerSubmission = string.Empty;
                        if (!string.IsNullOrEmpty(xDoc_BDM.OuterXml))
                        {
                            if (xDoc_BDM.SelectNodes("/DocumentElement/BDM[SOURCE_SUBMISSION_ID='" + Submission + "']/TOT_APRP_PRM_EXCLD_TX_SRCHRG").Count > 0)
                            {
                                BDMNonTaxPremiumPerSubmission = GetSumFromXMLDocument(xDoc_BDM.OuterXml, "sum(/DocumentElement/BDM[SOURCE_SUBMISSION_ID='" + Submission + "']/TOT_APRP_PRM_EXCLD_TX_SRCHRG)");
                                BDMTaxPremiumPerSubmission = GetSumFromXMLDocument(xDoc_BDM.OuterXml, "sum(/DocumentElement/BDM[SOURCE_SUBMISSION_ID='" + Submission + "']/TOT_APRP_PREM_TX_SRCHRG_AMT)");
                            }
                        }

                        string EDWNonTaxPremiumPerSubmission = string.Empty;
                        if (!string.IsNullOrEmpty(xDoc_EDW.OuterXml))
                        {
                            if (xDoc_EDW.SelectNodes("/DocumentElement/EDW[SUBM_ID='" + Submission + "']/PREM_AMT").Count > 0)
                            {
                                EDWNonTaxPremiumPerSubmission = GetSumFromXMLDocument(xDoc_EDW.OuterXml, "sum(/DocumentElement/EDW[SUBM_ID='" + Submission + "']/PREM_AMT)");
                            }
                        }

                        string EDWTaxPremiumPerSubmission = "";
                        if (!string.IsNullOrEmpty(xDoc_EDWTax.OuterXml))
                        {
                            if (xDoc_EDWTax.SelectNodes("/DocumentElement/EDW_x0020_Tax[SUBM_ID='" + Submission + "'][PRCNONPREM_CATGCD='APRP']/NON_PREM_AMT").Count > 0)
                            {
                                EDWTaxPremiumPerSubmission = GetSumFromXMLDocument(xDoc_EDWTax.OuterXml, "sum(/DocumentElement/EDW_x0020_Tax[SUBM_ID='" + Submission + "'][PRCNONPREM_CATGCD='APRP']/NON_PREM_AMT)");
                            }
                        }

                        string ADWNonTaxPremiumPerSubmission = string.Empty;
                        if (!string.IsNullOrEmpty(xDoc_ADW.OuterXml))
                        {
                            if (xDoc_ADW.SelectNodes("/DocumentElement/ADW[SUBM_ID='" + Submission + "']/PREMIUM_CHANGE_AMOUNT").Count > 0)
                            {
                                ADWNonTaxPremiumPerSubmission = GetSumFromXMLDocument(xDoc_ADW.OuterXml, "sum(/DocumentElement/ADW[SUBM_ID='" + Submission + "']/PREMIUM_CHANGE_AMOUNT)");
                            }
                        }

                        string ADWTaxPremiumPerSubmission = string.Empty;
                        if (!string.IsNullOrEmpty(xDoc_ADWTax.OuterXml))
                        {
                            if (xDoc_ADWTax.SelectNodes("/DocumentElement/ADW_x0020_Tax[SUBM_ID='" + Submission + "']/PREMIUM_CHANGE_AMOUNT").Count > 0)
                            {
                                ADWTaxPremiumPerSubmission = GetSumFromXMLDocument(xDoc_ADWTax.OuterXml, "sum(/DocumentElement/ADW_x0020_Tax[SUBM_ID='" + Submission + "']/PREMIUM_CHANGE_AMOUNT)");
                            }
                        }

                        result.Rows.Add(Submission, CubePremiumPerSubmission, BDMNonTaxPremiumPerSubmission, BDMTaxPremiumPerSubmission, EDWNonTaxPremiumPerSubmission, EDWTaxPremiumPerSubmission, ADWNonTaxPremiumPerSubmission, ADWTaxPremiumPerSubmission);
                    }
                    catch { }
                }

                result.TableName = "Submission Total";
                return result;
            }

            private static DataTable BuildPolicyTotalForBigPolicy(string PolicyNumber, bool isPolicyProcessingSystemIsDuck, string DCTServerURL, XmlDocument xDoc_CUBE, XmlDocument xDoc_BDM, XmlDocument xDoc_SubmissionTotal)
            {
                DataTable result = new DataTable();

                try
                {
                    string[] Columns = { "Policy", "Cube", "Duck Non Tax", "Duck Tax", "BDM Non Tax", "BDM  Tax", "EDW Non Tax", "EDW  Tax", "ADW Non Tax", "ADW  Tax" };
                    foreach (string Col in Columns) { result.Columns.Add(Col, typeof(string)); }

                    // Cube
                    string CubeAPRPPremiumSum = string.Empty;
                    CubeAPRPPremiumSum = GetSumFromXMLDocument(xDoc_CUBE.OuterXml, "sum(/DocumentElement/Cube/CUBEAPRP)");

                    // Duck
                    string PremiumWrittenNonTax = "NA"; string TaxesSurchargesValues = "NA"; string DuckPremiumWritten = "NA";
                    if (isPolicyProcessingSystemIsDuck)
                    {
                        DuckPremiumWritten = GetValueFromPolicy(DCTServerURL, PolicyNumber, "//data/policy/PremiumWritten");
                        TaxesSurchargesValues = GetValueFromPolicy(DCTServerURL, PolicyNumber, "//data/policy/line/TaxesSurchargesValues/@written");
                        if (!string.IsNullOrEmpty(DuckPremiumWritten) && !string.IsNullOrEmpty(TaxesSurchargesValues))
                        {
                            float nonTaxPremium = float.Parse(DuckPremiumWritten) - float.Parse(TaxesSurchargesValues);
                            PremiumWrittenNonTax = Convert.ToString(nonTaxPremium);
                        }
                    }

                    // BDM
                    string BDMNonTaxTotal = string.Empty; string BDMTaxTotal = string.Empty;
                    BDMNonTaxTotal = GetSumFromXMLDocument(xDoc_BDM.OuterXml, "sum(/DocumentElement/BDM/TOT_APRP_PRM_EXCLD_TX_SRCHRG)");
                    BDMTaxTotal = GetSumFromXMLDocument(xDoc_BDM.OuterXml, "sum(/DocumentElement/BDM/TOT_APRP_PREM_TX_SRCHRG_AMT)");

                    // EDW
                    string EDWNonTaxTotal = string.Empty; string EDWTaxTotal = string.Empty;
                    EDWNonTaxTotal = GetSumFromXMLDocument(xDoc_SubmissionTotal.OuterXml, "sum(/DocumentElement/Submission_x0020_Total[EDW_x0020_Non_x0020_Tax !='']/EDW_x0020_Non_x0020_Tax)");

                    //ADW
                    string ADWNonTaxTotal = string.Empty; string ADWTaxTotal = string.Empty; string ADWNonTaxComission = string.Empty; string ADWTaxComission = string.Empty;
                    ADWNonTaxTotal = GetSumFromXMLDocument(xDoc_SubmissionTotal.OuterXml, "sum(/DocumentElement/Submission_x0020_Total[ADW_x0020_Non_x0020_Tax !='']/ADW_x0020_Non_x0020_Tax)");
                    ADWTaxTotal = GetSumFromXMLDocument(xDoc_SubmissionTotal.OuterXml, "sum(/DocumentElement/Submission_x0020_Total[ADW_x0020__x0020_Tax !='']/ADW_x0020__x0020_Tax)");

                    // Policy Total
                    string BDMPolicyTotal = string.Empty; string EDWPolicyTotal = string.Empty; string ADWPolicyTotal = string.Empty; string EDWComissionTotal = string.Empty;
                    try
                    {
                        BDMPolicyTotal = Convert.ToString(float.Parse(BDMNonTaxTotal) + float.Parse(BDMTaxTotal));
                        EDWPolicyTotal = Convert.ToString(float.Parse(EDWNonTaxTotal) + 0);
                        ADWPolicyTotal = Convert.ToString(float.Parse(ADWNonTaxTotal) + float.Parse(ADWTaxTotal));
                    }
                    catch { }

                    result.Rows.Add(PolicyNumber, CubeAPRPPremiumSum, PremiumWrittenNonTax, TaxesSurchargesValues, BDMNonTaxTotal, BDMTaxTotal, EDWNonTaxTotal, EDWTaxTotal, ADWNonTaxTotal, ADWTaxTotal);
                    result.Rows.Add("", CubeAPRPPremiumSum, "", DuckPremiumWritten, "", BDMPolicyTotal, "", EDWPolicyTotal, "", ADWPolicyTotal);
                }
                catch { }

                return result;
            }

            // Functions related to ML Policy Data Fetch
            private static DataTable BuildSubmissionTotalForMLPolicy(XmlDocument xDoc_CUBE, XmlDocument xDoc_BDM)
            {
                DataTable result = new DataTable();
                HashSet<string> submissionIDs = new HashSet<string>();

                string[] Columns = { "Submission", "Cube", "BDM Non Tax", "BDM  Tax" };
                foreach (string Col in Columns) { result.Columns.Add(Col, typeof(string)); }

                foreach (XmlElement xmlElement in xDoc_BDM.SelectNodes("/DocumentElement/BDM/SOURCE_SUBMISSION_ID")) { submissionIDs.Add(xmlElement.InnerText); }

                foreach (var Submission in submissionIDs)
                {
                    try
                    {
                        string CubePremiumPerSubmission = string.Empty;
                        if (!string.IsNullOrEmpty(xDoc_CUBE.OuterXml)) { CubePremiumPerSubmission = GetSumFromXMLDocument(xDoc_CUBE.OuterXml, "sum(/DocumentElement/Cube[SUBMISSION_ID='" + Submission + "']/CUBEAPRP)"); }

                        string BDMNonTaxPremiumPerSubmission = string.Empty; string BDMTaxPremiumPerSubmission = string.Empty;
                        if (!string.IsNullOrEmpty(xDoc_BDM.OuterXml))
                        {
                            if (xDoc_BDM.SelectNodes("/DocumentElement/BDM[SOURCE_SUBMISSION_ID='" + Submission + "']/TOT_APRP_PRM_EXCLD_TX_SRCHRG").Count > 0)
                            {
                                BDMNonTaxPremiumPerSubmission = GetSumFromXMLDocument(xDoc_BDM.OuterXml, "sum(/DocumentElement/BDM[SOURCE_SUBMISSION_ID='" + Submission + "']/TOT_APRP_PRM_EXCLD_TX_SRCHRG)");
                                BDMTaxPremiumPerSubmission = GetSumFromXMLDocument(xDoc_BDM.OuterXml, "sum(/DocumentElement/BDM[SOURCE_SUBMISSION_ID='" + Submission + "']/TOT_APRP_PREM_TX_SRCHRG_AMT)");
                            }
                        }

                        result.Rows.Add(Submission, CubePremiumPerSubmission, BDMNonTaxPremiumPerSubmission, BDMTaxPremiumPerSubmission);
                    }
                    catch { }
                }

                string CubeSubmissionTotal = GetSumFromXMLDocument(xDoc_CUBE.OuterXml, "sum(/DocumentElement/Cube/CUBEAPRP)");
                string BDMNonTaxPremiumPerSubmissionTotal = GetSumFromXMLDocument(xDoc_BDM.OuterXml, "sum(/DocumentElement/BDM/TOT_APRP_PRM_EXCLD_TX_SRCHRG)");
                string BDMTaxPremiumPerSubmissionTotal = GetSumFromXMLDocument(xDoc_BDM.OuterXml, "sum(/DocumentElement/BDM/TOT_APRP_PREM_TX_SRCHRG_AMT)");

                result.Rows.Add("", CubeSubmissionTotal, BDMNonTaxPremiumPerSubmissionTotal, BDMTaxPremiumPerSubmissionTotal);

                return result;
            }

            private static DataTable BuildPolicyTotalForMLPolicy(string DCTServerURL, string PolicyNumber, bool isPolicyProcessingSystemIsDuck, XmlDocument xDoc_CUBE, XmlDocument xDoc_BDM)
            {
                DataTable result = new DataTable();

                string[] Columns = { "Policy", "Cube", "Duck Non Tax", "Duck Tax", "BDM Non Tax", "BDM  Tax" };
                foreach (string Col in Columns) { result.Columns.Add(Col, typeof(string)); }

                try
                {
                    // Cube
                    string CubeAPRPPremiumSum = string.Empty;
                    CubeAPRPPremiumSum = GetSumFromXMLDocument(xDoc_CUBE.OuterXml, "sum(/DocumentElement/Cube/CUBEAPRP)");

                    // Duck
                    string PremiumWrittenNonTax = "NA"; string TaxesSurchargesValues = "NA"; string DuckPremiumWritten = "NA";

                    if (isPolicyProcessingSystemIsDuck)
                    {
                        DuckPremiumWritten = GetValueFromPolicy(DCTServerURL, PolicyNumber, "//data/policy/PremiumWritten");
                        TaxesSurchargesValues = GetValueFromPolicy(DCTServerURL, PolicyNumber, "//data/policy/TaxesSurchargesValues/@written");
                        if (!string.IsNullOrEmpty(DuckPremiumWritten) && !string.IsNullOrEmpty(TaxesSurchargesValues))
                        {
                            float nonTaxPremium = float.Parse(DuckPremiumWritten) - float.Parse(TaxesSurchargesValues);
                            PremiumWrittenNonTax = Convert.ToString(nonTaxPremium);
                        }
                    }

                    // BDM
                    string BDMNonTaxTotal = string.Empty; string BDMTaxTotal = string.Empty;
                    BDMNonTaxTotal = GetSumFromXMLDocument(xDoc_BDM.OuterXml, "sum(/DocumentElement/BDM/TOT_APRP_PRM_EXCLD_TX_SRCHRG)");
                    BDMTaxTotal = GetSumFromXMLDocument(xDoc_BDM.OuterXml, "sum(/DocumentElement/BDM/TOT_APRP_PREM_TX_SRCHRG_AMT)");

                    // Policy Total
                    string BDMPolicyTotal = string.Empty;
                    try
                    {
                        BDMPolicyTotal = Convert.ToString(float.Parse(BDMNonTaxTotal) + float.Parse(BDMTaxTotal));
                    }
                    catch { }

                    result.Rows.Add(PolicyNumber, CubeAPRPPremiumSum, PremiumWrittenNonTax, TaxesSurchargesValues, BDMNonTaxTotal, BDMTaxTotal);
                    result.Rows.Add("", CubeAPRPPremiumSum, "", DuckPremiumWritten, "", BDMPolicyTotal);
                }
                catch { }

                return result;
            }

            // Others
			private static DataTable BuildCoverageBreakDownForSubmission(string policyNumber, string submission, string server)
            {
                DataTable results = new DataTable();

				DataTable EDW = ExctractPolicyDataFromEDWNonTax(policyNumber, submission, server);
				DataTable ADW = ExctractPolicyDataFromADWNonTax(policyNumber, submission, server);

                if (EDW.Rows.Count > 0 && ADW.Rows.Count > 0)
                {
                    XmlDocument xDoc_EDW = Common.ConvertDataTableToXML(EDW);
                    XmlDocument xDoc_ADW = Common.ConvertDataTableToXML(ADW);

                    string[] Columns = { "Submission", "Risk #", "Risk ID", "Coverage ID", "Coverage CD", "EDW Amount", "ADW Amount", "Difference" };
                    foreach (string Col in Columns) { results.Columns.Add(Col, typeof(string)); }

                    List<string> EDW_COVERAGE_IDs = xDoc_EDW.SelectNodes("/DocumentElement/EDW[SUBM_ID='" + submission + "'][PRCPREM_CATGCD='APRP']/COVERAGE_ID").Cast<XmlNode>().Select(c => c.InnerText).Distinct().ToList();
                    List<string> ADW_COVERAGE_IDs = xDoc_ADW.SelectNodes("/DocumentElement/ADW[SUBM_ID='" + submission + "']/COVERAGE_ID").Cast<XmlNode>().Select(c => c.InnerText).Distinct().ToList();

                    string EDWTotalAmount = GetSumFromXMLDocument(xDoc_EDW.OuterXml, "sum(/DocumentElement/EDW[PRCPREM_CATGCD='APRP']/PREM_AMT)");
                    string ADWTotalAmount = GetSumFromXMLDocument(xDoc_ADW.OuterXml, "sum(/DocumentElement/ADW/PREMIUM_CHANGE_AMOUNT)");

                    foreach (string COVERAGE_ID in EDW_COVERAGE_IDs)
                    {
                        string EDWAmount = string.Empty; string ADWAmount = string.Empty; string Diff = string.Empty; string CVRG_CD = string.Empty;
                        string Risk_ID = string.Empty; string RiskNumber = string.Empty;

                        try
                        {
                            RiskNumber = xDoc_EDW.SelectSingleNode("/DocumentElement/EDW[SUBM_ID='" + submission + "' and COVERAGE_ID='" + COVERAGE_ID + "' and PRCPREM_CATGCD='APRP']/SCHDL_NO").InnerXml;
                            Risk_ID = xDoc_EDW.SelectSingleNode("/DocumentElement/EDW[SUBM_ID='" + submission + "' and COVERAGE_ID='" + COVERAGE_ID + "' and PRCPREM_CATGCD='APRP']/RISK_ID").InnerXml;
                            CVRG_CD = xDoc_EDW.SelectSingleNode("/DocumentElement/EDW[SUBM_ID='" + submission + "' and COVERAGE_ID='" + COVERAGE_ID + "' and PRCPREM_CATGCD='APRP']/CVRG_CD").InnerXml;
                            EDWAmount = GetSumFromXMLDocument(xDoc_EDW.OuterXml, "sum(/DocumentElement/EDW[SUBM_ID='" + submission + "'][COVERAGE_ID='" + COVERAGE_ID + "'][PRCPREM_CATGCD='APRP']/PREM_AMT)");
                            ADWAmount = GetSumFromXMLDocument(xDoc_ADW.OuterXml, "sum(/DocumentElement/ADW[SUBM_ID='" + submission + "'][COVERAGE_ID='" + COVERAGE_ID + "']/PREMIUM_CHANGE_AMOUNT)");

                            if (EDWTotalAmount == ADWTotalAmount)
                            {
                                results.Rows.Add(submission, RiskNumber, Risk_ID, COVERAGE_ID, CVRG_CD, EDWAmount, ADWAmount, Diff);
                            }
                            else
                            {
                                if (EDWAmount != ADWAmount)
                                {
                                    Diff = Convert.ToString(int.Parse(EDWAmount) - int.Parse(ADWAmount));
                                    results.Rows.Add(submission, RiskNumber, Risk_ID, COVERAGE_ID, CVRG_CD, EDWAmount, ADWAmount, Diff);
                                }
                            }
                        }
                        catch { results.Rows.Add(submission, RiskNumber, Risk_ID, COVERAGE_ID, CVRG_CD, EDWAmount, ADWAmount, "Exception"); }
                    }

                    if (EDW_COVERAGE_IDs.Count != ADW_COVERAGE_IDs.Count) { results.Rows.Add(submission, "", "", "", "", "", "EDW & ADW Coverage Counts are not matching."); }
                }

                return results;
            }

			private static DataTable BuildCoverageBreakDownForPolicy(string policyNumber, string server)
			{
				DataTable results = new DataTable();

				DataTable EDW = ExctractPolicyDataFromEDWNonTax(policyNumber, server);
				DataTable ADW = ExctractPolicyDataFromADWNonTax(policyNumber, server);

                if (EDW.Rows.Count > 0 && ADW.Rows.Count > 0)
                {
                    XmlDocument xDoc_EDW = Common.ConvertDataTableToXML(EDW);
                    XmlDocument xDoc_ADW = Common.ConvertDataTableToXML(ADW);

                    string[] Columns = { "Risk #", "Risk ID", "Coverage ID", "Coverage CD", "EDW Amount", "ADW Amount", "Difference" };
                    foreach (string Col in Columns) { results.Columns.Add(Col, typeof(string)); }

                    List<string> EDW_COVERAGE_IDs = xDoc_EDW.SelectNodes("/DocumentElement/EDW[PRCPREM_CATGCD='APRP']/COVERAGE_ID").Cast<XmlNode>().Select(c => c.InnerText).Distinct().ToList();
                    List<string> ADW_COVERAGE_IDs = xDoc_ADW.SelectNodes("/DocumentElement/ADW/COVERAGE_ID").Cast<XmlNode>().Select(c => c.InnerText).Distinct().ToList();

                    //EDW_COVERAGE_IDs = new List<string> { "47473892" };

                    string EDWTotalAmount = GetSumFromXMLDocument(xDoc_EDW.OuterXml, "sum(/DocumentElement/EDW[PRCPREM_CATGCD='APRP']/PREM_AMT)");
                    string ADWTotalAmount = GetSumFromXMLDocument(xDoc_ADW.OuterXml, "sum(/DocumentElement/ADW/PREMIUM_CHANGE_AMOUNT)");

                    foreach (string COVERAGE_ID in EDW_COVERAGE_IDs)
                    {
                        string EDWAmount = string.Empty; string ADWAmount = string.Empty; string Diff = string.Empty; string CVRG_CD = string.Empty;
                        string Risk_ID = string.Empty; string RiskNumber = string.Empty;

                        try
                        {
                            RiskNumber = xDoc_EDW.SelectSingleNode("/DocumentElement/EDW[COVERAGE_ID='" + COVERAGE_ID + "' and PRCPREM_CATGCD='APRP']/SCHDL_NO").InnerXml;
                            Risk_ID = xDoc_EDW.SelectSingleNode("/DocumentElement/EDW[COVERAGE_ID='" + COVERAGE_ID + "' and PRCPREM_CATGCD='APRP']/RISK_ID").InnerXml;
                            CVRG_CD = xDoc_EDW.SelectSingleNode("/DocumentElement/EDW[COVERAGE_ID='" + COVERAGE_ID + "' and PRCPREM_CATGCD='APRP']/CVRG_CD").InnerXml;
                            EDWAmount = GetSumFromXMLDocument(xDoc_EDW.OuterXml, "sum(/DocumentElement/EDW[COVERAGE_ID='" + COVERAGE_ID + "'][PRCPREM_CATGCD='APRP']/PREM_AMT)");
                            ADWAmount = GetSumFromXMLDocument(xDoc_ADW.OuterXml, "sum(/DocumentElement/ADW[COVERAGE_ID='" + COVERAGE_ID + "']/PREMIUM_CHANGE_AMOUNT)");

                            if (EDWTotalAmount == ADWTotalAmount)
                            {
                                results.Rows.Add(RiskNumber, Risk_ID, COVERAGE_ID, CVRG_CD, EDWAmount, ADWAmount, Diff);
                            }
                            else
                            {
                                if (EDWAmount != ADWAmount)
                                {
                                    Diff = Convert.ToString(int.Parse(EDWAmount) - int.Parse(ADWAmount));
                                    results.Rows.Add(RiskNumber, Risk_ID, COVERAGE_ID, CVRG_CD, EDWAmount, ADWAmount, Diff);
                                }
                            }
                        }
                        catch { results.Rows.Add(RiskNumber, Risk_ID, COVERAGE_ID, CVRG_CD, EDWAmount, ADWAmount, "Exception"); }
                    }

                    if (EDW_COVERAGE_IDs.Count != ADW_COVERAGE_IDs.Count) { results.Rows.Add("", "", "", "", "", "EDW & ADW Coverage Count not matching."); }
                }

                return results;
            }

            // Common
            private static string GetSumFromXMLDocument(string outerXml, string xPath)
            {
                string Results = string.Empty;

                try
                {
                    using (StringReader sr = new StringReader(outerXml))
                    {
                        XPathDocument xPathDocument = new XPathDocument(sr);
                        XPathNavigator xPathNavigator = xPathDocument.CreateNavigator();
                        Results = Convert.ToString(xPathNavigator.Evaluate(xPath));
                    }
                }
                catch { Results = "0"; }

                return Results;
            }

            private static string GetValueFromPolicy(string DCTServerURL, string PolicyNumber, string xPath)
            {
                string Value = string.Empty;

                XmlDocument getElementRs = Common.GetElementRq(DCTServerURL, PolicyNumber, xPath);

                string loadPolicyRs_Status = getElementRs.SelectSingleNode("/server/responses/OnlineData.loadPolicyRs/@status").Value;
                string getElementRs_Status = getElementRs.SelectSingleNode("/server/responses/Session.getElementRs/@status").Value;

                if (loadPolicyRs_Status == Success && getElementRs_Status == Success) { Value = getElementRs.SelectSingleNode("/server/responses/Session.getElementRs/@value").Value; }

                return Value;
            }

			private static HashSet<string> GetSubmissionIDsFromPolicyTerm(string PolicyNumber, string Server)
			{
				HashSet<string> results = new HashSet<string>();

				DataTable _temp = ExctractPolicyDataFromPolicyTerm(PolicyNumber, Server);
				XmlDocument xDoc = Common.ConvertDataTableToXML(_temp);

                foreach (XmlElement xmlElement in xDoc.SelectNodes("/DocumentElement/Policy_x0020_Term/SUBM_ID")) { results.Add(xmlElement.InnerText); }

                return results;
            }
        }
    }
}