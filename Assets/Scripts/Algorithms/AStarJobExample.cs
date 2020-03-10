using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace Pathfinding
{
    /* Copyright (C) Anton Trukhan <anton.truhan@gmail.com>, 2020 - All Rights Reserved.
     */
    [BurstCompile]
    public struct AStarJobExample : IJob
    {
        [ReadOnly] public PathfindingGrid Grid;
        [ReadOnly] public int StartX;
        [ReadOnly] public int StartY;
        [ReadOnly] public int FinishX;
        [ReadOnly] public int FinishY;
        [ReadOnly] public int UnitSize;
        [ReadOnly] public long UnitCapabilities;

        public NativeList<int> ResultPath;
        public NativeArray<bool> PathFound;
            
        public void Execute()
        {
            var result = AStarSearch.TryGetPath(Grid, StartX, StartY, FinishX, FinishY, UnitSize, UnitCapabilities,
                Allocator.Temp, ResultPath);
            PathFound[0] = result;
        }
    }
}