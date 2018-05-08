using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace PipeGame
{
    class GameObject
    {
        #region Fields

        protected int WINDOW_WIDTH;
        protected int WINDOW_HEIGHT;

        protected const int REFRESH_RATE = 16;
        protected const int ELAPSED_TIME = 0;
        
        protected Sprite sprite;
        protected Rectangle destinationRectangle;
        protected Point currentCenter;
        protected Vector2 centerVector;

        protected double zoom;
        protected double minZoom = 0.05;
        protected double maxZoom = 2;
        protected double zoomIncrement = 0.05;
        
        protected bool active;

        #endregion

        #region Properties

        //Read-Write Properties
        internal int X
        {
            get
            {
                return destinationRectangle.X;
            }
            set
            {
                destinationRectangle.X = value;
                currentCenter.X = destinationRectangle.X + destinationRectangle.Width / 2;
                centerVector.X = currentCenter.X;
            }
        }
        internal int Y
        {
            get
            {
                return destinationRectangle.Y;
            }
            set
            {
                destinationRectangle.Y = value;
                currentCenter.Y = destinationRectangle.Y + destinationRectangle.Height / 2;
                centerVector.Y = currentCenter.Y;
            }
        }

        //Read-Only Properties
        internal int Width { get { return destinationRectangle.Width; } }
        internal int Height { get { return destinationRectangle.Height; } }

        internal Vector2 CenterVector { get { return centerVector; } }
        internal Point Center { get { return currentCenter; } }
        internal double Zoom { get { return zoom; } }

        #endregion

        #region Constructors

        internal GameObject(int _x, int _y, Sprite _sprite, int _windowWidth, int _windowHeight)
        {
            WINDOW_WIDTH = _windowWidth;
            WINDOW_HEIGHT = _windowHeight;
            sprite = _sprite;
            destinationRectangle = new Rectangle(_x, _y, _sprite.Width, _sprite.Height);
            active = true;
        }

        #endregion

        #region Methods

        internal virtual void Update()
        {
        }

        internal virtual void Draw(SpriteBatch _spriteBatch)
        {
            if (active)
            {
                _spriteBatch.Draw(sprite.EntireImage, destinationRectangle, Color.GhostWhite);
            }
        }

        internal virtual bool ContainsPoint(Point _point)
        {
            return destinationRectangle.Contains(_point);
        }

        internal void SetZoom(double _zoom)
        {
            if(_zoom > minZoom && _zoom < maxZoom)
            {
                zoom = _zoom;
                updateDestinationRectangle();
            }
        }

        internal void ZoomIn()
        {
            zoom = Math.Min(zoom + zoomIncrement, maxZoom);
            updateDestinationRectangle();
        }

        internal void ZoomOut()
        {
            zoom = Math.Max(zoom - zoomIncrement, minZoom);
            updateDestinationRectangle();
        }

        private void updateDestinationRectangle()
        {
            destinationRectangle = new Rectangle(this.X, this.Y, (int)(this.sprite.Width * zoom), (int)(this.sprite.Height * zoom));
        }

        #endregion
    }
}
