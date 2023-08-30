using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Newtonsoft.Json;

namespace WorkWithBpmByWebServices
{
    class ResponseStatus
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public object Exception { get; set; }
        public object PasswordChangeUrl { get; set; }
        public object RedirectUrl { get; set; }
    }

    class Program
    {
		// Change URL below to point to specific bpm'online application.
        private const string baseUri = "https://pharmstore-test.gbc-team.com";
        private const string authServiceUri = baseUri + @"/ServiceModel/AuthService.svc/Login";
        private const string processServiceUri = baseUri + @"/0/ServiceModel/ProcessEngineService.svc/";
        private static ResponseStatus status = null;

        public static CookieContainer AuthCookie = new CookieContainer();

        /// <summary>
        /// Attempts to authenticate.
        /// </summary>
        /// <param name="userName">Bpm'online user name.</param>
        /// <param name="userPassword">Bpm'online user password.</param>
        /// <returns>True if authenticated. Otherwise returns false.</returns>
        public static bool TryLogin(string userName, string userPassword)
        {
            var authRequest = HttpWebRequest.Create(authServiceUri) as HttpWebRequest;
            authRequest.Method = "POST";
            authRequest.ContentType = "application/json";
            authRequest.CookieContainer = AuthCookie;

            using (var requesrStream = authRequest.GetRequestStream())
            {
                using (var writer = new StreamWriter(requesrStream))
                {
                    writer.Write(@"{
                    ""UserName"":""" + userName + @""",
                    ""UserPassword"":""" + userPassword + @"""
                    }");
                }
            }

            using (var response = (HttpWebResponse)authRequest.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    string responseText = reader.ReadToEnd();
                    status = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<ResponseStatus>(responseText);
                }
            }

            if (status != null)
            {
                if (status.Code == 0)
                {
                    return true;
                }
                Console.WriteLine(status.Message);
            }
            return false;
        }

        /// <summary>
        /// Adds new contact to bpm'online.
        /// </summary>
        /// <param name="contactName">Name of the contact.</param>
        /// <param name="contactPhone">Phone of the contact.</param>
        public static void AddContact(string contactName, string contactPhone)
        {
            string requestString = string.Format(processServiceUri +
                    "UsrAddNewExternalContact/Execute?ContactName={0}&ContactPhone={1}",
                                     contactName, contactPhone);
            HttpWebRequest request = HttpWebRequest.Create(requestString) as HttpWebRequest;
            request.Method = "GET";
            request.CookieContainer = AuthCookie;
            using (var response = request.GetResponse())
            {
                Console.WriteLine(response.ContentLength);
                Console.WriteLine(response.Headers.Count);
            }
        }

        public static void StartRPOpportunityInStageProcess()
        {
            string requestString = string.Format(processServiceUri +
                    "RPOpportunityInStageProcess/Execute");
            HttpWebRequest request = HttpWebRequest.Create(requestString) as HttpWebRequest;
            request.Method = "GET";
            request.CookieContainer = AuthCookie;
            using (var response = request.GetResponse())
            {
                Console.WriteLine(response.ContentLength);
                Console.WriteLine(response.Headers.Count);
            }
        }

        public static void GbcGenerationDbStructureProcess()
        {
            string requestString = string.Format(processServiceUri +
                    "GbcGenerationDbStructureProcess/Execute?ResultParameterName=GbcFile"); //
            HttpWebRequest request = HttpWebRequest.Create(requestString) as HttpWebRequest;
            request.Method = "GET";
            request.CookieContainer = AuthCookie;
            using (var response = request.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    string responseText = reader.ReadToEnd();
                    responseText = responseText.Replace("<string xmlns=\"http://schemas.microsoft.com/2003/10/Serialization/\">\"", "");
                    responseText  = responseText.Replace("\"</string>", "");

                    var bbb = Convert.FromBase64String(responseText);
                    File.WriteAllBytes(string.Format("DbStructure.xlsx", DateTime.Today.ToString("ddMMyyyy")), bbb);
                    Console.WriteLine(responseText);
                }
            }
        }

        /// <summary>
        /// Reads all bpm'online contacts and displays them.
        /// </summary>
        public static void GetAllContacts()
        {
            string requestString = processServiceUri +
                               "UsrGetAllContacts/Execute?ResultParameterName=ContactList";
            HttpWebRequest request = HttpWebRequest.Create(requestString) as HttpWebRequest;
            request.Method = "GET";
            request.CookieContainer = AuthCookie;
            using (var response = request.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    string responseText = reader.ReadToEnd();
                    Console.WriteLine(responseText);
                }
            }
        }

        static void Main(string[] args)
        {
            var f = File.ReadAllText("C:\\crm82\\Terrasoft.WebApp\\Terrasoft.Configuration\\Pkg\\IBD\\Schemas\\RPAccountPageBPKD\\RPAccountPageBPKD.js");

            Regex diffRegexp = new Regex(@"(\/\*\*SCHEMA_DIFF\*\/)([\s\S]*)(\/\*\*SCHEMA_DIFF\*\/)");
            Regex functionRegexp = new Regex(@"(\:\s*function\s*\(.*\)\s*\{[\s\S]*\})");
            string json = diffRegexp.Match(f).Value;
            json =  json.Replace("/**SCHEMA_DIFF*/", "");

            dynamic jsonDe = JsonConvert.DeserializeObject(json);


            foreach (dynamic jsons in jsonDe)
            { }

                List<Root> myDeserializedClass = JsonConvert.DeserializeObject<List<Root>>(json);

            //Product json = JsonConvert.DeserializeObject<Product>(json);

            return;

            if (!TryLogin("clio", "dominator"))
            {
                Console.WriteLine("Wrong login or password. Application will be terminated.");
            }
            else
            {
                try
                {
                    GbcGenerationDbStructureProcess();
                }
                catch (Exception)
                {
                    // Process exception here. Or throw it further.
                    throw;
                }

            };

        }
    }

    //
    public class Caption
    {
        public string bindTo { get; set; }
    }

    public class Classes
    {
        public List<string> wrapperClass { get; set; }
    }

    public class Click
    {
        public string bindTo { get; set; }
    }

    public class Content
    {
        public string bindTo { get; set; }
    }

    public class DefaultImage
    {
        public string bindTo { get; set; }
    }

    public class ImageConfig
    {
        public string bindTo { get; set; }
    }

    public class LabelConfig
    {
        public Caption caption { get; set; }
    }

    public class Layout
    {
        public int column { get; set; }
        public int row { get; set; }
        public int colSpan { get; set; }
        public int? rowSpan { get; set; }
        public string layoutName { get; set; }
    }

    public class Root
    {
        public string operation { get; set; }
        public string name { get; set; }
        public string parentName { get; set; }
        public int index { get; set; }
        public string propertyName { get; set; }
        public Values values { get; set; }
    }

    public class Selectors
    {
        public string wrapEl { get; set; }
    }

    public class Tip
    {
        public Content content { get; set; }
    }

    public class Values
    {
        public object itemType { get; set; }
        public List<string> wrapClass { get; set; }
        public List<object> items { get; set; }
        public Layout layout { get; set; }
        public string getSrcMethod { get; set; }
        public string onPhotoChange { get; set; }
        public bool? @readonly { get; set; }
        public DefaultImage defaultImage { get; set; }
        public string generator { get; set; }
        public string id { get; set; }
        public Selectors selectors { get; set; }
        public ImageConfig imageConfig { get; set; }
        public Caption caption { get; set; }
        public Classes classes { get; set; }
        public Click click { get; set; }
        public string bindTo { get; set; }
        public object enabled { get; set; }
        public LabelConfig labelConfig { get; set; }
        public object visible { get; set; }
        public int? contentType { get; set; }
        public Tip tip { get; set; }
        public string markerValue { get; set; }
        public int? order { get; set; }
    }


}
