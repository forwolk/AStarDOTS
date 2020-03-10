using System;
using UnityEngine;
using UnityEngine.UI;

namespace Examples
{
    /* Copyright (C) Anton Trukhan <anton.truhan@gmail.com>, 2020 - All Rights Reserved.
    */
    public class GridCell : MonoBehaviour
    {
        [SerializeField]
        private Text ClearanceLabel;
        [SerializeField]
        private Button Button;
        [SerializeField]
        private Image Background;
        
        public int Clearance;
        public long Capabilities;
        public int GridX;
        public int GridY;

        public event Action<GridCell> OnButtonClicked;
        public event Action<GridCell> OnClearanceChanged;

        public void Awake()
        {
            Button.onClick.AddListener(() => OnButtonClicked?.Invoke(this));
        }

        public void OnDestroy()
        {
            Button.onClick.RemoveAllListeners();
        }

        public void SetCellColor(Color color)
        {
            Background.color = color;
        }

        public void DispatchClearanceChanged()
        {
            OnClearanceChanged?.Invoke(this);
        }

        public void SetText(string text)
        {
            ClearanceLabel.text = text;
        }
    }
}