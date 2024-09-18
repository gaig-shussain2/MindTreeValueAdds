using System.IO;
using System.Web.Hosting;

namespace MindTreeValueAdds.API.Models
{
    public class OOBBalanceReport
    {
        static readonly string DBScriptsPath = "~/App_Data/DB Scripts/OOB/";

        internal static string GetSQLScript(string Type)
        {
            switch (Type)
            {
                case "FetchCubeManageActivityScreenDetails":
                    return File.ReadAllText(HostingEnvironment.MapPath(DBScriptsPath + "FetchCubeManageActivityScreenDetails.txt"));

                case "FetchCubeManageActivityScreenDetailsForSubmission":
                    return File.ReadAllText(HostingEnvironment.MapPath(DBScriptsPath + "FetchCubeManageActivityScreenDetailsForSubmission.txt"));

                case "BDMScript":
                    return File.ReadAllText(HostingEnvironment.MapPath(DBScriptsPath + "BDMScript.txt"));

                case "EDWNonTax":
                    return "select * from qdm.VW_TRANS_PREMIUM_CLSMDMKEY where POL_NO_CD in (&PolicyNo) and POL_MODULE_CD in (&PolicyMod) and POL_VERSION_CD in (&POL_VERSION_CD)";

                case "EDWTax":
                    return "select * from qdm.VW_TRANS_NON_PREMIUM where POL_NO_CD in (&PolicyNo) and POL_MODULE_CD in (&PolicyMod) and POL_VERSION_CD in (&POL_VERSION_CD)";

                case "ADWNonTax":
                    return File.ReadAllText(HostingEnvironment.MapPath(DBScriptsPath + "ADWNonTax.txt"));

                case "ADWTax":
                    return File.ReadAllText(HostingEnvironment.MapPath(DBScriptsPath + "ADWTax.txt"));

                default:
                    return "";
            }
        }
    }
}