using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using GameLibrary;

public class ScreenInput : MonoBehaviour
{
    public enum MouseButton
    {
        LEFT = 0,
        RIGHT,
        MIDDLE,
    }

    public RectTransform PanelFullScreen;
    public RectTransform JoystickBkg;
    public Image JoystickButton;
    public Text TxtFps;
    public bool IsFollowYaw = true;

    public void SetFollowYaw(object arg)
    {
        bool val = (bool)System.Convert.ChangeType(arg, typeof(bool));
        IsFollowYaw = val;
    }

    // Use this for initialization
    internal void Start()
    {
        UIEvent.BindBeginDrag(JoystickBkg.gameObject, JoystickOnBeginDrag);
        UIEvent.BindDrag(JoystickBkg.gameObject, JoystickOnDrag);
        UIEvent.BindEndDrag(JoystickBkg.gameObject, JoystickOnEndDrag);

        UIEvent.BindBeginDrag(PanelFullScreen.gameObject, ScreenOnBeginDrag);
        UIEvent.BindDrag(PanelFullScreen.gameObject, ScreenOnDrag);
        UIEvent.BindEndDrag(PanelFullScreen.gameObject, ScreenOnEndDrag);

        UIEvent.BindDown(PanelFullScreen.gameObject, ScreenOnDown);
        UIEvent.BindUp(PanelFullScreen.gameObject, ScreenOnUp);
    }

    internal void Update()
    {
        m_Count++;
        m_TimeElapse = Time.realtimeSinceStartup - m_TimeCountStart;
        if (m_TimeElapse >= INTERVAL) {
            float newFps = 1.0f / m_TimeElapse * m_Count;
            m_TimeCountStart = Time.realtimeSinceStartup;
            m_Count = 0;
            if (Mathf.Abs(newFps - m_Fps) >= 3) {
                m_Fps = newFps;
                TxtFps.text = ((int)m_Fps).ToString();
            }
        }

        if (m_IsMoving) {
            var player = SceneSystem.Instance.GetEntityById(SceneSystem.Instance.PlayerId);
            if (null != player) {
                if(IsFollowYaw){
                    float nPiToPi = ((m_MovingAngle + Mathf.PI) % (Mathf.PI * 2) - Mathf.PI);
                    float worldAngle = player.GetMovementStateInfo().GetFaceDir() + nPiToPi * m_Coefficient;
                    SceneSystem.Instance.MoveDir(worldAngle);
                } else {
                    SceneSystem.Instance.MoveDir(m_MovingAngle);
                }
            }
        } else {
            SceneSystem.Instance.StopMove();
        }

        if (!m_IsDraging) {
            MoveDirection dir = getKeyboardDirection();
            if (dir != MoveDirection.Stand) {
                m_IsMoving = true;
                m_MovingAngle = (((int)dir) / 8.0f) * Mathf.PI * 2;
            } else {
                m_IsMoving = false;
            }
        }

        float delta = UnityEngine.Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(delta) > Geometry.c_FloatPrecision) {
            Utility.SendMessage("Camera", "CameraChangeDistance", -delta * 32);
        }

