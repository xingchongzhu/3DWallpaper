using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawTrajectory : MonoBehaviour
{
    public Color c1 = Color.white;
    public Color c2 = new Color(1, 1, 1, 0);
    private List<Vector3> points = new List<Vector3>();
    private LineRenderer line;
    Vector3[] path;
    private void Awake()
    {
        line = GetComponent<LineRenderer>();//获得该物体上的LineRender组件
        line.startColor = c1;
        line.endColor = c2;
        line.startWidth = 1f;
        line.endWidth = 1f;
        path = points.ToArray();
    }


    public void AddPoints()
    {
        Vector3 pt = transform.position;
        //float dif = (pt - lastPoint).magnitude;
        //Debug.Log("AddPoints points.Count = "+ points.Count+ " dif = " + dif);
        
        
        points.Add(pt);
        path = points.ToArray();//转成数组
        line.positionCount = points.Count;
        line.SetPosition(points.Count-1,pt);
    }

    public Vector3 lastPoint
    {
        get
        {
            if (points == null || points.Count < 1)
            {
                return Vector3.zero;
            }
            return (points[points.Count - 1]);
        }
    }

    void FixedUpdate()
    {

        //AddPoints();
    }
}
