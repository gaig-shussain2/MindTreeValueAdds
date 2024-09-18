using System;
using System.IO;
using System.Web.Hosting;

namespace MindTreeValueAdds.API.Models
{
    public class Common
    {
        internal static void LogException(Exception exception, string errorLogFileName)
        {
            string message = string.Format("Time: {0}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
            message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;
            message += string.Format("Message: {0}", exception.Message);
            message += Environment.NewLine;
            message += string.Format("StackTrace: {0}", exception.StackTrace);
            message += Environment.NewLine;
            message += string.Format("Source: {0}", exception.Source);
            message += Environment.NewLine;
            message += string.Format("TargetSite: {0}", Convert.ToString(exception.TargetSite));
            message += Environment.NewLine;

            string serverMapPath = string.Empty;
            if (string.IsNullOrEmpty(errorLogFileName) & errorLogFileName == "") { serverMapPath = HostingEnvironment.MapPath("~/App_Data/ErrorLog/ErrorLog.txt"); }
            else { serverMapPath = HostingEnvironment.MapPath("~/App_Data/ErrorLog/" + errorLogFileName + ".txt"); }

            using (StreamWriter writer = new StreamWriter(serverMapPath, true))
            {
                writer.WriteLine(message);
                writer.Close();
            }
        }
    }
}