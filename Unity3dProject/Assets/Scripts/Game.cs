using System.Collections;
using System.Collections.Generic;
using GameLibrary;
using GameLibrary.Story;
using System.IO;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public AudioClip[] Musics;
    public EventSystem eventSystem;
    private bool m_IsInited;

    void Awake()
    {
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(eventSystem.gameObject);
        Input.multiTouchEnabled = true;
        Application.runInBackground = true;

        InitFilePath();
        StartCoroutine(Startup());

        Physics.gravity = new Vector3(0, 9.8f, 0);
    }

    void OnDestroy()
    {
    }

    void Update()
    {
        if (!m_IsInited)
            return;
        GameControler.Instance.TickGame();
        
#if UNITY_EDITOR_WIN
        //Pause key
        if (Input.GetKeyDown(KeyCode.Pause)) {
            Debug.Break();
        }
#endif
    }

    void OnApplicationQuit()
    {
        if (!m_IsInited)
            return;
        GameControler.Instance.Release();
    }

    public static void InitFilePath()
    {
        string persistentDataPath = Application.persistentDataPath;
        string streamingAssetsPath = Application.streamingAssetsPath;
#if UNITY_EDITOR || UNITY_STANDALONE
		HomePath.CurHomePath = streamingAssetsPath;
#else
		HomePath.CurHomePath = streamingAssetsPath;
#endif
        Debug.Log("persistentDataPath=" + persistentDataPath);
        Debug.Log("streamingAssetsPath = " + streamingAssetsPath);
    }

    private IEnumerator Startup()
    {
        string suffix = string.Empty;
#if UNITY_STANDALONE_WIN
        {
            string[] args = System.Environment.GetCommandLineArgs();
            int suffixIndex = Array.FindIndex(args, item => item == "-suffix");
            if (suffixIndex != -1 && suffixIndex < args.Length - 1) {
                suffix = args[suffixIndex + 1];
            }
        }
#endif
        string dataPath = Application.dataPath;
        string tempPath = Application.temporaryCachePath;
        string logPath = Application.persistentDataPath;

#if DEVELOPMENT_BUILD
        GlobalVariables.Instance.IsDevelopment = true;
        StoryScript.StoryConfigManager.Instance.IsDevelopment = true;
#else
        GlobalVariables.Instance.IsDevelopment = false;
        StoryScript.StoryConfigManager.Instance.IsDevelopment = false;
#endif

#if UNITY_EDITOR
        GlobalVariables.Instance.IsDevice = false;
        StoryScript.StoryConfigManager.Instance.IsDevice = false;
#elif UNITY_ANDROID || UNITY_IOS
        GlobalVariables.Instance.IsEditor = false;
        GlobalVariables.Instance.IsDevice = true;
        StoryScript.StoryConfigManager.Instance.IsDevice = true;
#endif

#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        GameControler.Instance.InitLog(logPath, suffix);
#else
        if (Application.isEditor) {
            GameControler.Instance.InitLog(".", suffix);
        } else {
            GameControler.Instance.InitLog(dataPath, suffix);
        }
#endif
        GameControler.Instance.InitGame();
        yield return null;
        m_IsInited = true;
        yield break;
    }

    IEnumerator LoadScene(string sceneName)
    {
        yield return SceneManager.LoadSceneAsync("loading");
        yield return SceneManager.LoadSceneAsync(sceneName);
        SceneSystem.Instance.OnSceneLoaded(sceneName);
    }

    private void LogToConsole(string msg)
    {
        DebugConsole.Log(msg);
    }
    private void OnResetDsl()
    {
        Utility.EventSystem.Publish("gm_resetdsl", "gm");
    }
    private void OnExecScript(string script)
    {
        Utility.EventSystem.Publish("gm_execscript", "gm", script);
    }
    private void OnExecCommand(string command)
    {
        Utility.EventSystem.Publish("gm_execcommand", "gm", command);
    }
    private void CameraEnable(object[] args)
    {
        if (null != args[0] && null != args[1]) {
            string cameraName = args[0] as string;
            int enable = (int)args[1];

            if (null != cameraName) {
                CameraEnable(cameraName, enable != 0);
            }
        }
    }
    private void CameraEnable(string cameraNode, bool enable)
    {
        GameObject obj = GameObject.Find(cameraNode);
        if (null != obj) {
            var fading = obj.GetComponent<ScreenFading>();
            if (null != fading && !enable) {
                fading.enabled = false;
            }
            var storyCamera = obj.GetComponent<StoryCamera>();
            if (null != storyCamera && !enable) {
                storyCamera.enabled = false;
            }
            Camera camera = obj.GetComponent<Camera>();
            if (null != camera) {
                camera.enabled = enable;
            }
            var audioListener = obj.GetComponent<AudioListener>();
            if (null != audioListener) {
                audioListener.enabled = enable;
            }
            if (null != fading && enable) {
                fading.enabled = true;
            }
            if (null != storyCamera && enable) {
                storyCamera.enabled = true;
            }
        } else if (enable) {
            LogSystem.Error("Can't find camera node {0}", cameraNode);
        }
    }
    private void UiCameraEnable(object[] args)
    {
        if (null != args[0] && null != args[1]) {
            string cameraName = args[0] as string;
            int enable = (int)args[1];

            if (null != cameraName) {
                UiCameraEnable(cameraName, enable != 0);
            }
        }
    }
    private void UiCameraEnable(string cameraNode, bool enable)
    {
        GameObject obj = GameObject.Find(cameraNode);
        if (null != obj) {
            Camera camera = obj.GetComponent<Camera>();
            if (null != camera) {
                camera.enabled = enable;
            }
        } else if (enable) {
            LogSystem.Error("Can't find camera node {0}", cameraNode);
        }
    }
    private void PlayMusic(int index)
    {
        AudioSource audioSource = gameObject.GetComponent<AudioSource>();
        if (null != audioSource && null != Musics && index >= 0 && index < Musics.Length) {
            var clip = Musics[index];
            if (audioSource.isPlaying) {
                audioSource.Stop();
            }
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
    private void StopMusic()
    {
        AudioSource audioSource = gameObject.GetComponent<AudioSource>();
        if (null != audioSource) {
            audioSource.Stop();
        }
    }
}
