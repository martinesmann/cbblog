using System;
using System.Collections.Generic;
using Couchbase;
using Couchbase.Configuration.Client;

namespace MyCBNodeTestConsoleApp
{
    class Program
    {
        public static ClientConfiguration GetClientConfiguration()
        {
            return new ClientConfiguration
            {
                Servers = new List<Uri>
                {
                    new Uri("http://MyCBServerNode.cloudapp.net:8091/pools"),
                },
                UseSsl = false,
                BucketConfigs = new Dictionary<string, BucketConfiguration>
                {
                    {
                        "default", 
                        new BucketConfiguration
                        {
                            BucketName = "default",
                            UseSsl = false,
                            Password = "",
                            PoolConfiguration = new PoolConfiguration
                            {
                                MaxSize = 10,
                                MinSize = 5
                            }
                        }
                    }
                }
            };
        }

        static void Main(string[] args)
        {
            // Long lived object should be created on App startup and live for the app life time.
            ClusterHelper.Initialize(GetClientConfiguration());

            // Get shared bucket instance (this shared instance is thread safe)
            var bucket = ClusterHelper.GetBucket("default");

            // Guid/Id for document.
            string id = Guid.NewGuid().ToString();
            
            var upsert =
                bucket.Upsert<dynamic>(
                    new Document<dynamic> { 
                        Id = id,
                        Content = new
                        {
                            Id = id,
                            Type = "Test document",
                            Name = "Couchbase Server on Azure."
                        }
                });

            Console.WriteLine("Upsert status: " + upsert.Success);

            var get = bucket.Get<dynamic>(id);

            Console.WriteLine("Get status: " + get.Success);

            if (get.Success)
            {
                Console.WriteLine(get.Value);
            }

            // Dispose all buckets and connections etc.
            ClusterHelper.Close();

            Console.WriteLine("Press Enter to continue.");
            Console.ReadLine();
        }
    }
}