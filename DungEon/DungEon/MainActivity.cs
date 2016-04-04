using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using CocosSharp;

namespace DungEon
{
    [Activity(Label = "DungEon", MainLauncher = true, Icon = "@drawable/icon",
        AlwaysRetainTaskState = true,
        LaunchMode = LaunchMode.SingleInstance,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden)]
    public class MainActivity : Activity
    {

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our game view from the layout resource,
            // and attach the view created event to it
            CCGameView gameView = (CCGameView)FindViewById(Resource.Id.GameView);
            gameView.ViewCreated += LoadGame;
        }

        void LoadGame(object sender, EventArgs e)
        {
            CCGameView gameView = sender as CCGameView;

            if (gameView != null)
            {

                gameView.ContentManager.SearchPaths = new List<string>()
                {"Images", "Fonts","Sounds", "Tiles"};

                //CCScene.SetDefaultDesignResolution(380, 240, CCSceneResolutionPolicy.ShowAll);
                //gameView.ResolutionPolicy = CCViewResolutionPolicy.ExactFit;
                CCScene gameScene = new CCScene(gameView);
                gameScene.AddLayer(new GameStartLayer());
                gameView.RunWithScene(gameScene);
            }
        }
    }
}

