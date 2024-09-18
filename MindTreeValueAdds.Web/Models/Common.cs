using System;
using System.IO;
using System.Xml;
using System.Net;
using System.Text;
using System.Data;
using System.Configuration;
using Oracle.ManagedDataAccess.Client;
using System.Data.OleDb;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Xml.Xsl;

namespace MindTreeValueAdds.Web.Models
{
    public class Common
    {
        internal static XmlDocument PostDCTRequestReturnResponse(string Request, string DCTServerURL)
        {
            XmlDocument resposeXML = new XmlDocument();
            try
            {
                XmlDocument xDoc = new XmlDocument();

                string password = "admin";
                if (DCTServerURL == "https://uncp26.duckcreekondemand.com/ServerPolicy/DCTserver.aspx") { password = "GAIC@prd1515x"; }

                //Create a web request with url that can receive a post
                WebRequest request = WebRequest.Create(DCTServerURL);

                //Set the method property to the request 
                request.Method = "POST";
                string req = string.Format("<server><requests><Session.loginRq userName=\"{0}\" password=\"{1}\" />", "admin", password);
                req += Request + "<Session.closeRq /></requests></server>";

                //create a postData variable to store that request and convert that into byte array
                string postData = req;
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);

                //Set the content type property of the web request
                request.ContentType = "text/xml;encoding='utf-8'";

                //Set the content length property of the web request
                request.ContentLength = byteArray.Length;

                //Get the request stream
                Stream datastream = request.GetRequestStream();

                //Write the data to the  request stream
                datastream.Write(byteArray, 0, byteArray.Length);

                //Close the datastream
                datastream.Close();

                //Get the response
                WebResponse response = request.GetResponse();

                //Get the stream containing content retured by the server
                datastream = response.GetResponseStream();

                //open the stream using stream reader for easy access.
                StreamReader reader = new StreamReader(datastream);
                string responseFromServer = reader.ReadToEnd();

                //Close all the streams
                reader.Close();
                datastream.Close();
                response.Close();

                //create an xml document to keep the response
                resposeXML = new XmlDocument();
                resposeXML.LoadXml(responseFromServer);
            }

            catch (Exception ex) { Console.Write("Issue with DCT request: " + Convert.ToString(ex.Message)); }

            return resposeXML;
        }

        internal static bool CheckOracleDBConnection(string OracleDBConnection)
        {
            bool result = false;

            if (!string.IsNullOrEmpty(OracleDBConnection))
            {
                OracleConnection Con = new OracleConnection(OracleDBConnection);

                try
                {
                    Con.Open();
                    result = true;
                }
                catch (Exception ex)
                {
                    string Message = ex.Message;
                }
                finally
                {
                    if (Con.State == ConnectionState.Open)
                    {
                        Con.Close();
                        Con.Dispose();
                    }
                }
            }

            return result;
        }

        internal static DataTable ExicuteOracleDBQuery(string SQLQuery, string connectionString)
        {

            DataTable results = new DataTable();

            if (!string.IsNullOrEmpty(connectionString) && !string.IsNullOrEmpty(SQLQuery))
            {
                OracleConnection oracleConnection = new OracleConnection(connectionString);

                try
                {
                    oracleConnection.Open();
                    OracleDataAdapter oracleDataAdapter = new OracleDataAdapter(SQLQuery, oracleConnection);
                    oracleDataAdapter.Fill(results);
                }
                catch (Exception ex)
                {
                    string message = ex.Message;
                    results.Clear();
                }
                finally
                {
                    if (oracleConnection.State == ConnectionState.Open)
                    {
                        oracleConnection.Close();
                        oracleConnection.Dispose();
                    }
                }
            }

            return results;
        }

