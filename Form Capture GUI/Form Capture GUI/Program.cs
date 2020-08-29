using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Form_Capture_GUI
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        /// 

        public static ArrayList fieldNames;
        public static ArrayList formNames;
        public static Dictionary<string, string> map;

        public static string username;
        public static string password;
        public static string perm_token;

        [STAThread]
        static void Main()
        {
            fieldNames = new ArrayList();
            formNames = new ArrayList();

            map = new Dictionary<string, string>();

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new CredentialsForm());
        }

        public static string call_api(String url, String json)
        {

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {


                streamWriter.Write(json);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                return result;
            }


        }

        public static string get_token(String username, String password)
        {
            string json = "[{\"function\":\"get-login-token\"," +
                          "\"parameters\":{" +
                          "\"email-address\":\"" + username + "\"," +
                          "\"password\":\"" + password + "\"" +
                          "}," +
                          "\"request-id\":\"token request\"" +
                          "}]";

            string response = call_api("https://logon.salesnexus.com/api/call-v1", json);
            return getJsonProperty(response, "login-token");

        }

        public static List<string> get_fields()
        {
            List<string> list = new List<string>();
            string token;
            if (perm_token.Equals(""))
            {
                token = get_token(username, password);
            }
            else
            {
                token = perm_token;
            }

            string json = "["+
                "{"+
                "\"function\": \"get-all-fields\","+
                "\"parameters\": {"+
                    "\"login-token\": \""+token+"\""+
                 "},"+
                "\"request-id\": \"request to get all fields in the database\""+
                "}"+
                "]";

            string response = call_api("https://logon.salesnexus.com/api/call-v1", json);
            Debug.WriteLine(response);
            while (response.IndexOf("\"label\":") > -1)
            {
                
                response = response.Substring(response.IndexOf("\"table-name\":") + 15, response.Length - (response.IndexOf("\"table-name\":") + 15));
                string tableName = response.Substring(0, response.IndexOf(",")-1);
                response = response.Substring(response.IndexOf("\"label\":") + 10, response.Length - (response.IndexOf("\"label\":") + 10));
                string label = response.Substring(0, response.IndexOf(",")-1);
                response = response.Substring(response.IndexOf("\"unique-id\":") + 14, response.Length - (response.IndexOf("\"unique-id\":") + 14));
                string fieldId = response.Substring(0, response.IndexOf(",") - 1);
                if (tableName == "Contact")
                {
                    Debug.WriteLine("id: " + fieldId + " label: " + label);

                    list.Add((label));
                    map.Add(label,fieldId);
                }
            }

            return list; //filter this into some array or something

        }

        public static string getJsonProperty(string json, string key)
        {
            string bottom = json.Substring(json.IndexOf(key) + key.Length + 4);
            return bottom.Substring(0, bottom.IndexOf("\""));
        }

        public static string generate_script(ArrayList fieldNums, string token)
        {
            string top = "function formAction(){\r\nvar idList = [";
            for(int i = 0; i <formNames.Count; i++)
            {
                top += "\""+formNames[i]+"\"";
                if(i < formNames.Count - 1)
                {
                    top += ",";
                }

            }
            top += "];\r\nvar numList = [";
            for(int i = 0; i < fieldNums.Count; i++)
            {
                top += "\"" + map[(string)fieldNums[i]] + "\"";
                if (i < fieldNums.Count - 1)
                {
                    top += ",";
                }
            }
            top += "];\r\nvar token = \""+token+"\";\r\n";
            return top + script;
        }

        public static string script = "var contactFieldData = \"{\";\r\nfor(var i = 0; i < idList.length; i++){\r\ncontactFieldData += numList[i]+\":\\\"\"+document.getElementById(idList[i]).value+\"\\\"\";\r\nif(i != idList.length-1){\r\ncontactFieldData += \",\";\r\n}\r\n}\r\ncontactFieldData += \"}\";\r\nconsole.log(contactFieldData);\r\n\r\nxml = new XMLHttpRequest();\r\nxml.open('POST',\"https://logon.salesnexus.com/api/call-v1\");\r\nlet data = JSON.stringify({\r\n\"function\": \"create-contact\",\"parameters\": {\"login-token\": token,\"contact-field-data\": contactFieldData},\"request-id\": \"Create a contact\"});\r\nxml.send(\"[\"+data+\"]\");\r\n\r\nxml.onreadystatechange = processReq;\r\n\r\nfunction processReq(e){\r\nif(xml.readyState == 4){\r\nif(xml.status == 200){\r\nconsole.log(xml.responseText);\r\n}else{\r\nconsole.log(xml.statusText);\r\n}\r\n}\r\n}\r\n}";


        public static void set_creds(string _username, string _password, string _token)
        {
            username = _username;
            password = _password;
            perm_token = _token;
        }

        }


}
