using System;
using System.Collections.Generic;
using CocosSharp;
using Microsoft.Xna.Framework;

namespace DungEon
{
    public class GameLayer : CCLayer
    {

        // Make children public by initializing here - tile lists ,sprites, etc. go here
        CCLabel label;
        int touchCount = 0;

        public GameLayer() : base()
        {
            //https://developer.xamarin.com/guides/cross-platform/game_development/cocossharp/first_game/part2/
            var touchListener = new CCEventListenerTouchAllAtOnce();
            touchListener.OnTouchesEnded = handleEndTouches;
            AddEventListener(touchListener);

            

            Schedule(RunGameLogic);
        }
        //Handle all touches for this scene
        void handleEndTouches(System.Collections.Generic.List<CCTouch> touches, CCEvent touchEvent)
        {
            var touchLocation = touches[0].Location;
            touchCount++;
            //if (spriteName.BoundingBoxTransformedToWorld.ContainsPoint(touchLocation))
                //Do stuff
        }

        void RunGameLogic(float frameTimeInSeconds)
        {
            label.Text = "Num Touches : " + touchCount;
        }

        protected override void AddedToScene()
        {
            base.AddedToScene();
            label = new CCLabel("Num Touches : 0", "Arial", 40, CCLabelFormat.SystemFont);
            label.Position = VisibleBoundsWorldspace.Center;
            AddChild(label);

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

