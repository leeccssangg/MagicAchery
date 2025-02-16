using System;

namespace TW.UGUI.Shared
{
    public interface IUnitTransitionAnimation
    {
        public bool IsInitialized { get; set; }
        /// <summary>
        /// Delay before the animation starts.
        /// </summary>
        public float Delay { get; set; }
        /// <summary>
        /// Duration of the animation exclude delay.
        /// </summary>
        public float Duration { get; set; }
        /// <summary>
        /// Called once when the animation is created.
        /// </summary>
        void Initialize();
        /// <summary>
        /// Called when the animation is about to start.
        /// </summary>
        void Setup();
        /// <summary>
        /// Called when the animation is about to update.
        /// </summary>
        /// <param name="time">Current update time</param>
        void SetTime(float time);
    }
}