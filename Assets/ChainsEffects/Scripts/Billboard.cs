using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour {

    Camera m_camera;
    Quaternion rotation;
    Vector3 up;
    Vector3 forward;
    Vector3 viewDir;
    // Use this for initialization
    void Start()
    {
        m_camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        viewDir = (m_camera.transform.position - transform.position).normalized;
        up = Vector3.Cross(-viewDir, transform.right).normalized;
        forward = Vector3.Cross(transform.right, up).normalized;
        rotation = Quaternion.LookRotation(forward,up);
        transform.rotation = rotation;
    }
}
