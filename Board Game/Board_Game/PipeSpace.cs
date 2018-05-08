using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PipeGame
{
    internal enum Orientation { Rotated0Degrees, Rotated90Degrees, Rotated180Degrees, Rotated270Degrees }
    internal enum Connection { Up, Right, Down, Left }

    internal class PipeSpace : GameObject
    {        
        Sprite filledSprite;
        float angle;
        Orientation currentOrientation;
        List<Connection> currentConnections;
        
        internal Orientation CurrentOrientation
        {
            get { return currentOrientation; }
            private set
            {
                currentOrientation = value;
                switch (value)
                {
                    case Orientation.Rotated0Degrees:
                        {
                            angle = 0.0F;
                            break;
                        }
                    case Orientation.Rotated90Degrees:
                        {
                            angle = (float)Math.PI/2;
                            break;
                        }
                    case Orientation.Rotated180Degrees:
                        {
                            angle = (float)Math.PI;
                            break;
                        }
                    case Orientation.Rotated270Degrees:
                        {
                            angle = (float)Math.PI*3/2;
                            break;
                        }
                    default:
                        {
                            throw new Exception("Unrecognized sprite orientation: " + value.ToString());
                        }
                }
            }
        }

        internal List<Connection> CurrentConnections { get { return currentConnections; } }

        internal bool Filled { get; set; }

        public PipeSpace(int _x, int _y, Sprite _pipeSprite, Sprite _filledPipeSprite, int _windowWidth, int _windowHeight, List<Connection> _defaultConnections) : base(_x, _y, _pipeSprite,_windowWidth,_windowHeight)
        {
            this.active = true;
            this.Filled = false;
            this.filledSprite = _filledPipeSprite;
            currentOrientation = Orientation.Rotated0Degrees;
            currentConnections = _defaultConnections;
            angle = 0.0F;
            zoom = 1;
            UpdateXYCoords(_x, _y);
        }

        internal void Draw(SpriteBatch _spriteBatch)
        {
            Sprite spriteToDraw = this.Filled ? filledSprite : sprite;
            _spriteBatch.Draw(spriteToDraw.EntireImage,this.CenterVector,null, Color.White, angle, sprite.SourceRectangleCenter,(float)zoom,SpriteEffects.None,0f);
            
        }

        internal void UpdateXYCoords(int _x, int _y)
        {
            this.X = _x;
            this.Y = _y;
        }

        internal void UpdateZoom(double _zoom)
        {
            this.zoom = _zoom;
            this.destinationRectangle = new Rectangle(this.X, this.Y, (int)(this.sprite.Width * _zoom), (int)(this.sprite.Height * _zoom));
        }

        internal void Rotate90DegreesClockwise()
        {
            CurrentOrientation = (Orientation)(((int)CurrentOrientation + 1) % 4);
            for(int i = 0; i < currentConnections.Count; ++i) 
            {
                currentConnections[i] = (Connection)(((int)currentConnections[i] + 1) % 4);
            }
        }
    }
}
