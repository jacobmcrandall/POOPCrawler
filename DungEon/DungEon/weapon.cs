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
    class weapon : CCNode
    {
        public int attack { get; set; }
        CCSprite sprite { get; }
        public weapon(string spriteString, int setAttack)
        {
            sprite = new CCSprite(spriteString);
            this.AddChild(sprite);
            attack = setAttack;
        }
    }
}