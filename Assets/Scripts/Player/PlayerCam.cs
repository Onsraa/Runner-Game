using UnityEngine;

public class PlayerCam : MonoBehaviour
{

    public float sensX;
    public float sensY;
    
    public Transform orientation;

    private float _xRotation;
    private float _yRotation;
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        var mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        var mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensX;

        _yRotation += mouseX;
        _xRotation -= mouseY;

        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(_xRotation, _yRotation, transform.rotation.z);
        orientation.rotation = Quaternion.Euler(0, _yRotation, orientation.rotation.z);
    }
}
