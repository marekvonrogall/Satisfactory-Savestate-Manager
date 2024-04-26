using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Windows;

namespace SatisfactorySavestateManager.classes
{
    public class Credentials
    {

        public bool AreCredentialsValid(string path)
        {
            try
            {
                string jsonContent = File.ReadAllText(path);
                JObject jsonObj = JObject.Parse(jsonContent);

                // Verify if all parameters exist
                if (jsonObj.ContainsKey("type") &&
                    jsonObj.ContainsKey("project_id") &&
                    jsonObj.ContainsKey("private_key_id") &&
                    jsonObj.ContainsKey("private_key") &&
                    jsonObj.ContainsKey("client_email") &&
                    jsonObj.ContainsKey("client_id") &&
                    jsonObj.ContainsKey("auth_uri") &&
                    jsonObj.ContainsKey("token_uri") &&
                    jsonObj.ContainsKey("auth_provider_x509_cert_url") &&
                    jsonObj.ContainsKey("client_x509_cert_url") &&
                    jsonObj.ContainsKey("universe_domain"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch { return false; }
        }
    }
}
