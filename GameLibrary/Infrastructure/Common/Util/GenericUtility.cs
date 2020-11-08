using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using UnityEngine;

public class BoolList : List<bool>
{
    public BoolList() { }
    public BoolList(IEnumerable<bool> coll) : base(coll) { }
}
public class CharList : List<char>
{
    public CharList() { }
    public CharList(IEnumerable<char> coll) : base(coll) { }
}
public class ByteList : List<byte>
{
    public ByteList() { }
    public ByteList(IEnumerable<byte> coll) : base(coll) { }
}
public class SbyteList : List<sbyte>
{
    public SbyteList() { }
    public SbyteList(IEnumerable<sbyte> coll) : base(coll) { }
}
public class ShortList : List<short>
{
    public ShortList() { }
    public ShortList(IEnumerable<short> coll) : base(coll) { }
}
public class UshortList : List<ushort>
{
    public UshortList() { }
    public UshortList(IEnumerable<ushort> coll) : base(coll) { }
}
public class UintList : List<uint>
{
    public UintList() { }
    public UintList(IEnumerable<uint> coll) : base(coll) { }
}
public class LongList : List<long>
{
    public LongList() { }
    public LongList(IEnumerable<long> coll) : base(coll) { }
}
public class UlongList : List<ulong>
{
    public UlongList() { }
    public UlongList(IEnumerable<ulong> coll) : base(coll) { }
}
public class DoubleList : List<double>
{
    public DoubleList() { }
    public DoubleList(IEnumerable<double> coll) : base(coll) { }
}

public class IntList : List<int>
{
    public IntList() { }
    public IntList(IEnumerable<int> coll) : base(coll) { }
}
public class FloatList : List<float>
{
    public FloatList() { }
    public FloatList(IEnumerable<float> coll) : base(coll) { }
}
public class StrList : List<string>
{
    public StrList() { }
    public StrList(IEnumerable<string> coll) : base(coll) { }
}
public class ObjList : List<object>
{
    public ObjList() { }
    public ObjList(IEnumerable<object> coll) : base(coll) { }
}
public class IntIntDict : Dictionary<int, int>
{
    public IntIntDict() { }
    public IntIntDict(IDictionary<int, int> dict) : base(dict) { }
}
public class IntStrDict : Dictionary<int, string>
{
    public IntStrDict() { }
    public IntStrDict(IDictionary<int, string> dict) : base(dict) { }
}
public class IntObjDict : Dictionary<int, object>
{
    public IntObjDict() { }
    public IntObjDict(IDictionary<int, object> dict) : base(dict) { }
}
public class StrIntDict : Dictionary<string, int>
{
    public StrIntDict() { }
    public StrIntDict(IDictionary<string, int> dict) : base(dict) { }
}
public class StrStrDict : Dictionary<string, string>
{
    public StrStrDict() { }
    public StrStrDict(IDictionary<string, string> dict) : base(dict) { }
}
public class StrObjDict : Dictionary<string, object>
{
    public StrObjDict() { }
    public StrObjDict(IDictionary<string, object> dict) : base(dict) { }
}
public class ObjIntDict : Dictionary<object, int>
{
    public ObjIntDict() { }
    public ObjIntDict(IDictionary<object, int> dict) : base(dict) { }
}
public class ObjStrDict : Dictionary<object, string>
{
    public ObjStrDict() { }
    public ObjStrDict(IDictionary<object, string> dict) : base(dict) { }
}
public class ObjObjDict : Dictionary<object, object>
{
    public ObjObjDict() { }
    public ObjObjDict(IDictionary<object, object> dict) : base(dict) { }
}
public class IntHashSet : HashSet<int>
{
    public IntHashSet() { }
    public IntHashSet(IEnumerable<int> coll) : base(coll) { }
}
public class StrHashSet : HashSet<string>
{
    public StrHashSet() { }
    public StrHashSet(IEnumerable<string> coll) : base(coll) { }
}
public class ObjHashSet : HashSet<object>
{
    public ObjHashSet() { }
    public ObjHashSet(IEnumerable<object> coll) : base(coll) { }
}
public class IntQueue : Queue<int>
{    
    public IntQueue() { }
    public IntQueue(IEnumerable<int> coll) : base(coll) { }
}
public class ObjQueue : Queue<object>
{
    public ObjQueue() { }
    public ObjQueue(IEnumerable<object> coll) : base(coll) { }
}
public class StrQueue : Queue<string>
{
    public StrQueue() { }
    public StrQueue(IEnumerable<string> coll) : base(coll) { }
}
public class IntStack : Stack<int>
{
    public IntStack() { }
    public IntStack(IEnumerable<int> coll) : base(coll) { }
}
public class ObjStack : Stack<object>
{
    public ObjStack() { }
    public ObjStack(IEnumerable<object> coll) : base(coll) { }
}
public class StrStack : Stack<string>
{
    public StrStack() { }
    public StrStack(IEnumerable<string> coll) : base(coll) { }
}
public class IntObjSortedDict : SortedDictionary<int, object>
{
    public IntObjSortedDict() { }
    public IntObjSortedDict(IDictionary<int, object> dict) : base(dict) { }
}

//unity相关
public class UobjList : List<UnityEngine.Object>
{
    public UobjList() { }
    public UobjList(ICollection<UnityEngine.Object> coll) : base(coll) { }
}
public class Vector2List : List<UnityEngine.Vector2>
{
    public Vector2List() { }
    public Vector2List(ICollection<UnityEngine.Vector2> coll) : base(coll) { }
}
public class Vector3List : List<UnityEngine.Vector3>
{
    public Vector3List() { }
    public Vector3List(ICollection<UnityEngine.Vector3> coll) : base(coll) { }
}
public class IntUobjDict : Dictionary<int, UnityEngine.Object>
{
    public IntUobjDict() { }
    public IntUobjDict(IDictionary<int, UnityEngine.Object> dict) : base(dict) { }
}
public class StrUobjDict : Dictionary<string, UnityEngine.Object>
{
    public StrUobjDict() { }
    public StrUobjDict(IDictionary<string, UnityEngine.Object> dict) : base(dict) { }
}
public class ObjUobjDict : Dictionary<object, UnityEngine.Object>
{
    public ObjUobjDict() { }
    public ObjUobjDict(IDictionary<object, UnityEngine.Object> dict) : base(dict) { }
}
public class UobjIntDict : Dictionary<UnityEngine.Object, int>
{
    public UobjIntDict() { }
    public UobjIntDict(IDictionary<UnityEngine.Object, int> dict) : base(dict) { }
}
public class UobjStrDict : Dictionary<UnityEngine.Object, string>
{
    public UobjStrDict() { }
    public UobjStrDict(IDictionary<UnityEngine.Object, string> dict) : base(dict) { }
}
public class UobjObjDict : Dictionary<UnityEngine.Object, object>
{
    public UobjObjDict() { }
    public UobjObjDict(IDictionary<UnityEngine.Object, object> dict) : base(dict) { }
}
public class UobjUobjDict : Dictionary<UnityEngine.Object, UnityEngine.Object>
{
    public UobjUobjDict() { }
    public UobjUobjDict(IDictionary<UnityEngine.Object, UnityEngine.Object> dict) : base(dict) { }
}
public class UobjHashSet : HashSet<UnityEngine.Object>
{
    public UobjHashSet() { }
    public UobjHashSet(IEnumerable<UnityEngine.Object> coll) : base(coll) { }
}
public class UobjQueue : Queue<UnityEngine.Object>
{
    public UobjQueue() { }
    public UobjQueue(IEnumerable<UnityEngine.Object> coll) : base(coll) { }
}
public class UobjStack : Stack<UnityEngine.Object>
{
    public UobjStack() { }
    public UobjStack(IEnumerable<UnityEngine.Object> coll) : base(coll) { }
}
public class IntUobjSortedDict : SortedDictionary<int, UnityEngine.Object>
{
    public IntUobjSortedDict() { }
    public IntUobjSortedDict(IDictionary<int, UnityEngine.Object> dict) : base(dict) { }
}

