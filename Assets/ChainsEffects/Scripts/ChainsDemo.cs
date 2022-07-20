using UnityEngine;
using System.Collections;

public class ChainsDemo : MonoBehaviour {
    public float w = 60;
    public GameObject[] chainsPrefabs;
    GameObject chains;
    int id = 0;
    float maxDistance = 30;
    float minDistance = 3;
    float d;
    Vector3 pos;
    Transform m_camera;
    // Use this for initialization
    void Start()
    {
        pos = new Vector3(0, 3, 0);
        chains = Instantiate(chainsPrefabs[id]);
        chains.transform.position = pos;
        m_camera = transform.Find("Main Camera");
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float m = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(h) > 0.01f)
        {
            transform.Rotate(-Vector3.up * w * h * Time.deltaTime, Space.World);
        }

        if (Mathf.Abs(m) > 0.01f)
        {
            d = -m_camera.localPosition.z - m * 5;
            d = Mathf.Clamp(d, minDistance, maxDistance);
            m_camera.localPosition = new Vector3(0, 0, -d);
        }

        if (Input.GetButtonDown("Fire1"))
        {
            id++;
            if (id >= chainsPrefabs.Length) id = 0;
            DestroyImmediate(chains);
            chains = Instantiate(chainsPrefabs[id]);
            chains.transform.position = pos;
        }
        if (Input.GetButtonDown("Fire2"))
        {
            id--;
            if (id < 0) id = chainsPrefabs.Length - 1;
            DestroyImmediate(chains);
            chains = Instantiate(chainsPrefabs[id]);
            chains.transform.position = pos;
        }
    }
}
