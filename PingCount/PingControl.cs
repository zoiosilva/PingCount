using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;

namespace PingCount
{
    public class PingControl : IDisposable
    {
        private readonly Ping ping = new Ping();
        private readonly int sleep;
        private readonly string endereco;
        private Action Acao;

        private enum Estado
        {
            Funcionando,
            Caido,
        }

        private static long Success { get; set; }
        private static long Fail { get; set; }
        private static Estado Situacao { get; set; }
        private static bool Cancelling { get; set; }
        private static bool Cancelled { get; set; }

        public PingControl(int sleep = 750, string endereco = "8.8.8.8")
        {
            this.sleep = sleep;
            this.endereco = endereco;

            Init();
        }

        private void Init()
        {
            ping.PingCompleted += (s, args) =>
            {
                if (args.Reply.Status == IPStatus.Success)
                    Pingou();
                else
                    Falhou();

                ShowStatus();

                ((AutoResetEvent)args.UserState).Set();
            };

            Acao = () =>
            {
                using (var waiter = new AutoResetEvent(false))
                    do
                    {
                        try
                        {
                            ping.SendAsync(endereco, waiter);
                            waiter.WaitOne();
                        }
                        catch (PingException)
                        {
                            Falhou();
                            ShowStatus();
                        }

                        Thread.Sleep(sleep);
                    } while (!Cancelling);

                Cancelled = true;
            };
        }

        public void StartAsync()
        {
            Cancelling = false;
            Cancelled = false;

            Acao.BeginInvoke(Acao.EndInvoke, null);
        }

        public void Stop()
        {
            Cancelling = true;
            while (!Cancelled) ;
        }

        private static void Pingou()
        {
            Success++;
            if (Situacao == Estado.Caido)
            {
                Console.Beep(1318, 180); //E (Mi)
                Console.Beep(1760, 180); //A (Lá)
            }
            Situacao = Estado.Funcionando;
        }

        private static void Falhou()
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

        public void Dispose()
        {
            if (!Cancelling)
                throw new Exception("Tentativa de liberação de recurso ainda em uso.");

            ping.Dispose();
        }
    }
}
