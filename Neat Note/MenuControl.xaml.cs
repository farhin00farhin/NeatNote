using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;

namespace Neat_Note
{

    public partial class MenuControl : UserControl
    {
        public List<string> fileQueue = new List<string>();

        //creating main window object
        MainWindow mainWindow = ((MainWindow)Application.Current.MainWindow);

        public MenuControl()
        {
            InitializeComponent();
        }

        private void btnCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        //methods for using shortcuts
        public void CommandBinding_Executed_New(object sender, ExecutedRoutedEventArgs e)
        {
            //creating an object of new document window
            NewDocumentWindow newDoc = new NewDocumentWindow();
            newDoc.Owner = Application.Current.MainWindow;


            if (newDoc.ShowDialog() == true)
            {
                this.Visibility = Visibility.Hidden;
               mainWindow.CreateNewTab(newDoc.filePath);
                mainWindow.SetFocusToCurrentRtb();


            }
        }

        public void CommandBinding_Executed_Open(object sender, ExecutedRoutedEventArgs e)
        {
           
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            openDialog.Filter = ("Supported file types|*.rtf;*.txt|Text Files (*.txt)|*.txt|Rich Text Files (*.rtf)|*.rtf");

            if (openDialog.ShowDialog() == true)
            {
                this.Visibility = Visibility.Hidden;
                mainWindow.CreateNewTab(openDialog.FileName);
                mainWindow.SetFocusToCurrentRtb();

            }

        }

        public void CommandBinding_Executed_SaveAs(object sender, ExecutedRoutedEventArgs e)
        {
            //giving out error message if there is no tabitem but user is still trying to save
            if (!mainWindow.hastabItem)
            {
                MessageBox.Show("You need to have atleast 1 file open to save as.", "Warning", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            saveDialog.Filter = ("Supported file types|*.rtf;*.txt|Text Files (*.txt)|*.txt|Rich Text Files (*.rtf)|*.rtf");

            if (saveDialog.ShowDialog() == true)
            {
                this.Visibility = Visibility.Hidden;
                mainWindow.SaveAs(saveDialog.FileName);
            }
        }

        public void CommandBinding_Executed_Print(object sender, ExecutedRoutedEventArgs e)
        {
            mainWindow.CommandBinding_Executed_Print(sender, e);
        }


        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Properties.Settings.Default.RecentFileList == null)
            {
                return;
            }

            //adding to list of recents from Properties.Settings.Default
            fileQueue = Properties.Settings.Default.RecentFileList.Cast<string>().ToList();
            ((ListBoxItem)recentListBox.Items[0]).Content = fileQueue.ElementAtOrDefault(fileQueue.Count-1);
            ((ListBoxItem)recentListBox.Items[1]).Content = fileQueue.ElementAtOrDefault(fileQueue.Count - 2);
            ((ListBoxItem)recentListBox.Items[2]).Content = fileQueue.ElementAtOrDefault(fileQueue.Count - 3);
            recentListBox.SelectedItem = null;
        }

        private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
          
                mainWindow.CreateNewTab(((ListBoxItem)sender).Content.ToString());
                this.Visibility = Visibility.Hidden;
            
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (recentListBox.SelectedItem == null)
                {
                    MessageBox.Show("Please select a file from the recent files list to delete", "Warning", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                foreach (var item in fileQueue)
                {
                    if (item == ((ListBoxItem)recentListBox.SelectedItem).Content.ToString())
                    {
                        MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this file?", "Warning: Deleting file", MessageBoxButton.YesNo);
                        if (result == MessageBoxResult.Yes)
                        {
                            //closing tab and deleting file
                            mainWindow.btnCloseTab_Click(sender, e);
                            File.Delete(item);
                            fileQueue.Remove(item);
                            try
                            {
                                Properties.Settings.Default.RecentFileList = new System.Collections.Specialized.StringCollection();
                                Properties.Settings.Default.RecentFileList.AddRange(fileQueue.ToArray());
                                Properties.Settings.Default.Save();
                            }
                            catch (Exception)
                            {
                                // fail quietly as this is not crucial functionality
                            }
                            UserControl_IsVisibleChanged(sender, new DependencyPropertyChangedEventArgs());
                            this.Visibility = Visibility.Hidden;
                            return;
                        }

                        else
                        {
                            return;
                        }

                    }

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("This file could not be deleted\r\n" + ex.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
            }
          
        }

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                btnDelete_Click(sender, e);
            }
           
        }
    }
}
