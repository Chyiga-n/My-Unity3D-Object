using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paths {
    List<PacGrid> openList = new List<PacGrid>();
    List<PacGrid> closeList = new List<PacGrid>();
    /// <summary>
    /// 方法：判断当前节点是否在指定列表中，是则返回true
    /// </summary>
    ///<param name="x">坐标x
    ///<param name="y">坐标y
    ///<param name="list">列表
    /// <returns>Bool：在指定列表中则返回true</returns>
    protected bool IsInList(int x, int y, List<PacGrid> list)
    {
        foreach (PacGrid g in list)
        {
            if (g.X == x && g.Y == y)
                return true;
        }
        return false;
    }
    /// <summary>
    /// 方法：从指定列表里获取指定节点
    /// </summary>
    ///<param name="grid">节点
    ///<param name="list">列表
    /// <returns>Grid：返回指定节点</returns>
    protected PacGrid GetGridFromList(PacGrid grid, List<PacGrid> list)
    {
        foreach (PacGrid g in list)
        {
            if (g.X == grid.X && g.Y == grid.Y)
                return g;
        }
        return null;
    }
    /// <summary>
    /// 方法：计算G耗费
    /// </summary>
    ///<param name="x">坐标x
    ///<param name="y">坐标y
    ///<param name="sg">起点
    /// <returns>Int：G值</returns>
    protected int GetGridCostG(PacGrid CurrentGrid, PacGrid sg)
    {
        //if (CurrentGrid.fatherGrid != null)
        //    return (sg.X == CurrentGrid.X || sg.Y == CurrentGrid.Y) ? CurrentGrid.fatherGrid.GCostAttribute + 10 : CurrentGrid.fatherGrid.GCostAttribute + 21;
        //else
        //    return 0;
        if (CurrentGrid.fatherGrid != null)
            return (sg.X == CurrentGrid.X || sg.Y == CurrentGrid.Y) ? sg.GCostAttribute + 10 : sg.GCostAttribute + 14;
        else
            return 0;
    }
    /// <summary>
    /// 方法：计算H耗费
    /// </summary>
    ///<param name="x">坐标x
    ///<param name="y">坐标y
    ///<param name="eg">终点
    /// <returns>Int：H值</returns>
    protected int GetGridCostH(int x, int y, PacGrid eg)
    {
        return Math.Abs(x - eg.X) + Math.Abs(y - eg.Y);
    }
    /// <summary>
    /// 方法：从指定列表中获取F值最小的节点
    /// </summary>
    ///<param name="list">列表
    /// <returns>Grid：F值最小的节点</returns>
    protected PacGrid GetMinFFromList(List<PacGrid> list)
    {
        PacGrid rg = new PacGrid();
        if (list.Count == 0)
            return null;
        rg = list[0];
        //int tmpF = list[0].GCostAttribute + list[0].HCostAttribute;
        foreach (PacGrid g in list)
        {
            if (g.GCostAttribute + g.HCostAttribute < rg.GCostAttribute + rg.HCostAttribute)
                rg = g;
        }
        return rg;
    }
    /// <summary>
    /// 方法：检查当前节点周边的节点
    /// </summary>
    ///<param name="sg">当前节点
    ///<param name="eg">终点
    ///<param name="map">Map类的实例
    protected void CheckAround(PacGrid sg, PacGrid eg, PacMap map)//
    {
        int gridmapRow = map.LenY;//获取地图的行数
        int gridmapCol = map.LenX;//获取地图的列数
        PacGrid[,] gridmap = map.simplemap;
        for (int i = sg.X - 1; i < sg.X + 2; i++)
            for (int j = sg.Y - 1; j < sg.Y + 2; j++)//筛选出相邻点
            {
                if (i < 0 || i > gridmapCol - 1 || j < 0 || j > gridmapRow - 1)//若超出地图，pass
                    continue;
                if (gridmap[i, j].LandAttribute == 0 || IsInList(i, j, closeList) || (i == sg.X && j == sg.Y))//若为障碍点/已考虑节点/当前点，pass
                    continue;
                gridmap[i, j].HCostAttribute = GetGridCostH(i, j, eg);//计算H值(此节点到目标节点的移动损耗)
                if (!IsInList(i, j, openList))//不在open列表，则加入
                {
                    openList.Add(gridmap[i,j]);//加入open列表
                    gridmap[i, j].fatherGrid = sg;//将此节点的父节点设为当前节点
                    gridmap[i, j].GCostAttribute = GetGridCostG(gridmap[i, j], sg);//计算G值(从起点到此节点的移动损耗)
                }
                else
                {
                    int k = GetGridCostG(gridmap[i, j], sg);
                    if (gridmap[i, j].GCostAttribute > k)//如果此节点经由当前节点离起点更近，则指向当前节点
                    {
                        gridmap[i, j].GCostAttribute = k;//更新G值
                        gridmap[i, j].fatherGrid = sg;//更新父节点
                    }
                }
            }
    }
    /// <summary>
    /// 方法：寻路算法
    /// </summary>
    ///<param name="sg">起点
    ///<param name="eg">终点
    ///<param name="map">Map类的实例
    internal PacGrid Find(PacGrid sg, PacGrid eg, PacMap map)
    {
        openList.Clear();
        closeList.Clear();
        openList.Add(sg);
        PacGrid g;
        int i = 0;
        while (!IsInList(eg.X, eg.Y, openList) || openList.Count == 0)
        {
            g = GetMinFFromList(openList);
            if (g != null)
            {
                openList.Remove(g);
                closeList.Add(g);
                CheckAround(g, eg, map);
            }
            else
                return g;
            i++;
        }
        PacGrid tmpg = GetGridFromList(eg, openList);
        return Save(tmpg);
    }
    /// <summary>
    /// 方法：保存路径
    /// </summary>
    ///<param name="g">节点
    internal PacGrid Save(PacGrid g)
    {
        PacGrid TransGrid;
        while (g.fatherGrid != null)
        {
            g.PathAttribute = true;
            TransGrid = g;
            g = g.fatherGrid;
            g.sunGrid = TransGrid;
        }
        return g;
    }
}
