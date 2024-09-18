using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;

namespace MindTreeValueAdds.Web.Models
{
    public class LoginDtl
    {
        private static NLog.Logger nLogLogger = NLog.LogManager.GetCurrentClassLogger();
        public string idToken { get; set; }
        public int tokenExpiryTime { get; set; }
        public string loginUserName { get; set; }

        private static LoginDtl instance;
        public static string env = ConfigurationManager.AppSettings["SSOEnv"];
        public static string authorization_endpoint = ConfigurationManager.AppSettings[env + "authorization_endpoint"];
        public static string client_id = ConfigurationManager.AppSettings[env + "client_id"];
        public static string client_secret = ConfigurationManager.AppSettings[env + "client_secret"];
        public static string scope = ConfigurationManager.AppSettings[env + "scope"];
        public static string redirect_url = ConfigurationManager.AppSettings[env + "redirect_url"];
        public static string issuer = ConfigurationManager.AppSettings[env + "issuer"];
        public static string token_endpoint = ConfigurationManager.AppSettings[env + "token_endpoint"];
        public static string logout_endpoint = ConfigurationManager.AppSettings[env + "logout_endpoint"];
        public static string base_url = ConfigurationManager.AppSettings[env + "base_url"];

        public String GetLoginReqUrl()
        {
            return authorization_endpoint + "?client_id=" + client_id + "&response_type=code"
               + "&scope=" + scope + "&state=ok" + "&redirect_uri=" + redirect_url;
        }

        public string GetLogout_Endpoint() { return logout_endpoint; }

        public string GetBase_Url() { return base_url; }

        public string GetIssuer() { return issuer; }

        public string GetToken(string code)
        {
            var values = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code"},
                { "client_id", client_id},
                { "client_secret", client_secret},
                { "code" , code},
                { "redirect_uri", redirect_url}
            };

            nLogLogger.Info("client_id" + client_id);

            HttpClient tokenClient = new HttpClient();
            var content = new FormUrlEncodedContent(values);
            var response = tokenClient.PostAsync(token_endpoint, content).Result;

            nLogLogger.Info("response" + response);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content;

                return responseContent.ReadAsStringAsync().Result;
            }

            return ("Error: Token request failed with status code: " + response.StatusCode);
        }

        public string GetToken(string code, bool isRunningInLocalHost)
        {
            if (isRunningInLocalHost)
            {
                JObject jsonObj = JObject.Parse("{\"token_type\":\"Bearer\",\"expires_in\":3600,\"access_token\":\"eyJraWQiOiJ6OG1vMWlaRXc3c2R4enlwUjlsNlAxZEtTamprRXA3NjdTYWUzSmtoUXFBIiwiYWxnIjoiUlMyNTYifQ.eyJ2ZXIiOjEsImp0aSI6IkFULnBEdzdEaGZsNEI1ZnN3U0xndV9XOWotTWRDOEpjYlNPektsU3hVUjlIT28iLCJpc3MiOiJodHRwczovL2xvZ2luZGV2LmdhaWcuY29tIiwiYXVkIjoiaHR0cHM6Ly9sb2dpbmRldi5nYWlnLmNvbSIsInN1YiI6Im1rcmlzaG5hc2FteSIsImlhdCI6MTcwMjY4NjU1MywiZXhwIjoxNzAyNjkwMTUzLCJjaWQiOiIwb2Fic3Y4bXljSnJTSjVVaTFkNyIsInVpZCI6IjAwdWJ0MXpzcDdZM1FvRUF2MWQ3Iiwic2NwIjpbIm9wZW5pZCIsInByb2ZpbGUiXSwiYXV0aF90aW1lIjoxNzAyNjg2NTMxfQ.WxlljQxlb-fIzdLT5QU0wwChhC2n_M1tzYtOF--UFiLTD7qYnuGW4ajpX23xfnPDH6tb1rO22SqAmhq-s9G9UXs8OMhsqbstfxJtXaaL6QONz9aqSUOALB2yaBd7Pknisl0LFnjG9SOU_RHCVlf07bcNRkawAPZvm_iVWNvbPF1nxv_aIAed9Jl2iTBRmSkF2l4AIYOPYeV-45H5K7he47XM5EY3Y7EPauwhfLQG_98N863K9Rt5baYHjUs6sJYZQDZZSyJ9qP-yOqxVUkMP3pU2eLVsXEm1eE5y-bGp7ueA1Q2_XnwsuEYPC6PT1HEe1mFGIbcTLTfIeX8ajbH-NA\",\"scope\":\"openid profile\",\"id_token\":\"eyJraWQiOiJvNGJJa0NrNVRPSW5VM0tldXpNQzZiaGJRU3ZsZ2FrOTFJTk5PV2hhdHYwIiwiYWxnIjoiUlMyNTYifQ.eyJzdWIiOiIwMHVidDF6c3A3WTNRb0VBdjFkNyIsIm5hbWUiOiJNdXRodWt1bWFyIEtyaXNobmFzYW15IiwidmVyIjoxLCJpc3MiOiJodHRwczovL2xvZ2luZGV2LmdhaWcuY29tIiwiYXVkIjoiMG9hYnN2OG15Y0pyU0o1VWkxZDciLCJpYXQiOjE3MDI2ODY1NTMsImV4cCI6MTcwMjY5MDE1MywianRpIjoiSUQuT1NyalRaejVWWXFkYkdSTU5FdWpJMGJmLXhFMkJIc2hyTG5CUHV3SEpsTSIsImFtciI6WyJtZmEiLCJvdHAiLCJwd2QiXSwiaWRwIjoiMDBvMW93eWJ6RjRITHhCMDMxZDYiLCJwcmVmZXJyZWRfdXNlcm5hbWUiOiJta3Jpc2huYXNhbXkiLCJhdXRoX3RpbWUiOjE3MDI2ODY1MzEsImF0X2hhc2giOiJiaW9OdlM4RlI2WXN2N1JhRUd1NFRBIn0.J1WDyox_4PtNx2ULdGUZnCska9NYNfpjVV_huQW_7zMwbehk-ymtm38gFZDUHFLh_XMHuNxxRGr9aZI4XFvmav_CP232-69-mvwMD8tRBKkU4FB3di4dXdVcvAh5Q4jc0Oq6lJerMt8mfHtGCeq5umaPzzc88ScZHU-RpKd7nBbO-f2hiU0vb734Df17LCz9vVa4N3sPXKMbCNQ9VLQT4T3c3i4VMI8eYMzV8VFDe66_nQRYsJnBiDfkKbCCes5I4_eCkc77SIrl8cJUFCZJp0w0WJhFeYjkXNARuODEXFUrbdqyvr3LmId22Quqsm7_QMwcB66m1-L1g6swtj9V8Q\"}");
                return jsonObj.ToString();
            }

            return string.Empty;
        }

        public static LoginDtl Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LoginDtl();
                }
                return instance;
            }
        }
    }
}