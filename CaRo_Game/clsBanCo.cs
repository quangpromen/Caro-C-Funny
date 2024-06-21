using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;
using System.Windows.Threading;
using System.IO;
using System.Windows.Media.Imaging;

namespace CaRo_Game
{
    enum Player
    {
        None = 0,
        Human = 1,
        Com = 2,
    }
    struct Node
    {
        public int Row;
        public int Column;
        public Node(int rw, int cl)
        {
            this.Row = rw;
            this.Column = cl;
        }
    }

    class clsBanCo
    {
        //Các biến chính
        private int row, column; //Số hàng, cột
        private const int length = 30;//Độ dài mỗi ô
        public Player currPlayer; //lượt đi
        public Player[,] board; //mảng lưu vị trí các con cờ
        private Player end; //biến kiểm tra trò chơi kết thúc
        private MainWindow frmParent; //Form thực hiện
        private Grid grdBanCo; // Nơi vẽ bàn cờ
        private clsLuongGiaBanCo eBoard; //Bảng lượng giá bàn cờ
        private cls5OWin OWin; // Kiểm tra 5 ô win
        private clsVideo video; // Quay lại trò chơi
        public clsOption Option; // Tùy chọn trò chơi
        //Các biến phụ
        private hinhvuong hv; 
        private Image coAo1; // cờ ảo cho người chơi thứ 1
        private Image coAo2; // cờ ảo cho người chơi thứ 2
        // Điểm lượng giá
        public int[] PhongThu = new int[5] { 0, 1, 9, 85, 769 };
        public int[] TanCong = new int[5] { 0, 2, 28, 256, 2308 };
        
        

        //Properties
        public Player End
        {
            get { return this.end; }
            set { this.end = value; }
        }
        public int Row
        {
            get { return this.row; }
        }
        public int Column
        {
            get { return this.column; }
        }
        //Contructors
            public clsBanCo(MainWindow frm, Grid grd)
            {
                Option = new clsOption();
                OWin = new cls5OWin();
                row = column = 18;
                currPlayer = Player.None;
                end = Player.None;
                frmParent = frm;
                grdBanCo = grd;
                board = new Player[row, column];
                ResetBoard();
                eBoard = new clsLuongGiaBanCo(this);
                hv = new hinhvuong();
                coAo1 = new Image(); coAo2 = new Image();
                coAo1.Source = new BitmapImage(new Uri("pack://application:,,,/Image/Chess/Chess_0_1.png")); coAo2.Source = new BitmapImage(new Uri("pack://application:,,,/Image/Chess/Chess_X_1.png"));
                CreateHV();
                CreateCoAo();
                grdBanCo.Children.Add(hv);
                grdBanCo.MouseDown += new System.Windows.Input.MouseButtonEventHandler(grdBanCo_MouseDown);
                grdBanCo.MouseMove += new System.Windows.Input.MouseEventHandler(grdBanCo_MouseMove);
                video = new clsVideo(this);
            }

            public void CreateCoAo()
            {
		        if(Option.KindOfChess==ChessStyle.Chess1)
                { coAo1.Source = new BitmapImage(new Uri("pack://application:,,,/Image/Chess/Chess_0_1.png")); coAo2.Source = new BitmapImage(new Uri("pack://application:,,,/Image/Chess/Chess_X_1.png")); }
			        else
			        {
				        if(Option.KindOfChess==ChessStyle.Chess2)
                        { coAo1.Source = new BitmapImage(new Uri("pack://application:,,,/Image/Chess/Chess_0_2.png")); coAo2.Source = new BitmapImage(new Uri("pack://application:,,,/Image/Chess/Chess_X_2.png")); }
				        else
                        { coAo1.Source = new BitmapImage(new Uri("pack://application:,,,/Image/Chess/Chess_0_3.png")); coAo2.Source = new BitmapImage(new Uri("pack://application:,,,/Image/Chess/Chess_X_3.png")); }
			        }
    				
                coAo2.Width = coAo2.Height = length;
                coAo2.HorizontalAlignment = 0;
                coAo2.VerticalAlignment = 0;
                coAo2.Opacity = 0;

                coAo1.Width = coAo1.Height = length;
                coAo1.HorizontalAlignment = 0;
                coAo1.VerticalAlignment = 0;
                coAo1.Opacity = 0;
            }
            private void CreateHV()
             {
                 hv.Width = hv.Height = 50;
                 hv.HorizontalAlignment = 0;
                 hv.VerticalAlignment = 0;
                 hv.Opacity = 0;
             }
             
