using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Neat_Note
{
    public class CommonWindowHelper
    {
        #region Displaying Message and exception to the user
        public static void DisplayMessage(string message, Label displayingLabel)
        {
            displayingLabel.Foreground = (Brush)(new BrushConverter().ConvertFromString("#FF979797"));
            displayingLabel.ToolTip = "";
            displayingLabel.Content = message;
        }

        public static void DisplayException(string message, Label displayingLabel, Exception ex)
        {
            displayingLabel.Foreground = Brushes.Red;
            displayingLabel.ToolTip = ex.Message;
            displayingLabel.Content = message;
        }

        public static void DisplayUpdatedWordAndLineCount(TextRange range, Label displayingLabel)
        {
            MatchCollection wordCollection = Regex.Matches(range.Text, @"[\W]+");
            string[] lineCollection = range.Text.Split('\n');
            displayingLabel.Content = "Words: " + wordCollection.Count.ToString() + " Lines: " + (lineCollection.Length - 1);
        }
        #endregion

        public static void CloseWindow()
        {
            MessageBoxResult result = MessageBox.Show("Leaving so soon?", "Exit", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        public static void SetRestoredBackground(Border maxBorder)
        {
            if (Application.Current.Resources.MergedDictionaries[0].Source.ToString() == "Theme1.xaml")
            {
                maxBorder.Background = (Brush)(new BrushConverter().ConvertFrom("#00021218"));
            }
            else
            {
                maxBorder.Background = (Brush)(new BrushConverter().ConvertFrom("#00000000"));
            }
            
        }

        public static void SetMaximizedBackground(Border maxBorder)
        {
            if (Application.Current.Resources.MergedDictionaries[0].Source.ToString() == "Theme1.xaml")
            {
                maxBorder.Background = (Brush)(new BrushConverter().ConvertFrom("#FF021218"));
            }
            else
            {
                maxBorder.Background = (Brush)(new BrushConverter().ConvertFrom("#FF000000"));
            }
        }
    }
}
