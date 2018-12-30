using UnityEngine;

namespace Complete
{
    public class ShellExplosion : MonoBehaviour
    {
        public LayerMask m_TankMask;                  // 用于过滤爆炸的影响，蒙层设置为"Player"
        public ParticleSystem m_ExplosionParticles;   // 爆炸粒子特效
        public AudioSource m_ExplosionAudio;          // 爆炸音效
        public float m_MaxDamage = 100f;              // 如果爆炸集中在坦克上，那么造成的损害量
        public float m_ExplosionForce = 1000f;        // 在爆炸中心加到坦克上的力
        public float m_MaxLifeTime = 2f;              // 删除子弹之前以秒为单位的时间
        public float m_ExplosionRadius = 5f;          // 爆炸半径


        private void Start ()
        {
            // 如果到时候它还没有被摧毁，那么在它生命时间之后就把它摧毁
            Destroy(gameObject, m_MaxLifeTime);
        }


        private void OnTriggerEnter (Collider other)
        {
            // 收集一个球体中从弹壳当前位置到爆炸半径范围内的所有碰撞器。
            Collider[] colliders = Physics.OverlapSphere (transform.position, m_ExplosionRadius, m_TankMask);

            // 遍历
            for (int i = 0; i < colliders.Length; i++)
            {
                // 找到他们身上的刚体组件
                Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody> ();

                // 如果没有刚体，略过该物体
                if (!targetRigidbody)
                    continue;

                // 给碰撞体施加力
                targetRigidbody.AddExplosionForce (m_ExplosionForce, transform.position, m_ExplosionRadius);

                TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth> ();

                if (!targetHealth)
                    continue;

                // 根据目标与炮弹的距离来计算目标应该受到的伤害量
                float damage = CalculateDamage (targetRigidbody.position);

                // 对坦克造成伤害
                targetHealth.TakeDamage (damage);
            }

            // 原来粒子系统是子弹的子物体，现在将其脱离
            m_ExplosionParticles.transform.parent = null;

            // 播放粒子特效
            m_ExplosionParticles.Play();

            // 播放声效
            m_ExplosionAudio.Play();

            // 播放完特效，将其删除
            Destroy (m_ExplosionParticles.gameObject, m_ExplosionParticles.duration);

            // 删除子弹
            Destroy (gameObject);
        }


        private float CalculateDamage (Vector3 targetPosition)
        {
            // 计算子弹到目标的距离矢量
            Vector3 explosionToTarget = targetPosition - transform.position;

            // 计算距离矢量的长度
            float explosionDistance = explosionToTarget.magnitude;

            // 计算目标离开的最大距离（爆炸半径）的比例
            float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;

            //相对伤害转化为绝对伤害
            float damage = relativeDistance * m_MaxDamage;

            damage = Mathf.Max (0f, damage);

            return damage;
        }
    }
}