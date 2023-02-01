using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eclipse2D
{
    /// <summary>
    /// Represents the interface of a game screen.
    /// </summary>
    public interface IGameScreen
    {
        /// <summary>
        /// Represents if the game screen is updating.
        /// </summary>
        Boolean IsUpdating { get; }

        /// <summary>
        /// Represents if the game screen is drawing.
        /// </summary>
        Boolean IsDrawing { get; }

        /// <summary>
        /// Initializes the game screen.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Loads the game screen assets.
        /// </summary>
        void LoadContent();

        /// <summary>
        /// Updates the game screen.
        /// </summary>
        /// <param name="GameTime">The delta-time between frames.</param>
        void Update(GameTime GameTime);

        /// <summary>
        /// Draws the game screen.
        /// </summary>
        /// <param name="GameTime">The delta-time between frames.</param>
        void Draw(GameTime GameTime);

        /// <summary>
        /// Unloads the game screen assets.
        /// </summary>
        void UnloadContent();

        /// <summary>
        /// Un-initializes the game screen.
        /// </summary>
        void Uninitialize();
    }
}
