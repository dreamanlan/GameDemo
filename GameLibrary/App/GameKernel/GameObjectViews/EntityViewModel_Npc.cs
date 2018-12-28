using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AI;
using GameLibrary;

namespace GameLibrary
{
    public partial class EntityViewModel
    {
        internal const string c_StandAnim = "idle";
        internal const string c_MoveAnim = "running";
        internal const float c_CrossFadeTime = 0.1f;

        internal EntityInfo Entity
        {
            get { return m_Entity; }
        }
        internal NavMeshAgent Agent
        {
            get { return m_Agent; }
        }
        internal Animator Animator
        {
            get { return m_Animator; }
        }
        internal void Create(EntityInfo entity)
        {
            if (null != entity) {
                m_Entity = entity;
                MovementStateInfo msi = m_Entity.GetMovementStateInfo();
                UnityEngine.Vector3 pos = msi.GetPosition3D();
                float dir = msi.GetFaceDir();
                CreateActor(m_Entity.GetId(), m_Entity.GetModel(), pos.x, pos.y, pos.z, dir, m_Entity.Scale);
                if (null != Actor) {                    
                    m_Agent = Actor.GetComponent<NavMeshAgent>();
                    if (m_Agent == null) {
                        m_Agent = Actor.AddComponent<NavMeshAgent>();
                        m_Agent.angularSpeed = c_AngularSpeed;
                        m_Agent.acceleration = c_Acceleration;
                        m_Agent.radius = entity.GetRadius();
                        m_Agent.speed = entity.Speed;
                        m_Agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
                    }
                    m_Animator = Actor.GetComponentInChildren<Animator>();
                    SetMoveAgentEnable(true);
                    try{
                        if (null != m_Agent) {
                            m_Agent.ResetPath();
                        }
                    } catch {
                        if (null != m_Agent) {
                            m_Agent.enabled = true;
                        }
                    }
                }
            }
        }

        internal void Destroy()
        {
            DestroyActor();
            m_Entity = null;
        }

        internal void Recreate()
        {
            UnityEngine.Quaternion q = Actor.transform.rotation;
            UnityEngine.Vector3 scale = Actor.transform.localScale;

            SceneSystem.Instance.EntityViewManager.RemoveActorForRecreate(Actor);
            DestroyActor();

            MovementStateInfo msi = m_Entity.GetMovementStateInfo();
            UnityEngine.Vector3 pos = msi.GetPosition3D();
            float dir = msi.GetFaceDir();
            int id = m_Entity.GetId();
            CreateActor(id, m_Entity.GetModel(), pos.x, pos.y, pos.z, dir, m_Entity.Scale);
            if (null != Actor) {
                Actor.transform.rotation = q;
                Actor.transform.localScale = scale;

                SceneSystem.Instance.EntityViewManager.AddActorForRecreate(Actor, id);
            }
        }

        internal void Update()
        {
            UpdateSpatial();
        }

        internal void Death()
        {
            SetMoveAgentEnable(false);
        }

        internal void SetMoveAgentEnable(bool enable)
        {
            if (null != Agent) {
                Agent.enabled = enable;
            }
        }

        internal void MoveDir(float dir, float deltaTime)
        {
            MovementStateInfo msi = m_Entity.GetMovementStateInfo();
            msi.SetFaceDir(dir);
            if(null!=Actor){
                var deg = Geometry.RadianToDegree(msi.GetFaceDir());
                Actor.transform.localEulerAngles = new Vector3(0, deg, 0);
            }
            if (!msi.IsMoving) {
                msi.IsMoving = true;

                if (null != Animator) {
                    if (ObjId == SceneSystem.Instance.PlayerId) {
                        //Animator.CrossFade(c_MoveAnim, c_CrossFadeTime);
                        Animator.Play(c_MoveAnim);
                    } else {
                        Animator.Play(c_MoveAnim);
                    }
                }
            }
            if (null != Agent && Agent.enabled) {
                try {
                    var forwardDir = msi.GetFaceDir3D();
                    var offset = forwardDir * m_Entity.Speed * deltaTime;
                    Agent.Move(offset);
                } catch {
                    m_Agent.enabled = true;
                }
            }
        }

