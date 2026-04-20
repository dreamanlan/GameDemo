using UnityEngine;
using System.Collections.Generic;
using GameLibrary;
using GameLibrary.Story;
using StoryScript;

namespace GameLibrary.Story
{
    public class UiStoryInitializer : MonoBehaviour
    {
        public string WindowName = string.Empty;
        public void Init()
        {
            StoryInstance instance = ClientStorySystem.Instance.GetStory("main", WindowName);
            if (null != instance) {
                instance.InstanceVariables.Clear();
                instance.InstanceVariables.Add("@window", gameObject);
                ClientStorySystem.Instance.StartStory("main", WindowName);
            }
        }
    }
}
