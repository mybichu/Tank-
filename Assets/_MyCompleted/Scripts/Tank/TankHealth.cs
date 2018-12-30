using UnityEngine;
using UnityEngine.UI;

namespace Complete
{
    public class TankHealth : MonoBehaviour
    {
        public float m_StartingHealth = 100f;               // 总血量
        public Slider m_Slider;                             // 血条
        public Image m_FillImage;                           // 血条填充图像
        public Color m_FullHealthColor = Color.green;       // 满血时颜色为绿色
        public Color m_ZeroHealthColor = Color.red;         // 没血时颜色为红色
        public GameObject m_ExplosionPrefab;                // 坦克爆炸时的特效
        
        
        private AudioSource m_ExplosionAudio;               // 坦克爆炸音效播放器
        private ParticleSystem m_ExplosionParticles;        // 坦克爆炸时粒子系统
        private float m_CurrentHealth;                      // 当前血量
        private bool m_Dead;                                // 是否死亡


        private void Awake ()
        {
            m_ExplosionParticles = Instantiate (m_ExplosionPrefab).GetComponent<ParticleSystem> ();

            m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource> ();

            // 禁用预制件，以便可以在需要时激活它
            m_ExplosionParticles.gameObject.SetActive (false);
        }


        private void OnEnable()
        {
            // 当坦克启用时，重置坦克的健康状况以及它是否已经死亡
            m_CurrentHealth = m_StartingHealth;
            m_Dead = false;

            // 更新血条的值和颜色
            SetHealthUI();
        }


        public void TakeDamage (float amount)
        {
            // 减少当前健康造成的伤害量
            m_CurrentHealth -= amount;

            SetHealthUI ();

            if (m_CurrentHealth <= 0f && !m_Dead)
            {
                OnDeath ();
            }
        }


        private void SetHealthUI ()
        {
            m_Slider.value = m_CurrentHealth;

            // 基于当前开始健康的百分比，在选择的颜色之间插入条形图的颜色
            m_FillImage.color = Color.Lerp (m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
        }


        private void OnDeath ()
        {
            // 设置标志，以便该函数只被调用一次
            m_Dead = true;

            // 将实例化的爆炸预制件移动到坦克的位置并打开它。
            m_ExplosionParticles.transform.position = transform.position;
            m_ExplosionParticles.gameObject.SetActive (true);

            // 播放坦克爆炸的粒子系统
            m_ExplosionParticles.Play ();

            // 播放坦克爆炸声效果
            m_ExplosionAudio.Play();

            // 坦克状态为 非活动
            // 体会：如非必须，少用Destory()函数，通过SetActive()方式可以提高性能
            gameObject.SetActive (false);
        }
    }
}