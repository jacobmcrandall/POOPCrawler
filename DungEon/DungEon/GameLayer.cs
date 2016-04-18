using System;
using System.Collections.Generic;
using System.IO;
using CocosSharp;
using Microsoft.Xna.Framework;

namespace DungEon
{
    /*
        TODO:
        add in useItem and armor
        TODO - add in labels for health,attack & level - currently they are there but they look horrrrrrible and don't scale to level size

    */

    /*
    general stuff / explanations of confusing stuff
    TILEMAPS : A tile map contains what's called a "TileLayersContainer" which holds layers of maps
    so in level 1 for example there are 3 layers, a layer for the ground, a layer for water, and a layer for objects, like the door and user
    tiles don't exist in the map they exist in the tile layers container which is basically a list of layers that hold tiles
    so to interact with tiles do a foreach layer in tilelayerscontainer OR do a TileMap.LayerNamed("LayerName") to get a reference to a layer

    COORDINATES : There is a difference between WORLD coordinates and TILECOORDINATES - tile cooridnates are like row 5, column 3
    world coordinates are by pixel - so sometimes we have to translate between the two
    generally if you need to reference a tile use tilecoordinates - when interacting with objects ON tiles or touches use WORLD coordinates
    check touchlistener for some examples of this
    */
    public class GameLayer : CCTileMap
    {
        static int numLevels = 6; //The number of levels that we have - we load a level 1 through numLevels randomly
        CCEventListenerTouchAllAtOnce touchListener;
        character user = null; // start off as null - looptiles will spawn

        bool ifdied = false;
        CCSprite died;

        List<character> enemiesList = new List<character>();
        List<CCPoint> userMoves = new List<CCPoint>();
        //UI for the user
        CCLabel userInfo;

        //statically loaded - this could be done better - but if you want to add weapons do so here
        List<weapon> availableWeapons = new List<weapon>()
        {   new weapon("wep_1", 2),
            new weapon("wep_1", 2),
            new weapon("wep_1", 2),
            new weapon("wep_2", 3),
            new weapon("wep_2", 3),
            new weapon("wep_2", 3),
            new weapon("wep_3", 4),
            new weapon("wep_3", 4),
            new weapon("wep_3", 4)};

        //List<String> availableLevels = new List<String>()
        //{
        //    "level_3.tmx",
        //    "level_4.tmx",
        //    "level_5.tmx",
        //    "level_6.tmx"
        //}

        public GameLayer() : base("level_" + CCRandom.GetRandomInt(3, numLevels) + ".tmx"/*"level_5.tmx"*/) // get a random level and load it on initialization
        {
            character.map = this;
            //touch listener - calles tileHandler to handle tile touches
            touchListener = new CCEventListenerTouchAllAtOnce();
            touchListener.OnTouchesEnded = handleEndTouches;
            AddEventListener(touchListener);

            loopTiles(null);
            //Schedule the main game loop
            enemiesList = placeEnemiesRandomly();
            foreach (character enemy in enemiesList)
                this.AddChild(enemy);

            //Add labels to the upper left hand corner
            //Might be better to have a bar with width based on a percentage of health/maxhealth
            userInfo = new CCLabel
                ("Health: " + user.health + "/" + user.maxHealth + "    Attack : " + user.weapon.attack,"arial",12);
            userInfo.Position = new CCPoint(70, VisibleBoundsWorldspace.UpperRight.Y + 5);
            userInfo.IsAntialiased = true;
            this.AddChild(userInfo);

            //run main game loop - frames happen every 1 second
            Schedule(RunGameLogic,(float)0.5);
        }
        private GameLayer(character oldUser) : base("level_" + CCRandom.GetRandomInt(3, numLevels) + ".tmx"/*"level_4.tmx"*/) // get a random level and load it on initialization
        {
            //int tileDimension = (int)TileTexelSize.Width;
            //int numberOfColumns = (int)MapDimensions.Size.Width;
            //int numberOfRows = (int)MapDimensions.Size.Height;
            //CCPointI world = new CCPointI(0, 0);
            //foreach (CCTileMapLayer layer in TileLayersContainer.Children)
            //{
            //    // Loop through the columns and rows to find all tiles
            //    for (int column = 0; column < numberOfColumns; column++)
            //    {
            //        // We're going to add tileDimension / 2 to get the position
            //        // of the center of the tile - this will help us in 
            //        // positioning entities, and will eliminate the possibility
            //        // of floating point error when calculating the nearest tile:
            //        world.X = tileDimension * column + tileDimension / 2;
            //        for (int row = 0; row < numberOfRows; row++)
            //        {
            //            // See above on why we add tileDimension / 2
            //            world.Y = tileDimension * row + tileDimension / 2;
            //            tileHandler(world, layer, true);
            //        }
            //    }
            //}
            character.map = this;
            //user = new character("userChar", oldUser.health, world, user.weapon);
            //new character("userChar", 20, world, availableWeapons[0]);
            //user.Position = world;
            //Layer.AddChild(user);
            //touch listener - calles tileHandler to handle tile touches
            touchListener = new CCEventListenerTouchAllAtOnce();
            touchListener.OnTouchesEnded = handleEndTouches;
            AddEventListener(touchListener);

            loopTiles(oldUser);
            //Schedule the main game loop
            enemiesList = placeEnemiesRandomly();
            foreach (character enemy in enemiesList)
                this.AddChild(enemy);

            //Add labels to the upper left hand corner
            //Might be better to have a bar with width based on a percentage of health/maxhealth
            userInfo = new CCLabel
                ("Health: " + user.health + "/" + user.maxHealth + "    Attack : " + user.weapon.attack, "arial", 12);
            userInfo.Position = new CCPoint(70, VisibleBoundsWorldspace.UpperRight.Y + 5);
            userInfo.IsAntialiased = true;
            this.AddChild(userInfo);

            //run main game loop - frames happen every 1 second
            Schedule(RunGameLogic, (float)0.5);
        }

