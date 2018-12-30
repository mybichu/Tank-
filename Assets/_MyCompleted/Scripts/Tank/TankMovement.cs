using UnityEngine;

namespace Complete
{
    public class TankMovement : MonoBehaviour
    {
        public int m_PlayerNumber = 1;              // 用于区分坦克的编号
        public float m_Speed = 12f;                 // 前后移动速度
        public float m_TurnSpeed = 180f;            // 每秒旋转的角度
        public AudioSource m_MovementAudio;         // 用于播放引擎声音
        public AudioClip m_EngineIdling;            // 引擎空闲的声音
        public AudioClip m_EngineDriving;           // 引擎发送的声音
		public float m_PitchRange = 0.2f;           // 音高的变化范围


        private string m_MovementAxisName;          // 前后移动的轴向名称
        private string m_TurnAxisName;              // 左右旋转的轴向名称
        private Rigidbody m_Rigidbody;              // 刚体
        private float m_MovementInputValue;         // 移动的幅度，范围[-1,1]
        private float m_TurnInputValue;             // 旋转的幅度，范围[-1,1]
        private float m_OriginalPitch;              // 场景开始时的初始音高


        private void Awake ()
        {
            m_Rigidbody = GetComponent<Rigidbody> ();
        }


        private void OnEnable ()
        {
            // 坦克刚刚开始时，运动随键盘控制
            m_Rigidbody.isKinematic = false;

            // 初值
            m_MovementInputValue = 0f;
            m_TurnInputValue = 0f;
        }


        private void OnDisable ()
        {
            // 当坦克停止时，设置isKinematic以取消坦克的运动
            m_Rigidbody.isKinematic = true;
        }


        private void Start ()
        {
            // 获取移动轴和转向轴
            m_MovementAxisName = "Vertical" + m_PlayerNumber;
            m_TurnAxisName = "Horizontal" + m_PlayerNumber;

            // 初始音高
            m_OriginalPitch = m_MovementAudio.pitch;
        }


        private void Update ()
        {
            // 获得移动和转向幅度
            m_MovementInputValue = Input.GetAxis (m_MovementAxisName);
            m_TurnInputValue = Input.GetAxis (m_TurnAxisName);

            EngineAudio ();   //引擎声音
        }


        private void EngineAudio ()
        {
            // 如果没有输入，坦克静止
            if (Mathf.Abs (m_MovementInputValue) < 0.1f && Mathf.Abs (m_TurnInputValue) < 0.1f)
            {
                // 如果当前播放的driving声效，则改变声效
                if (m_MovementAudio.clip == m_EngineDriving)
                {
                    m_MovementAudio.clip = m_EngineIdling;
                    m_MovementAudio.pitch = Random.Range (m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange); //设置音高
                    m_MovementAudio.Play ();
                }
            }
            else
            {
                // 坦克正在移动，播放声效是idling，则改变声效
                if (m_MovementAudio.clip == m_EngineIdling)
                {
                    m_MovementAudio.clip = m_EngineDriving;
                    m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                    m_MovementAudio.Play();
                }
            }
        }


        private void FixedUpdate ()
        {
            Move ();
            Turn ();
        }


        private void Move ()
        {
            // 根据输入、速度和帧之间的时间，创建坦克所面对的方向的向量，该向量具有幅度。
            Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;

            // 把这个运动应用到刚体的位置
            m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
        }


        private void Turn ()
        {
            // 根据输入、速度和帧之间的时间确定要转弯的度数。
            float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;

            // 使它在y轴旋转。
            Quaternion turnRotation = Quaternion.Euler (0f, turn, 0f);

            // 把这个旋转应用到刚体的位置
            m_Rigidbody.MoveRotation (m_Rigidbody.rotation * turnRotation);
        }
    }
}