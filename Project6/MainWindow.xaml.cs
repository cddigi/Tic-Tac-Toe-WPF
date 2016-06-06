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
        PlayerID player, opponent;
        bool currentTurn = false;        
        TcpListener listener;
        TcpClient socket;
        NetworkStream netStream;
        BinaryReader reader;
        BinaryWriter writer;
        TTTCell[,] board = new TTTCell[3, 3];

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
                    board[r, c] = cell;
                    Grid.SetRow(cell, r * 2);
                    Grid.SetColumn(cell, c * 2);
                    GameBoard.Children.Add(cell);
                    cell.MouseLeftButtonDown += CellClicked;
                }
        }

        void Reset()
        {
            for (int r = 0; r < 3; r++)
                for (int c = 0; c < 3; c++)
                    board[r, c].Owner = PlayerID.None;
        }

        void CellClicked(object sender, InputEventArgs e)
        {
            if (!currentTurn) return;
            currentTurn = !currentTurn;
            var cell = sender as TTTCell;
            if (cell.Owner == PlayerID.None)
                cell.Owner = player;
            writer.Write("move");
            writer.Write(cell.Row);
            writer.Write(cell.Column);
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
            player = PlayerID.X;
            opponent = PlayerID.O;
            currentTurn = !currentTurn;
            listener = new TcpListener(IPAddress.Any, 50001);
            listener.Start();
            socket = listener.AcceptTcpClient();
            ChatLog.Text += "Connected to client.\n";
            Task.Run(() => HandleRequest());
        }

        private void SetupClient(String ip)
        {
            try
            {
                player = PlayerID.O;
                opponent = PlayerID.X;
                socket = new TcpClient(ip, 50001);
                ChatLog.Text += "Connected to server.\n";
                Task.Run(() => HandleRequest());
            }
            catch (SocketException e)
            {
                ChatLog.Text += "Server Connection Failed, Host Not Found\n";
            }
            
        }

        private void HandleRequest()
        {
            netStream = socket.GetStream();
            reader = new BinaryReader(netStream);
            writer = new BinaryWriter(netStream);

            while (true)
            {
                var cmd = reader.ReadString();
                switch(cmd)
                {
                    case "move":
                        var r = reader.ReadInt32();
                        var c = reader.ReadInt32();
                        Application.Current.Dispatcher.Invoke(() => { board[r, c].Owner = opponent; });
                        currentTurn = !currentTurn;
                        break;
                    case "chat":
                        var msg = reader.ReadString() + "\n";
                        Application.Current.Dispatcher.Invoke(() => { ChatLog.Text += msg; });
                        break;
                    case "bye": break;
                }
            }

            socket.Close();
        }
    }
}
