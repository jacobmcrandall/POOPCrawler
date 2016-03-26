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

                int width = 1280;
                int height = 720;
                // Set world dimensions
                gameView.DesignResolution = new CCSizeI(width, height);

                //gameView.ContentManager.RootDirectory = "Assets/Content";
                gameView.ContentManager.SearchPaths = new List<string>()
                {"Images", "Fonts","Sounds"};
                
                CCScene gameScene = new CCScene(gameView);
                gameScene.AddLayer(new GameStartLayer());
                gameView.RunWithScene(gameScene);
            }
        }
    }
}

