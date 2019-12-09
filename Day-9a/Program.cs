using System;
using System.IO;
using System.Linq;

namespace Day_9a
{
    class Program
    {
        static void Main(string[] args)
        {
            long[] code = File.ReadAllText(@"day9a-input.txt").Split(",").Select(long.Parse).ToArray();

            //code = new long[] { 109, 8, 203, 1, 204, 1, 99, 0, 0 };

            Intcode processor = new Intcode(code);
            processor.Process();
        }
    }
}
