using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Web.Hosting;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MindTreeValueAdds.Web.Models
{
    public class ReprocessSubmission
    {
        static readonly string ReprocessSubmissionScriptsPath = "~/App_Data/DB Scripts/ReprocessSubmission/";
        static readonly string ReprocessScriptsPath = "~/App_Data/ReprocessSubmission/ReprocessingScript/";
        static readonly string RegExPattern = "[^\x00-\x7F]+";
        static readonly string Success = "success";
        static readonly string Fail = "fail";

        public static class UserDefineFunctions
        {
            internal static string ReprocessAnyTransaction(string OriginalSubmissionID, string Submission, string VersionNumber, string DCTServerURL, string Server)
            {
                IDictionary<string, string> RequiredDatatoReprocessTransaction = GetRequiredDatatoReprocessTransaction(OriginalSubmissionID, Submission, VersionNumber, DCTServerURL);

                if (RequiredDatatoReprocessTransaction.Count > 0)
                {
                    string policyID = RequiredDatatoReprocessTransaction["policyID"];
                    string OOSTransactionID = RequiredDatatoReprocessTransaction["TransactionID"];
                    string pageManuScriptID = RequiredDatatoReprocessTransaction["pageManuScriptID"];
                    string HistoryID = RequiredDatatoReprocessTransaction["HistoryID"];
                    string PolicyNumber = RequiredDatatoReprocessTransaction["policyNumber"];
                    string formManuscriptID = RequiredDatatoReprocessTransaction["CPPFormsManuscriptID"];

                    return BuildDUCKScriptToRemoveSpecialCharFromDCXML(PolicyNumber, OriginalSubmissionID, Submission, VersionNumber, DCTServerURL, policyID, OOSTransactionID, pageManuScriptID, formManuscriptID, HistoryID, Server);
                }

                return Fail;
            }

            private static IDictionary<string, string> GetRequiredDatatoReprocessTransaction(string OriginalSubmissionID, string SubmissionID, string VersionNumber, string DCTServerURL)
            {
                IDictionary<string, string> results = new Dictionary<string, string>();
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
                CustomServerProcessRq.Append("<Session.getElementRq path=\"" + "session^data/policy/CPPFormsManuscriptID" + "\"/>");
                CustomServerProcessRq.Append("<Session.getElementRq path=\"" + "session^data/policyAdmin/transactions/transaction[SubmissionID='" + SubmissionID + "']/@id" + "\"/>");
                CustomServerProcessRq.Append("<Session.getElementRq path=\"" + "session^data/policyAdmin/transactions/transaction[SubmissionID='" + SubmissionID + "']/HistoryID" + "\"/>");
                CustomServerProcessRq.Append("</xsltRequests>");
                CustomServerProcessRq.Append("</CustomServer.processRq>");

                XmlDocument CustomServerProcessRs = Common.PostDCTRequestReturnResponse(CustomServerProcessRq.ToString(), DCTServerURL);

                if (!string.IsNullOrEmpty(CustomServerProcessRs.InnerXml))
                {
                    if (CustomServerProcessRs.SelectSingleNode("/server/responses/CustomServer.processRs/@status").Value == Success)
                    {
                        if (CustomServerProcessRs.SelectSingleNode("/server/responses/CustomServer.processRs/server/responses/OnlineData.listRs/@status").Value == Success)
                        {
                            if (CustomServerProcessRs.SelectSingleNode("/server/responses/CustomServer.processRs/server/responses/OnlineData.loadPolicyRs/@status").Value == Success)
                            {
                                if (CustomServerProcessRs.SelectSingleNode("/server/responses/CustomServer.processRs/server/responses/TransACT.listTransactionsRs/@status").Value == Success)
                                {
                                    string policyID = CustomServerProcessRs.SelectSingleNode("/server/responses/CustomServer.processRs[@status='success']/server/responses/OnlineData.listRs[@status='success']/policyList/policy/@policyID").Value;
                                    string policyNumber = CustomServerProcessRs.SelectSingleNode("/server/responses/CustomServer.processRs[@status='success']/server/responses/OnlineData.listRs[@status='success']/policyList/policy/@policyNumber").Value;
                                    string LOB = CustomServerProcessRs.SelectSingleNode("/server/responses/CustomServer.processRs[@status='success']/server/responses/OnlineData.listRs[@status='success']/policyList/policy/@LOB").Value;
                                    string pageManuScriptID = CustomServerProcessRs.SelectSingleNode("/server/responses/CustomServer.processRs[@status='success']/server/responses/OnlineData.loadPolicyRs[@status='success']/@manuScriptID").Value;

                                    // getElementRs
                                    string CPPFormsManuscriptID = CustomServerProcessRs.SelectSingleNode("/server/responses/CustomServer.processRs[@status='success']/server/responses/Session.getElementRs[@status='success'][1]/@value").Value;
                                    string TransactionID = CustomServerProcessRs.SelectSingleNode("/server/responses/CustomServer.processRs[@status='success']/server/responses/Session.getElementRs[@status='success'][2]/@value").Value;
                                    string HistoryID = CustomServerProcessRs.SelectSingleNode("/server/responses/CustomServer.processRs[@status='success']/server/responses/Session.getElementRs[@status='success'][3]/@value").Value;

                                    results.Add("LOB", LOB);
                                    results.Add("policyID", policyID);
                                    results.Add("policyNumber", policyNumber);
                                    results.Add("pageManuScriptID", pageManuScriptID);
                                    results.Add("CPPFormsManuscriptID", CPPFormsManuscriptID);
                                    results.Add("TransactionID", TransactionID);
                                    results.Add("HistoryID", HistoryID);
                                }
                            }
                        }
                    }
                }

                return results;
            }

            private static string BuildDUCKScriptToRemoveSpecialCharFromDCXML(string policyNumber, string OriginalSubmissionID, string OOSSubmission, string VersionNumber, string DCTServerURL, string policyID, string transGroupID, string pageManuScriptID, string formManuscriptID, string historyID, string Server)
            {
                string result = string.Empty;
                bool isOOS = false;

                if (!string.IsNullOrEmpty(policyID) && !string.IsNullOrEmpty(transGroupID) && !string.IsNullOrEmpty(pageManuScriptID))
                {
                    IDictionary<string, List<string>> ListOfTransactionsUsingSessionReconcile = GetListOfTransactionsUsingSessionReconcile(policyID, transGroupID, DCTServerURL);

                    List<string> HistoryIDs = ListOfTransactionsUsingSessionReconcile["HistoryIDs"];
                    List<string> SubmissionIDs = ListOfTransactionsUsingSessionReconcile["SubmissionIDs"];
                    IDictionary<string, List<string>> LIstOfHistoryIDHavingSpecialChar = new Dictionary<string, List<string>>();

                    //HistoryIDs = new List<string> { "189529" };

                    if (HistoryIDs.Count == 0) { return "Oops. Unable to find any HistoryID to processed."; }
                    else if (HistoryIDs.Count > 1) { isOOS = true; }

                    foreach (string HistoryID in HistoryIDs)
                    {
                        XmlDocument getAllDocumentsRs = Common.GetAllDocumentsRqUsingHistoryID(DCTServerURL, policyID, HistoryID);
                        if (getAllDocumentsRs.SelectSingleNode("/server/responses/Session.getAllDocumentsRs/@status").Value == Success)
                        {
                            if (GetSpecialCharCount(getAllDocumentsRs) > 0)
                            {
                                LIstOfHistoryIDHavingSpecialChar.Add(HistoryID, GetXpathsWhereWeHaveSpecialChar(getAllDocumentsRs));
                            }
                        }
                    }

                    // Generate Script.
                    string _result = string.Empty;
                    if (LIstOfHistoryIDHavingSpecialChar.Count > 0)
                    {
                        foreach (var HistoryIDHavingSpecialChar in LIstOfHistoryIDHavingSpecialChar)
                        {
                            _result += BuildReuqestToRemoveSpecialCharFromDCXML(policyID, HistoryIDHavingSpecialChar.Key, HistoryIDHavingSpecialChar.Value, pageManuScriptID, DCTServerURL);
                        }
                    }

                    if (Server == "Prod") { _result += BuildReprocessingScript(policyNumber, OOSSubmission, VersionNumber, policyID, transGroupID, formManuscriptID, historyID, DCTServerURL, isOOS); }

                    if (!string.IsNullOrEmpty(_result))
                    {
                        string requestFilePath = StoreTheFile(_result, "Request");
                        return Success + "==" + requestFilePath;
                    }
                    else { return "Oops. Nothing to display."; }
                }

                return result;
            }

            private static IDictionary<string, List<string>> GetListOfTransactionsUsingSessionReconcile(string policyID, string OOSTransactionID, string DCTServerURL)
            {
                IDictionary<string, List<string>> results = new Dictionary<string, List<string>>();
                List<string> HistoryIDs = new List<string>();
                List<string> SubmissionIDs = new List<string>();

                string getTransactionReportRq = "<SessionReconcile.getTransactionReportRq policyID=\"" + policyID + "\" transGroup=\"" + OOSTransactionID + "\"/>";
                XmlDocument CustomServerProcessRs = Common.PostDCTRequestReturnResponse(getTransactionReportRq, DCTServerURL);

                if (CustomServerProcessRs.SelectSingleNode("/server/responses/SessionReconcile.getTransactionReportRs/@status").Value == Success)
                {
                    XmlNodeList transactions = CustomServerProcessRs.SelectNodes("/server/responses/SessionReconcile.getTransactionReportRs[@status='success']/report/entry/transaction");

                    foreach (XmlNode transaction in transactions)
                    {
                        HistoryIDs.Add(transaction["HistoryID"].InnerText);
                        SubmissionIDs.Add(transaction["SubmissionID"].InnerText);
                    }

                    results.Add("HistoryIDs", HistoryIDs);
                    results.Add("SubmissionIDs", SubmissionIDs);
                }

                return results;
            }

            private static int GetSpecialCharCount(XmlDocument xmlDocument)
            {
                int result = 0;

                try
                {
                    if (!string.IsNullOrEmpty(xmlDocument.OuterXml))
                    {
                        MatchCollection matchCollection = Regex.Matches(xmlDocument.OuterXml, RegExPattern);
                        result = matchCollection.Count;
                    }
                }
                catch (Exception ex)
                {
                    string message = ex.Message;
                    result = -1;
                }

                return result;
            }

            private static List<string> GetXpathsWhereWeHaveSpecialChar(XmlDocument xmlDocument)
            {
                List<string> results = new List<string>();
                IEnumerable<XElement> parentNodes = null;

                try
                {
                    string startElementName = string.Empty; string returnXpath = string.Empty; string xmlNodeReaderValue = string.Empty;

                    Regex regex = new Regex(RegExPattern);
                    XmlNodeReader xmlNodeReader = new XmlNodeReader(xmlDocument);

                    while (xmlNodeReader.Read())
                    {
                        if (xmlNodeReader.NodeType == XmlNodeType.Element)
                        {
                            startElementName = xmlNodeReader.Name;

                            if (xmlNodeReader.HasAttributes)
                            {
                                for (int i = 0; i < xmlNodeReader.AttributeCount; i++)
                                {
                                    xmlNodeReader.MoveToAttribute(i);
                                    if (regex.IsMatch(xmlNodeReader.Value))
                                    {
                                        var reader = new XmlNodeReader(xmlDocument);
                                        XDocument xdoc1 = XDocument.Load(reader);
                                        parentNodes = from node in xdoc1.Descendants(startElementName) where node.Attribute(xmlNodeReader.Name.ToString()).Value.Contains(xmlNodeReader.Value.ToString()) select node;

                                        foreach (var parentNode in parentNodes)
                                        {
                                            string path = GetXpath(parentNodes.First());
                                            returnXpath = path.Substring(0, path.LastIndexOf("^") + 1) + path.Substring(path.LastIndexOf("^") + 2) + "/@" + xmlNodeReader.Name.ToString();
                                            //xmlNodeReaderValue = xmlDocument.SelectSingleNode("//" + returnXpath.Replace("^", "/")).InnerXml.ToString();
                                            results.Add(returnXpath);
                                        }
                                    }
                                }
                            }
                        }
                        else if (xmlNodeReader.NodeType == XmlNodeType.Text)
                        {
                            if (regex.IsMatch(xmlNodeReader.Value))
                            {
                                var reader = new XmlNodeReader(xmlDocument);
                                XDocument xdoc1 = XDocument.Load(reader);
                                parentNodes = from node in xdoc1.Descendants(startElementName) where node.Value.Contains(xmlNodeReader.Value) select node;

                                foreach (var parentNode in parentNodes)
                                {
                                    string path = GetXpath(parentNode);
                                    returnXpath = path.Substring(0, path.LastIndexOf("^") + 1) + path.Substring(path.LastIndexOf("^") + 2);
                                    //xmlNodeReaderValue = xmlDocument.SelectSingleNode("//" + returnXpath.Replace("^", "/")).InnerXml.ToString();
                                    results.Add(returnXpath);
                                }
                            }
                        }
                    }
                }
                catch { }

                results = results.Distinct().ToList();
                return results;
            }

            private static string FindAndAddSpecialCharList(XmlDocument xmlDocument, string startElementName, string xmlNodeReaderValue, string attributeName, XmlNodeReader xmlNodeReader)
            {
                using (var reader = new XmlNodeReader(xmlDocument))
                {
                    XDocument xdoc1 = XDocument.Load(reader);
                    IEnumerable<XElement> nodes = null;

                    var specialCharElements = xdoc1.Descendants().Where(e => e.Elements().Any(a => a.Value == xmlNodeReader.Value));

                    if (string.IsNullOrEmpty(attributeName))
                        nodes = from node in xdoc1.Descendants(startElementName) where node.Value.Contains(xmlNodeReaderValue) select node;
                    else
                        nodes = from node in xdoc1.Descendants(startElementName) where node.Attribute(attributeName).Value.Contains(xmlNodeReaderValue) select node;

                    var parents = nodes.Ancestors().First();

                    string path = GetXpath(parents);

                    if (string.IsNullOrEmpty(attributeName))
                        return (path.Substring(0, path.LastIndexOf("^") + 1) + path.Substring(path.LastIndexOf("^") + 2) + "/" + startElementName);
                    else
                    {
                        bool idAttributeFound = false;
                        for (int i = 0; i < xmlNodeReader.AttributeCount; i++)
                        {
                            xmlNodeReader.MoveToAttribute(i);
                            if (xmlNodeReader.Name == "id")
                            {
                                idAttributeFound = true;
                                return (path.Substring(0, path.LastIndexOf("^") + 1) + path.Substring(path.LastIndexOf("^") + 2) + "/" + startElementName + "['@" + xmlNodeReader.Name + "=" + xmlNodeReader.Value + "']/@" + attributeName);
                            }
                        }
                        if (idAttributeFound == false)
                            return (path.Substring(0, path.LastIndexOf("^") + 1) + path.Substring(path.LastIndexOf("^") + 2) + "/" + startElementName + "/" + attributeName);
                    }
                }
                return null;
            }

            private static List<string> GetXpathsWhereWeHaveSpecialChar_New(XmlDocument xmlDocument)
            {
                List<string> results = new List<string>();
                try
                {
                    XmlNodeReader xmlNodeReader = new XmlNodeReader(xmlDocument);
                    while (xmlNodeReader.Read())
                    {
                        Regex regex = new Regex(RegExPattern);
                        if (xmlNodeReader.NodeType == XmlNodeType.Text)
                        {
                            if (regex.IsMatch(xmlNodeReader.Value))
                            {
                                using (var reader = new XmlNodeReader(xmlDocument))
                                {
                                    XDocument xdoc1 = XDocument.Load(reader);
                                    var specialCharElements = xdoc1.Descendants().Where(e => e.Elements().Any(a => a.Value == xmlNodeReader.Value));

                                    var parents = specialCharElements.Ancestors().Reverse().Select(a => new XElement(a.Name.LocalName)).ToArray();
                                    XElement root1 = new XElement(xdoc1.Root.Name.LocalName);
                                    XDocument xdoc22 = new XDocument(root1);
                                    XElement lastAdded = root1;
                                    for (int i = 1; i < parents.Count(); i++)
                                    {
                                        lastAdded.Add(parents[i]);
                                        lastAdded = parents[i];
                                    }

                                    lastAdded.Add(specialCharElements);

                                    foreach (XElement specialCharElement in specialCharElements)
                                    {
                                        string path = GetXpath(specialCharElement);
                                        results.Add(path);
                                    }
                                }
                            }
                        }
                    }
                }
                catch { }

                return results;
            }

            private static string BuildReuqestToRemoveSpecialCharFromDCXML_OLD(string policyID, string historyID, List<string> xPaths, string pageManuScriptID)
            {
                string results = string.Empty;

                string _tempRequest = File.ReadAllText(HostingEnvironment.MapPath(ReprocessSubmissionScriptsPath + "SampleReuqestToRemoveSpecialCharFromDCXML" + ".txt"));
                _tempRequest = _tempRequest.Replace("&policyID&", policyID).Replace("&historyID&", historyID).Replace("&PageManuScriptID&", pageManuScriptID);

                StringBuilder getElementRqs = new StringBuilder();
                StringBuilder setElementRqs = new StringBuilder();

                foreach (string xPath in xPaths)
                {
                    getElementRqs.Append("<Session.getElementRq path=\"" + xPath + "\"/>");
                    setElementRqs.Append("<Session.setElementRq path=\"" + xPath + "\" value=\"\"/>");
                }

                _tempRequest = _tempRequest.Replace("<getElementRqs />", getElementRqs.ToString());
                _tempRequest = _tempRequest.Replace("<setElementRqs />", setElementRqs.ToString());

                return results;
            }

            private static string BuildReuqestToRemoveSpecialCharFromDCXML(string policyID, string historyID, List<string> xPaths, string pageManuScriptID, string DCTServerURL)
            {
                string results = string.Empty;

                string _tempRequest = File.ReadAllText(HostingEnvironment.MapPath(ReprocessSubmissionScriptsPath + "SampleReuqestToRemoveSpecialCharFromDCXML" + ".txt"));
                _tempRequest = _tempRequest.Replace("&policyID&", policyID).Replace("&historyID&", historyID).Replace("&PageManuScriptID&", pageManuScriptID);

                StringBuilder getElementRqs = new StringBuilder();
                StringBuilder setElementRqs = new StringBuilder();

                foreach (string xPath in xPaths)
                {
                    string value = string.Empty;
                    XmlDocument getElementRs = Common.GetElementRqUsingHistoryID(DCTServerURL, policyID, historyID, xPath);
                    if (getElementRs.SelectSingleNode("/server/responses/OnlineData.loadPolicyRs/@status").Value == Success)
                    {
                        if (getElementRs.SelectSingleNode("/server/responses/OnlineData.loadHistoryRs/@status").Value == Success)
                        {
                            if (getElementRs.SelectSingleNode("/server/responses/Session.getElementRs/@status").Value == Success)
                            {
                                value = RemoveSpecialCharecterFromAnyString(getElementRs.SelectSingleNode("/server/responses/Session.getElementRs/@value").Value);
                            }
                        }
                    }

                    getElementRqs.Append("<Session.getElementRq path=\"" + xPath + "\"/>");
                    setElementRqs.Append("<Session.setElementRq path=\"" + xPath + "\" value=\"" + value + "\"/>");
                }

                _tempRequest = _tempRequest.Replace("<getElementRqs />", getElementRqs.ToString());
                _tempRequest = _tempRequest.Replace("<setElementRqs />", setElementRqs.ToString());

                return _tempRequest;
            }

            private static string RemoveSpecialCharecterFromAnyString(string item)
            {
                var results = string.Empty;

                // Replace know special char.
                results = item.Replace("–", "-").Replace("â", "a").Replace("\"", "&quot;").Replace("’", "'");

                // Remove any unknow special char.
                results = Regex.Replace(results, RegExPattern, string.Empty);

                if (item == results) { return string.Empty; }
                return results;
            }

            private static string GetXpath(XmlNode xmlNode)
            {
                if (xmlNode.ParentNode == null)
                {
                    return "/" + xmlNode.Name;
                }

                return GetXpath(xmlNode.ParentNode) + "/" + xmlNode.Name;
            }

            private static string GetXpath(XElement xElement)
            {
                if (xElement.Parent == null)
                    return xElement.Name.ToString() + "^";
                else
                {
                    if (xElement.Name == "session" || xElement.Name == "_processingData") return xElement.Name.ToString() + "^";

                    if (xElement.Name == "data" || xElement.Name == "policyAdmin")
                        return GetXpath(xElement.Parent) + "/" + xElement.Name;
                    else
                    {
                        if (xElement.HasAttributes)
                        {
                            bool idAttributeFound = false;
                            IEnumerable<XAttribute> listOfAttributes = from att in xElement.Attributes() select att;
                            foreach (XAttribute a in listOfAttributes)
                            {
                                if (a.Name == "id")
                                {
                                    idAttributeFound = true;
                                    return GetXpath(xElement.Parent) + "/" + xElement.Name + "[@" + a.Name + "='" + a.Value + "']";
                                }
                            }
                            if (idAttributeFound == false) return GetXpath(xElement.Parent) + "/" + xElement.Name;
                        }
                        else
                            return GetXpath(xElement.Parent) + "/" + xElement.Name;
                    }
                }
                return GetXpath(xElement.Parent) + "/" + xElement.Name;
            }

            private static string StoreTheFile(string strWhatToStore, string strTypeOfFileToStore)
            {
                if (!string.IsNullOrEmpty(strTypeOfFileToStore))
                {
                    string outputFileName = strTypeOfFileToStore + "_Script__" + DateTime.Now.ToString("hh_mm_ss_fff_tt") + ".txt";
                    string outputFilepath = HostingEnvironment.MapPath("~/App_Data/ReprocessSubmission/Output_") + strTypeOfFileToStore + "_Script\\" + DateTime.Today.ToString("yyyyMMdd");
                    Directory.CreateDirectory(outputFilepath);

                    if (Directory.Exists(outputFilepath))
                    {
                        using (StreamWriter writer = new StreamWriter(outputFilepath + "\\" + outputFileName, true))
                        {
                            writer.WriteLine(strWhatToStore.ToString());
                            writer.Close();
                        }
                        return outputFilepath + "\\" + outputFileName;
                    }
                }

                return "Error";
            }

            private static string BuildReprocessingScript(string policyNumber, string Submission, string Version, string policyID, string transGroup, string FormManuscriptID, string historyID, string DCTServerURL, bool isOOS)
            {
                try
                {
                    string retryScriptPath = string.Empty;

                    if (Common.ValidateMLPolicySymbol(policyNumber))
                    {
                        if (isOOS) { retryScriptPath = ""; }
                        else { retryScriptPath = GetRetryScriptPath("DUCKRetry_ML"); }
                    }
                    else
                    {
                        if (isOOS) { retryScriptPath = GetRetryScriptPath("DUCKRetry_OOS"); }
                        else { retryScriptPath = GetRetryScriptPath("DUCKRetry"); }
                    }

                    XmlDocument Script = new XmlDocument();
                    if (!string.IsNullOrEmpty(retryScriptPath)) { Script.Load(HostingEnvironment.MapPath(retryScriptPath)); }

                    if (!string.IsNullOrEmpty(Script.InnerXml))
                    {
                        Script.InnerXml = Script.InnerXml.Replace("$policyID", policyID).Replace("$transGroup", transGroup).Replace("$FormManuscriptID", FormManuscriptID)
                            .Replace("$SubmissionID", Submission).Replace("$Version", Version).Replace("$historyID", historyID);

                        if (isOOS == false)
                        {
                            string token = GetBarrerToken(DCTServerURL);
                            Script.InnerXml = Script.InnerXml.Replace("$bearerToken", "Bearer " + token);
                        }

                        return Script.InnerXml;
                    }

                    return string.Empty;
                }
                catch (Exception x)
                {
                    string message = x.Message;
                    return string.Empty;
                }
            }

            private static string GetBarrerToken(string DCTServerURL)
            {
                string result = string.Empty;

                try
                {
                    StringBuilder CustomServerProcessRq = new StringBuilder();
                    CustomServerProcessRq.Append("<Settings.getPropertyRq name=\"Carrier_Integration_Flags.OAuthClientID\" noexpand=\"True\" var.OAuthClientID=\"//Settings.getPropertyRs/@value\"/>");
                    CustomServerProcessRq.Append("<Settings.getPropertyRq name=\"Carrier_Integration_Flags.OAuthClientSecret\" noexpand=\"True\" var.OAuthClientSecret=\"//Settings.getPropertyRs/@value\"/>");
                    CustomServerProcessRq.Append("<Settings.getPropertyRq name=\"Carrier_Integration_Flags.OAuthURL\" noexpand=\"True\" var.OAuthURL=\"//Settings.getPropertyRs/@value\"/>");
                    CustomServerProcessRq.Append("<Send.httpRq url=\"~OAuthURL~\" verb=\"POST\" responseSessionPath=\"_IntegrationData^OAuthAccessToken\" var.OAUTH=\"concat('Bearer ',//body/access_token)\">");
                    CustomServerProcessRq.Append("<headers>");
                    CustomServerProcessRq.Append("<header name=\"content-type\" value=\"application/x-www-form-urlencoded\"/>");
                    CustomServerProcessRq.Append("</headers>");
                    CustomServerProcessRq.Append("<query>");
                    CustomServerProcessRq.Append("<parameter name=\"grant_type\" value=\"client_credentials\"/>");
                    CustomServerProcessRq.Append("<parameter name=\"client_id\" value=\"~OAuthClientID~\"/>");
                    CustomServerProcessRq.Append("<parameter name=\"client_secret\" value=\"~OAuthClientSecret~\"/>");
                    CustomServerProcessRq.Append("</query>");
                    CustomServerProcessRq.Append("</Send.httpRq>");

                    XmlDocument CustomServerProcessRs = Common.PostDCTRequestReturnResponse(CustomServerProcessRq.ToString(), DCTServerURL);

                    if (CustomServerProcessRs.SelectSingleNode("/server/responses/Send.httpRs/@status").Value == Success)
                    {
                        if (!string.IsNullOrEmpty(CustomServerProcessRs.SelectSingleNode("/server/responses/Send.httpRs/body/access_token").InnerText))
                        {
                            result = CustomServerProcessRs.SelectSingleNode("/server/responses/Send.httpRs/body/access_token").InnerText;
                        }
                    }
                }
                catch { }

                return result;
            }

            private static string GetRetryScriptPath(string Type)
            {
                switch (Type)
                {
                    case "DUCKRetry":
                        return ReprocessScriptsPath + "Insequence retry Package Monoline.xml";

                    case "DUCKRetry_OOS":
                        return ReprocessScriptsPath + "OOS retry Package Monoline - New.xml";

                    case "DUCKRetry_ML":
                        return ReprocessScriptsPath + "Insequence retry Mark Logic.xml";

                    default:
                        return "";
                }
            }
        }
    }
}