using System;
using System.IO;
using System.Runtime.InteropServices;

namespace KQLParam
{
    class Program
    {
        static string Filename = "sample.csl";
        static String ClusterName = "masvaas";
        static String DBName = "TenantAvailabilityPNU-TestPass-MAS_Prod_1.1910.0.58-20191127173625-5454420b-2e38-4b9e-8b56-1712d321cf33";

        static void Main(string[] args)
        {

            funcfilters(ClusterName, DBName);
            static void funcfilters(string Cluster, String Database)
            {
                Console.Write(Cluster, Database);
                string readerdata;
                using (StreamReader reader = new StreamReader(Filename))
                {
                    readerdata = reader.ReadToEnd();
                    reader.Close();
                }
                readerdata = readerdata.Replace("{Cluster}", Cluster).Replace("{Database}", Database);
                using (StreamWriter writer = new StreamWriter(Filename))
                {
                    writer.Write(readerdata);
                    writer.Close();
                }

            }
        }
    }
}
