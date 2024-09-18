using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Xml;

namespace MindTreeValueAdds.Web.Models
{
    public class ExtractSessionData
    {
        static readonly string Success = "success";
        static readonly string Fail = "fail";

        public static class UserDefineFunctions
        {
            internal static Tuple<string, string> GenerateReport(string Server, string OriginalSubmissionID, string SubmissionID, string VersionNumber, string DCTServerURL)
            {
                string status = Fail;
                string result = string.Empty;

                XmlDocument getAllDocumentsRs = Common.GetAllDocumentsRq(DCTServerURL, OriginalSubmissionID, VersionNumber, SubmissionID);

                if (!string.IsNullOrEmpty(getAllDocumentsRs.OuterXml))
                {
                    result = "Oops. Unable to fetch DCXML from DUCK.";

                    try
                    {
                        if (getAllDocumentsRs.SelectSingleNode("/server/responses/CustomServer.processRs/server/responses/OnlineData.loadPolicyRs/@status").Value == Success)
                        {
                            if (getAllDocumentsRs.SelectSingleNode("/server/responses/CustomServer.processRs/server/responses/Session.getAllDocumentsRs/@status").Value == Success)
                            {
                                string LOB = getAllDocumentsRs.SelectSingleNode("/server/responses/CustomServer.processRs/server/responses/Session.getAllDocumentsRs/session/data/policy/LineOfBusiness").InnerText;
                                string TransactionType = getAllDocumentsRs.SelectSingleNode("/server/responses/CustomServer.processRs/server/responses/Session.getAllDocumentsRs/session/data/policyAdmin/transactions/transaction[SubmissionID='" + SubmissionID + "']/Type").InnerText;

                                if (CheckLOBConfiguration(LOB) && CheckTransactionTypeConfiguration(TransactionType))
                                {
                                    XmlDocument FinalReport = new XmlDocument();
                                    FinalReport.LoadXml(Common.TransformXML(getAllDocumentsRs, HostingEnvironment.MapPath("~/XSLT/ExtractSessionData/FinalReport_V2/XSLT_In.xslt")));

                                    if (string.IsNullOrEmpty(FinalReport.OuterXml)) { result = "Oops. Unable to transform the session data."; }

                                    string filePath = StoreTheFile(FinalReport.OuterXml, "Request");
                                    if (string.IsNullOrEmpty(filePath) || filePath == "Error") { result = "Oops. Unable save the session data."; }

                                    status = Success;
                                    result = Success + "==" + filePath;
                                }
                                else { result = "Oops. Line of Business or Transaction Type is not yet configured."; }
                            }
                        }                        
                    }
                    catch { }
                }

                return Tuple.Create(status, result);
            }

            static bool CheckLOBConfiguration(string LOB)
            {
                return true;
            }

            static bool CheckTransactionTypeConfiguration(string transactionType)
            {
                return true;
            }

            private static string StoreTheFile(string strWhatToStore, string strTypeOfFileToStore)
            {
                if (!string.IsNullOrEmpty(strTypeOfFileToStore))
                {
                    string outputFileName = strTypeOfFileToStore + "_Script__" + DateTime.Now.ToString("hh_mm_ss_fff_tt") + ".xml";
                    string outputFilepath = HostingEnvironment.MapPath("~/App_Data/ExtractSessionData/Output_") + strTypeOfFileToStore + "_Script\\" + DateTime.Today.ToString("yyyyMMdd");
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
        }
    }
}