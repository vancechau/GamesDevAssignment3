using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    readonly int[,] levelMap = {
        { 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 7 },
        { 2, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 4 },
        { 2, 5, 3, 4, 4, 3, 5, 3, 4, 4, 4, 3, 5, 4 },
        { 2, 6, 4, 0, 0, 4, 5, 4, 0, 0, 0, 4, 5, 4 },
        { 2, 5, 3, 4, 4, 3, 5, 3, 4, 4, 4, 3, 5, 3 },
        { 2, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 },
        { 2, 5, 3, 4, 4, 3, 5, 3, 3, 5, 3, 4, 4, 4 },
        { 2, 5, 3, 4, 4, 3, 5, 4, 4, 5, 3, 4, 4, 3 },
        { 2, 5, 5, 5, 5, 5, 5, 4, 4, 5, 5, 5, 5, 4 },
        { 1, 2, 2, 2, 2, 1, 5, 4, 3, 4, 4, 3, 0, 4 },
        { 0, 0, 0, 0, 0, 2, 5, 4, 3, 4, 4, 3, 0, 3 },
        { 0, 0, 0, 0, 0, 2, 5, 4, 4, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 2, 5, 4, 4, 0, 3, 4, 4, 0 },
        { 2, 2, 2, 2, 2, 1, 5, 3, 3, 0, 4, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 5, 0, 0, 0, 4, 0, 0, 0 },
    };

    public int pixelsPerUnit = 16;
    public int tileSize = 16;
    public GameObject tileBase;
    public GameObject pellet;
    public GameObject powerPellet;
    public Sprite[] sprites;

    // Start is called before the first frame update
    void Start()
    {
        if (sprites.Length != 8 || tileBase == null)
        {
            throw new Exception("not enough tiles provided to LevelGenerator (requires 8)");
        }

        List<int[]> mapList = new List<int[]>();
        for (int i = 0; i < levelMap.GetLength(0); i++)
        {
            List<int> current = new List<int>();
            for (int j = 0; j < levelMap.GetLength(1); j++)
            {
                current.Add(levelMap[i, j]);
            }

            List<int> columnsList = new List<int>(current);
            columnsList.Reverse();
            current.AddRange(columnsList);
            mapList.Add(current.ToArray());
        }

        List<int[]> rowsList = new List<int[]>(mapList);
        rowsList.RemoveAt(rowsList.Count - 1);
        rowsList.Reverse();
        mapList.AddRange(rowsList);

        int[][] map = mapList.ToArray();
        int multiplier = tileSize / pixelsPerUnit;

        for (int y = 0, rows = map.Length; y < rows; ++y)
        {
            int[] row = map[y];
            for (int x = 0, cols = row.Length; x < cols; ++x)
            {
                int type = row[x];
                int quadrant = (y < rows / 2 ? 0 : 2) + (x < cols / 2 ? 0 : 1);
                int[] neighbors = new int[] {
                    y + 1 == rows ? -1 : map[y+1][x],
                    x + 1 == cols ? -1 : map[y][x+1],
                    y - 1 < 0 ? -1 : map[y-1][x],
                    x - 1 < 0 ? -1 : map[y][x-1],
                };

                if (type == 5)
                {
                    GameObject p = Instantiate(pellet);
                    p.transform.position = new Vector3(x * multiplier, y * multiplier, 1);
                    continue;
                }

                if (type == 6)
                {
                    GameObject p = Instantiate(powerPellet);
                    p.transform.position = new Vector3(x * multiplier, y * multiplier, 1);
                    continue;
                }

                NewTile(type, x * multiplier, y * multiplier, RotationCalculator(type, neighbors, quadrant));
            }
        }
    }

    Vector3 RotationCalculator(int tile, int[] neighbors, int quadrant)
    {
        bool any(int check, int[] list)
        {
            foreach (int item in list)
            {
                if (item == check)
                {
                    return true;
                }
            }

            return false;
        }

        bool up(int[] matches) => any(neighbors[0], matches);
        bool down(int[] matches) => any(neighbors[2], matches);
        bool left(int[] matches) => any(neighbors[3], matches);
        bool right(int[] matches) => any(neighbors[1], matches);

        int[] outerWall = new int[] { 1, 2, 7 };
        int[] innerWall = new int[] { 3, 4 };
        int[] nil = new int[] { -1 };
        int[] innerCorner = new[] { 3 };
        int[] empty = new[] { 5, 6, 0 };

        if (tile == 0)
        {
            return new Vector3(0, 0, 0);
        }

        if (tile == 1)
        {
            if (up(outerWall) && left(outerWall)) return new Vector3(0, 0, 270);
            if (left(outerWall) && down(outerWall)) return new Vector3(0, 0, 0);
            if (down(outerWall) && right(outerWall)) return new Vector3(0, 0, 90);
            if (right(outerWall) && up(outerWall)) return new Vector3(0, 0, 180);
        }

        if (tile == 2)
        {
            if (up(outerWall) && down(outerWall)) return new Vector3(0, 0, 90);
            if (left(outerWall) && right(outerWall)) return new Vector3(0, 0, 0);

            if (right(outerWall) && left(nil)) return new Vector3(0, 0, 0);
            if (right(nil) && left(outerWall)) return new Vector3(0, 0, 0);
        }

        if (tile == 3)
        {
            if (quadrant == 0 && up(innerCorner) && right(innerWall) && left(innerWall)) return new Vector3(0, 0, 90);
            if (quadrant == 0 && up(innerWall) && left(innerWall) && !down(innerCorner)) return new Vector3(0, 0, 270);
            if (quadrant == 1 && up(innerWall) && down(innerCorner)) return new Vector3(0, 0, 270);
            if (quadrant == 1 && up(innerCorner) && down(innerWall)) return new Vector3(0, 0, 0);
            if (quadrant == 2 && up(innerCorner) && down(innerWall) && !down(innerCorner)) return new Vector3(0, 0, 90);
            if (quadrant == 2 && right(innerCorner) && down(innerWall) && !left(empty)) return new Vector3(0, 0, 0);
            if (quadrant == 3 && left(innerCorner) && right(innerWall)) return new Vector3(0, 0, 90);
            if (quadrant == 3 && up(innerWall) && down(innerCorner)) return new Vector3(0, 0, 270);
            if (quadrant == 3 && up(innerCorner) && down(innerWall)) return new Vector3(0, 0, 0);

            if (up(innerWall) && right(innerWall)) return new Vector3(0, 0, 180);
            if (left(innerWall) && up(innerWall)) return new Vector3(0, 0, 270);
            if (down(innerWall) && right(innerWall)) return new Vector3(0, 0, 90);
            if (left(innerWall) && down(innerWall)) return new Vector3(0, 0, 0);
        }

        if (tile == 4)
        {
            if (up(innerWall) && down(innerWall)) return new Vector3(0, 0, 90);
            if (right(innerWall) && left(innerWall)) return new Vector3(0, 0, 0);
            if (up(innerWall) && down(outerWall)) return new Vector3(0, 0, 90);
            if (down(innerWall) && up(outerWall)) return new Vector3(0, 0, 90);
        }

        if (tile == 7)
        {
            if (quadrant == 1) return new Vector3(0, 180, 0);
            if (quadrant == 2) return new Vector3(0, 180, 180);
            if (quadrant == 3) return new Vector3(-180, 180, 0);
        }

        return new Vector3(0, 0, 0);
    }

    void NewTile(int type, float x, float y, Vector3 rot)
    {
        if (type != 0)
        {
            GameObject tile = Instantiate(tileBase);
            tile.transform.position = new Vector3(x, y, 1);
            tile.transform.eulerAngles = rot;
            tile.GetComponent<SpriteRenderer>().sprite = sprites[type];
        }
    }
}
