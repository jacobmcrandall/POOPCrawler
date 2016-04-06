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
    //Used for pathfinding
    class Node
    {
        private Node parentNode;

        public CCTileMapCoordinates Location { get; private set; }

        public bool IsWalkable { get; set; }

        /// Cost from start to here
        public float G { get; private set; }

        /// Estimated cost from here to end
        public float H { get; private set; }

        /// Flags whether the node is open, closed or untested by the PathFinder
        public enum NodeState {open,closed,untested};
        public NodeState State { get; set; }

        /// Estimated total cost (F = G + H)
        public float F
        {
            get { return this.G + this.H; }
        }

        /// Gets or sets the parent node. The start node's parent is always null.
        public Node ParentNode
        {
            get { return this.parentNode; }
            set
            {
                // When setting the parent, also calculate the traversal cost from the start node to here (the 'G' value)
                this.parentNode = value;
                this.G = this.parentNode.G + GetTraversalCost(this.Location, this.parentNode.Location);
            }
        }

        /// Creates a new instance of Node.
        /// <param name="x">The node's location along the X axis</param>
        /// <param name="y">The node's location along the Y axis</param>
        /// <param name="isWalkable">True if the node can be traversed, false if the node is a wall</param>
        /// <param name="endLocation">The location of the destination node</param>
        public Node(int x, int y, bool isWalkable, CCTileMapCoordinates endLocation)
        {
            this.Location = new CCTileMapCoordinates(x,y);
            this.State = NodeState.untested;
            this.IsWalkable = isWalkable;
            this.H = GetTraversalCost(this.Location, endLocation);
            this.G = 0;
        }
        /// Gets the distance between two points
        internal static float GetTraversalCost(CCTileMapCoordinates location, CCTileMapCoordinates otherLocation)
        {
            float deltaX = Math.Abs(otherLocation.Column - location.Column);
            float deltaY = Math.Abs(otherLocation.Row - location.Row);
            return (float)Math.Sqrt( deltaX + deltaY );
        }
    }
}