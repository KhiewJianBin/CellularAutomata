using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static CAGridMapScriptableObject;

public class CellularAutomataCompute : MonoBehaviour
{
    readonly int size = 50;

    [Header("Scene")]
    [SerializeField] Camera cam;
    [SerializeField] GridOverlayGizmo gridoverlay;

    [Header("UI")]
    [SerializeField] Toggle PlayPause_Toggle;
    [SerializeField] Button Clear_Btn;

    [Header("Data")]
    [SerializeField] CAGridMapScriptableObject map;

    [Header("System")]
    [SerializeField] ComputeShader shader;

    [Header("Settings")]
    public float updateInterval = 0.2f;
    public bool loop = false;

    // Vars
    float timer;
    bool playMode = false;
    Cell[] nextgrid2D;
    CellState? mouseCopy;

    //Compute
    ComputeBuffer gridBuffer;
    ComputeBuffer nextgridBuffer;
    int GOLKernelHandle;
    void Awake()
    {
        PlayPause_Toggle.onValueChanged.AddListener(OnEditRunChanged);
        Clear_Btn.onClick.AddListener(OnClearClicked);

        void OnEditRunChanged(bool b)
        {
            playMode = b;
            Clear_Btn.interactable = !b;
        }
        void OnClearClicked()
        {
            map.Clear(CellState.Off);
        }

        nextgrid2D = new Cell[size * size];

        // Init Compute
        GOLKernelHandle = shader.FindKernel("GOL");

        int stride = 1 * sizeof(int);
        gridBuffer = new ComputeBuffer(size * size, stride);
        nextgridBuffer = new ComputeBuffer(size * size, stride);
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            var worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (worldMousePos.x < 0 || worldMousePos.x > size) return;
            if (worldMousePos.y < 0 || worldMousePos.y > size) return;

            int i = (int)(worldMousePos.x);
            int j = (int)(worldMousePos.y);

            var cell = map[i, j];

            mouseCopy ??= cell.state == CellState.On ? CellState.Off : CellState.On;

            cell.state = mouseCopy.Value;
            map[i, j] = cell;
        }
        else
        {
            mouseCopy = null;
        }

        if (playMode)
        {
            timer += Time.deltaTime;

            if (timer < updateInterval) return;

            timer -= updateInterval;

            // Prep Data To Send To GPU
            gridBuffer.SetData(map.grid2D);
            shader.SetFloat("size", size);
            shader.SetBool("loop", loop);
            shader.SetBuffer(GOLKernelHandle, nameof(gridBuffer), gridBuffer);
            shader.SetBuffer(GOLKernelHandle, nameof(nextgridBuffer), nextgridBuffer);

            // Send to GPU
            shader.Dispatch(GOLKernelHandle, size, size, 1);

            // Get From GPU
            nextgridBuffer.GetData(nextgrid2D);

            for (int i = 0; i < nextgrid2D.Length; i++)
            {
                map.grid2D[i] = nextgrid2D[i];
            }
        }
    }

    void OnDrawGizmos()
    {
        if (map.grid2D.Length != size * size)
        {
            map.Init(size);
        }

        gridoverlay.GridsizeX = size;
        gridoverlay.GridsizeY = size;
        gridoverlay.GridsizeZ = 0;

        if (!cam) cam = Camera.main;

        cam.transform.position = new Vector3(size / 2f, size / 2f, -10);
        cam.orthographicSize = size / 2f * 1.1f;

        var cellSize = Vector3.one;

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                var center = new Vector3(i, j, 0) + cellSize / 2;
                var cell = map[i, j];

                if (cell.state == CellState.On)
                {
                    DrawBlackCube(center, cellSize);
                }
                else
                {
                    DrawWhiteCube(center, cellSize);
                }
            }
        }

        void DrawWhiteCube(Vector3 center, Vector3 cellSize)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawCube(center, cellSize);
        }
        void DrawBlackCube(Vector3 center, Vector3 cellSize)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawCube(center, cellSize);
        }
    }

    void OnDestroy()
    {
        gridBuffer.Dispose();
        nextgridBuffer.Dispose();

        EditorUtility.SetDirty(map);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}