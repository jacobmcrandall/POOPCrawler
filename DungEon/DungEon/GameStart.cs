using System;
using System.Collections.Generic;
using CocosSharp;

namespace DungEon
{
    public class GameStartLayer : CCLayerColor
    {
        CCSprite play;
        CCSprite quit;
        public GameStartLayer() : base(CCColor4B.LightGray)
        {
            var touchListener = new CCEventListenerTouchAllAtOnce();
            touchListener.OnTouchesEnded = handleEndTouches;
            AddEventListener(touchListener);
        }
        //Handle all touches for this scene
        void handleEndTouches(System.Collections.Generic.List<CCTouch> touches, CCEvent touchEvent)
        {
            var touchLocation = touches[0].Location;
            //If play is hit
            if (play.BoundingBoxTransformedToWorld.ContainsPoint(touchLocation))
            {
                //Transfer to gameplay scene
                CCScene gamePlayScene = new CCScene(GameView);
                gamePlayScene.AddLayer(new GameLayer());

                GameView.Director.ReplaceScene(gamePlayScene);
            }
            //If quit is hit
            else if (quit.BoundingBoxTransformedToWorld.ContainsPoint(touchLocation))
            {
                Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
                //System.Exit(0) may work too
            }
        }
        protected override void AddedToScene()
        {
            base.AddedToScene();
            AddButtons();
        }
        void AddButtons()
        {
            play = new CCSprite("play");
            play.Position = VisibleBoundsWorldspace.Center;

            quit = new CCSprite("quit");
            quit.PositionX = play.PositionX;
            quit.PositionY = play.PositionY - 200;

            AddChild(play);
            AddChild(quit);
        }
        public static CCScene GameStartLayerScene(CCScene mainWindow)
        {
            var scene = new CCScene(mainWindow);
            var layer = new GameStartLayer();

            scene.AddChild(layer);

            return scene;
        }
    }
}