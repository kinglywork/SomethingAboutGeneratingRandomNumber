using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SomethingAboutGeneratingRandomNumber
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("start");
            List<int> valueSet1, valueSet2, valueSet3;
            const int count = 10;
            const int min = 1;
            const int max = 10;

            Console.WriteLine("----------------different instance");
            Console.WriteLine(Environment.TickCount);
            valueSet1 = GetRandomValuesFromRandom(new Random(), count, min, max);
            Console.WriteLine(Environment.TickCount);
            valueSet2 = GetRandomValuesFromRandom(new Random(), count, min, max);
            Console.WriteLine(Environment.TickCount);
            valueSet3 = GetRandomValuesFromRandom(new Random(), count, min, max);
            Console.WriteLine(Environment.TickCount);
            PrintValues(valueSet1, valueSet2, valueSet3);

            Console.WriteLine("----------------different instance with sleep");
            Console.WriteLine(Environment.TickCount);
            valueSet1 = GetRandomValuesFromRandom(new Random(), count, min, max);
            Console.WriteLine(Environment.TickCount);
            Thread.Sleep(100);
            valueSet2 = GetRandomValuesFromRandom(new Random(), count, min, max);
            Console.WriteLine(Environment.TickCount);
            valueSet3 = GetRandomValuesFromRandom(new Random(), count, min, max);
            Console.WriteLine(Environment.TickCount);
            PrintValues(valueSet1, valueSet2, valueSet3);

            Console.WriteLine("----------------different instance with guid seed");
            valueSet1 = GetRandomValuesFromRandom(new Random(Guid.NewGuid().GetHashCode()), count, min, max);
            valueSet2 = GetRandomValuesFromRandom(new Random(Guid.NewGuid().GetHashCode()), count, min, max);
            valueSet3 = GetRandomValuesFromRandom(new Random(Guid.NewGuid().GetHashCode()), count, min, max);
            PrintValues(valueSet1, valueSet2, valueSet3);

            Console.WriteLine("----------------same instance");
            var rnd = new Random();
            valueSet1 = GetRandomValuesFromRandom(rnd, count, min, max);
            valueSet2 = GetRandomValuesFromRandom(rnd, count, min, max);
            valueSet3 = GetRandomValuesFromRandom(rnd, count, min, max);
            PrintValues(valueSet1, valueSet2, valueSet3);

            Console.WriteLine("----------------use RNGCryptoServiceProvider");
            valueSet1 = GetRandomValuesWithRNGCryptoServiceProvider(count, min, max);
            valueSet2 = GetRandomValuesWithRNGCryptoServiceProvider(count, min, max);
            valueSet3 = GetRandomValuesWithRNGCryptoServiceProvider(count, min, max);
            PrintValues(valueSet1, valueSet2, valueSet3);

            Console.WriteLine("all done");

            Console.ReadLine();
        }

        private static List<int> GetRandomValuesFromRandom(Random rnd, int count, int min, int max)
        {
            var values = new List<int>();
            for (var i = 0; i < count; i++)
            {
                values.Add(rnd.Next(min, max));
            }

            return values;
        }

        // https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.rngcryptoserviceprovider?redirectedfrom=MSDN&view=netframework-4.8
        // https://stackoverflow.com/questions/6299197/rngcryptoserviceprovider-generate-number-in-a-range-faster-and-retain-distribu
        static List<int> GetRandomValuesWithRNGCryptoServiceProvider(int count, int min, int max)
        {
            var values = new List<int>();
            //or using (var rng = RandomNumberGenerator.Create())
            using (var rng = new RNGCryptoServiceProvider())
            {
                var randomNumber = new byte[4];
                for (var i = 0; i < count; i++)
                {
                    var value = RandomNumberGeneratorNext(rng, randomNumber, min, max);
                    values.Add(value);
                }
            }

            return values;
        }

        private static int RandomNumberGeneratorNext(RandomNumberGenerator rng, byte[] buffer, int minValue, int maxValue)
        {
            if (minValue == maxValue) return minValue;
            long diff = maxValue - minValue;
            while (true)
            {
                rng.GetBytes(buffer);
                var rand = BitConverter.ToUInt32(buffer, 0);

                var max = (1 + (long) uint.MaxValue);
                var remainder = max % diff;
                if (rand < max - remainder)
                {
                    return (int) (minValue + (rand % diff));
                }
            }
        }

        private static void PrintValues(params List<int>[] valuesArray)
        {
            valuesArray.ToList().ForEach(values => Console.WriteLine(string.Join(",", values)));
        }
    }
}