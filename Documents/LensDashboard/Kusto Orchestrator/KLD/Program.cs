namespace KLD
{
    using System;
    using Kusto.Data;
    using Kusto.Data.Common;
    using Kusto.Data.Net.Client;
    using Kusto.Cloud.Platform.Data;
    using System.Windows.Forms;
    using System.Diagnostics;
    using System.Collections.Generic;
    using System.Linq;

    class Program
    {
        static void Main(string[] args)
        {
            int currentCount = 0;
            List<DBReader> DBData = new List<DBReader>();
            var builder = new KustoConnectionStringBuilder("https://masvaas.kusto.windows.net/").WithAadUserPromptAuthentication();
            //int count = 0;
            using (var adminProvider = KustoClientFactory.CreateCslAdminProvider(builder))
            {
                var command = CslCommandGenerator.GenerateDatabasesShowCommand();
                var objectReader = new ObjectReader<DatabasesShowCommandResult>(adminProvider.ExecuteControlCommand(command));

                foreach (var temp in objectReader)
                {
                    currentCount++;
                    var db = temp.DatabaseName;

                    var databaseJournalCommand = CslCommandGenerator.GenerateDatabaseJournalShowCommand(db);
                    databaseJournalCommand += " | where Event == 'ADD-DATABASE' | project EventTimestamp";
                    //hardcoded queries to get the created time for db using journal command

                    using (var journalCmdResult = adminProvider.ExecuteControlCommand(db, databaseJournalCommand))
                    {
                        ///List<DateTime> dates= new List<string>() journalCmdResult["EventTimestamp"];
                        if (journalCmdResult.Read() && DateTime.TryParse(journalCmdResult["EventTimestamp"].ToString(), out var createdTime))
                        {
                            DBReader DBRead = new DBReader();
                            DBRead.databasename = db;
                            DBRead.Timestamp = createdTime.Ticks;
                            DBData.Add(DBRead);
                        }
                    }
                }
            }

            var sorted = from d in DBData
                         orderby d.Timestamp descending
                         select d;

            foreach (var c in sorted)
            { 
                Console.WriteLine($"Database: {c.databasename}  Timestamp: {new DateTime(c.Timestamp)}"); 
            }
        }
    }
}
