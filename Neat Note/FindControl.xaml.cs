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

namespace Neat_Note
{
    /// <summary>
    /// Interaction logic for FindControl.xaml
    /// </summary>
    public partial class FindControl : UserControl
    {
        public FindControl()
        {
            InitializeComponent();
        }

        private void btnCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).FindText(txtFind.Text);

        }

        private void btnReplace_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).FindReplaceText(txtFind.Text, txtReplace.Text);

        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.Visibility == Visibility.Visible)
            {
                txtFind.Focus();
            }
        }

    }
}
