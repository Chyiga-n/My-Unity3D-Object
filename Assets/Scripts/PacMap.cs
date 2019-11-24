using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacMap {
    /// <summary>
    /// 字段：地图长
    /// </summary>
    internal int LenX;
    /// <summary>
    /// 字段：地图宽
    /// </summary>
    internal int LenY;
    /// <summary>
    /// 字段：用网格节点描述的地图
    /// </summary>
    internal PacGrid[,] simpleMap;
    /// <summary>
    /// 构造方法：初始化地图
    /// </summary>
    ///<param name="x">地图长</param>
    ///<param name="y">地图宽</param>
    public PacMap(int x, int y)
    {
        LenX = x;
        LenY = y;
        simpleMap = new PacGrid[LenX, LenY];
        for (int i = 0; i < LenX; i++)
            for (int j = 0; j < LenY; j++)
            {
                simpleMap[i, j] = new PacGrid() { X = i, Y = j };
            }
    }
    private PacGrid startGrid;
    /// <summary>
    /// 属性：地图起点
    /// </summary>
    internal PacGrid StartGrid
    {
        get { return startGrid; }
        set
        {
            if (value.LandAttribute != 0)
            {
                startGrid = value;
                startGrid.GCostAttribute = 0;
                startGrid.fatherGrid = null;
                startGrid.PathAttribute = true;
                simpleMap[startGrid.X, startGrid.Y] = startGrid;
            }
            else startGrid = null;
        }
    }
    private PacGrid endGrid;
    /// <summary>
    /// 属性：地图终点
    /// </summary>
    internal PacGrid EndGrid
    {
        get { return endGrid; }
        set
        {
            endGrid = value;
            endGrid.PathAttribute = true;
            simpleMap[endGrid.X, endGrid.Y] = endGrid;
        }
    }
    /// <summary>
    /// 方法：对所有网格的地形属性进行初始化设置从而生成一张地图
    /// </summary>
    ///<param name="grid">当前节点</param>
    ///<param name="landform">地形选项</param>
    internal void MapGridSet(PacGrid grid, byte landform)
    {
        if (landform > landTypes)
            return;
        else
            grid.LandAttribute = landform;
    }
    /// <summary>
    /// 字段：地形种类数
    /// </summary>
    private int landTypes = Enum.GetValues(typeof(LandFormEnum)).Length - 1;
}
