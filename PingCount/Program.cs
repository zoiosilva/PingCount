using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;
using System.Threading;

namespace PingCount
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("WRAAAAAAAAAAAAAA!!");
            //Console.ReadKey();
            Console.SetWindowSize(65, 05);
            Teste();
        }

        private static void Teste()
        {
            long success = 0, fail = 0;

            using (var waiter = new AutoResetEvent(false))
            using (var ping = new Ping())
            {
                ping.PingCompleted += (s, args) =>
                {
                    if (args.Reply.Status == IPStatus.Success)
                        success++;
                    else
                    {
                        fail++;
                        Console.Beep();
                    }

                    ShowStatus(success, fail);

                    ((AutoResetEvent)args.UserState).Set();
                };

                do
                {
                    try
                    {
                        ping.SendAsync("8.8.8.8", waiter);
                        waiter.WaitOne();
                    }
                    catch (PingException)
                    {
                        fail++;
                        Console.Beep();
                        ShowStatus(success, fail);
                    }

                    Thread.Sleep(500);
                } while (true);
            }
        }

        private static void ShowStatus(long success, long fail)
        {
            decimal all = fail + success;

            Console.Clear();
            Console.WriteLine("Ping status: {0} ({1:N0}%) succeeded, {2} ({3:N0}%) failed somehow.", success, (success / all) * 100, fail, (fail / all) * 100);
        }
    }
}

//Console.WriteLine("{0}: {1}", DateTime.Now, args.Reply.Status);
//Console.WriteLine("Resposta de {0}: tempo={1}ms TTL={2}", args.Reply.Address, args.Reply.RoundtripTime, args.Reply.Options.Ttl);