        //Handle all touches
        void handleEndTouches(System.Collections.Generic.List<CCTouch> touches, CCEvent touchEvent)
        {
            var touchLocation = touches[0].Location; //get our touch location in WORLD coords
            if (ifdied && died.BoundingBoxTransformedToWorld.ContainsPoint(touchLocation))
            {
                died.RotationX = 30;
                //CCScene mainWindow = new CCScene(GameView);
                //GameStartLayer.GameStartLayerScene(mainWindow);
                //mainWindow.RunWithScene(gameScene);
                GameLayer.getNewMap(GameView);
                //Window.DefaultDirector.ReplaceScene(GameLayer.GameScene(Window));
                //Scene.AddChild(layer);
            }
            foreach (CCTileMapLayer layer in TileLayersContainer.Children) // check every layer that was touched
            {
                var tileCoordinates = layer.ClosestTileCoordAtNodePosition(touchLocation); //get the closest tile to our touch
                var world = layer.TilePosition(tileCoordinates); //translate this tile to WORLD coordinates

                

                

                //Center our coordinates to the center of our tile
                world = new CCPoint(world.X + TileTexelSize.Width / 2, world.Y + TileTexelSize.Width / 2);
                tileHandler(world, layer, null);
            }
            
            userInfo.Text = "Health: " + user.health + "/" + user.maxHealth + "    Attack : " + user.weapon.attack;
        }

        void RunGameLogic(float frameTimeInSeconds)
        {
            loopTiles(null);
            if (userMoves.Count != 0)
            {
                
                user.healFor(1); 
                //TODO - call user.MoveOne() - for each item in an array of moveOne calls - generated by our path finding algorithm

                user.moveOne(userMoves[0]);
                foreach (character enemy in enemiesList)
                {
                    CCTileMapCoordinates positionAsTile = LayerNamed("Map").ClosestTileCoordAtNodePosition(enemy.Position);
                    if (!isUserNear(positionAsTile)) //dont do twice
                        enemy.moveOneRandom();
                    else
                    {
                        if (user.attacked(enemy.weapon.attack) && !ifdied)
                            userDeath();
                    }

                    enemy.healFor(1);
                }
                userMoves.Remove(userMoves[0]);
                userInfo.Text = "Health: " + user.health + "/" + user.maxHealth + "    Attack : " + user.weapon.attack;
            }
            else
            {
                foreach (CCTileMapLayer layer in TileLayersContainer.Children) // check every layer that was touched
                {
                    CCTileMapCoordinates tileAtXy = layer.ClosestTileCoordAtNodePosition(user.Position); // get tile coordinates corresponding to our touch location
                    CCTileGidAndFlags info = layer.TileGIDAndFlags(tileAtXy.Column, tileAtXy.Row);
                    //CCTileMapCoordinates positionAsTile = LayerNamed("Map").ClosestTileCoordAtNodePosition(enemy.Position);
                    Dictionary<string, string> properties = TilePropertiesForGID(info.Gid);
                    if (properties != null && properties.ContainsKey("name") && properties["name"] == "spawn" && enemiesList.Count == 0)
                    {
                        GameLayer.getNewMap(GameView, user);
                    }
                }
            }

        }

