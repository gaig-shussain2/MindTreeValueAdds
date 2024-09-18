using System;
using System.IO;
using System.Web.Mvc;
using System.Web.Hosting;
using System.Configuration;
using System.Collections.Generic;
using MindTreeValueAdds.Web.Models;

namespace MindTreeValueAdds.Web.Controllers
{
	public class ReprocessSubmissionController : Controller
	{
		private static NLog.Logger nLogLogger = NLog.LogManager.GetCurrentClassLogger();
        public static string globalPath_Output = HostingEnvironment.MapPath("~/App_Data/ReprocessSubmission/Output_Request_Script");

		public ActionResult Index()
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

            // Delete all output folders created in past.
            //foreach (var directory in Directory.GetDirectories(globalPath_Output)) { if (directory != globalPath_Output + DateTime.Today.ToString("yyyyMMdd")) { Directory.Delete(directory, true); } }

            ViewBag.Server = Servers;
			return View();
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

		public JsonResult ReprocessTransaction(string Server, string OriginalSubmissionID, string Submission, string VersionNumber)
		{
			if (!string.IsNullOrEmpty(Server) && !string.IsNullOrEmpty(OriginalSubmissionID) && !string.IsNullOrEmpty(Submission) && !string.IsNullOrEmpty(VersionNumber))
			{
				nLogLogger.Info("--------------------------------------------------------------" + Environment.NewLine);
				nLogLogger.Info(" " + Session["LoginUserName"] + " " + " Checking Special Character Process is started. Input Details : " + Server + ", " + OriginalSubmissionID + ", " + Submission + ", " + VersionNumber);

				string DCTServerURL = Common.ReturnValidDUCKServerURL(Server);
				string ReprocessOOSTransactionRs = ReprocessSubmission.UserDefineFunctions.ReprocessAnyTransaction(OriginalSubmissionID, Submission, VersionNumber, DCTServerURL, Server);

				if (string.IsNullOrEmpty(ReprocessOOSTransactionRs))
				{
                    nLogLogger.Info(" " + Session["LoginUserName"] + " " + " Reprocess failed. Input Details : " + Server + ", " + OriginalSubmissionID + ", " + Submission + ", " + VersionNumber);
                    Json("Oops. Something went wrong.", JsonRequestBehavior.AllowGet);
				}

                nLogLogger.Info(" " + Session["LoginUserName"] + " " + " Reprocess complited. Input Details : " + Server + ", " + OriginalSubmissionID + ", " + Submission + ", " + VersionNumber);
                return Json(ReprocessOOSTransactionRs, JsonRequestBehavior.AllowGet);
			}
			else { return Json("Oops. Invalid/Insufficient Parametrs.", JsonRequestBehavior.AllowGet); }
		}

		public ActionResult DownloadDBScriptt(string FileName)
		{
			string FileNotFoundMessage = "Oops. File not found.";

			if (!string.IsNullOrEmpty(FileName))
			{
				if (System.IO.File.Exists(FileName)) { return File(FileName, "text/plain", "Request_Script.txt"); }
				else { return Content(FileNotFoundMessage); }
			}
			else { return Content(FileNotFoundMessage); }

		}
	}
}