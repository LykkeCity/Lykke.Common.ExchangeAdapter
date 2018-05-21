using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Common.ExchangeAdapter.Client;
using Lykke.Common.ExchangeAdapter.Contracts;
using Lykke.Common.ExchangeAdapter.SpotController;
using Lykke.Common.ExchangeAdapter.SpotController.Records;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Lykke.ExchangeAdapter.ConsoleClient
{
    public enum Command
    {
        Workflow,
        Cancel
    }

    public sealed class CommandOptions
    {
        public Command Command { get; set; } = Command.Workflow;
        public string AdapterUrl { get; set; }
        public string ApiKey { get; set; }
        public string OrderId { get; set; }
        public string Asset { get; set; }
        public decimal Price { get; set; }
        public decimal Volume { get; set; }
        public TradeType TradeType { get; set; } = TradeType.Sell;
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
                switch (_options.Command)
                {
                    case Command.Workflow:
                        await WholeWorkflow();
                        break;
                    case Command.Cancel:
                        await CancelLimitOrder(_options.OrderId);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
                return -2;
            }

            return 0;
        }

        private static async Task WholeWorkflow()
        {
            await GetBalances();
            var id = await CreateLimitOrder();
            await ShowLimitOrder(id);
            await CancelLimitOrder(id);
            await ShowLimitOrder(id);
            await GetBalances();
        }

        private static void Announce(string msg)
        {
            var prevColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine();
            Console.WriteLine(msg);
            Console.WriteLine(new string('=', 10));
            Console.ForegroundColor = prevColor;
        }

        private static async Task CancelLimitOrder(string id)
        {
            Announce($"Canceling limit order {id}");
            var response = await _client.CancelLimitOrderAsync(new CancelLimitOrderRequest {OrderId = id});
            Console.WriteLine(Dump(response));
        }

        private static async Task ShowLimitOrder(string id)
        {
            Announce($"Fetching order {id}");
            var order = await _client.LimitOrderStatusAsync(id);
            Console.WriteLine(Dump(order));
        }

        private static async Task<string> CreateLimitOrder()
        {
            Announce($"Creating a {_options.TradeType:G} order for instrument {_options.Asset}, " +
                     $"price: {_options.Price}, volume: {_options.Volume}");

            var order = await _client.CreateLimitOrderAsync(new LimitOrderRequest
            {
                Instrument = _options.Asset,
                Price = _options.Price,
                Volume = _options.Volume,
                TradeType = _options.TradeType
            });

            Console.WriteLine(Dump(order));

            return order.OrderId;
        }

        private static ISpotController CreateClient()
        {
            const string stub = "stub";

            var factory = new ExchangeAdapterClientFactory(new Dictionary<string, AdapterEndpoint>
            {
                {stub, new AdapterEndpoint(_options.ApiKey, new Uri(_options.AdapterUrl))}
            });

            return factory[stub];
        }

        private static async Task GetBalances()
        {
            Announce("Fetching balances...");

            var balances = await _client.GetWalletBalancesAsync();

            var nonZeroBalances = balances.Wallets.Where(x => x.Balance != 0 || x.Reserved != 0).ToArray();

            Console.WriteLine($"Fetched balances total: {balances.Wallets.Count}, non-zero: {nonZeroBalances.Length}");
            Console.WriteLine(Dump(nonZeroBalances));
        }

        private static string Dump(object nonZeroBalances)
        {
            return JsonConvert.SerializeObject(nonZeroBalances, Formatting.Indented);
        }

        private static void ShowUsage()
        {
            Console.WriteLine(@"
Usage: dotnet Lykke.ExchangeAdapter.ConsoleClient.dll [OPTIONS]:
    -url=<adapter-url>    Adapter Url                         (REQUIRED)
    -key=<x-api-key>      API-KEY to authenticate client      (REQUIRED)
    -c=<cancel/workflow>  Run whole workflow or just cancel order

  Workflow options:

    -asset=<symbol>       Asset (Lykke currencies pair)       (REQUIRED)
    -p=<price>            Price of the order                  (REQUIRED)
    -v=<volume>           Volume of the order                 (REQUIRED)
    -t=<buy/sell>         Type of the order                   (default = sell)

  Cancel options:

    -id=<orderId>                                             (REQUIRED)");
        }

        private static bool TryBuildCommandOptions(string[] args, out CommandOptions options)
        {
            var switchMapping = new Dictionary<string, string>()
            {
                { "-url",     nameof(CommandOptions.AdapterUrl) },
                { "-key",     nameof(CommandOptions.ApiKey) },
                { "-asset",   nameof(CommandOptions.Asset) },
                { "-p",       nameof(CommandOptions.Price) },
                { "-v",       nameof(CommandOptions.Volume) },
                { "-t",       nameof(CommandOptions.TradeType) },
                { "-c",       nameof(CommandOptions.Command) },
                { "-id",      nameof(CommandOptions.OrderId) }
            };

            var root = new ConfigurationBuilder()
                .AddCommandLine(args, switchMapping)
                .Build();

            options = new CommandOptions();
            root.Bind(options);

            var result = IsDefault(options.AdapterUrl) && IsDefault(options.ApiKey);

            switch (options.Command)
            {
                case Command.Workflow:
                    result = result && IsDefault(options.Asset);
                    result = result && IsDefault(options.Price);
                    result = result && IsDefault(options.Volume);
                    break;
                case Command.Cancel:
                    result = result && IsDefault(options.OrderId);
                    break;
                default:
                    return false;
            }

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