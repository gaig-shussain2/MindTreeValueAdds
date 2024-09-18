using System;
using System.IO;
using System.Web.Hosting;

namespace MindTreeValueAdds.Web.Models
{
    public static class Logger
    {
        public static void logWarning(string message, string additionalInfo, bool logger)
        {
            if (logger)
            {
                string result = string.Format("{0},{1},{2},{3}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "warning", additionalInfo, message);
                Writelog(result);
            }
        }

        public static void logInformation(string message, string additionalInfo, bool logger)
        {
            if (logger)
            {
                string result = string.Format("{0},{1},{2},{3}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "info", additionalInfo, message);
                Writelog(result);
            }

        }

        public static void logException(Exception exception, string additionalInfo)
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

            string result = string.Format("{0},{1},{2},{3}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "error", additionalInfo, message);
            Writelog(result);
        }

        private static void Writelog(string log)
        {
            string serverMapPath = HostingEnvironment.MapPath("~/App_Data/ErrorLog/" + String.Format("{0} {1:ddMMyyyy}", "log ", DateTime.Now) + ".log");

            if (!File.Exists(serverMapPath)) { File.Create(serverMapPath).Close(); }

            using (StreamWriter writer = new StreamWriter(serverMapPath, true))
            {
                writer.WriteLine(log);
                writer.Flush();
                writer.Close();
            }
        }
    }
}

