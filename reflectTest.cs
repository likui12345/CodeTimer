using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace ConsoleCodeTimerApp
{
    public class reflectTest
    {
        //测试方法的直接调用和通过反射实例对象的性能差异
        public void test()
        {
            CodeTimer.Time("Direct", 100 * 10000,
          () =>
          {
              var instance = new object();
          });

            CodeTimer.Time("Reflect", 100 * 10000,
                () =>
                {
                    this.GetType().Assembly.CreateInstance("ConsoleCodeTimerApp.reflactTest");
                });
        }
       
    }
}
