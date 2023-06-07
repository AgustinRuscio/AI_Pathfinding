using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Components")]

    [SerializeField]
    private Transform player;

    [SerializeField]
    private Transform follow; 
    
    private new Camera camera;
    private Vector2 nearPlaneSize;


    [Header("Atributs")]

    [SerializeField]
    private Vector2 StartAngle = new Vector2(90 * Mathf.Deg2Rad, 0);

    [SerializeField]
    private float maxDistance;

    [SerializeField]
    private float sensitivity;

    private bool _onPause;

    private void Awake()
    {
        EventManager.Subscribe(EventEnum.Pause, SetOnPause);
        EventManager.Subscribe(EventEnum.Resume, SetOnPause);

        camera = GetComponent<Camera>();

        CalculateNearPlaneSize();
    }

    private void SetAngle(int newAngle) => StartAngle = new Vector2(newAngle * Mathf.Deg2Rad, 0);
    

    void Update()
    {
        if(!_onPause)
            PlayerRotationWithMouse();
    }

    void LateUpdate()
    {
        if(!_onPause)
            CameraMovement();
    }

    private void SetOnPause(params object[] isPaused) => _onPause = (bool)isPaused[0];
    

    private void PlayerRotationWithMouse()
    {
        float horizontalCam = Input.GetAxis("Mouse X");

        if (horizontalCam != 0)
        {
            StartAngle.x += horizontalCam * Mathf.Deg2Rad * sensitivity;

            player.forward = transform.forward;
            player.localRotation = Quaternion.Euler(0, player.localEulerAngles.y, 0);
        }

        float verticalCam = Input.GetAxis("Mouse Y");

        if (verticalCam != 0)
        {
            StartAngle.y += verticalCam * Mathf.Deg2Rad * sensitivity;
            StartAngle.y = Mathf.Clamp(StartAngle.y, -80 * Mathf.Deg2Rad, 80 * Mathf.Deg2Rad);
        }
    }

    private void CameraMovement()
    {
        Vector3 direction = new Vector3(
            Mathf.Cos(StartAngle.x) * Mathf.Cos(StartAngle.y),
            -Mathf.Sin(StartAngle.y),
            -Mathf.Sin(StartAngle.x) * Mathf.Cos(StartAngle.y));

        RaycastHit hit;
        float distance = maxDistance;
        Vector3[] points = GetCameraCollisionPoints(direction);

        foreach (Vector3 point in points)
        {
            if (Physics.Raycast(point, direction, out hit, maxDistance))
            {
                distance = Mathf.Min((hit.point - follow.position).magnitude, distance);
            }
        }

        transform.position = follow.position + direction * distance;
        transform.rotation = Quaternion.LookRotation(follow.position - transform.position);
    }

    private void CalculateNearPlaneSize()
    {
        float height = Mathf.Tan(camera.fieldOfView * Mathf.Deg2Rad / 2) * camera.nearClipPlane;
        float width = height * camera.aspect;

        nearPlaneSize = new Vector2(width, height);
    }

    private Vector3[] GetCameraCollisionPoints(Vector3 direction)
    {
        Vector3 position = follow.position;
        Vector3 center = position + direction * (camera.nearClipPlane + 0.2f);

        Vector3 right = transform.right * nearPlaneSize.x;
        Vector3 up = transform.up * nearPlaneSize.y;

        return new Vector3[]
        {
            center - right + up,
            center + right + up,
            center - right - up,
            center + right - up
        };
    }

    private void OnDestroy()
    {
        EventManager.Unsubscribe(EventEnum.Pause, SetOnPause);
        EventManager.Unsubscribe(EventEnum.Resume, SetOnPause);
    }
}
