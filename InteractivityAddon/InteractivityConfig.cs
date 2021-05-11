using System;

namespace Interactivity
{
    /// <summary>
    /// Represents a configuration class for the <see cref="InteractivityService"/>
    /// </summary>
    public sealed class InteractivityConfig
    {
        public static InteractivityConfig Default => new InteractivityConfig();
        
        /// <summary>
        /// The <see cref="TimeSpan"/> to use for interactivity timeouts.
        /// Defaults to 45 seconds.
        /// </summary>
        public TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromSeconds(45);

        /// <summary>
        /// Whether to run the internal event handlers used for interactivity in a separate task.
        /// This should be used to prevent blocking the gateway during high loads.
        /// </summary>
        public bool RunOnGateway { get; set; } = true;
    }
}