using System;
using System.Text;
using System.Threading;

namespace ConsoleCodeTimerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //int iteration = 100*10000;
            //string s = "";
            //CodeTimer.Initalize();
            //CodeTimer.Time("String Concat", iteration, () => { s += "a"; });

            //StringBuilder sb = new StringBuilder();
            //CodeTimer.Time("StringBuilder", iteration, () => { sb.Append("a"); });

            #region 反射的性能差异
            //CodeTimer.Initalize();
            reflectTest reflect = new reflectTest();
            reflect.test();
            #endregion
            Console.ReadKey();

        }
    }
}
