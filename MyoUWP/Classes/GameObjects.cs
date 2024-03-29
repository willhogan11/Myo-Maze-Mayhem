﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;


namespace MyoUWP.Classes
{
    public class GameObjects
    {
        Rectangle debris = new Rectangle();


        /* A Function that Creates a Ship object
         * If ship is null, create it using the various properties
         * Add it to the canvas
         * return the ship object */
        public Rectangle CreateShip(Rectangle ship, Rectangle eMyo, Canvas cvsRoller)
        {
            if (ship == null)
            {
                SolidColorBrush blackBrush = new SolidColorBrush(Windows.UI.Colors.Black);
                ship = new Rectangle();
                ship.RadiusX = (float)Canvas.GetLeft(eMyo);
                ship.RadiusY = (float)Canvas.GetTop(eMyo);
                ship.Width = (float)eMyo.Width;
                ship.Height = (float)eMyo.Height;
                ship.Fill = blackBrush;

                cvsRoller.Children.Add(ship);
            }
            return ship;
        }


        /* A function that deals with creating debris objects
         * Steps:
         * Create a Debris object using width, height and so on.
         * Randomly generate a location within the canvas for the object to appear
         * If this particular debris object is created on the escape pod or base, set to null and move on to next one
         * Add to canvas */
        public void CreateDebris(Rectangle debris, List<Rectangle> debrisArray, Canvas cvsRoller)
        {
            Random random = new Random();
            debris = new Rectangle();
            SolidColorBrush brownBrush = new SolidColorBrush(Windows.UI.Colors.Brown);

            debris.Width = 25;
            debris.Height = 25;

            CompositeTransform transform = new CompositeTransform();
            transform.TranslateX = random.Next(0, 975);
            transform.TranslateY = random.Next(0, 525);

            debris.RenderTransform = transform;     

            debris.RadiusX = transform.TranslateX;
            debris.RadiusY = transform.TranslateY;

            if (debris.RadiusX >= 900 && debris.RadiusY <= 65 || debris.RadiusX <= 65 && debris.RadiusY >= 460)
            {
                this.debris = null;
                dbg("Debris Removed from Escape Pod or Mars Base");
            }
            else
            {
                debris.Fill = brownBrush;
                debrisArray.Add(debris);
                cvsRoller.Children.Add(debris);
            }
        }


        // For debug purposes
        private void dbg(string str, Object strOpt = null)
        {
            Debug.WriteLine(str);
        }
    }
}
