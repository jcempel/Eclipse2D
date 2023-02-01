﻿using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using Eclipse2D.Graphics;
using Eclipse2D.Input;

namespace Eclipse2D.GameClient.GameScreens
{
    class MainScreen : IGameScreen
    {
        private Game m_Game;
        /// <summary>
        /// Represents the player.
        /// </summary>
        private Texture2D m_PlayerTexture;

        /// <summary>
        /// Represents the terrain the player is standing on.
        /// </summary>
        private Texture2D m_TerrainTexture;

        private Int32 m_HalfWidth;

        private Int32 m_HalfHeight;

        private Single m_AlphaLevel;

        private Single m_RotationLevel;

        private Single m_ScaleLevel;

        private Boolean m_ScalingUp;

        private Double m_FramePerSecond = 0D;
        private Double m_UpdatesPerSecond = 0D;

        private System.Diagnostics.Stopwatch Watch = new System.Diagnostics.Stopwatch();

        public MainScreen(Game Game)
        {
            m_Game = Game;
        }

        public void Initialize()
        {
            m_AlphaLevel = 1.0F;
            m_RotationLevel = 0.0F;
            m_ScaleLevel = 1.0F;
            m_ScalingUp = true;
        }

        public void LoadContent()
        {
            m_PlayerTexture = new Texture2D(m_Game.GraphicsDevice, "/Assets/Bitmaps/player.png");
            m_TerrainTexture = new Texture2D(m_Game.GraphicsDevice, "/Assets/Bitmaps/terrain.png");
        }

        public void Update(GameTime GameTime)
        {
            m_HalfWidth = m_Game.GraphicsDevice.ModeDescription.Width / 2;
            m_HalfHeight = m_Game.GraphicsDevice.ModeDescription.Height / 2;

            // Test opacity with the rendered bitmaps.
            if (m_AlphaLevel > 1.0F)
                m_AlphaLevel = 0.0F;
            else
                m_AlphaLevel += 0.01F;

            // Test rotation with rendered bitmaps.
            m_RotationLevel += 0.1F;

            if (m_ScalingUp)
            {
                if (m_ScaleLevel > 8.0F)
                    m_ScalingUp = false;
                else
                    m_ScaleLevel += 0.1F;
            }
            else
            {
                if (m_ScaleLevel < 0.5F)
                    m_ScalingUp = true;
                else
                    m_ScaleLevel -= 0.1F;
            }

            m_FramePerSecond += GameTime.DeltaTime;
            m_UpdatesPerSecond += 1;
            if (m_FramePerSecond >= 1)
            {
                System.Diagnostics.Debug.WriteLine("Updates Per Second: {0}", m_UpdatesPerSecond);
                m_UpdatesPerSecond = 0;
                m_FramePerSecond = 0D;
            }
        }

        public void Draw(GameTime GameTime)
        {
            m_Game.GraphicsDevice.BeginDraw();

            m_Game.GraphicsDevice.Clear(Color.CornflowerBlue);

            m_Game.GraphicsDevice.Transform = Matrix3x2.Translation(m_HalfWidth, m_HalfHeight - m_PlayerTexture.Size.Height);

            // Apply a rotation effect.
            m_Game.GraphicsDevice.Transform = m_Game.GraphicsDevice.Transform * Matrix3x2.Rotation(m_RotationLevel, new Vector2(m_HalfWidth + (66 / 2), (m_HalfHeight - m_PlayerTexture.Size.Height) + (92 / 2)));

            // Apply a scaling effect.
            m_Game.GraphicsDevice.Transform = m_Game.GraphicsDevice.Transform * Matrix3x2.Scaling(m_ScaleLevel, m_ScaleLevel, new Vector2(m_HalfWidth + (66 / 2), (m_HalfHeight - m_PlayerTexture.Size.Height) + (92 / 2)));

            m_Game.GraphicsDevice.DrawTexture2D(m_PlayerTexture, 1.0F);

            m_Game.GraphicsDevice.EndDraw();

            //DrawTest();

            m_Game.GraphicsDevice.Present();
        }

        private void DrawTest()
        {
            m_Game.GraphicsDevice.BeginDraw();

            m_Game.GraphicsDevice.Clear(Color.CornflowerBlue);

            m_Game.GraphicsDevice.Transform = Matrix3x2.Identity;

            m_Game.GraphicsDevice.DrawTexture2D(m_PlayerTexture, new RectangleF(0, 0, m_PlayerTexture.Size.Width, m_PlayerTexture.Size.Height), 1.0F);
            m_Game.GraphicsDevice.DrawTexture2D(m_TerrainTexture, new RectangleF(0, 0, m_TerrainTexture.Size.Width, m_TerrainTexture.Size.Height), 1.0F);

            m_Game.GraphicsDevice.EndDraw();
        }

        public void UnloadContent()
        {
            m_PlayerTexture?.Dispose();
            m_TerrainTexture?.Dispose();
        }

        public void Uninitialize()
        {

        }

        public Boolean IsDrawing
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Boolean IsUpdating
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}