public static class GenericValueConverter
{
    public static BoxedValue ToBoxedValue<T>(T v)
    {
        var from = s_FromBoxedValue as FromGenericDelegation<BoxedValue, T>;
        if (null != from)
            return from(v);
        return BoxedValue.NullObject;
    }
    public static object ToObject<T>(T v)
    {
        var from = s_FromObject as FromGenericDelegation<object, T>;
        if (null != from)
            return from(v);
        return null;
    }
    public static string ToString<T>(T v)
    {
        var from = s_FromString as FromGenericDelegation<string, T>;
        if (null != from)
            return from(v);
        return null;
    }
    public static bool ToBool<T>(T v)
    {
        var from = s_FromBool as FromGenericDelegation<bool, T>;
        if (null != from)
            return from(v);
        return false;
    }
    public static char ToChar<T>(T v)
    {
        var from = s_FromChar as FromGenericDelegation<char, T>;
        if (null != from)
            return from(v);
        return (char)0;
    }
    public static sbyte ToSByte<T>(T v)
    {
        var from = s_FromSByte as FromGenericDelegation<sbyte, T>;
        if (null != from)
            return from(v);
        return 0;
    }
    public static short ToShort<T>(T v)
    {
        var from = s_FromShort as FromGenericDelegation<short, T>;
        if (null != from)
            return from(v);
        return 0;
    }
    public static int ToInt<T>(T v)
    {
        var from = s_FromInt as FromGenericDelegation<int, T>;
        if (null != from)
            return from(v);
        return 0;
    }
    public static long ToLong<T>(T v)
    {
        var from = s_FromLong as FromGenericDelegation<long, T>;
        if (null != from)
            return from(v);
        return 0;
    }
    public static byte ToByte<T>(T v)
    {
        var from = s_FromByte as FromGenericDelegation<byte, T>;
        if (null != from)
            return from(v);
        return 0;
    }
    public static ushort ToUShort<T>(T v)
    {
        var from = s_FromUShort as FromGenericDelegation<ushort, T>;
        if (null != from)
            return from(v);
        return 0;
    }
    public static uint ToUInt<T>(T v)
    {
        var from = s_FromUInt as FromGenericDelegation<uint, T>;
        if (null != from)
            return from(v);
        return 0;
    }
    public static ulong ToULong<T>(T v)
    {
        var from = s_FromULong as FromGenericDelegation<ulong, T>;
        if (null != from)
            return from(v);
        return 0;
    }
    public static float ToFloat<T>(T v)
    {
        var from = s_FromFloat as FromGenericDelegation<float, T>;
        if (null != from)
            return from(v);
        return 0;
    }
    public static double ToDouble<T>(T v)
    {
        var from = s_FromDouble as FromGenericDelegation<double, T>;
        if (null != from)
            return from(v);
        return 0;
    }
    public static decimal ToDecimal<T>(T v)
    {
        var from = s_FromDecimal as FromGenericDelegation<decimal, T>;
        if (null != from)
            return from(v);
        return 0;
    }
    public static UnityEngine.Vector2 ToVector2<T>(T v)
    {
        var from = s_FromVector2 as FromGenericDelegation<UnityEngine.Vector2, T>;
        if (null != from)
            return from(v);
        return UnityEngine.Vector2.zero;
    }
    public static UnityEngine.Vector3 ToVector3<T>(T v)
    {
        var from = s_FromVector3 as FromGenericDelegation<UnityEngine.Vector3, T>;
        if (null != from)
            return from(v);
        return UnityEngine.Vector3.zero;
    }
    public static UnityEngine.Vector4 ToVector4<T>(T v)
    {
        var from = s_FromVector4 as FromGenericDelegation<UnityEngine.Vector4, T>;
        if (null != from)
            return from(v);
        return UnityEngine.Vector4.zero;
    }
    public static UnityEngine.Quaternion ToQuaternion<T>(T v)
    {
        var from = s_FromQuaternion as FromGenericDelegation<UnityEngine.Quaternion, T>;
        if (null != from)
            return from(v);
        return UnityEngine.Quaternion.identity;
    }
    public static UnityEngine.Color ToColor<T>(T v)
    {
        var from = s_FromColor as FromGenericDelegation<UnityEngine.Color, T>;
        if (null != from)
            return from(v);
        return UnityEngine.Color.black;
    }
    public static UnityEngine.Color32 ToColor32<T>(T v)
    {
        var from = s_FromColor32 as FromGenericDelegation<UnityEngine.Color32, T>;
        if (null != from)
            return from(v);
        return UnityEngine.Color.black;
    }

