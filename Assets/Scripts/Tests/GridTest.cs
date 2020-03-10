using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Pathfinding;
using Unity.Collections;

namespace Tests
{
    /* Copyright (C) Anton Trukhan <anton.truhan@gmail.com>, 2020 - All Rights Reserved.
    */
    public class GridTest
    {

        [Flags]
        public enum LandscapeType : long
        {
            Land = 1, Sea = 2, Mountain = 4, Impassable = 1L << 63
        }
        
        [Test]
        public void CellClearanceIsCorrectlySet()
        {
            const int clearance = 20;
            const int X = 0;
            const int Y = 1;
            var grid = new PathfindingGrid(20, 20, Allocator.Persistent);
            grid.SetClearance(X, Y, clearance);
            Assert.AreEqual(clearance, grid.GetClearance(X, Y));
            grid.Dispose();
        }
        
        [Test]
        public void CellFlagIsCorrectlySet()
        {
            const int flag = 20;
            const int X = 19;
            const int Y = 19;
            var grid = new PathfindingGrid(20, 20, Allocator.Persistent);
            grid.SetFlag(X, Y, flag);
            Assert.AreEqual(flag, grid.GetFlag(X, Y));
            grid.Dispose();
        }
        
        //Terrain type check
        [TestCase(19, 19, LandscapeType.Land, 1, LandscapeType.Sea, 1, false)]
        [TestCase(19, 19, LandscapeType.Land, 1, LandscapeType.Land, 1, true)]
        [TestCase(19, 19, LandscapeType.Land, 1, LandscapeType.Land | LandscapeType.Sea, 1, true)]
        [TestCase(19, 19, LandscapeType.Land, 1, LandscapeType.Mountain | LandscapeType.Sea, 1, false)]
        [TestCase(19, 19, LandscapeType.Impassable, 1, LandscapeType.Mountain | LandscapeType.Sea | LandscapeType.Land, 1, false)]
        
        //Clearance check
        [TestCase(19, 19, LandscapeType.Land, 1, LandscapeType.Land, 3, false)]
        [TestCase(19, 19, LandscapeType.Land, 3, LandscapeType.Land, 3, true)]
        [TestCase(19, 19, LandscapeType.Mountain, 4, LandscapeType.Mountain, 2, true)]
        public void GridIsPassableMethodWorksCorrectly(int x, int y, LandscapeType cellType, int cellClearance, 
                                                        LandscapeType unitType, int unitySize, bool result)
        {
            var grid = new PathfindingGrid(20, 20, Allocator.Persistent);
            grid.SetFlag(x, y, (long) cellType);
            grid.SetClearance(x, y, cellClearance);
            Assert.AreEqual(result, grid.IsPassable( x,y, (long) unitType, unitySize) );
            grid.Dispose();
        }
        
    }
}
