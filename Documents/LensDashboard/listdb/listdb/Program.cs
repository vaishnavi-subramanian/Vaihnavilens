namespace listdb
{
    using System;
    using Kusto.Data;
    using Kusto.Data.Net.Client;

    class Program
    {
        static void Main(string[] args)
        {
            var builder = new KustoConnectionStringBuilder("https://help.kusto.windows.net/Samples")
                .WithAadUserPromptAuthentication();

            using (var client = KustoClientFactory.CreateCslQueryProvider(builder))
            {
                var command = Kusto.Data.Net.Client.KustoClientFactory.CslCommandGenerator.GenerateDatabasesShowCommand();
                var databases = Kusto.Data.Net.Client.KustoClientFactory.adminProvider.ExecuteControlCommand<DatabasesShowCommandResult>(command).ToList();

                foreach (var database in databases)
                {
                    Console.WriteLine(database.DatabaseName);
                }
            }
        }
    }
}
    