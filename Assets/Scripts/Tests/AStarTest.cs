using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Pathfinding;
using Unity.Collections;

namespace Tests
{

    /* Copyright (C) Anton Trukhan <anton.truhan@gmail.com>, 2020 - All Rights Reserved.
    */
    public class AStarTest
    {
        private const int GRID_SIZE = 30;

        [TestCase(0, 5, GRID_SIZE, 5)]
        [TestCase(0, 0, GRID_SIZE / 2, GRID_SIZE / 2)]
        [TestCase(0, GRID_SIZE / 2, GRID_SIZE / 2, GRID_SIZE / 2)]
        public void PathIsDeterminedCorrectly(int x, int y, int x2, int y2)
        {
            var grid = new PathfindingGrid(GRID_SIZE, GRID_SIZE, Allocator.Persistent);
            SetArea(grid, GRID_SIZE, GRID_SIZE, 1, 1);
            var pathfound = ExecuteSearch(grid, x, y, x2, y2, 0, 1);
            Assert.IsTrue(pathfound);
        }
        
        [TestCase(0, 0, GRID_SIZE / 2, 0, true)]
        [TestCase(0, GRID_SIZE / 2, GRID_SIZE - 1, GRID_SIZE / 2, false)]
        [TestCase(0, 0, 0, GRID_SIZE - 1 , true)]
        public void PathIsDetermined_Correctly_WithVerticalObstacle_InTheMiddle(int x, int y, int x2, int y2, bool isPathValid)
        {
            var grid = new PathfindingGrid(GRID_SIZE, GRID_SIZE, Allocator.Persistent);
            SetArea(grid, GRID_SIZE, GRID_SIZE, 1, 1);
            SetVerticalLine(grid, GRID_SIZE / 2, 0, 1);
            var pathfound = ExecuteSearch(grid, x, y, x2, y2, 0, 1);
            Assert.AreEqual(isPathValid, pathfound);
        }
        
        [TestCase(0, 0, GRID_SIZE / 2, 0, true)]
        [TestCase(0, GRID_SIZE / 2 - 1, GRID_SIZE - 1, GRID_SIZE / 2 - 1, true)]
        [TestCase(0, 0, 0, GRID_SIZE - 1, false)]
        public void PathIsDetermined_Correctly_WithHorizontalObstacle_InTheMiddle(int x, int y, int x2, int y2, bool isPathValid)
        {
            var grid = new PathfindingGrid(GRID_SIZE, GRID_SIZE, Allocator.Persistent);
            SetArea(grid, GRID_SIZE, GRID_SIZE, 1, 1);
            SetHorizontaLine(grid, GRID_SIZE / 2, 0, 1);
            var pathfound = ExecuteSearch(grid, x, y, x2, y2, 0, 1);
            Assert.AreEqual(isPathValid, pathfound);
        }

        [TestCase(1, 0, GRID_SIZE - 1, 0, true)]
        [TestCase(0, GRID_SIZE - 1, GRID_SIZE - 2, GRID_SIZE - 1, true)]
        [TestCase(0, 0, GRID_SIZE - 1, GRID_SIZE - 1, false)]
        public void PathIsDetermined_Correctly_WithDiagonalObstacle(int x, int y, int x2, int y2, bool isPathValid)
        {
            var grid = new PathfindingGrid(GRID_SIZE, GRID_SIZE, Allocator.Persistent);
            SetArea(grid, GRID_SIZE, GRID_SIZE, 1, 1);
            for (var i = 0; i < GRID_SIZE; ++i)
            {
                grid.SetFlag(i, i, 0);
            }

            var pathfound = ExecuteSearch(grid, x, y, x2, y2, 0, 1);
            Assert.AreEqual(isPathValid, pathfound);
        }

        [TestCase(0, 0, GRID_SIZE / 2, 0, true)]
        [TestCase(0, GRID_SIZE / 2 - 1, GRID_SIZE - 1, GRID_SIZE / 2 - 1, true)]
        [TestCase(0, 0, 0, GRID_SIZE - 1, false)]
        public void PathIsFound_IfCellsHave_EnoughClearance(int x, int y, int x2, int y2, bool isPathValid)
        {
            const int unitClearance = 2;
            var grid = new PathfindingGrid(GRID_SIZE, GRID_SIZE, Allocator.Persistent);
            SetArea(grid, GRID_SIZE, GRID_SIZE, 1, unitClearance);
            SetHorizontaLine(grid, GRID_SIZE / 2, 1, unitClearance - 1);
            var pathfound = ExecuteSearch(grid, x, y, x2, y2, unitClearance, 1);
            Assert.AreEqual(isPathValid, pathfound);
        }

        [Test] //Partial vertical block
        public void PathFoundIsTheShortest_WhenPathIsPartiallyBlocked()
        {
            var grid = new PathfindingGrid(6, 5, Allocator.Persistent);
            SetArea(grid, 6, 5, 1, 1);
            grid.SetClearance(3, 0, 0);
            grid.SetClearance(3, 1, 0);
            grid.SetClearance(3, 2, 0);
            grid.SetClearance(3, 3, 0);

            NativeList<int> resultPath = new NativeList<int>(Allocator.Persistent);
            var pathFound = AStarSearch.TryGetPath(grid,
                2, 0,
                5, 4,
                1, 1, Allocator.Persistent, resultPath);
            
            Assert.AreEqual(true, pathFound);
            Assert.AreEqual(2, resultPath[0]);
            Assert.AreEqual(8, resultPath[1]);
            Assert.AreEqual(14, resultPath[2]);
            Assert.AreEqual(20, resultPath[3]);
            Assert.AreEqual(27, resultPath[4]);
            Assert.AreEqual(28, resultPath[5]);
            
            grid.Dispose();
            resultPath.Dispose();
        }
        
        public void SetVerticalLine(PathfindingGrid grid, int x, int flag, int clearance)
        {
            for (var i = 0; i < grid.Height; ++i)
            {
                grid.SetFlag(x, i, flag);
                grid.SetClearance(x, i, clearance);
            }
        }

        public void SetHorizontaLine(PathfindingGrid grid, int y, int flag, int clearance)
        {
            for (var i = 0; i < grid.Width; ++i)
            {
                grid.SetFlag(i, y, flag);
                grid.SetClearance(i, y, clearance);
            }
        }

        public void SetArea(PathfindingGrid Grid, int width, int height, int flag, int clearance)
        {
            for (var i = 0; i < width; ++i)
            {
                for (var j = 0; j < height; ++j)
                {
                    Grid.SetFlag(i, j, flag);
                    Grid.SetClearance(i, j, clearance);
                }
            }
        }

        private bool ExecuteSearch(PathfindingGrid grid, int x, int y, int x2, int y2, int unitSize, long unitCapabilities)
        {
            NativeArray<bool> pathFoundResult = new NativeArray<bool>(1, Allocator.Persistent);
            NativeList<int> path = new NativeList<int>(1, Allocator.Persistent);
            var job = new AStarJobExample
            {
                StartX = x,
                StartY = y,
                FinishX = x2,
                FinishY = y2,
                Grid = grid,
                UnitSize = unitSize,
                UnitCapabilities = unitCapabilities,
                PathFound = pathFoundResult,
                ResultPath = path
            };
            
            job.Execute();
            var result = pathFoundResult[0];
            pathFoundResult.Dispose();
            path.Dispose();
            return result;
        }
        
    }
}