    public static T From<T>(BoxedValue v)
    {
        var from = s_FromBoxedValue as FromGenericDelegation<T, BoxedValue>;
        if (null != from)
            return from(v);
        return default(T);
    }
    public static T From<T>(object v)
    {
        var from = s_FromObject as FromGenericDelegation<T, object>;
        if (null != from)
            return from(v);
        return default(T);
    }
    public static T From<T>(string v)
    {
        var from = s_FromString as FromGenericDelegation<T, string>;
        if (null != from)
            return from(v);
        return default(T);
    }
    public static T From<T>(bool v)
    {
        var from = s_FromBool as FromGenericDelegation<T, bool>;
        if (null != from)
            return from(v);
        return default(T);
    }
    public static T From<T>(char v)
    {
        var from = s_FromChar as FromGenericDelegation<T, char>;
        if (null != from)
            return from(v);
        return default(T);
    }
    public static T From<T>(sbyte v)
    {
        var from = s_FromSByte as FromGenericDelegation<T, sbyte>;
        if (null != from)
            return from(v);
        return default(T);
    }
    public static T From<T>(short v)
    {
        var from = s_FromShort as FromGenericDelegation<T, short>;
        if (null != from)
            return from(v);
        return default(T);
    }
    public static T From<T>(int v)
    {
        var from = s_FromInt as FromGenericDelegation<T, int>;
        if (null != from)
            return from(v);
        return default(T);
    }
    public static T From<T>(long v)
    {
        var from = s_FromLong as FromGenericDelegation<T, long>;
        if (null != from)
            return from(v);
        return default(T);
    }
    public static T From<T>(byte v)
    {
        var from = s_FromByte as FromGenericDelegation<T, byte>;
        if (null != from)
            return from(v);
        return default(T);
    }
    public static T From<T>(ushort v)
    {
        var from = s_FromUShort as FromGenericDelegation<T, ushort>;
        if (null != from)
            return from(v);
        return default(T);
    }
    public static T From<T>(uint v)
    {
        var from = s_FromUInt as FromGenericDelegation<T, uint>;
        if (null != from)
            return from(v);
        return default(T);
    }
    public static T From<T>(ulong v)
    {
        var from = s_FromULong as FromGenericDelegation<T, ulong>;
        if (null != from)
            return from(v);
        return default(T);
    }
    public static T From<T>(float v)
    {
        var from = s_FromFloat as FromGenericDelegation<T, float>;
        if (null != from)
            return from(v);
        return default(T);
    }
    public static T From<T>(double v)
    {
        var from = s_FromDouble as FromGenericDelegation<T, double>;
        if (null != from)
            return from(v);
        return default(T);
    }
    public static T From<T>(decimal v)
    {
        var from = s_FromDecimal as FromGenericDelegation<T, decimal>;
        if (null != from)
            return from(v);
        return default(T);
    }
    public static T From<T>(UnityEngine.Vector2 v)
    {
        var from = s_FromVector2 as FromGenericDelegation<T, UnityEngine.Vector2>;
        if (null != from)
            return from(v);
        return default(T);
    }
    public static T From<T>(UnityEngine.Vector3 v)
    {
        var from = s_FromVector3 as FromGenericDelegation<T, UnityEngine.Vector3>;
        if (null != from)
            return from(v);
        return default(T);
    }
    public static T From<T>(UnityEngine.Vector4 v)
    {
        var from = s_FromVector4 as FromGenericDelegation<T, UnityEngine.Vector4>;
        if (null != from)
            return from(v);
        return default(T);
    }
    public static T From<T>(UnityEngine.Quaternion v)
    {
        var from = s_FromQuaternion as FromGenericDelegation<T, UnityEngine.Quaternion>;
        if (null != from)
            return from(v);
        return default(T);
    }
    public static T From<T>(UnityEngine.Color v)
    {
        var from = s_FromColor as FromGenericDelegation<T, UnityEngine.Color>;
        if (null != from)
            return from(v);
        return default(T);
    }
    public static T From<T>(UnityEngine.Color32 v)
    {
        var from = s_FromColor32 as FromGenericDelegation<T, UnityEngine.Color32>;
        if (null != from)
            return from(v);
        return default(T);
    }

    public static BoxedValue ToBoxedValue(Type t, object o)
    {
        return CastTo<BoxedValue>(o);
    }
    public static bool ToBool(Type t, object o)
    {
        return CastTo<bool>(o);
    }
    public static char ToChar(Type t, object o)
    {
        return CastTo<char>(o);
    }
    public static sbyte ToSByte(Type t, object o)
    {
        return CastTo<sbyte>(o);
    }
    public static short ToShort(Type t, object o)
    {
        return CastTo<short>(o);
    }
    public static int ToInt(Type t, object o)
    {
        return CastTo<int>(o);
    }
    public static long ToLong(Type t, object o)
    {
        return CastTo<long>(o);
    }
    public static byte ToByte(Type t, object o)
    {
        return CastTo<byte>(o);
    }
    public static ushort ToUShort(Type t, object o)
    {
        return CastTo<ushort>(o);
    }
    public static uint ToUInt(Type t, object o)
    {
        return CastTo<uint>(o);
    }
    public static ulong ToULong(Type t, object o)
    {
        return CastTo<ulong>(o);
    }
    public static float ToFloat(Type t, object o)
    {
        return CastTo<float>(o);
    }
    public static double ToDouble(Type t, object o)
    {
        return CastTo<double>(o);
    }
    public static decimal ToDecimal(Type t, object o)
    {
        return CastTo<decimal>(o);
    }
    public static string ToString(Type t, object o)
    {
        return CastTo<string>(o);
    }
    public static object ToObject(Type t, object o)
    {
        return o;
    }
    public static UnityEngine.Vector2 ToVector2(Type t, object o)
    {
        return CastTo<UnityEngine.Vector2>(o);
    }
    public static UnityEngine.Vector3 ToVector3(Type t, object o)
    {
        return CastTo<UnityEngine.Vector3>(o);
    }
    public static UnityEngine.Vector4 ToVector4(Type t, object o)
    {
        return CastTo<UnityEngine.Vector4>(o);
    }
    public static UnityEngine.Quaternion ToQuaternion(Type t, object o)
    {
        return CastTo<UnityEngine.Quaternion>(o);
    }
    public static UnityEngine.Color ToColor(Type t, object o)
    {
        return CastTo<UnityEngine.Color>(o);
    }
    public static UnityEngine.Color32 ToColor32(Type t, object o)
    {
        return CastTo<UnityEngine.Color32>(o);
    }
    public static object From(Type t, object o)
    {
        return o;
    }

    internal static T CastTo<T>(object obj)
    {
        if (obj is T) {
            return (T)obj;
        }
        else {
            try {
                return (T)Convert.ChangeType(obj, typeof(T));
            }
            catch {
                return default(T);
            }
        }
    }

    private delegate R FromGenericDelegation<R, T>(T v);
    private static FromGenericDelegation<BoxedValue, BoxedValue> s_FromBoxedValue = FromHelper<BoxedValue>;
    private static FromGenericDelegation<bool, bool> s_FromBool = FromHelper<bool>;
    private static FromGenericDelegation<char, char> s_FromChar = FromHelper<char>;
    private static FromGenericDelegation<sbyte, sbyte> s_FromSByte = FromHelper<sbyte>;
    private static FromGenericDelegation<short, short> s_FromShort = FromHelper<short>;
    private static FromGenericDelegation<int, int> s_FromInt = FromHelper<int>;
    private static FromGenericDelegation<long, long> s_FromLong = FromHelper<long>;
    private static FromGenericDelegation<byte, byte> s_FromByte = FromHelper<byte>;
    private static FromGenericDelegation<ushort, ushort> s_FromUShort = FromHelper<ushort>;
    private static FromGenericDelegation<uint, uint> s_FromUInt = FromHelper<uint>;
    private static FromGenericDelegation<ulong, ulong> s_FromULong = FromHelper<ulong>;
    private static FromGenericDelegation<float, float> s_FromFloat = FromHelper<float>;
    private static FromGenericDelegation<double, double> s_FromDouble = FromHelper<double>;
    private static FromGenericDelegation<decimal, decimal> s_FromDecimal = FromHelper<decimal>;
    private static FromGenericDelegation<string, string> s_FromString = FromHelper<string>;
    private static FromGenericDelegation<object, object> s_FromObject = FromHelper<object>;
    private static FromGenericDelegation<UnityEngine.Vector2, UnityEngine.Vector2> s_FromVector2 = FromHelper<UnityEngine.Vector2>;
    private static FromGenericDelegation<UnityEngine.Vector3, UnityEngine.Vector3> s_FromVector3 = FromHelper<UnityEngine.Vector3>;
    private static FromGenericDelegation<UnityEngine.Vector4, UnityEngine.Vector4> s_FromVector4 = FromHelper<UnityEngine.Vector4>;
    private static FromGenericDelegation<UnityEngine.Quaternion, UnityEngine.Quaternion> s_FromQuaternion = FromHelper<UnityEngine.Quaternion>;
    private static FromGenericDelegation<UnityEngine.Color, UnityEngine.Color> s_FromColor = FromHelper<UnityEngine.Color>;
    private static FromGenericDelegation<UnityEngine.Color32, UnityEngine.Color32> s_FromColor32 = FromHelper<UnityEngine.Color32>;
    private static T FromHelper<T>(T v)
    {
        return v;
    }
}

