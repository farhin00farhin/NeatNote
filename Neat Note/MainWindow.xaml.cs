using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Neat_Note
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DF myDf;
        bool isMaximized = false;
        //creating different timers
        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        DispatcherTimer saveTimer = new DispatcherTimer();

        //fields to store current window position
        private double RestoredHeight;
        private double RestoredWidth;
        private double RestoredLeft;
        private double RestoredTop;

        //property to show whether there is a tabitem or no
        public bool hastabItem
        {
            get
            {
                //looking for more than 1 item because there is a hidden tabitem that does not do anything
                if (tabControl.Items.Count > 1)
                {
                    return true;
                }

                return false;
            }
        }



        public MainWindow()
        {
            InitializeComponent();
            this.Focus();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 2);

            saveTimer.Tick += SaveTimer_Tick;
            saveTimer.Interval = new TimeSpan(0, 0, 3);
            saveTimer.Start();

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
            RichTextBox rtb = ((RichTextBox)tabControl.SelectedContent);
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


        #region Resize, Maximize, Minimize, Close Buttons
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            btnMaximize.Visibility = Visibility.Hidden;
            btnMinimize.Visibility = Visibility.Hidden;
            btnClose.Visibility = Visibility.Hidden;
            dispatcherTimer.Stop();
        }

        private void resizeHandle_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.Height = e.GetPosition(this).Y;
                this.Width = e.GetPosition(this).X + 20;
                isMaximized = false;
                CommonWindowHelper.SetRestoredBackground(maxBorder);
                btnMaximize.Style = (Style)App.Current.Resources["MaximizeButtonStyle"];
            }
            e.Handled = true;
        }

        

        private void resizeHandle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ((UIElement)e.Source).CaptureMouse();

        }

        private void resizeHandle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ((UIElement)e.Source).ReleaseMouseCapture();

        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            CommonWindowHelper.CloseWindow();
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
                btnMaximize.Style = (Style)App.Current.Resources["MaximizeButtonStyle"];
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
                btnMaximize.Style = (Style)App.Current.Resources["RestoreButtonStyle"];

            }

        } 
        #endregion

        private void btnMenu_Click(object sender, RoutedEventArgs e)
        {
            if (menuControl.Visibility == Visibility.Hidden)
            {
                menuControl.Visibility = Visibility.Visible;
            }

            else
            {
                menuControl.Visibility = Visibility.Hidden;
            }
        }

        private void btnHideEditPanel_Click(object sender, RoutedEventArgs e)
        {
            if (editSection.Width.Value != 0)
            {
                editSection.Width = new GridLength(0, GridUnitType.Pixel);
            }
            else
            {
                editSection.Width = new GridLength(90, GridUnitType.Pixel);
            }

        }

        private void btnHideSearchPanel_Click(object sender, RoutedEventArgs e)
        {
            if (searchSection.Height.Value != 0)
            {
                searchSection.Height = new GridLength(0, GridUnitType.Pixel);
            }
            else
            {
                searchSection.Height = new GridLength(90, GridUnitType.Pixel);
                HideAllOverlays();
            }
        }

        private void btnFormat_Click(object sender, RoutedEventArgs e)
        {
            if (formatMenu.Height.Value != 0)
            {
                formatMenu.Height = new GridLength(0, GridUnitType.Pixel);
                spFormat.Height = 0;
                txtBlock1.RenderTransform = new RotateTransform(180);
                txtBlock1.Margin = new Thickness(0, 0, -20, 0);
            }
            else
            {
                formatMenu.Height = new GridLength(1, GridUnitType.Star);
                spFormat.Height = double.NaN;
                txtBlock1.RenderTransform = new RotateTransform(0);
                txtBlock1.Margin = new Thickness(0, -10, -20, 0);
            }
        }

        public void DisconnectDF()
        {
            myDf = null;
        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            btnMaximize.Visibility = Visibility.Visible;
            btnMinimize.Visibility = Visibility.Visible;
            btnClose.Visibility = Visibility.Visible;
            dispatcherTimer.Stop();
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            dispatcherTimer.Start();
        }

        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {
            HideAllOverlays();
            keyboardControl.Visibility = Visibility.Visible;
            searchSection.Height = new GridLength(0, GridUnitType.Pixel);

        }

        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            HideAllOverlays();
            aboutControl.Visibility = Visibility.Visible;
            searchSection.Height = new GridLength(0, GridUnitType.Pixel);
        }

        private void btnFont_Click(object sender, RoutedEventArgs e)
        {
            if (!SomethingIsOpen())
            {
                MessageBox.Show("You need to have at least 1 file open to edit styles.", "Warning", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (fontControl.Visibility == Visibility.Visible)
            {
                fontControl.Visibility = Visibility.Hidden;
            }
            else
            {
                fontControl.Visibility = Visibility.Visible;
            }
        }

        private bool SomethingIsOpen()
        {
            if (tabControl.Items.Count > 1 || myDf != null)
            {
                return true;
            }

            return false;
        }

        private void HideAllOverlays()
        {
            aboutControl.Visibility = Visibility.Hidden;
            keyboardControl.Visibility = Visibility.Hidden;
            fontControl.Visibility = Visibility.Hidden;
            menuControl.Visibility = Visibility.Hidden;
            findControl.Visibility = Visibility.Hidden;
        }

        private void Window_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!menuControl.IsMouseOver && !btnMenu.IsMouseOver)
            {
                menuControl.Visibility = Visibility.Hidden;
            }

            if (!fontControl.IsMouseOver && !btnFont.IsMouseOver)
            {
                fontControl.Visibility = Visibility.Hidden;

            }

            if (!keyboardControl.IsMouseOver && !btnHelp.IsMouseOver)
            {
                keyboardControl.Visibility = Visibility.Hidden;

            }

            if (!aboutControl.IsMouseOver && !btnAbout.IsMouseOver)
            {
                aboutControl.Visibility = Visibility.Hidden;

            }
        }

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            if (!SomethingIsOpen())
            {
                MessageBox.Show("You need to have at least 1 file open to find/replace text.", "Warning", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            searchSection.Height = new GridLength(0, GridUnitType.Pixel);

            if (findControl.Visibility == Visibility.Visible)
            {
                findControl.Visibility = Visibility.Hidden;
            }
            else
            {
                findControl.Visibility = Visibility.Visible;
            }
        }
      
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!resizeHandle.IsMouseOver)
            {
                this.DragMove();
            }

        }

        

        public void colourUpdater_DoWork(object sender, DoWorkEventArgs e) {
            RichTextBox rtb = ((RichTextBox)e.Argument);
            if (rtb != null) {
                ChangeBlackTextToWhiteText(rtb);
            }
        }


        // We need to set the colour of the text to white if it is black since Neat Note is a dark themed app.
        public void ChangeBlackTextToWhiteText(RichTextBox textbox) {
            try {
                    TextPointer startPointer = textbox.Document.ContentStart;
                    for (long i = 0; i < long.MaxValue; i++) {

                        //Updating the textbox in the background. Need to use Dispatcher.Invoke otherwise get an access error
                        Dispatcher.Invoke(() => {
                            
                            //checking if the next character is null. Escaping if it is.
                            if (startPointer.GetNextContextPosition(LogicalDirection.Forward) == null) {
                                i = long.MaxValue-1; //setting to max value so dispatcher.invoke doesn't run in the background.
                                return;
                            }


                            TextRange nextCharacter = new TextRange(startPointer, startPointer.GetNextContextPosition(LogicalDirection.Forward));
                            string colorAtRange = nextCharacter.GetPropertyValue(TextElement.ForegroundProperty).ToString();
                            if (colorAtRange == Brushes.Black.ToString()) {
                                nextCharacter.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.White);
                            }
                            startPointer = startPointer.GetNextContextPosition(LogicalDirection.Forward);


                            //Double checking at the end for next character. Escape if it is.
                            if (startPointer.GetNextContextPosition(LogicalDirection.Forward) == null) {
                                i = long.MaxValue-1;
                                return;
                            }
                        });
                    }
            }
            catch (Exception ex) {
                //Ignore this exception as it runs in the background and isn't important functionality.
            }

        }

        public void CreateNewTab(string path)
        {
            //Checking if file is already open in Neat Note
            foreach (var tab in tabControl.Items)
            {
                if (((TabItem)tab).Tag != null)
                {
                    if ((string)(((TabItem)tab).Tag) == path)
                    {
                        MessageBox.Show("This file is already open in Neat Note","Warning",MessageBoxButton.OK,MessageBoxImage.Error);
                        return;
                    }
                }
            }

            if (myDf != null && myDf.rtb.Tag.ToString() == path)
            {
                MessageBox.Show("This file is already open in Neat Note");
                return;
            }


            //generating empty textbox
            RichTextBox textbox = new RichTextBox();
            textbox.Tag = path;
            //adding to tab
            textbox.LostFocus += Textbox_LostFocus;
            textbox.SelectionChanged += Textbox_SelectionChanged;
            textbox.Style = (Style)Application.Current.Resources["DocumentRichTextBoxStyle"];
            textbox.FontFamily = new FontFamily("Arial");
            textbox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            textbox.SpellCheck.IsEnabled = btnToggleSpellCheck.IsChecked.Value;
            textbox.Focusable = true;
            textbox.IsEnabled = true;

            //Loading text in textbox
            TextRange range = new TextRange(textbox.Document.ContentStart, textbox.Document.ContentEnd);

            try
            {
                using (FileStream file = new FileStream(path, FileMode.Open))
                {
                    if (file.Length > 0)
                    {
                        if (path.EndsWith(".txt", StringComparison.InvariantCultureIgnoreCase))
                        {
                            range.Load(file, DataFormats.Text);
                        }

                        else if (path.EndsWith(".rtf", StringComparison.InvariantCultureIgnoreCase))
                        {
                            range.Load(file, DataFormats.Rtf);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not open file.\r\n" + ex.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            AttachRichTextBox(textbox);
        }

        public async void AttachRichTextBox(RichTextBox textbox) {
            //creating tabitem
            TabItem tabItem = new TabItem();
            tabControl.Items.Add(tabItem);
            tabItem.IsSelected = true;

            //setting tab name
            FileInfo info = new FileInfo(textbox.Tag.ToString());
            tabItem.Header = info.Name;
            tabItem.Tag = textbox.Tag.ToString();

            //adding the rich textbox to the tabitem
            tabItem.Content = textbox;

            //preparing the UI
            lblCount.Content = "Words: 0  Lines: 0";

            if (textbox.Tag.ToString().EndsWith("rtf", StringComparison.InvariantCultureIgnoreCase)) {
                try {
                    BackgroundWorker colourUpdater = new BackgroundWorker();
                    colourUpdater.DoWork += colourUpdater_DoWork;
                    colourUpdater.RunWorkerAsync(textbox);
                }
                catch (Exception ex) {
                    MessageBox.Show("This file is taking too long to convert to neat note\'s style. Displaying in original style");
                }
            }
            SetRecent(textbox.Tag.ToString());

            //Have to set the focus to the new textBox with a delay since it's not getting set at this point 
            DispatcherTimer setFocusTimer = new DispatcherTimer();
            setFocusTimer.Interval = new TimeSpan(0, 0, 1);
            setFocusTimer.Tick += SetFocusTimer_Tick;
            setFocusTimer.Start();

            textbox.Focus();
        }

        
        private void SetFocusTimer_Tick(object sender, EventArgs e) {
            ((DispatcherTimer)sender).Stop();
            SetFocusToCurrentRtb();
        }

        private void SetRecent(string path)
        {
            if (menuControl.fileQueue.Contains(path))
            {
                return;
            }

            if (menuControl.fileQueue.Count >= 3)
            {
                menuControl.fileQueue.RemoveAt(0);
            }
            menuControl.fileQueue.Add(path);
            

            try
            {
                Properties.Settings.Default.RecentFileList = new System.Collections.Specialized.StringCollection();
                Properties.Settings.Default.RecentFileList.AddRange(menuControl.fileQueue.ToArray());
                Properties.Settings.Default.Save();
            }
            catch (Exception)
            {
                // fail quietly as this is not crucial functionality
            }

        }

        public void SaveAs(string path)
        {
            RichTextBox rtb = ((RichTextBox)tabControl.SelectedContent);
            TabItem tabitem = (TabItem)tabControl.SelectedItem;
            if (rtb != null)
            {
                try {
                    //creating file
                    using (FileStream file = File.Create(path)) { }
                }
                catch (Exception ex) {
                    MessageBox.Show("This file could not be saved\r\n" + ex.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                rtb.Tag = path;

                //setting tab name
                FileInfo info = new FileInfo(path);
                tabitem.Header = info.Name;
                tabitem.Tag = path;
                SetRecent(path);

            }

           
        }
        private void Textbox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            RichTextBox rtb = ((RichTextBox)tabControl.SelectedContent);
            if (rtb != null)
            {
                TextRange selectedRange = rtb.Selection;
                try
                {
                    fontControl.UpdateFontFamilyCb((FontFamily)(selectedRange.GetPropertyValue(TextElement.FontFamilyProperty)));
                    fontControl.UpdateFontSizeTxt((double)(selectedRange.GetPropertyValue(TextElement.FontSizeProperty)));
                    if ((FontWeight)selectedRange.GetPropertyValue(TextElement.FontWeightProperty) == FontWeights.Bold)
                    {
                        fontControl.UpdateFontWeight(true);
                    }
                    else
                    {
                        fontControl.UpdateFontWeight(false);
                    }
                }
                catch (Exception)
                {
                    //Fail quietly since this is not crucial functionality.
                }
            }
        }

        private void Textbox_LostFocus(object sender, RoutedEventArgs e)
        {
            // When the RichTextBox loses focus the user can no longer see the selection.
            // This is a hack to make the RichTextBox think it did not lose focus.
            e.Handled = true;
        }

        

        public void btnCloseTab_Click(object sender, RoutedEventArgs e)
        {
            SaveTimer_Tick(null, EventArgs.Empty);
            if (tabControl.SelectedIndex > 0) {
                tabControl.Items.RemoveAt(tabControl.SelectedIndex);
                lblCount.Content = "" ;
            }
        }

        private void CommandBinding_Executed_New(object sender, ExecutedRoutedEventArgs e)
        {
            menuControl.CommandBinding_Executed_New(sender, e);
        }

        private void CommandBinding_Executed_Open(object sender, ExecutedRoutedEventArgs e)
        {
            menuControl.CommandBinding_Executed_Open(sender, e);
        }

        private void CommandBinding_Executed_SaveAs(object sender, ExecutedRoutedEventArgs e)
        {
            menuControl.CommandBinding_Executed_SaveAs(sender, e);
        }

        public void CommandBinding_Executed_Print(object sender, ExecutedRoutedEventArgs e)
        {
            RichTextBox rtb = ((RichTextBox)tabControl.SelectedContent);
            if (rtb != null)
            {
                try {
                    PrintDialog printDialog = new PrintDialog();

                    if (printDialog.ShowDialog() == true) {
                        rtb.Foreground = Brushes.Black;
                        printDialog.PrintVisual(rtb as Visual, rtb.Tag.ToString());
                        rtb.Foreground = Brushes.White;
                    }
                }
                catch (Exception ex) {
                    
                MessageBox.Show("Unable to print this document. \r\n", "Warning", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                }
            }
            else
            {
                MessageBox.Show("You need to have at least 1 document open to print.", "Warning", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public void ChangeFont(FontFamily newFont)
        {
            RichTextBox rtb = ((RichTextBox)tabControl.SelectedContent);
            if (rtb != null)
            {
                TextRange selectedRange = rtb.Selection;
                rtb.Focus();
                fontControl.Visibility = Visibility.Hidden;
                selectedRange.ApplyPropertyValue(TextElement.FontFamilyProperty, newFont);
            }
        }

        public void ChangeColor(Color fontColor)
        {
            RichTextBox rtb = ((RichTextBox)tabControl.SelectedContent);
            if (rtb != null)
            {
                TextRange selectedRange = rtb.Selection;
                rtb.Focus();
                fontControl.Visibility = Visibility.Hidden;
                selectedRange.ApplyPropertyValue(TextElement.ForegroundProperty, (new BrushConverter().ConvertFromString(fontColor.ToString())));
            }
        }

        public void ChangeSize(double newSize)
        {
            RichTextBox rtb = ((RichTextBox)tabControl.SelectedContent);
            if (rtb != null)
            {
                TextRange selectedRange = rtb.Selection;
                selectedRange.ApplyPropertyValue(TextElement.FontSizeProperty, newSize);
            }
        }

        public void ChangeWeight(bool isBold)
        {
            RichTextBox rtb = ((RichTextBox)tabControl.SelectedContent);
            if (rtb != null)
            {
                TextRange selectedRange = rtb.Selection;
                if (isBold)
                {

                    selectedRange.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
                }
                else
                {
                    selectedRange.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Normal);

                }

                rtb.Focus();
                fontControl.Visibility = Visibility.Hidden;

            }
        }

        public void ClearFormatting()
        {
            RichTextBox rtb = ((RichTextBox)tabControl.SelectedContent);
            if (rtb != null)
            {
                TextRange selectedRange = rtb.Selection;
                selectedRange.ClearAllProperties();
            }
        }

        public void SetFocusToCurrentRtb()
        {
            RichTextBox rtb = ((RichTextBox)tabControl.SelectedContent);
            if (rtb != null)
            {
                rtb.Focus();
            }
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {

            RichTextBox rtb = ((RichTextBox)tabControl.SelectedContent);
            
            if (rtb != null)
            {

                rtb.Copy();
              
            }

        }

        private void btnPaste_Click(object sender, RoutedEventArgs e)
        {
            RichTextBox rtb = ((RichTextBox)tabControl.SelectedContent);
            if (rtb != null)
            {
                rtb.Paste();


            }

        }

        private void btnCut_Click(object sender, RoutedEventArgs e)
        {
            RichTextBox rtb = ((RichTextBox)tabControl.SelectedContent);
            if (rtb != null)
            {
                rtb.Cut();
            }
        }


        private void CommandBinding_Executed_Spelling(object sender, ExecutedRoutedEventArgs e)
        {
            btnToggleSpellCheck.IsChecked = !btnToggleSpellCheck.IsChecked.Value;
        }

        private void btnToggleSpellCheck_Checked(object sender, RoutedEventArgs e)
        {
            if (myDf != null)
            {
                myDf.rtb.SpellCheck.IsEnabled = btnToggleSpellCheck.IsChecked.Value;
            }

            if (tabControl == null)
            {
                return;
            }

            foreach (var tab in tabControl.Items)
            {
                if (((TabItem)tab).Tag != null)
                {
                    RichTextBox rtb = ((RichTextBox)tabControl.SelectedContent);
                    if (rtb != null)
                    {
                        rtb.SpellCheck.IsEnabled = btnToggleSpellCheck.IsChecked.Value;
                    }
                }
            }
        }

        private void CommandBinding_Executed_DF(object sender, ExecutedRoutedEventArgs e)
        {
             RichTextBox rtb = ((RichTextBox)tabControl.SelectedContent);
            TabItem tabitem = (TabItem)tabControl.SelectedItem;
            if (rtb != null)
            {

                tabitem.Content = null;
                myDf = new DF(rtb);               
                myDf.Show();
                myDf.Focus();
                btnCloseTab_Click(sender, e);

            }

            else
            {
                MessageBox.Show("You need to have at least 1 file open to edit it in distraction free mode.", "Warning", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        internal void FindReplaceText(string textToFind, string textToReplaceWith)
        {
            TextRange rangeToReplace = FindText(textToFind);
            if (rangeToReplace != null)
            {
                rangeToReplace.Text = textToReplaceWith;
            }

        }

        internal TextRange FindText(string textToFind)
        {
            RichTextBox rtb = ((RichTextBox)tabControl.SelectedContent);
            TabItem tabitem = (TabItem)tabControl.SelectedItem;
            if (rtb != null)
            {
                TextRange searchRange = new TextRange(rtb.Selection.End.GetPositionAtOffset(0), rtb.Document.ContentEnd);

                // Do the search
                TextRange foundRange = FindWordFromPosition(rtb.Selection.End, textToFind);
                
                if (foundRange == null)
                {
                    // If not found, go to the start of the document and search again
                    foundRange = FindWordFromPosition(rtb.Document.ContentStart, textToFind);
                    if (foundRange == null)
                    {
                        MessageBox.Show("The text you\'re searching for was not found.", "Not Found", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                        return null;
                    }
                }

                // Select the found range
                rtb.Selection.Select(foundRange.Start, foundRange.End);
                rtb.Focus();
                return foundRange;
            }
            return null;
        }

        

        public TextRange FindWordFromPosition(TextPointer position, string word)
        {
            while (position != null)
            {
                if (position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    string textRun = position.GetTextInRun(LogicalDirection.Forward);

                    // Find the starting index of any substring that matches "word".
                    int indexInRun = textRun.IndexOf(word, StringComparison.InvariantCultureIgnoreCase);
                    if (indexInRun >= 0)
                    {
                        TextPointer start = position.GetPositionAtOffset(indexInRun);
                        TextPointer end = start.GetPositionAtOffset(word.Length);
                        return new TextRange(start, end);
                    }
                }

                position = position.GetNextContextPosition(LogicalDirection.Forward);
            }

            return null;
        }

        private void btnTheme_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.Resources.MergedDictionaries[0].Source.ToString() == "Theme1.xaml")
            {
                Application.Current.Resources.MergedDictionaries.Clear();
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("Theme2.xaml", UriKind.Relative) });
            }
            else
            {
                Application.Current.Resources.MergedDictionaries.Clear();
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("Theme1.xaml", UriKind.Relative) });
            }

            if (isMaximized)
            {
                CommonWindowHelper.SetMaximizedBackground(maxBorder);
            }
            else
            {
                CommonWindowHelper.SetRestoredBackground(maxBorder);
            }
        }


    }
}
