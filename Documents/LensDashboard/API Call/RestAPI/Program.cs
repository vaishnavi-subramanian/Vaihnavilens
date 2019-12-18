using System;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
//using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RestAPICALL
{ 
    class Program
    {
        // To run this sample, you'll need to create your own app id and key:
        // Create a new app registration via the Azure Portal and choose "Web app / API". Once it has been created, click Settings > Required Permissions.
        // Click Add and search for kustoorchestratorapi. Check the box to give delegated permission to access your function app.
        // Accept all the panels and save those changes. In the Settings panel, click Keys and generate a key. Azure Key Vault is a recommended
        // place to store the key and this sample helps you retrieve it from KeyVault.
        const string ClientId = "1c4e1a1d-ddc7-4a26-8a46-43285e340549";
        static string KOCluster = "Masvaas"; // Case sensitive. Match it to what you see at http://aka.ms/ko
        static string KODatabase = "KustoOrchestratorAggregatedData"; // Case sensitive. Match it to what you see at http://aka.ms/ko

        // These variables don't need to change
        const string ServiceBaseAddress = "https://kustoorchestrator.azcompute.com";
        const string AadInstance = "https://login.windows.net/{0}";
        const string Tenant = "microsoft.onmicrosoft.com";
        const string ServiceResourceId = "https://microsoft.onmicrosoft.com/3a7a47f4-103b-4c2b-8b35-d2f163fb267a";
        static string AppKey= "Pup=@PDVwB2z8:YxQUWoc.ejDuUPQs16";

        /// <summary>
        /// Main method.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //AppKey = GetKeyVaultSecret(KeyVaultName, KeyVaultSecretName).Result;

            CreateOrEditSchedule().Wait();
            //GetSchedule().Wait();
            //DeleteSchedule().Wait();
            GetSchedules().Wait();

            Console.Write("Press enter to continue...");
            Console.ReadLine();
        }

        /// <summary>
        /// Sample method for retrieving a specific schedule
        /// </summary>
        /// <returns></returns>
        static async Task GetSchedule()
        {
            string url = ServiceBaseAddress + $"/api/OrchestrationRequests/{KOCluster}/{KODatabase}/StreamingQuery/ApiTest";
            var httpClient = await BuildHttpClientWithAuth();
            Console.WriteLine($"GET {url}");
            HttpResponseMessage response = await httpClient.GetAsync(url);
            await PrintOutput(response);
        }

        /// <summary>
        /// Sample method for retrieving all the schedules for a database
        /// </summary>
        /// <returns></returns>
        static async Task GetSchedules()
        {
            string url = ServiceBaseAddress + $"/api/OrchestrationRequests/{KOCluster}/{KODatabase}/StreamingQuery";
            var httpClient = await BuildHttpClientWithAuth();
            Console.WriteLine($"GET {url}");
            HttpResponseMessage response = await httpClient.GetAsync(url);
            await PrintOutput(response);
        }

        /// <summary>
        /// Sample method for creating a new schedule and editing an existing one. If a schedule already
        /// exists on this cluster/database with the specified activityId, it will be overwritten with 
        /// the payload of this request. For additional help crafting the body of the request, check out
        /// the documentation https://kustoorchestrator-docs.azurewebsites.net/concepts/StreamingQuery.html
        /// </summary>
        /// <returns>An awaitable task</returns>
        static async Task CreateOrEditSchedule()
        {
            
            //string url = ServiceBaseAddress + $"/Manage?cluster={KOCluster}&database={KODatabase}";
            var httpClient = await BuildHttpClientWithAuth();
            

            var schedule = new
            {
                activityId = "Info_StampInfo",
                //folder = "api",
                functionName = "Info_StampInfo",
                outputTable = "KODemo_Output",
                queryWindowSize = "10:00:00",
                delayFromUtcNow = "00:00:00",
                maxParallelism = 5,
                ShowAdvancedSettings=true,
                TargetCluster = "Masvaas",
                TargetDatabase= "KustoOrchestratorAggregatedData"
            };
            string url = ServiceBaseAddress + $"/Manage?cluster={KOCluster}&database={KODatabase}&actorId={schedule.activityId}";
            Console.WriteLine($"PUT {url}");
            HttpResponseMessage response = await httpClient.PutAsJsonAsync(url, schedule);
            await PrintOutput(response);
        }

        /// <summary>
        /// Sample method for deleting a schedule.
        /// </summary>
        /// <returns>An awaitable task</returns>
        static async Task DeleteSchedule()
        {
            string url = ServiceBaseAddress + $"/api/OrchestrationRequests/{KOCluster}/{KODatabase}/StreamingQuery/ApiTest";
            var httpClient = await BuildHttpClientWithAuth();
            Console.WriteLine($"DELETE {url}");

            HttpResponseMessage response = await httpClient.DeleteAsync(url);
            await PrintOutput(response);
        }

        /// <summary>
        /// Read the response and write a nicely formatted output.
        /// </summary>
        /// <param name="response">Response from the service</param>
        /// <returns>An awaitable task</returns>
        static async Task PrintOutput(HttpResponseMessage response)
        {
            Console.WriteLine($"{(int)(response.StatusCode)} {response.ReasonPhrase}");
            try
            {
                string json = await response.Content.ReadAsStringAsync();
                string formattedJson = JValue.Parse(json).ToString(Formatting.Indented);
                Console.WriteLine(formattedJson);
            }
            catch { }
        }

        /// <summary>
        /// Get auth token and add the access token to the authorization header of the request.
        /// </summary>
        /// <returns>An HttpClient with the proper bearer token attached</returns>
        static async Task<HttpClient> BuildHttpClientWithAuth()
        {
            var httpClient = new HttpClient();
            var authContext = new AuthenticationContext(string.Format(CultureInfo.InvariantCulture, AadInstance, Tenant));
            var clientCredential = new ClientCredential(ClientId, AppKey);
            AuthenticationResult result = await authContext.AcquireTokenAsync(ServiceResourceId, clientCredential);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
            return httpClient;
        }

        /// <summary>
        /// Retrieve a secret from KeyVault. Only string secrets are supported.
        /// </summary>
        /// <param name="vault">The name of the KeyVault</param>
        /// <param name="name">The name of the secret</param>
        /// <returns></returns>
        static async Task<string> GetKeyVaultSecret(string vault, string name)
        {
            //var azureServiceTokenProvider = new AzureServiceTokenProvider();
            //var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            //var secret = await keyVaultClient.GetSecretAsync($"https://{vault}.vault.azure.net", name);
            //return secret.Value;
            return null;
        }
    }
}
