using System;
using System.Collections.Generic;
using CocosSharp;

namespace DungEon
{
    public class GameStartLayer : CCLayerColor
    {
        CCSprite play;
        CCSprite quit;
        public GameStartLayer() : base(CCColor4B.Gray)
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
                GameLayer.getNewMap(GameView);
            //If quit is hit
            else if (quit.BoundingBoxTransformedToWorld.ContainsPoint(touchLocation))
                Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
        }
        protected override void AddedToScene()
        {
            base.AddedToScene();
            AddButtons();
        }
        void AddButtons()
        {
            play = new CCSprite("Play_DungEon");
            play.Position = VisibleBoundsWorldspace.Center;

            quit = new CCSprite("Quit_DungEon2");
            quit.PositionX = play.PositionX;
            quit.PositionY = play.PositionY - 160;

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