public struct BoxedValue
{
    public const int c_ObjectType = 0;
    public const int c_StringType = 1;
    public const int c_BoolType = 2;
    public const int c_CharType = 3;
    public const int c_SByteType = 4;
    public const int c_ShortType = 5;
    public const int c_IntType = 6;
    public const int c_LongType = 7;
    public const int c_ByteType = 8;
    public const int c_UShortType = 9;
    public const int c_UIntType = 10;
    public const int c_ULongType = 11;
    public const int c_FloatType = 12;
    public const int c_DoubleType = 13;
    public const int c_DecimalType = 14;
    public const int c_Vector2Type = 15;
    public const int c_Vector3Type = 16;
    public const int c_Vector4Type = 17;
    public const int c_QuaternionType = 18;
    public const int c_ColorType = 19;
    public const int c_Color32Type = 20;

    [StructLayout(LayoutKind.Explicit)]
    internal struct UnionValue
    {
        [FieldOffset(0)]
        public bool BoolVal;
        [FieldOffset(0)]
        public char CharVal;
        [FieldOffset(0)]
        public sbyte SByteVal;
        [FieldOffset(0)]
        public short ShortVal;
        [FieldOffset(0)]
        public int IntVal;
        [FieldOffset(0)]
        public long LongVal;
        [FieldOffset(0)]
        public byte ByteVal;
        [FieldOffset(0)]
        public ushort UShortVal;
        [FieldOffset(0)]
        public uint UIntVal;
        [FieldOffset(0)]
        public ulong ULongVal;
        [FieldOffset(0)]
        public float FloatVal;
        [FieldOffset(0)]
        public double DoubleVal;
        [FieldOffset(0)]
        public decimal DecimalVal;
        [FieldOffset(0)]
        public UnityEngine.Vector2 Vector2Val;
        [FieldOffset(0)]
        public UnityEngine.Vector3 Vector3Val;
        [FieldOffset(0)]
        public UnityEngine.Vector4 Vector4Val;
        [FieldOffset(0)]
        public UnityEngine.Quaternion QuaternionVal;
        [FieldOffset(0)]
        public UnityEngine.Color ColorVal;
        [FieldOffset(0)]
        public UnityEngine.Color32 Color32Val;
    }

    public string StringVal
    {
        get { return ObjectVal as string; }
        set { ObjectVal = value; }
    }
    public int Type;
    public object ObjectVal;
    private UnionValue Union;

    public static implicit operator BoxedValue(string v)
    {
        return BoxedValue.From(v);
    }
    public static implicit operator string(BoxedValue v)
    {
        return v.Get<string>();
    }
    public static implicit operator BoxedValue(bool v)
    {
        return BoxedValue.From(v);
    }
    public static implicit operator bool(BoxedValue v)
    {
        return v.Get<bool>();
    }
    public static implicit operator BoxedValue(char v)
    {
        return BoxedValue.From(v);
    }
    public static implicit operator char(BoxedValue v)
    {
        return v.Get<char>();
    }
    public static implicit operator BoxedValue(sbyte v)
    {
        return BoxedValue.From(v);
    }
    public static implicit operator sbyte(BoxedValue v)
    {
        return v.Get<sbyte>();
    }
    public static implicit operator BoxedValue(short v)
    {
        return BoxedValue.From(v);
    }
    public static implicit operator short(BoxedValue v)
    {
        return v.Get<short>();
    }
    public static implicit operator BoxedValue(int v)
    {
        return BoxedValue.From(v);
    }
    public static implicit operator int(BoxedValue v)
    {
        return v.Get<int>();
    }
    public static implicit operator BoxedValue(long v)
    {
        return BoxedValue.From(v);
    }
    public static implicit operator long(BoxedValue v)
    {
        return v.Get<long>();
    }
    public static implicit operator BoxedValue(byte v)
    {
        return BoxedValue.From(v);
    }
    public static implicit operator byte(BoxedValue v)
    {
        return v.Get<byte>();
    }
    public static implicit operator BoxedValue(ushort v)
    {
        return BoxedValue.From(v);
    }
    public static implicit operator ushort(BoxedValue v)
    {
        return v.Get<ushort>();
    }
    public static implicit operator BoxedValue(uint v)
    {
        return BoxedValue.From(v);
    }
    public static implicit operator uint(BoxedValue v)
    {
        return v.Get<uint>();
    }
    public static implicit operator BoxedValue(ulong v)
    {
        return BoxedValue.From(v);
    }
    public static implicit operator ulong(BoxedValue v)
    {
        return v.Get<ulong>();
    }
    public static implicit operator BoxedValue(float v)
    {
        return BoxedValue.From(v);
    }
    public static implicit operator float(BoxedValue v)
    {
        return v.Get<float>();
    }
    public static implicit operator BoxedValue(double v)
    {
        return BoxedValue.From(v);
    }
    public static implicit operator double(BoxedValue v)
    {
        return v.Get<double>();
    }
    public static implicit operator BoxedValue(decimal v)
    {
        return BoxedValue.From(v);
    }
    public static implicit operator decimal(BoxedValue v)
    {
        return v.Get<decimal>();
    }
    public static implicit operator BoxedValue(UnityEngine.Vector2 v)
    {
        return BoxedValue.From(v);
    }
    public static implicit operator UnityEngine.Vector2(BoxedValue v)
    {
        return v.Get<UnityEngine.Vector2>();
    }
    public static implicit operator BoxedValue(UnityEngine.Vector3 v)
    {
        return BoxedValue.From(v);
    }
    public static implicit operator UnityEngine.Vector3(BoxedValue v)
    {
        return v.Get<UnityEngine.Vector3>();
    }
    public static implicit operator BoxedValue(UnityEngine.Vector4 v)
    {
        return BoxedValue.From(v);
    }
    public static implicit operator UnityEngine.Vector4(BoxedValue v)
    {
        return v.Get<UnityEngine.Vector4>();
    }
    public static implicit operator BoxedValue(UnityEngine.Quaternion v)
    {
        return BoxedValue.From(v);
    }
    public static implicit operator UnityEngine.Quaternion(BoxedValue v)
    {
        return v.Get<UnityEngine.Quaternion>();
    }
    public static implicit operator BoxedValue(UnityEngine.Color v)
    {
        return BoxedValue.From(v);
    }
    public static implicit operator UnityEngine.Color(BoxedValue v)
    {
        return v.Get<UnityEngine.Color>();
    }
    public static implicit operator BoxedValue(UnityEngine.Color32 v)
    {
        return BoxedValue.From(v);
    }
    public static implicit operator UnityEngine.Color32(BoxedValue v)
    {
        return v.Get<UnityEngine.Color32>();
    }

