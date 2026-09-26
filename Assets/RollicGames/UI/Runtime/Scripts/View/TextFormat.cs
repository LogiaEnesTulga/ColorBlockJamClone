namespace RollicGames.UI.Runtime.View
{
    public static class TextFormat
    {
        public static string FormatCoinAmount(int amount)
        {
            if (amount < 1000)
            {
                return amount.ToString();
            }

            if (amount < 1000000)
            {
                return (amount / 1000f).ToString("0.##") + "k";
            }

            return (amount / 1000000f).ToString("0.##") + "m";
        }
    }
}