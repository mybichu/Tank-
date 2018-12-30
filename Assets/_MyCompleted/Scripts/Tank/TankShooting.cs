using UnityEngine;
using UnityEngine.UI;

namespace Complete
{
    public class TankShooting : MonoBehaviour
    {
        public int m_PlayerNumber = 1;              // 坦克玩家编号
        public Rigidbody m_Shell;                   // 子弹
        public Transform m_FireTransform;           // 坦克发射子弹的位置
        public Slider m_AimSlider;                  // 显示当前发射位置的滑条
        public AudioSource m_ShootingAudio;         // 射击音效播放器
        public AudioClip m_ChargingClip;            // 蓄力时声音
        public AudioClip m_FireClip;                // 发射时声音
        public float m_MinLaunchForce = 15f;        // 如果不蓄力，弹壳所受的力度
        public float m_MaxLaunchForce = 30f;        // 如果发射按钮保持在最大蓄力时间时给予子弹的力度
        public float m_MaxChargeTime = 0.75f;       // 最大蓄力时间


        private string m_FireButton;                // 用于发射炮弹的输入轴。
        private float m_CurrentLaunchForce;         // 当开火按钮释放时，弹壳所受到的力度
        private float m_ChargeSpeed;                // 蓄力速度
        private bool m_Fired;                       // 无论子弹是否已经启动，按下这个按钮，发射


        private void OnEnable()
        {
            // 当坦克重新开局时
            m_CurrentLaunchForce = m_MinLaunchForce;
            m_AimSlider.value = m_MinLaunchForce;
        }


        private void Start ()
        {
            m_FireButton = "Fire" + m_PlayerNumber;

            // 发射蓄力的速率是在最大充电时间之前可能的力的范围
            m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
        }


        private void Update ()
        {
            // 滑块应该具有的最小启动力的默认值
            m_AimSlider.value = m_MinLaunchForce;

            // 如果已经超过最大作用力并且炮弹尚未发射
            if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired)
            {
                // 用最大力开火
                m_CurrentLaunchForce = m_MaxLaunchForce;
                Fire ();
            }
            // 如果开火按钮刚刚开始被按下(此时子弹没有发射)
            else if (Input.GetButtonDown (m_FireButton))
            {
                //重置发射标志并重置发射力度
                m_Fired = false;
                m_CurrentLaunchForce = m_MinLaunchForce;

                // 蓄力声效开始播放
                m_ShootingAudio.clip = m_ChargingClip;
                m_ShootingAudio.Play ();
            }
            // 如果开火按钮已经被按下一段时间并且子弹没有发射
            else if (Input.GetButton (m_FireButton) && !m_Fired)
            {
                // 蓄力并更新力度
                m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;

                m_AimSlider.value = m_CurrentLaunchForce;
            }
            // 开火按钮被松开并且子弹还没有被发射
            else if (Input.GetButtonUp (m_FireButton) && !m_Fired)
            {
                // 开火
                Fire ();
            }
        }


        private void Fire ()
        {
            m_Fired = true;

            Rigidbody shellInstance =
                Instantiate (m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

            // 子弹速度
            shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward; 

            // 子弹发射时音效
            m_ShootingAudio.clip = m_FireClip;
            m_ShootingAudio.Play ();

            //重置蓄力的力度
            m_CurrentLaunchForce = m_MinLaunchForce;
        }
    }
}