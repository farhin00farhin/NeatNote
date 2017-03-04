using System;
using System.Windows;
using System.IO;

namespace Neat_Note
{
    public partial class NewDocumentWindow : Window
    {
        public string filePath;

        public NewDocumentWindow()
        {
            InitializeComponent();

            //setting default file location creating if not already created
            txtDocName.Focus();
            txtDocName.SelectAll();
            txtFileLocation.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Neat Note");
            DirectoryInfo info = new DirectoryInfo(txtFileLocation.Text);
            if (!info.Exists)
            {
                try
                {
                    info.Create();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("We expect that you have a My Documents folder. Can not create default folder.\r\n" + ex.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.DialogResult = false;
                    return;
                }
            }
        }

        private void btnCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            //checking if file name is empty
            if (txtDocName.Text.Trim()=="")
            {
                MessageBox.Show("Please enter a name first.", "Warning", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            //checking if file name matches
            filePath = Path.Combine(txtFileLocation.Text, txtDocName.Text);

            if (!(filePath.EndsWith(cbType.SelectedValue.ToString())))
            {
                filePath += (string)cbType.SelectedValue;
            }
            try
            {
                if (File.Exists(filePath))
                {
                    MessageBoxResult result = MessageBox.Show("This file already exists. Do you want to replace it?", "Warning:File Already Exists", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        CreateFile();
                    }
                    else
                    {
                        txtDocName.Focus();
                        txtDocName.SelectAll();
                        return;
                    }
                }

                //if file name is unique, creating file without validation
                else
                {
                    CreateFile();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Can not create this file.\r\n" + ex.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
                this.DialogResult = false;
                return;

            }
          
            this.DialogResult = true;
            this.Close();
        }
        

        //method for creating files
        private void CreateFile()
        {
            using (FileStream file = File.Create(filePath)) { }
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog browserDialog = new System.Windows.Forms.FolderBrowserDialog();
          

            if (browserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtFileLocation.Text = browserDialog.SelectedPath;
            }
        }
    }
}
