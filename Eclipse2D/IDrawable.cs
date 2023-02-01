using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eclipse2D
{
    /// <summary>
    /// Represent the interface of a drawable game component.
    /// </summary>
    public interface IDrawable
    {
        /// <summary>
        /// An event that fires when the 'DrawOrder' property changes.
        /// </summary>
        event EventHandler<EventArgs> DrawOrderChanged;

        /// <summary>
        /// An event that fires when the 'Visible' property changes.
        /// </summary>
        event EventHandler<EventArgs> VisibleChanged;

        /// <summary>
        /// Draws the game component.
        /// </summary>
        /// <param name="GameTime">The delta-time between frames.</param>
        void Draw(GameTime GameTime);

        /// <summary>
        /// Gets whether or not the game component is visible.
        /// </summary>
        Boolean Visible { get; }

        /// <summary>
        /// Gets the draw order relative to other game components. Lower value are drawn first.
        /// </summary>
        Int32 DrawOrder { get; }
    }
}