    public static implicit operator BoxedValue(Type v)
    {
        return BoxedValue.From(v);
    }
    public static implicit operator Type(BoxedValue v)
    {
        return v.ObjectVal as Type;
    }
    public static implicit operator BoxedValue(UnityEngine.Object v)
    {
        return BoxedValue.From((object)v);
    }
    public static implicit operator UnityEngine.Object(BoxedValue v)
    {
        return v.ObjectVal as UnityEngine.Object;
    }
    public static implicit operator BoxedValue(UnityEngine.GameObject v)
    {
        return BoxedValue.From((object)v);
    }
    public static implicit operator UnityEngine.GameObject(BoxedValue v)
    {
        return v.ObjectVal as UnityEngine.GameObject;
    }
    public static implicit operator BoxedValue(UnityEngine.Component v)
    {
        return BoxedValue.From((object)v);
    }
    public static implicit operator UnityEngine.Component(BoxedValue v)
    {
        return v.ObjectVal as UnityEngine.Component;
    }
    public static implicit operator BoxedValue(UnityEngine.Transform v)
    {
        return BoxedValue.From((object)v);
    }
    public static implicit operator UnityEngine.Transform(BoxedValue v)
    {
        return v.ObjectVal as UnityEngine.Transform;
    }
    public static implicit operator BoxedValue(UnityEngine.Component[] v)
    {
        return BoxedValue.From(v);
    }
    public static implicit operator UnityEngine.Component[](BoxedValue v)
    {
        return v.ObjectVal as UnityEngine.Component[];
    }
    public static implicit operator BoxedValue(List<UnityEngine.Component> v)
    {
        return BoxedValue.From(v);
    }
    public static implicit operator List<UnityEngine.Component>(BoxedValue v)
    {
        return v.ObjectVal as List<UnityEngine.Component>;
    }
    public static implicit operator BoxedValue(ObjList v)
    {
        return BoxedValue.From(v);
    }
    public static implicit operator ObjList(BoxedValue v)
    {
        return v.ObjectVal as ObjList;
    }
    public static implicit operator BoxedValue(ArrayList v)
    {
        return BoxedValue.From(v);
    }
    public static implicit operator ArrayList(BoxedValue v)
    {
        return v.ObjectVal as ArrayList;
    }

    public string GetTypeName()
    {
        switch (Type) {
            case c_ObjectType:
                return "object";
            case c_StringType:
                return "string";
            case c_BoolType:
                return "bool";
            case c_CharType:
                return "char";
            case c_SByteType:
                return "sbyte";
            case c_ShortType:
                return "short";
            case c_IntType:
                return "int";
            case c_LongType:
                return "long";
            case c_ByteType:
                return "byte";
            case c_UShortType:
                return "ushort";
            case c_UIntType:
                return "uint";
            case c_ULongType:
                return "ulong";
            case c_FloatType:
                return "float";
            case c_DoubleType:
                return "double";
            case c_DecimalType:
                return "decimal";
            case c_Vector2Type:
                return "Vector2";
            case c_Vector3Type:
                return "Vector3";
            case c_Vector4Type:
                return "Vector4";
            case c_QuaternionType:
                return "Quaternion";
            case c_ColorType:
                return "Color";
            case c_Color32Type:
                return "Color32";
            default:
                return "Unknown";
        }
    }

    public bool IsNullObject
    {
        get { return Type == c_ObjectType && ObjectVal == null; }
    }
    public bool IsNullOrEmptyString
    {
        get { return Type == c_StringType && string.IsNullOrEmpty(StringVal); }
    }
    public bool IsObject
    {
        get {
            return Type == c_ObjectType;
        }
    }
    public bool IsString
    {
        get {
            return Type == c_StringType;
        }
    }
    public bool IsBoolean
    {
        get {
            return Type == c_BoolType;
        }
    }
    public bool IsChar
    {
        get {
            return Type == c_CharType;
        }
    }
    public bool IsInteger
    {
        get {
            switch (Type) {
                case c_SByteType:
                case c_ShortType:
                case c_IntType:
                case c_LongType:
                case c_ByteType:
                case c_UShortType:
                case c_UIntType:
                case c_ULongType:
                    return true;
                default:
                    return false;
            }
        }
    }
    public bool IsFloat
    {
        get {
            return Type == c_FloatType || Type == c_DoubleType || Type == c_DecimalType;
        }
    }
    public string AsString
    {
        get {
            return IsString ? StringVal : (IsObject ? ObjectVal as string : null);
        }
    }
    public T As<T>() where T : class
    {
        return IsObject || IsString ? ObjectVal as T : null;
    }
    public object As(Type t)
    {
        if (null == ObjectVal) {
            return null;
        }
        else if (IsObject || IsString) {
            Type st = ObjectVal.GetType();
            if (t.IsAssignableFrom(st) || st.IsSubclassOf(t))
                return ObjectVal;
            else
                return null;
        }
        else {
            return null;
        }
    }

    public void SetNullObject()
    {
        Type = c_ObjectType;
        ObjectVal = null;
    }
    public void SetNullString()
    {
        Type = c_StringType;
        StringVal = null;
    }
    public void SetEmptyString()
    {
        Type = c_StringType;
        StringVal = string.Empty;
    }
    public void Set<T>(T v)
    {
        var t = v != null ? v.GetType() : typeof(T);
        Set<T>(t, v);
    }
    public void Set(Type t, object v)
    {
        Set<object>(t, v);
    }
    public T Get<T>()
    {
        var t = typeof(T);
        return Get<T>(t);
    }
    public object Get(Type t)
    {
        return Get<object>(t);
    }
    //供lua或防止隐式转换出问题时使用
    public void SetBool(bool v)
    {
        Set(v);
    }
    public void SetNumber(double v)
    {
        Set(v);
    }
    public void SetString(string v)
    {
        Set(v);
    }
    public void SetObject(object v)
    {
        Set(v);
    }
    public bool GetBool()
    {
        return Get<bool>();
    }
    public double GetNumber()
    {
        return Get<double>();
    }
    public string GetString()
    {
        return Get<string>();
    }
    public object GetObject()
    {
        return Get<object>();
    }

