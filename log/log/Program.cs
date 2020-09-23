using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace log
{
    class Program
    {
        static void TestSpeed1()
        {
            log.Delete();
            log.SerialNumber("1111111111");

            int i = 0;
            var start = DateTime.Now.Ticks;
            for (i = 0; i < 300; i++)
            {
                log.WriteLine("Hello world!!! " + i.ToString());
            }
            var end = DateTime.Now.Ticks;
            var diff = (end - start) / 10000;
            log.WriteLine(diff.ToString() + "ms");
            Console.WriteLine(diff.ToString() + "ms");
        }
        static void TestSpeed2()
        {
            log.Delete();
            log.SerialNumber("22222222222");

            int i = 0;
            var start = DateTime.Now.Ticks;
            for (i = 0; i < 200; i++)
            {
                log.WriteLine("Hi world!!! " + i.ToString());
            }
            var end = DateTime.Now.Ticks;
            var diff = (end - start) / 10000;
            log.WriteLine(diff.ToString() + "ms");
            Console.WriteLine(diff.ToString() + "ms");
        }
        static void Main(string[] args)
        {
            Thread th1 = new Thread(TestSpeed1);
            Thread th2 = new Thread(TestSpeed2);
            th1.Start();
            th2.Start();
            Console.ReadLine();
        }
    }
}
