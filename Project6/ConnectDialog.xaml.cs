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
    /// Interaction logic for ConnectDialog.xaml
    /// </summary>
    public partial class ConnectDialog : Window
    {
        public ConnectDialog()
        {
            InitializeComponent();
        }

        public String[] ResponseText = new String[2];

        private void Role_Checked(object sender, RoutedEventArgs e)
        {
            var rb = sender as RadioButton;
            if (rb.Name == "ClientRB") IPAddressTB.IsEnabled = true;
            else IPAddressTB.IsEnabled = false;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)ServerRB.IsChecked) ResponseText[0] = "Server";
            else ResponseText[0] = "Client";
            ResponseText[1] = IPAddressTB.Text;
            DialogResult = true;
        }
    }
}
