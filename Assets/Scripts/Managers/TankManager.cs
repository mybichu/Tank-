using System;
using UnityEngine;

[Serializable]
public class TankManager
{
    public Color m_PlayerColor;          //坦克颜色     
    public Transform m_SpawnPoint;        //坦克出生点 
    [HideInInspector] public int m_PlayerNumber;    //坦克编号         
    [HideInInspector] public string m_ColoredPlayerText;  //坦克文本
    [HideInInspector] public GameObject m_Instance;       //坦克实例   
    [HideInInspector] public int m_Wins;                     


    private TankMovement m_Movement;       
    private TankShooting m_Shooting;
    private GameObject m_CanvasGameObject;


    public void Setup()
    {
        m_Movement = m_Instance.GetComponent<TankMovement>();
        m_Shooting = m_Instance.GetComponent<TankShooting>();
        m_CanvasGameObject = m_Instance.GetComponentInChildren<Canvas>().gameObject;

        m_Movement.m_PlayerNumber = m_PlayerNumber;
        m_Shooting.m_PlayerNumber = m_PlayerNumber;

        m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">PLAYER " + m_PlayerNumber + "</color>";

        MeshRenderer[] renderers = m_Instance.GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = m_PlayerColor;
        }
    }

    //设置脚本不可用
    public void DisableControl()
    {
        m_Movement.enabled = false;
        m_Shooting.enabled = false;

        m_CanvasGameObject.SetActive(false);
    }

    //设置可用
    public void EnableControl()
    {
        m_Movement.enabled = true;
        m_Shooting.enabled = true;

        m_CanvasGameObject.SetActive(true);
    }

    //重生
    public void Reset()
    {
        m_Instance.transform.position = m_SpawnPoint.position;
        m_Instance.transform.rotation = m_SpawnPoint.rotation;

        m_Instance.SetActive(false);
        m_Instance.SetActive(true);
    }
}
