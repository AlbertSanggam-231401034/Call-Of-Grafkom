using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform player;
    public float sensitivity = 2f;
    public float distanceFromPlayer = 3f;
    public float heightOffset = 1f;

    private float rotationX = 0f;
    private float rotationY = 0f;

    void LateUpdate()
    {
        if (!player) return;

        rotationX += Input.GetAxis("Mouse X") * sensitivity;
        rotationY -= Input.GetAxis("Mouse Y") * sensitivity;
        rotationY = Mathf.Clamp(rotationY, -30f, 60f);

        Quaternion rotation = Quaternion.Euler(rotationY, rotationX, 0);
        Vector3 position = player.position + Vector3.up * heightOffset - (rotation * Vector3.forward * distanceFromPlayer);

        transform.rotation = rotation;
        transform.position = position;
    }
}