using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Common.ExchangeAdapter.Client;
using Lykke.Common.ExchangeAdapter.SpotController;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Lykke.ExchangeAdapter.ConsoleClient
{
    public sealed class CommandOptions
    {
        public string AdapterUrl { get; set; }
        public string ApiKey { get; set; }
        public string Asset { get; set; }
        public decimal Price { get; set; }
        public decimal Volume { get; set; }
    }

    class Program
    {
        private static CommandOptions _options;
        private static ISpotController _client;

        static async Task<int> Main(string[] args)
        {
            if (!TryBuildCommandOptions(args, out _options))
            {
                ShowUsage();

                return -1;
            }

            _client = CreateClient();

            try
            {
                await GetBalances();
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
                return -2;
            }

            return 0;
        }

        private static ISpotController CreateClient()
        {
            const string stub = "specified";

            var factory = new ExchangeAdapterClientFactory(new Dictionary<string, AdapterEndpoint>
            {
                {stub, new AdapterEndpoint(_options.ApiKey, new Uri(_options.AdapterUrl))}
            });

            return factory[stub];
        }

        private static async Task GetBalances()
        {
            Console.WriteLine("Fetching balances...");

            var balances = await _client.GetWalletBalancesAsync();

            var nonZeroBalances = balances.Wallets.Where(x => x.Balance != 0 || x.Reserved != 0).ToArray();

            Console.WriteLine($"Fetched balances total: {balances.Wallets.Count}, non-zero: {nonZeroBalances.Length}");
            Console.WriteLine(JsonConvert.SerializeObject(nonZeroBalances, Formatting.Indented));
        }

        private static void ShowUsage()
        {
            Console.WriteLine(@"
Usage: dotnet Lykke.ExchangeAdapter.ConsoleClient.dll [OPTIONS]:
    -url=<adapter-url>    Adapter Url                     (REQUIRED)
    -key=<x-api-key>      API-KEY to authenticate client  (REQUIRED)
    -asset=<symbol>       Asset (Lykke currencies pair)   (REQUIRED)
    -p=<price>            Price of the order              (REQUIRED)
    -v=<volume>           Volume of the order             (REQUIRED)");
        }

        private static bool TryBuildCommandOptions(string[] args, out CommandOptions options)
        {
            var switchMapping = new Dictionary<string, string>()
            {
                { "-url",     nameof(CommandOptions.AdapterUrl) },
                { "-key",     nameof(CommandOptions.ApiKey) },
                { "-asset",   nameof(CommandOptions.Asset) },
                { "-p",       nameof(CommandOptions.Price) },
                { "-v",       nameof(CommandOptions.Volume) }
            };

            var root = new ConfigurationBuilder()
                .AddCommandLine(args, switchMapping)
                .Build();

            options = new CommandOptions();
            root.Bind(options);

            var result = IsDefault(options.Asset);
            result = result && IsDefault(options.AdapterUrl);
            result = result && IsDefault(options.ApiKey);
            result = result && IsDefault(options.Price);
            result = result && IsDefault(options.Volume);

            return result;
        }

        private static bool IsDefault(string str)
        {
            return !string.IsNullOrEmpty(str);
        }

        private static bool IsDefault<T>(T v)
        {
            return !v.Equals(default(T));
        }
    }
}