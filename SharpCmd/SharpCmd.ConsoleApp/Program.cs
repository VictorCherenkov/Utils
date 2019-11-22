using System;

namespace SharpCmd.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var result = CommandLineExecutor.ExecuteCleartool("lsview -host HS7PW12 -quick -long");
            Console.WriteLine(result);
        }
    }
}
