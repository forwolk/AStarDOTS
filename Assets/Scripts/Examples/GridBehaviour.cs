using System;
using Pathfinding;
using Unity.Collections;
using UnityEngine;

namespace Examples
{
    /* Copyright (C) Anton Trukhan <anton.truhan@gmail.com>, 2020 - All Rights Reserved.
    */
    public class GridBehaviour : UnityEngine.MonoBehaviour
    {
        private const int DEFAULT_CLEARANCE = 1;
        private const long DEFAULT_CAPABILITIES = 1;
        
        [HideInInspector]
        public int GridWidth = 20;
        
        [HideInInspector]
        public int GridHeight = 20;
        
        public Color DefaultColor = Color.white;
        public Color SelectedColor = Color.green;
        public Color BlockedColor = Color.red;
        
        public GridCell CellPrefab;
        public int UnitSize = 1;
        public long UnitCapabilities = 1;
        
        [HideInInspector]
        public GridCell[] Cells;

        [NonSerialized]
        public GridCell SelectedCell1;
        
        [NonSerialized]
        public GridCell SelectedCell2;
        

        private void Awake()
        {
            for (var i = 0; i < Cells.Length; ++i)
            {
                Cells[i].OnButtonClicked += OnCellClicked;
                Cells[i].OnClearanceChanged += OnClearanceChanged;
                Cells[i].GridX = i % GridWidth;
                Cells[i].GridY = i / GridWidth;
            }

            ResetCells();
        }

        private void OnDestroy()
        {
            for (var i = 0; i < Cells.Length; ++i)
            {
                Cells[i].OnButtonClicked -= OnCellClicked;
            }
        }

        private void OnClearanceChanged(GridCell cell)
        {
            var color = cell.Clearance >= UnitSize ? DefaultColor : BlockedColor;
            cell.SetCellColor(color);
        }

        private void OnCellClicked(GridCell cell)
        {
            if (SelectedCell1 == null)
            {
                SelectedCell1 = cell;
                SelectedCell1.SetCellColor(SelectedColor);
                return;
            }

            if (SelectedCell2 == null)
            {
                SelectedCell2 = cell;
                SelectedCell2.SetCellColor(SelectedColor);
                RunSearch();
                return;
            }

            ResetCells();
        }
        
        private void RunSearch()
        {
            var grid = new PathfindingGrid(GridWidth, GridHeight, Allocator.Persistent);
            for (var i = 0; i < Cells.Length; ++i)
            {
                var cell = Cells[i];
                grid.SetClearance(cell.GridX, cell.GridY, cell.Clearance);
                grid.SetFlag(cell.GridX, cell.GridY, cell.Capabilities);
            }
            
            NativeList<int> path = new NativeList<int>(Allocator.Persistent);
            var pathFound = AStarSearch.TryGetPath(grid,
                SelectedCell1.GridX, SelectedCell1.GridY,
                SelectedCell2.GridX, SelectedCell2.GridY,
                UnitSize, UnitCapabilities,
                Allocator.Persistent, path);

            foreach (var index in path)
            {
                Cells[index].SetCellColor(SelectedColor);
            }

            if (path.IsCreated)
            {
                path.Dispose();
            }
            
            grid.Dispose();
            
        }

        private void ResetCells()
        {
            SelectedCell1 = null;
            SelectedCell2 = null;
            for (var i = 0; i < Cells.Length; ++i)
            {
                var clearanceConditionMet = Cells[i].Clearance >= UnitSize;
                var landscapeTraversable = (Cells[i].Capabilities & UnitCapabilities) > 0;
                var color = (clearanceConditionMet && landscapeTraversable) ? DefaultColor : BlockedColor;
                Cells[i].SetCellColor(color);
            }
        }

        public void RecreateCells()
        {
            DestroyChildren();
            Cells = new GridCell[GridWidth * GridHeight];
            var rect = CellPrefab.GetComponent<RectTransform>().rect;
            for (var i = 0; i < GridWidth; ++i)
            {
                for (var j = 0; j < GridHeight; ++j)
                {
                    var cell = Instantiate(CellPrefab, transform);
                    cell.Clearance = DEFAULT_CLEARANCE;
                    cell.Capabilities = DEFAULT_CAPABILITIES;
                    cell.GridX = i;
                    cell.GridY = j;
                    cell.SetText($"({i};{j})");
                    var rectTransform = cell.GetComponent<RectTransform>();
                    rectTransform.position = new Vector3(i * rect.width, (GridHeight - j) * rect.height);
                    var index = j * GridWidth + i;
                    Cells[index] = cell;
                }
            }
        }

        private void DestroyChildren()
        {
            while (transform.childCount > 0)
            {
                GameObject.DestroyImmediate(transform.GetChild(0).gameObject);
            }
        }
    }
}