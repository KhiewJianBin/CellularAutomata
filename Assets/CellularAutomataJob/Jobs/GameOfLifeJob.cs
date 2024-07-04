using Unity.Collections;
using Unity.Jobs;
using static CAGridMapScriptableObject;

struct GameOfLifeJob : IJobFor
{
    //Read
    [ReadOnly] public int size;
    [ReadOnly] public bool loop;
    [ReadOnly] public NativeArray<Cell> inputGrid;

    //Write
    public NativeArray<Cell> outputGrid;

    public void Execute(int index)
    {
        int i = index / size;
        int j = index % size;

        var state = inputGrid[i * size + j].state;
        var On_neighbours = CountNeighboursWithState(i, j, CellState.On, loop);

        // Rules
        if (state == CellState.Off && On_neighbours == 3)
        {
            outputGrid[i * size + j] = new Cell { state = CellState.On };
        }
        else if (state == CellState.On && (On_neighbours != 2 && On_neighbours != 3))
        {
            outputGrid[i * size + j] = new Cell { state = CellState.Off };
        }
        else
        {
            outputGrid[i * size + j]= new Cell { state = state };
        }
    }

    int CountNeighboursWithState(int idx, int idy, CellState state, bool loop)
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

                count += inputGrid[row * size + col].state == state ? 1 : 0;
            }
        }
        count -= inputGrid[idx * size + idy].state == state ? 1 : 0;
        return count;
    }
}