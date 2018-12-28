using UnityEngine;
using System.Collections;

public class RoamingCamera : MonoBehaviour
{
    private Camera camModel;
    //private Vector2 mouseDownPosition;
    private Vector2 lastMousePosition;
    private Vector3 eulerAngle;

    public void Start()
    {
        camModel = Camera.main;
        camModel.gameObject.transform.position = Vector3.back * 30 + Vector3.up * 20;
        camModel.gameObject.transform.rotation = Quaternion.AngleAxis(-30, Vector3.left);
        eulerAngle = camModel.gameObject.transform.eulerAngles;
    }
    public void OnGUI()
    {
        Event e = Event.current;
        if (e != null) {
            if (e.type == EventType.MouseDown) {
                //mouseDownPosition = e.mousePosition;
                lastMousePosition = e.mousePosition;
            } else if (e.type == EventType.MouseDrag) {
                Vector2 offset = e.mousePosition - lastMousePosition;
                if (e.button == 2) {
                    float speed = 0.004f * camModel.transform.position.magnitude;
                    camModel.gameObject.transform.position -= camModel.gameObject.transform.TransformDirection(Vector3.right) * offset.x * speed;
                    camModel.gameObject.transform.position += camModel.gameObject.transform.TransformDirection(Vector3.up) * offset.y * speed;
                } else if (e.button == 1) {
                    float speed = 0.2f;
                    eulerAngle.y += offset.x * speed;
                    eulerAngle.x += offset.y * speed;
                    camModel.gameObject.transform.rotation = Quaternion.Euler(eulerAngle);
                }
                lastMousePosition = e.mousePosition;
            } else if (e.type == EventType.ScrollWheel) {
                float speed = 0.1f * camModel.transform.position.magnitude;
                camModel.gameObject.transform.position -= camModel.gameObject.transform.TransformDirection(Vector3.forward) * e.delta.y * speed;
            }
        }
        if (Input.GetKey(KeyCode.W)) {
            float speed = 0.004f * camModel.transform.position.magnitude;
            Vector3 moveDir = camModel.gameObject.transform.TransformDirection(Vector3.forward);
            moveDir.y = 0;
            moveDir.Normalize();
            camModel.gameObject.transform.position += moveDir * speed;

        } else if (Input.GetKey(KeyCode.A)) {
            float speed = 0.004f * camModel.transform.position.magnitude;
            camModel.gameObject.transform.position -= camModel.gameObject.transform.TransformDirection(Vector3.right) * speed;

        } else if (Input.GetKey(KeyCode.S)) {
            float speed = 0.004f * camModel.transform.position.magnitude;
            Vector3 moveDir = camModel.gameObject.transform.TransformDirection(Vector3.forward);
            moveDir.y = 0;
            moveDir.Normalize();
            camModel.gameObject.transform.position -= moveDir * speed;

        } else if (Input.GetKey(KeyCode.D)) {
            float speed = 0.004f * camModel.transform.position.magnitude;
            camModel.gameObject.transform.position += camModel.gameObject.transform.TransformDirection(Vector3.right) * speed;
        }
    }
}
