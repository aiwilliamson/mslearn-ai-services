using System;
using Azure;
using Microsoft.Extensions.Configuration;
using System.Text;
using Azure.AI.TextAnalytics;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System.Threading.Tasks;

namespace sdk_client
{
    class Program
    {

        private static string AISvcEndpoint;
        private static string AISvcKey;
        static async Task Main(string[] args)
        {
            try
            {
                AISvcEndpoint = await GetSecret("aw8-keyvault", "AIServicesEndpoint");
                AISvcKey = await GetSecret("aw8-keyvault", "AIServicesKey");

                // Get user input (until they enter "quit")
                string userText = "";
                while (userText.ToLower() != "quit")
                {
                    Console.WriteLine("\nEnter some text ('quit' to stop)");
                    userText = Console.ReadLine();
                    if (userText.ToLower() != "quit")
                    {
                        // Call function to detect language
                        string language = GetLanguage(userText);
                        string sentiment = AnalyseSentiment(userText);
                        Console.WriteLine("Language: " + language + ".\t Sentiment: " + sentiment);

                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static async Task<string> GetSecret(string keyVaultName, string secretName)
        {
            var kvUri = $"https://{keyVaultName}.vault.azure.net";
            var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());
            KeyVaultSecret secret = await client.GetSecretAsync(secretName);
            Console.WriteLine($"Retrieved secret: {secret.Value}");
            return secret.Value;
        }

        static string GetLanguage(string text)
        {

            // Create client using endpoint and key
            AzureKeyCredential credentials = new AzureKeyCredential(AISvcKey);
            Uri endpoint = new Uri(AISvcEndpoint);
            var client = new TextAnalyticsClient(endpoint, credentials);
            // Call the service to get the detected language
            DetectedLanguage detectedLanguage = client.DetectLanguage(text);
            return (detectedLanguage.Name);
        }

        private static string AnalyseSentiment(string text)
        {
            // Create client using endpoint and key
            AzureKeyCredential credentials = new AzureKeyCredential(AISvcKey);
            Uri endpoint = new Uri(AISvcEndpoint);
            var client = new TextAnalyticsClient(endpoint, credentials);
            // Call the service to get the sentiment
            DocumentSentiment documentSentiment = client.AnalyzeSentiment(text);
            return (documentSentiment.Sentiment.ToString());
        }
    }
}
