using UnityEngine;

/// <summary>
/// How To Use:
/// Attach this script to empty gameobject
/// 
/// Note:
/// Uses Gizmo to draw
/// 
/// Reference:
/// https://docs.unity3d.com/ScriptReference/GL.html
/// </summary>
public class GridOverlayGizmo : MonoBehaviour
{
    public bool Show = true;
    public bool Centralized = false;
    public int GridsizeX { get; set; } = 10;
    public int GridsizeY { get; set; } = 10;
    public int GridsizeZ { get; set; } = 10;
    public float GridSizeMultipllier = 1;
    public Color mainColor = new Color(0f, 1f, 0f, 1f);

    void OnDrawGizmos()
    {
        if (Show && GridSizeMultipllier != 0)
        {
            Gizmos.color = mainColor;
            var GridPosition = transform.position;

            int starti = 0;
            int startj = 0;
            int startk = 0;
            int endi = GridsizeX;
            int endj = GridsizeY;
            int endk = GridsizeZ;
            if (Centralized)
            {
                starti = -GridsizeX / 2;
                startj = -GridsizeY / 2;
                startk = -GridsizeZ / 2;
                endi = (GridsizeX + 1) / 2;
                endj = (GridsizeY + 1) / 2;
                endk = (GridsizeZ + 1) / 2;
            }
            Vector3 startline;
            Vector3 endline;
            //x
            for (int i = starti; i < endi + 1; i++)
            {
                //y
                for (int j = startj; j < endj + 1; j++)
                {
                    //z
                    for (int k = startk; k < endk + 1; k++)
                    {
                        //x
                        if (i + 1 < endi + 1)
                        {
                            startline = new Vector3(GridPosition.x + i * GridSizeMultipllier, GridPosition.y + j * GridSizeMultipllier, GridPosition.z + k * GridSizeMultipllier);
                            endline = new Vector3(GridPosition.x + (i + 1) * GridSizeMultipllier, GridPosition.y + j * GridSizeMultipllier, GridPosition.z + k * GridSizeMultipllier);
                            Gizmos.DrawLine(startline, endline);
                        }
                        //y
                        if (j + 1 < endj + 1)
                        {
                            startline = new Vector3(GridPosition.x + i * GridSizeMultipllier, GridPosition.y + j * GridSizeMultipllier, GridPosition.z + k * GridSizeMultipllier);
                            endline = new Vector3(GridPosition.x + i * GridSizeMultipllier, GridPosition.y + (j + 1) * GridSizeMultipllier, GridPosition.z + k * GridSizeMultipllier);
                            Gizmos.DrawLine(startline, endline);
                        }
                        //z
                        if (k + 1 < endk + 1)
                        {
                            startline = new Vector3(GridPosition.x + i * GridSizeMultipllier, GridPosition.y + j * GridSizeMultipllier, GridPosition.z + k * GridSizeMultipllier);
                            endline = new Vector3(GridPosition.x + i * GridSizeMultipllier, GridPosition.y + j * GridSizeMultipllier, GridPosition.z + (k + 1) * GridSizeMultipllier);
                            Gizmos.DrawLine(startline, endline);
                        }
                    }
                    GL.End();
                }
            }
        }
    }
}
