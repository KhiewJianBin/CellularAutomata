using UnityEngine;
using UnityEngine.UI;
using static CAGridMapScriptableObject;

public class CellularAutomata : MonoBehaviour
{
    readonly int size = 100;

    [Header("Scene")]
    [SerializeField] Camera cam;
    [SerializeField] GridOverlayGizmo gridoverlay;

    [Header("UI")]
    [SerializeField] Toggle PlayPause_Toggle;
    [SerializeField] Button Clear_Btn;

    [Header("Data")]
    [SerializeField] CAGridMapScriptableObject map;

    [Header("Settings")]
    public float updateInterval = 0.2f;

    // Vars
    float timer;
    bool playMode = false;

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
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            var worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int i = (int)Mathf.Clamp(worldMousePos.x, 0, size);
            int j = (int)Mathf.Clamp(worldMousePos.y, 0, size);

            var cell = map[i, j];
            cell.state = CellState.On;
            map[i, j] = cell;
        }

        if (playMode)
        {
            timer += Time.deltaTime;

            if (timer < updateInterval) return;

            timer -= updateInterval;

            var nextgrid2D = new Cells[size * size];

            //Update
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    var state = map[i, j].state;
                    var On_neighbours = map.CountNeighboursWithState(i, j, CellState.On);

                    // Rules
                    if (state == CellState.Off && On_neighbours == 3)
                    {
                        nextgrid2D[i * size + j].state = CellState.On;
                    }
                    else if (state == CellState.On && (On_neighbours < 2 || On_neighbours > 3))
                    {
                        nextgrid2D[i * size + j].state = CellState.Off;
                    }
                    else
                    {
                        nextgrid2D[i * size + j].state = state;
                    }
                }
            }

            map.grid2D = nextgrid2D;
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

        cam.transform.position = new Vector3(size / 2, size / 2, -10);
        cam.orthographicSize = size / 2 * 1.1f;

        var cellSize = Vector3.one;

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                var center = new Vector3(i, j, 0) + cellSize / 2;
                var cell = map[i,j];

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
}