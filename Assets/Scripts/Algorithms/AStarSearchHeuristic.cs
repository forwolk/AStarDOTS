using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace Pathfinding
{
    public static class AStarSearchHeuristic
    {
        /* Copyright (C) Anton Trukhan <anton.truhan@gmail.com>, 2020 - All Rights Reserved.
        */
        
        //Useful when you have a grid with 8-degrees of freedom
        //For grid with 4-degrees - manhattan distance is enough
        //For any angle movement, Euclid distance has to be used
        //Squared distance can not be used, because this heuristic is not admissible
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DiagonalDistance(int x1, int y1, int x2, int y2)
        { 
            var dx = math.abs(x1 - x2);
            var dy = math.abs(y1 - y2);
            return AStarConsts.STRAIGHT_MOVEMENT * (dx + dy) + 
                   (AStarConsts.DIAGONAL_MOVEMENT - 2 * AStarConsts.STRAIGHT_MOVEMENT) * math.min(dx, dy);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ManhattanDistance(int x1, int y1, int x2, int y2)
        {
            var dx = math.abs(x1 - x2);
            var dy = math.abs(y1 - y2);
            var distance = dx + dy;
            return distance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Euclid(int x1, int y1, int x2, int y2)
        {
            var dx = x1 - x2;
            var dy = y1 - y2;

            return math.sqrt(dx * dx + dy * dy);
        }
    }
}