    public void CopyFrom(BoxedValue other)
    {
        Type = other.Type;
        switch (Type) {
            case c_ObjectType:
                ObjectVal = other.ObjectVal;
                break;
            case c_StringType:
                StringVal = other.StringVal;
                break;
            case c_BoolType:
                Union.BoolVal = other.Union.BoolVal;
                break;
            case c_CharType:
                Union.CharVal = other.Union.CharVal;
                break;
            case c_SByteType:
                Union.SByteVal = other.Union.SByteVal;
                break;
            case c_ShortType:
                Union.ShortVal = other.Union.ShortVal;
                break;
            case c_IntType:
                Union.IntVal = other.Union.IntVal;
                break;
            case c_LongType:
                Union.LongVal = other.Union.LongVal;
                break;
            case c_ByteType:
                Union.ByteVal = other.Union.ByteVal;
                break;
            case c_UShortType:
                Union.UShortVal = other.Union.UShortVal;
                break;
            case c_UIntType:
                Union.UIntVal = other.Union.UIntVal;
                break;
            case c_ULongType:
                Union.ULongVal = other.Union.ULongVal;
                break;
            case c_FloatType:
                Union.FloatVal = other.Union.FloatVal;
                break;
            case c_DoubleType:
                Union.DoubleVal = other.Union.DoubleVal;
                break;
            case c_DecimalType:
                Union.DecimalVal = other.Union.DecimalVal;
                break;
            case c_Vector2Type:
                Union.Vector2Val = other.Union.Vector2Val;
                break;
            case c_Vector3Type:
                Union.Vector3Val = other.Union.Vector3Val;
                break;
            case c_Vector4Type:
                Union.Vector4Val = other.Union.Vector4Val;
                break;
            case c_QuaternionType:
                Union.QuaternionVal = other.Union.QuaternionVal;
                break;
            case c_ColorType:
                Union.ColorVal = other.Union.ColorVal;
                break;
            case c_Color32Type:
                Union.Color32Val = other.Union.Color32Val;
                break;
        }
    }
    public override string ToString()
    {
        switch (Type) {
            case c_ObjectType:
                return null != ObjectVal ? ObjectVal.ToString() : string.Empty;
            case c_StringType:
                return null != StringVal ? StringVal : string.Empty;
            case c_BoolType:
                return Union.BoolVal.ToString();
            case c_CharType:
                return Union.CharVal.ToString();
            case c_SByteType:
                return Union.SByteVal.ToString();
            case c_ShortType:
                return Union.ShortVal.ToString();
            case c_IntType:
                return Union.IntVal.ToString();
            case c_LongType:
                return Union.LongVal.ToString();
            case c_ByteType:
                return Union.ByteVal.ToString();
            case c_UShortType:
                return Union.UShortVal.ToString();
            case c_UIntType:
                return Union.UIntVal.ToString();
            case c_ULongType:
                return Union.ULongVal.ToString();
            case c_FloatType:
                return Union.FloatVal.ToString();
            case c_DoubleType:
                return Union.DoubleVal.ToString();
            case c_DecimalType:
                return Union.DecimalVal.ToString();
            case c_Vector2Type:
                return Union.Vector2Val.ToString();
            case c_Vector3Type:
                return Union.Vector3Val.ToString();
            case c_Vector4Type:
                return Union.Vector4Val.ToString();
            case c_QuaternionType:
                return Union.QuaternionVal.ToString();
            case c_ColorType:
                return Union.ColorVal.ToString();
            case c_Color32Type:
                return Union.Color32Val.ToString();
        }
        return string.Empty;
    }

