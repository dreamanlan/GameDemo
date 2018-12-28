using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryFollow : MonoBehaviour
{
    public GameObject Target;

    internal void LateUpdate()
    {
        if (null != Target) {
            var t = Target.transform;
            transform.position = t.position;
            transform.localRotation = t.localRotation;
            transform.localScale = t.localScale;
        }
    }

    private void FollowTarget(GameObject t)
    {
        Target = t;
    }

    private void StopFollow()
    {
        Target = null;
    }
}
