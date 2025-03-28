﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameLibrary;

public class StoryCamera : MonoBehaviour
{
    public bool m_IsFollowYaw = true;
    // The distance in the x-z plane to the target
    public float m_Distance = 7.0f;
    // the height we want the camera to be above the target
    public float m_Height = 6.0f;
    public float m_FixedYaw = 0;
    public Vector3 m_CenterOffset = Vector3.zero;
    public float m_DistanceOnMaxSpeed = 50;
    public float m_DistanceOnMinSpeed = 0.5f;
    public float m_MaxSpeed = 50;
    public float m_MinSpeed = 20;
    public float m_Power = 1;
    public float m_CameraFollowSpeed = 10.0f;
    public float m_MinHeight = 0.5f;
    public float m_MaxHeight = 64.0f;
    public float m_MinDistance = 1.0f;
    public float m_MaxDistance = 128.0f;
    public float m_RestoreYawInterval = 2.0f;
    public float m_RestoreYawPerTick = 1.5f;

    void Awake()
    {
        try {
            m_TerrainLayerMask = 1 << LayerMask.NameToLayer("Terrain");
            m_Camera = GetComponentInChildren<Camera>();
            if (null == m_Camera) {
                m_Camera = GetComponent<Camera>();
            }
            if (null == m_CameraTransform) {
                m_CameraTransform = transform;
            }

            m_CameraSpeed = m_CameraFollowSpeed;
            if (null == m_Camera) {
                Debug.Log("Please assign the StoryCamera script to a camera.");
                enabled = false;
            }
            ComputeSpeedFactor();
            m_OrigDistanceOnMaxSpeed = m_DistanceOnMaxSpeed;
            m_OrigDistanceOnMinSpeed = m_DistanceOnMinSpeed;
            m_OrigMaxSpeed = m_MaxSpeed;
            m_OrigMinSpeed = m_MinSpeed;
            m_OrigPower = m_Power;

            m_OrigHeight = Height;
            m_OrigDistance = Distance;
            m_CurDistance = Distance;
        }
        catch (System.Exception ex) {
            LogSystem.Error("Exception {0}\n{1}", ex.Message, ex.StackTrace);
        }
    }
    void LateUpdate()
    {
        try {
            if (null == m_Camera || !m_Camera.enabled) {
                return;
            }
            if (!m_ControlByOtherScript) {
                Apply();
                //CheckCollision();
            }
        }
        catch (System.Exception ex) {
            LogSystem.Error("MainCamera.LateUpdate throw exception {0}\n{1}", ex.Message, ex.StackTrace);
        }
    }

