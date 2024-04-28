using System;
using System.Threading;

namespace ET
{
    public static class Program
    {
        public static void Main()
        {
            castSpell();
            await time1();  0.3
            hit1();
            await time2();  0.4
            hit2();
            await time3();  0.5
            hit3();
        }
    }
}