using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryLookat : MonoBehaviour
{
    public GameObject Target;

    internal void LateUpdate()
    {
        if (null != Target) {
            transform.LookAt(Target.transform, Vector3.up);
        }
    }

    private void Lookat(GameObject t)
    {
        Target = t;
    }
}
