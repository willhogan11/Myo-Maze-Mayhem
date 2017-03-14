using System;
using System.Linq;
using Windows.Storage;
using System.Diagnostics;
using Windows.UI.Xaml.Controls;

namespace MyoUWP
{
    /* Class thats deals with Reading and Writing files to and from Local Storage
     * This Files contain the scores of users who have completed the game */
    public class ScoresStorage
    {

        /* Function that writes scores to file.
         * Details:
         * 1. Create a Folder and file to store the scores in
         * 2. If it already exists, open it
         * 3. If it's the first time write score to file
         * 4. If not, then append new score to file to preserve old scores 
         * 5. The parameter is for the finished string text that contains the score to be added to the file  */
        public async void WriteScoreToFile(string finishedState)
        {
            var folder = ApplicationData.Current.LocalFolder;
            var scoresFolder = await folder.CreateFolderAsync("ScoresFolder", CreationCollisionOption.OpenIfExists);

            dbg(scoresFolder.Path);

            try
            {
                var textFile = await scoresFolder.CreateFileAsync("scores.txt");
                await FileIO.WriteTextAsync(textFile, finishedState + System.Environment.NewLine);
            }
            catch (Exception)
            {
                folder = ApplicationData.Current.LocalFolder;
                scoresFolder = await folder.CreateFolderAsync("ScoresFolder", CreationCollisionOption.OpenIfExists);

                var files = await scoresFolder.GetFilesAsync();
                var desiredFile = files.FirstOrDefault(x => x.Name == "scores.txt");
                await FileIO.AppendTextAsync(desiredFile, finishedState + System.Environment.NewLine);
            }
        }


        /* Function that reads values from Local Storage and outputs to TextBlock in Scores.xaml
         * Steps:
         * 1. If it already exists, open it.
         * 2. Store all the scores in a string object with a new line appended to each
         * 3. Outout the details of the object in the TextBlock in Scores.xaml
         * 4. If no scores exist, output a meaningful message */
        public async void ReadScoresFromFile(TextBlock scoresText)
        {
            try
            {
                var folder = ApplicationData.Current.LocalFolder;
                var scoresFolder = await folder.CreateFolderAsync("ScoresFolder", CreationCollisionOption.OpenIfExists);

                var files = await scoresFolder.GetFilesAsync();
                var desiredFile = files.FirstOrDefault(x => x.Name == "scores.txt");
                var textContent = await FileIO.ReadTextAsync(desiredFile);

                dbg(textContent);

                scoresText.Text = textContent;
            }
            catch (Exception)
            {
                scoresText.Text = "NO SCORES SAVED YET\nWIN A GAME FIRST....";
            }
        }


        // For debug purposes
        private void dbg(string str, Object strOpt = null)
        {
            Debug.WriteLine(str);
        }
    }
}