            private void grdBanCo_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
             {
                 //Hàm này tạo bóng mờ khi di chuyển chuột
                 Point toado = e.GetPosition(grdBanCo);//Lấy tọa độ chuột
                 if (toado.X <= 540 && toado.Y <= 540) //Nếu tọa độ chuột nằm trong vùng kiểm soát
                 {
                     //Xử lý tọa độ
                     int cl = ((int)toado.X / length);
                     int rw = ((int)toado.Y / length);
                     if (board[rw, cl] == Player.None)//Nếu ô bấm còn trống
                     {
                         if (currPlayer == Player.Human)//Nếu lượt đi là người thì in bóng mờ của người,ẩn bóng mờ của máy
                         {
                             coAo2.Opacity = 0;
                             coAo1.Opacity = 0.5;
                             coAo1.Margin = new Thickness(cl * length , rw * length , 0, 0); 
                         }
                         else if (currPlayer == Player.Com) //Nếu lượt đi là máy thì in bóng mờ của máy,ẩn bóng mờ của người
                         {
                             coAo1.Opacity = 0;
                             coAo2.Opacity = 0.5;
                             coAo2.Margin = new Thickness(cl * length , rw * length , 0, 0);
                         }
                     }
                 }
             }
            // Thiết lập các giá trị lưu vị trí bàn cờ.
            public void ResetBoard()
             {
                 for (int i = 0; i < row; i++)
                 {
                     for (int j = 0; j < column; j++)
                     {
                         board[i, j] = Player.None;
                     }
                 }
             }
        //Bắt đầu trò chơi mới
         public void NewGame()
         {
             currPlayer = this.Option.LuotChoi;//Lấy thông tin lượt chơi
             if (this.Option.WhoPlayWith == Player.Com)//Nếu chọn kiểu chơi với máy
             {
                 if (this.currPlayer == Player.Com)//Nếu lược đi là máy
                 {
                     DiNgauNhien();
                 }
             }
         }
        //Thiết lập lại toàn bộ dữ liệu bàn cờ
         public void ResetAllBoard()
         {
             OWin = new cls5OWin();
             video = new clsVideo(this);
             grdBanCo.Children.Clear();
             grdBanCo.Children.Add(hv);
             grdBanCo.Children.Add(coAo1);
             grdBanCo.Children.Add(coAo2);
             ResetBoard();
             end = Player.None;
             this.DrawGomokuBoard();
         }
        //Bắt đầu lại trò chơi mới
         public void PlayAgain()
         {
             OWin = new cls5OWin();
             video = new clsVideo(this);
             grdBanCo.Children.Clear();
             grdBanCo.Children.Add(hv);
             grdBanCo.Children.Add(coAo1);
             grdBanCo.Children.Add(coAo2);
             ResetBoard();
             this.DrawGomokuBoard();
             if (this.Option.WhoPlayWith == Player.Com)
             {
                 if (end == Player.None)
                 {
                     currPlayer = Player.Com;
                     this.Option.PlayerBScore++;
                 }
                 if (this.currPlayer == Player.Com && this.Option.WhoPlayWith == Player.Com)
                 {
                     DiNgauNhien();
                 }
             }
             else
             {
                 if (end == Player.None)
                 {
                     if (currPlayer == Player.Human)
                     {
                         currPlayer = Player.Com;
                         this.Option.PlayerBScore++;
                     }
                     else if (currPlayer == Player.Com)
                     {
                         currPlayer = Player.Human;
                         this.Option.PlayerAScore++;
                     }
                 }
             }
             end = Player.None;
         }

         public void DiNgauNhien()
         {
             if (currPlayer == Player.Com)
             {
                 board[row / 2, column / 2] = currPlayer;
                 DrawDataBoard(row / 2, column / 2,true,true);
                 currPlayer = Player.Human;
                 OnComDanhXong();//Khai báo sự kiện khi máy đánh xong
             }
         }

        //Hàm đánh cờ
         public void grdBanCo_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
         {
             System.GC.Collect();//Thu gôm rác
             if (this.Option.WhoPlayWith == Player.Com)//Nếu chọn kiểu chơi đánh với máy
             {
                 Point toado = e.GetPosition(grdBanCo); //Lấy tọa độ chuột
                 //Xử lý tọa độ
                 int cl = ((int)toado.X / length);
                 int rw = ((int)toado.Y / length);
                 Node node = new Node();
                 if (board[rw, cl] == Player.None) //Nếu ô bấm chưa có cờ
                 {
                     if (currPlayer == Player.Human && end == Player.None)//Nếu lượt đi là người và trận đấu chưa kết thúc
                     {
                         board[rw, cl] = currPlayer;//Lưu loại cờ vừa đánh vào mảng
                         DrawDataBoard(rw, cl,true,true);//Vẽ con cờ theo lượt chơi
                         end = CheckEnd(rw, cl);//Kiểm tra xem trận đấu kết thúc chưa
                         if (end == Player.Human)//Nếu người thắng cuộc là người
                         {
                             OnWin();//Khai báo sự kiện Win
                             OnWinOrLose();//Hiển thị 5 ô Win.
                         }
                         else if(end == Player.None) //Nếu trận đấu chưa kết thúc
                         {
                             currPlayer = Player.Com;//Thiết lập lại lượt chơi
                             OnHumanDanhXong(); // Khai báo sự kiện người đánh xong
                         }
                     }
                     if (currPlayer == Player.Com && end == Player.None)//Nếu lượt đi là máy và trận đấu chưa kết thúc
                     {
                         //Tìm đường đi cho máy
                         eBoard.ResetBoard();
                         LuongGia(Player.Com);//Lượng giá bàn cờ cho máy
                         node = eBoard.GetMaxNode();//lưu vị trí máy sẽ đánh
                         int r, c;
                         r = node.Row; c = node.Column;
                         board[r, c] = currPlayer; //Lưu loại cờ vừa đánh vào mảng
                         DrawDataBoard(r, c, true, true); //Vẽ con cờ theo lượt chơi
                         end = CheckEnd(r, c);//Kiểm tra xem trận đấu kết thúc chưa

                         if (end == Player.Com)//Nếu máy thắng
                         {
                             OnLose();//Khai báo sư kiện Lose
                             OnWinOrLose();//Hiển thị 5 ô Lose.
                         }
                         else if (end == Player.None)
                         {
                             currPlayer = Player.Human;//Thiết lập lại lượt chơi
                             OnComDanhXong();// Khai báo sự kiện người đánh xong
                         }
                     }
                 }
             }
             else if (this.Option.WhoPlayWith == Player.Human) //Nếu chọn kiểu chơi 2 người đánh với nhau
             {
                 //Player.Com sẽ đóng vai trò người chơi thứ 2
                 Point toado = e.GetPosition(grdBanCo);//Lấy thông tin tọa độ chuột
                 //Xử lý tọa độ
                 int cl = ((int)toado.X / length);
                 int rw = ((int)toado.Y / length);
                 if (board[rw, cl] == Player.None)//Nếu ô bấm chưa có cờ
                 {
                     if (currPlayer == Player.Human && end == Player.None)//Nếu lượt đi là người và trận đấu chưa kết thúc
                     {
                         board[rw, cl] = currPlayer;//Lưu loại cờ vừa đánh vào mảng
                         DrawDataBoard(rw, cl, true, true);//Vẽ con cờ theo lượt chơi
                         end = CheckEnd(rw, cl);//Kiểm tra xem trận đấu kết thúc chưa
                         if (end == Player.Human)//Nếu người chơi 1 thắng
                         {
                             currPlayer = Player.Human; //Thiết lập lại lượt chơi
                             OnWin();//Khai báo sư kiện Win
                             OnWinOrLose();//Hiển thị 5 ô Win.
                         }
                         else
                         {
                             currPlayer = Player.Com;//Thiết lập lại lượt chơi
                             OnHumanDanhXong();// Khai báo sự kiện người chơi 1 đánh xong
                         }
                     }
                     else if (currPlayer == Player.Com && end == Player.None)
                     {
                         board[rw, cl] = currPlayer;//Lưu loại cờ vừa đánh vào mảng
                         DrawDataBoard(rw, cl, true, true);//Vẽ con cờ theo lượt chơi
                         end = CheckEnd(rw, cl);//Kiểm tra xem trận đấu kết thúc chưa
                         if (end == Player.Com)//Nếu người chơi 2 thắng
                         {
                             OnWin();//Khai báo sư kiện Win
                             OnWinOrLose();//Hiển thị 5 ô Win.
                         }
                         else
                         {
                             currPlayer = Player.Human;//Thiết lập lại lượt chơi
                             OnComDanhXong();// Khai báo sự kiện người chơi 2 đánh xong
                         }
                     }
                 }
             }
         }
        //delegate sự kiện Win
         public delegate void WinEventHander();
         public event WinEventHander WinEvent;
         public void OnWin()
         {
             if (WinEvent != null)
             {
                 WinEvent();
             }
         }
         //delegate sự kiện Lose
         public delegate void LoseEventHander();
         public event LoseEventHander LoseEvent;
         public void OnLose()
         {
             if (LoseEvent != null)
             {
                 LoseEvent();
             }
         }
        //delegate sự kiện Replay
         public delegate void ReplayEventHander();
         public event ReplayEventHander ReplayEvent;
         public void OnReplay()
         {
             if (ReplayEvent != null)
             {
                 ReplayEvent();
             }
         }

