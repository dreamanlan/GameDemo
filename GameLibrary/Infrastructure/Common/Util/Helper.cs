﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace GameLibrary
{
    public sealed class Helper
    {
        public static void BubbleSort<T>(List<T> list, Comparison<T> comparison)
        {
            int ct = list.Count;
            for (int i = ct - 1; i >= 0; --i) {
                for (int j = 0; j < i; ++j) {
                    T iv = list[j];
                    T jv = list[j + 1];
                    if (comparison(iv, jv) > 0) {
                        list[j] = jv;
                        list[j + 1] = iv;
                    }
                }
            }
        }
        public static byte CalcCrc8(byte[] data)
        {
            return Crc8.Instance.Calc(data);
        }
        public static int CalcCrc8WithCrcPoly(byte[] data)
        {
            byte crcPoly = 0x85;//CRC polynomial as divisor
            return CalcCrc8WithCrcPoly(crcPoly, data);
        }
        public static int CalcCrc8WithCrcPoly(byte crcPoly, byte[] data)
        {
            byte[] Data = data;
            byte CRCTempResult = 0x00;//The result of the CRC operation, but not the final value
            byte CRCResult = 0x00;//CRC result operation final value

            CRCTempResult = (byte)(Data[0] ^ crcPoly);

            for (int arrayLength = 1; arrayLength <= (data.Length - 1); arrayLength++) {
                for (int i = 0; i < 8; i++) {
                    if ((CRCTempResult & 0x80) == 0x00) {
                        CRCTempResult = (byte)(CRCTempResult << 1);
                        CRCTempResult = (byte)(
                            CRCTempResult |
                            ((Data[arrayLength] & 0x80) == 0x80 ? 0x01 : 0x00)
                            );
                        Data[arrayLength] <<= 1;
                    } else {
                        CRCTempResult = (byte)(CRCTempResult ^ crcPoly);
                        i--;
                    }
                }
            }
            CRCResult = CRCTempResult;
            return CRCResult;
        }
        public static string BinToHex(byte[] bytes)
        {
            return BinToHex(bytes, 0);
        }
        public static string BinToHex(byte[] bytes, int start)
        {
            return BinToHex(bytes, start, bytes.Length - start);
        }
        public static string BinToHex(byte[] bytes, int start, int count)
        {
            if (start < 0 || count <= 0 || start + count > bytes.Length)
                return "";
            StringBuilder sb = new StringBuilder(count * 4);
            for (int ix = 0; ix < count; ++ix) {
                sb.AppendFormat("{0,2:X2}", bytes[ix + start]);
                if ((ix + 1) % 16 == 0)
                    sb.AppendLine();
                else
                    sb.Append(' ');
            }
            return sb.ToString();
        }

        public static bool StringIsNullOrEmpty(string str)
        {
            if (str == null || str == "")
                return true;
            return false;
        }

        public static bool IsSameFloat(float arg0, float arg1)
        {
            return Math.Abs(arg0 - arg1) < c_Precision;
        }
        public static bool IsSameVector3(UnityEngine.Vector3 arg0, UnityEngine.Vector3 arg1)
        {
            return IsSameFloat(arg0.x, arg1.x) && IsSameFloat(arg0.y, arg1.y) && IsSameFloat(arg0.z, arg1.z);
        }
        public static bool IsDifferentMonth(DateTime last_time)
        {
            if (last_time.Year != DateTime.Now.Year || last_time.Month != DateTime.Now.Month) {
                return true;
            }
            return false;
        }
        public static bool IsDifferentDay(double last_time)
        {
            DateTime dt = (new DateTime(1970, 1, 1, 0, 0, 0)).AddHours(8).AddSeconds(last_time);
            if (dt.Year != DateTime.Now.Year || dt.Month != DateTime.Now.Month || dt.Day != DateTime.Now.Day) {
                return true;
            }
            return false;
        }
        public static bool IsDifferentDay(DateTime last_time)
        {
            if (last_time.Year != DateTime.Now.Year || last_time.Month != DateTime.Now.Month || last_time.Day != DateTime.Now.Day) {
                return true;
            }
            return false;
        }
        public static bool IsDifferentMinute(DateTime time)
        {
            if (time.Minute != DateTime.Now.Minute) {
                return true;
            }
            return false;
        }
        public static bool IsInterval24Hours(DateTime last_time)
        {
            double c_hours_per_day = 24f;
            TimeSpan ts = DateTime.Now - last_time;
            if (ts.TotalHours >= c_hours_per_day) {
                return true;
            }
            return false;
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
    internal class Crc8
    {
        internal byte Calc(byte[] ptr)
        {
            int len = ptr.Length;
            byte crc = 0;
            for (int i = 0; i < len; ++i) {
                crc = crc8Table[crc ^ ptr[i]];
            }
            return (crc);
        }

        internal Crc8()
        {
            InitCrcTable(crc8Table, 0x07); //CRC-8 x8+x2+x1+1 0x07 
        }

        private void InitCrcTable(byte[] crcTable8, byte key)
        {
            for (int i = 0; i < 256; i++) {
                byte cRc_8 = (byte)i;
                for (byte j = 8; j > 0; j--) {
                    if ((cRc_8 & 0x80) != 0)
                        cRc_8 = (byte)((cRc_8 << 1) ^ key);
                    else
                        cRc_8 <<= 1;
                }
                crcTable8[i] = cRc_8;
            }
        }

        private byte[] crc8Table = new byte[256];

        private const byte CSHeadSize = 7;
        private const byte SCHeadSize = 4;
        private const byte CSCrcOffset = 5;
        private const byte CommandOffset = 2;
        private const byte LengthOffset = 0;

        internal static Crc8 Instance
        {
            get { return s_Instance; }
        }
        private static Crc8 s_Instance = new Crc8();
    }
}

