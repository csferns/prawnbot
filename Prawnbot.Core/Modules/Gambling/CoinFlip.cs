namespace Prawnbot.Core.Modules.Gambling
{
    /// <summary>
    /// <para>Represents the flip of a coin</para>
    /// <para>Return type is bool because the result can only be heads or tails</para>
    /// </summary>
    public class CoinFlip : Gamble<bool>
    {
        public override bool Execute()
        {
            // 50/50 chance of getting heads or tails.
            return Random.Next(100) <= 50;
        }
    }
}
