using System.Collections.Generic;
using Zenject;
using RollicGames.Math.Runtime.Model;
using RollicGames.ColorBlockJamClone.Level.Runtime.Model;

namespace RollicGames.ColorBlockJamClone.Level.Runtime.Controller
{
    public class LevelGridController
    {
        [Inject] private readonly LevelGridModel _gridModel;

        private void Initialize()
        {
            // TODO : Change here, its mocked Level 1 initializing.
            _gridModel.Width = 6;
            _gridModel.Height = 7;

            _gridModel.Grid.Clear();
            for(var rowIndex = 0; rowIndex < _gridModel.Height; rowIndex++)
            {
                _gridModel.Grid.Add(new List<LevelObjectModel>());
            }
        }
    }
}