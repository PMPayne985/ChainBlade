using UnityEngine;
using System.Collections;

public class ClearSight : MonoBehaviour
{
    [Header("player transfrom")]
    public Transform transform_player;

    [Header("Detection distance")]
    public float detection_diatance;

    [Header("alpha")]
    public float alpha;

    void Update()
    {
        //发送射线，要是检测到玩家和摄像机中间有物体，就让检测到的物体透明 Send a ray, if an object is detected between the player and the camera, make the detected object transparent
        RaycastHit[] hits;
        hits = Physics.RaycastAll(this.transform.position, (this.transform_player.position - transform.position).normalized, detection_diatance);
        Debug.DrawRay(transform.position, (this.transform_player.position-transform.position).normalized* detection_diatance, Color.red);
        foreach (RaycastHit hit in hits)
        {
            //string tag = hit.collider.gameObject.tag;
            Renderer R = hit.collider.GetComponent<Renderer>();
            if (R == null)
                return;
            AutoTransparent AT = R.GetComponent<AutoTransparent>();

            if (AT == null)// if no script is attached, attach one
            {
                AT = R.gameObject.AddComponent<AutoTransparent>();
            }
            AT.set_obj_transparent_and_change_gamelayer(alpha);
        }
    }
}

