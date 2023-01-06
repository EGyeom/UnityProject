using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapManager
{    public struct Pos
    {
        public Pos(int y, int x) { Y = y; X = x; }
        public int Y;
        public int X;
    }

    public struct PQNode : IComparable<PQNode>
    {
        public int F;
        public int G;
        public int X;
        public int Y;

        public int CompareTo(PQNode data)
        {
            if (F == data.F)
                return 0;

            return F < data.F ? 1 : -1;
        }
    }

    public Grid CurrentGrid { get; private set; }
    public int MinX { get; set;}
    public int MinY { get; set;}
    public int MaxX { get; set;}
    public int MaxY { get; set;}

    bool[,] _collision;

    public bool CanGo(Vector3Int cellPos)
    {
        if (cellPos.x < MinX || cellPos.x > MaxX)
            return false;
        if (cellPos.y < MinY || cellPos.y > MaxY)
            return false;

        int x = cellPos.x - MinX;
        int y = MaxY - cellPos.y;

        return !_collision[y,x];
    }

    public void LoadMap(int mapId)
    {
        DestroyMap();

        string mapName = "Map_" + mapId.ToString("000");
        GameObject go = Managers.Resource.Instantiate($"Map/{mapName}");
        go.name = "Map";

        GameObject collision = Util.FindChild(go, "Tilemap_Collision", true);
        if (collision != null)
            collision.SetActive(false);
        
        CurrentGrid = go.GetComponent<Grid>();

        //Collision 관련 파일
        TextAsset txt = Managers.Resource.Load<TextAsset>($"Map/{mapName}");
        StringReader reader = new StringReader(txt.text);

        MinX = int.Parse(reader.ReadLine());
        MaxX = int.Parse(reader.ReadLine());
        MinY = int.Parse(reader.ReadLine());
        MaxY = int.Parse(reader.ReadLine());

        int xCount = MaxX - MinX + 1;
        int yCount = MaxY - MinY + 1;
        _collision = new bool[yCount, xCount];

        for(int y = 0; y < yCount; y++)
        {
            string line = reader.ReadLine();
            for(int x = 0; x < xCount; x++)
            {
                _collision[y, x] = (line[x] == '1' ? true : false);
            }
        }
    }

    public void DestroyMap()
    {
        GameObject map = GameObject.Find("Map");
        if (map != null)
        {
            GameObject.Destroy(map);
            CurrentGrid = null;
        }
    }


    int[] moveX = { -1, 1, 0, 0 };
    int[] moveY = { 0, 0, -1, 1 };
    int[] cost = { 1, 1, 1, 1 };
    public List<Vector3Int> AStar(Vector3Int startCellPos, Vector3Int destCellPos, bool ignored = false)
    {
        PriorityQueue<PQNode> pq = new PriorityQueue<PQNode>();
        int yCount = MaxY - MinY + 1;
        int xCount = MaxX - MinX + 1;

        bool[,] close = new bool[yCount, xCount];
        int[,] open = new int[yCount, xCount];

        //초기화
        for (int i = 0; i < yCount; i++)
        {
            for (int j = 0; j < xCount; j++)
            {
                open[i, j] = int.MaxValue;
                close[i, j] = false;
            }
        }
        List<Vector3Int> path = new List<Vector3Int>();
        Pos cellPos = ConvertCellPos2AbsolutePos(startCellPos);
        Pos targetPos = ConvertCellPos2AbsolutePos(destCellPos);
        Pos[,] parent = new Pos[yCount, xCount];
        int h = 10 * (Math.Abs(targetPos.Y - cellPos.Y) + Math.Abs(targetPos.X - cellPos.X));
        open[cellPos.Y, cellPos.X] = h;
        pq.Push(new PQNode() { F = h, G = 0, X = cellPos.X, Y = cellPos.Y });
        parent[cellPos.Y, cellPos.X] = cellPos;

        while (pq.Count > 0)
        {
            PQNode node = pq.Pop();

            if (close[node.Y, node.X])
                continue;

            close[node.Y, node.X] = true;

            if (node.Y == targetPos.Y && node.X == targetPos.X)
                break;

            for (int i = 0; i < 4; i++)
            {
                int nextX = node.X + moveX[i];
                int nextY = node.Y + moveY[i];

                if (ignored == false || nextY != targetPos.Y && nextX != targetPos.X)
                {
                    if (CanGo(ConvertPos2AbsoluteCellPos(new Pos(nextY, nextX))) == false)
                        continue;
                }

                if (close[nextY, nextX])
                    continue;

                int g = node.G + cost[i];
                h = 10 * (Math.Abs(targetPos.Y - nextY) + Math.Abs(targetPos.X - nextX));

                if (open[nextY, nextX] < h + g)
                    continue;
                pq.Push(new PQNode() { F = g + h, G = g, X = nextX, Y = nextY });
                parent[nextY, nextX] = new Pos(node.Y, node.X);
            }
        }

        //백트래킹
        int cur_X = targetPos.X;
        int cur_Y = targetPos.Y;

        while (cur_Y != parent[cur_Y, cur_X].Y || cur_X != parent[cur_Y, cur_X].X)
        {
            path.Add(ConvertPos2AbsoluteCellPos(new Pos(cur_Y, cur_X)));
            Pos temp = parent[cur_Y, cur_X];
            cur_Y = temp.Y;
            cur_X = temp.X;
        }

        path.Add(ConvertPos2AbsoluteCellPos(new Pos(cur_Y, cur_X)));
        path.Reverse();

        return path;
    }

    Pos ConvertCellPos2AbsolutePos(Vector3Int cellPos)
    {
        return new Pos(MaxY - cellPos.y, cellPos.x - MinX);
    }

    Vector3Int ConvertPos2AbsoluteCellPos(Pos cellPos)
    {
        return new Vector3Int(MinX + cellPos.X, MaxY - cellPos.Y);
    }
}