        internal void MoveTo(float x, float y, float z)
        {
            MovementStateInfo msi = m_Entity.GetMovementStateInfo();
            msi.IsMoving = true;
            if (null != Agent && Agent.enabled) {
                try {
                    Agent.SetDestination(new Vector3(x, y, z));
                    Agent.isStopped = false;
                } catch {
                    m_Agent.enabled = true;

                    LogSystem.Error("MoveTo({0}, {1}, {2}) agent {3}({4}) failed.", x, y, z, m_ObjId, m_Entity.GetUnitId());
                }
            }
            if (null != Animator) {
                if (ObjId == SceneSystem.Instance.PlayerId) {
                    //Animator.CrossFade(c_MoveAnim, c_CrossFadeTime);
                    Animator.Play(c_MoveAnim);
                } else {
                    Animator.Play(c_MoveAnim);
                }
            }
        }

        internal void StopMove()
        {
            MovementStateInfo msi = m_Entity.GetMovementStateInfo();
            msi.IsMoving = false;
            if (null != Agent && Agent.enabled) {
                try {
                    m_Agent.isStopped = true;
                } catch {
                    m_Agent.enabled = true;
                }
            }
            if (null != Animator) {
                if (ObjId == SceneSystem.Instance.PlayerId) {
                    Animator.CrossFade(c_StandAnim, c_CrossFadeTime);
                    //Animator.Play(c_StandAnim);
                } else {
                    Animator.Play(c_StandAnim);
                }
            }
        }

        internal void PlayAnimation(string anim)
        {
            if (null != Animator) {
                if (ObjId == SceneSystem.Instance.PlayerId) {
                    //Animator.CrossFade(anim, c_CrossFadeTime);
                    Animator.Play(anim);
                } else {
                    Animator.Play(anim);
                }
            }
        }
                
        private void UpdateActive()
        {
            bool active = m_Active;
            if (null != m_Actor) {
                m_Actor.SetActive(active);
            }
        }

        private void UpdateVisible()
        {
            if (null != m_Actor) {
                bool visible = m_Visible;
                var renderes = m_Actor.GetComponentsInChildren<Renderer>();
                for (int i = 0; i < renderes.Length; ++i) {
                    renderes[i].enabled = visible;
                }
            }
        }

        private void UpdateSpatial()
        {
            if (null != m_Entity && null != Actor) {
                MovementStateInfo msi = m_Entity.GetMovementStateInfo();
                if (Math.Abs(msi.GetWantedFaceDir() - msi.GetFaceDir()) > Geometry.c_FloatPrecision) {
                    float degree = Geometry.RadianToDegree(msi.GetWantedFaceDir());
                    Quaternion targetQ = Quaternion.Euler(0, degree, 0);
                    Actor.transform.localRotation = Quaternion.RotateTowards(Actor.transform.localRotation,
                        targetQ, c_TurnRoundSpeed * Time.deltaTime);
                    float dir = Geometry.DegreeToRadian(Actor.transform.localEulerAngles.y);
                    msi.SetFaceDir(dir, false);
                } else {
                    SyncFaceDir();
                }
            }
            SyncPosition();
        }

        private void SyncFaceDir()
        {
            if (null != m_Entity && null != Actor) {
                MovementStateInfo msi = m_Entity.GetMovementStateInfo();
                float dir = Geometry.DegreeToRadian(Actor.transform.localEulerAngles.y);
                msi.SetFaceDir(dir);
            }
        }

        private void SyncPosition()
        {
            if (null != m_Entity && null != Actor) {
                MovementStateInfo msi = m_Entity.GetMovementStateInfo();
                UnityEngine.Vector3 v3 = Actor.transform.position;
                msi.SetPosition(v3.x, v3.y, v3.z);
            }
        }

        private EntityInfo m_Entity = null;
        private NavMeshAgent m_Agent = null;
        private Animator m_Animator = null;

        private const float c_StopDistSqr = 0.25f;
        private const float c_AngularSpeed = 3600;
        private const float c_Acceleration = 64;
        private const float c_TurnRoundSpeed = 360;
    }
}
