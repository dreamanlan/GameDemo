using UnityEngine;
using UnityEngine.Playables;
using System.Collections;
using GameLibrary;
using GameLibrary.Story;

public class StoryObject : MonoBehaviour
{
    public AudioClip[] Musics;

    public void SetLightVisible(int visible)
    {
        Light[] lights = gameObject.GetComponentsInChildren<Light>();
        for (int i = 0; i < lights.Length; ++i)
        {
            Light light = lights[i];
            if (visible == 0)
            {
                light.enabled = false;
            }
            else
            {
                light.enabled = true;
            }
        }
    }
    public void SetVisible(int visible)
    {
        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; ++i)
        {
            if (visible == 0)
            {
                renderers[i].enabled = false;
            }
            else
            {
                renderers[i].enabled = true;
            }
        }
    }
    public void PlayAnimation(object[] args)
    {
        if (args.Length < 2) return;
        string animName = (string)args[0];
        float speed = 1;
        string speedStr = args[1].ToString();
        if(!float.TryParse(speedStr, out speed))
        {
            LogSystem.Error("Try get speed with incorrect value type.");
        }
        Animator[] animators = gameObject.GetComponentsInChildren<Animator>(true);
        if (null != animators)
        {
            for (int i = 0; i < animators.Length; ++i)
            {
                var animator = animators[i];
                animator.Play(animName, -1, 0);
                animator.speed = speed;
            }
        }
    }
    public void PlayParticle(int arg)
    {
        bool enable = (int)arg != 0 ? true : false;
        ParticleSystem[] pss = gameObject.GetComponentsInChildren<ParticleSystem>();
        for (int i = 0; i < pss.Length; ++i)
        {
            if (enable)
            {
                pss[i].Stop(true);
                pss[i].Play(true);
            }
            else
            {
                pss[i].Stop(true);
            }
        }
    }
    public void EnableObstacle(int arg)
    {
        bool enable = (int)arg != 0 ? true : false;
        var obs = gameObject.GetComponentInChildren<UnityEngine.AI.NavMeshObstacle>();
        if (null != obs)
        {
            obs.enabled = enable;
        }
    }
    public void EnableGravity(int arg)
    {
        bool enable = (int)arg != 0 ? true : false;
        var rigidbody = gameObject.GetComponentInChildren<Rigidbody>();
        if (null != rigidbody) {
            rigidbody.useGravity = enable;
        }
    }
    public void PlaySound(int index)
    {
        AudioSource audioSource = gameObject.GetComponentInChildren<AudioSource>();
        if (null != audioSource && null != Musics && index >= 0 && index < Musics.Length) {
            var clip = Musics[index];
            if (audioSource.isPlaying) {
                audioSource.Stop();
            }
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
    public void StopSound()
    {
        AudioSource audioSource = gameObject.GetComponentInChildren<AudioSource>();
        if (null != audioSource)
        {
            audioSource.Stop();
        }
    }
    public void TweenUiPrefab(object[] args)
    {
        if (null != args && args.Length >= 3) {
            string name = args[0] as string;
            string prefab = args[1] as string;
            Vector3 offset = (Vector3)args[2];
            float duration = 0;
            if (args.Length >= 4) {
                duration = (float)System.Convert.ChangeType(args[3], typeof(float));
            }
            var prefabObj = ResourceSystem.Instance.GetSharedResource(prefab) as GameObject;
            if (null != prefabObj) {
                var canvas = GameObject.Find("Canvas");
                if (null != canvas) {
                    var obj = Utility.AttachUiAsset(canvas, prefabObj, name);
                    if (null != obj) {
                        var pos3d = gameObject.transform.TransformPoint(offset);
                        var canvasRect = canvas.transform as RectTransform;
                        var rectTrans = obj.transform as RectTransform;

                        var pos2d = Camera.main.WorldToViewportPoint(pos3d);
                        var screen2d = new Vector2(((pos2d.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)), ((pos2d.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)));

                        if (null != rectTrans) {
                            rectTrans.anchoredPosition = screen2d;
                            rectTrans.anchorMin = pos2d;
                            rectTrans.anchorMax = pos2d;
                        } else {
                            obj.transform.position = pos3d;
                        }
                        StartTweeners(obj, duration);
                    }
                }
            }
        }
    }
    public void TweenUiPrefabWithText(object[] args)
    {
        if (null != args && args.Length >= 5) {
            string name = args[0] as string;
            string prefab = args[1] as string;
            Vector3 offset = (Vector3)args[2];
            string path = args[3] as string;
            string txt = args[4] as string;
            float duration = 0;
            if (args.Length >= 6) {
                duration = (float)System.Convert.ChangeType(args[5], typeof(float));
            }
            var prefabObj = ResourceSystem.Instance.GetSharedResource(prefab) as GameObject;
            if (null != prefabObj) {
                var canvas = GameObject.Find("Canvas");
                if (null != canvas) {
                    var obj = Utility.AttachUiAsset(canvas, prefabObj, name);
                    if (null != obj) {
                        var pos3d = gameObject.transform.TransformPoint(offset);
                        var canvasRect = canvas.transform as RectTransform;
                        var rectTrans = obj.transform as RectTransform;

                        var pos2d = Camera.main.WorldToViewportPoint(pos3d);
                        var screen2d = new Vector2(((pos2d.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)), ((pos2d.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)));

                        if (null != rectTrans) {
                            rectTrans.anchoredPosition = screen2d;
                            rectTrans.anchorMin = pos2d;
                            rectTrans.anchorMax = pos2d;
                        } else {
                            obj.transform.position = pos3d;
                        }
                        var child = obj;
                        if (!string.IsNullOrEmpty(path)) {
                            child = Utility.FindChildObjectByPath(obj, path);
                        }
                        if (null != child) {
                            var text = child.GetComponent<UnityEngine.UI.Text>();
                            if (null != text) {
                                text.text = txt;
                            }
                        }
                        StartTweeners(obj, duration);
                    }
                }
            }
        }
    }
    internal void OnCollisionEnter(Collision collisionInfo)
    {
        var args = ClientStorySystem.Instance.NewBoxedValueList();
        var collider = gameObject.GetComponent<Collider>();
        args.Add(gameObject);
        args.Add(collider);
        args.Add(collisionInfo.gameObject);
        args.Add(collisionInfo.collider);
        var pt1 = gameObject.transform.position;
        args.Add(pt1);
        var pt2 = collisionInfo.gameObject.transform.position;
        args.Add(pt2);
        var time = Time.time;
        args.Add(time);
        ClientStorySystem.Instance.SendConcurrentMessage("on_collision_enter", args);
    }
    internal void OnCollisionExit(Collision collisionInfo)
    {
        var args = ClientStorySystem.Instance.NewBoxedValueList();
        var collider = gameObject.GetComponent<Collider>();
        args.Add(gameObject);
        args.Add(collider);
        args.Add(collisionInfo.gameObject);
        args.Add(collisionInfo.collider);
        var pt1 = gameObject.transform.position;
        args.Add(pt1);
        var pt2 = collisionInfo.gameObject.transform.position;
        args.Add(pt2);
        var time = Time.time;
        args.Add(time);
        ClientStorySystem.Instance.SendConcurrentMessage("on_collision_exit", args);
    }
    internal void OnTriggerEnter(Collider other)
    {
        var args = ClientStorySystem.Instance.NewBoxedValueList();
        var collider = gameObject.GetComponent<Collider>();
        args.Add(gameObject);
        args.Add(collider);
        args.Add(other.gameObject);
        args.Add(other);
        var pt1 = gameObject.transform.position;
        args.Add(pt1);
        var pt2 = other.transform.position;
        args.Add(pt2);
        var time = Time.time;
        args.Add(time);
        ClientStorySystem.Instance.SendConcurrentMessage("on_trigger_enter", args);
    }
    internal void OnTriggerExit(Collider other)
    {
        var args = ClientStorySystem.Instance.NewBoxedValueList();
        var collider = gameObject.GetComponent<Collider>();
        args.Add(gameObject);
        args.Add(collider);
        args.Add(other.gameObject);
        args.Add(other);
        var pt1 = gameObject.transform.position;
        args.Add(pt1);
        var pt2 = other.transform.position;
        args.Add(pt2);
        var time = Time.time;
        args.Add(time);
        ClientStorySystem.Instance.SendConcurrentMessage("on_trigger_exit", args);
    }

    private void StartTweeners(GameObject obj, float duration)
    {
        var tweeners = obj.GetComponentsInChildren<uTools.Tweener>();
        if (null != tweeners && tweeners.Length>0) {
            var evt = new UnityEngine.Events.UnityEvent();
            evt.AddListener(() => {
                obj.SetActive(false);
                obj.transform.SetParent(null);
                Utility.DestroyObject(obj);
            });
            bool first = true;
            for (int i = 0; i < tweeners.Length; ++i) {
                var tweener = tweeners[i];
                if (!tweener.enabled) {
                    if (first) {
                        first = false;
                        tweener.SetOnFinished(evt);
                    }
                    if (duration > Geometry.c_FloatPrecision) {
                        tweener.duration = duration;
                    }
                    tweener.ResetToBeginning();
                    tweener.enabled = true;
                }
            }
        }
    }
}
