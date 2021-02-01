using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace OutToTheEdgePuzzleFinal
{
    public class Tile
    {
        //Class Properties
        private int x;
        private int y;
        private Rectangle tileImage;

        //Class Getter/Setters for class properties
        public int X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
            }
        }
        public int Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
            }
        }
        public Rectangle TileImage
        {
            get
            {
                return tileImage;
            }
            set
            {
                tileImage = value;
            }
        }

        //Class Constructer for implementation
        public Tile(int x, int y, Rectangle tileImage)
        {
            X = x;
            Y = y;
            TileImage = tileImage;
        }

    }
}
