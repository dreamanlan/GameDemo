using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace StoryScript
{
    public sealed class StoryHelper
    {
        public static void LogCallStack()
        {
            LogCallStack(true);
        }
        public static void LogCallStack(bool useErrorLog)
        {
            if (useErrorLog)
                LogSystem.Error("LogCallStack:\n{0}\n", Environment.StackTrace);
            else
                LogSystem.Warn("LogCallStack:\n{0}\n", Environment.StackTrace);
        }
        public static void LogInnerException(Exception ex, StringBuilder sb)
        {
            while (null != ex.InnerException) {
                ex = ex.InnerException;
                sb.AppendFormat("\t=> exception:{0} stack:{1}", ex.Message, ex.StackTrace);
                sb.AppendLine();
            }
        }

        public sealed class Random
        {
            public static int Next()
            {
                return NextInt() % 100;
            }
            public static int Next(int max)
            {
                return NextInt() % max;
            }
            public static int Next(int min, int max)
            {
                int delta = max - min;
                return NextInt() % delta + min;
            }
            public static float NextFloat()
            {
                return (float)SysRand.NextDouble();
            }

            private static int NextInt()
            {
                return SysRand.Next();
            }
            private static System.Random SysRand
            {
                get
                {
                    if (null == s_Rand) {
                        s_Rand = new System.Random();
                    }
                    return s_Rand;
                }
            }
            [ThreadStatic]
            private static System.Random s_Rand = null;
        }

        static private float c_Precision = 0.001f;
    }
}

