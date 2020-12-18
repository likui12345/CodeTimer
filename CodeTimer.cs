using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace ConsoleCodeTimerApp
{
    public static class CodeTimer
    {
        //当前进程及当前线程的优先级设为最高，这样便可以相对减少操作系统在调度上造成的干扰。然后调用一次Time方法进行“预热”，
        //让JIT将IL编译成本地代码，让Time方法尽快“进入状态”。Time方法则是真正用于性能计数的方法
        public static void Initalize()
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            Time("", 1, () => { });

        }

        public static void Time(string name,int iteration,Action action)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            //1、保留当前控制台前景色，并使用黄色输出名称参数。
            ConsoleColor currentForeColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(name);
            
            //2、第一行：强制从0代到指定代进行垃圾回收,立刻执行
            //2、强制GC进行收集，并记录目前各代已经收集的次数。
            GC.Collect(GC.MaxGeneration,GCCollectionMode.Forced);
            int[] gcCounts = new int[GC.MaxGeneration + 1];
            for (int i = 0; i < GC.MaxGeneration; i++)
            {
                gcCounts[i] = GC.CollectionCount(i);
            }

            //3、执行代码，记录下消耗的时间及CPU时钟周期
            Stopwatch watch = new Stopwatch();
            watch.Start();
            ulong cycleCount = GetCycleCount();
            for (int i = 0; i < iteration; i++) action();
            ulong cpuCycles = GetCycleCount() - cycleCount;

            //4、恢复控制台默认前景色，并打印出消耗时间及CPU时钟周期。
            Console.ForegroundColor = currentForeColor;
            Console.WriteLine("\tTime Elapsed:\t" + watch.ElapsedMilliseconds.ToString("N0") + "ms");
            Console.WriteLine("\tCPU Cycles:\t" + cpuCycles.ToString("N0"));

            //5、打印执行过程中各代垃圾收集回收次数。
            for (int i = 0; i < GC.MaxGeneration; i++)
            {
                int count = GC.CollectionCount(i) - gcCounts[i];
                Console.WriteLine("\tGen " + i + ": \t\t" + count);
            }

            Console.WriteLine();
        }


        private static ulong GetCycleCount()
        {
            ulong cycleCount = 0;
            QueryThreadCycleTime(GetCurrentThread(), ref cycleCount);
            return cycleCount;
        }

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool QueryThreadCycleTime(IntPtr threadHandle, ref ulong cycleTime);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetCurrentThread();

    }
}
