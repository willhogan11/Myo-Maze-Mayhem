﻿using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MyoUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HowToPlay : Page
    {

        public HowToPlay()
        {
            this.InitializeComponent();
            Story();
            GameInstructions();
        }


        // Displays the System back button's Visibility to Visible
        // If there is a page to go back to, then navigate back through the stack
        // If not then the user is at the main first page. 
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame.CanGoBack)
            {
                // Show UI in title bar if opted-in and in-app backstack is not empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Visible;
                SystemNavigationManager.GetForCurrentView().BackRequested += HowToPlay_BackRequested;
            }
            else
            {
                // Remove the UI from the title bar if in-app back stack is empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Collapsed;
            }
        }


        // Story Text
        private void Story()
        {
            story.Text =
                "Its the year 2034 and humans are beginning to colonise Mars.\n\nUpon receiving news from NASA back on earth, that a storm of " +
                "gargantuan proportions is on a collision course with your Habitat, the future of human existence on Mars is uncertain.\n\nHowever, there is hope.....\n" +
                "Your job is to ferry your crew out of harms way and get to the escape pod located in a nearby location, before the Storms hits.\nYou have a " +
                "certain amount of time to get from the Hab to the Escape pod without time running out, or coming into contact with hazardous " +
                "debris or it literally is Game Over!\nThe World is counting on you........"; 
        }

        // Game Instructions text
        private void GameInstructions()
        {
            howToPlay.Text =
                "There are two ways to play the game.\nYou can play using the Arrow Keys on the keyboard of your laptop or PC\n " +
                "The below arrows move the Ship in that direction, If you keep any arrow key pressed, the Ship will continue to move in that direction." +
                "Before you start playing either Key Play or Myo Play mode, you are asked to Choose a difficulty. The options are easy, medium or hard.\n";

            myoPlay.Text =
                "To play using the myo, you control the ship by following the below instructions.";
        }


        // Microsoft Back Button
        private void HowToPlay_BackRequested(object sender, BackRequestedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)
                return;

            // Navigate back if possible, and if the event has not already been handled...
            if (rootFrame.CanGoBack && e.Handled == false)
            {
                e.Handled = true;
                rootFrame.GoBack();
            }
        }
    }
}
