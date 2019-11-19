using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour, EventControlCenter
{
    public float PacManSpeed = 0.01f;           //移动速度
    public int SensingRange = 20;                  //感应范围
    Vector2 dest = Vector2.zero;
    public GameObject BeginPoint;                //初始移动位置
    public GameObject dotmap;
    public GameObject PacMan;

    Vector3 ResurrectionPoint;                                  //复活点
    PacMap mapSample=new PacMap(31,31);        //初始化地图
    PacGrid PacmanGrid;                                         //主角

    PacGrid CurrentGrid;                                         //当前位置
    PacGrid nextGrid;                                              //下一个路径点
    Paths path=new Paths();
    public EnemyStatus enemyStatus = new EnemyStatus();

    public  int dX;
    public  int dY;
    PacGrid[,] gripMap;
    public static bool isMapInit=false;                    //初始化地图
    public int PatrolRange = 60;                              //巡逻范围
    private Rigidbody2D enemyRigidbody2d;
    private SpriteRenderer spriteRender;
    private bool isReset = false;
    private bool isChase = true;
    private bool isEat = true;
    private EnemyStatus LastEnemyStatus = new EnemyStatus();

    void Start()
    {
        enemyRigidbody2d = GetComponent<Rigidbody2D>();
        ResurrectionPoint = this.transform.position;       //记录复活点
        spriteRender=GetComponent<SpriteRenderer>();
        //设置障碍          
        foreach (Transform child in dotmap.transform)
            mapSample.MapGridSet(mapSample.simplemap[(int)child.transform.position.x, (int)child.transform.position.y], 1);


        EnemyReset();

        gripMap = mapSample.simplemap;

        isMapInit = true;
    }
    /// <summary>
    /// 方法：怪物归元回到起点
    /// </summary>
    private void EnemyReset()
    {
        dest = BeginPoint.transform.position;
        dX = (int)dest.x;
        dY = (int)dest.y;
        nextGrid = new PacGrid() { X = dX, Y = dY };
    }
    private void FixedUpdate()
    {
        if (transform.position != PacMan.transform.position&&enemyStatus!=EnemyStatus.Still)
        {
            if (isReset == false)
            {
                Vector2 temp = Vector2.MoveTowards(transform.position, dest, PacManSpeed);
                enemyRigidbody2d.MovePosition(temp);

                if ((Vector2)transform.position == dest)
                {
                    if (dest.x == dX && dest.y == dY)
                    {
                        if (nextGrid.sunGrid != null)
                            nextGrid = nextGrid.sunGrid;
                        else
                        {
                            Debug.Log("到头了");
                            //PacGrid gg = PatrolTarget(nextGrid, PatrolRange);
                            //nextGrid = LookAt(nextGrid, gg);
                            CarryOut(enemyStatus);
                        }
                        dX = nextGrid.X;
                        dY = nextGrid.Y;

                        if (gripMap[dX, dY].LandAttribute != 0)
                            dest = new Vector2(dX, dY);
                    }
                    if (isChase == true)
                    {
                        if (GetDistance(nextGrid, PacmanGrid) <= SensingRange)
                        {
                            enemyStatus = EnemyStatus.Chase;
                        }
                        else
                        {
                            enemyStatus = EnemyStatus.Patrol;
                        }
                    }
                }
                Vector2 dir = dest - (Vector2)transform.position;
                GetComponent<Animator>().SetFloat("DirX", dir.x);
                GetComponent<Animator>().SetFloat("DirY", dir.y);
            }
            else
            {//归元
                isReset = false;
                EnemyReset();
            }

        }

        PacmanGrid = new PacGrid() { X = (int)PacMan.transform.position.x, Y = (int)PacMan.transform.position.y };

      
        if (LastEnemyStatus != enemyStatus)
        {
            CarryOut(enemyStatus);
        }
        LastEnemyStatus = enemyStatus;
    }
    /// <summary>
    /// 方法：根据怪物状态开始新一轮寻路
    /// </summary>
    ///<param name="ES">怪物状态
    private void CarryOut(EnemyStatus ES)
    {
        switch (ES)
        {
            case EnemyStatus.Patrol:
                Patrol();
                break;
            case EnemyStatus.Chase:
                Chase();
                break;
            case EnemyStatus.Flee:
                Flee();
                break;
            case EnemyStatus.Still:
                Still();
                break;
        }
    }

    /// <summary>
    ///方法：寻找路径
    /// <summary>
    ///<param name="sg">寻路起点
    ///<param name="eg">寻路终点
    private PacGrid LookAt(PacGrid sg,PacGrid eg)
    {
        ClearPath();
        sg.LandAttribute=1;
        mapSample.StartGrid = sg;
        eg.LandAttribute = 1;
        mapSample.EndGrid = eg;
        //寻路
        return path.Find(mapSample.StartGrid, mapSample.EndGrid, mapSample);
    }

    /// <summary>
    ///方法：巡逻范围内随机寻点
    /// <summary>
    ///<param name="sg">当前位置
    ///<param name="range">巡逻范围
    private PacGrid PatrolTarget(PacGrid sg,int range)
    {
        List<PacGrid> gripPath = isInRange(sg,gripMap,range);
        return gripPath[UnityEngine.Random.Range(0, gripPath.Count) - 1];
    }
    /// <summary>
    ///方法：巡逻范围内寻找躲避点
    /// <summary>
    ///<param name="sg">当前位置
    ///<param name="Target">目标位置
    ///<param name="range">巡逻范围
    private PacGrid DodgePoint(PacGrid sg,PacGrid Target, int range)
    {
        List<PacGrid> gripPath = isInRange(sg, gripMap,range);
        PacGrid rg = gripPath[0];
        foreach(PacGrid g in gripPath)
        {
            if (GetDistance(Target, g) > GetDistance(Target, rg))
                rg = g;
        }
        return rg;
    }
    /// <summary>
    ///方法：计算两个格子的路程
    /// <summary>
    /// <param name="sg">起点
    /// <param name="sg">终点
    protected int GetDistance(PacGrid sg, PacGrid eg)
    {
        return (int)Math.Abs(sg.X - eg.X) + (int)Math.Abs(sg.Y -eg.Y);
    }
    /// <summary>
    ///方法：寻找巡逻范围内所有路径点
    /// <summary>
    /// <param name="sg">当前位置
    /// <param name="gripMap">网格路径图
    /// <param name="range">巡逻范围
    private List<PacGrid> isInRange(PacGrid sg,PacGrid[,] gripMap,int range)
    {
        List<PacGrid> gripPath = new List<PacGrid>();
        foreach (PacGrid g in gripMap)
        {
            if (g.LandAttribute==1&&GetDistance(sg,g)<range)
                gripPath.Add(g);
        }
        return gripPath;
    }
    /// <summary>
    ///方法：巡逻寻路
    /// <summary>
    private void Patrol()
    {
        CurrentGrid = nextGrid;
        //if (nextGrid.sunGrid != null)
        //    nextGrid = nextGrid.sunGrid;
        //else
        //{
            PacGrid gg = PatrolTarget(CurrentGrid, PatrolRange);
            nextGrid = LookAt(CurrentGrid, gg);
        //}
    }
    /// <summary>
    ///方法：追击寻路
    /// <summary>
    private void Chase()
    {
        CurrentGrid = nextGrid;
        nextGrid = LookAt(CurrentGrid, PacmanGrid);
        //if (nextGrid.sunGrid != null)
        //    nextGrid = nextGrid.sunGrid;
    }
    /// <summary>
    ///方法：逃跑寻路
    /// <summary>
    private void Flee()
    {
        CurrentGrid = nextGrid;
        PacGrid gg = DodgePoint(CurrentGrid, PacmanGrid, SensingRange);
        nextGrid = LookAt(CurrentGrid, gg);
        //if (nextGrid.sunGrid != null)
        //    nextGrid = nextGrid.sunGrid;
    }
    /// <summary>
    ///方法：寻路停止
    /// <summary>
    private void Still()
    {

    }
    /// <summary>
    ///方法：路径记忆清除
    /// <summary>
    private void ClearPath()
    {
        PacGrid bg;
        while (nextGrid.sunGrid != null)
            nextGrid = nextGrid.sunGrid;

        while (nextGrid.fatherGrid != null)
        {
            bg = nextGrid;
            nextGrid = nextGrid.fatherGrid;
            bg.fatherGrid = null;
        }
        while (nextGrid.sunGrid != null)
        {
            bg = nextGrid;
            nextGrid = nextGrid.sunGrid;
            bg.sunGrid = null;
        }
    }
   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Pacman" )
        {
            if (enemyStatus == EnemyStatus.Flee)
            {
                transform.position = ResurrectionPoint;
                isReset = true;
                GameManager.Instance.Score += 200;
            }
            else if(isEat)
            {
                GameManager.Instance.PacmanAlive = false;
            }
        }
    }
    public void GameObjectMessageReceive(GameObject g)
    {
        
    }
    /// <summary>
    ///方法：接收增益状态信息并改变怪物状态
    /// <summary>
    ///<param name="sds">被吃豆子提供的增益状态
    public void PacDotsMessageReceive(SuperDotStyle sds)
    {
        switch (sds)
        {
            case SuperDotStyle.ordinary:
                isChase = true;
                isEat = true;
                enemyStatus = EnemyStatus.Patrol;
                spriteRender.color= new Color(1.0f,1.0f,1.0f,1.0f);
                break;
            case SuperDotStyle.Still:
                enemyStatus = EnemyStatus.Still;
                isChase = false;
                isEat = true;
                spriteRender.color = new Color(1.0f, 1.0f, 1.0f,1.0f);
                break;
            case SuperDotStyle.Strengthen:
                enemyStatus = EnemyStatus.Flee;
                isChase = false;
                isEat = false;
                spriteRender.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
                break;
            case SuperDotStyle.Invincible:
                enemyStatus = EnemyStatus.Patrol;
                isChase = false;
                isEat = false;
                spriteRender.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
                break;
        }
        //LastEnemyStatus = enemyStatus;
    }
}