    private void BeginControlByOtherScript()
    {
        m_ControlByOtherScript = true;
    }
    private void EndControlByOtherScript()
    {
        m_ControlByOtherScript = false;
    }
    private void CameraFollow(int id)
    {
        GameObject obj = SceneSystem.Instance.GetGameObject(id);
        if (null != obj) {
            m_CurTargetId = id;
            m_Target = obj.transform;
            m_CurTargetPos = m_Target.position;
            Cut();
            m_ControlByOtherScript = false;
        }
    }
    private void CameraLook(object[] coord)
    {
        if (null == coord || coord.Length != 3)
            return;
        m_Target = null;
        m_TargetPos = new Vector3((float)System.Convert.ChangeType(coord[0], typeof(float)), (float)System.Convert.ChangeType(coord[1], typeof(float)), (float)System.Convert.ChangeType(coord[2], typeof(float)));
        Cut();
    }
    private void CameraLookImmediately(object[] coord)
    {
        if (null == coord || coord.Length != 3)
            return;
        m_Target = null;
        m_TargetPos = new Vector3((float)System.Convert.ChangeType(coord[0], typeof(float)), (float)System.Convert.ChangeType(coord[1], typeof(float)), (float)System.Convert.ChangeType(coord[2], typeof(float)));
        m_CurTargetPos = m_TargetPos;
        Cut();
    }
    private void CameraLookToward(object[] coord)
    {
        if (null == coord || coord.Length != 3 || null == m_CameraTransform)
            return;
        m_Target = null;
        m_TargetPos = new Vector3((float)System.Convert.ChangeType(coord[0], typeof(float)), (float)System.Convert.ChangeType(coord[1], typeof(float)), (float)System.Convert.ChangeType(coord[2], typeof(float)));
        m_CurTargetPos = m_TargetPos;
        Vector3 targetCenter = m_CurTargetPos + m_CenterOffset;
        Distance = (targetCenter - m_CameraTransform.transform.position).magnitude;
        m_CurDistance = Distance;
        float radian = Geometry.GetYRadian(m_CameraTransform.transform.position, m_TargetPos);
        FixedYaw = Geometry.RadianToDegree(radian);
        m_AngularSmoothLag = 0.3f;
        m_SnapSmoothLag = 0.2f;
        Cut();
    }
    private void CameraLookCopy(object[] args)
    {
        if (null == args || args.Length != 4) {
            return;
        }
        var t = args[0] as Transform;
        var tpos = new Vector3((float)System.Convert.ChangeType(args[1], typeof(float)), (float)System.Convert.ChangeType(args[2], typeof(float)), (float)System.Convert.ChangeType(args[3], typeof(float)));

        if (null != t) {
            var pos = t.position;
            var euler = t.eulerAngles;

            m_CenterOffset = Vector3.zero;
            Vector3 targetCenter = tpos + m_CenterOffset;

            m_Target = null;
            m_TargetPos = tpos;
            m_CurTargetPos = m_TargetPos;

            Distance = (targetCenter - pos).magnitude;
            m_CurDistance = Distance;
            Height = pos.y - targetCenter.y;
            FixedYaw = euler.y;

            m_AngularSmoothLag = 0.3f;
            m_SnapSmoothLag = 0.2f;
            Cut();
        }
    }
    private void CameraLookObjCopy(object[] args)
    {
        if (null == args || args.Length != 2) {
            return;
        }
        var t = args[0] as Transform;
        int id = (int)System.Convert.ChangeType(args[1], typeof(int));

        GameObject obj = SceneSystem.Instance.GetGameObject(id);
        if (null != t && null != obj) {
            var tran = obj.transform;
            var pos = t.position;
            var euler = t.eulerAngles;
            var tpos = tran.position;

            var pos2d = new Vector2(pos.x, pos.z);
            var tpos2d = new Vector2(tpos.x, tpos.z);
            var dir = t.rotation * Vector3.forward;
            var dir2d = tpos2d - pos2d;
            var dist2d = dir2d.magnitude;
            dir2d.Normalize();
            var cos = Vector3.Dot(dir, Vector3.down);
            var sin = Vector3.Dot(dir, new Vector3(dir2d.x, 0, dir2d.y));
            var delta = cos * dist2d / sin;
            float h = 0;
            if (pos.y > tpos.y) {
                h = pos.y - delta - tpos.y;
            }
            else {
                h = pos.y + delta - tpos.y;
            }

            m_CenterOffset.Set(0, h, 0);

            Vector3 targetCenter = tpos + m_CenterOffset;

            m_Target = null;
            m_TargetPos = tpos;
            m_CurTargetPos = m_TargetPos;

            Distance = (targetCenter - pos).magnitude;
            m_CurDistance = Distance;
            Height = pos.y - targetCenter.y;
            FixedYaw = euler.y;

            m_AngularSmoothLag = 0.3f;
            m_SnapSmoothLag = 0.2f;
            Cut();
        }
    }
    private void CameraFixedYaw(float angle)
    {
        FixedYaw = ((angle % 360) + 360) % 360;
        m_AngularSmoothLag = 0.3f;
        m_SnapSmoothLag = 0.2f;
    }
    private void CameraYaw(object[] args)
    {
        if (null == args || args.Length != 3)
            return;
        float angle = (float)System.Convert.ChangeType(args[0], typeof(float));
        float alag = (float)System.Convert.ChangeType(args[1], typeof(float)) / 1000.0f;
        float slag = (float)System.Convert.ChangeType(args[2], typeof(float)) / 1000.0f;
        FixedYaw = ((angle % 360) + 360) % 360; ;
        m_AngularSmoothLag = alag;
        m_SnapSmoothLag = slag;
    }
    private void CameraHeight(object[] args)
    {
        if (null == args || args.Length != 2)
            return;
        float height = (float)System.Convert.ChangeType(args[0], typeof(float));
        float lag = (float)System.Convert.ChangeType(args[1], typeof(float)) / 1000.0f;
        if (height > 0)
            Height = height;
        else
            Height = m_OrigHeight;
        m_HeightSmoothLag = lag;
        m_NeedLookat = true;
    }
    private void CameraDistance(object[] args)
    {
        if (null == args || args.Length != 2)
            return;
        float dist = (float)System.Convert.ChangeType(args[0], typeof(float));
        float lag = (float)System.Convert.ChangeType(args[1], typeof(float)) / 1000.0f;
        if (dist > 0) {
            Distance = dist;
        }
        else {
            Distance = m_OrigDistance;
        }
        m_DistanceSmoothLag = lag;
        m_NeedLookat = true;
    }
    private void CameraChangeFixedYaw(float delta)
    {
        float angle = FixedYaw;
        angle += delta;
        FixedYaw = ((angle % 360) + 360) % 360;
        m_AngularSmoothLag = 0.01f;
        m_SnapSmoothLag = 0.01f;
    }
    private void CameraChangeHeight(float delta)
    {
        Height += delta;
        if (Height < 0.5f)
            Height = 0.5f;
        else if (Height > 64)
            Height = 64;
        m_HeightSmoothLag = 0.01f;
    }
    private void CameraChangeDistance(float delta)
    {
        Distance += delta;
        if (Distance < 1)
            Distance = 1;
        else if (Distance > 128)
            Distance = 128;
        m_DistanceSmoothLag = 0.01f;
    }
    private void SetDistanceAndHeight(object[] args)
    {
        if (null == args || args.Length != 2) {
            return;
        }
        Distance = (float)System.Convert.ChangeType(args[0], typeof(float));
        Height = (float)System.Convert.ChangeType(args[1], typeof(float));
    }
    private void StoreDistanceAndHeight()
    {
        m_OrigDistance = Distance;
        m_OrigHeight = Height;
    }
    private void RestoreDistanceAndHeight()
    {
        Distance = m_OrigDistance;
        Height = m_OrigHeight;
    }
    private void SetFollowSpeed(object[] args)
    {
        if (null == args || args.Length != 5) {
            return;
        }
        float maxdistance = (float)System.Convert.ChangeType(args[0], typeof(float));
        float mindistance = (float)System.Convert.ChangeType(args[1], typeof(float));
        float maxspeed = (float)System.Convert.ChangeType(args[2], typeof(float));
        float minspeed = (float)System.Convert.ChangeType(args[3], typeof(float));
        int power = (int)System.Convert.ChangeType(args[4], typeof(int));
        m_DistanceOnMaxSpeed = maxdistance;
        m_DistanceOnMinSpeed = mindistance;
        m_MaxSpeed = maxspeed;
        m_MinSpeed = minspeed;
        m_Power = power;
        ComputeSpeedFactor();
    }
    private void ResetFollowSpeed()
    {
        m_DistanceOnMaxSpeed = m_OrigDistanceOnMaxSpeed;
        m_DistanceOnMinSpeed = m_OrigDistanceOnMinSpeed;
        m_MaxSpeed = m_OrigMaxSpeed;
        m_MinSpeed = m_OrigMinSpeed;
        m_Power = m_OrigPower;
        ComputeSpeedFactor();
    }
    private void SetFollowYaw(object arg)
    {
        bool val = (bool)System.Convert.ChangeType(arg, typeof(bool));
        m_IsFollowYaw = val;
    }

