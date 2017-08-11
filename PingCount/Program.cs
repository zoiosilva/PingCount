using System;

namespace PingCount
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.SetWindowSize(65, 05);

            //TODO: Implementar Reset dos contadores.
            //Talvez um pause também.

            using (var pc = new PingControl())
            {
                pc.StartAsync();

                Console.ReadKey();

                pc.Stop();
            }
        }
    }
}
