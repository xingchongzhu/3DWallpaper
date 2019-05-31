using ParticleSpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour {

    public int number = 50000;          //粒子数量
    public float MaxSize = 0.1f;        //粒子最大半径
    public float MinSize = 0.03f;       //粒子最小半径
    public float MaxRadius = 25f;       //扩展时的最大运动半径
    public float MinRadius = 20f;        //扩展时的最小运动半径
    public float collect_MaxRadius = 15f;//收缩时的最大运动半径
    public float collect_MinRadius = 10f;//收缩时的最小运动半径
    public bool clockWise = true;       //是否顺时针运动
    public float defaultSpeed = 2f; // 默认速度
    private float realSpeed = 2f;            //运动速度
    public float pingPong = 0.02f;      //浮游偏移量

    public float scaleSpeed = 80;
    public ParticleSystem particleSys;             //粒子系统
    private ParticleSystem.Particle[] particleArr;  //粒子系统中的粒子数组
    private ParticleInfo[] particles;
    private int tier = 10;//将粒子分为10层
    public Gradient gradient;
    private float[] radius;         //扩展后的每个粒子的运动半径
    private float[] collect_radius; //收缩后每个粒子的运动半径

    //之所以申请为公共成员是为了iner可以直接进行修改
    //0代表扩展状态，1代表收缩状态
    public int isCollected = 0;
    // Use this for initialization
    void Start () {
        //私有变量的初始化
        particleArr = new ParticleSystem.Particle[number];
        particles = new ParticleInfo[number];
        particleSys = GetComponent<ParticleSystem>();
        particleSys.startColor = Color.white;
        //粒子系统属性的初始化
        particleSys.startSpeed = 0;
        particleSys.loop = false;
        particleSys.maxParticles = number;
        particleSys.Emit(number);
        particleSys.GetParticles(particleArr);

        //透明度设置
        GradientAlphaKey[] alphaKey = new GradientAlphaKey[5];
        alphaKey[0].time = 0; alphaKey[0].alpha = 1f;
        alphaKey[1].time = 0.4f; alphaKey[1].alpha = 0.4f;
        alphaKey[2].time = 0.6f; alphaKey[2].alpha = 1f;
        alphaKey[3].time = 0.9f; alphaKey[3].alpha = 0.4f;
        alphaKey[4].time = 1f; alphaKey[4].alpha = 0.9f;

        //颜色设置
        GradientColorKey[] colorKey = new GradientColorKey[2];
        colorKey[0].time = 0;
        colorKey[0].color = Color.white;
        colorKey[1].time = 1f;
        colorKey[1].color = Color.white;

        //加入设置
        gradient.SetKeys(colorKey, alphaKey);

        radius = new float[number];
        collect_radius = new float[number];
        //随机粒子的位置，大小，半径
        randomLocationAndSize();
	}

    bool lastIsTouchMove = false;
    //随机粒子的位置，大小，半径
    void randomLocationAndSize()
    {
        for (int i = 0; i < number; i++)
        {
            //扩展后的随机运动半径
            float MidRadius = (MaxRadius + MinRadius) / 2;
            float outRate = Random.Range(1f, MidRadius / MinRadius);
            float inRate = Random.Range(MaxRadius / MidRadius, 1f);
            float _radius = Random.Range(MinRadius * outRate, MaxRadius * inRate);
            radius[i] = _radius;

            //收缩后的随机运动半径
            float collect_MidRadius = (collect_MaxRadius + collect_MinRadius) / 2;
            float collect_outRate = Random.Range(1f, collect_MidRadius / collect_MinRadius);
            float collect_inRate = Random.Range(collect_MaxRadius / collect_MidRadius, 1f);
            float _collect_radius = Random.Range(collect_MinRadius * collect_outRate, collect_MaxRadius * collect_inRate);
            collect_radius[i] = _collect_radius;

            //下面保持不变
            float size = Random.Range(MinSize, MaxSize);
            float angleDgree = Random.Range(0, 360f);
            float angle = (angleDgree * Mathf.PI) / 180;

            float time = Random.Range(0, 360f);

            particles[i] = new ParticleInfo(_radius, angleDgree, time, size);
            particleArr[i].position = new Vector3(particles[i].radius * Mathf.Cos(angle), particles[i].radius * Mathf.Sin(angle), 0);
            particleArr[i].startSize = particles[i].size;
        }
        particleSys.SetParticles(particleArr, particleArr.Length);
   
    }
    //private int time = 0;
    // Update is called once per frame
    void Update () {

        if (Input.touchCount == 1) //单点触碰移动摄像机
        {
            if (Input.touches[0].phase == TouchPhase.Began)
            {
                lastIsTouchMove = false;
            }else if (Input.GetTouch(0).phase == TouchPhase.Moved )
            {
                lastIsTouchMove = true;
                
            }else if(Input.GetTouch(0).phase == TouchPhase.Ended && !lastIsTouchMove)
            {
                isCollected = 1 - isCollected;
            }
            accesslate();
        }
        if (Input.GetMouseButtonDown(1))
        {
            isCollected = 1 - isCollected;
        }

        for (int i = 0; i < number; i++)
        {
            //如果现在要进行收缩
            if (isCollected == 1 && (i < number/5*4))
            {

                //半径大的进行收缩直到小于目标半径
                if (particles[i].radius > collect_radius[i])
                {
                    particles[i].radius -= scaleSpeed * (collect_radius[i] / collect_radius[i]) * Time.deltaTime;
                }
                else
                {
                    particles[i].radius = collect_radius[i];
                    //由于半径过小就不进行浮游了
                }
            }
            //如果现在要进行扩展
            else
            {

                //半径小的进行扩展直到大于目标半径
                if (particles[i].radius < radius[i])
                {
                    particles[i].radius += scaleSpeed * (collect_radius[i] / collect_radius[i]) * Time.deltaTime;
                }
                else
                {
                    particles[i].time += Time.deltaTime;
                    //浮游
                    particles[i].radius += Mathf.PingPong(particles[i].time / MinRadius / MaxRadius, pingPong) - pingPong / 2.0f;
                }
            }
            if (clockWise)
            {
                particles[i].angle -= 0.1f;    // 顺时针旋转   
            }
            else
            {
                particles[i].angle += 0.1f;    // 顺时针旋转   
            }
           
            float light = Random.Range(0, 1);
            particleArr[i].startColor = gradient.Evaluate(light);
            if (clockWise)
            {
                particles[i].angle -= (i % tier + 1) * (realSpeed / particles[i].radius / tier);// 顺时针旋转  
            }
            else
            {
                particles[i].angle += (i % tier + 1) * (realSpeed / particles[i].radius / tier);// 顺时针旋转  
            }
            float angle = (particles[i].angle * Mathf.PI) / 180;
            particles[i].time += Time.deltaTime;
            particleArr[i].position = new Vector3(particles[i].radius * Mathf.Cos(angle), particles[i].radius * Mathf.Sin(angle), 0);
        }

        particleSys.SetParticles(particleArr, particleArr.Length);
	}

    private void accesslate()
    {
        if (Input.touches[0].phase == TouchPhase.Moved) //手指在屏幕上移动，移动摄像机
        {
            float translateY = Input.touches[0].deltaPosition.y * Time.deltaTime ;
            float translateX = Input.touches[0].deltaPosition.x * Time.deltaTime;
            float diff = Mathf.Abs(translateY) - Mathf.Abs(translateX);//小于0左右滑动
            if (diff < 0 )
            {
                if (translateX < 0)//左划
                {
                    clockWise = false;
                }
                else
                {
                    clockWise = true;
                }
                realSpeed += Mathf.Abs(translateX);
                Debug.Log("accesslate "+ translateX+ "  realSpeed " + realSpeed);
                //scaleSpeed
            }
        }
    }
}
