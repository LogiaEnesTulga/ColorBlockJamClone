using RollicGames.ColorBlockJamClone.Player.Runtime.Model;
using RollicGames.Persistence.Runtime.Presenter;
using Zenject;

namespace RollicGames.ColorBlockJamClone.Player.Runtime.Controller
{
    public interface IPlayerController
    {
        void InitializePlayer();
        int GetLevel();
        int GetCoin();
        void SetPlayerData(int level, int coin);
    }

    public class PlayerController : IPlayerController
    {
        private const string PlayerDataKey = "PlayerData";

        [Inject] private readonly PlayerModel _playerModel;
        [Inject] private readonly IPersistence _persistence;

        public void InitializePlayer()
        {
            var savedModel = _persistence.LoadData<PlayerModel>(PlayerDataKey);
            if (savedModel == null) return;

            _playerModel.Level = savedModel.Level;
            _playerModel.Coin = savedModel.Coin;
        }

        public int GetLevel()
        {
            return _playerModel.Level;
        }

        public int GetCoin()
        {
            return _playerModel.Coin;
        }

        public void SetPlayerData(int level, int coin)
        {
            _playerModel.Coin = coin;
            _playerModel.Level = level;
            SavePlayerData();
        }

        private void SavePlayerData()
        {
            _persistence.SaveData(PlayerDataKey, _playerModel);
        }
    }
}
