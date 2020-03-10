using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Mathematics;

namespace Pathfinding
{
	/* Copyright (C) Anton Trukhan <anton.truhan@gmail.com>, 2020 - All Rights Reserved.
     */
	
	public enum Expanded : byte
	{
		False = 0,
		True = 1
	}

	public struct OpenSetRecord
	{
		public float DistancePassed;
		public int ParentCell;
		public Expanded Expanded;
	}
	
	public struct CellSearchRecord : IComparable<CellSearchRecord>, IEquatable<CellSearchRecord>
	{
		public float Priority;
		public int CellId;
		
		public int CompareTo(CellSearchRecord other)
		{
			return Priority.CompareTo(other.Priority);
		}

		public bool Equals(CellSearchRecord other)
		{
			return CellId == other.CellId;
		}
	}

	public static class AStarSearch
	{
		private static readonly int2[] OFFSETS = new int2[8]
		{
			//Vertical
			new int2(-1, y: 0),
			new int2( 1, y: 0),
			//Horizontal
			new int2(0, y: -1), 
			new int2(0, y: 1),
			//Diagonal
			new int2(-1, y: 1),
			new int2(1, y: 1),
			new int2(-1, y: -1),
			new int2( 1, y: -1)
		};

		public static bool TryGetPath (PathfindingGrid grid, int startX, int startY, int finishX, int finishY, 
										int unitSize, long unitCapabilities, Allocator allocator, NativeList<int> resultPath)
		{
			//Requests Min
			var priorityQueue = new PriorityQueue<CellSearchRecord>(HeapType.Min, allocator);
			var openSetSupportData = new NativeHashMap<int, OpenSetRecord>(20, allocator);
			
			var startCellIndex = PathfindingGrid.CalculateIndex(startX, startY, grid.Width);
			var endCellIndex = PathfindingGrid.CalculateIndex(finishX, finishY, grid.Width);
			
			var endCell = new CellSearchRecord();
			endCell.Priority = AStarSearchHeuristic.DiagonalDistance( finishX, finishY, startX, startY);
			endCell.CellId = endCellIndex;
			priorityQueue.Add ( endCell );
			
			//Set distance passed
			openSetSupportData[endCellIndex] = new OpenSetRecord
			{
				DistancePassed = 0,
				ParentCell = -1,
			};

			while (priorityQueue.Count > 0)
			{
				var currentCell = priorityQueue.Remove();
				var currentCellIndex = currentCell.CellId;
				
				//Mark cell as expanded
				var currentCellData = openSetSupportData[currentCellIndex];
				currentCellData.Expanded = Expanded.True;
				openSetSupportData[currentCellIndex] = currentCellData;
				
				//Reconstruct path
				var pathFound = currentCellIndex == startCellIndex;
				if (!pathFound)
				{
					var endX = startX;
					var endY = startY;
					PathfindingGrid.CalculateCoordinates(currentCellIndex, grid.Width, out var x, out var y);
					for (var i = 0; i < OFFSETS.Length; ++i)
					{
						var moveCost = Math.Abs(OFFSETS[i].x) == Math.Abs(OFFSETS[i].y) ? 
							AStarConsts.DIAGONAL_MOVEMENT : AStarConsts.STRAIGHT_MOVEMENT;
						
						ProcessNeighbour(x + OFFSETS[i].x, y + OFFSETS[i].y, unitSize, unitCapabilities, 
							currentCellData.DistancePassed + moveCost,
							currentCellIndex,
							endX, endY, grid, openSetSupportData, priorityQueue);
					}
				}
				else
				{ 
					ReconstructPath(openSetSupportData, startCellIndex, endCellIndex, resultPath );
					
					priorityQueue.Dispose();
					openSetSupportData.Dispose();
					return true;
				}

			}

			
			priorityQueue.Dispose();
			openSetSupportData.Dispose();
			return false;
		}

		private static void ProcessNeighbour(
			int x, int y, int unitSize, long unitCapabilities, 
			float distancePassed, int parenCellId,
			int finishX, int finishY,
			PathfindingGrid grid, 
			NativeHashMap<int, OpenSetRecord> openSetSupportData, 
			PriorityQueue<CellSearchRecord> priorityQueue)
		{
			//Ignore invalid cells
			if (x < 0 || y < 0)
			{
				return;
			}

			if (x >= grid.Width || y >= grid.Height)
			{
				return;
			}

			var neighbourCellIndex = PathfindingGrid.CalculateIndex(x, y, grid.Width);
			var isInOpenList = openSetSupportData.TryGetValue(neighbourCellIndex, out var neighbourCell);
			
			if (!isInOpenList)
			{
				neighbourCell = openSetSupportData[neighbourCellIndex] = new OpenSetRecord();
				openSetSupportData[neighbourCellIndex] = neighbourCell;
			}
			else if (neighbourCell.Expanded == Expanded.True)
			{
				//Item is in Closed list, return
				return;
			}
			else if (distancePassed >= neighbourCell.DistancePassed) 
			{
				//if it's already in priority queue and the distance has not improved, we are returning
				return;
			}

			//Item is not traversable, return
			if (!grid.IsPassable(x, y, unitCapabilities, unitSize))
			{
				return;
			}

			var cellRecord = new CellSearchRecord { Priority = 0, CellId = neighbourCellIndex};
			//We found a better route to this cell, so we need to remove it from Priority queue as later we are going to add it with better metrics
			if ( isInOpenList && distancePassed < neighbourCell.DistancePassed )
			{
				priorityQueue.Remove(cellRecord);
			}

			float distanceToTarget = AStarSearchHeuristic.DiagonalDistance(x, y, finishX, finishY);

			neighbourCell.ParentCell = parenCellId;
			neighbourCell.DistancePassed = distancePassed;
			openSetSupportData[neighbourCellIndex] = neighbourCell;

			cellRecord.Priority = distanceToTarget + distancePassed;
			priorityQueue.Add(cellRecord);
		}

		private static void ReconstructPath(NativeHashMap<int, OpenSetRecord> openSetSupportData, 
														int startCellIndex, int finishCellIndex, NativeList<int> resultPath)
		{
			var currentCellIndex = startCellIndex;
			while (currentCellIndex != finishCellIndex)
			{
				resultPath.Add(currentCellIndex);
				currentCellIndex = openSetSupportData[currentCellIndex].ParentCell;
			}
		}
	}
}
