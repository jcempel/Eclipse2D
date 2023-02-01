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
    class TitleScreen : IGameScreen
    {
        private Game m_Game;

        private Int32 m_Width;

        private Int32 m_Height;

        private Single m_Opacity;

        private Double m_ElapsedTime;

        private Texture2D m_GameLogo;

        public TitleScreen(Game Game)
        {
            m_Game = Game;
        }

        public void Initialize()
        {
            m_Opacity = 0F;
            m_ElapsedTime = 0D;
        }

        public void LoadContent()
        {
            m_GameLogo = new Texture2D(m_Game.GraphicsDevice, "/Assets/Bitmaps/eclipse2dlogo.png");
        }

        public void Update(GameTime GameTime)
        {
            m_Width = m_Game.GraphicsDevice.ModeDescription.Width;
            m_Height = m_Game.GraphicsDevice.ModeDescription.Height;

            if (m_Opacity != 1.0F)
            {
                if (m_Opacity > 1.0F)
                    m_Opacity = 1.0F;
                else
                    m_Opacity = m_Opacity + 0.01F;
            }

            if (m_Opacity == 1.0F)
            {
                // Wait three seconds, and then proceed to the next game screen.
                if (m_ElapsedTime < 3)
                    m_ElapsedTime += GameTime.DeltaTime;
                else
                    m_Game.ScreenManager.SetGameScreen("MainScreen");
            }

            if (Mouse.GetState().IsButtonPressed(MouseButton.LeftButton))
                m_Game.ScreenManager.SetGameScreen("MainScreen");
        }

        public void Draw(GameTime GameTime)
        {
            m_Game.GraphicsDevice.BeginDraw();

            m_Game.GraphicsDevice.Clear(Color.Black);

            m_Game.GraphicsDevice.DrawTexture2D(m_GameLogo, new RectangleF(0, 0, m_Width, m_Height), m_Opacity);

            m_Game.GraphicsDevice.EndDraw();

            m_Game.GraphicsDevice.Present(1);
        }

        public void UnloadContent()
        {
            m_GameLogo.Dispose();
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