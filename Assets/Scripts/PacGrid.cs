using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 枚举：地形的集合
/// </summary>
enum LandFormEnum
{
    Block = 0,
    FlatGround = 1,
    Mountain = 2
}
public class PacGrid {

    private int x;
    internal int X
    {
        get { return x; }
        set { x = value; }
    }

    private int y;
    internal int Y
    {
        get { return y; }
        set { y = value; }
    }
    private byte landAttribute = 0;
    /// <summary>
    /// 属性：描述地形,默认为平地，0表示该节点为障碍（不可通行）
    /// </summary>
    internal byte LandAttribute
    {
        get { return landAttribute; }
        set { landAttribute = value; }
    }

    private bool pathAttribute = false;
    /// <summary>
    /// 属性：是否为路径节点，是为true，默认为false
    /// </summary>
    internal bool PathAttribute
    {
        get { return pathAttribute; }
        set { pathAttribute = value; }
    }

    private int gCostAttribute;
    /// <summary>
    /// 属性：当前网格距离起点的移动耗费
    /// </summary>
    internal int GCostAttribute
    {
        get { return gCostAttribute; }
        set { gCostAttribute = value; }
    }
    private int hCostAttribute;
    /// <summary>
    /// 属性：当前网格距离终点的移动耗费
    /// </summary>
    internal int HCostAttribute
    {
        get { return hCostAttribute; }
        set { hCostAttribute = value; }
    }
    /// <summary>
    /// 字段：当前网格的父节点
    /// </summary>
    internal PacGrid fatherGrid;
    /// <summary>
    /// 字段：当前路径的子节点
    /// </summary>
    internal PacGrid sunGrid;
}
