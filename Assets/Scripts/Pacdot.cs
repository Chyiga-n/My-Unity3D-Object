using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Pacdot : MonoBehaviour {

    public GameObject gameManager;
    public GameObject Blinky;
    public GameObject Clyde;
    public GameObject Inky;
    public GameObject Pinky;
    public bool isSuperDot = false;         //超级豆标志
    public SuperDotStyle DotStyle=SuperDotStyle.ordinary;

    /// <summary>
    /// 方法：碰撞检测
    /// </summary>
    /// <param name="collision"></param>
private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Pacman" && EnemyMove.isMapInit)
        {
            if (isSuperDot == true)
            {
                GameManager.Instance.Score += 300;
                GameManager.Instance.SuperIndex++;
                ExecuteEvents.Execute<EventControlCenter>(Blinky, null, (x, y) => x.PacDotsMessageReceive(DotStyle));
                ExecuteEvents.Execute<EventControlCenter>(Clyde, null, (x, y) => x.PacDotsMessageReceive(DotStyle));
                ExecuteEvents.Execute<EventControlCenter>(Inky, null, (x, y) => x.PacDotsMessageReceive(DotStyle));
                ExecuteEvents.Execute<EventControlCenter>(Pinky, null, (x, y) => x.PacDotsMessageReceive(DotStyle));
                ExecuteEvents.Execute<EventControlCenter>(gameManager, null, (x, y) => x.PacDotsMessageReceive(DotStyle));
            }
            else
                ExecuteEvents.Execute<EventControlCenter>(gameManager, null, (x, y) => x.GameObjectMessageReceive(gameObject));
            GameManager.Instance.EatenIndex++;
            GameManager.Instance.Score += 100;
            //GameManager.Instance.RemoveDot(gameObject);
            Destroy(gameObject);
            
        }
         
    }
    /// <summary>
    /// 方法：变成超级豆
    /// </summary>
    /// <param name="g"></param>
    public void PacDotChange(GameObject g)
    {
        if (this.gameObject != null)
        {
            if (g.name == this.gameObject.name)
            {
                isSuperDot = true;
                SuperDotStyle[] DotStyles = Enum.GetValues(typeof(SuperDotStyle)) as SuperDotStyle[];
                DotStyle = DotStyles[UnityEngine.Random.Range(1,DotStyles.Length)];

                transform.localScale = new Vector3(3, 3, 3);
                ExecuteEvents.Execute<EventControlCenter>(gameManager, null, (x, y) => x.GameObjectMessageReceive(gameObject));
            }
        }
    }

}
