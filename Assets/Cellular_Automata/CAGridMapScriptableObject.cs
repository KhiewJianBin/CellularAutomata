using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CAGridMap", menuName = "CA/CAGridMap")]
public class CAGridMapScriptableObject : ScriptableObject
{
    public Cells[] grid2D;
    public int size;

    public Cells this[int i, int j]
    {
        get => grid2D[i * size + j];
        set => grid2D[i * size + j] = value;
    }

    public void Init(int size)
    {   
        this.size = size;
        grid2D = new Cells[size*size];
    }

    public void Clear(CellState state)
    {
        for (int i = 0; i < grid2D.Length; i++)
        {
            grid2D[i].state = state;
        }
    }

    public int CountNeighboursWithState(int idx, int idy, CellState state, bool loop)
    {
        var count = 0;
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                var row = idx + i;
                var col = idy + j;

                if (loop)
                {
                    row = (row + size) % size;
                    col = (col + size) % size;
                }

                if (row < 0 || row > size - 1) continue;
                if (col < 0 || col > size - 1) continue;

                count += grid2D[row * size + col].state == state ? 1 : 0;
            }
        }
        count -= grid2D[idx * size + idy].state == state ? 1 : 0;
        return count;
    }



    [Serializable]
    public struct Cells
    {
        public CellState state;
    }
    public enum CellState
    {
        Off = 0,
        On = 1
    }
}
