using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Complete
{
    public class GameManager : MonoBehaviour
    {
        public int m_NumRoundsToWin = 3;            // 某个选手赢得比赛的轮数
        public float m_StartDelay = 1.5f;           // 启动阶段和游戏阶段之间的延迟时间
        public float m_EndDelay = 1.5f;             // 游戏阶段和结束阶段之间的延迟时间
        public CameraControl m_CameraControl;  
        public Text m_MessageText;                  // 显示获胜的文本
        public GameObject m_TankPrefab;             
        public TankManager[] m_Tanks;               // 用于启用和禁用坦克不同方面的Tank Manager集合


        private int m_RoundNumber;                  // 目前正在进行的比赛回合数
        private WaitForSeconds m_StartWait;         
        private WaitForSeconds m_EndWait;           
        private TankManager m_RoundWinner;          // 本回合比赛的获胜者
        private TankManager m_GameWinner;           // 整场比赛的获胜者


        private void Start()
        {
            m_StartWait = new WaitForSeconds (m_StartDelay);
            m_EndWait = new WaitForSeconds (m_EndDelay);

            SpawnAllTanks();      // 所有坦克出生
            SetCameraTargets();   // 相机跟随

            // 一旦坦克被实例化，并且相机正在使用它们作为追随目标，开始游戏
            //协程
            StartCoroutine(GameLoop ());
        }

        //所有坦克出生
        private void SpawnAllTanks()
        {
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                m_Tanks[i].m_Instance =
                    Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
                m_Tanks[i].m_PlayerNumber = i + 1;
                m_Tanks[i].Setup();
            }
        }

        //设置相机跟随目标
        private void SetCameraTargets()
        {
            Transform[] targets = new Transform[m_Tanks.Length];

            for (int i = 0; i < targets.Length; i++)
            {
                targets[i] = m_Tanks[i].m_Instance.transform;
            }

            // 设置摄像机需要跟随的目标集合
            m_CameraControl.m_Targets = targets;
        }


        // 这是从一开始就被调用的，并将一个接一个地运行游戏的每个阶段
        private IEnumerator GameLoop ()
        {
            // 首先运行“RoundStarting”协程，直到完成之后返回
            yield return StartCoroutine (RoundStarting ());

            // 一旦“RoundStarting”协程完成，运行“RoundPlaying”协程，在完成之前不返回
            yield return StartCoroutine (RoundPlaying());

            // 一旦执行返回这里，运行“RoundEnding”协程，在完成之前不要返回
            yield return StartCoroutine (RoundEnding());

            // 此代码直到“RoundEnding”完成后才运行。此时，检查是否已经找到游戏获胜者
            if (m_GameWinner != null)
            {
                SceneManager.LoadScene (0);
            }
            else
            {
                // 如果没有获胜者，重新启动该协程，继续循环
                // 注意：这个协程不会让步。这意味着本回合的GameLoop()将结束
                StartCoroutine(GameLoop ());
            }
        }


        private IEnumerator RoundStarting ()
        {
            // 一旦回合开始，重新设置坦克并确保他们不能移动
            ResetAllTanks();
            DisableTankControl ();

            // 将相机的变焦和位置复位
            m_CameraControl.SetStartPositionAndSize ();

            // 增加一个回合
            m_RoundNumber++;
            m_MessageText.text = "ROUND " + m_RoundNumber;

            // 等待指定的时间长度，直到将控制返回到GameLoop()
            yield return m_StartWait;
        }


        private IEnumerator RoundPlaying ()
        {
            // 游戏阶段，可以控制坦克
            EnableTankControl ();

            m_MessageText.text = string.Empty;

            // 场上玩家大于1
            while (!OneTankLeft())
            {
                // 下一帧
                yield return null;
            }
        }


        private IEnumerator RoundEnding ()
        {
            // 坦克停止控制
            DisableTankControl ();

            m_RoundWinner = null;

            // 查看本回合获胜者（如果平局返回null）
            m_RoundWinner = GetRoundWinner ();

            if (m_RoundWinner != null)
                m_RoundWinner.m_Wins++;

            // 查看有没有玩家赢得整场比赛
            m_GameWinner = GetGameWinner ();

            // 根据得分以及是否存在赢家获取消息并显示
            string message = EndMessage ();
            m_MessageText.text = message;

            yield return m_EndWait;
        }


        // 这是用来检查是否有一个或更少的坦克剩余，因此回合结束
        private bool OneTankLeft()
        {
            int numTanksLeft = 0;

            for (int i = 0; i < m_Tanks.Length; i++)
            {
                if (m_Tanks[i].m_Instance.activeSelf)
                    numTanksLeft++;
            }

            return numTanksLeft <= 1;
        }


        // 找出本回合获胜者
        private TankManager GetRoundWinner()
        {
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                if (m_Tanks[i].m_Instance.activeSelf)
                    return m_Tanks[i];
            }

            // 如果没有坦克active，平局，返回null
            return null;
        }


        // 找出整场比赛获胜者
        private TankManager GetGameWinner()
        {
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                if (m_Tanks[i].m_Wins == m_NumRoundsToWin)
                    return m_Tanks[i];
            }

            return null;
        }


        // 每回合结束时返回要显示的字符串
        private string EndMessage()
        {
            // 默认情况下，当没有赢家时，默认的结束消息是平局DRAW
            string message = "DRAW!";

            //本回合有玩家获胜
            if (m_RoundWinner != null)
                message = m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND!";

            message += "\n\n\n\n";

            //遍历显示所有玩家的获胜回合数
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_Wins + " WINS\n";
            }

            if (m_GameWinner != null)
                message = m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!";

            return message;
        }


        private void ResetAllTanks()
        {
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                m_Tanks[i].Reset();
            }
        }


        private void EnableTankControl()
        {
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                m_Tanks[i].EnableControl();
            }
        }


        private void DisableTankControl()
        {
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                m_Tanks[i].DisableControl();
            }
        }
    }
}