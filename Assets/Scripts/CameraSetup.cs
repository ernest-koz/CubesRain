using UnityEngine;

public class CameraSetup : MonoBehaviour
{
    [SerializeField] private bool _autoSetupOnAwake = true;
    [SerializeField] private Vector3 _position = new Vector3(0f, 18f, -12f);
    [SerializeField] private Vector3 _eulerAngles = new Vector3(50f, 0f, 0f);

    private void Awake()
    {
        if (_autoSetupOnAwake == false)
            return;

        Camera camera = Camera.main;
        if (camera == null)
            return;

        camera.transform.position = _position;
        camera.transform.eulerAngles = _eulerAngles;
    }
}
