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
using System.Windows.Shapes;

namespace Project6
{
    /// <summary>
    /// Interaction logic for PlayAgainDialog.xaml
    /// </summary>
    public partial class PlayAgainDialog : Window
    {
        public String ResponseText;
        public PlayAgainDialog()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {            
            if ((bool)Yes.IsChecked) ResponseText = "yes";
            else ResponseText = "no";
            DialogResult = true;
        }
    }
}
