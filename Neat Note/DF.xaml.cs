using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;


namespace Neat_Note
{

    public partial class DF : Window
    {
        MainWindow mainWindow = ((MainWindow)Application.Current.MainWindow);
        public RichTextBox rtb;
        bool isMaximized = false;
        //creating diferent timers
        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        DispatcherTimer saveTimer = new DispatcherTimer();

        //fields to store current window position
        private double RestoredHeight;
        private double RestoredWidth;
        private double RestoredLeft;
        private double RestoredTop;

        public DF(RichTextBox rtb)
        {
            InitializeComponent();
            this.rtb = rtb;
            Grid.SetColumn(rtb, 1);
            Grid.SetRow(rtb, 2);
            dfGrid.Children.Add(rtb);

            //creating different timers
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 2);

            saveTimer.Tick += SaveTimer_Tick;
            saveTimer.Interval = new TimeSpan(0, 0, 3);
            saveTimer.Start();

        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            btnMaximize.Visibility = Visibility.Hidden;
            btnMinimize.Visibility = Visibility.Hidden;
           
            dispatcherTimer.Stop();
        }

        private void SaveTimer_Tick(object sender, EventArgs e)
        {
            //Don't save anything if this window is not active
            if (!this.IsActive)
            {
                return;
            }

            CommonWindowHelper.DisplayMessage("", lblSave);

            //saving contents in active rich text box
            if (rtb != null)
            {
                int difference = rtb.Document.ContentStart.GetOffsetToPosition(rtb.Document.ContentEnd);
                //adding a character make difference 5 or more
                if (difference >= 5)
                {
                    CommonWindowHelper.DisplayMessage("Saved", lblSave);
                    TextRange range = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);

                    try
                    {
                        //saving as different file types
                        if (((string)rtb.Tag).EndsWith(".rtf", StringComparison.InvariantCultureIgnoreCase))
                        {
                            rtb.Foreground = Brushes.Black;
                            using (FileStream file = new FileStream((string)rtb.Tag, FileMode.OpenOrCreate, FileAccess.Write))
                            {
                                range.Save(file, DataFormats.Rtf);
                            }
                            rtb.Foreground = Brushes.White;
                        }

                        else
                        {
                            File.WriteAllText((string)rtb.Tag, range.Text);
                        }

                    }
                    catch (Exception ex)
                    {
                        rtb.Foreground = Brushes.White;
                        CommonWindowHelper.DisplayException("Error saving.\r\nHover over me to learn more.", lblSave, ex);
                    }

                    //updating count of words and lines 
                    CommonWindowHelper.DisplayUpdatedWordAndLineCount(range, lblCount);
                }
            }
        }




        private void resizeHandle_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.Height = e.GetPosition(this).Y;
                this.Width = e.GetPosition(this).X + 20;
                isMaximized = false;
                CommonWindowHelper.SetRestoredBackground(maxBorder);
            }
            e.Handled = true;
        }

        private void resizeHandle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ((UIElement)e.Source).ReleaseMouseCapture();
        }

        private void resizeHandle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ((UIElement)e.Source).CaptureMouse();
        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            //showing window bar buttons
            btnMaximize.Visibility = Visibility.Visible;
            btnMinimize.Visibility = Visibility.Visible;
            dispatcherTimer.Stop();
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            dispatcherTimer.Start();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;

        }

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (isMaximized)
            {
                Height = RestoredHeight;
                Width = RestoredWidth;
                Left = RestoredLeft;
                Top = RestoredTop;

                isMaximized = false;
                CommonWindowHelper.SetRestoredBackground(maxBorder);

            }
            else
            {

                WindowStartupLocation = WindowStartupLocation.Manual;

                RestoredHeight = Height;
                RestoredWidth = Width;
                RestoredLeft = Left;
                RestoredTop = Top;
                Height = SystemParameters.WorkArea.Height;
                Width = SystemParameters.WorkArea.Width;
                Left = (SystemParameters.WorkArea.Location.X);
                Top = (SystemParameters.WorkArea.Location.Y);
                isMaximized = true;

                CommonWindowHelper.SetMaximizedBackground(maxBorder);

            }
        }

        private void btnReattach_Click(object sender, RoutedEventArgs e)
        {
            SaveTimer_Tick(sender, e);
            dfGrid.Children.Remove(rtb);
            mainWindow.AttachRichTextBox(rtb);
            Hide();
            mainWindow.DisconnectDF();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!resizeHandle.IsMouseOver)
            {
                DragMove();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
