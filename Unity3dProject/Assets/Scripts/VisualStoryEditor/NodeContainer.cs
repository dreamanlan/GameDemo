using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System;
using System.Reflection;
using UnityEditor;

namespace VisualStoryTool
{
    public class NodeContainer : ScriptableObject
    {
        public bool Foldout = true;
        public Rect Rect;
        public NodeInfo Node;
        [NonSerialized]
        public int WinID;
        [NonSerialized]
        public IList<NodeContainer> AllNodeContainers;

        public void SetNode(NodeInfo node)
        {
            this.Node = node;
            node.Container = this;
        }
        public Rect GetInputPortRect()
        {
            return new Rect(Rect.xMin - 16, Rect.center.y - 8, 16, 16);
        }
        public int GetOutputNum()
        {
            return Node.Outputs.Count;
        }
        public Rect GetOutputPortRect(int ix)
        {
            var output = Node.Outputs[ix];
            int index = Node.Commands.IndexOf(output.Command);
            return new Rect(Rect.xMax, Rect.yMin + 25 + 18 * index, 16, 16);
        }
    }
}