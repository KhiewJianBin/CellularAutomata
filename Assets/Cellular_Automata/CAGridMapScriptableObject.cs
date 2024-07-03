using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CAGridMap", menuName = "CA/CAGridMap")]
public class CAGridMapScriptableObject : ScriptableObject
{
    public Cells[] grid2D;
    public int size;

    public Cells this[int i, int j]
    {
        get => grid2D[j * size + i];
        set => grid2D[j * size + i] = value;
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

    public int CountNeighboursWithState(int idx, int idy, CellState state)
    {
        var count = 0;
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                var col = (idx + i + size) % size;
                var row = (idy + j + size) % size;
                count += grid2D[col * size + row].state == state ? 1 : 0;
            }
        }
        count -= grid2D[idy * size + idx].state == state ? 1 : 0;
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
