using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace StoryScript
{
    public sealed class StoryHelper
    {
        public static T ConvertTo<T>(object obj)
        {
            if (obj is T) {
                return (T)obj;
            } else {
                try {
                    return (T)Convert.ChangeType(obj, typeof(T));
                }
                catch (OverflowException) {
                    return (T)Convert.ChangeType(obj.ToString(), typeof(T));
                }
                catch {
                    return default(T);
                }
            }
        }
        public static object ConvertTo(object obj, Type type)
        {
            if(obj == null) { 
                return null;
            }
            if (type.IsAssignableFrom(obj.GetType()) || obj.GetType().IsSubclassOf(type)) {
                return obj;
            } else {
                try {
                    return Convert.ChangeType(obj, type);
                }
                catch (OverflowException) {
                    return Convert.ChangeType(obj.ToString(), type);
                }
                catch {
                    return null;
                }
            }
        }        
        public static bool IsSameFloat(float arg0, float arg1)
        {
            return Math.Abs(arg0 - arg1) < c_Precision;
        }
        public static bool IsSameVector3(UnityEngine.Vector3 arg0, UnityEngine.Vector3 arg1)
        {
            return IsSameFloat(arg0.x, arg1.x) && IsSameFloat(arg0.y, arg1.y) && IsSameFloat(arg0.z, arg1.z);
        }

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
        public static string RemoveExtension(string filePath)
        {
            if (Path.HasExtension(filePath)) {
                var dir = Path.GetDirectoryName(filePath);
                var name = Path.GetFileNameWithoutExtension(filePath);
                filePath = Path.Combine(dir, name);
                filePath = filePath.Replace('\\', '/');
            }
            return filePath;
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

