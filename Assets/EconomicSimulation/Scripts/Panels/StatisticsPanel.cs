﻿using Nashet.UnityUIUtils;
using UnityEngine;

namespace Nashet.EconomicSimulation
{
    public class StatisticsPanel : DragPanel
    {
        [SerializeField]
        private StatisticsPanelTable table;

        // Use this for initialization
        private void Start()
        {
            MainCamera.StatisticPanel = this;
            GetComponent<RectTransform>().anchoredPosition = new Vector2(20f, -460f);
            //show(false);
            Canvas.ForceUpdateCanvases();
            Hide();
        }

        public override void Refresh()
        {
            table.Refresh();
        }
    }
}