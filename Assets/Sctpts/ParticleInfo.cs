
namespace ParticleSpace
{
    public class ParticleInfo
    {

        public float radius;    //粒子旋转的半径
        public float angle;     //粒子旋转的角度
        public float time;      //用于计算ingPong函数的时间
        public float size;      //粒子大小

        public ParticleInfo(float r, float a, float t, float s)
        {
            radius = r;
            angle = a;
            time = t;
            size = s;
        }
    }
}

