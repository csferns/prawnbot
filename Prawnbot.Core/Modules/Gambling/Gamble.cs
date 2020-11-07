using System;

namespace Prawnbot.Core.Modules.Gambling
{
    /// <summary>
    /// Represents a basic gamble the user can undertake
    /// </summary>
    /// <typeparam name="T">Expected return type</typeparam>
    public abstract class Gamble<T>
    {
        protected Random Random { get; } = new Random();

        /// <summary>
        /// Executes and returns the result of the gamble
        /// </summary>
        /// <returns></returns>
        public abstract T Execute();
    }
}
