using System;
using System.Collections.Generic;
using System.Text;

namespace GameLibrary
{
    public class MovementStateInfo
    {
        public bool IsMoving
        {
            get { return m_IsMoving; }
            set { m_IsMoving = value; }
        }
        public float PositionX
        {
            get { return m_Position.x; }
            set { m_Position.x = value; }
        }
        public float PositionY
        {
            get { return m_Position.y; }
            set { m_Position.y = value; }
        }
        public float PositionZ
        {
            get { return m_Position.z; }
            set { m_Position.z = value; }
        }

        public void SetPosition(float x, float y, float z)
        {
            m_Position.x = x;
            m_Position.y = y;
            m_Position.z = z;
        }
        public void SetPosition(UnityEngine.Vector3 pos)
        {
            m_Position = pos;
        }
        public UnityEngine.Vector3 GetPosition3D()
        {
            return m_Position;
        }
        public void SetPosition2D(float x, float y)
        {
            m_Position.x = x;
            m_Position.z = y;
        }
        public void SetPosition2D(UnityEngine.Vector2 pos)
        {
            m_Position.x = pos.x;
            m_Position.z = pos.y;
        }
        public UnityEngine.Vector2 GetPosition2D()
        {
            return new UnityEngine.Vector2(m_Position.x, m_Position.z);
        }
        public void SetFaceDir(float rot)
        {
            SetFaceDir(rot, true);
        }
        public void SetFaceDir(float rot, bool includeWantedDir)
        {
            if (Math.Abs(m_FaceDir - rot) > c_Precision) {
                m_FaceDir = rot;
                m_FaceDir3D = new UnityEngine.Vector3((float)Math.Sin(rot), 0, (float)Math.Cos(rot));
            }
            if (includeWantedDir) {
                m_WantedFaceDir = rot;
            }
        }
        public float GetFaceDir()
        {
            return m_FaceDir;
        }
        public void SetWantedFaceDir(float rot)
        {
            m_WantedFaceDir = rot;
        }
        public float GetWantedFaceDir()
        {
            return m_WantedFaceDir;
        }
        public UnityEngine.Vector2 GetFaceDir2D()
        {
            return new UnityEngine.Vector2(m_FaceDir3D.x, m_FaceDir3D.z);
        }
        public UnityEngine.Vector3 GetFaceDir3D()
        {
            return m_FaceDir3D;
        }
        public MovementStateInfo()
        {
            m_Position = UnityEngine.Vector3.zero;
        }
        public void Reset()
        {
            m_IsMoving = false;
            m_Position = UnityEngine.Vector3.zero;
            m_FaceDir = 0;
            m_FaceDir3D = UnityEngine.Vector3.zero;
        }

        private bool m_IsMoving = false;
        private UnityEngine.Vector3 m_Position = UnityEngine.Vector3.zero;
        private float m_FaceDir = 0;
        private float m_WantedFaceDir = 0;
        private UnityEngine.Vector3 m_FaceDir3D = UnityEngine.Vector3.zero;

        private const float c_Precision = 0.001f;
    }
}