        //delegate sự kiện máy đánh xong
         public delegate void ComDanhXongEventHandler();
         public event ComDanhXongEventHandler ComDanhXongEvent;
         private void OnComDanhXong()
         {
             if (ComDanhXongEvent != null)
             {
                 ComDanhXongEvent();
             }
         }
         //delegate sự kiện người đánh xong
         public delegate void HumanDanhXongEventHandler();
         public event HumanDanhXongEventHandler HumanDanhXongEvent;
         private void OnHumanDanhXong()
         {
             if (HumanDanhXongEvent != null)
             {
                 HumanDanhXongEvent();
             }
         }
        //Vẽ lại thế cờ khi đọc dữ liệu từ file
         public void VeTheCo()
         {
             Player temp = this.currPlayer;
             for (int i = 0; i < Row; i++)
             {
                 for (int j = 0; j < Column; j++)
                 {
                     this.currPlayer = board[i, j];
                     DrawDataBoard(i, j,false,false);
                 }
             }
             this.currPlayer = temp;
         }
        //Hàm save
         public void Save(string name)
         {
             using (StreamWriter sr = new StreamWriter("Save\\"+name+".cro"))
             {
                 //ghi trận đấu kết thúc chưa
                 sr.WriteLine(this.end);
                 //ghi độ khó (thời gian)
                 sr.WriteLine(this.Option.Time.ToString());
                 //ghi kiểu chơi
                 sr.WriteLine(this.Option.WhoPlayWith.ToString());
                 //ghi Luật chơi
                 sr.WriteLine(this.Option.GamePlay.ToString());
                 //ghi tên người chơi
                 sr.WriteLine(this.Option.PlayerAName);
                 sr.WriteLine(this.Option.PlayerBName);
                 //ghi tỷ số
                 sr.WriteLine(this.Option.PlayerAScore.ToString());
                 sr.WriteLine(this.Option.PlayerBScore.ToString());
                 //ghi lượt đi
                 if (this.currPlayer == Player.None)
                 {
                     sr.WriteLine(0);
                 }
                 else if (this.currPlayer == Player.Human)
                 {
                     sr.WriteLine(1);
                 }
                 else if (this.currPlayer == Player.Com)
                 {
                     sr.WriteLine(2);
                 }
                 //ghi thế cờ
                 for (int i = 0; i < this.row; i++)
                 {
                     for (int j = 0; j < this.column; j++)
                     {
                         sr.WriteLine(this.board[i, j].ToString());
                     }
                 }
                 //ghi video
                 sr.WriteLine(this.video.length.ToString());
                 for (int i = 0; i < this.video.length; i++)
                 {
                     sr.WriteLine(this.video.ToaDo[i].Row.ToString());
                     sr.WriteLine(this.video.ToaDo[i].Column.ToString());
                     sr.WriteLine(this.video.player[i].ToString());
                 }
             }
         }
         public void Load(string name)
         {
             string line = "";
            
                 using (StreamReader sr = new StreamReader("Save\\" + name + ".cro"))
                 {
                     //Load trận đấu kết thúc chưa
                     if ((line = sr.ReadLine()) != null)
                     {
                         if (line == "Human")
                             this.end = Player.Human;
                         else if (line == "Com")
                             this.end = Player.Com;
                         else this.end = Player.None;
                     }
                     //Load độ khó (thời gian)
                     if ((line = sr.ReadLine()) != null)
                     {
                         this.Option.Time = int.Parse(line);
                     }
                     //Load kiểu chơi
                     if ((line = sr.ReadLine()) != null)
                     {
                         if (line == "Human")
                             this.Option.WhoPlayWith = Player.Human;
                         else if (line == "Com")
                             this.Option.WhoPlayWith = Player.Com;
                     }

                     //Load luật chơi
                     if ((line = sr.ReadLine()) != null)
                     {
                         if (line == "International")
                             this.Option.GamePlay = LuatChoi.International;
                         else if (line == "Vietnamese")
                             this.Option.GamePlay = LuatChoi.Vietnamese;
                     }
                     //Load tên người chơi
                     if ((line = sr.ReadLine()) != null)
                     {
                         this.Option.PlayerAName = line;
                     }
                     if ((line = sr.ReadLine()) != null)
                     {
                         this.Option.PlayerBName = line;
                     }
                     //Load tỷ số
                     if ((line = sr.ReadLine()) != null)
                     {
                         this.Option.PlayerAScore = int.Parse(line);
                     }
                     if ((line = sr.ReadLine()) != null)
                     {
                         this.Option.PlayerBScore = int.Parse(line);
                     }

                     //Load lượt đi
                     int luotdi = -1;
                     if ((line = sr.ReadLine()) != null)
                     {
                         luotdi = int.Parse(line);
                     }
                     if (luotdi == 0)
                     {
                         this.currPlayer = Player.None;
                     }
                     else if (luotdi == 1)
                     {
                         this.currPlayer = Player.Human;
                     }
                     else if (luotdi == 2)
                     {
                         this.currPlayer = Player.Com;
                     }
                     //Load thế cờ
                     for (int i = 0; i < this.row; i++)
                     {
                         for (int j = 0; j < this.column; j++)
                         {
                             if ((line = sr.ReadLine()) != null)
                             {
                                 if (line == "None")
                                     this.board[i, j] = Player.None;
                                 else if (line == "Human")
                                     this.board[i, j] = Player.Human;
                                 else if (line == "Com")
                                     this.board[i, j] = Player.Com;
                             }
                         }
                     }
                     //Vẽ lại thế cờ 
                     this.VeTheCo();
                     //Lấy video
                     int chieudai = 0;
                     int x = 0;
                     int y = 0;
                     Player temp = Player.None;
                     if ((line = sr.ReadLine()) != null)
                         chieudai = int.Parse(line);
                     for (int i = 0; i < chieudai; i++)
                     {
                         //Lấy tọa độ x
                         if ((line = sr.ReadLine()) != null)
                         {
                             x = int.Parse(line);
                         }
                         //Lấy tọa độ y
                         if ((line = sr.ReadLine()) != null)
                         {
                             y = int.Parse(line);
                         }
                         //Lấy thông tin loại cờ

                         if ((line = sr.ReadLine()) != null)
                         {
                             if (line == "Human")
                                 temp = Player.Human;
                             else if (line == "Com")
                                 temp = Player.Com;
                         }
                         this.video.Add(new Node(x, y), temp);
                     }
                 }
             
             
         }
        //Hàm lượng giá thế cờ
         private void LuongGia(Player player)
         {
             int cntHuman = 0, cntCom = 0;//Biến đếm Human,Com
             #region Luong gia cho hang
             for (int i = 0; i < row; i++)
             {
                 for (int j = 0; j < column - 4; j++)
                 {
                     //Khởi tạo biến đếm
                     cntHuman = cntCom = 0;
                     //Đếm số lượng con cờ trên 5 ô kế tiếp của 1 hàng
                     for (int k = 0; k < 5; k++)
                     {
                         if (board[i, j + k] == Player.Human) cntHuman++;
                         if (board[i, j + k] == Player.Com) cntCom++;
                     }
                     //Lượng giá
                     //Nếu 5 ô kế tiếp chỉ có 1 loại cờ (hoặc là Human,hoặc la Com)
                     if (cntHuman * cntCom == 0 && cntHuman != cntCom)
                     {
                         //Gán giá trị cho 5 ô kế tiếp của 1 hàng
                         for (int k = 0; k < 5; k++)
                         {
                             //Nếu ô đó chưa có quân đi
                             if (board[i, j + k] == Player.None)
                             {
                                 //Nếu trong 5 ô đó chỉ tồn tại cờ của Human
                                 if (cntCom == 0)
                                 {
                                     //Nếu đối tượng lượng giá là Com
                                     if (player == Player.Com)
                                     {
                                         //Vì đối tượng người chơi là Com mà trong 5 ô này chỉ có Human
                                         //nên ta sẽ cộng thêm điểm phòng thủ cho Com
                                         eBoard.GiaTri[i, j + k] += PhongThu[cntHuman];
                                     }
                                     //Ngược lại cộng điểm phòng thủ cho Human
                                     else
                                     {
                                         eBoard.GiaTri[i, j + k] += TanCong[cntHuman];
                                     }
                                     //Nếu chơi theo luật Việt Nam
                                     if(this.Option.GamePlay == LuatChoi.Vietnamese)
                                         //Xét trường hợp chặn 2 đầu
                                        //Nếu chận 2 đầu thì gán giá trị cho các ô đó bằng 0
                                         if (j - 1 >= 0 && j + 5 <= column - 1 && board[i, j - 1] == Player.Com && board[i, j + 5] == Player.Com)
                                         {
                                             eBoard.GiaTri[i, j + k] = 0;
                                         }

                                 }
                                 //Tương tự như trên
                                 if (cntHuman == 0) //Nếu chỉ tồn tại Com
                                 {
                                     if (player == Player.Human) //Nếu người chơi là Người
                                     {
                                         eBoard.GiaTri[i, j + k] += PhongThu[cntCom];
                                     }
                                     else
                                     {
                                         eBoard.GiaTri[i, j + k] += TanCong[cntCom];
                                     }
                                     //Trường hợp chặn 2 đầu
                                     if (this.Option.GamePlay == LuatChoi.Vietnamese)
                                         if (j - 1 >= 0 && j + 5 <= column - 1 && board[i, j - 1] == Player.Human && board[i, j + 5] == Player.Human)
                                         {
                                             eBoard.GiaTri[i, j + k] = 0;
                                         }

                                 }
                                 if ((j + k - 1 > 0) && (j + k + 1 <= column - 1) && (cntHuman == 4 || cntCom == 4)
                                    && (board[i, j + k - 1] == Player.None || board[i, j + k + 1] == Player.None))
                                 {
                                     eBoard.GiaTri[i, j + k] *= 3;
                                 }
                             }
                         }
                     }
                 }
             }
             #endregion
             //Tương tự như lượng giá cho hàng
             #region Luong gia cho cot
             for (int i = 0; i < row - 4; i++)
             {
                 for (int j = 0; j < column; j++)
                 {
                     cntHuman = cntCom = 0;
                     for (int k = 0; k < 5; k++)
                     {
                         if (board[i + k, j] == Player.Human) cntHuman++;
                         if (board[i + k, j] == Player.Com) cntCom++;
                     }
                     if (cntHuman * cntCom == 0 && cntCom != cntHuman)
                     {
                         for (int k = 0; k < 5; k++)
                         {
                             if (board[i + k, j] == Player.None)
                             {
                                 if (cntCom == 0)
                                 {
                                     if (player == Player.Com) eBoard.GiaTri[i + k, j] += PhongThu[cntHuman];
                                     else eBoard.GiaTri[i + k, j] += TanCong[cntHuman];
                                     // Truong hop bi chan 2 dau.
                                     if ((i - 1) >= 0 && (i + 5) <= row - 1 && board[i - 1, j] == Player.Com && board[i + 5, j] == Player.Com)
                                     {
                                         eBoard.GiaTri[i + k, j] = 0;
                                     }
                                 }
                                 if (cntHuman == 0)
                                 {
                                     if (player == Player.Human) eBoard.GiaTri[i + k, j] += PhongThu[cntCom];
                                     else eBoard.GiaTri[i + k, j] += TanCong[cntCom];
                                     // Truong hop bi chan 2 dau.
                                     if (this.Option.GamePlay == LuatChoi.Vietnamese)
                                         if (i - 1 >= 0 && i + 5 <= row - 1 && board[i - 1, j] == Player.Human && board[i + 5, j] == Player.Human)
                                             eBoard.GiaTri[i + k, j] = 0;
                                 }
                                 if ((i + k - 1) >= 0 && (i + k + 1) <= row - 1 && (cntHuman == 4 || cntCom == 4)
                                     && (board[i + k - 1, j] == Player.None || board[i + k + 1, j] == Player.None))
                                 {
                                     eBoard.GiaTri[i + k, j] *= 3;
                                 }
                             }
                         }
                     }
                 }
             }
             #endregion
             //Tương tự như lượng giá cho hàng
             #region  Luong gia tren duong cheo chinh (\)
             for (int i = 0; i < row - 4; i++)
             {
                 for (int j = 0; j < column - 4; j++)
                 {
                     cntHuman = cntCom = 0;
                     for (int k = 0; k < 5; k++)
                     {
                         if (board[i + k, j + k] == Player.Human) cntHuman++;
                         if (board[i + k, j + k] == Player.Com) cntCom++;
                     }
                     if (cntHuman * cntCom == 0 && cntCom != cntHuman)
                     {
                         for (int k = 0; k < 5; k++)
                         {
                             if (board[i + k, j + k] == Player.None)
                             {
                                 if (cntCom == 0)
                                 {
                                     if (player == Player.Com) eBoard.GiaTri[i + k, j + k] += PhongThu[cntHuman];
                                     else eBoard.GiaTri[i + k, j + k] += TanCong[cntHuman];
                                     // Truong hop bi chan 2 dau.
                                     if(this.Option.GamePlay == LuatChoi.Vietnamese)
                                         if (i - 1 >= 0 && j - 1 >= 0
                                             && i + 5 <= row - 1 && j + 5 <= column - 1
                                             && board[i - 1, j - 1] == Player.Com && board[i + 5, j + 5] == Player.Com)
                                             eBoard.GiaTri[i + k, j + k] = 0;
                                 }
                                 if (cntHuman == 0)
                                 {
                                     if (player == Player.Human) eBoard.GiaTri[i + k, j + k] += PhongThu[cntCom];
                                     else eBoard.GiaTri[i + k, j + k] += TanCong[cntCom];
                                     // Truong hop bi chan 2 dau.
                                     if (this.Option.GamePlay == LuatChoi.Vietnamese)
                                         if ((i - 1) >= 0 && j - 1 >= 0
                                             && i + 5 <= row - 1 && j + 5 <= column - 1
                                             && board[i - 1, j - 1] == Player.Human && board[i + 5, j + 5] == Player.Human)
                                         {
                                             eBoard.GiaTri[i + k, j + k] = 0;
                                         }
                                 }
                                 if ((i + k - 1) >= 0 && (j + k - 1) >= 0 && (i + k + 1) <= row - 1 && (j + k + 1) <= column - 1 && (cntHuman == 4 || cntCom == 4)
                                     && (board[i + k - 1, j + k - 1] == Player.None || board[i + k + 1, j + k + 1] == Player.None))
                                 {
                                     eBoard.GiaTri[i + k, j + k] *= 3;
                                 }
                             }
                         }
                     }
                 }
             }
             #endregion
             //Tương tự như lượng giá cho hàng
             #region Luong gia tren duong cheo phu (/)
             for (int i = 4; i < row - 4; i++)
             {
                 for (int j = 0; j < column - 4; j++)
                 {
                     cntCom = 0; cntHuman = 0;
                     for (int k = 0; k < 5; k++)
                     {
                         if (board[i - k, j + k] == Player.Human) cntHuman++;
                         if (board[i - k, j + k] == Player.Com) cntCom++;
                     }
                     if (cntHuman * cntCom == 0 && cntHuman != cntCom)
                     {
                         for (int k = 0; k < 5; k++)
                         {
                             if (board[i - k, j + k] == Player.None)
                             {
                                 if (cntCom == 0)
                                 {
                                     if (player == Player.Com) eBoard.GiaTri[i - k, j + k] += PhongThu[cntHuman];
                                     else eBoard.GiaTri[i - k, j + k] += TanCong[cntHuman];
                                     // Truong hop bi chan 2 dau.
                                     if (i + 1 <= row - 1&&j - 1>=0&&i-5>=0&&j+5<=column-1&& board[i + 1, j - 1] == Player.Com && board[i - 5, j + 5] == Player.Com)
                                     {
                                         eBoard.GiaTri[i - k, j + k] = 0;
                                     }
                                 }
                                 if (cntHuman == 0)
                                 {
                                     if (player == Player.Human) eBoard.GiaTri[i - k, j + k] += PhongThu[cntCom];
                                     else eBoard.GiaTri[i - k, j + k] += TanCong[cntCom];
                                     // Truong hop bi chan 2 dau.
                                     if(this.Option.GamePlay== LuatChoi.Vietnamese)
                                         if (i + 1 <= row - 1 && j - 1 >= 0 && i - 5 >= 0 && j + 5 <= column - 1 && board[i + 1, j - 1] == Player.Human && board[i - 5, j + 5] == Player.Human)
                                         {
                                             eBoard.GiaTri[i - k, j + k] = 0;
                                         }
                                 }
                                 if ((i - k + 1) <= row - 1 && (j + k - 1) >= 0
                                     && (i - k - 1) >= 0 && (j + k + 1) <= column - 1
                                     && (cntHuman == 4 || cntCom == 4)
                                     && (board[i - k + 1, j + k - 1] == Player.None || board[i - k - 1, j + k + 1] == Player.None))
                                 {
                                     eBoard.GiaTri[i - k, j + k] *= 3;
                                 }
                             }
                         }
                     }
                 }
             }
             #endregion
         }
        //Hàm lấy đối thủ của người chơi hiện tại
         private Player DoiNgich(Player cur)
         {
             if (cur == Player.Com) return Player.Human;
             if (cur == Player.Human) return Player.Com;
             return Player.None;
         }
        //Hàm kiểm tra trận đấu kết thúc chưa
         private Player CheckEnd(int rw, int cl)
         {
             int rowTemp = rw;
             int colTemp = cl;
             int count1, count2, count3, count4;
             count1 = count2 = count3 = count4 = 1;
             Player cur = board[rw, cl];
             OWin.Reset();
             OWin.Add(new Node(rowTemp, colTemp));
             #region Kiem Tra Hang Ngang
             while (colTemp - 1 >= 0 && board[rowTemp, colTemp - 1] == cur)
             {
                 OWin.Add(new Node(rowTemp, colTemp - 1));
                 count1++;
                 colTemp--;
             }
             colTemp = cl;
             while (colTemp + 1 <= column - 1 && board[rowTemp, colTemp + 1] == cur)
             {
                 OWin.Add(new Node(rowTemp, colTemp + 1));
                 count1++;
                 colTemp++;
             }
             if (count1 == 5)
             {
                 if (this.Option.GamePlay == LuatChoi.Vietnamese)
                 {
                     if ((colTemp - 5 >= 0 && colTemp + 1 <= column - 1 && board[rowTemp, colTemp + 1] == board[rowTemp, colTemp - 5] && board[rowTemp, colTemp + 1] == DoiNgich(cur)) ||
                         (colTemp - 5 < 0 && (board[rowTemp, colTemp + 1] == DoiNgich(cur))) ||
                         (colTemp + 1 > column - 1 && (board[rowTemp, colTemp - 5] == DoiNgich(cur))))
                     { }
                     else
                         return cur;
                 }
                 else return cur;
             }
             #endregion
             #region Kiem Tra Hang Doc
             OWin.Reset();
             colTemp = cl;
             OWin.Add(new Node(rowTemp, colTemp));
            
             while (rowTemp - 1 >= 0 && board[rowTemp - 1, colTemp] == cur)
             {
                 OWin.Add(new Node(rowTemp-1, colTemp));
                 count2++;
                 rowTemp--;
             }
             rowTemp = rw;
             while (rowTemp + 1 <= row - 1 && board[rowTemp + 1, colTemp] == cur)
             {
                 OWin.Add(new Node(rowTemp+1, colTemp));
                 count2++;
                 rowTemp++;
             }
             if (count2 == 5)
             {
                 if (this.Option.GamePlay == LuatChoi.Vietnamese)
                 {

                     if ((rowTemp - 5 >= 0 && rowTemp + 1 <= column - 1 && board[rowTemp + 1, colTemp] == board[rowTemp - 5, colTemp] && board[rowTemp + 1, colTemp] == DoiNgich(cur)) ||
                         (rowTemp - 5 < 0 && (board[rowTemp + 1, colTemp] == DoiNgich(cur))) ||
                         (rowTemp + 1 > row - 1 && (board[rowTemp - 5, colTemp] == DoiNgich(cur))))
                     { }
                     else
                         return cur;
                 }
                 else return cur;
             }
             #endregion
             #region Kiem Tra Duong Cheo Chinh (\)
             colTemp = cl;
             rowTemp = rw;
             OWin.Reset();
             OWin.Add(new Node(rowTemp, colTemp));
             while (rowTemp - 1 >= 0 && colTemp - 1 >= 0 && board[rowTemp - 1, colTemp - 1] == cur)
             {
                 OWin.Add(new Node(rowTemp - 1, colTemp - 1));
                 count3++;
                 rowTemp--;
                 colTemp--;
             }
             rowTemp = rw;
             colTemp = cl;
             while (rowTemp + 1 <= row - 1 && colTemp + 1 <= column - 1 && board[rowTemp + 1, colTemp + 1] == cur)
             {
                 OWin.Add(new Node(rowTemp + 1, colTemp + 1));
                 count3++;
                 rowTemp++;
                 colTemp++;
             }
             if (count3 == 5)
             {
                 if (this.Option.GamePlay == LuatChoi.Vietnamese)
                 {
                     if ((colTemp - 5 >= 0 && rowTemp - 5 >= 0 && colTemp + 1 <= column - 1 && rowTemp + 1 <= row - 1 && board[rowTemp + 1, colTemp + 1] == board[rowTemp - 5, colTemp - 5] && board[rowTemp + 1, colTemp + 1] == DoiNgich(cur)) ||
                            ((colTemp - 5 < 0 || rowTemp - 5 < 0) && (board[rowTemp + 1, colTemp + 1] == DoiNgich(cur))) ||
                            ((colTemp + 1 > column - 1 || rowTemp + 1 > row - 1) && (board[rowTemp - 5, colTemp - 5] == DoiNgich(cur))))
                     { }
                     else
                         return cur;
                 }
                 else return cur;
             }
             #endregion
             #region Kiem Tra Duong Cheo Phu
             rowTemp = rw;
             colTemp = cl;
             OWin.Reset();
             OWin.Add(new Node(rowTemp, colTemp));
             while (rowTemp + 1 <= row - 1 && colTemp - 1 >= 0 && board[rowTemp + 1, colTemp - 1] == cur)
             {
                 OWin.Add(new Node(rowTemp + 1, colTemp - 1));
                 count4++;
                 rowTemp++;
                 colTemp--;
             }
             rowTemp = rw;
             colTemp = cl;
             while (rowTemp - 1 >= 0 && colTemp + 1 <= column - 1 && board[rowTemp - 1, colTemp + 1] == cur)
             {
                 OWin.Add(new Node(rowTemp - 1, colTemp + 1));
                 count4++;
                 rowTemp--;
                 colTemp++;
             }
             if (count4 == 5)
             {
                 if (this.Option.GamePlay == LuatChoi.Vietnamese)
                 {
                     if ((rowTemp - 1 >= 0 && rowTemp + 5 <= row - 1 && colTemp + 1 <= column - 1 && colTemp - 5 >= 0 && rowTemp + 1 <= row - 1 && board[rowTemp - 1, colTemp + 1] == board[rowTemp + 5, colTemp - 5] && board[rowTemp - 1, colTemp + 1] == DoiNgich(cur)) ||
                           ((colTemp - 5 < 0 || rowTemp + 5 > row - 1) && (board[rowTemp - 1, colTemp + 1] == DoiNgich(cur))) ||
                           ((colTemp + 1 > column - 1 || rowTemp - 1 < 0) && (board[rowTemp + 5, colTemp - 5] == DoiNgich(cur))))
                     { }
                     else
                         return cur;
                 }
                 else return cur;
             }
             #endregion
             return Player.None;
         }
        //Hàm lấy thông tin 5 ô Win hoặc Lose
         private void OnWinOrLose()
         {
             Node node = new Node();
             for (int i = 0; i < 5; i++)
             {
                 node = OWin.GiaTri[i];
                 hinhvuong hv = new hinhvuong();
                 hv.Height = 50;
                 hv.Width = 50;
                 hv.Opacity = 100;
                 hv.HorizontalAlignment = 0;
                 hv.VerticalAlignment = 0;
                 hv.Margin = new Thickness(node.Column * length-10, node.Row * length-10, 0, 0);
                 grdBanCo.Children.Add(hv);
             }
         }
        //Hàm xem lại trò chơi vừa đấu
         public void XemLai()
         {
             grdBanCo.Children.Clear();
             grdBanCo.Children.Add(hv);
             this.DrawGomokuBoard();
             Node n;
             Player p;
             DispatcherTimer timer = new DispatcherTimer();
             timer.Interval = TimeSpan.FromMilliseconds(500);
             int i = 0;
             timer.Tick += delegate
             {
                 n = video.ToaDo[i];
                 p = video.player[i];
                 currPlayer = p;
                 DrawDataBoard(n.Row, n.Column,false,true);
                 i++;
                 if (i >= video.length)
                 {
                         timer.Stop();
                         grdBanCo.Children.Add(coAo1);
                         grdBanCo.Children.Add(coAo2);
                         currPlayer = DoiNgich(p);
				         OnReplay();
                 }
             };
             timer.Start();
         }
         public void ReDraw()
        {
            Player Memory = new Player();
            Memory = currPlayer;

	        grdBanCo.Children.Clear();
            DrawGomokuBoard();
            grdBanCo.Children.Add(hv);
	        for(int i=0;i<row;i++)
	        {
		        for(int j=0;j<column;j++)
		        {
                    if (board[i, j] == Player.Human)
                    {
                        currPlayer = Player.Human;
                        DrawDataBoard(i, j, false,false);
                    }
                    if (board[i, j] == Player.Com)
                    {
                        currPlayer = Player.Com;
                        DrawDataBoard(i, j, false,false);
                    }
		        }
	        }
            currPlayer = Memory;
	        grdBanCo.Children.Add(coAo1);
            grdBanCo.Children.Add(coAo2);
        }
         private void DrawDataBoard(int rw, int cl,bool record,bool type)
         {
             if (type == true)
             {
                 if (currPlayer == Player.Human)
                 {
                     UserControl chess;
                     if (Option.KindOfChess == ChessStyle.Chess1)
                         chess = new ChessO_1();
                     else
                     {
                         if (Option.KindOfChess == ChessStyle.Chess2)
                             chess = new ChessO_2();
                         else
                             chess = new ChessO_3();
                     }
                     chess.Height = length;
                     chess.Width = length;
                     chess.HorizontalAlignment = 0;
                     chess.VerticalAlignment = 0;
                     chess.Margin = new Thickness(cl * length, rw * length, 0, 0);
                     grdBanCo.Children.Add(chess);
                     //Ghi lại cờ vừa đánh
                     hv.Opacity = 100;
                     hv.Margin = new Thickness(cl * length - 10, rw * length - 10, 0, 0);
                 }
                 else if (currPlayer == Player.Com)
                 {
                     UserControl chess;
                     if (Option.KindOfChess == ChessStyle.Chess1)
                         chess = new ChessX_1();
                     else
                     {
                         if (Option.KindOfChess == ChessStyle.Chess2)
                             chess = new ChessX_2();
                         else
                             chess = new ChessX_3();
                     }
                     chess.Height = length;
                     chess.Width = length;
                     chess.HorizontalAlignment = 0;
                     chess.VerticalAlignment = 0;
                     chess.Margin = new Thickness(cl * length, rw * length, 0, 0);
                     grdBanCo.Children.Add(chess);
                     //
                     hv.Opacity = 100;
                     hv.Margin = new Thickness(cl * length - 10, rw * length - 10, 0, 0);
                 }
                 if (record == true)
                     video.Add(new Node(rw, cl), currPlayer);
             }
             else
             {
                 Image Chess1 = new Image();
                 if (currPlayer == Player.Human)
                 {
                     if (Option.KindOfChess == ChessStyle.Chess1)
                     { Chess1.Source = new BitmapImage(new Uri("pack://application:,,,/Image/Chess/Chess_0_1.png")); }
                     else
                     {
                         if (Option.KindOfChess == ChessStyle.Chess2)
                         { Chess1.Source = new BitmapImage(new Uri("pack://application:,,,/Image/Chess/Chess_0_2.png")); }
                         else
                         { Chess1.Source = new BitmapImage(new Uri("pack://application:,,,/Image/Chess/Chess_0_3.png")); }
                     }
                     Chess1.Width = Chess1.Height = length;
                     Chess1.HorizontalAlignment = 0;
                     Chess1.VerticalAlignment = 0;
                     Chess1.Margin = new Thickness(cl * length, rw * length, 0, 0);
                     Chess1.Opacity = 100;
                     grdBanCo.Children.Add(Chess1);
                 }
                 else if (currPlayer == Player.Com)
                 {
                     Image Chess2 = new Image();
                     if (Option.KindOfChess == ChessStyle.Chess1)
                     { Chess2.Source = new BitmapImage(new Uri("pack://application:,,,/Image/Chess/Chess_X_1.png")); }
                     else
                     {
                         if (Option.KindOfChess == ChessStyle.Chess2)
                         { Chess2.Source = new BitmapImage(new Uri("pack://application:,,,/Image/Chess/Chess_X_2.png")); }
                         else
                         { Chess2.Source = new BitmapImage(new Uri("pack://application:,,,/Image/Chess/Chess_X_3.png")); }
                     }
                     Chess2.Width = Chess2.Height = length;
                     Chess2.HorizontalAlignment = 0;
                     Chess2.VerticalAlignment = 0;
                     Chess2.Margin = new Thickness(cl * length, rw * length, 0, 0);
                     Chess2.Opacity = 100;
                     grdBanCo.Children.Add(Chess2);
                 }
             }
         }
         //Hàm vẽ bàn cờ
         public void DrawGomokuBoard()
         {
             for (int i = 0; i < row+1; i++)
             {
                 Line line = new Line();

                 line.Stroke = Brushes.Black;
                 line.X1 = 0;
                 line.Y1 = i * length;
                 line.X2 = length * row;
                 line.Y2 = i * length;
                 line.HorizontalAlignment = HorizontalAlignment.Left;
                 line.VerticalAlignment = VerticalAlignment.Top;
                 grdBanCo.Children.Add(line);
             }
             for (int i = 0; i < column+1; i++)
             {
                 Line line = new Line();
                 line.Stroke = Brushes.Black;
                 line.X1 = i * length;
                 line.Y1 = 0;
                 line.X2 = i * length;
                 line.Y2 = length * column;
                 line.HorizontalAlignment = HorizontalAlignment.Left;
                 line.VerticalAlignment = VerticalAlignment.Top;
                 grdBanCo.Children.Add(line);
             }

         }
    }
}
