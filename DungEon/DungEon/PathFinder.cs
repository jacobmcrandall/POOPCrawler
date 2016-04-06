using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CocosSharp;

namespace DungEon
{
    class PathFinder
    {
        CCTileMap map;
        CCTileMapLayer mapLayer;
        CCTileMapCoordinates startTile, endTile;
        Node startNode, endNode;
        Node[,] nodes;

        public PathFinder(CCTileMap setMap)
        {
            map = setMap;
            mapLayer = map.LayerNamed("Map");
        }

        public List<CCPoint> FindPath (CCPoint start, CCPoint end)
        {
            startTile = mapLayer.ClosestTileCoordAtNodePosition(start);
            endTile = mapLayer.ClosestTileCoordAtNodePosition(end);
            InitializeMap();
            startNode = nodes[startTile.Column, startTile.Row];
            startNode.State = Node.NodeState.open;
            endNode = nodes[endTile.Column, endTile.Row];

            List<CCPoint> path = new List<CCPoint>();
            bool success = Search(startNode);
            if(success)
            {
                // If a path was found, follow the parents from the end node to build a list of locations
                Node node = this.endNode;
                while (node.ParentNode != null)
                {
                    var centeredPoint = mapLayer.TilePosition(node.Location);
                    centeredPoint = new CCPoint(centeredPoint.X + map.TileTexelSize.Width / 2, centeredPoint.Y + map.TileTexelSize.Width / 2);
                    path.Add(centeredPoint);
                    node = node.ParentNode;
                }

                // Reverse the list so it's in the correct order when returned
                path.Reverse();
            }

            return path;
        }

        Boolean Search(Node currentNode)
        {
            currentNode.State = Node.NodeState.closed;
            var adjacentNodes = AdjacentValidNodes(currentNode);

            // Sort by F-value so that the shortest possible routes are considered first
            adjacentNodes.Sort((node1, node2) => node1.F.CompareTo(node2.F));
            foreach(var nextNode in adjacentNodes)
            {
                if (nextNode.Location.Row == endTile.Row && nextNode.Location.Column == endTile.Column)
                    return true;
                else
                {
                    if (Search(nextNode))
                        return true;
                }
            }
            return false;
        }

        List<Node> AdjacentValidNodes(Node fromNode)
        {
            List<Node> adjacentNodes = new List<Node>();
            IEnumerable<CCTileMapCoordinates> nextLocations = GetAdjacentLocations(fromNode.Location);

            foreach (var location in nextLocations)
            {
                int column = location.Column;
                int row = location.Row;

                // Stay within the grid's boundaries
                //CHECK
                if (column < 0 || column >= map.MapDimensions.Size.Width || row < 0 || row >= map.MapDimensions.Size.Height)
                    continue;

                Node node = this.nodes[column, row];
                // Ignore non-walkable nodes
                if (!node.IsWalkable)
                    continue;

                // Ignore already-closed nodes
                if (node.State == Node.NodeState.closed)
                    continue;

                // Already-open nodes are only added to the list if their G-value is lower going via this route.
                if (node.State == Node.NodeState.open)
                {
                    float traversalCost = Node.GetTraversalCost(node.Location, node.ParentNode.Location);
                    float gTemp = fromNode.G + traversalCost;
                    if (gTemp < node.G)
                    {
                        node.ParentNode = fromNode;
                        adjacentNodes.Add(node);
                    }
                }
                else
                {
                    // If it's untested, set the parent and flag it as 'Open' for consideration
                    node.ParentNode = fromNode;
                    node.State = Node.NodeState.open;
                    adjacentNodes.Add(node);
                }
            }
            return adjacentNodes;
        }



        void InitializeMap()
        {
            int numberOfColumns = (int)map.MapDimensions.Size.Width;
            int numberOfRows = (int)map.MapDimensions.Size.Height;
            this.nodes = new Node[numberOfColumns, numberOfRows];
            // Loop through the columns and rows to find all tiles
            for (int row = 0; row < numberOfRows; row++)
            {
                for (int column = 0; column < numberOfColumns; column++)
                {
                    var currTile = new CCTileMapCoordinates(column,row);
                    bool isWalkable = character.checkSingleTileWithProperties(currTile,"walkable","true");
                    this.nodes[column, row] = new Node(column, row,isWalkable , endTile);
                }
            }
        }

        private static IEnumerable<CCTileMapCoordinates> GetAdjacentLocations(CCTileMapCoordinates fromLocation)
        {
            return new CCTileMapCoordinates[]
            {
                new CCTileMapCoordinates(fromLocation.Column-1, fromLocation.Row  ),
                new CCTileMapCoordinates(fromLocation.Column,   fromLocation.Row+1),
                new CCTileMapCoordinates(fromLocation.Column+1, fromLocation.Row  ),
                new CCTileMapCoordinates(fromLocation.Column,   fromLocation.Row-1),
            };
        }
    }
}