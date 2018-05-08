using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace PipeGame
{
    internal class VisualPipeGame
    {
        int windowXOffset;
        int windowYOffset;

        int windowWidth;
        int windowHeight;

        // A bit redundant here, but leaving it open for general rectangles.
        // This is the number of pixels each pipe piece image takes up
        const int SQUARE_WIDTH = 256;
        const int SQUARE_HEIGHT = 256; 

        // This is the number of pipe pieces in each row / column.
        int width;
        int height;

        const int maxWidth = 46;
        const int maxHeight = 26;
        
        // Displayed squares might be way different than destination rectangles of images.
        // Recording these when adjusting drawing images initially.
        List<int> displayedXOffsets;
        List<int> displayedYOffsets;

        // Boundaries for the zoom.  Zoom dynamically chosen by puzzle's dimensions
        const double minZoom = 0.1;
        const double maxZoom = 1;

        // Holds the original connection for the graph.
        PipeGraph pipeGraph;

        Sprite pipe1Sprite;
        Sprite pipe1FilledSprite;
        Sprite pipe2Sprite;
        Sprite pipe2FilledSprite;
        Sprite pipe2bSprite;
        Sprite pipe2bFilledSprite;
        Sprite pipe3Sprite;
        Sprite pipe3FilledSprite;
        Sprite pipe4Sprite;
        Sprite pipe4FilledSprite;

        PipeSpace[,] allPipeSpaces;
        int[,] randomPipeSpaceOrientations;

        Point sourceSpace;

        // puzzleSolved tracks if the player ever solved the puzzle without cheating.
        bool puzzleSolved;
        bool cheated;

        internal bool AllFilled
        {
            get
            {
                bool allFilled = this.allFilled();
                if(allFilled && !cheated)
                {
                    puzzleSolved = true;
                }
                return allFilled;
            }
        }
        
        internal bool PuzzleSolved { get { return puzzleSolved; } }
        internal bool Cheated { get { return cheated; } }
        
        #region Constructors

        internal VisualPipeGame(List<Sprite> _orderedListOfSprites, int _windowWidth, int _windowHeight, int _windowXOffset, int _windowYOffset)
        {
            windowWidth = _windowWidth;
            windowHeight = _windowHeight;

            windowXOffset = _windowXOffset;
            windowYOffset = _windowYOffset;

            width = 12;
            height = 8;
            
            // Assumption of order here!
            pipe1Sprite = _orderedListOfSprites[0];
            pipe1FilledSprite = _orderedListOfSprites[1];
            pipe2Sprite = _orderedListOfSprites[2];
            pipe2FilledSprite = _orderedListOfSprites[3];
            pipe2bSprite = _orderedListOfSprites[4];
            pipe2bFilledSprite = _orderedListOfSprites[5];
            pipe3Sprite = _orderedListOfSprites[6];
            pipe3FilledSprite = _orderedListOfSprites[7];
            pipe4Sprite = _orderedListOfSprites[8];
            pipe4FilledSprite = _orderedListOfSprites[9];
            
            initializeNewPuzzle();
        }

        #endregion

        // Called in the constructor.
        private void initializeNewPuzzle()
        {
            sourceSpace = new Point(m.Random.Next(width), m.Random.Next(height));

            pipeGraph = new PipeGraph(width, height);
            
            // Sets up all spaces.  
            // X and Y don't matter. They'll be set later along with zoom.
            // Orientation doesn't matter.  It'll be randomized later.
            allPipeSpaces = new PipeSpace[height, width];
            randomPipeSpaceOrientations = new int[height, width];

            cheated = false;
            puzzleSolved = false;

            foreach (Point point in pipeGraph.AllConnections.Keys)
            {
                List<Point> connections = pipeGraph.AllConnections[point];
                switch (connections.Count)
                {
                    case 1:
                        {
                            allPipeSpaces[point.Y, point.X] = new PipeSpace(0, 0, pipe1Sprite, pipe1FilledSprite, windowWidth, windowHeight, new List<Connection>() { Connection.Up });
                            break;
                        }
                    case 2:
                        {
                            // Need to figure out which 2 connection it is.
                            if ((point.X == connections[0].X && point.X == connections[1].X) || (point.Y == connections[0].Y && point.Y == connections[1].Y))
                            {
                                // Straight line pipe
                                allPipeSpaces[point.Y, point.X] = new PipeSpace(0, 0, pipe2bSprite, pipe2bFilledSprite, windowWidth, windowHeight, new List<Connection>() { Connection.Up, Connection.Down });
                            }
                            else
                            {
                                // Elbow pipe
                                allPipeSpaces[point.Y, point.X] = new PipeSpace(0, 0, pipe2Sprite, pipe2FilledSprite, windowWidth, windowHeight, new List<Connection>() { Connection.Up, Connection.Left });
                            }
                            break;
                        }
                    case 3:
                        {
                            allPipeSpaces[point.Y, point.X] = new PipeSpace(0, 0, pipe3Sprite, pipe3FilledSprite, windowWidth, windowHeight, new List<Connection>() { Connection.Up, Connection.Left, Connection.Down });
                            break;
                        }
                    case 4:
                        {
                            allPipeSpaces[point.Y, point.X] = new PipeSpace(0, 0, pipe4Sprite, pipe4FilledSprite, windowWidth, windowHeight, new List<Connection>() { Connection.Up, Connection.Left, Connection.Down, Connection.Right });
                            break;
                        }
                    default:
                        {
                            throw new Exception("Unrecognized number of connections.");
                        }
                }
            }

            // Measures what the max zoom for width and height could be to fit in window.
            double maxWidthZoom = (double)windowWidth / (double)width / (double)SQUARE_WIDTH;
            double maxHeightZoom = (double)windowHeight / (double)height / (double)SQUARE_HEIGHT;

            // Figures out what the overall zoom should be (min of these 2 variables, rounded to nearest twentieth).
            // Min zoom is 0.05, max zoom is 1.

            double zoom = Math.Min(Math.Max(Math.Floor(Math.Min(maxWidthZoom, maxHeightZoom) * 100) / 100, minZoom), maxZoom);
            double xOffset = windowXOffset + ((double)windowWidth - (double)(width * SQUARE_WIDTH * zoom)) / 2;
            double yOffset = windowYOffset + ((double)windowHeight - (double)(height * SQUARE_HEIGHT * zoom)) / 2;

            displayedXOffsets = new List<int>();
            for (int x = 0; x <= width; ++x)
            {
                displayedXOffsets.Add((int)(xOffset + zoom * SQUARE_WIDTH * x));
            }
            displayedYOffsets = new List<int>();
            for (int y = 0; y <= height; ++y)
            {
                displayedYOffsets.Add((int)(yOffset + zoom * SQUARE_HEIGHT * y));
            }

            for (int row = 0; row < height; ++row)
            {
                for (int col = 0; col < width; ++col)
                {
                    allPipeSpaces[row, col].X = (int)(col * zoom * SQUARE_WIDTH + xOffset - (1 - zoom) / 2 * SQUARE_WIDTH);
                    allPipeSpaces[row, col].Y = (int)(row * zoom * SQUARE_HEIGHT + yOffset - (1 - zoom) / 2 * SQUARE_HEIGHT);

                    allPipeSpaces[row, col].UpdateZoom(zoom);

                    int randomInt = m.Random.Next(4);
                    randomPipeSpaceOrientations[row, col] = randomInt;
                    for (int i = 0; i < randomInt; ++i)
                    {
                        allPipeSpaces[row, col].Rotate90DegreesClockwise();
                    }
                }
            }
            updateFilledSpaces();
        }

        private void resetPuzzle()
        {
            for(int row = 0; row < height; ++row)
            {
                for(int col = 0; col < width; ++col)
                {
                    while((int)allPipeSpaces[row,col].CurrentOrientation != randomPipeSpaceOrientations[row, col])
                    {
                        allPipeSpaces[row, col].Rotate90DegreesClockwise();
                    }
                    
                }
            }

            updateFilledSpaces();
        }

        internal void Draw(SpriteBatch _spriteBatch)
        {
            for(int row = 0; row < height; ++row)
            {
                for(int col = 0; col < width; ++col)
                {
                    allPipeSpaces[row, col].Draw(_spriteBatch);
                }
            }
        }

        internal void HandleKeyboardInput(List<Keys> _pressedKeys)
        {
            if (_pressedKeys.Contains(Keys.N))
            {
                initializeNewPuzzle();
            }
            else if (_pressedKeys.Contains(Keys.R))
            {
                resetPuzzle();
            }
            else if (_pressedKeys.Contains(Keys.H))
            {
                if (!puzzleSolved) { cheated = true; }
                moveOnePieceToCorrectPosition();
                updateFilledSpaces();
            }
            else if (_pressedKeys.Contains(Keys.S))
            {
                if (!puzzleSolved) { cheated = true; }
                solvePuzzle();
                updateFilledSpaces();
            }
            else if (_pressedKeys.Contains(Keys.Up))
            {
                if(height < maxHeight)
                {
                    ++height;
                    initializeNewPuzzle();
                }
                
            }
            else if (_pressedKeys.Contains(Keys.Down))
            {
                if(height > 1 && (height != 2 || width != 1))
                {
                    --height;
                    initializeNewPuzzle();
                }
            }
            else if (_pressedKeys.Contains(Keys.Right))
            {
                if(width < maxWidth)
                {
                    ++width;
                    initializeNewPuzzle();
                }
            }
            else if (_pressedKeys.Contains(Keys.Left))
            {
                if(width > 1 && (width != 2 || height != 1))
                {
                    --width;
                    initializeNewPuzzle();
                }
            }
        }

        internal void HandleMouseInput(ClickInfo _clickInfo)
        {
            int xCoord = _clickInfo.EndingCoordinates.X;
            int yCoord = _clickInfo.EndingCoordinates.Y;
            bool clickInArea =
                xCoord >= displayedXOffsets[0] &&
                xCoord < displayedXOffsets.Last() &&
                yCoord >= displayedYOffsets[0] &&
                yCoord < displayedYOffsets.Last();

            if (clickInArea)
            {
                for (int row = 1; row <displayedYOffsets.Count; ++row)
                {
                    if (yCoord < displayedYOffsets[row])
                    {
                        for (int col = 1; col < displayedXOffsets.Count; ++col)
                        {
                            if (xCoord < displayedXOffsets[col])
                            {
                                allPipeSpaces[row - 1, col - 1].Rotate90DegreesClockwise();
                                updateFilledSpaces();
                                return;
                            }
                        }
                    }
                }
            }
        }

        private void updateFilledSpaces()
        {
            // Clears out all spaces already filled
            for(int row = 0; row < height; ++row)
            {
                for(int col = 0; col < width; ++col)
                {
                    allPipeSpaces[row, col].Filled = false;
                }
            }

            updateFilledSpaceRecursive(sourceSpace, new List<Point>());
        }

        private void updateFilledSpaceRecursive(Point _currentPoint, List<Point> _pointsToIgnore)
        {
            allPipeSpaces[_currentPoint.Y, _currentPoint.X].Filled = true;

            if (_currentPoint.X > 0 && allPipeSpaces[_currentPoint.Y, _currentPoint.X].CurrentConnections.Contains(Connection.Left))
            {
                Point leftConnection = new Point(_currentPoint.X - 1, _currentPoint.Y);
                if (!_pointsToIgnore.Contains(leftConnection) && allPipeSpaces[leftConnection.Y, leftConnection.X].CurrentConnections.Contains(Connection.Right))
                {
                    _pointsToIgnore.Add(_currentPoint);
                    updateFilledSpaceRecursive(leftConnection, _pointsToIgnore);
                }
            }
            if (_currentPoint.X < width - 1 && allPipeSpaces[_currentPoint.Y, _currentPoint.X].CurrentConnections.Contains(Connection.Right))
            {
                Point rightConnection = new Point(_currentPoint.X + 1, _currentPoint.Y);
                if (!_pointsToIgnore.Contains(rightConnection) && allPipeSpaces[rightConnection.Y, rightConnection.X].CurrentConnections.Contains(Connection.Left))
                {
                    _pointsToIgnore.Add(_currentPoint);
                    updateFilledSpaceRecursive(rightConnection, _pointsToIgnore);
                }
            }
            if (_currentPoint.Y > 0 && allPipeSpaces[_currentPoint.Y, _currentPoint.X].CurrentConnections.Contains(Connection.Up))
            {
                Point upConnection = new Point(_currentPoint.X, _currentPoint.Y - 1);
                if (!_pointsToIgnore.Contains(upConnection) && allPipeSpaces[upConnection.Y, upConnection.X].CurrentConnections.Contains(Connection.Down))
                {
                    _pointsToIgnore.Add(_currentPoint);
                    updateFilledSpaceRecursive(upConnection, _pointsToIgnore);
                }
            }
            if (_currentPoint.Y < height - 1 && allPipeSpaces[_currentPoint.Y, _currentPoint.X].CurrentConnections.Contains(Connection.Down))
            {
                Point downConnection = new Point(_currentPoint.X, _currentPoint.Y + 1);
                if (!_pointsToIgnore.Contains(downConnection) && allPipeSpaces[downConnection.Y, downConnection.X].CurrentConnections.Contains(Connection.Up))
                {
                    _pointsToIgnore.Add(_currentPoint);
                    updateFilledSpaceRecursive(downConnection, _pointsToIgnore);
                }
            }
        }

        private void solvePuzzle()
        {
            Point currentPoint = new Point();
            List<Point> connections;
            for(int x = 0; x < width; ++x)
            {
                for(int y =0; y < height; ++y)
                {
                    currentPoint.X = x;
                    currentPoint.Y = y;
                    connections = pipeGraph.AllConnections[currentPoint];
                    List<Connection> visualConnections = allPipeSpaces[currentPoint.Y, currentPoint.X].CurrentConnections;
                    while (!inPosition(visualConnections, connections, currentPoint))
                    {
                        allPipeSpaces[y, x].Rotate90DegreesClockwise();
                        visualConnections = allPipeSpaces[currentPoint.Y, currentPoint.X].CurrentConnections;
                    }
                }
            }
        }

        private void moveOnePieceToCorrectPosition()
        {
            int randomStartingRow = m.Random.Next(height);
            int randomStartingColumn = m.Random.Next(width);

            Point currentPoint = new Point();
            List<Point> connections;

            for (int xOffset = 0; xOffset < width; ++xOffset)
            {
                for(int yOffset = 0; yOffset < height; ++yOffset)
                {
                    currentPoint.X = (randomStartingColumn + xOffset) % width;
                    currentPoint.Y = (randomStartingRow + yOffset) % height;
                    connections = pipeGraph.AllConnections[currentPoint];
                    List<Connection> visualConnections = allPipeSpaces[currentPoint.Y, currentPoint.X].CurrentConnections;
                    if (!inPosition(visualConnections, connections, currentPoint))
                    {
                        do
                        {
                            allPipeSpaces[currentPoint.Y, currentPoint.X].Rotate90DegreesClockwise();
                            visualConnections = allPipeSpaces[currentPoint.Y, currentPoint.X].CurrentConnections;
                        } while (!inPosition(visualConnections, connections, currentPoint));
                        return;
                    }
                }
            }
        }

        private bool inPosition(List<Connection> _visualConnections, List<Point> _graphConnections, Point _currentPoint)
        {
            foreach(Connection connection in _visualConnections)
            {
                switch (connection)
                {
                    case Connection.Up:
                        {
                            if(!_graphConnections.Contains(new Point(_currentPoint.X, _currentPoint.Y - 1)))
                            {
                                return false;
                            }
                            break;
                        }
                    case Connection.Right:
                        {
                            if (!_graphConnections.Contains(new Point(_currentPoint.X+1, _currentPoint.Y)))
                            {
                                return false;
                            }
                            break;
                        }
                    case Connection.Down:
                        {
                            if (!_graphConnections.Contains(new Point(_currentPoint.X, _currentPoint.Y+1)))
                            {
                                return false;
                            }
                            break;
                        }
                    case Connection.Left:
                        {
                            if (!_graphConnections.Contains(new Point(_currentPoint.X - 1, _currentPoint.Y)))
                            {
                                return false;
                            }
                            break;
                        }
                }
            }
            return true;
        }

        private bool allFilled()
        {
            for(int row = 0; row < height; ++row)
            {
                for(int col = 0; col < width; ++col)
                {
                    if (!allPipeSpaces[row, col].Filled)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
