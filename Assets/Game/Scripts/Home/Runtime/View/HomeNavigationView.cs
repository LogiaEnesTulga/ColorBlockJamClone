using System;
using System.Collections.Generic;
using RollicGames.ColorBlockJamClone.Home.Runtime.Model;
using UnityEngine;

namespace RollicGames.ColorBlockJamClone.Home.Runtime.View
{
    public class HomeNavigationView : MonoBehaviour
    {
        [SerializeField] private List<HomeNavigationTabView> _navigationTabList;
        
        private readonly Dictionary<HomeNavigationType, HomeNavigationTabView> _navigationTabDictionary = new();

        public void InitializeView(Action<HomeNavigationType> tabClickListener)
        {
            foreach(var tab in _navigationTabList)
            {
                tab.InitializeView(tabClickListener);
                _navigationTabDictionary.Add(tab.TabType, tab);
            }
        }

        public void PrepareForReuse()
        {
            foreach(var tab in _navigationTabDictionary.Values)
            {
                tab.PrepareForReuse();
            }

            _navigationTabDictionary.Clear();
        }

        public void NavigateTo(HomeNavigationType to, HomeNavigationType from = HomeNavigationType.None, bool animate = true)
        {
            if(from != HomeNavigationType.None)
            {
                _navigationTabDictionary[from].SetTabDeactive();
            }

            _navigationTabDictionary[to].SetTabActive(animate);
        }
    }
}