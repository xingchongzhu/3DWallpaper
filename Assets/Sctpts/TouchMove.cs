using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchMove : MonoBehaviour
{
    public float distance = 60f;
    //记录上一次手机触摸位置判断用户是在左放大还是缩小手势
    private Vector2 oldPosition1;
    private Vector2 oldPosition2;
    private Vector2 FingerPos;
    private Vector2 lastPos;
    private float speed = 0.01f;
    public float scale = 30;
    private const int minY = 180;
    private const int maxY = 1000;
    Vector2 m_screenPos = new Vector2(); //记录手指触碰的位置
    public LayerMask layerMask;
    private string tag = "ground";//覆盖在场景上的plane ，设置layer层为ground，tag 为ground
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        //没有触摸  
        if (Input.touchCount <= 0)
        {
            return;
        }

        if (Input.touchCount == 1)
        {//单指操作
            if (Input.touches[0].phase == TouchPhase.Began)
            {

                lastPos = Input.touches[0].position;
            }
            else if (Input.touches[0].phase == TouchPhase.Moved && Input.touches[0].phase != TouchPhase.Stationary)
            {


                if (lastPos != Input.touches[0].position)//解决手指用力向下按也会触发TouchPhase.Moved
                {
                    if (isRayHit(Input.touches[0].position))//手指触摸到场景区域，可移动，超出范围就不移动
                    {
                        transform.Translate(new Vector3(Input.touches[0].deltaPosition.x * speed, 0, Input.touches[0].deltaPosition.y * speed));
                    }

                }
                lastPos = Input.touches[0].position;
            }

            if (Input.touches[0].phase == TouchPhase.Began)
                m_screenPos = Input.touches[0].position;   //记录手指刚触碰的位置
            if (Input.touches[0].phase == TouchPhase.Moved) //手指在屏幕上移动，移动摄像机
            {
                float translateY = Input.touches[0].deltaPosition.y * Time.deltaTime * scale;
                float translateX = Input.touches[0].deltaPosition.x * Time.deltaTime * scale;
                float diff = Mathf.Abs(translateY) - Mathf.Abs(translateX);//大于零上下运动

                if (diff > 0 && (!(transform.position.y <= minY && translateY < 0) && !(transform.position.y >= maxY && translateY > 0))) { 
                    transform.Translate(new Vector3(0, translateY, 0));
                    float y = transform.position.y;
                    if (y < minY)
                    {
                        y = minY;
                    }
                    if (y > maxY)
                    {
                        y = maxY;
                    }
                    transform.position = new Vector3(transform.position.x,y, transform.position.z);
                   
                }
            }
           
        }
        else if (Input.touchCount > 1 && Input.touches[0].phase != TouchPhase.Stationary && Input.touches[1].phase != TouchPhase.Stationary)//多指操作
        {
            //前两只手指触摸类型都为移动触摸
            if (Input.GetTouch(0).phase == TouchPhase.Moved && Input.GetTouch(1).phase == TouchPhase.Moved)
            {

                //计算出当前两点触摸点的位置
                Vector2 tempPosition1 = Input.GetTouch(0).position;
                Vector2 tempPosition2 = Input.GetTouch(1).position;
                //函数返回真为放大，返回假为缩小
                if (isEnlarge(oldPosition1, oldPosition2, tempPosition1, tempPosition2))
                {

                    //这里的数据自己任意修改，根据项目需求而定
                    distance -= scale * Time.deltaTime;
                    if (distance <= 20)
                    {
                        distance = 20;

                    }
                   // Debug.Log("放大 distance" + distance);
                    Camera.main.fieldOfView = distance;
                }
                else
                {
                    distance += scale * Time.deltaTime;

                    if (distance >= 150)
                    {
                        distance = 150;
                    }
                   // Debug.Log("缩小 distance" + distance);
                    Camera.main.fieldOfView = distance;

                }
               // Debug.Log("Camera.main.fieldOfView " + Camera.main.fieldOfView);
                //备份上一次触摸点的位置，用于对比
                oldPosition1 = tempPosition1;
                oldPosition2 = tempPosition2;

            }
        }

    }
    //函数返回真为放大，返回假为缩小
    bool isEnlarge(Vector2 oP1, Vector2 oP2, Vector2 nP1, Vector2 nP2)
    {
        //函数传入上一次触摸两点的位置与本次触摸两点的位置计算出用户的手势
        var leng1 = Mathf.Sqrt((oP1.x - oP2.x) * (oP1.x - oP2.x) + (oP1.y - oP2.y) * (oP1.y - oP2.y));
        var leng2 = Mathf.Sqrt((nP1.x - nP2.x) * (nP1.x - nP2.x) + (nP1.y - nP2.y) * (nP1.y - nP2.y));
        if (leng1 < leng2)
        {
            //放大手势
            return true;
        }
        else
        {
            //缩小手势
            return false;
        }
    }
    /// <summary>
    /// 监测射线是否碰撞到有效地面
    /// </summary>
    /// <returns><c>true</c>, if ray hit was ised, <c>false</c> otherwise.</returns>
    /// <param name="mousePosition">Mouse position.</param>
    bool isRayHit(Vector3 mousePosition)
    {

        bool isTrue = false;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hitInfo;
        /*if (Physics.Raycast(ray, out hitInfo, 500, layerMask))
        {
            print("碰到层：" + isTrue);
            if (hitInfo.collider.transform.tag == tag)
            {
                isTrue = true;
            }

        }
        else
        {

        }*/
        print("碰到点：" + isTrue);
        return isTrue;
    }
}