        //Calls the tileHandler on EVERY map tile
        void loopTiles(character oldUser)
        {
            int tileDimension = (int)TileTexelSize.Width;
            int numberOfColumns = (int)MapDimensions.Size.Width;
            int numberOfRows = (int)MapDimensions.Size.Height;
            CCPointI world = new CCPointI(0, 0);
            // Tile maps can have multiple layers, so let's loop through all of them:
            foreach (CCTileMapLayer layer in TileLayersContainer.Children)
            {
                // Loop through the columns and rows to find all tiles
                for (int column = 0; column < numberOfColumns; column++)
                {
                    // We're going to add tileDimension / 2 to get the position
                    // of the center of the tile - this will help us in 
                    // positioning entities, and will eliminate the possibility
                    // of floating point error when calculating the nearest tile:
                    world.X = tileDimension * column + tileDimension / 2;
                    for (int row = 0; row < numberOfRows; row++)
                    {
                        // See above on why we add tileDimension / 2
                        world.Y = tileDimension * row + tileDimension / 2;
                        tileHandler(world, layer, oldUser, true);
                    }
                }
            }
        }

        //Handles individual tiles at individual layers
        //Some things shouldn't be done by the loop such as exiting the game - so use the calledByLoop to differentiate between the two
        //if this gets messy could split this function into two for each use
        void tileHandler(CCPoint world, CCTileMapLayer layer, character oldUser, Boolean calledByLoop=false)
        {
            CCTileMapCoordinates tileAtXy = layer.ClosestTileCoordAtNodePosition(world); // get tile coordinates corresponding to our touch location
            CCTileGidAndFlags info = layer.TileGIDAndFlags(tileAtXy.Column, tileAtXy.Row);
            //Tilemaps made in the "tiled" program can hold properties - get the properties of the tile
            Dictionary<string, string> properties = TilePropertiesForGID(info.Gid); 

            //Clicking a walkable tile
            if (!calledByLoop && properties != null && properties.ContainsKey("walkable") && properties["walkable"] == "true")
            {
                //if tile has an enemy and user is near it
                if(isTileOccupied(tileAtXy) && isUserNear(tileAtXy))
                {
                    //character enemy = getEnemyAt(tileAtXy);
                    character attackedEnemy = getEnemyAt(tileAtXy);
                    if (attackedEnemy.attacked(user.weapon.attack))
                        enemyDeath(attackedEnemy);

                    foreach (character enemy in enemiesList)
                    {
                        CCTileMapCoordinates positionAsTile = LayerNamed("Map").ClosestTileCoordAtNodePosition(enemy.Position);
                        if (!isUserNear(positionAsTile))
                            enemy.moveOneRandom();
                        else
                        {
                                if (user.attacked(enemy.weapon.attack) && !ifdied)
                                {
                                    userDeath();
                                    break;
                                }
                            
                        }
                    }
                }
                //TODO - if tile has an item, pick it up
                if(!isTileOccupied(tileAtXy))
                    userMoves = user.move(world); //get new pathfind
            }
            //Spawning in the user - only happens once - not called by user
            if (user == null && calledByLoop && properties != null && properties.ContainsKey("name") && properties["name"] == "exit")
            {
                if(oldUser==null)
                {
                    user = new character("userChar", 20, world, availableWeapons[0]);
                    layer.AddChild(user);
                }
                else
                {
                    user = new character(oldUser, world);
                    layer.AddChild(user);                }
            }
            //Exiting the map
            //if (!calledByLoop && properties != null && properties.ContainsKey("name") && properties["name"] == "exit" && enemiesList.Count == 0)
            //{
            //    GameLayer.getNewMap(GameView);
            //}
        }

        void userDeath()
        {
            //TODO - handle user deaths here
            //var touch = new CCTouch;
            //died = new CCTextField("You died", "arial", 10);
            died = new CCSprite("PlayAgain_DungEon3");
            died.Position = VisibleBoundsWorldspace.Center;

            ifdied = true;
            //died.TouchEnded() = handleDiedTouch;


            AddChild(died);

            Console.WriteLine("You died");
        }

        void enemyDeath(character enemy)
        {
            if(enemy.weapon.attack > user.weapon.attack)
            {
                user.RemoveChild(user.weapon);
                user.weapon = enemy.weapon;
                user.AddChild(user.weapon);
            }
            enemiesList.Remove(enemy);
            this.RemoveChild(enemy);
            //TODO - drop item
        }

