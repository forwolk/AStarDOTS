using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Pathfinding
{
    /* Copyright (C) Anton Trukhan <anton.truhan@gmail.com>, 2020 - All Rights Reserved.
    */
    public struct GridCell
    {
        public int Clearance;
        public long AreaType;
    }

    [NativeContainer]
    public struct PathfindingGrid : IDisposable
    {
        public readonly int Width;
        public readonly int Height;
        private NativeArray<GridCell> array;

        public PathfindingGrid(int width, int height, Allocator allocator)
        {
            Width = width;
            Height = height;
            
            var size = width * height;
            array = new NativeArray<GridCell>(size, allocator);
        }

        public void IndexToAxisCoordinates(int index, out int x, out int y)
        {
            x = index % Width;
            y = index / Width;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetFlag(int x, int y, long flag)
        {
            var index = CalculateIndex(x, y, Width);
            var cell = array[index];
            cell.AreaType = flag;
            array[index] = cell;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long GetFlag(int x, int y)
        {
            var index = CalculateIndex(x, y, Width);
            var result = array[index].AreaType;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public bool IsPassable(int x, int y, long unitCapabilities, int unitSize)
        {
            var clearance = GetClearance(x, y);
            var flag = GetFlag(x, y);
            var result = flag & unitCapabilities;
            return result > 0 && clearance >= unitSize;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetClearance(int x, int y, int clearance)
        {
            var index = CalculateIndex(x, y, Width);
            var cell = array[index];
            cell.Clearance = clearance;
            array[index] = cell;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetClearance(int x, int y)
        {
            var index = CalculateIndex(x, y, Width);
            var cell = array[index];
            return cell.Clearance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CalculateIndex(int x, int y, int width)
        {
            var result = y * width + x;
            return result;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CalculateCoordinates(int index, int width, out int x, out int y)
        {
            y = index / width;
            x = index % width;
        }
        
        public void Dispose()
        {
            array.Dispose();
        }
    }
}