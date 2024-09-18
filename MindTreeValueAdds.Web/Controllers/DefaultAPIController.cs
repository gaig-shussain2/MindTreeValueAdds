using System.Web.Http;
using MindTreeValueAdds.Web.Models;

namespace MindTreeValueAdds.Web.Controllers
{
    [RoutePrefix("api")]
    public class DefaultAPIController : ApiController
    {
        [HttpGet]
        [Route("pcf/GetPCFData")]
        public IHttpActionResult GetPCFData(string submission, string Server)
        {
            if (string.IsNullOrEmpty(submission) || string.IsNullOrEmpty(Server)) { return BadRequest("Submission ID & Server are name are important."); }

            var relatedSubmissionData = PCF.UserDefineFunctions.GetRelatedSubmissionData(submission, Common.ReturnValidDUCKServerURL(Server), Server);
            if (!string.IsNullOrEmpty(relatedSubmissionData["policyNumber"]) && 
                !string.IsNullOrEmpty(relatedSubmissionData["policyID"]) &&
                !string.IsNullOrEmpty(relatedSubmissionData["OriginalSubmissionID"]) &&
                !string.IsNullOrEmpty(relatedSubmissionData["TransactionType"]) &&
                !string.IsNullOrEmpty(relatedSubmissionData["VersionNumber"]) &&
                !string.IsNullOrEmpty(relatedSubmissionData["AzureFileName"]))
            {
                return Ok(relatedSubmissionData);
            }

            return NotFound();
        }

        [HttpGet]
        [Route("pcf/test")]
        public IHttpActionResult test()
        {
            return Ok();
        }
    }
}
