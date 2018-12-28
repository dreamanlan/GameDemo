using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenerSwitch : MonoBehaviour
{
    public void EnableTweener(int index)
    {
        var tweeners = gameObject.GetComponentsInChildren<uTools.Tweener>();
        if (null != tweeners && index >= 0 && index < tweeners.Length) {
            tweeners[index].ResetToBeginning();
            tweeners[index].enabled = true;
        }
    }
    public void DisableTweener(int index)
    {
        var tweeners = gameObject.GetComponentsInChildren<uTools.Tweener>();
        if (null != tweeners && index >= 0 && index < tweeners.Length) {
            tweeners[index].enabled = false;
        }
    }
}
