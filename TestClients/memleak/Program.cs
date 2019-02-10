using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MtApi5;

namespace FinTradeConsoleMt5
{
    class Program
    {
        static readonly object _locker = new object();
        public static MtApi5Client client;
        static void Main(string[] args)
        {
            client = new MtApi5Client();
            client.BeginConnect("192.168.178.15",8300);

            lock (_locker)
            {
                try
                {
                    client.BeginConnect("192.168.178.15",8300);
                    Thread.Sleep(500); // this for wait async connection...
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed connection!" + e.Message);

                }
                client.BeginConnect("192.168.178.15",8300);

                if (client.ConnectionState == Mt5ConnectionState.Connected)
                {
                    Console.WriteLine("Connected!");
                }
            }

            Thread.Sleep(1000);

            if (client.ConnectionState == Mt5ConnectionState.Connected)
            {
                TimerCallback callbackHourly = new TimerCallback(HourlyJob);
                Timer hourlyTimer = new Timer(callbackHourly, null, 0, 2000);
            }
            else
            {
                Console.WriteLine("Timeout for Connection, exit...");
                return;
            }
            //Timer hourlyTimer = new Timer(callbackHourly, null, TimeSpan.Zero, TimeSpan.FromHours(1.0));


           

            Console.WriteLine("Press ANY key to exit...");
            Console.ReadKey();
        }

        // Getting hourly Symbols data
        public static void HourlyJob(object state)
        {
            Console.WriteLine("Exec CopyRates: " + DateTime.UtcNow);
            ExecuteCopyRates(client);
            Console.WriteLine("In TimerCallback: " + DateTime.UtcNow);

        }

        private static async void ExecuteCopyRates(MtApi5Client client)
        {
            var result = await Execute(() =>
            {
                MqlRates[] array;
                var count = client.CopyRates("EURUSD", ENUM_TIMEFRAMES.PERIOD_CURRENT, 1, 12, out array);
                Console.WriteLine("Count: " + count);
                return count > 0 ? array : null;
            });

            foreach (var rates in result)
            {
                //Console.WriteLine(
                //    $"time={rates.time}; mt_time={rates.mt_time}; open={rates.open}; high={rates.high}; low={rates.low}; close={rates.close}; tick_volume={rates.tick_volume}; spread={rates.spread}; real_volume={rates.tick_volume}");
                Console.WriteLine($"close={rates.close}");
            }

        }

        private static async Task<TResult> Execute<TResult>(Func<TResult> func)
        {
            return await Task.Factory.StartNew(() =>
            {
                var result = default(TResult);
                try
                {
                    result = func();
                }
                catch (ExecutionException ex)
                {
                    Console.WriteLine($"Exception: {ex.ErrorCode} - {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                }

                return result;
            });
        }
    }
}