    private void Set<T>(Type t, T v)
    {
        if (typeof(T) == typeof(object)) {

            if (t == typeof(BoxedValue)) {
                var cv = (BoxedValue)(object)v;
                CopyFrom(cv);
            }
            else if (t == typeof(bool)) {
                var cv = (bool)(object)v;
                Type = c_BoolType;
                Union.BoolVal = cv;
            }
            else if (t == typeof(char)) {
                var cv = (char)(object)v;
                Type = c_CharType;
                Union.CharVal = cv;
            }
            else if (t == typeof(sbyte)) {
                var cv = (sbyte)(object)v;
                Type = c_SByteType;
                Union.SByteVal = cv;
            }
            else if (t == typeof(short)) {
                var cv = (short)(object)v;
                Type = c_ShortType;
                Union.ShortVal = cv;
            }
            else if (t == typeof(int)) {
                var cv = (int)(object)v;
                Type = c_IntType;
                Union.IntVal = cv;
            }
            else if (t == typeof(long)) {
                var cv = (long)(object)v;
                Type = c_LongType;
                Union.LongVal = cv;
            }
            else if (t == typeof(byte)) {
                var cv = (byte)(object)v;
                Type = c_ByteType;
                Union.ByteVal = cv;
            }
            else if (t == typeof(ushort)) {
                var cv = (ushort)(object)v;
                Type = c_UShortType;
                Union.UShortVal = cv;
            }
            else if (t == typeof(uint)) {
                var cv = (uint)(object)v;
                Type = c_UIntType;
                Union.UIntVal = cv;
            }
            else if (t == typeof(ulong)) {
                var cv = (ulong)(object)v;
                Type = c_ULongType;
                Union.ULongVal = cv;
            }
            else if (t == typeof(float)) {
                var cv = (float)(object)v;
                Type = c_FloatType;
                Union.FloatVal = cv;
            }
            else if (t == typeof(double)) {
                var cv = (double)(object)v;
                Type = c_DoubleType;
                Union.DoubleVal = cv;
            }
            else if (t == typeof(decimal)) {
                var cv = (decimal)(object)v;
                Type = c_DecimalType;
                Union.DecimalVal = cv;
            }
            else if (t == typeof(UnityEngine.Vector2)) {
                var cv = (UnityEngine.Vector2)(object)v;
                Type = c_Vector2Type;
                Union.Vector2Val = cv;
            }
            else if (t == typeof(UnityEngine.Vector3)) {
                var cv = (UnityEngine.Vector3)(object)v;
                Type = c_Vector3Type;
                Union.Vector3Val = cv;
            }
            else if (t == typeof(UnityEngine.Vector4)) {
                var cv = (UnityEngine.Vector4)(object)v;
                Type = c_Vector4Type;
                Union.Vector4Val = cv;
            }
            else if (t == typeof(UnityEngine.Quaternion)) {
                var cv = (UnityEngine.Quaternion)(object)v;
                Type = c_QuaternionType;
                Union.QuaternionVal = cv;
            }
            else if (t == typeof(UnityEngine.Color)) {
                var cv = (UnityEngine.Color)(object)v;
                Type = c_ColorType;
                Union.ColorVal = cv;
            }
            else if (t == typeof(UnityEngine.Color32)) {
                var cv = (UnityEngine.Color32)(object)v;
                Type = c_Color32Type;
                Union.Color32Val = cv;
            }
            else if (t == typeof(string)) {
                var cv = (string)(object)v;
                Type = c_StringType;
                ObjectVal = cv;
            }
            else {
                object vObj = v;
                Type = c_ObjectType;
                ObjectVal = vObj;
            }
        }
        else {
            if (t == typeof(BoxedValue)) {
                var cv = GenericValueConverter.ToBoxedValue(v);
                CopyFrom(cv);
            }
            else if (t == typeof(bool)) {
                var cv = GenericValueConverter.ToBool(v);
                Type = c_BoolType;
                Union.BoolVal = cv;
            }
            else if (t == typeof(char)) {
                var cv = GenericValueConverter.ToChar(v);
                Type = c_CharType;
                Union.CharVal = cv;
            }
            else if (t == typeof(sbyte)) {
                var cv = GenericValueConverter.ToSByte(v);
                Type = c_SByteType;
                Union.SByteVal = cv;
            }
            else if (t == typeof(short)) {
                var cv = GenericValueConverter.ToShort(v);
                Type = c_ShortType;
                Union.ShortVal = cv;
            }
            else if (t == typeof(int)) {
                var cv = GenericValueConverter.ToInt(v);
                Type = c_IntType;
                Union.IntVal = cv;
            }
            else if (t == typeof(long)) {
                var cv = GenericValueConverter.ToLong(v);
                Type = c_LongType;
                Union.LongVal = cv;
            }
            else if (t == typeof(byte)) {
                var cv = GenericValueConverter.ToByte(v);
                Type = c_ByteType;
                Union.ByteVal = cv;
            }
            else if (t == typeof(ushort)) {
                var cv = GenericValueConverter.ToUShort(v);
                Type = c_UShortType;
                Union.UShortVal = cv;
            }
            else if (t == typeof(uint)) {
                var cv = GenericValueConverter.ToUInt(v);
                Type = c_UIntType;
                Union.UIntVal = cv;
            }
            else if (t == typeof(ulong)) {
                var cv = GenericValueConverter.ToULong(v);
                Type = c_ULongType;
                Union.ULongVal = cv;
            }
            else if (t == typeof(float)) {
                var cv = GenericValueConverter.ToFloat(v);
                Type = c_FloatType;
                Union.FloatVal = cv;
            }
            else if (t == typeof(double)) {
                var cv = GenericValueConverter.ToDouble(v);
                Type = c_DoubleType;
                Union.DoubleVal = cv;
            }
            else if (t == typeof(decimal)) {
                var cv = GenericValueConverter.ToDecimal(v);
                Type = c_DecimalType;
                Union.DecimalVal = cv;
            }
            else if (t == typeof(UnityEngine.Vector2)) {
                var cv = GenericValueConverter.ToVector2(v);
                Type = c_Vector2Type;
                Union.Vector2Val = cv;
            }
            else if (t == typeof(UnityEngine.Vector3)) {
                var cv = GenericValueConverter.ToVector3(v);
                Type = c_Vector3Type;
                Union.Vector3Val = cv;
            }
            else if (t == typeof(UnityEngine.Vector4)) {
                var cv = GenericValueConverter.ToVector4(v);
                Type = c_Vector4Type;
                Union.Vector4Val = cv;
            }
            else if (t == typeof(UnityEngine.Quaternion)) {
                var cv = GenericValueConverter.ToQuaternion(v);
                Type = c_QuaternionType;
                Union.QuaternionVal = cv;
            }
            else if (t == typeof(UnityEngine.Color)) {
                var cv = GenericValueConverter.ToColor(v);
                Type = c_ColorType;
                Union.ColorVal = cv;
            }
            else if (t == typeof(UnityEngine.Color32)) {
                var cv = GenericValueConverter.ToColor32(v);
                Type = c_Color32Type;
                Union.Color32Val = cv;
            }
            else if (t == typeof(string)) {
                var cv = GenericValueConverter.ToString(v);
                Type = c_StringType;
                ObjectVal = cv;
            }
            else {
                object vObj = v;
                Type = c_ObjectType;
                ObjectVal = vObj;
            }
        }
    }
    private T Get<T>(Type t)
    {
        if (typeof(T) == typeof(object)) {
            var obj = ToObject();
            return GenericValueConverter.CastTo<T>(obj);
        }
        else {
            if (t == typeof(BoxedValue)) {
                return GenericValueConverter.From<T>(this);
            }
            else if (t == typeof(bool) && Type == c_BoolType) {
                return GenericValueConverter.From<T>(Union.BoolVal);
            }
            else if (t == typeof(char) && Type == c_CharType) {
                return GenericValueConverter.From<T>(Union.CharVal);
            }
            else if (t == typeof(sbyte) && Type == c_SByteType) {
                return GenericValueConverter.From<T>(Union.SByteVal);
            }
            else if (t == typeof(short) && Type == c_ShortType) {
                return GenericValueConverter.From<T>(Union.ShortVal);
            }
            else if (t == typeof(int) && Type == c_IntType) {
                return GenericValueConverter.From<T>(Union.IntVal);
            }
            else if (t == typeof(long) && Type == c_LongType) {
                return GenericValueConverter.From<T>(Union.LongVal);
            }
            else if (t == typeof(byte) && Type == c_ByteType) {
                return GenericValueConverter.From<T>(Union.ByteVal);
            }
            else if (t == typeof(ushort) && Type == c_UShortType) {
                return GenericValueConverter.From<T>(Union.UShortVal);
            }
            else if (t == typeof(uint) && Type == c_UIntType) {
                return GenericValueConverter.From<T>(Union.UIntVal);
            }
            else if (t == typeof(ulong) && Type == c_ULongType) {
                return GenericValueConverter.From<T>(Union.ULongVal);
            }
            else if (t == typeof(float) && Type == c_FloatType) {
                return GenericValueConverter.From<T>(Union.FloatVal);
            }
            else if (t == typeof(double) && Type == c_DoubleType) {
                return GenericValueConverter.From<T>(Union.DoubleVal);
            }
            else if (t == typeof(decimal) && Type == c_DecimalType) {
                return GenericValueConverter.From<T>(Union.DecimalVal);
            }
            else if (t == typeof(UnityEngine.Vector2) && Type == c_Vector2Type) {
                return GenericValueConverter.From<T>(Union.Vector2Val);
            }
            else if (t == typeof(UnityEngine.Vector3) && Type == c_Vector3Type) {
                return GenericValueConverter.From<T>(Union.Vector3Val);
            }
            else if (t == typeof(UnityEngine.Vector4) && Type == c_Vector4Type) {
                return GenericValueConverter.From<T>(Union.Vector4Val);
            }
            else if (t == typeof(UnityEngine.Quaternion) && Type == c_QuaternionType) {
                return GenericValueConverter.From<T>(Union.QuaternionVal);
            }
            else if (t == typeof(UnityEngine.Color) && Type == c_ColorType) {
                return GenericValueConverter.From<T>(Union.ColorVal);
            }
            else if (t == typeof(UnityEngine.Color32) && Type == c_Color32Type) {
                return GenericValueConverter.From<T>(Union.Color32Val);
            }
            else if (t == typeof(bool)) {
                long v = ToLong();
                return GenericValueConverter.From<T>(v != 0);
            }
            else if (t == typeof(char)) {
                long v = ToLong();
                return GenericValueConverter.From<T>((char)v);
            }
            else if (t == typeof(sbyte)) {
                long v = ToLong();
                return GenericValueConverter.From<T>((sbyte)v);
            }
            else if (t == typeof(short)) {
                long v = ToLong();
                return GenericValueConverter.From<T>((short)v);
            }
            else if (t == typeof(int)) {
                long v = ToLong();
                return GenericValueConverter.From<T>((int)v);
            }
            else if (t == typeof(long)) {
                long v = ToLong();
                return GenericValueConverter.From<T>(v);
            }
            else if (t == typeof(byte)) {
                long v = ToLong();
                return GenericValueConverter.From<T>((byte)v);
            }
            else if (t == typeof(ushort)) {
                long v = ToLong();
                return GenericValueConverter.From<T>((ushort)v);
            }
            else if (t == typeof(uint)) {
                long v = ToLong();
                return GenericValueConverter.From<T>((uint)v);
            }
            else if (t == typeof(ulong)) {
                long v = ToLong();
                return GenericValueConverter.From<T>((ulong)v);
            }
            else if (t == typeof(float)) {
                double v = ToDouble();
                return GenericValueConverter.From<T>((float)v);
            }
            else if (t == typeof(double)) {
                double v = ToDouble();
                return GenericValueConverter.From<T>(v);
            }
            else if (t == typeof(decimal)) {
                double v = ToDouble();
                return GenericValueConverter.From<T>((decimal)v);
            }
            else if (t == typeof(string) && Type == c_StringType) {
                return GenericValueConverter.From<T>(StringVal);
            }
            else if (t == typeof(object) && Type == c_ObjectType) {
                return GenericValueConverter.From<T>(ObjectVal);
            }
            else if (t == typeof(string)) {
                var str = ToString();
                return GenericValueConverter.From<T>(str);
            }
            else if (t == typeof(object)) {
                var obj = ToObject();
                return GenericValueConverter.From<T>(obj);
            }
            else {
                var obj = ToObject();
                return GenericValueConverter.CastTo<T>(obj);
            }
        }
    }
    private object ToObject()
    {
        switch (Type) {
            case c_ObjectType:
            case c_StringType:
                return ObjectVal;
            case c_BoolType:
                return Union.BoolVal;
            case c_CharType:
                return Union.CharVal;
            case c_SByteType:
                return Union.SByteVal;
            case c_ShortType:
                return Union.ShortVal;
            case c_IntType:
                return Union.IntVal;
            case c_LongType:
                return Union.LongVal;
            case c_ByteType:
                return Union.ByteVal;
            case c_UShortType:
                return Union.UShortVal;
            case c_UIntType:
                return Union.UIntVal;
            case c_ULongType:
                return Union.ULongVal;
            case c_FloatType:
                return Union.FloatVal;
            case c_DoubleType:
                return Union.DoubleVal;
            case c_DecimalType:
                return Union.DecimalVal;
            case c_Vector2Type:
                return Union.Vector2Val;
            case c_Vector3Type:
                return Union.Vector3Val;
            case c_Vector4Type:
                return Union.Vector4Val;
            case c_QuaternionType:
                return Union.QuaternionVal;
            case c_ColorType:
                return Union.ColorVal;
            case c_Color32Type:
                return Union.Color32Val;
        }
        return null;
    }
    private bool ToBool()
    {
        if (Type == c_BoolType)
            return Union.BoolVal;
        else
            return ToLong() != 0;
    }
    private char ToChar()
    {
        if (Type == c_CharType)
            return Union.CharVal;
        else
            return (char)(ulong)ToLong();
    }
    private long ToLong()
    {
        long v = 0;
        switch (Type) {
            case c_BoolType:
                return Union.BoolVal ? 1 : 0;
            case c_CharType:
                return Union.CharVal;
            case c_SByteType:
                return Union.SByteVal;
            case c_ShortType:
                return Union.ShortVal;
            case c_IntType:
                return Union.IntVal;
            case c_LongType:
                return Union.LongVal;
            case c_ByteType:
                return Union.ByteVal;
            case c_UShortType:
                return Union.UShortVal;
            case c_UIntType:
                return Union.UIntVal;
            case c_ULongType:
                return (long)Union.ULongVal;
            case c_FloatType:
                return (long)Union.FloatVal;
            case c_DoubleType:
                return (long)Union.DoubleVal;
            case c_DecimalType:
                return (long)Union.DecimalVal;
            case c_StringType:
                if (null != StringVal) {
                    long.TryParse(StringVal, out v);
                }
                return v;
            case c_ObjectType:
                if (null != ObjectVal) {
                    v = GenericValueConverter.CastTo<long>(ObjectVal);
                }
                return v;
        }
        return v;
    }
    private double ToDouble()
    {
        double v = 0;
        switch (Type) {
            case c_BoolType:
                return Union.BoolVal ? 1 : 0;
            case c_CharType:
                return Union.CharVal;
            case c_SByteType:
                return Union.SByteVal;
            case c_ShortType:
                return Union.ShortVal;
            case c_IntType:
                return Union.IntVal;
            case c_LongType:
                return Union.LongVal;
            case c_ByteType:
                return Union.ByteVal;
            case c_UShortType:
                return Union.UShortVal;
            case c_UIntType:
                return Union.UIntVal;
            case c_ULongType:
                return Union.ULongVal;
            case c_FloatType:
                return Union.FloatVal;
            case c_DoubleType:
                return Union.DoubleVal;
            case c_DecimalType:
                return (double)Union.DecimalVal;
            case c_StringType:
                if (null != StringVal) {
                    double.TryParse(StringVal, out v);
                }
                return v;
            case c_ObjectType:
                if (null != ObjectVal) {
                    v = GenericValueConverter.CastTo<double>(ObjectVal);
                }
                return v;
        }
        return v;
    }

