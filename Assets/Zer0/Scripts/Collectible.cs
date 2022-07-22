using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    int numCollected;
    // Start is called before the first frame update
    void Start()
    {
        numCollected = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnControlColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.gameObject.tag == "collectible")
        {
            numCollected = numCollected + 1;
            Destroy(hit.collider.gameObject);
            print(numCollected);
        }
    }
}
