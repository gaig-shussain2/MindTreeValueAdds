using System.Linq;
using System.Collections.Generic;

namespace MindTreeValueAdds.Web.Models
{
    public class PCF
    {
        public static readonly string SUCCESS = "success";

        public class UserDefineFunctions
        {
            internal static string GetBUFromShortDescription(string shortDescription)
            {
                switch (shortDescription)
                {
                    case "ESB - RefAppDownstream Error (Application : CUBE-Alternative Markets)":
                        return "ALT";

                    case "ESB - DuckCreek Print Error (Application : CUBE-Alternative Markets)":
                        return "ALT";

                    case "ESB - RefAppDownstream Error (Application = 'CUBE-Alternative Markets')":
                        return "ALT";

                    case "ESB - RefAppDownstream Error (Application : CUBE-Specialty Human Services)":
                        return "SHS";

                    case "ESB - DuckCreek Print Error (Application : CUBE-Specialty Human Services)":
                        return "SHS";

                    case "ESB - RefAppDownstream Error (Application = 'CUBE-Specialty Human Services')":
                        return "SHS";

                    case "ESB - RefAppDownstream Error (Application = 'CUBE-Strategic Comp')":
                        return "STG COMP";

                    case "ESB - DuckCreek Print Error (Application : CUBE-Strategic Comp)":
                        return "STG COMP";

                    case "ESB - RefAppDownstream Error (Application : CUBE-AgriBusiness)":
                        return "AGRI";

                    case "ESB - DuckCreek Print Error (Application : CUBE-AgriBusiness)":
                        return "AGRI";

                    case "ESB - RefAppDownstream Error (Application : CUBE-Ocean Marine Divison)":
                        return "OM";

                    case "ESB - DuckCreek Print Error (Application : CUBE-Ocean Marine Divison)":
                        return "OM";

                    case "ESB - RefAppDownstream Error (Application : CUBE-ABA Insurance Services)":
                        return "ABA";

                    case "ESB - DuckCreek Print Error (Application : CUBE-ABA Insurance Services)":
                        return "ABA";

                    default:
                        return "";
                }
            }

            internal static Dictionary<string, string> GetRelatedSubmissionData(string submission, string DCTServerURL, string server)
            {
                Dictionary<string, string> result = new Dictionary<string, string>();

                try
                {
                    string EXXML = Common.GetXMLBasedonXMLTypeFromCUBEDB(submission, "exxml", server);

                    if (!string.IsNullOrEmpty(EXXML))
                    {
                        string azureFileName = EXXML.Replace("<Policy_Application><Policy_Transaction><FileName>", "").
                            Replace(".xml</FileName></Policy_Transaction></Policy_Application>", "");

                        if (!string.IsNullOrEmpty(azureFileName))
                        {
                            string[] policyData = azureFileName.Split('-');

                            if (policyData.Count() > 0)
                            {
                                string policyNumber = policyData[0];
                                string VersionNumber = string.Empty;

                                var policyDataUsingOnlineDataListRq = Common.GetPolicyDataUsingOnlineDataListRq(policyNumber, DCTServerURL);
                                var listTransactionsRs = Common.ListTransactionsRq(policyNumber, DCTServerURL);

                                if (listTransactionsRs.SelectSingleNode("/server/responses/OnlineData.loadPolicyRs/@status").InnerText == SUCCESS)
                                {
                                    if (listTransactionsRs.SelectSingleNode("/server/responses/TransACT.listTransactionsRs/@status").InnerText == SUCCESS)
                                    {
                                        VersionNumber = listTransactionsRs.SelectSingleNode("/server/responses/TransACT.listTransactionsRs/transactions/transaction[SubmissionID='" + submission + "']/VersionNumber").InnerText;
                                    }
                                }

                                if (!string.IsNullOrEmpty(policyDataUsingOnlineDataListRq["policyID"]) &&
                                    !string.IsNullOrEmpty(policyDataUsingOnlineDataListRq["OriginalSubmissionID"]) &&
                                    !string.IsNullOrEmpty(VersionNumber))
                                {
                                    result.Add("policyNumber", policyNumber);
                                    result.Add("policyID", policyDataUsingOnlineDataListRq["policyID"]);
                                    result.Add("OriginalSubmissionID", policyDataUsingOnlineDataListRq["OriginalSubmissionID"]);
                                    result.Add("TransactionType", policyData[1]);
                                    result.Add("VersionNumber", VersionNumber);
                                    result.Add("AzureFileName", azureFileName);
                                }
                            }
                        }
                    }
                }
                catch { }

                return result;
            }
        }
    }
}