using System;
using UnityEngine;

namespace Complete
{
    [Serializable]
    public class TankManager
    {
        // 这个类用于管理坦克的各种设置
        // 它与GameManager类一起控制坦克的行为，以及玩家在游戏的不同阶段控制他们的坦克。

        public Color m_PlayerColor;                             // 坦克颜色
        public Transform m_SpawnPoint;                          // 出生地
        [HideInInspector] public int m_PlayerNumber;            // 玩家编号
        [HideInInspector] public string m_ColoredPlayerText;    // 一个代表玩家的字符串，其数字被着色以匹配他们的坦克
        [HideInInspector] public GameObject m_Instance;         // 坦克实例
        [HideInInspector] public int m_Wins;                    // 到目前为止该坦克赢了多少局
        

        private TankMovement m_Movement;                        
        private TankShooting m_Shooting;    
        private GameObject m_CanvasGameObject;               


        //坦克的一些设置
        public void Setup ()
        {
            // 获得组件与脚本的引用
            m_Movement = m_Instance.GetComponent<TankMovement> ();
            m_Shooting = m_Instance.GetComponent<TankShooting> ();
            m_CanvasGameObject = m_Instance.GetComponentInChildren<Canvas> ().gameObject;

            // 保持玩家编号的一致
            m_Movement.m_PlayerNumber = m_PlayerNumber;
            m_Shooting.m_PlayerNumber = m_PlayerNumber;

            // 根据坦克的颜色和玩家的数字，使用对应的颜色创建一个字符串，上面写着“PLAYER 1”等
            m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">PLAYER " + m_PlayerNumber + "</color>";

            // 获取坦克上所有组件的渲染器
            MeshRenderer[] renderers = m_Instance.GetComponentsInChildren<MeshRenderer> ();

            for (int i = 0; i < renderers.Length; i++)
            {
                // 染色
                renderers[i].material.color = m_PlayerColor;
            }
        }


        // 用于游戏中玩家不能控制坦克的阶段
        public void DisableControl ()
        {
            m_Movement.enabled = false;
            m_Shooting.enabled = false;

            m_CanvasGameObject.SetActive (false);
        }


        // 在游戏阶段使用，玩家能够控制自己的坦克
        public void EnableControl ()
        {
            m_Movement.enabled = true;
            m_Shooting.enabled = true;

            m_CanvasGameObject.SetActive (true);
        }


        // 在每轮比赛开始时使用，使坦克进入默认状态
        public void Reset ()
        {
            m_Instance.transform.position = m_SpawnPoint.position;
            m_Instance.transform.rotation = m_SpawnPoint.rotation;

            m_Instance.SetActive (false);
            m_Instance.SetActive (true);
        }
    }
}