        character getEnemyAt(CCTileMapCoordinates checkHere)
        {
            
            foreach (character enemy in enemiesList)
            {
             CCTileMapCoordinates enemyLocation = LayerNamed("Map").ClosestTileCoordAtNodePosition(enemy.Position);
                if (enemyLocation.Column == checkHere.Column && enemyLocation.Row == checkHere.Row)
                    return enemy;
            }
            return null;
        }

        List<character> placeEnemiesRandomly()
        {
            List<character> enemies = new List<character>();
            int tileDimension = (int)TileTexelSize.Width;
            int numberOfColumns = (int)MapDimensions.Size.Width;
            int numberOfRows = (int)MapDimensions.Size.Height;
            CCTileMapCoordinates randomTile;
            CCPoint randomLocation;
            for (int i =0; i < Int32.Parse(MapPropertyNamed("numEnemies")); i++)
            {   
                int randCol = CCRandom.GetRandomInt(0, numberOfColumns - 1);
                int randRow = CCRandom.GetRandomInt(0, numberOfRows - 1);

                randomTile = new CCTileMapCoordinates(randCol, randRow);

                //if you randomly chose a non-walkable tile OR another char is on that tile
                if (character.checkSingleTileWithProperties(randomTile, "walkable", "true")
                    && !isTileOccupied(randomTile))
                {

                    randomLocation = LayerNamed("Map").TilePosition(randomTile);
                    randomLocation = new CCPointI((int)randomLocation.X + tileDimension / 2, (int)randomLocation.Y + tileDimension / 2);
                    enemies.Add(new character("enemyChar", 5, randomLocation, availableWeapons[i+1] ));//CCRandom.GetRandomInt(0,availableWeapons.Count-1)]  ));
                }
                else
                    i--;
                
            }
            return enemies;
        }

        //check if the user occupies a tile adjacent to an enemy
        bool isUserNear(CCTileMapCoordinates hostPosition)
        {
            CCTileMapCoordinates up, down, left, right;
            up = new CCTileMapCoordinates(hostPosition.Column, hostPosition.Row-1);
            down = new CCTileMapCoordinates(hostPosition.Column, hostPosition.Row+1);
            left = new CCTileMapCoordinates(hostPosition.Column-1, hostPosition.Row);
            right = new CCTileMapCoordinates(hostPosition.Column+1, hostPosition.Row);

            CCTileMapCoordinates userPosition = LayerNamed("Map").ClosestTileCoordAtNodePosition(user.Position);
            //first check if user occupies a tile
            if (userPosition.Column == up.Column && userPosition.Row == up.Row)
                return true;
            if (userPosition.Column == down.Column && userPosition.Row == down.Row)
                return true;
            if (userPosition.Column == left.Column && userPosition.Row == left.Row)
                return true;
            if (userPosition.Column == right.Column && userPosition.Row == right.Row)
                return true;
            return false;



        }

        public Boolean isTileOccupied(CCTileMapCoordinates checkHere)
        {
            CCTileMapCoordinates characterPosition = LayerNamed("Map").ClosestTileCoordAtNodePosition(user.Position);
            //first check if user occupies a tile
            if (characterPosition.Column == checkHere.Column && characterPosition.Row == checkHere.Row)
                return true;
            //check if any enemies occupy that tile
            foreach (character enemy in enemiesList)
            {
                characterPosition = LayerNamed("Map").ClosestTileCoordAtNodePosition(enemy.Position);
                if (characterPosition.Column == checkHere.Column && characterPosition.Row == checkHere.Row)
                    return true;
            }
            //UNOCCUPIED!
            return false;
        }

        //Adds a random tilemap - call this to load a new level
        public static void getNewMap(CCGameView gameView)
        {
            CCScene gamePlayScene = new CCScene(gameView);
            gamePlayScene.AddLayer(new GameLayer());
            gameView.Director.ReplaceScene(gamePlayScene);
        }
        //Carry user over to next level
        private static void getNewMap(CCGameView gameView, character user)
        {
            CCScene gamePlayScene = new CCScene(gameView);
            gamePlayScene.AddLayer(new GameLayer(user));
            gameView.Director.ReplaceScene(gamePlayScene);
        }
        //add items to scene here
        protected override void AddedToScene()
        {
            //Sets our screen size appropriately
            GameView.DesignResolution = new CCSizeI(
                                          (int)(MapDimensions.Size.Width * TileTexelSize.Height)
                                        , (int)(MapDimensions.Size.Height * TileTexelSize.Height));
            base.AddedToScene();
        }
        public static CCScene GameScene(CCGameView mainWindow)
        {
            var scene = new CCScene(mainWindow);
            var layer = new GameLayer();
            scene.AddChild(layer);

            return scene;
        }
    }
}
