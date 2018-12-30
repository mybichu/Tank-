using UnityEngine;

namespace Complete
{
    public class UIDirectionControl : MonoBehaviour
    {
        // 该类用于确保world space的UI
        // 血条等元素朝向正确方向

        public bool m_UseRelativeRotation = true;      


        private Quaternion m_RelativeRotation;          


        private void Start ()
        {
            m_RelativeRotation = transform.parent.localRotation;
        }


        private void Update ()
        {
            if (m_UseRelativeRotation)
                transform.rotation = m_RelativeRotation;
        }
    }
}