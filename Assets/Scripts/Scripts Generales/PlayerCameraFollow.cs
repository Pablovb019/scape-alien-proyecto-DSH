using Cinemachine;
using DilmerGames.Core.Singletons;
using UnityEngine;

public class PlayerCameraFollow : Singleton<PlayerCameraFollow>
{
    [SerializeField]
    private float mouseSensitivity = 10f;

    private CinemachineVirtualCamera cinemachineVirtualCamera;

    private void Awake()
    {
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    /* public void Rotation(Transform transform)
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up, mouseX);
    } */

    public void FollowPlayer(Transform transform)
    {
        Cursor.lockState = CursorLockMode.Locked;

        // not all scenes have a cinemachine virtual camera so return in that's the case
        if (cinemachineVirtualCamera == null) return;

        cinemachineVirtualCamera.Follow = transform;
    }
}