    public static BoxedValue From<T>(T v)
    {
        BoxedValue bv = new BoxedValue();
        bv.Set(v);
        return bv;
    }
    public static BoxedValue From(Type t, object o)
    {
        BoxedValue bv = new BoxedValue();
        bv.Set(o);
        return bv;
    }
    //供lua或防止隐式转换出问题时使用
    public static BoxedValue FromBool(bool v)
    {
        BoxedValue bv = new BoxedValue();
        bv.Set(v);
        return bv;
    }
    public static BoxedValue FromNumber(double v)
    {
        BoxedValue bv = new BoxedValue();
        bv.Set(v);
        return bv;
    }
    public static BoxedValue FromString(string v)
    {
        BoxedValue bv = new BoxedValue();
        bv.Set(v);
        return bv;
    }
    public static BoxedValue FromObject(object v)
    {
        BoxedValue bv = new BoxedValue();
        bv.Set(v);
        return bv;
    }

    public static BoxedValue NullObject
    {
        get { return s_NullObject; }
    }
    public static BoxedValue EmptyString
    {
        get { return s_EmptyString; }
    }
    private static BoxedValue s_NullObject = BoxedValue.FromObject(null);
    private static BoxedValue s_EmptyString = BoxedValue.FromString(string.Empty);
}

public class BoxedValueList : List<BoxedValue>
{
    public BoxedValueList() { }
    public BoxedValueList(int capacity) : base(capacity) { }
    public BoxedValueList(IEnumerable<BoxedValue> coll) : base(coll) { }
    public void AddBool(bool v)
    {
        Add(BoxedValue.FromBool(v));
    }
    public void AddNumber(double v)
    {
        Add(BoxedValue.FromNumber(v));
    }
    public void AddString(string v)
    {
        Add(BoxedValue.FromString(v));
    }
    public void AddObject(object v)
    {
        Add(BoxedValue.FromObject(v));
    }
}

public class StrBoxedValueDict : Dictionary<string, BoxedValue>
{
    public StrBoxedValueDict() { }
    public StrBoxedValueDict(int capacity) : base(capacity) { }
    public StrBoxedValueDict(IDictionary<string, BoxedValue> dict) : base(dict) { }
    public void AddBool(string k, bool v)
    {
        Add(k, BoxedValue.FromBool(v));
    }
    public void AddNumber(string k, double v)
    {
        Add(k, BoxedValue.FromNumber(v));
    }
    public void AddString(string k, string v)
    {
        Add(k, BoxedValue.FromString(v));
    }
    public void AddObject(string k, object v)
    {
        Add(k, BoxedValue.FromObject(v));
    }
}