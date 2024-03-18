using System.Threading;
using System.Security.Cryptography;
using System.Numerics;
using System.Diagnostics;
using System.ComponentModel;
using System.Security.Principal;
using System.Runtime.CompilerServices;
using System.Diagnostics.Metrics;
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
                return;
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
                    return;
                }
            }
            option = args[1];
            if (bits % 8 != 0 | bits < 32)
            {
                Console.WriteLine(bits + " is not a valid input for bits." + Environment.NewLine);
                Console.WriteLine(help);
                return;
            }
            if (count < 0) count = 0;
            if (option != "odd" & option != "prime")
            {
                Console.WriteLine(option + " is not a valid input for option." + Environment.NewLine);
                Console.WriteLine(help);
            }
            Console.Write("BitLength: " + bits + " bits");
            if (option == "odd") oddGen(bits, count);
            else primeGen(bits, count);
        }
        static public BigInteger generator(int bits)
        {
            Byte[] bytes = RandomNumberGenerator.GetBytes(bits / 8);
            BigInteger num = new BigInteger(bytes);
            while (num % 2 == 0 | num < 0)
            {
                num = new BigInteger(RandomNumberGenerator.GetBytes(bits / 8));
            }
            return num;
        }
        static public void oddGen(int bits, int count)
        {
            if (count == 0) return;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            int ccount = 1;
            Parallel.For(0, count, i =>
            {
                BigInteger num = generator(bits);
                int fCount = factorCount(num);
                lock (ConsoleLock)
                {
                    Console.WriteLine(Environment.NewLine + ccount + ": " + num);
                    Console.WriteLine("Number of factors: " + fCount);
                    ccount++;
                }
            }
            );
            sw.Stop();
            TimeSpan t = sw.Elapsed;
            Console.WriteLine("Time to Generate: " + t);
            return;
        }
        static public void primeGen(int bits, int count)
        {
            if (count == 0) return;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            int ccount = 1;
            Parallel.For(0, count, i =>
            {
                BigInteger num = generator(bits);
                Boolean prime = num.IsProbablyPrime();
                while (!prime)
                {
                    num = generator(bits);
                    prime = num.IsProbablyPrime();
                }
                lock (ConsoleLock)
                {
                    Console.WriteLine(Environment.NewLine + ccount + ": " + num);
                    ccount++;
                }
            }
            );
            sw.Stop();
            TimeSpan t = sw.Elapsed;
            Console.WriteLine("Time to Generate: " + t);
            return;

        }
        static private int factorCount(BigInteger num)
        {
            int count = 2;
            for(int i = 3; i < num /2; i += 2)
            {
                if (num%i == 0)
                {
                    count++;
                }
            }
            return count;
        }
    }
    public static class MyExtension
    {
        public static Boolean IsProbablyPrime(this BigInteger value, int k = 10)
        {
            BigInteger r;
            BigInteger d;
            (r, d) = nbull(value);
            for (int i = 0; i < k; i++)
            {
                BigInteger a = aCalc(value);
                BigInteger x = BigInteger.ModPow(a, d, value);
                if (x == BigInteger.One | x == value - 1) continue;
                for (int j = 0; j < r - 1; j++)
                {
                    x = BigInteger.ModPow(x, 2, value);
                    if (x == value - 1) continue;
                }
                return false;
            }
            return true;
        }
        static private BigInteger aCalc(BigInteger value)
        {
            BigInteger a;
            a = NumberGen.generator(value.GetByteCount() - 1);
            return a;
        }
        static private (BigInteger, BigInteger) nbull(BigInteger value)
        {
            value--;
            BigInteger num = value;
            BigInteger counter = 0;
            while (num % 1 != 0)
            {
                counter++;
                value = num;
                num = num / 2;
            }
            return (counter, value);
        }
    }
}