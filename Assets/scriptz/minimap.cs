using UnityEngine;
using UnityEngine.UI;

public class MinimapRenderer : MonoBehaviour
{
    public RawImage minimapUI;
    public int width = 40;
    public int depth = 40;

    private Texture2D minimapTexture;
    private Color[,] cellColors;   // Stores current colors
    private Color[,] baseColors;   // Stores base maze colors (walls/floor)

    void Awake()
    {
        minimapTexture = new Texture2D(width, depth);
        minimapTexture.filterMode = FilterMode.Point;

        cellColors = new Color[width, depth];
        baseColors = new Color[width, depth];

        ClearMinimap();
    }

    public void ClearMinimap()
    {
        for (int x = 0; x < width; x++)
            for (int z = 0; z < depth; z++)
                SetPixel(x, z, Color.black);

        Apply();
    }

    // Draw maze base and save colors for restoration later
    public void DrawMaze(int[,] maze)
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                Color color = maze[x, z] == 1 ? Color.white : Color.black;
                SetPixel(x, z, color);
                baseColors[x, z] = color;  // Store base color
            }
        }
        Apply();
    }

    // Mark a cell visited (green)
    public void MarkVisited(int x, int z)
    {
        if (IsInBounds(x, z))
        {
            SetPixel(x, z, Color.green);
            Apply();
        }
    }

    // Set player position (blue), clearing old player positions correctly
    public void SetPlayerPosition(int x, int z)
    {
        if (!IsInBounds(x, z)) return;

        // Restore all pixels to base color or visited (green)
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < depth; j++)
            {
                // If currently blue, restore to base or green if already visited
                if (cellColors[i, j] == Color.blue)
                {
                    // Restore to base color (white or black)
                    SetPixel(i, j, baseColors[i, j]);
                }
            }
        }

        SetPixel(x, z, Color.blue); // Set new player position to blue
        Apply();
    }

    // Set goal position (red) - call this after DrawMaze to avoid overwrite
    public void SetGoalPosition(int x, int z)
    {
        if (IsInBounds(x, z))
        {
            SetPixel(x, z, Color.red);
            Apply();
        }
    }

    private void SetPixel(int x, int z, Color color)
    {
        cellColors[x, z] = color;
        minimapTexture.SetPixel(x, z, color);
    }

    private void Apply()
    {
        minimapTexture.Apply();
        if (minimapUI != null)
            minimapUI.texture = minimapTexture;
    }

    private bool IsInBounds(int x, int z)
    {
        return x >= 0 && x < width && z >= 0 && z < depth;
    }
}
