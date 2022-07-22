using UnityEngine;

public class Collectible : MonoBehaviour
{
    private int _numCollected;
    
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!hit.collider.CompareTag("collectible")) return;
        
        _numCollected ++;
        Destroy(hit.collider.gameObject);
        print($"Links collected: {_numCollected}");
    }
}
