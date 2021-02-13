﻿using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Server.Game
{

    public struct Pos
    {
        public Pos(int y, int x) { Y = y; X = x; }
        public int Y;
        public int X;

        public static bool operator==(Pos lhs, Pos rhs)
        {
            return lhs.Y == rhs.Y && lhs.X == rhs.X;
        }

        public static bool operator!=(Pos lhs, Pos rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object obj)
        {
            return (Pos)obj == this;
        }

        public override int GetHashCode()
        {
            long value = (Y << 32) | X;           
            return value.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }

    public struct PQNode : IComparable<PQNode>
    {
        public int F;
        public int G;
        public int Y;
        public int X;

        public int CompareTo(PQNode other)
        {
            if (F == other.F)
                return 0;
            return F < other.F ? 1 : -1;
        }
    }

    public struct Vector2Int
    {
        public int x;
        public int y;

        public Vector2Int(int x, int y) { this.x = x; this.y = y; }

        public static Vector2Int up { get { return new Vector2Int(0, 1); } }
        public static Vector2Int down { get { return new Vector2Int(0, -1); } }
        public static Vector2Int left { get { return new Vector2Int(-1, 0); } }
        public static Vector2Int right { get { return new Vector2Int(1, 0); } }

        public static Vector2Int operator+(Vector2Int a, Vector2Int b)
        {
            return new Vector2Int(a.x + b.x, a.y + b.y);
        }

        public static Vector2Int operator-(Vector2Int a, Vector2Int b)
        {
            return new Vector2Int(a.x - b.x, a.y - b.y);
        }

        public float magnitude { get { return (float)Math.Sqrt(sqrMagnitude); } }
        public int sqrMagnitude { get { return (x + x * y * y); } }

        public int cellDistFromZero { get { return Math.Abs(x) + Math.Abs(y); } }
    }

    public class Map
    {
        public int MinX { get; set; }
        public int MaxX { get; set; }
        public int MinY { get; set; }
        public int MaxY { get; set; }

        public int SizeX { get { return MaxX - MinX + 1; } }
        public int SizeY { get { return MaxY - MinY + 1; } }

        bool[,] _collision;
        GameObject[,] _objects;

        public bool CanGo(Vector2Int cellPos, bool checkObjects = true)
        {
            if (cellPos.x < MinX || cellPos.x > MaxX)
                return false;

            if (cellPos.y < MinY || cellPos.y > MaxY)
                return false;

            int x = cellPos.x - MinX;
            int y = MaxY - cellPos.y;
            return !_collision[y, x] && ((!checkObjects) || _objects[y,x] == null);
        }

        public GameObject Find(Vector2Int cellPos)
        {
            if (cellPos.x < MinX || cellPos.x > MaxX)
                return null;
            if (cellPos.y < MinY || cellPos.y > MaxY)
                return null;

            int x = cellPos.x - MinX;
            int y = MaxY - cellPos.y;
            return _objects[y, x];
        }

        public bool ApplyLeave(GameObject gameObject)
        {
            if (gameObject.Room == null)
                return false;

            if (gameObject.Room.Map != this)
                return false;

            PositionInfo posInfo = gameObject.PosInfo;
            if (posInfo.PosX < MinX || posInfo.PosX > MaxX)
                return false;
            if (posInfo.PosY < MinY || posInfo.PosY > MaxY)
                return false;


            {
                int x = posInfo.PosX - MinX;
                int y = MaxY - posInfo.PosY;

                if (_objects[y, x] == gameObject)
                    _objects[y, x] = null;
            }

            return true;
        }

        public bool ApplyMove(GameObject gameObject, Vector2Int dest)
        {
            ApplyLeave(gameObject);

            if (gameObject.Room == null)
                return false;

            if (gameObject.Room.Map != this)
                return false;


            PositionInfo posInfo = gameObject.PosInfo;
            if (CanGo(dest, true) == false)
                return false;

            {
                int x = dest.x - MinX;
                int y = MaxY - dest.y;
                _objects[y, x] = gameObject;
            }

            GameObjectType type = ObjectManager.GetObjectTypeById(gameObject.Id);
            if(type == GameObjectType.Player)
            {
                Player player = (Player)gameObject;

                Zone now = gameObject.Room.GetZone(gameObject.CellPos);
                Zone after = gameObject.Room.GetZone(dest);

                if (now != after)
                {
                    now.Players.Remove(player);
                    after.Players.Add(player);
                }
            }
            else if (type == GameObjectType.Monster)
            {
                Monster monster = (Monster)gameObject;

                Zone now = gameObject.Room.GetZone(gameObject.CellPos);
                Zone after = gameObject.Room.GetZone(dest);

                if (now != after)
                {
                    now.Monsters.Remove(monster);
                    after.Monsters.Add(monster);
                }
            }
            else if (type == GameObjectType.Projectile)
            {
                Projectile projectile = (Projectile)gameObject;

                Zone now = gameObject.Room.GetZone(gameObject.CellPos);
                Zone after = gameObject.Room.GetZone(dest);

                if (now != after)
                {
                    now.Projectiles.Remove(projectile);
                    after.Projectiles.Add(projectile);
                }
            }
            

            
            


            // 실제 좌표 이동
            posInfo.PosX = dest.x;
            posInfo.PosY = dest.y;

            return true;
        }

        public void LoadMap(int mapID, string pathPrefix = "../../../../../Common/MapData")
        {
            //DestoryMap();

            string mapName = "Map_" + mapID.ToString("000");

            // collision 관련 파일 
            string text = File.ReadAllText($"{pathPrefix}/{mapName}.txt");
            StringReader reader = new StringReader(text);

            MinX = int.Parse(reader.ReadLine());
            MaxX = int.Parse(reader.ReadLine());
            MinY = int.Parse(reader.ReadLine());
            MaxY = int.Parse(reader.ReadLine());

            int xCount = MaxX - MinX + 1;
            int yCount = MaxY - MinY + 1;
            _collision = new bool[yCount, xCount];
            _objects = new GameObject[yCount, xCount];


            for (int y = 0; y < yCount; y++)
            {
                string line = reader.ReadLine();
                for (int x = 0; x < xCount; x++)
                {
                    _collision[y, x] = (line[x] == '1' ? true : false);
                }
            }
        }


        #region A* PathFinding
        int[] _deltaY = new int[] { 1, -1, 0, 0 };
        int[] _deltaX = new int[] { 0, 0, -1, 1 };
        int[] _cost = new int[] { 10, 10, 10, 10 };

        public List<Vector2Int> FindPath(Vector2Int startCellPos, Vector2Int destCellPos, bool checkObject = false)
        {
            List<Pos> path = new List<Pos>();

            // 점수 매기기
            // F = G + H
            // F = 최종 점수 (작을수록 좋음.)
            // G = 시작점에서 해당 좌표까지 비용
            // H = 목적지에서 얼마나 가까운지


            // 방문 여부
            HashSet<Pos> closeList = new HashSet<Pos>(); // CloseList

            // (y,x) 가는 길을 한 번이라도 발견했나?
            // 발견x => maxVal
            // 발견o => F = G + H
            Dictionary<Pos, int> openList = new Dictionary<Pos, int>();       

            Dictionary<Pos, Pos> parent = new Dictionary<Pos, Pos>();

            // 오픈 리스트에 있는 정보 중에서, 가장 좋은 후보를 빠르게 뽑기 위한 도구 (Heap 대신)
            PriorityQueue<PQNode> pq = new PriorityQueue<PQNode>();

            Pos pos = Cell2Pos(startCellPos);
            Pos dest = Cell2Pos(destCellPos);


            // 시작점 발견 (예약 진행)
            openList.Add(pos, 10 * (Math.Abs(dest.Y - pos.Y) + Math.Abs(dest.X - pos.X)));

            pq.Push(new PQNode() { F = (Math.Abs(dest.Y - pos.Y) + Math.Abs(dest.X - pos.X)), G = 0, Y = pos.Y, X = pos.X });
            parent.Add(pos, pos);

            while (pq.Count > 0)
            {
                // 제일 좋은 후보를 찾는다.
                PQNode pqNode = pq.Pop();
                Pos node = new Pos(pqNode.Y, pqNode.X);

                // 동일한 좌표를 여러 경로로 찾아서, 더 빠른 경로로 인하여 이미 close에 담긴경우 스킵
                if (closeList.Contains(node))
                    continue;


                // 방문.
                closeList.Add(node);
                // 목적지다.
                if (node.Y == dest.Y && node.X == dest.X)
                    break;

                // 상하좌우 등 이동할 수 있는 좌표인지 확인하고 예약
                for (int i = 0; i < _deltaY.Length; i++)
                {
                    Pos next = new Pos(node.Y + _deltaY[i], node.X + _deltaX[i]);

                    // 유효 범위를 벗어나면 스킵
                    // 벽으로 막혀도.

                    if (next.Y != dest.Y || next.X != dest.X)
                    {
                        if (CanGo(Pos2Cell(next), checkObject) == false)
                            continue;
                    }

                    if (closeList.Contains(next))
                        continue;

                    int g = 0; // node.G + _cost[i];
                    int h = 10 * ((dest.Y - next.Y) * (dest.Y - next.Y) + (dest.X - next.X) * (dest.X - next.X));

                    // 다른 경로에서 더 빠른 길 찾으면 스킵
                    int value = 0;
                    if (!openList.TryGetValue(next, out value))
                        value = Int32.MaxValue;

                    if (value < g + h)
                        continue;

                    // 예약 진행
                    if (openList.TryAdd(next, g + h) == false)
                        openList[next] = g + h;

                    pq.Push(new PQNode() { F = g + h, G = g, Y = next.Y, X = next.X });

                    if (parent.TryAdd(next, node) == false)
                        parent[next] = node;
                    
                }
            }

            return CalcCellPathFromParent(parent, dest);
        }

        List<Vector2Int> CalcCellPathFromParent(Dictionary<Pos, Pos> parent, Pos dest)
        {
            List<Vector2Int> cells = new List<Vector2Int>();


            Pos pos = dest;
            while (parent[pos] != pos)
            {
                cells.Add(Pos2Cell(pos));
                pos = parent[pos];
            }

            cells.Add(Pos2Cell(pos));
            cells.Reverse();

            return cells;
        }

        Pos Cell2Pos(Vector2Int cell)
        {
            // CellPos -> ArrayPos
            return new Pos(MaxY - cell.y, cell.x - MinX);
        }

        Vector2Int Pos2Cell(Pos pos)
        {
            // ArrayPos -> CellPos
            return new Vector2Int(pos.X + MinX, MaxY - pos.Y);
        }

        #endregion
    }

}
