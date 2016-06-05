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

namespace Project6
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CreateCells();
            Reset();
        }

        private void CreateCells()
        {
            for(int r = 0; r < 3; r++)
                for(int c = 0; c < 3; c++)
                {
                    var cell = new TTTCell(r, c);
                    Grid.SetRow(cell, r * 2);
                    Grid.SetColumn(cell, c * 2);
                    GameBoard.Children.Add(cell);
                    cell.MouseLeftButtonDown += CellClicked;
                }
        }

        void Reset()
        {

        }

        void CellClicked(object sender, InputEventArgs e)
        {

        }
    }
}
