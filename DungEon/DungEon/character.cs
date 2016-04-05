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
    /*CHARACTER NOTES
    - Originally this was gonna be split up
    */
    class character : CCNode //Inherits CCNode - a cocossharp class meaning its basically a placeable/trackable object
    {
        
        CCSprite myChar { get; } //Just the visual for my character - static - no animation yet   
        int health { get; set; } 
        //Health of character - we were gonna have health functions but I think it might just be easier/ less complicated touse getters/ setters
        int maxHealth { get; } //Max health of character - gets set at initialization
        public static GameLayer map { get; set; }  // the map that this character is placed on
        //TODO add items class variables once items are implemented
        public weapon weapon { get; set; }
        //public bool inCombat { get; set; }


        public character(string spriteString,int setHealth,CCPoint setLocation, weapon equippedWeapon ) : base()
        {
            myChar = new CCSprite(spriteString); //set sprite based on the name of a passed in string
            this.AddChild(myChar); //connect the sprite to the character
            this.Position = setLocation;
            health = setHealth;
            maxHealth = setHealth;

            weapon = equippedWeapon;
            this.AddChild(weapon);
            AddChild(weapon);
            
        }

        //returns a list of tiles with a certain property and value - checks up,down,left,right nodes
        //if you're in the upper left hand corner for instance and check for walkable tiles it will return bottom and right coords
        public List<CCTileMapCoordinates> getSurroundingTilesWithProperties(CCPoint myPosition,string property, string value)
        {
            var adjacentCoordinates = new List<CCTileMapCoordinates>();
            foreach (CCTileMapLayer layer in map.TileLayersContainer.Children)
            {
                CCTileMapCoordinates currentTile = layer.ClosestTileCoordAtNodePosition(myPosition); // (touchlocation if depending on who passed in)
                CCTileMapCoordinates up, left, right, down;
                //Up
                up = new CCTileMapCoordinates(currentTile.Column + 1, currentTile.Row);
                if (checkSingleTileWithProperties(up, property, value))
                    adjacentCoordinates.Add(up);
                //Left
                left = new CCTileMapCoordinates(currentTile.Column, currentTile.Row - 1);
                if (checkSingleTileWithProperties(left, property, value))
                    adjacentCoordinates.Add(left);
                //Down
                down = new CCTileMapCoordinates(currentTile.Column - 1, currentTile.Row);
                if (checkSingleTileWithProperties(down, property, value))
                    adjacentCoordinates.Add(down);
                //Right
                right = new CCTileMapCoordinates(currentTile.Column, currentTile.Row + 1);
                if (checkSingleTileWithProperties(right, property, value))
                    adjacentCoordinates.Add(right);
            }
            

            return adjacentCoordinates;
        }

        //helper function for getSurroundingTilesWithProperties - also can be used individually
        //check a single tile if it satisfies a certain condition ie "walkable" == "true"
        public static Boolean checkSingleTileWithProperties(CCTileMapCoordinates checkTile,string property, string value)
        {
            CCTileGidAndFlags info;
            Dictionary<string, string> properties;
            foreach (CCTileMapLayer layer in map.TileLayersContainer.Children)
            {
                try
                {
                    info = layer.TileGIDAndFlags(checkTile.Column, checkTile.Row);
                    properties = map.TilePropertiesForGID(info.Gid);
                    if (properties.ContainsKey(property) && properties[property] == value)
                    {
                        return true;
                    }
                        
                }
                catch { }
            }
            return false;
            
        }

        //TODO
        void attack()
        {

        }

        //TODO - but I'm thinking it might be easier to just interact with the health variable directly in the main gameloop
        void healUser()
        {

        }

        public void move(CCPoint moveHere)
        {
            if (!map.isTileOccupied(map.LayerNamed("Map").ClosestTileCoordAtNodePosition(moveHere)))
                moveOne(moveHere);
            else
                Console.WriteLine("Can't move there");
        }

        //move to a specified location
        //NOTE: This doesn't check that it is necessarily a one tile move, it checks that it is a valid move of one action
        void moveOne(CCPoint moveHere)
        {
            CCTileMapCoordinates moveHereTile = map.LayerNamed("Map").ClosestTileCoordAtNodePosition(moveHere);
            if (checkSingleTileWithProperties(moveHereTile, "walkable", "true")) // if the above space is walkable you can move there
            {
                this.Position = moveHere;
            }
            else
            {
                Console.WriteLine("Cannot move there, invalid,");
            }           
        }

        public void moveOneRandom()
        {
            var walkableTiles = getSurroundingTilesWithProperties(this.Position,"walkable", "true");
            //get a random tile to moveto
            CCTileMapCoordinates moveTo = walkableTiles[CCRandom.GetRandomInt(0,walkableTiles.Count-1)];
            CCPoint moveToWorld = map.LayerNamed("Map").TilePosition(moveTo);

            moveToWorld = new CCPoint(moveToWorld.X + map.TileTexelSize.Width / 2, moveToWorld.Y + map.TileTexelSize.Width / 2);
            if (!map.isTileOccupied(moveTo))
                moveOne(moveToWorld);
            else
                Console.WriteLine("Can't move here");
        }
    }
}