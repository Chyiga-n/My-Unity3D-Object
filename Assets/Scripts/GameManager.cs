using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum EnemyStatus : byte
{
    Patrol = 0,//巡逻
    Chase,      //追击
    Flee,         //逃跑
    Still          //静止
}

public enum SuperDotStyle : byte
{
    ordinary=0,        //普通豆
    Still,                   //静止豆
    Strengthen,      //强化豆
    Invincible         //无敌豆
}

public class GameManager : MonoBehaviour, EventControlCenter
{

    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            return _instance;
        }
    }

    public GameObject PacMan;
    public GameObject Blinky;
    public GameObject Clyde;
    public GameObject Inky;
    public GameObject Pinky;
    public GameObject StartCutDownPrefab;       //开始倒计时
    public GameObject GameOverPrefab;    
    public GameObject WinPrefab;
    public GameObject StartPanel;                    //开始界面
    public GameObject GamePanel;                //游戏计分界面
    public GameObject StatePanel;                //增益状态显示界面
    public GameObject StartClip;                //开始音乐

    public float SuperDotTime = 10.0f;       //生成超级豆的时间间隔
    public float SuperTime = 5.0f;       //超级进化持续时间
    public int SuperIndex = 0;
    public bool OneClick = false;
    public bool PacmanAlive = true;
    public bool GameOver = false;

    public int Score = 0;               //分数
    public int EatenIndex = 0;       //已吃豆子的数量
    public int PacdotIndex = 0;       //豆子总量

    public Text StateText;               //增益状态text
    public Text EatenText;               //已吃豆子text
    public Text ScoreText;               //分数text
    public Text RemainText;           //剩余豆子text

    private List<GameObject> PacdotAlive = new List<GameObject>();     //场上存在的豆子（未强化）
	// Use this for initialization
	void Start () {
        GameStateControl(false);
        //Invoke("CreateSuperDot",SuperDotTime);//延迟生成超级豆
	}
    /// <summary>
    /// 方法：开始按钮
    /// </summary>
    public void OnStartButton()
    {
        if (OneClick == false)
        {
            StartCoroutine(PlayStartCutDown());
            StartPanel.SetActive(false);
            OneClick = true;
        }
    }
    /// <summary>
    /// 方法：结束按钮
    /// </summary>
    public void OnExitButton()
    {
        Application.Quit();
    }
    /// <summary>
    /// 方法：播放开始倒计时并开始游戏
    /// </summary>
    IEnumerator PlayStartCutDown()
    {
        GameObject g = Instantiate(StartCutDownPrefab);
        yield return new WaitForSeconds(1.8f);
        Destroy(g);
        GamePanel.SetActive(true);
        GameStateControl(true);
        Invoke("CreateSuperDot", SuperDotTime);//延迟生成超级豆
    }
	
	void Update () {
        if ((PacmanAlive == false || EatenIndex == PacdotIndex) && GameOver==false)
        {
            if (PacmanAlive)
            {
                Instantiate(WinPrefab);
            }
            else
            {
                Instantiate(GameOverPrefab);
                PacmanAlive = true;
            }
            GamePanel.SetActive(false);
            StopAllCoroutines();
            CancelInvoke();
            GameStateControl(false);
            StateText.text = "按任意键继续。。。";
            GameOver = true;
        }
        if (GameOver)
        {
            if (Input.anyKeyDown)
            {
                GameOver = false;
                SceneManager.LoadSceneAsync(0);
            }
        }
        if (GamePanel.activeInHierarchy)
        {
            RemainText.text = "Remain：\n\n"+(PacdotIndex-EatenIndex);
            EatenText.text = "Eaten：\n\n" +EatenIndex;
            ScoreText.text = "Score：\n\n" +Score;
        }
	}

    private void Awake()
    {
        _instance = this;
        foreach (Transform t in GameObject.Find("Pacdots").transform)//初始化数组
        {
            PacdotAlive.Add(t.gameObject);
        }
        PacdotIndex = gameObject.transform.childCount;
    }
    /// <summary>
    /// 方法：更新豆子数组
    /// </summary>
    ///<param name="g">被吃掉的豆子
    public void RemoveDot(GameObject g)
    {
        PacdotAlive.Remove(g);
    }
    /// <summary>
    /// 方法：生成超级豆
    /// </summary>
    public void CreateSuperDot()
    {
        if (PacdotAlive.Count>=1)
        {
            int tempIndex = Random.Range(0, PacdotAlive.Count);     //随机选取场上豆子
            BroadcastMessage("PacDotChange", PacdotAlive[tempIndex]);       //通知生成
            Invoke("CreateSuperDot", SuperDotTime);//延迟生成下一个超级豆
        }
    }
    /// <summary>
    /// 方法：接收豆子消息
    /// </summary>
    ///<param name="g">被吃掉的豆子
    public void GameObjectMessageReceive(GameObject g)
    {
        RemoveDot(g);
    }
    /// <summary>
    /// 方法：接收增益状态并显示
    /// </summary>
    ///<param name="sds">增益状态
    public void PacDotsMessageReceive(SuperDotStyle sds)
    {
        switch (sds)
        {
            case SuperDotStyle.ordinary:
                StateText.text = "正常状态";
                break;
            case SuperDotStyle.Still:
                StateText.text = "静止状态";
                break;
            case SuperDotStyle.Strengthen:
                StateText.text = "强化状态";
                break;
            case SuperDotStyle.Invincible:
                StateText.text = "无敌状态";
                break;
        }
        Invoke("SuperTimeEnd", SuperTime);//延迟解除变身
    }
    public int TestIndex = 0;
    /// <summary>
    /// 方法：解除增益状态
    /// </summary>
    ///<param name="sds">增益状态
    private void SuperTimeEnd()
    {
        TestIndex++;
        if (TestIndex == SuperIndex && EatenIndex != PacdotIndex)
        {
            ExecuteEvents.Execute<EventControlCenter>(Blinky, null, (x, y) => x.PacDotsMessageReceive(SuperDotStyle.ordinary));
            ExecuteEvents.Execute<EventControlCenter>(Clyde, null, (x, y) => x.PacDotsMessageReceive(SuperDotStyle.ordinary));
            ExecuteEvents.Execute<EventControlCenter>(Inky, null, (x, y) => x.PacDotsMessageReceive(SuperDotStyle.ordinary));
            ExecuteEvents.Execute<EventControlCenter>(Pinky, null, (x, y) => x.PacDotsMessageReceive(SuperDotStyle.ordinary));
            StateText.text = "正常状态";
        }
    }

    /// <summary>
    /// 方法：控制游戏状态
    /// </summary>
    ///<param name="bStartPause">游戏状态
    private void GameStateControl(bool bStartPause)
    {
        PacMan.GetComponent<PacManMove>().enabled = bStartPause;
        Blinky.GetComponent<EnemyMove>().enabled = bStartPause;
        Clyde.GetComponent<EnemyMove>().enabled = bStartPause;
        Inky.GetComponent<EnemyMove>().enabled = bStartPause;
        Pinky.GetComponent<EnemyMove>().enabled = bStartPause;
    }
}
