using System;
using Kusto.Data;
using Kusto.Data.Net.Client;
using System.IO;
namespace readcsl
{ 
    class Program
    {
        static string Filename = "sample.csl";
        static void Main(string[] args)
        {
            var builder = new KustoConnectionStringBuilder("https://masvaas.kusto.windows.net/KustoOrchestratorAggregatedData").WithAadUserPromptAuthentication();
            string readerdata;
            using (StreamReader reader = new StreamReader(Filename))
            {
                readerdata = reader.ReadToEnd();
                reader.Close();
            }
            using (var client = KustoClientFactory.CreateCslQueryProvider(builder))
            {
                using (var reader = client.ExecuteQuery(readerdata))
                {
                    reader.Read();
                    var count = reader.GetValue(0);
                    Console.WriteLine(count);
                }
            }
        }
    }
}


