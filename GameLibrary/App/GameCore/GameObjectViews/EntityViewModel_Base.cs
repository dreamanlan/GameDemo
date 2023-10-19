using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using GameLibrary;

namespace GameLibrary
{
    public partial class EntityViewModel
    {
        public GameObject Actor
        {
            get { return m_Actor; }
        }

        public int ObjId
        {
            get { return m_ObjId; }
        }

        public bool Active
        {
            get { return m_Active; }
            set
            {
                m_Active = value;
                UpdateActive();
            }
        }

        public bool Visible
        {
            get { return m_Visible; }
            set
            {
                if (m_Visible != value) {
                    m_Visible = value;
                    UpdateVisible();
                }
            }
        }

        public UnityEngine.Vector3 position
        {
            get { return Actor.transform.position; }
        }
        
        private void SetVisible(bool visible)
        {
            if (null != m_Actor) {
                ResourceSystem.Instance.SetVisible(m_Actor, visible, null);
            }
        }

        private void CreateActor(int objId, string model, float x, float y, float z, float dir, float scale)
        {
            m_ObjId = objId;
            m_Actor = ResourceSystem.Instance.NewObject(model) as GameObject;
            if (null != m_Actor) {
                m_Actor.transform.position = new Vector3(x, y, z);
                m_Actor.transform.localRotation = Quaternion.Euler(0, Geometry.RadianToDegree(dir), 0);
                m_Actor.transform.localScale = new Vector3(scale, scale, scale);
            }
        }

        private void DestroyActor()
        {
            if (null != m_Actor) {
                ResourceSystem.Instance.RecycleObject(m_Actor);
                m_Actor = null;
            }
        }

        private int m_ObjId = 0;
        private bool m_Active = true;
        private bool m_Visible = true;
        private GameObject m_Actor = null;
    }
}

