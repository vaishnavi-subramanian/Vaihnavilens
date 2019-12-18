

namespace LensDashboardOptimization
{

    using System;
    using Kusto.Data;
    using Kusto.Data.Common;
    using Kusto.Data.Net.Client;
    using Kusto.Cloud.Platform.Data;
    using System.Windows.Forms;

    class Program
    {
        static void Main(string[] args)
        {
            var builder = new KustoConnectionStringBuilder("https://masvaas.kusto.windows.net/")
                .WithAadUserPromptAuthentication();


            using (var adminProvider = KustoClientFactory.CreateCslAdminProvider(builder))
            {
                var command = CslCommandGenerator.GenerateDatabasesShowCommand();
                var objectReader = new ObjectReader<DatabasesShowCommandResult>(adminProvider.ExecuteControlCommand(command));



                foreach (var temp in objectReader)
                {
                    var db = temp.DatabaseName;

                    var databaseJournalCommand = CslCommandGenerator.GenerateDatabaseJournalShowCommand(db);
                    databaseJournalCommand += " | where Event == 'ADD-DATABASE' | project EventTimestamp";
                    using (var journalCmdResult = adminProvider.ExecuteControlCommand(db, databaseJournalCommand))
                    {
                        if (journalCmdResult.Read() && DateTime.TryParse(journalCmdResult["EventTimestamp"].ToString(), out var createdTime))
                        {
                            //ValhallaLogger.LogInformationalMessage(operationId, nameof(DatabaseInfoProvider), $"Database {database} Created time {createdTime.ToUniversalTime():o}");
                            Console.WriteLine($"Database: {db}, CreationTime: {createdTime}");
                        }

                    }
                }
            }
        }

    }
}