using UnityEngine;
using System.Collections;
using GameLibrary.Story;

public class StoryMessage : MonoBehaviour
{
    public string[] Args;
    public void SendStoryMessage(string msgId)
    {
        if (null != Args && Args.Length > 0) {
            ArrayList al = new ArrayList();
            al.Add(gameObject.name);
            al.AddRange(Args);
            ClientStorySystem.Instance.SendMessage(msgId, al.ToArray());
        } else {
            ClientStorySystem.Instance.SendMessage(msgId, gameObject.name);
        }
    }
}
