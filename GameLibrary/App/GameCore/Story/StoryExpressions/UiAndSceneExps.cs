using System;
using System.Collections.Generic;
using UnityEngine;
using StoryScript;
using StoryScript.DslExpression;

namespace GameLibrary.Story.StoryExpressions
{
    /// <summary>
    /// loadui(name, prefab, dslfile[, dont_destroy_old]) - load a UI prefab
    /// </summary>
    internal sealed class LoadUiExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 3)
                throw new Exception("Expected: loadui(name, prefab, dslfile[, dont_destroy_old])");

            string name = operands[0].ToString();
            string prefab = operands[1].ToString();
            string dslfile = operands[2].ToString();
            int dontDestroyOld = operands.Count > 3 ? operands[3].GetInt() : 0;

            ClientStorySystem.Instance.LoadStoryFromFile(name, dslfile);
            GameObject asset = ResourceSystem.Instance.GetSharedResource(prefab) as GameObject;
            if (null != asset)
            {
                if (dontDestroyOld <= 0)
                {
                    var old = GameObject.Find(name);
                    if (null != old)
                    {
                        old.transform.SetParent(null);
                        Utility.DestroyObject(old);
                    }
                }
                var rootUi = GameObject.Find("Canvas");
                var uiObj = Utility.AttachUiAsset(rootUi, asset, name);
                SceneSystem.Instance.LoadedUiPrefabs.Add(prefab);
                if (null != uiObj)
                {
                    if (!string.IsNullOrEmpty(dslfile))
                    {
                        GameLibrary.Story.UiStoryInitializer initer = uiObj.GetComponent<GameLibrary.Story.UiStoryInitializer>();
                        if (null == initer)
                        {
                            initer = uiObj.AddComponent<GameLibrary.Story.UiStoryInitializer>();
                        }
                        if (null != initer)
                        {
                            initer.WindowName = name;
                            initer.Init();
                        }
                    }
                }
            }
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// changescene(target_scene) - change to a different scene
    /// </summary>
    internal sealed class ChangeSceneExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: changescene(target_scene)");

            string targetScene = operands[0].ToString();
            SceneSystem.Instance.ChangeScene(targetScene);
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// quit() - quit the application
    /// </summary>
    internal sealed class QuitExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            UnityEngine.Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// openurl(url) - open a URL
    /// </summary>
    internal sealed class OpenUrlExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: openurl(url)");

            string url = operands[0].ToString();
            UnityEngine.Application.OpenURL(url);
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// highlightprompt(info) - show a highlight prompt
    /// </summary>
    internal sealed class HighlightPromptExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: highlightprompt(info)");

            string info = operands[0].ToString();
            SceneSystem.Instance.HighlightPrompt(info);
            return BoxedValue.NullObject;
        }
    }
}
