using System;
using System.Net.NetworkInformation;
using System.Threading;

namespace PingCount
{
    class Program
    {
        private enum Estado
        {
            Funcionando,
            Caido,
        }

        private static long Success { get; set; }
        private static long Fail { get; set; }
        private static Estado Situacao { get; set; }

        static void Main(string[] args)
        {
            //Console.WriteLine("WRAAAAAAAAAAAAAA!!");
            //Console.ReadKey();
            Console.SetWindowSize(65, 05);
            Teste();
        }

        private static void Teste()
        {
            using (var waiter = new AutoResetEvent(false))
            using (var ping = new Ping())
            {
                ping.PingCompleted += (s, args) =>
                {
                    if (args.Reply.Status == IPStatus.Success)
                        Yay();
                    else
                        Ahhh();

                    ShowStatus();

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
                        Ahhh();
                        ShowStatus();
                    }

                    Thread.Sleep(750);
                } while (true);
            }
        }

        private static void Yay()
        {
            Success++;
            if (Situacao == Estado.Caido)
            {
                Console.Beep(1318, 180); //E (Mi)
                Console.Beep(1760, 180); //A (Lá)
            }
            Situacao = Estado.Funcionando;
        }

        private static void Ahhh()
        {
            Situacao = Estado.Caido;
            Fail++;
            Console.Beep(880, 200); //A (Lá)
        }

        private static void ShowStatus()
        {
            decimal all = Fail + Success;

            Console.Clear();
            Console.WriteLine("Ping status: {0} ({1:N0}%) succeeded, {2} ({3:N0}%) failed somehow.", Success, (Success / all) * 100, Fail, (Fail / all) * 100);
        }
    }
}