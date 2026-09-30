using UnityEngine;

public class CameraSetup : MonoBehaviour
{
    [SerializeField] private bool _autoSetupOnAwake = true;
    [SerializeField] private Vector3 _position = new Vector3(0f, 24f, -20f);
    [SerializeField] private Vector3 _eulerAngles = new Vector3(50f, 0f, 0f);

    private void Awake()
    {
        if (_autoSetupOnAwake == false)
        {
            return;
        }

        Camera camera = Camera.main;

        if (camera == null)
        {
            Debug.LogError($"Main Camera not found for {nameof(CameraSetup)} on {gameObject.name}.", gameObject);
            return;
        }

        camera.transform.position = _position;
        camera.transform.eulerAngles = _eulerAngles;
    }
}