    private void DebugDrawStuff()
    {
        if (null != m_Target) {
            Debug.DrawLine(m_Target.position, m_Target.position + m_CenterOffset);
        }
    }

    private void Apply()
    {
        if (null == m_Camera || !m_Camera.enabled) {
            return;
        }
        if (null == m_CameraTransform) {
            return;
        }

        if (m_IsFollowYaw && null != m_Target && m_LastUpdateYawTime + m_RestoreYawInterval < Time.time) {
            if (m_FixedYaw > 180 && m_FixedYaw < 360) {
                m_FixedYaw += m_RestoreYawPerTick;
                if (m_FixedYaw > 360)
                    m_FixedYaw = 0;
            }
            else if (m_FixedYaw > float.Epsilon) {
                m_FixedYaw -= m_RestoreYawPerTick;
                if (m_FixedYaw < 0)
                    m_FixedYaw = 0;
            }
        }

        SetUpPosition();

        Vector3 targetCenter = m_CurTargetPos + m_CenterOffset;

        // Always look at the target	
        SetUpRotation(targetCenter);
    }
    private void CheckCollision()
    {
        Vector3 targetCenter = m_CurTargetPos + m_CenterOffset;
        var ray = new Ray(targetCenter, -m_CameraTransform.forward);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, Distance, m_TerrainLayerMask)) {
            m_CameraTransform.position = hitInfo.point;
        }
    }
    private void SetUpPosition()
    {
        if (Time.deltaTime < float.Epsilon) {
            return;
        }
        AdjustSpeedAndMoveTarget();

        Vector3 targetCenter = m_CurTargetPos + m_CenterOffset;
        //DebugDrawStuff();

        // Calculate the current & target rotation angles
        float targetYaw = 0;
        if (null != m_Target && m_IsFollowYaw) {
            targetYaw = m_Target.eulerAngles.y;
        }
        float originalTargetAngle = targetYaw + FixedYaw;
        float currentAngle = m_CameraTransform.eulerAngles.y;

        // Adjust real target angle when camera is locked
        float targetAngle = originalTargetAngle;

        // When pressing Fire2 (alt) the camera will snap to the target direction real quick.
        // It will stop snapping when it reaches the target
        //m_Snap = true;

        if (m_Snap) {
            // We are close to the target, so we can stop snapping now!
            if (AngleDistance(currentAngle, originalTargetAngle) < 3.0)
                m_Snap = false;

            currentAngle = Mathf.SmoothDampAngle(currentAngle, targetAngle, ref m_AngleVelocity, m_SnapSmoothLag, m_SnapMaxSpeed);
        }
        // Normal camera motion
        else {
            currentAngle = Mathf.SmoothDampAngle(currentAngle, targetAngle, ref m_AngleVelocity, m_AngularSmoothLag, m_AngularMaxSpeed);
        }

        /*
          // When jumping don't move camera upwards but only down!
          if (false)
          {
              // We'd be moving the camera upwards, do that only if it's really high
              float newTargetHeight = targetCenter.y + m_Height;
              if (newTargetHeight < m_TargetHeight || newTargetHeight - m_TargetHeight > 5)
                  m_TargetHeight = targetCenter.y + m_Height;
          }
          // When walking always update the target height
          else*/
        {
            m_TargetHeight = targetCenter.y + Height;
        }

        // Damp the height
        float currentHeight = m_CameraTransform.position.y;
        currentHeight = Mathf.SmoothDamp(currentHeight, m_TargetHeight, ref m_HeightVelocity, m_HeightSmoothLag);
        m_CurDistance = Mathf.SmoothDamp(m_CurDistance, Distance, ref m_DistanceVelocity, m_DistanceSmoothLag);

        // Convert the angle into a rotation, by which we then reposition the camera
        Quaternion currentRotation = Quaternion.Euler(0, currentAngle, 0);

        // Set the position of the camera on the x-z plane to:
        // distance meters behind the target
        Vector3 pos = targetCenter;
        pos += currentRotation * Vector3.back * m_CurDistance;

        // Set the height of the camera
        pos.y = currentHeight;

        m_CameraTransform.position = pos;
    }
    private void Cut()
    {
        float oldHeightSmooth = m_HeightSmoothLag;
        float oldDistanceSmooth = m_DistanceSmoothLag;
        float oldSnapMaxSpeed = m_SnapMaxSpeed;
        float oldSnapSmooth = m_SnapSmoothLag;

        m_SnapMaxSpeed = 10000;
        m_SnapSmoothLag = 0.001f;
        m_HeightSmoothLag = 0.001f;
        m_DistanceSmoothLag = 0.001f;

        m_Snap = true;
        Apply();

        m_HeightSmoothLag = oldHeightSmooth;
        m_DistanceSmoothLag = oldDistanceSmooth;
        m_SnapMaxSpeed = oldSnapMaxSpeed;
        m_SnapSmoothLag = oldSnapSmooth;
    }
    private void AdjustSpeedAndMoveTarget()
    {
        if (null != m_Target) {
            m_TargetPos = m_Target.position;
        }
        float delta = Time.deltaTime;
        Vector3 distDir = m_TargetPos - m_CurTargetPos;
        float dist = distDir.magnitude;
        distDir.Normalize();
        m_CameraSpeed = GetCurSpeed(dist);
        Vector3 motion = distDir * m_CameraSpeed * delta;
        if (motion.magnitude >= dist) {
            m_CurTargetPos = m_TargetPos;
            m_CameraSpeed = dist / Time.deltaTime;
        }
        else {
            m_CurTargetPos += motion;
        }
    }
    private void SetUpRotation(Vector3 centerPos)
    {
        //When height and distance change, the lookat target needs to be maintained
        float currentHeight = m_CameraTransform.position.y;
        if (m_NeedLookat) {
            if (!Geometry.IsSameFloat(currentHeight, m_TargetHeight) || !Geometry.IsSameFloat(m_CurDistance, Distance)) {
                m_CameraTransform.LookAt(centerPos, Vector3.up);
            }
            else {
                m_NeedLookat = false;
            }
        }
        else {
            // Now it's getting hairy. The devil is in the details here, the big issue is jumping of course.
            // * When jumping up and down we don't want to center the guy in screen space.
            //  This is important to give a feel for how high you jump and avoiding large camera movements.
            //   
            // * At the same time we dont want him to ever go out of screen and we want all rotations to be totally smooth.
            //
            // So here is what we will do:
            //
            // 1. We first find the rotation around the y axis. Thus he is always centered on the y-axis
            // 2. When grounded we make him be centered
            // 3. When jumping we keep the camera rotation but rotate the camera to get him back into view if his head is above some threshold
            // 4. When landing we smoothly interpolate towards centering him on screen
            Vector3 cameraPos = m_CameraTransform.position;
            Vector3 offsetToCenter = centerPos - cameraPos;

            // Generate base rotation only around y-axis
            Quaternion yRotation = Quaternion.LookRotation(new Vector3(offsetToCenter.x, 0, offsetToCenter.z));

            Vector3 relativeOffset = Vector3.forward * m_CurDistance + Vector3.down * Height;
            m_CameraTransform.rotation = yRotation * Quaternion.LookRotation(relativeOffset);

            // Calculate the projected center position and top position in world space
            // Ray centerRay = m_CameraTransform.GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5f, 0.5f, 1));
            // Ray topRay = m_CameraTransform.GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5f, m_ClampHeadPositionScreenSpace, 1));
            Ray centerRay = m_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1));
            Ray topRay = m_Camera.ViewportPointToRay(new Vector3(0.5f, m_ClampHeadPositionScreenSpace, 1));


            Vector3 centerRayPos = centerRay.GetPoint(m_CurDistance);
            Vector3 topRayPos = topRay.GetPoint(m_CurDistance);

            float centerToTopAngle = Vector3.Angle(centerRay.direction, topRay.direction);

            float heightToAngle = centerToTopAngle / (centerRayPos.y - topRayPos.y);

            float extraLookAngle = heightToAngle * (centerRayPos.y - centerPos.y);
            if (extraLookAngle < centerToTopAngle) {
                extraLookAngle = 0;
            }
            else {
                extraLookAngle = extraLookAngle - centerToTopAngle;
                m_CameraTransform.rotation *= Quaternion.Euler(-extraLookAngle, 0, 0);
            }
        }
    }

    private int GetTargetId()
    {
        return m_CurTargetId;
    }
    private Vector3 GetCenterOffset()
    {
        return m_CenterOffset;
    }
    private float GetCurSpeed(float distance)
    {
        float result = m_MinSpeed;
        if (distance > m_DistanceOnMaxSpeed) {
            return m_MaxSpeed;
        }
        if (distance < m_DistanceOnMinSpeed) {
            return m_MinSpeed;
        }
        result = m_FactorA * Mathf.Pow(distance, m_Power) + m_FactorB;
        return result;
    }
    private void ComputeSpeedFactor()
    {
        //a*min_distance^n + b = min_speed
        //a*max_distance^n + b = max_speed
        float denominator = Mathf.Pow(m_DistanceOnMaxSpeed, m_Power) - Mathf.Pow(m_DistanceOnMinSpeed, m_Power);
        if (denominator > float.Epsilon) {
            m_FactorA = (m_MaxSpeed - m_MinSpeed) / denominator;
        }
        else {
            m_FactorA = 0;
        }
        m_FactorB = m_MinSpeed - m_FactorA * Mathf.Pow(m_DistanceOnMinSpeed, m_Power);
    }
    private float AngleDistance(float a, float b)
    {
        a = Mathf.Repeat(a, 360);
        b = Mathf.Repeat(b, 360);

        return Mathf.Abs(b - a);
    }
    private float FixedYaw
    {
        get { return m_FixedYaw; }
        set {
            m_FixedYaw = value;
            while (m_FixedYaw < 0) {
                m_FixedYaw += 360;
            }
            m_FixedYaw %= 360;
            m_LastUpdateYawTime = Time.time;
        }
    }
    private float Height
    {
        get { return m_Height; }
        set {
            m_Height = value;
            if (m_Height < m_MinHeight)
                m_Height = m_MinHeight;
            else if (m_Height > m_MaxHeight)
                m_Height = m_MaxHeight;
        }
    }
    private float Distance
    {
        get { return m_Distance; }
        set {
            m_Distance = value;
            if (m_Distance < m_MinDistance)
                m_Distance = m_MinDistance;
            else if (m_Distance > m_MaxDistance)
                m_Distance = m_MaxDistance;
        }
    }

    private bool m_ControlByOtherScript = false;
    private bool m_NeedLookat = false;

    private Camera m_Camera;
    private Transform m_CameraTransform;
    private Transform m_Target;
    private Vector3 m_CurTargetPos;
    private Vector3 m_TargetPos;
    private float m_CurDistance;
    private float m_CameraSpeed;

    private float m_FactorA;
    private float m_FactorB;
    private float m_OrigDistanceOnMaxSpeed;
    private float m_OrigDistanceOnMinSpeed;
    private float m_OrigMaxSpeed;
    private float m_OrigMinSpeed;
    private float m_OrigPower;
    private float m_OrigHeight;
    private float m_OrigDistance;

    private float m_HeightSmoothLag = 0.3f;
    private float m_DistanceSmoothLag = 0.5f;
    private float m_AngularSmoothLag = 0.3f;
    private float m_AngularMaxSpeed = 360.0f;
    private float m_SnapSmoothLag = 0.2f;
    private float m_SnapMaxSpeed = 360.0f;
    private float m_ClampHeadPositionScreenSpace = 0.75f;
    private float m_HeightVelocity = 0.0f;
    private float m_AngleVelocity = 0.0f;
    private float m_DistanceVelocity = 0.0f;
    private bool m_Snap = false;
    private float m_TargetHeight = 100000.0f;
    private int m_CurTargetId;

    private float m_LastUpdateYawTime = 0;
    private int m_TerrainLayerMask = 0;
}
