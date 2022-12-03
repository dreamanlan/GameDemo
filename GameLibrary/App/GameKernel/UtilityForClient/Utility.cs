using System;
using System.Threading;
using System.Text;
using System.IO;
using UnityEngine;
using GameLibrary;

public static partial class Utility
{
    public static PublishSubscribeSystem EventSystem
    {
        get
        {
            return s_EventSystem;
        }
    }

    [System.Diagnostics.Conditional("DEBUG")]
    public static void GfxLog(string format, params object[] args)
    {
        string msg = string.Format(format, args);
        SendMessageImpl("StartScript", "LogToConsole", msg, false);
#if DEBUG
        UnityEngine.Debug.LogWarning(msg);
#endif
    }
    [System.Diagnostics.Conditional("DEBUG")]
    public static void GfxErrorLog(string format, params object[] args)
    {
        string msg = string.Format(format, args);
        SendMessageImpl("StartScript", "LogToConsole", msg, false);
#if DEBUG
        UnityEngine.Debug.LogError(msg);
#endif
    }
    public static GameObject FindChildObject(GameObject fromGameObject, string withName)
    {
        Transform[] ts = fromGameObject.transform.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < ts.Length; ++i) {
            Transform t = ts[i];
            if (t.gameObject.name == withName) {
                return t.gameObject;
            }
        }
        return null;
    }
    public static Transform FindChildRecursive(Transform parent, string bonePath)
    {
        Transform t = parent.Find(bonePath);
        if (null != t) {
            return t;
        } else {
            int ct = parent.childCount;
            for (int i = 0; i < ct; ++i) {
                t = FindChildRecursive(parent.GetChild(i), bonePath);
                if (null != t) {
                    return t;
                }
            }
        }
        return null;
    }
    public static GameObject FindChildObjectByPath(GameObject gameObject, string name)
    {
        if (name.IndexOf('/') == -1) {
            Transform child = gameObject.transform.Find(name);
            if (null == child) {
                return null;
            }
            return child.gameObject;
        } else {
            string[] path = name.Split('/');
            Transform child = gameObject.transform;
            for (int i = 0; i < path.Length; i++) {
                child = child.Find(path[i]);
                if (null == child) {
                    return null;
                }
            }
            return child.gameObject;
        }
    }
    public static T FindComponentInChildren<T>(GameObject _gameObject, string _name)
    {
        if (_name.IndexOf('/') == -1) {
            Transform child = _gameObject.transform.Find(_name);
            if (child == null)
                return default(T);
            return child.GetComponent<T>();
        } else {
            string[] path = _name.Split('/');
            Transform child = _gameObject.transform;
            for (int i = 0; i < path.Length; i++) {
                child = child.Find(path[i]);
                if (child == null) {
                    return default(T);
                }
            }
            return child.GetComponent<T>();
        }
    }
    public static GameObject AttachUiAsset(GameObject targetObject, GameObject asset, string name)
    {
        GameObject result = null;
        RectTransform assetRect = (RectTransform)asset.transform;
        result = (GameObject)GameObject.Instantiate(asset);
        result.name = name;
        RectTransform rect = (RectTransform)result.transform;
        rect.SetParent(targetObject.transform, false);
        return result;
    }
    public static GameObject AttachUiObject(GameObject targetObject, GameObject uiObj, string name)
    {
        uiObj.name = name;
        RectTransform rect = (RectTransform)uiObj.transform;
        rect.SetParent(targetObject.transform, false);
        return uiObj;
    }
    public static bool VectorEquals(Vector3 p1, Vector3 p2)
    {
        return Mathf.Approximately(p1.x, p2.x) && Mathf.Approximately(p1.y, p2.y) && Mathf.Approximately(p1.z, p2.z);
    }
    public static Vector3 GetBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        t = Mathf.Clamp01(t);
        float num = 1f - t;
        return (Vector3)((((num * num) * p0) + (((2f * num) * t) * p1)) + ((t * t) * p2));
    }
    public static Vector3 GetBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        t = Mathf.Clamp01(t);
        float num = 1f - t;
        return (Vector3)(num * num * num * p0 + 3 * num * num * t * p1 + 3 * num * t * t * p2 + ((t * t * t) * p3));
    }
    public static void DrawGizmosCircle(Vector3 center, float radius, int step = 16)
    {
        for (int i = 1; i < step + 1; i++) {
            Vector3 pos0 = center + Quaternion.AngleAxis(360.0f * (i - 1) / step, Vector3.up) * Vector3.forward * radius;
            Vector3 pos1 = center + Quaternion.AngleAxis(360.0f * i / step, Vector3.up) * Vector3.forward * radius;
            Gizmos.DrawLine(pos0, pos1);
        }
    }
    public static void DrawGizmosArraw(Vector3 start, Vector3 end, float size = 0.5f)
    {
        Vector3 dir = (end - start).normalized;
        float length = (end - start).magnitude;
        Gizmos.DrawLine(start, end);
        Vector3 left = Quaternion.AngleAxis(45, Vector3.up) * (-dir);
        Gizmos.DrawLine(end, end + left * size);
        Vector3 right = Quaternion.AngleAxis(-45, Vector3.up) * (-dir);
        Gizmos.DrawLine(end, end + right * size);
    }
    public static void DestroyObject(UnityEngine.Object obj)
    {
        if (Application.isPlaying) {
            UnityEngine.Object.Destroy(obj);
        }
    }
    public static void DestroyObjectFull(UnityEngine.Object obj)
    {
        if (obj != null) {
            if (Application.isEditor && !Application.isPlaying) {
                UnityEngine.Object.DestroyImmediate(obj);
            } else {
                UnityEngine.Object.Destroy(obj);
            }
        }
    }
    
    public static byte[] LoadFileFromStreamingAssets(string file)
    {
        if (Application.platform == RuntimePlatform.Android) {
            AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject androidJavaActivity = null;
            AndroidJavaObject assetManager = null;
            AndroidJavaObject inputStream = null;
            if (androidJavaClass != null)
                androidJavaActivity = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
            if (androidJavaActivity != null)
                assetManager = androidJavaActivity.Call<AndroidJavaObject>("getAssets");
            if (assetManager != null)
                inputStream = assetManager.Call<AndroidJavaObject>("open", file);
            if (inputStream != null) {
                int available = inputStream.Call<int>("available");
                System.IntPtr buffer = AndroidJNI.NewSByteArray(available);
                System.IntPtr javaClass = AndroidJNI.FindClass("java/io/InputStream");
                System.IntPtr javaMethodID = AndroidJNIHelper.GetMethodID(javaClass, "read", "([B)I");
                int read = AndroidJNI.CallIntMethod(inputStream.GetRawObject(), javaMethodID,
                    new[] { new jvalue() { l = buffer } });
                sbyte[] sbytes = AndroidJNI.FromSByteArray(buffer);
                AndroidJNI.DeleteLocalRef(buffer);
                inputStream.Call("close");
                inputStream.Dispose();
                byte[] bytes = new byte[sbytes.Length];
                Array.Copy(bytes, sbytes, bytes.Length);
                return bytes;
            }
        } else {
            string assembly = Path.Combine(Application.streamingAssetsPath, file);
            if (File.Exists(assembly)) {
                var bytes = File.ReadAllBytes(assembly);
                return bytes;
            }
        }
        return null;
    }

    public static void SendScriptMessage(string msg, object arg)
    {
        SendMessage("StartScript", msg, arg, false);
    }
    public static void SendMessage(string objname, string msg, object arg)
    {
        SendMessage(objname, msg, arg, false);
    }
    public static void SendMessage(string objname, string msg, object arg, bool needReceiver)
    {
        SendMessageImpl(objname, msg, arg, needReceiver);
    }
    public static void SendMessageWithTag(string objtag, string msg, object arg)
    {
        SendMessageWithTag(objtag, msg, arg, false);
    }
    public static void SendMessageWithTag(string objtag, string msg, object arg, bool needReceiver)
    {
        SendMessageWithTagImpl(objtag, msg, arg, needReceiver);
    }
    public static Type GetType(string type)
    {
        Type ret = null;
        try {
            ret = Type.GetType("UnityEngine." + type + ", UnityEngine");
            if (null == ret) {
                ret = Type.GetType("UnityEngine.UI." + type + ", UnityEngine.UI");
            }
            if (null == ret) {
                ret = Type.GetType(type + ", Assembly-CSharp");
            }
            if (null == ret) {
                ret = Type.GetType(type);
            }
            if (null == ret) {
                LogSystem.Warn("null == Type.GetType({0})", type);
            }
        } catch (Exception ex) {
            LogSystem.Warn("Exception:{0}\n{1}", ex.Message, ex.StackTrace);
        }
        return ret;
    }

    private static void SendMessageImpl(string objname, string msg, object arg, bool needReceiver)
    {
        GameObject obj = GameObject.Find(objname);
        if (null != obj) {
            try {
                obj.SendMessage(msg, arg, needReceiver ? SendMessageOptions.RequireReceiver : SendMessageOptions.DontRequireReceiver);
                if (msg.CompareTo("LogToConsole") != 0)
                    LogSystem.Info("SendMessage {0} {1} {2} {3}", objname, msg, arg, needReceiver);
            } catch (Exception ex) {
                LogSystem.Error("SendMessage({0} {1} {2} {3}) Exception {4}\n{5}", objname, msg, arg, needReceiver, ex.Message, ex.StackTrace);
            }
        }
    }
    private static void SendMessageWithTagImpl(string objtag, string msg, object arg, bool needReceiver)
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag(objtag);
        if (null != objs) {
            for (int i = 0; i < objs.Length; i++) {
                try {
                    objs[i].SendMessage(msg, arg, needReceiver ? SendMessageOptions.RequireReceiver : SendMessageOptions.DontRequireReceiver);
                    if (msg.CompareTo("LogToConsole") != 0)
                        LogSystem.Info("SendMessageWithTag {0} {1} {2} {3}", objtag, msg, arg, needReceiver);
                } catch (Exception ex) {
                    LogSystem.Error("SendMessageWithTag({0} {1} {2} {3}) Exception {4}\n{5}", objtag, msg, arg, needReceiver, ex.Message, ex.StackTrace);
                }
            }
        }
    }

    private static PublishSubscribeSystem s_EventSystem = new PublishSubscribeSystem();
}
