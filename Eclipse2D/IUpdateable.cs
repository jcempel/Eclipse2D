using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eclipse2D
{
    /// <summary>
    /// Represents the interface of an updateable game component.
    /// </summary>
    public interface IUpdateable
    {
        /// <summary>
        /// An event that fires when the 'Enabled' property changes.
        /// </summary>
        event EventHandler<EventArgs> EnabledChanged;

        /// <summary>
        /// An event that fires when the 'UpdateOrder' property changes.
        /// </summary>
        event EventHandler<EventArgs> UpdateOrderChanged;

        /// <summary>
        /// Updates the game component.
        /// </summary>
        /// <param name="GameTime">The delta-time between frames.</param>
        void Update(GameTime GameTime);

        /// <summary>
        /// Gets whether or not the game component should be updated.
        /// </summary>
        Boolean Enabled { get; }

        /// <summary>
        /// Gets the update order relative to other game components. Lower values are updated first.
        /// </summary>
        Int32 UpdateOrder { get; }
    }
}
