using System;
using System.Collections.Generic;
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
