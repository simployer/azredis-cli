using Azure.Identity;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace azredis_cli
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: azredis_cli <command> [args...]");
                Console.WriteLine("Known commands: get");
                return;
            }

            string command = args[0].ToLower();

            switch (command)
            {
                case "get":
                    await HandleGetCommand(args);
                    break;

                default:
                    Console.WriteLine($"Unknown command '{command}'");
                    Console.WriteLine("Known commands: get");
                    break;
            }

            Console.WriteLine("Hello, World!");
        }

        static async Task HandleGetCommand(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Usage: azredis_cli get <cluster> <key>");
                return;
            }

            string cluster = args[1];
            string key = args[2];

            try
            {
                var configurationOptions = ConfigurationOptions
                    .Parse(cluster)
                    .ConfigureForAzureWithTokenCredentialAsync(new DefaultAzureCredential())
                    .Result;
                
                configurationOptions.AbortOnConnectFail = true;

                var cm = await ConnectionMultiplexer.ConnectAsync(configurationOptions, null);
                var db = cm.GetDatabase();
                var value = await db.StringGetAsync(key);
                
                Console.WriteLine(value.IsNullOrEmpty ? $"Key '{key}' does not exist in Redis." : $"Value for '{key}': {value}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
