using UnityEditor;
using UnityEngine;

namespace Examples.Editor
{
    /* Copyright (C) Anton Trukhan <anton.truhan@gmail.com>, 2020 - All Rights Reserved.
    */
    [CustomEditor(typeof(GridBehaviour))]
    public class GridEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (!Application.isPlaying)
            {
                var targetObject = (GridBehaviour) target;
                var width = EditorGUILayout.IntSlider("Width: ", targetObject.GridWidth, 1, 20);
                var height = EditorGUILayout.IntSlider("Height: ", targetObject.GridHeight, 1, 20);

                var sizeChanged = width != targetObject.GridWidth || height != targetObject.GridHeight;
                targetObject.GridWidth = width;
                targetObject.GridHeight = height;
                if (sizeChanged)
                {
                    targetObject.RecreateCells();
                }
            }

        }

    }
}