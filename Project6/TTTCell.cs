using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Project6
{
    enum PlayerID { X, O, None };

    class TTTCell : Label
    {
        public readonly int Row;
        public readonly int Column;
        private PlayerID _owner;
        public PlayerID Owner
        {
            get { return this._owner; }
            set
            {
                this._owner = value;
                switch(this._owner)
                {
                    case PlayerID.X: this.Content = "X"; break;
                    case PlayerID.O: this.Content = "O"; break;
                    case PlayerID.None: this.Content = " "; break;
                }
            }
        }

        public TTTCell(int r, int c)
        {
            Row = r;
            Column = c;
            Owner = PlayerID.X;
            this.FontSize = 50;
            this.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
            this.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
        }        
    }
}
