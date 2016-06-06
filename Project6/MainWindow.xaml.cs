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
        PlayerID player, opponent, winner;
        bool currentTurn = false;        
        TcpListener listener;
        TcpClient socket;
        NetworkStream netStream;
        BinaryReader reader;
        BinaryWriter writer;
        TTTCell[,] board = new TTTCell[3, 3];
        int[] Rows, Columns, Diags;

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
            winner = PlayerID.None;
            Rows = new int[3];
            Columns = new int[3];
            Diags = new int[2];
            for (int r = 0; r < 3; r++)
                for (int c = 0; c < 3; c++)
                    board[r, c].Owner = PlayerID.None;
        }

        void CellClicked(object sender, InputEventArgs e)
        {
            if (!currentTurn) return;            
            var cell = sender as TTTCell;
            if (cell.Owner == PlayerID.None)
            {
                currentTurn = !currentTurn;
                cell.Owner = player;
                writer.Write("move");
                writer.Write(cell.Row);
                writer.Write(cell.Column);
                if (CheckWinner(cell.Row, cell.Column, player))
                    ChatLog.Text += "You have won!\n";
            }
        }

        bool CheckWinner(int r, int c, PlayerID id)
        {
            var num = 0;
            if (id == player) num = 1;
            else num = -1;
            Rows[r] += num;
            Columns[c] += num;
            if (r == 0 && c == 0) Diags[0] += num;
            else if (r == 0 && c == 2) Diags[1] += num;
            else if (r == 2 && c == 0) Diags[1] += num;
            else if (r == 2 && c == 2) Diags[0] += num;
            else if (r == 1 && c == 1)
            {
                Diags[0] += num;
                Diags[1] += num;
            }
            foreach (int score in Rows)
                if (score == 3) winner = player;
                else if (score == -3) winner = opponent;
            foreach (int score in Columns)
                if (score == 3) winner = player;
                else if (score == -3) winner = opponent;
            foreach (int score in Diags)
                if (score == 3) winner = player;
                else if (score == -3) winner = opponent;
            if (winner == PlayerID.None)
                return false;
            else
                return true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(Message.Text.Length != 0)
            {
                writer.Write("chat");
                writer.Write(Message.Text);
                var msg = "You> " + Message.Text + "\n";
                ChatLog.Text += msg;
                Message.Text = "";
            }
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
                        if(CheckWinner(r, c, opponent))
                            Application.Current.Dispatcher.Invoke(() => { ChatLog.Text += "You have lost\n"; });
                        break;
                    case "chat":
                        var msg = "Opponent> " + reader.ReadString() + "\n";
                        Application.Current.Dispatcher.Invoke(() => { ChatLog.Text += msg; });
                        break;
                    case "bye": break;
                }
            }

            socket.Close();
        }
    }
}
