using UnityEngine;
 
public class BobAndRotate : MonoBehaviour 
{
    [SerializeField] 
    private bool rotate;
    [SerializeField]
    private float rotationInDegrees = 15.0f;
    [SerializeField] 
    private bool bob;
    [SerializeField]
    private float bobMagnitude = 0.5f;
    [SerializeField]
    private float bobFrequency = 1f;

    private Vector3 _offset;

    void Start () 
    {
        _offset = transform.position;
    }
    
    void Update () 
    {
        DoBob();
        DoRotation();
    }

    private void DoBob()
    {
        if (!bob) return;
        
        var tempPos = _offset;
        tempPos.y += Mathf.Sin (Time.fixedTime * Mathf.PI * bobFrequency) * bobMagnitude;
 
        transform.position = tempPos;
    }

    private void DoRotation()
    {
        if (!rotate) return;
        
        transform.Rotate(new Vector3(0f, Time.deltaTime * rotationInDegrees, 0f), Space.World);
    }
}
