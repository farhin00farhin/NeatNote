using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace Neat_Note
{

    public partial class FontControl : UserControl
    {
        public double CurrentFontSize { get; set; }


        public FontControl()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception)
            {
            }
        }

        private void txtFontSize_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            //disabling pasting in font size textbox
            if (e.Command == ApplicationCommands.Copy ||
                 e.Command == ApplicationCommands.Cut ||
                 e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }

        #region Size
        private void txtFontSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.Visibility == Visibility.Hidden)
            {
                return;
            }
        }

        public void UpdateFontSizeTxt(double size)
        {
            txtFontSize.Text = size.ToString();
        }

        private void btnFontSizeUp_Click(object sender, RoutedEventArgs e)
        {
            double newFontSize = Convert.ToDouble(txtFontSize.Text);
            newFontSize++;
            txtFontSize.Text = newFontSize.ToString();
            UpdateFontSize();
        }

        private void btnFontSizeDown_Click(object sender, RoutedEventArgs e)
        {
            double newFontSize = Convert.ToDouble(txtFontSize.Text);
            newFontSize--;
            txtFontSize.Text = newFontSize.ToString();
            UpdateFontSize();

        }



        private void txtFontSize_KeyDown(object sender, KeyEventArgs e)
        {
            if (Key.Enter == e.Key)
            {
                UpdateFontSize();

                this.Visibility = Visibility.Hidden;
                ((MainWindow)Application.Current.MainWindow).SetFocusToCurrentRtb();

            }
        }

        private void UpdateFontSize()
        {
            try
            {

                CurrentFontSize = Convert.ToDouble(txtFontSize.Text);
                if (CurrentFontSize <= 6)
                {
                    txtFontSize.Text = "6";
                    CurrentFontSize = 6;
                }
                if (CurrentFontSize >= 72)
                {
                    txtFontSize.Text = "72";
                    CurrentFontSize = 72;
                }
          ((MainWindow)Application.Current.MainWindow).ChangeSize(CurrentFontSize);

            }
            catch (Exception)
            {
                txtFontSize.Text = CurrentFontSize.ToString();
            }
        }
        #endregion

        #region Font Family
        private void fontComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                FontFamily family = ((ComboBoxItem)fontComboBox.SelectedValue).FontFamily;
                ((MainWindow)Application.Current.MainWindow).ChangeFont(family);
            }
            catch (Exception)
            {
                // Can't set the font. Fail quietly as this is not crucial.
            }
        }

        public void UpdateFontFamilyCb(FontFamily ff)
        {
            foreach (var item in fontComboBox.Items)
            {
                if ((((ComboBoxItem)item).FontFamily).ToString() == ff.ToString())
                {
                    ((ComboBoxItem)item).IsSelected = true;
                }

            }
        }

        #endregion

        #region Colour
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            Color fontColor = ((SolidColorBrush)(((Button)sender).Background)).Color;
            ((MainWindow)Application.Current.MainWindow).ChangeColor(fontColor);
        }
        #endregion

        #region Bold
        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            bool isBold = btnBold.IsChecked.Value;
            ((MainWindow)Application.Current.MainWindow).ChangeWeight(isBold);
        }


        public void UpdateFontWeight(bool isBold)
        {
            btnBold.IsChecked = isBold;
        }

        #endregion



        private void btnDefaultStyle_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).ClearFormatting();

            txtFontSize.Text = "12"; // setting to 12
            UpdateFontSize();

            btnBold.IsChecked = false; //Setting to not bold
            ToggleButton_Click(sender, e);
            
            fontComboBox.SelectedIndex = 0; //setting to Arial
            ((MainWindow)Application.Current.MainWindow).ChangeColor(Colors.White);
        }
    }
}
