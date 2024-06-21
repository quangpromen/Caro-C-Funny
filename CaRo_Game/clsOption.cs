using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CaRo_Game
{
    enum LuatChoi
    {
        None,
        International,
        Vietnamese,
    }
    enum ChessStyle
    {
        Chess1,
        Chess2,
        Chess3,
    }
    enum BackgounrStyle
    {
        Background1,
        Background2,
        Background3,
        Background4,
    }
    class clsOption
    {
        private LuatChoi gamePlay = LuatChoi.None;//Luật chơi
        private Player whoPlayWith = Player.None;//Kiểu chơi
        private string music = "";//Nhạc nền
        private ChessStyle kindOfChess = ChessStyle.Chess1;//Loại cờ
        private BackgounrStyle kindOfBackground = BackgounrStyle.Background1;//Loại nền
        private int time=20;//Thời gian
        private string playerA = "";//Tên người chơi thứ 1
        private string playerB = "";//Tên người chơi thứ 2
        private Player luotChoi= Player.Human;//Lượt đi
        private int playerAScore = 0;//Điểm người chơi thứ 1
        private int playerBScore = 0;//Điểm người chơi thứ 2
        public int PlayerAScore 
        {
            get { return this.playerAScore; }
            set { this.playerAScore = value; }
        }
        public int PlayerBScore
        {
            get { return this.playerBScore; }
            set { this.playerBScore = value; }
        }
        public Player LuotChoi
        {
            get{return this.luotChoi;}
            set{this.luotChoi=value;}
        }
        public LuatChoi GamePlay
        {
            get { return this.gamePlay; }
            set { this.gamePlay = value; }
        }
        public Player WhoPlayWith
        {
            get { return this.whoPlayWith; }
            set { this.whoPlayWith = value; }
        }
        public string Music
        {
            get { return this.music; }
            set { this.music = value; }
        }
        public ChessStyle KindOfChess
        {
            get { return this.kindOfChess; }
            set { this.kindOfChess = value; }
        }
        public BackgounrStyle KindOfBackground
        {
            get { return this.kindOfBackground; }
            set { this.kindOfBackground = value; }
        }
        public int Time
        {
            get { return this.time; }
            set { this.time = value; }
        }
        public string PlayerAName
        {
            get { return this.playerA; }
            set { this.playerA = value; }
        }
        public string PlayerBName
        {
            get { return this.playerB; }
            set { this.playerB = value; }
        }
        public clsOption()
        { 
            
        }
    }
}