        internal static string ReturnValidDUCKServerURL(string Server)
        {
            string result = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(Server))
                {
                    Server = Server.ToLower();

                    switch (Server)
                    {
                        case "app 12":
                            result = "https://uncd218.duckcreekondemand.com/ServerPolicy/DCTserver.aspx";
                            break;

                        case "app 13":
                            result = "https://uncd219.duckcreekondemand.com/ServerPolicy/DCTserver.aspx";
                            break;

                        case "app 14":
                            result = "https://uncd220.duckcreekondemand.com/ServerPolicy/DCTserver.aspx";
                            break;

                        case "perf.":
                            result = "https://uncd274.duckcreekondemand.com/ServerPolicy/DCTserver.aspx";
                            break;

                        case "cert":
                            result = "https://uncu41.duckcreekondemand.com/ServerPolicy/DCTserver.aspx";
                            break;

                        case "uat":
                            result = "https://uncu23s.duckcreekondemand.com/ServerPolicy/DCTserver.aspx";
                            break;

                        case "prod":
                            result = "https://uncp26.duckcreekondemand.com/ServerPolicy/DCTserver.aspx";
                            break;

                        case "int":
                            result = "https://uncd221.duckcreekondemand.com/ServerPolicy/DCTserver.aspx";
                            break;
                        default:
                            result = "";
                            break;
                    }
                }
            }
            catch { }

            return result;
        }

        internal static XmlDocument LoadPolicyRq(string DCTServerURL, string fullPolicy)
        {
            XmlDocument results = new XmlDocument();

            try
            {
                results = PostDCTRequestReturnResponse("<OnlineData.loadPolicyRq policyNumber=\"" + fullPolicy + "\" />", DCTServerURL);
            }
            catch { }

            return results;
        }

        internal static XmlDocument ConvertDataTableToXML(DataTable DT)
        {
            XmlDocument xDoc = new XmlDocument();
            string finalResults = string.Empty;

            try
            {
                using (StringWriter stringWriter = new StringWriter())
                {
                    DT.WriteXml(stringWriter);
                    finalResults = Convert.ToString(stringWriter);
                }
                if (!string.IsNullOrEmpty(finalResults)) { xDoc.LoadXml(finalResults); }
            }
            catch (Exception ex) { string message = ex.Message; }

            return xDoc;
        }

        internal static XmlDocument ConvertDataTableToXML_New(DataTable dt)
        {
            XmlDocument xDoc = new XmlDocument();
            string finalResults = string.Empty;

            try
            {
                MemoryStream str = new MemoryStream();
                dt.WriteXml(str, true);
                str.Seek(0, SeekOrigin.Begin);
                StreamReader sr = new StreamReader(str);
                finalResults = sr.ReadToEnd();
                if (!string.IsNullOrEmpty(finalResults)) { xDoc.LoadXml(finalResults); }
            }
            catch (Exception ex) { string message = ex.Message; }

            return xDoc;
        }

        internal static XmlDocument GetElementRq(string DCTServerURL, string PolicyNumber, string xPath)
        {
            XmlDocument getElementRs = new XmlDocument();

            try
            {
                StringBuilder getElementRq = new StringBuilder();
                getElementRq.Append("<OnlineData.loadPolicyRq policyNumber=\"" + PolicyNumber + "\" />");
                getElementRq.Append("<Session.getElementRq path=\"" + xPath + "\"/>");
                getElementRs = PostDCTRequestReturnResponse(getElementRq.ToString(), DCTServerURL);
            }
            catch { }

            return getElementRs;
        }

        internal static XmlDocument GetElementRqUsingHistoryID(string DCTServerURL, string policyID, string historyID, string xPath)
        {
            XmlDocument getElementRs = new XmlDocument();

            try
            {
                StringBuilder getElementRq = new StringBuilder();
                getElementRq.Append("<OnlineData.loadPolicyRq policyID=\"" + policyID + "\" />");
                getElementRq.Append("<OnlineData.loadHistoryRq historyID=\"" + historyID + "\" />");
                getElementRq.Append("<Session.getElementRq path=\"" + xPath + "\"/>");
                getElementRs = PostDCTRequestReturnResponse(getElementRq.ToString(), DCTServerURL);
            }
            catch { }

            return getElementRs;
        }

        internal static XmlDocument GetDocumentRq(string DCTServerURL, string OriginalSubmissionID, string VersionNumber, string SubmissionID)
        {
            XmlDocument getDocumentRs = new XmlDocument();

            try
            {
                StringBuilder getDocumentRq = new StringBuilder();
                getDocumentRq.Append("<CustomServer.processRq abortOnError=\"1\">");
                getDocumentRq.Append("<xsltRequests>");
                getDocumentRq.Append("<OnlineData.listRq var.policyID=\"policyList/policy/@policyID\">");
                getDocumentRq.Append("<keys>");
                getDocumentRq.Append("<key op=\"=\" name=\"OriginalSubmissionID\" value=\"" + OriginalSubmissionID + "\"/>");
                getDocumentRq.Append("</keys>");
                getDocumentRq.Append("<sort name=\"OriginalSubmissionID\" direction=\"ascending\"/>");
                getDocumentRq.Append("</OnlineData.listRq>");
                getDocumentRq.Append("<OnlineData.loadPolicyRq policyID=\"~policyID~\" var.manuscriptID=\"//OnlineData.loadPolicyRs/@manuScriptID\"/>");
                getDocumentRq.Append("<TransACT.listTransactionsRq var.historyID=\"transactions/transaction[VersionNumber='" + VersionNumber + "' and SubmissionID='" + SubmissionID + "' and (not(DeprecatedBy) or DeprecatedBy='')]/HistoryID\"/>");
                getDocumentRq.Append("<OnlineData.loadHistoryRq historyID=\"~historyID~\"/>");
                getDocumentRq.Append("<Session.getDocumentRq/>");
                getDocumentRq.Append("</xsltRequests>");
                getDocumentRq.Append("</CustomServer.processRq>");
                getDocumentRs = PostDCTRequestReturnResponse(getDocumentRq.ToString(), DCTServerURL);
            }
            catch { }

            return getDocumentRs;
        }

        internal static XmlDocument GetAllDocumentsRq(string DCTServerURL, string OriginalSubmissionID, string VersionNumber, string SubmissionID)
        {
            XmlDocument getAllDocumentsRs = new XmlDocument();

            try
            {
                StringBuilder getAllDocumentsRq = new StringBuilder();
                getAllDocumentsRq.Append("<CustomServer.processRq abortOnError=\"1\">");
                getAllDocumentsRq.Append("<xsltRequests>");
                getAllDocumentsRq.Append("<OnlineData.listRq var.policyID=\"policyList/policy/@policyID\">");
                getAllDocumentsRq.Append("<keys>");
                getAllDocumentsRq.Append("<key op=\"=\" name=\"OriginalSubmissionID\" value=\"" + OriginalSubmissionID + "\"/>");
                getAllDocumentsRq.Append("</keys>");
                getAllDocumentsRq.Append("<sort name=\"OriginalSubmissionID\" direction=\"ascending\"/>");
                getAllDocumentsRq.Append("</OnlineData.listRq>");
                getAllDocumentsRq.Append("<OnlineData.loadPolicyRq policyID=\"~policyID~\" var.manuscriptID=\"//OnlineData.loadPolicyRs/@manuScriptID\"/>");
                getAllDocumentsRq.Append("<TransACT.listTransactionsRq var.historyID=\"transactions/transaction[VersionNumber='" + VersionNumber + "' and SubmissionID='" + SubmissionID + "' and (not(DeprecatedBy) or DeprecatedBy='')]/HistoryID\"/>");
                getAllDocumentsRq.Append("<OnlineData.loadHistoryRq historyID=\"~historyID~\"/>");
                getAllDocumentsRq.Append("<Session.getAllDocumentsRq/>");
                getAllDocumentsRq.Append("</xsltRequests>");
                getAllDocumentsRq.Append("</CustomServer.processRq>");
                getAllDocumentsRs = PostDCTRequestReturnResponse(getAllDocumentsRq.ToString(), DCTServerURL);
            }
            catch { }

            return getAllDocumentsRs;
        }

        internal static XmlDocument GetAllDocumentsRqUsingHistoryID(string DCTServerURL, string policyID, string historyID)
        {
            XmlDocument getAllDocumentsRs = new XmlDocument();

            try
            {
                StringBuilder getAllDocumentsRq = new StringBuilder();
                getAllDocumentsRq.Append("<OnlineData.loadPolicyRq policyID=\"" + policyID + "\"/>");
                getAllDocumentsRq.Append("<OnlineData.loadHistoryRq historyID=\"" + historyID + "\"/>");
                getAllDocumentsRq.Append("<Session.getAllDocumentsRq/>");
                getAllDocumentsRs = PostDCTRequestReturnResponse(getAllDocumentsRq.ToString(), DCTServerURL);
            }
            catch { }

            return getAllDocumentsRs;
        }

        internal static bool ValidateMLPolicySymbol(string policyNumber)
        {
            if (policyNumber.StartsWith("UMB") || policyNumber.StartsWith("EXC") || policyNumber.StartsWith("EXX") || policyNumber.StartsWith("OMH")
                || policyNumber.StartsWith("OMC") || policyNumber.StartsWith("OML") || policyNumber.StartsWith("OMM") || policyNumber.StartsWith("OMP")
                || policyNumber.StartsWith("OMX") || policyNumber.StartsWith("HHQ") || policyNumber.StartsWith("BOP")) { return true; }
            return false;
        }

        internal static DataTable ConvertXSLXtoDataTable(string FilePath, string connString, string sheetName)
        {
            OleDbConnection oledbConn = new OleDbConnection(connString);
            DataTable dt = new DataTable();
            try
            {
                oledbConn.Open();
                using (OleDbCommand cmd = new OleDbCommand(@"SELECT * FROM [" + sheetName + "$]", oledbConn))
                {
                    OleDbDataAdapter oleda = new OleDbDataAdapter { SelectCommand = cmd };
                    DataSet ds = new DataSet();
                    oleda.Fill(ds);
                    dt = ds.Tables[0];
                }
            }
            catch (Exception ex) { string message = ex.Message; }
            finally { oledbConn.Close(); }

            if (File.Exists(FilePath)) { File.Delete(FilePath); }
            return dt;
        }

        internal static string RemoveWhiteSpaceFromInputString(string Input)
        {
            try { Input = Regex.Replace(Input, @"\s+", ""); return Input; }
            catch { return Input; }
        }

        internal static string GetXMLBasedonXMLTypeFromCUBEDB(string submission, string message_type_cd, string server)
        {
            string result = string.Empty;
            message_type_cd = message_type_cd.ToLower();

            try
            {
                string SQLScript = "SELECT XMLTYPE(UTL_COMPRESS.LZ_UNCOMPRESS(message_content_bin), (SELECT NLS_CHARSET_ID('UTF8') FROM DUAL)) AS XML_TEXT from RAMT.message_exchange ME WHERE ME.key_value_txt IN ('" + submission + "') AND ME.MESSAGE_TYPE_CD IN ('" + message_type_cd + "') ORDER BY ME.KEY_VALUE_TXT asc, ME.MESSAGE_TYPE_CD DESC";
                DataTable dbResult = ExicuteOracleDBQuery(SQLScript, ConfigurationManager.ConnectionStrings[GetConnectionStringName(server).Split(':')[0]].ConnectionString);

                if (dbResult.Rows.Count > 0)
                {
                    result = dbResult.Rows[0].Field<string>(0);
                    if (!string.IsNullOrEmpty(result)) { return result; }
                }
            }
            catch (Exception ex) { string message = ex.Message; }

            return result;
        }

        internal static XmlDocument listRq(string policyNumber, string DCTServerURL)
        {
            XmlDocument results = new XmlDocument();

            try
            {
                StringBuilder listRq = new StringBuilder();
                listRq.Append("<OnlineData.listRq deleteStatus=\"2\">");
                listRq.Append("<keys>");
                listRq.Append("<key name=\"policyNumber\" op=\"=\" value=\"" + policyNumber + "\"/>");
                listRq.Append("</keys>");
                listRq.Append("<sort direction=\"ascending\" name=\"policyid\"/>");
                listRq.Append("</OnlineData.listRq>");
                results = PostDCTRequestReturnResponse(listRq.ToString(), DCTServerURL);
            }
            catch { }

            return results;
        }

        internal static Dictionary<string, string> GetPolicyDataUsingOnlineDataListRq(string policyNumber, string DCTServerURL)
        {
            Dictionary<string, string> results = new Dictionary<string, string>();
            XmlDocument listRs = listRq(policyNumber, DCTServerURL);
            if (listRs.SelectSingleNode("/server/responses/OnlineData.listRs/@status").InnerText == "success")
            {
                results.Add("policyID", listRs.SelectSingleNode("/server/responses/OnlineData.listRs/policyList/policy[1]/@policyID").InnerText);
                results.Add("OriginalSubmissionID", listRs.SelectSingleNode("/server/responses/OnlineData.listRs/policyList/policy[1]/@OriginalSubmissionID").InnerText);
            }
            return results;
        }

        internal static XmlDocument ListTransactionsRq(string policyNumber, string DCTServerURL)
        {
            XmlDocument results = new XmlDocument();

            try
            {
                StringBuilder listTransactionsRq = new StringBuilder();
                listTransactionsRq.Append("<OnlineData.loadPolicyRq policyNumber=\"" + policyNumber + "\" />");
                listTransactionsRq.Append("<TransACT.listTransactionsRq />");
                results = PostDCTRequestReturnResponse(listTransactionsRq.ToString(), DCTServerURL);
            }
            catch { }

            return results;
        }

        internal static string GetRuleManuscriptBasedOnTransaction(string transactionType)
        {
            transactionType = transactionType.ToLower();

            switch (transactionType)
            {
                case "new":
                    return "Carrier_New_Rules_2_1_0";

                case "renew":
                    return "Carrier_Renew_Rules_2_1_0";

                case "endorse":
                    return "Carrier_Endorse_Rules_2_1_0";

                case "cancel":
                    return "Carrier_Cancel_Rules_2_1_0";

                case "nonrenew":
                    return "Carrier_NonRenew_Rules_2_1_0";

                case "reinstate":
                    return "Carrier_Reinstate_Rules_2_1_0";

                case "finalaudit":
                    return "Carrier_FinalAudit_Rules_2_1_0";

                default:
                    return string.Empty;
            }
        }

        internal static string BuildScriptToTriggerAttachPolicyFormRuleInDuck(string OriginalSubmissionID, string SubmissionID, string VersionNumber, string TransactionType)
        {
            string ruleManuscript = GetRuleManuscriptBasedOnTransaction(TransactionType);
            if (!string.IsNullOrEmpty(OriginalSubmissionID) && !string.IsNullOrEmpty(SubmissionID) && !string.IsNullOrEmpty(VersionNumber) && !string.IsNullOrEmpty(ruleManuscript))
            {
                StringBuilder CustomServerProcessRq = new StringBuilder();
                CustomServerProcessRq.Append("<CustomServer.processRq abortOnError=\"1\">");
                CustomServerProcessRq.Append("<xsltRequests>");
                CustomServerProcessRq.Append("<OnlineData.listRq var.policyID=\"policyList/policy/@policyID\">");
                CustomServerProcessRq.Append("<keys>");
                CustomServerProcessRq.Append("<key op=\"=\" name=\"OriginalSubmissionID\" value=\"" + OriginalSubmissionID + "\"/>");
                CustomServerProcessRq.Append("</keys>");
                CustomServerProcessRq.Append("<sort name=\"OriginalSubmissionID\" direction=\"ascending\"/>");
                CustomServerProcessRq.Append("</OnlineData.listRq>");
                CustomServerProcessRq.Append("<OnlineData.loadPolicyRq policyID=\"~policyID~\" var.manuscriptID=\"//OnlineData.loadPolicyRs/@manuScriptID\"/>");
                CustomServerProcessRq.Append("<TransACT.listTransactionsRq var.historyID=\"transactions/transaction[VersionNumber='" + VersionNumber + "' and SubmissionID='" + SubmissionID + "' and(not(DeprecatedBy) or DeprecatedBy='')]/HistoryID\"/>");
                CustomServerProcessRq.Append("<OnlineData.loadHistoryRq historyID=\"~historyID~\"/>");
                CustomServerProcessRq.Append("<ManuScript.getValueRq manuscript=\"" + ruleManuscript + "\" field=\"AttachPolicyFormRule.Execute\"/>");
                CustomServerProcessRq.Append("<TransACT.updateTransactionRecordRq/>");
                CustomServerProcessRq.Append("<OnlineData.storePolicyRq manuScriptID=\"~manuscriptID~\"/>");
                CustomServerProcessRq.Append("</xsltRequests>");
                CustomServerProcessRq.Append("</CustomServer.processRq>");
                return CustomServerProcessRq.ToString();
            }

            return string.Empty;
        }

        internal static string GetConnectionStringName(string Server)
        {
            switch (Server.ToLower())
            {
                case "prod":
                    return "CUBEPRODDB:EDWPRODDB";

                case "uat":
                    return "CUBEUATDB:EDWUATDB";

                case "int":
                    return "CUBEINTDB:EDWINTDB";

                default:
                    return "CUBEDEVDB:EDWDEVDB";
            }
        }

        internal static string TransformXML(XmlDocument xDoc, string stylesheetPath)
        {
            try
            {
                var document = xDoc;
                XsltSettings settings = new XsltSettings(true, false);
                XslCompiledTransform transform = new XslCompiledTransform();
                transform.Load(stylesheetPath, settings, new XmlUrlResolver());
                StringWriter writer = new StringWriter();
                XmlReader xmlReadB = new XmlTextReader(new StringReader(document.DocumentElement.OuterXml));
                transform.Transform(xmlReadB, null, writer);
                return writer.ToString();
            }
            catch (Exception ex) { throw ex; }
        }
    }
}