using UnityEngine;

/// <summary>
/// How To Use:
/// Attach this script to camera
/// 
/// Note:
/// Uses Camera OnPostRender with GL to draw
/// 
/// Reference:
/// https://docs.unity3d.com/ScriptReference/GL.html
/// </summary>
public class GridOverlay : MonoBehaviour
{
    public bool Show = true;
    public bool Centralized = false;
    public int GridsizeX = 10;
    public int GridsizeY = 10;
    public int GridsizeZ = 10;
    public float GridSizeMultipllier = 1;
    Material lineMaterial;
    public Color mainColor = new Color(0f, 1f, 0f, 1f);

    void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            var shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }
    void OnPostRender()
    {
        CreateLineMaterial();
        // set the current material
        lineMaterial.SetPass(0);//becuase GL uses current materal to draw.

        if (Show && GridSizeMultipllier != 0)
        {
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
                endi = (GridsizeX+1) / 2;
                endj = (GridsizeY+1) / 2;
                endk = (GridsizeZ+1) / 2;
            }
            //x
            for (int i = starti; i < endi + 1; i++)
            {
                //y
                for (int j = startj; j < endj + 1; j++)
                {
                    GL.Begin(GL.LINES);
                    GL.Color(mainColor);
                    //z
                    for (int k = startk; k < endk + 1; k++)
                    {
                        //x
                        if(i+1 < endi + 1)
                        {
                            GL.Vertex3(GridPosition.x+i* GridSizeMultipllier, GridPosition.y+j* GridSizeMultipllier, GridPosition.z+k* GridSizeMultipllier);
                            GL.Vertex3(GridPosition.x+(i+1)* GridSizeMultipllier, GridPosition.y+j* GridSizeMultipllier, GridPosition.z+k* GridSizeMultipllier);
                        }
                        //y
                        if(j+1 < endj + 1)
                        {
                            GL.Vertex3(GridPosition.x+i* GridSizeMultipllier, GridPosition.y+j* GridSizeMultipllier, GridPosition.z+k* GridSizeMultipllier);
                            GL.Vertex3(GridPosition.x+i* GridSizeMultipllier, GridPosition.y+(j+1)* GridSizeMultipllier, GridPosition.z+k* GridSizeMultipllier);
                        }
                        //z
                        if(k+1 < endk + 1)
                        {
                            GL.Vertex3(GridPosition.x+i* GridSizeMultipllier, GridPosition.y+j* GridSizeMultipllier, GridPosition.z+k* GridSizeMultipllier);
                            GL.Vertex3(GridPosition.x+i* GridSizeMultipllier, GridPosition.y+j* GridSizeMultipllier, GridPosition.z+(k+1)* GridSizeMultipllier);
                        }
                    }
                    GL.End();
                }
            }
        }
    }
}