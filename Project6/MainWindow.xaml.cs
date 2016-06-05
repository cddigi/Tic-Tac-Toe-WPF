using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
        PlayerID player = PlayerID.X;
        bool currentTurn = false;
        TcpListener listener;
        TcpClient socket;
        NetworkStream netStream;
        StreamReader reader;
        StreamWriter writer;

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
            foreach(var label in GameBoard.Children)
            {
                TTTCell cell;
                if (label is TTTCell)
                {
                    cell = label as TTTCell;
                    cell.Owner = PlayerID.None;
                }
            }
        }

        void CellClicked(object sender, InputEventArgs e)
        {
            if (!currentTurn) return;
            var cell = sender as TTTCell;
            if (cell.Owner == PlayerID.None)
                cell.Owner = player;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ConnectDialog();
            if (dialog.ShowDialog() == true)
            {
                if (dialog.ResponseText[0] == "Server") SetupServer();
                else SetupClient(dialog.ResponseText[1]);
            }
        }

        private void SetupServer()
        {
            currentTurn = !currentTurn;
            listener = new TcpListener(IPAddress.Any, 50001);
            listener.Start();
            socket = listener.AcceptTcpClient();            
            Task.Run(() => HandleRequest());
        }

        private void SetupClient(String ip)
        {
            socket = new TcpClient(ip, 50001);
            Task.Run(() => HandleRequest());
        }

        private void HandleRequest()
        {
            netStream = socket.GetStream();
            reader = new StreamReader(netStream);
            writer = new StreamWriter(netStream);
            writer.AutoFlush = true;

            while (true)
            {
                var msg = reader.ReadLine();
                var parts = msg.Split();

            }

            socket.Close();
        }
    }
}
