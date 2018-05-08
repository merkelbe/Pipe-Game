using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PipeGame
{

    internal class PipeGraph
    {
        #region Fields

        int width;
        int height;
        Dictionary<Point, List<Point>> allConnections;

        #endregion

        #region Properties

        internal int Width { get { return width; } }
        internal int Height { get { return height; } }
        internal Dictionary<Point, List<Point>> AllConnections { get { return allConnections; } }

        #endregion

        #region Constructor

        // Looooong constructor.  Sets up a board of the specified dimensions where every point is connected to every other point by randomly adding connections in a convoluted way.
        // After constructed, can use 'allConnections' variable to create the actual pipe squares that the game will have.
        internal PipeGraph(int _width, int _height)
        {
            width = _width;
            height = _height;

            // Group ID is what current connection group a point is apart of.  
            // When a connection is made between 2 groups, they all will take the lower group number.
            Dictionary<int, List<Point>> allPointsByGroupID = new Dictionary<int, List<Point>>();
            allPointsByGroupID.Add(int.MaxValue, new List<Point>());

            Dictionary<Point, int> pointToGroupIDMap = new Dictionary<Point, int>();

            // Adds a vector for each pipe in the grid.
            for (int x = 0; x < _width; ++x)
            {
                for (int y = 0; y < _height; ++y)
                {
                    Point vector = new Point(x, y);
                    allPointsByGroupID[int.MaxValue].Add(vector);
                    pointToGroupIDMap.Add(vector, int.MaxValue);
                }
            }

            int currentHighestGroupNumber = -1;
            List<Point> pointsLeftToConnect = new List<Point>(allPointsByGroupID[int.MaxValue]);
            List<Point> connectionOptions = new List<Point>();
            allConnections = new Dictionary<Point, List<Point>>();

            // This ensures all points are connect to at least one other point.  There could however, be several possible subgroups that are all disconnected.  Will connect everything up later.
            while (pointsLeftToConnect.Count > 0)
            {
                int randomIndex = m.Random.Next(pointsLeftToConnect.Count);
                Point randomPoint = pointsLeftToConnect[randomIndex];
                connectionOptions = getAllAdjacentPoints(randomPoint);
                randomIndex = m.Random.Next(connectionOptions.Count);
                Point randomConnection = connectionOptions[randomIndex];
                
                // Updates all connections variable 2 ways (random point is connected to random connection AND random connection is connected to random point).
                if (!allConnections.ContainsKey(randomPoint))
                {
                    allConnections.Add(randomPoint, new List<Point>());
                }
                if (!allConnections.ContainsKey(randomConnection))
                {
                    allConnections.Add(randomConnection, new List<Point>());
                }
                allConnections[randomPoint].Add(randomConnection);
                allConnections[randomConnection].Add(randomPoint);
                
                // Both points are not apart of a group, give them group number.
                if (allPointsByGroupID[int.MaxValue].Contains(randomConnection))
                {
                    allPointsByGroupID.Add(++currentHighestGroupNumber, new List<Point>() { randomPoint, randomConnection });
                    allPointsByGroupID[int.MaxValue].Remove(randomPoint);
                    allPointsByGroupID[int.MaxValue].Remove(randomConnection);

                    pointToGroupIDMap[randomPoint] = currentHighestGroupNumber;
                    pointToGroupIDMap[randomConnection] = currentHighestGroupNumber;
                }
                // Point connecting to already is apart of a group.  Give it that group number.
                else
                {
                    int groupNumber = pointToGroupIDMap[randomConnection];

                    allPointsByGroupID[groupNumber].Add(randomPoint);
                    allPointsByGroupID[int.MaxValue].Remove(randomPoint);

                    pointToGroupIDMap[randomPoint] = groupNumber;
                }

                pointsLeftToConnect.Remove(randomPoint);
                pointsLeftToConnect.Remove(randomConnection);
            }

            // Now we need to connect groups together so there's only one group (or in other words, the entire pipe array is connected).
            for(int i = currentHighestGroupNumber; i > 0; --i)
            {
                bool connectionToAnotherGroupFound = false;

                // Starting with the highest group and working our way down, we start connecting groups to another.
                // Using for loops instead of foreach since we'll be mucking around with the variables we're looping through.
                //foreach(Vector2 point in allPointsByGroupID[i])
                for(int j = 0; j < allPointsByGroupID[i].Count; ++j)
                {
                    Point point = allPointsByGroupID[i][j];
                    List<Point> allAdjacentPoints = getAllAdjacentPoints(point);
                    for(int k = 0; k < allAdjacentPoints.Count; ++k)
                    {
                        Point possibleConnectingPointInAnotherGroup = allAdjacentPoints[k];
                        int otherPointsGroupNumber = pointToGroupIDMap[possibleConnectingPointInAnotherGroup];
                        if (otherPointsGroupNumber < i)
                        {
                            // Found a way to connect two disconnected groups.
                            // Need to update allConnections.  
                            // At this point, we know every point is at least connected to one other point, so we don't have to check if connection key exists.
                            connectionToAnotherGroupFound = true;

                            allConnections[point].Add(possibleConnectingPointInAnotherGroup);
                            allConnections[possibleConnectingPointInAnotherGroup].Add(point);

                            foreach(Point pointInGroup in allPointsByGroupID[i])
                            {
                                // Need to continually update groups as they eventually go to 0, because they might connect to other groups in the meantime.
                                pointToGroupIDMap[pointInGroup] = otherPointsGroupNumber;
                            }

                            allPointsByGroupID[otherPointsGroupNumber].AddRange(allPointsByGroupID[i]);
                            allPointsByGroupID.Remove(i);
                            
                            break;
                        }
                    }

                    if (connectionToAnotherGroupFound)
                    {
                        break;
                    }
                }
            }

            // Should be fully connected graph now.  Now all that's left to do is draw it.

        }

        #endregion

        #region Methods

        List<Point> getAllAdjacentPoints(Point _point)
        {
            List<Point> adjacentPoints = new List<Point>();
            
            // Figures out where point can connect to orthogonally.
            // Only time it can't add all 4 options is if it's on an edge of the 2D array.
            if (_point.X > 0)
            {
                adjacentPoints.Add(new Point(_point.X - 1, _point.Y));
            }
            if (_point.X < width - 1)
            {
                adjacentPoints.Add(new Point(_point.X + 1, _point.Y));
            }
            if (_point.Y > 0)
            {
                adjacentPoints.Add(new Point(_point.X, _point.Y - 1));
            }
            if (_point.Y < height - 1)
            {
                adjacentPoints.Add(new Point(_point.X, _point.Y + 1));
            }

            return adjacentPoints;
        }



        #endregion
    }


}
