using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface EventControlCenter : IEventSystemHandler
{
    /// <summary>
    /// 方法：游戏对象信息传递
    /// </summary>
    void GameObjectMessageReceive(GameObject g);
    /// <summary>
    /// 方法：判断型消息传递
    /// </summary>
    void PacDotsMessageReceive(SuperDotStyle sds);

    
}
