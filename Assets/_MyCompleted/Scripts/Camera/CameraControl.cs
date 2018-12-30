using UnityEngine;

namespace Complete
{
    public class CameraControl : MonoBehaviour
    {
        public float m_DampTime = 0.2f;                 // 相机重新聚焦的大致时间
        public float m_ScreenEdgeBuffer = 4f;           // 最顶部/最底部目标与屏幕边缘之间的空间
        public float m_MinSize = 6.5f;                  // 相机最小的正交尺寸
        [HideInInspector] public Transform[] m_Targets; // 摄像机需要包含的所有目标(坦克)


        private Camera m_Camera;                        // 相机引用
        private float m_ZoomSpeed;                      // 参考速度为平滑阻尼的正交尺寸
        private Vector3 m_MoveVelocity;                 // 参考速度为平滑阻尼的位置
        private Vector3 m_DesiredPosition;              // 相机正在移动的位置(期望位置)


        private void Awake ()
        {
            m_Camera = GetComponentInChildren<Camera> ();
        }


        private void FixedUpdate ()
        {
            // 相机移动
            Move ();

            // 相机改变大小与焦点
            Zoom ();
        }


        private void Move ()
        {
            // 寻找所有坦克的平均位置（中心点）
            FindAveragePosition ();

            // 平滑移动
            transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
        }

        // 寻找所有坦克的平均位置（中心点）
        private void FindAveragePosition ()
        {
            Vector3 averagePos = new Vector3 ();
            int numTargets = 0;

            // 遍历所有坦克
            for (int i = 0; i < m_Targets.Length; i++)
            {
                // 如果坦克处于 非活动 状态，不计算
                if (!m_Targets[i].gameObject.activeSelf)
                    continue;

                // 加到平均值中并增加平均目标数
                averagePos += m_Targets[i].position;
                numTargets++;
            }

            // 如果目标存在，则将位置之和除以它们的数量以求平均值。
            if (numTargets > 0)
                averagePos /= numTargets;

            // 保持y轴的值
            averagePos.y = transform.position.y;

            // 期望位置就是 找到的平均位置
            m_DesiredPosition = averagePos;
        }


        private void Zoom ()
        {
            // 根据所需的位置找到所需的大小，并平稳过渡到该大小
            float requiredSize = FindRequiredSize();
            m_Camera.orthographicSize = Mathf.SmoothDamp (m_Camera.orthographicSize, requiredSize, ref m_ZoomSpeed, m_DampTime);
        }


        private float FindRequiredSize ()
        {
            // 找到摄像机设备在其本地空间中朝向的位置。
            Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition);

            // 开始相机的尺寸为零
            float size = 0f;

            // 遍历
            for (int i = 0; i < m_Targets.Length; i++)
            {
                if (!m_Targets[i].gameObject.activeSelf)
                    continue;

                // 找到目标在相机的本地空间中的位置
                Vector3 targetLocalPos = transform.InverseTransformPoint(m_Targets[i].position);

                // 从相机本地空间的期望位置找到目标的位置
                Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;

                // 从当前尺寸和坦克与相机的“上”或“下”距离中选择最大的。
                size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.y));

                // 根据相机左边或右边的坦克选择当前尺寸和计算出的尺寸中最大的。
                size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.x) / m_Camera.aspect);
            }

            // buffer
            size += m_ScreenEdgeBuffer;

            // 确保相机的尺寸不低于最小尺寸
            size = Mathf.Max (size, m_MinSize);

            return size;
        }

        //设置初始相机位置和尺寸，可从外部调用
        public void SetStartPositionAndSize ()
        {

            FindAveragePosition ();

            // 将相机的位置设置到期望的位置
            transform.position = m_DesiredPosition;

            m_Camera.orthographicSize = FindRequiredSize ();
        }
    }
}