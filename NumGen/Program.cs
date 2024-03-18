using System.Threading;
using System.Security.Cryptography;
using System.Numerics;
using System.Diagnostics;
using System.ComponentModel;
using System.Security.Principal;
//Author: Nathaniel Shah
namespace NumGen
{
    class NumberGen
    {
        //BigInteger
        //Environment.NewLine
        //RandomNumberGenerator
        static string help = "dotnet run <bits> <option> <count>" + Environment.NewLine +
            "- bits - the number of bits of the number to be generated, this must be a multiple of 8, and at least 32 bits." + Environment.NewLine +
            "- option - 'odd' or 'prime' (the type of numbers to be generated)" + Environment.NewLine +
            "- count - the count of numbers to generate, defaults to 1" + Environment.NewLine;
        private static readonly object ConsoleLock = new object();
        static void Main(string[] args)
        {
            int bits = 0;
            string option;
            int count = 1;
            if (args.Length < 2 | args.Length > 4) { 
                Console.WriteLine("Incorrect number of arguments" +  Environment.NewLine);
                Console.WriteLine(help);
                return;
            }
            try
            {
                bits = int.Parse(args[0]);
            }
            catch { 
                Console.WriteLine("bits must be an integer, " + args[0] + " is not an int" + Environment.NewLine);
                Console.WriteLine(help);
            }
            if (args.Length == 3) {
                try
                {
                    count = int.Parse(args[2]);
                }
                catch
                {
                    Console.WriteLine("count must be an integer, " + args[2] + " is not an int" + Environment.NewLine);
                    Console.WriteLine(help);
                }
            }
            option = args[1];
            if (bits % 8 != 0 | bits !> 31)
            {
                Console.WriteLine(bits + " is not a valid input for bits." + Environment.NewLine);
                Console.WriteLine(help);
            }
            if (count < 0) count = 0;
            if (option != "odd" | option != "prime")
            {
                Console.WriteLine(option + " is not a valid input for option." + Environment.NewLine);
                Console.WriteLine(help);
            }
            Console.WriteLine("BitLength: " + bits + " bits" + Environment.NewLine);
            if (option == "odd") oddGen(bits, count);
            else primeGen(bits, count);
        }
        static private BigInteger generator(int bits)
        {
            byte[] bytes = RandomNumberGenerator.GetBytes(bits % 8);
            BigInteger num = new BigInteger(bytes);
            while (num % 2 == 0)
            {
                num = new BigInteger(RandomNumberGenerator.GetBytes(bits % 8));
            }
            return num;
        }
        static public void oddGen(int bits, int count)
        {
            if (count == 0) return;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            int ccount = 0;
            Parallel.For(0, count, i =>
            {
                BigInteger num = generator(bits);
                int fCount = factorCount(num);
                lock (ConsoleLock) {
                    Console.WriteLine(Environment.NewLine + ccount + ": " + num + Environment.NewLine);
                    Console.WriteLine("Number of factors: " + fCount + Environment.NewLine);
                }
            }
            );
            sw.Stop();
            TimeSpan t = sw.Elapsed;

        }
        static public void primeGen(int bits, int count)
        {
            if (count == 0) return;
            BigInteger num = generator(bits);

        }
        static private int factorCount(BigInteger num)
        {
            double dnum = Convert.ToDouble(num);
            int square = (int)Math.Sqrt(dnum);
            int count = 2;
            Parallel.For(2, square, i =>
                {
                    if (num % i == 0)
                    {
                        Interlocked.Increment(ref count);
                    }
                }
            );
            return count;
        }
    }
}
