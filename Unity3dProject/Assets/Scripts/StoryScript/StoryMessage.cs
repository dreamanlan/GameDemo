using UnityEngine;
using System.Collections;
using GameLibrary.Story;
using StoryScript;

public class StoryMessage : MonoBehaviour
{
    public string[] Args;
    public void SendStoryMessage(string msgId)
    {
        if (null != Args && Args.Length > 0) {
            BoxedValueList al = ClientStorySystem.Instance.NewBoxedValueList();
            al.Add(gameObject.name);
            foreach (var arg in Args) {
                al.Add(arg);
            }
            ClientStorySystem.Instance.SendMessage(msgId, al);
        } else {
            ClientStorySystem.Instance.SendMessage(msgId, gameObject.name);
        }
    }
}
