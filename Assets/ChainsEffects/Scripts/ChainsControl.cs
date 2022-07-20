using UnityEngine;
using System.Collections;

public class ChainsControl : MonoBehaviour {
    public float moveTime = 5;                //The flying time of knife.
    public float knifeVelocity = 20;          //The start velocity of knife.
    public float dragVelocity = 25;          //The velocity of knife when it is dragged.
    public float emissionRate = 10;          //The emission rate of chains.
    public float force = 0;                  //The force employed on knife.
    public float frequency = 1;              //The frequency of force.
    [Range(0, 1)]
    public float delay = 0;                 
    public int maxChainsLength = 50;
    public GameObject[] knifePrefabs;
    public GameObject[] chainPrefabs;
    ParticleSystem shuriken;
    Transform cube;                          //This is the object to emit chains.
    float startTime = 0;                     //The start time to emit chains.
    float dv = 30;                           
    float dr = 500;
    float distance = 0;
    float d;
    Vector3 velocity;
    Vector3 lastPosion;
    Vector3 mousePosition;
    GameObject knife;
    GameObject firstChain;
    GameObject[] chains;
    GameObject[] weapons;
    Camera m_camera;
    bool press = false;
    bool drag = false;
    int id = 0;                             //The index of current track.
    int num = 0;                            //The index of current type of knife.
    int chainsLength = 0;
    int start = 0;
	// Use this for initialization
    void Start()
    {
        d = 1 / (float)emissionRate;
        chains = new GameObject[maxChainsLength];
        mousePosition = Input.mousePosition;
        shuriken = gameObject.GetComponentInChildren<ParticleSystem>();
        cube = transform.root;
        m_camera = Camera.main;
    }
	
	// Update is called once per frame
	void Update () {
	 if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            id = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            id = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            id = 2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            id = 3;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            id = 4;
        }

        if (!(press || drag))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                num++;
                if (num >= chainPrefabs.Length)
                {
                    num = 0;
                }
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                num--;
                if (num < 0)
                {
                    num = chainPrefabs.Length - 1;
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            var em = shuriken.emission;
            em.enabled = true;
        }

        if (Input.GetMouseButtonUp(1))
        {
            var em = shuriken.emission;
            em.enabled = false;
        }

        if (Input.GetMouseButtonDown(0) && knife == null)
        {
            knife = Instantiate(knifePrefabs[num]);
            knife.transform.position = transform.position + transform.forward * 0.3f;
            knife.transform.rotation = transform.rotation;
            startTime = Time.time;
            velocity = knife.transform.forward * knifeVelocity;
            press = true;
        }
        if (Input.GetMouseButtonUp(0)&&knife!=null)
        {
                drag = true;
                press=false;
        }
        if (press && (Time.time - startTime > moveTime||chainsLength>=maxChainsLength))
        {
            drag = true;
            press = false;
        }

        if (Vector2.Distance(Input.mousePosition, mousePosition) > 3&&press)
        {
            drag = true;
            press = false;
        }
        mousePosition = Input.mousePosition;
        CubeControl();
	}

    void FixedUpdate()
    {
        if (press)
        {
            EmitChains();
        }

        if (drag)
        {
            DragChains();
        }
    }

    private void EmitChains()
    {
        velocity += GetForce() * Time.deltaTime;
        knife.transform.position += velocity * Time.deltaTime;
        knife.transform.rotation = Quaternion.LookRotation(velocity);
        if (chainsLength < maxChainsLength)
        {
            distance += Vector3.Distance(lastPosion, knife.transform.position);
            lastPosion = knife.transform.position;
            if (distance > d)
            {
                GameObject chain = Instantiate(chainPrefabs[num]);
                chain.transform.position = knife.transform.position - knife.transform.forward;
                chain.transform.rotation=knife.transform.rotation;
                chains[chainsLength] = chain;
                chainsLength++;
                distance = 0;
            }
        }
        if (knife.GetComponent<Impulse>().hit)
        {
            press = false;
            drag = true;
        }
    }

    private void DragChains()
    {
        if (chainsLength == 0)
        {
            DestroyImmediate(knife);
            return; 
        }
        if (chains[chainsLength - 1] == null)
        {
            move(gameObject, ref knife);
            if (Vector3.Distance(transform.position, knife.transform.position) < 0.1f)
            {
                DestroyImmediate(knife);
                chainsLength = 0;
                start = 0;
                drag = false;
            }
        }
        else
        {
            GameObject first = gameObject;
            if (Vector3.Distance(first.transform.position, chains[start].transform.position) < 0.1f)
            {
                DestroyImmediate(chains[start]);
                start++;
            }
            for (int i = start; i < chainsLength; i++)
            {
                if (chains[i] == null) continue;
                move(first, ref chains[i]);
                first = chains[i];
            }
            move(first, ref knife);
        }

    }

    private void move(GameObject first, ref GameObject next)
    {
        Vector3 direction = first.transform.position - next.transform.position;
        float l = direction.magnitude;
        direction.Normalize();
        l = Mathf.Min(l, dragVelocity * Time.deltaTime);
        next.transform.position += direction * l;
        if (direction != Vector3.zero)
        {
            next.transform.rotation = Quaternion.RotateTowards(next.transform.rotation, Quaternion.LookRotation(-direction), dr * Time.deltaTime);
        }
    }

    private Vector3 GetForce()
    {
        Vector3 currentForce = Vector3.zero;
        float t = (Time.time - startTime) / moveTime;
        if (t < delay || delay==1 || moveTime <= 0) return Vector3.zero;
        t = (t- delay) / (1 - delay);
        t *= frequency;
        switch (id)
        {
            case 0:
                currentForce = Mathf.Min(force, 15) * Vector3.down;
                break;
            case 1:
                currentForce = force * Mathf.Sin(t * 2 * Mathf.PI) * Vector3.down;
                break;
            case 2:
                currentForce = force * Mathf.Sin(t * 2 * Mathf.PI) * Vector3.right;
                break;
            case 3:
                currentForce = force * Mathf.Sin(t * 2 * Mathf.PI) * Vector3.right + force * Mathf.Sin(t * 2 * Mathf.PI + Mathf.PI * 0.5f) * Vector3.up;
                break;
            case 4:
                break;
        }
        return currentForce;
    }


    private void CubeControl()
    {
        Vector2 pos = m_camera.ScreenToViewportPoint(Input.mousePosition);
        pos = pos - new Vector2(0.5f, 0.5f);
        Vector3 direction = new Vector3(pos.x, pos.y, 0.75f);
        Vector3 up = Vector3.Cross(direction, Vector3.right);
        cube.rotation = Quaternion.LookRotation(direction, up);
    }

}
