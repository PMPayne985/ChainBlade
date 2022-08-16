using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace epoching.fps
{
    public class Rotate_self_around_target_vertical : MonoBehaviour
    {
        [Header("表示绕着哪个点旋转 Indicates which point to rotate around")]
        public Transform target;

        [Header("editor旋转速度 editor rotation speed")]
        public float editor_rotate_speed = 1;

        [Header("mobile旋转速度 mobile rotation speed")]
        public float mobile_rotate_speed = 1;

        [Header("最大旋转角度 Maximum rotation angle")]
        public float maxAngles = 50;

        [Header("最小旋转角度 Minimum rotation angle")]
        public float minAngles = 20;

        //水平方向的旋转 Horizontal rotation
        private float rotate_offset;

        // Update is called once per frame
        void LateUpdate()
        {
            this.rotate_offset = Input.GetAxis("Mouse Y") * -1 * this.editor_rotate_speed * Time.deltaTime;
            this.CameraArithmetic(this.rotate_offset);

            return;

            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (Input.touchCount > 0)
                {
                    foreach (Touch tou in Input.touches)
                    {
                        //找出一根没在UI上的手指 Find a finger that is not on the UI
                        if (!EventSystem.current.IsPointerOverGameObject(tou.fingerId) && tou.phase == TouchPhase.Moved)
                        {
                            this.rotate_offset = tou.deltaPosition.y;
                            this.transform.Rotate(Vector3.left, this.rotate_offset * this.mobile_rotate_speed * Time.deltaTime);
                            break;
                        }
                    }
                }
            }
            else
            {
                this.rotate_offset = Input.GetAxis("Mouse Y");
                this.transform.Rotate(Vector3.left, this.rotate_offset * this.editor_rotate_speed * Time.deltaTime);
            }
        }

        //这个函数牛逼，陈江写的，让物体绕着某个点旋转上下旋转并且限制了旋转角度 This function is awesome, written by Chen Jiang, which makes the object rotate up and down around a certain point and limits the angle of rotation
        private void CameraArithmetic(float vertical)
        {
            //相机和目标的夹角 判断当前相机是在上还是下, false在上，true为下 The angle between the camera and the target Determine whether the current camera is up or down, false is up, true is down
            vertical = Mathf.Abs(vertical) > 100 ? vertical * 0.01f : (Mathf.Abs(vertical) > 10 ? vertical * 0.1f : vertical);
            //Debug.Log(vertical);

            bool lookFromBelow = Vector3.Angle(this.transform.forward, target.transform.up * -1) > Vector3.Angle(this.transform.forward, target.transform.up);
            //Debug.Log(lookFromBelow ? "在下" : "在上");

            transform.RotateAround(target.position, this.transform.right, vertical);

            //当前目标和相机的夹角 The angle between the current target and the camera
            float forwardAngle = Vector3.Angle(target.transform.forward, this.transform.forward);

            //当上一次的RotateAround超过了限制的最大度数，则往回旋转超过的度数！！ When the last RotateAround exceeds the limit of the maximum degree, it will rotate back by the excess degree! !
            //zaixia
            if (lookFromBelow)
            {
                if (forwardAngle > this.minAngles)
                {
                    transform.RotateAround(target.position, this.transform.right, forwardAngle - this.minAngles);
                }
            }
            //zaishang
            else
            {
                if (forwardAngle > this.maxAngles)
                {
                    transform.RotateAround(target.position, this.transform.right, this.maxAngles - forwardAngle);
                }
            }
        }

    }
}