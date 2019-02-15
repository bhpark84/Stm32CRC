using System;
using System.Diagnostics;

namespace Stm32CRC
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var sw = new Stopwatch();

            for (var j = 0; j < 10; j++)
            {
                var rand = new Random();
                var size = rand.Next(1024 * 1024, 4*1024*1024);
                size = size / 4 * 4;

                var inputdataByte = new byte[size];

                for (var i = 0; i < size; i++) inputdataByte[i] = (byte) rand.Next(256);

                var inputdataInt = new uint[size / 4];
                for (var i = 0; i < inputdataByte.Length / 4; i++)
                    inputdataInt[i] = BitConverter.ToUInt32(inputdataByte, i * 4);

                sw.Restart();
                var crc1 = ComputeStm32CheckSum(inputdataInt);
                sw.Stop();
                var t1 = sw.Elapsed;

                sw.Restart();
                var crc2 = Stm32Crc.Compute(inputdataByte);
                sw.Stop();
                var t2 = sw.Elapsed;

                if (crc1 != crc2)
                {
                    Console.WriteLine("ERR");
                    break;
                }

                Console.WriteLine("{0}\tsize:{1}\tTime: {2}\t{3}\tx{4}", j, size,t1,t2,t1.Ticks / t2.Ticks);

                if (j == 99) Console.WriteLine("PASS");
            }

            Console.ReadLine();
        }

        public static uint ComputeStm32CheckSum(uint[] inputData, uint initial = 0xFFFFFFFF,
            uint polynomial = 0x04C11DB7)
        {
            var crc = initial;

            foreach (var input in inputData)
            {
                crc ^= input;

                // Process all the bits in input data.
                for (uint bitIndex = 0; bitIndex < 32; ++bitIndex)
                    // If the MSB for CRC == 1
                    if ((crc & 0x80000000) != 0)
                        crc = (crc << 1) ^ polynomial;
                    else
                        crc <<= 1;
            }

            return crc;
        }
    }
}