        if (Input.touchCount == 2) {
            var touch1 = Input.GetTouch(0);
            var touch2 = Input.GetTouch(1);
            if (touch1.phase == TouchPhase.Began) {
                m_MouseDownPos2D1 = touch1.position;
            }
            if (touch2.phase == TouchPhase.Began) {
                m_MouseDownPos2D2 = touch2.position;
            }
            if (touch1.phase == TouchPhase.Ended) {
                m_MouseDownPos2D1 = Vector2.zero;
            }
            if (touch2.phase == TouchPhase.Ended) {
                m_MouseDownPos2D2 = Vector2.zero;
            }
            if (touch1.phase == TouchPhase.Moved && touch2.phase == TouchPhase.Moved) {
                var sqr1 = (m_MouseDownPos2D1 - m_MouseDownPos2D2).sqrMagnitude;
                var sqr2 = (touch1.position - touch2.position).sqrMagnitude;
                if (sqr1 > Geometry.c_FloatPrecision && sqr2 > Geometry.c_FloatPrecision) {
                    if (sqr2 > sqr1) {
                        Utility.SendMessage("Camera", "CameraChangeDistance", -0.5f * sqr2 / sqr1);
                    } else {
                        Utility.SendMessage("Camera", "CameraChangeDistance", 0.5f * sqr1 / sqr2);
                    }
                }
            }
        }
    }

    private void JoystickOnBeginDrag(PointerEventData eventData)
    {
        m_PositionDown = GetLocalPos(eventData);
    }

    private void JoystickOnDrag(PointerEventData eventData)
    {
        Vector2 pos = GetLocalPos(eventData);
        Vector2 move = (pos - m_PositionDown);
        if (move.magnitude < JoystickButton.rectTransform.sizeDelta.x) {
            JoystickButton.transform.localPosition = move;
        } else {
            move = move.normalized * m_MoveLimit;
            JoystickButton.transform.localPosition = move;
        }
        m_IsMoving = move.magnitude > m_MoveThreshold;
        m_IsDraging = true;
        m_MovingAngle = get2DAngle(Vector2.up, move.normalized);
    }

    private void JoystickOnEndDrag(PointerEventData eventData)
    {
        m_IsMoving = false;
        m_IsDraging = false;
        JoystickButton.transform.localPosition = Vector2.zero;
    }

    private void ScreenOnDown(PointerEventData eventData)
    {
        m_MouseDownPos = Input.mousePosition;
    }
    private void ScreenOnUp(PointerEventData eventData)
    {
        var pos = Input.mousePosition;
        if (null != Camera.main && (pos-m_MouseDownPos).sqrMagnitude<0.01f) {
            Ray ray = Camera.main.ScreenPointToRay(pos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, (1 << LayerMask.NameToLayer("Terrain")) | (1 << LayerMask.NameToLayer("Default")))) {
                Vector3 hitPos = hit.point;
                SceneSystem.Instance.MoveTo(hitPos.x, hitPos.y, hitPos.z);
            }
        }
        m_MouseDownPos = Vector3.zero;
    }

    private void ScreenOnBeginDrag(PointerEventData eventData)
    {
        m_LastDragCameraPosition = GetLocalPos(eventData);
    }

    private void ScreenOnDrag(PointerEventData eventData)
    {        
        if (m_MouseDownPos.y > Screen.height / 2) {
            Vector2 pos = GetLocalPos(eventData);
            Vector2 move = (pos - m_LastDragCameraPosition);
            m_LastDragCameraPosition = pos;

            var yawDelta = move.x * 0.3f;
            Utility.SendMessage("Camera", "CameraChangeFixedYaw", yawDelta);

            var hDelta = -move.y * 0.3f;
            Utility.SendMessage("Camera", "CameraChangeHeight", hDelta);
        }
    }

    private void ScreenOnEndDrag(PointerEventData eventData)
    {
    }

    private enum MoveDirection
    {
        Stand = -1,
        Up = 0,
        UpRight,
        Right,
        DownRight,
        Down,
        DownLeft,
        Left,
        UpLeft,
    };
    private MoveDirection getKeyboardDirection()
    {
        MoveDirection moveDir = MoveDirection.Stand;
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A)) {
            moveDir = MoveDirection.UpLeft;
        } else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D)) {
            moveDir = MoveDirection.UpRight;
        } else if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S)) {
            moveDir = MoveDirection.DownLeft;
        } else if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S)) {
            moveDir = MoveDirection.DownRight;
        } else if (Input.GetKey(KeyCode.W)) {
            moveDir = MoveDirection.Up;
        } else if (Input.GetKey(KeyCode.A)) {
            moveDir = MoveDirection.Left;
        } else if (Input.GetKey(KeyCode.D)) {
            moveDir = MoveDirection.Right;
        } else if (Input.GetKey(KeyCode.S)) {
            moveDir = MoveDirection.Down;
        }
        return moveDir;
    }

    private Vector2 GetLocalPos(PointerEventData eventData)
    {
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(PanelFullScreen, eventData.position, eventData.pressEventCamera, out pos)) {
            return pos;
        }
        return Vector2.zero;
    }

    private float m_MoveThreshold = 32;
    private float m_MoveLimit = 96;
    private float m_Coefficient = 6.0f/360;

    private Vector2 m_PositionDown;
    private bool m_IsMoving;
    private bool m_IsDraging;
    private float m_MovingAngle;

    private Vector2 m_LastDragCameraPosition;

    private float m_TimeCountStart = 0.0f;
    private float INTERVAL = 0.5f;
    private float m_Fps = 0.0f;
    private int m_Count = 0;
    private float m_TimeElapse;

    private Vector3 m_MouseDownPos;
    private Vector2 m_MouseDownPos2D1;
    private Vector2 m_MouseDownPos2D2;

    public static float get2DAngle(Vector2 from, Vector2 to)
    {
        float angle = Vector2.Angle(from, to) * (Mathf.PI / 180);
        Vector3 cross = Vector3.Cross(from, to);
        if (cross.z > 0) {
            angle = Mathf.PI * 2 - angle;
        }
        return angle;
    }
}
