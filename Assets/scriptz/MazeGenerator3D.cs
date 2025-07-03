using UnityEngine;
using UnityEngine.UI;

public class MazeGenerator3D : MonoBehaviour
{
    public int width = 40;
    public int depth = 40;

    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public GameObject goalPrefab;

    public RawImage minimapUI;

    private int[,] maze;
    private bool[,] visited;

    private Texture2D minimapTexture;

    private GameObject mazeParent;
    private GameObject goalInstance;

    public float cellSize = 4f; // Made public to be accessible outside this class

    void Start()
    {
        RegenerateMaze();
    }

    public void RegenerateMaze()
    {
        GenerateMaze();
        DrawMaze();
        GenerateMinimap();
    }

    private void GenerateMaze()
    {
        maze = new int[width, depth];
        visited = new bool[width, depth];

        for (int x = 0; x < width; x++)
            for (int z = 0; z < depth; z++)
            {
                maze[x, z] = 0;
                visited[x, z] = false;
            }

        RecursiveBacktrack(1, 1);

        // Clear start area
        maze[1, 1] = 1;
        if (IsInBounds(1, 2)) maze[1, 2] = 1;
        if (IsInBounds(2, 1)) maze[2, 1] = 1;

        // Clear goal area
        maze[width - 1, depth - 1] = 1;
        maze[width - 2, depth - 1] = 1;
    }

    private void RecursiveBacktrack(int x, int z)
    {
        visited[x, z] = true;
        maze[x, z] = 1;

        int[] dx = { 0, 0, -2, 2 };
        int[] dz = { -2, 2, 0, 0 };

        for (int i = 0; i < 4; i++)
        {
            int r = Random.Range(i, 4);
            (dx[i], dx[r]) = (dx[r], dx[i]);
            (dz[i], dz[r]) = (dz[r], dz[i]);
        }

        for (int i = 0; i < 4; i++)
        {
            int nx = x + dx[i];
            int nz = z + dz[i];

            if (IsInBounds(nx, nz) && !visited[nx, nz])
            {
                maze[(x + nx) / 2, (z + nz) / 2] = 1;
                RecursiveBacktrack(nx, nz);
            }
        }
    }

    private bool IsInBounds(int x, int z)
    {
        return x > 0 && x < width && z > 0 && z < depth;
    }

    private void DrawMaze()
    {
        if (mazeParent != null)
            Destroy(mazeParent);

        mazeParent = new GameObject("Maze");

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                Vector3 position = new Vector3(x * cellSize, 0, z * cellSize);

                if (maze[x, z] == 1)
                {
                    GameObject floor = Instantiate(floorPrefab, position, Quaternion.identity, mazeParent.transform);
                    floor.transform.localScale = new Vector3(cellSize, 0.1f, cellSize);
                }
                else
                {
                    GameObject wall = Instantiate(wallPrefab, position + Vector3.up * 3f, Quaternion.identity, mazeParent.transform);
                    wall.transform.localScale = new Vector3(cellSize, 6f, cellSize);
                }
            }
        }

        // Spawn Goal
        if (goalInstance != null)
            Destroy(goalInstance);

        Vector3 goalPosition = new Vector3((width - 2) * cellSize, 1.5f, (depth - 2) * cellSize);
        goalInstance = Instantiate(goalPrefab, goalPosition, Quaternion.identity);
        goalInstance.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
    }

    private void GenerateMinimap()
    {
        minimapTexture = new Texture2D(width, depth);
        minimapTexture.filterMode = FilterMode.Point;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                Color color = (maze[x, z] == 0) ? Color.black : Color.white;
                minimapTexture.SetPixel(x, z, color);
            }
        }

        minimapTexture.Apply();

        if (minimapUI != null)
            minimapUI.texture = minimapTexture;
    }

    public void UpdateMinimap(Vector2Int playerCell, bool markVisited)
    {
        if (!IsInBounds(playerCell.x, playerCell.y)) return;

        if (markVisited)
        {
            visited[playerCell.x, playerCell.y] = true;
            minimapTexture.SetPixel(playerCell.x, playerCell.y, Color.green);
        }

        // Clear previous player position
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                if (minimapTexture.GetPixel(x, z) == Color.blue)
                {
                    minimapTexture.SetPixel(x, z, maze[x, z] == 1 ? (visited[x, z] ? Color.green : Color.white) : Color.black);
                }
            }
        }

        minimapTexture.SetPixel(playerCell.x, playerCell.y, Color.blue);
        minimapTexture.Apply();
    }
}
