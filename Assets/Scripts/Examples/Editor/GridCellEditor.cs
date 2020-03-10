using UnityEditor;

namespace Examples.Editor
{
    /* Copyright (C) Anton Trukhan <anton.truhan@gmail.com>, 2020 - All Rights Reserved.
    */
    [CustomEditor(typeof(GridCell))]
    [CanEditMultipleObjects]
    public class GridCellEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (targets.Length > 0)
            {
                GridCell gridCell;
                for (var i = 0; i < targets.Length; ++i)
                {
                    gridCell = (GridCell) targets[i];
                    EditorGUILayout.LabelField($"Cell: ({gridCell.GridX.ToString()}; {gridCell.GridY.ToString()})");
                }

                gridCell = (GridCell) targets[0];
                var clearance = EditorGUILayout.IntSlider("Clearance:", gridCell.Clearance, 0, 30);
                var clearanceChanged = clearance != gridCell.Clearance;

                if (clearanceChanged)
                {
                    for (var i = 0; i < targets.Length; ++i)
                    {
                        gridCell = (GridCell) targets[i];
                        gridCell.Clearance = clearance;
                        gridCell.DispatchClearanceChanged();
                    }
                }
            }
        }
    }
}