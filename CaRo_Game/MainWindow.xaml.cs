using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using System.IO;
using System.Windows.Resources;

namespace CaRo_Game
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		enum ControlMute
		{
			Muting=0,
			Playing=1,
		}
		enum ControlPlay
		{
			Pause=0,
			Play=1,
		}
		public MainWindow()
		{
            this.InitializeComponent();
            timer1 = new DispatcherTimer();
            timer1.Interval = TimeSpan.FromMilliseconds(10);
            timer2 = new DispatcherTimer();
            timer2.Interval = TimeSpan.FromMilliseconds(10);
            totalTimer = new DispatcherTimer();
            totalTimer.Interval = TimeSpan.FromSeconds(1);
			totalTimer.Tick+=new System.EventHandler(totalTimer_Tick);
			banco=new clsBanCo(this,grdBanCo);
			banco.DrawGomokuBoard();
			banco.Option.Music="1";
			banco.Option.KindOfChess=ChessStyle.Chess1;
            grdBanCo.MouseDown += new System.Windows.Input.MouseButtonEventHandler(banco.grdBanCo_MouseDown);
            banco.HumanDanhXongEvent += new clsBanCo.HumanDanhXongEventHandler(banco_HumanDanhXongEvent);
            banco.ComDanhXongEvent += new clsBanCo.ComDanhXongEventHandler(banco_ComDanhXongEvent);
            banco.WinEvent += new clsBanCo.WinEventHander(banco_WinEvent);
            banco.LoseEvent += new clsBanCo.LoseEventHander(banco_LoseEvent);
			banco.ReplayEvent+=	new clsBanCo.ReplayEventHander(ControlReplay);
			// Insert code required on object creation below this point.
		}
		private void ControlReplay()
		{
			Storyboard storyboard = (Storyboard)FindResource("Replay_Finish");
			storyboard.Begin();
		}
        private void banco_ComDanhXongEvent()
        {
            GetThongTin();
            XulyThoiGian(pbTimeA, timer1);
            timer2.Stop();
        }

        private void banco_HumanDanhXongEvent()
        {
            GetThongTin();
            XulyThoiGian(pbTimeB, timer2);
            timer1.Stop();
        }

        private void banco_LoseEvent()
        {
            banco.Option.PlayerBScore++;
            timer1.Stop();
            timer2.Stop();
			PauseMusic();
            Storyboard a = (Storyboard)FindResource("Lose");
            a.Begin();
			AddText("- Computer: chơi gà vậy !");
			AddText("- "+txtMainNamePlayerA.Text+" cố lên !!!");
			Lose_CrySound.Play();
            GetThongTin();
        }

        private void banco_WinEvent()
        {
			timer1.Stop();
            timer2.Stop();
            if (banco.End == Player.Human)
            {
                txtWinPlayer.Text = banco.Option.PlayerAName;
				AddText("- "+txtWinPlayer.Text+" chơi hay lắm ^_^ !!!");
                banco.Option.PlayerAScore++;
            }
            else if (banco.End == Player.Com)
            {
                txtWinPlayer.Text = banco.Option.PlayerBName;
				AddText("- "+txtWinPlayer.Text+" chơi hay lắm ^_^!!!");
                banco.Option.PlayerBScore++;
            }
			PauseMusic();
            Storyboard a = (Storyboard)FindResource("Win");
            a.Begin();
			Win_LaughSound.Play();
            GetThongTin();
        }
		ControlPlay controlplay = ControlPlay.Play;
		ControlMute controlsound = ControlMute.Playing;
		clsBanCo banco;
		bool ControlSave = false;


		private void Window_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{

			// TODO: Add event handler implementation here.
			//banco.PlayAgain();
		}

		private void btnOptionOK_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Click_sound.Play();
            //banco.Option.PlayerAScore = banco.Option.PlayerBScore = 0;
			if(rdMusic1.IsChecked==true) 
			{	
				banco.Option.Music = "1";
			}
			else if(rdMusic2.IsChecked==true) 
			{
				banco.Option.Music = "2";
			}
			else if(rdMusic3.IsChecked==true) 
			{
				banco.Option.Music = "3";
			}
            if (rdChess1.IsChecked == true) banco.Option.KindOfChess = ChessStyle.Chess1;
            else if (rdChess2.IsChecked == true) banco.Option.KindOfChess = ChessStyle.Chess2;
            else if (rdChess3.IsChecked == true) banco.Option.KindOfChess = ChessStyle.Chess3;
            GetThongTin();
            banco.ReDraw();
			banco.CreateCoAo();
			StopMusic();
		}
		private void btnPlayWithHuman_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Click_sound.Play();
            banco.Option.WhoPlayWith = Player.Human;
		}

		private void btnPlayWithComputer_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Click_sound.Play();
            banco.Option.WhoPlayWith = Player.Com;
		}


        private void GetThongTin()
        {
            txtMainNamePlayerA.Text = banco.Option.PlayerAName;
            txtMainNamePlayerB.Text = banco.Option.PlayerBName;
			txtPlayerAScore.Text = banco.Option.PlayerAScore.ToString();
			txtPlayerBScore.Text = banco.Option.PlayerBScore.ToString();
             if(banco.currPlayer == Player.Human)
             txtMainLuotDi.Text = "Who play : " + banco.Option.PlayerAName;
             else if(banco.currPlayer == Player.Com)
                txtMainLuotDi.Text = "Who play : " + banco.Option.PlayerBName;
        }
        DispatcherTimer timer1;
        DispatcherTimer timer2;
        DispatcherTimer totalTimer;
        int temp = 0;
       private void XulyThoiGian(ProgressBar progressBar,DispatcherTimer timer)
        {
            progressBar.Value = progressBar.Maximum = banco.Option.Time * 100;
            if (temp < 2)
            {
                timer.Tick += delegate
                {
                    progressBar.Value--;
                    if (progressBar.Value <= 0)
                    {
                        timer.Stop();
                        if (banco.Option.WhoPlayWith == Player.Com)
                        {
                            banco.End = Player.Com;
                            banco.OnLose();
                            banco.currPlayer = Player.Com;
                        }
                        else if (banco.Option.WhoPlayWith == Player.Human)
                        {
                            if (banco.currPlayer == Player.Human)
                            {
                                banco.End = Player.Com;
                                banco.OnWin();
                                banco.currPlayer = Player.Com;
                                timer1.Stop();
                                timer2.Stop();
                            }
                            else if (banco.currPlayer == Player.Com)
                            {
                                banco.End = Player.Human;
                                banco.OnWin();
                                banco.currPlayer = Player.Human;
                                timer1.Stop();
                                timer2.Stop();
                            }
                        }
                        progressBar.Value = banco.Option.Time * 100;
                    }
                };
                temp++;
            }
            timer.Start();
        }

        private void btnPlayerAgain_Click(object sender, System.Windows.RoutedEventArgs e)
        {
			System.GC.Collect();
			AddText("- Ván đấu mới");
			Click_sound.Play();
        	banco.PlayAgain();
			controlplay=ControlPlay.Play;
            GetThongTin();
            if (banco.currPlayer == Player.Human)
            {
                pbTimeA.Value = banco.Option.Time * 100;
                timer1.Start();
                timer2.Stop();
            }
            else if (banco.currPlayer == Player.Com)
            {
                pbTimeB.Value = banco.Option.Time * 100;
                timer2.Start();
                timer1.Stop();
            }
			
        }
		int count =0;
		int second=0;
		int minute=0;
		int hours=0;
        private void totalTimer_Tick(object sender, System.EventArgs e)
        {
             count++;
			 second =count%60;
			minute = (count/60)%60;
			hours = (count/3600);
			txtTotalTime.Text = hours.ToString() + " : " + minute.ToString() + " : " + second.ToString();
        }

        private void btnMainMenu_Click(object sender, System.Windows.RoutedEventArgs e)
        {
			Click_sound.Play();
        	timer1.Stop();
            timer2.Stop();
			totalTimer.Stop();
			PauseMusic();
        }

        private void btnMenuResume_Click(object sender, System.Windows.RoutedEventArgs e)
        {
			Click_sound.Play();
            Menu_to_MainPlay_BeginStoryboard.Storyboard.Completed += delegate
            {
				if(controlplay==ControlPlay.Play)
				{
                if (banco.currPlayer == Player.Human)
                    timer1.Start();
                else if (banco.currPlayer == Player.Com)
                    timer2.Start();
				}
				totalTimer.Start();
				if(controlsound==ControlMute.Playing)
				PlayMusic();
            };
        }

        private void btnHvsCInformationOK_Click(object sender, System.Windows.RoutedEventArgs e)
        {
			ControlSave=true;
			Click_sound.Play();
            count = 0;
            banco.Option.PlayerAScore = banco.Option.PlayerBScore = 0;
            banco.Option.PlayerAName = txtPlayer.Text;
            banco.Option.PlayerBName = "Computer";
            if (rdLevelEasy.IsChecked == true) banco.Option.Time = 10;
            else if (rdLevelNormal.IsChecked == true) banco.Option.Time = 15;
            else if (rdLevelHard.IsChecked == true) banco.Option.Time = 20;
            if (rdInternation1.IsChecked == true) banco.Option.GamePlay = LuatChoi.International;
            if (rdVietnamese1.IsChecked == true) banco.Option.GamePlay = LuatChoi.Vietnamese;
            banco.ResetAllBoard();
            txtTotalTime.Text = "00:00:00";
            GetThongTin();
            pbTimeA.Value = pbTimeA.Maximum = banco.Option.Time * 100;
            pbTimeB.Value = pbTimeB.Maximum = banco.Option.Time * 100;
            IHC_to_MainPlay_BeginStoryboard.Storyboard.Completed += delegate
            {
                banco.NewGame();
                GetThongTin();
                totalTimer.Start();
                if (banco.currPlayer == Player.Human)
                {
                    XulyThoiGian(pbTimeA, timer1);
                }
                else if (banco.currPlayer == Player.Com)
                {
                    XulyThoiGian(pbTimeB, timer2);
                }
				PlayMusic();
                MP_Status.Items.Clear();
                if (banco.Option.GamePlay == LuatChoi.International)
                {
                    AddText("***Luật Chơi Quốc Tế***");
                }
                else AddText("***Luật Chơi Việt Nam***");
            };
        }

        private void btnHvsHInformationOK_Click(object sender, System.Windows.RoutedEventArgs e)
        {
			ControlSave=true;
			Click_sound.Play();
            count = 0;
            banco.Option.PlayerAScore = banco.Option.PlayerBScore = 0;
            banco.Option.PlayerAName = txtPlayerAName.Text;
            banco.Option.PlayerBName = txtPlayerBName.Text;
            if (rdWaitingNone.IsChecked == true) banco.Option.Time = 1000;
            else if (rdWaiting10s.IsChecked == true) banco.Option.Time = 10;
            else if (rdWaiting15s.IsChecked == true) banco.Option.Time = 15;
            else if (rdWaiting20s.IsChecked == true) banco.Option.Time = 20;
            if (rdInterantionnal.IsChecked == true) banco.Option.GamePlay = LuatChoi.International;
            if (rdVietnamese.IsChecked == true) banco.Option.GamePlay = LuatChoi.Vietnamese;
            banco.ResetAllBoard();
            txtTotalTime.Text = "00:00:00";
            GetThongTin();
            pbTimeA.Value =pbTimeA.Maximum= banco.Option.Time * 100;
            pbTimeB.Value = pbTimeB.Maximum = banco.Option.Time * 100;
            IHH_to_MainPlay_BeginStoryboard.Storyboard.Completed += delegate
            {
                banco.NewGame();
                GetThongTin();
                totalTimer.Start();
                if (banco.currPlayer == Player.Human)
                {
                    XulyThoiGian(pbTimeA, timer1);
                }
                else if (banco.currPlayer == Player.Com)
                {
                    XulyThoiGian(pbTimeB, timer2);
                }
				PlayMusic();
                MP_Status.Items.Clear();
                if (banco.Option.GamePlay == LuatChoi.International)
                {
                    AddText("***Luật Chơi Quốc Tế***");
                }
                else AddText("***Luật Chơi Việt Nam***");
            };
        }

        private void btnMainPlayerPause_Click(object sender, System.Windows.RoutedEventArgs e)
        {
			Click_sound.Play();
            totalTimer.Stop();
            if (banco.currPlayer == Player.Human)
            {
                timer1.Stop();
            }
            if (banco.currPlayer == Player.Com)
            {
                timer2.Stop();
            }
             
            Storyboard storyboard = (Storyboard)FindResource("MainPlay_Pause");
			storyboard.Begin();
			PauseMusic();
        }

        private void Replay(object sender, System.Windows.RoutedEventArgs e)
        {	
			Click_sound.Play();
        	banco.XemLai();
			if(controlsound==ControlMute.Playing)
			PlayMusic();
			controlplay=ControlPlay.Pause;
        }

        private void btnSave_Click(object sender, System.Windows.RoutedEventArgs e)
        {
				Click_sound.Play();
                banco.Save(txtSaveName.Text);

        }
        
        private void btnLoad_Click(object sender, System.Windows.RoutedEventArgs e)
        {
			Click_sound.Play();
		    banco.ResetAllBoard();
            count=0;
            try
            {
                banco.Load(txtLoadName.Text);
            }
            catch (Exception)
            {
				Storyboard storyboard = (Storyboard)FindResource("Load_to_Eror");
				storyboard.Begin();
                return;
            }
    		ControlSave=true;
		    GetThongTin();
			Storyboard a = (Storyboard)FindResource("Load_to_MainPlay");
			a.Completed+=delegate
			{
				txtTotalTime.Text = "00:00:00";
            totalTimer.Start();
            if (banco.currPlayer == Player.Human)
            {
                XulyThoiGian(pbTimeA, timer1);
            }
            else if (banco.currPlayer == Player.Com)
            {
                XulyThoiGian(pbTimeB, timer2);
            }
			    StopMusic();
                PlayMusic();
                controlsound = ControlMute.Playing;
                MP_Status.Items.Clear();
                if (banco.Option.GamePlay == LuatChoi.International)
                {
                    AddText("***Luật Chơi Quốc Tế***");
                }
                else AddText("***Luật Chơi Việt Nam***");
				
			};
			
			a.Begin();

        }

        private void btnMenuLoad_Click(object sender, System.Windows.RoutedEventArgs e)
        {
			Click_sound.Play();
            listLoad.Items.Clear();
            string[] ds = Directory.GetFiles("Save\\");
			for(int i=0;i<ds.Length;i++)
            listLoad.Items.Add(ds[i].Split(new char[]{'\\'})[1].Split(new char[]{'.'})[0]);
			txtLoadName.Text = listLoad.Items[0].ToString();
        }

        private void listLoad_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
			Click_sound.Play();
             if(listLoad.Items.Count!=0)   
        	    txtLoadName.Text = listLoad.SelectedItem.ToString();
        }
        //---------------OPTION_BACKGROUND---------------
        private void Op_Button_Autumn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
			Click_sound.Play();
		    string url="pack://application:,,,/Image/BackGround/Autumn_BG.jpg";
        	MainBackGround.Source=new BitmapImage(new Uri(url));
        }

        private void Op_Button_Green_Click(object sender, System.Windows.RoutedEventArgs e)
        {
			Click_sound.Play();
        	string url="pack://application:,,,/Image/BackGround/Green_BG.jpg";
        	MainBackGround.Source=new BitmapImage(new Uri(url));
        }

        private void Op_Button_Main_Click(object sender, System.Windows.RoutedEventArgs e)
        {
			Click_sound.Play();
        	string url="pack://application:,,,/Image/BackGround/Main_BG.jpg";
        	MainBackGround.Source=new BitmapImage(new Uri(url));
        }

        private void Op_Button_Spring_Click(object sender, System.Windows.RoutedEventArgs e)
        {
			Click_sound.Play();
        	string url="pack://application:,,,/Image/BackGround/Spring_BG.jpg";
        	MainBackGround.Source=new BitmapImage(new Uri(url));
        }
        private void PlayMusic()
		{
			if(banco.Option.Music == "1")
			{
				Music_1.Play();
			}
			if(banco.Option.Music == "2")
			{
				Music_2.Play();
			}
		    if(banco.Option.Music == "3")
			{
				Music_3.Play();
			}				
		}
		private void AddText(string Text)
		{
			MP_Status.Items.Add(Text);
		}
		private void PauseMusic()
		{
			if(banco.Option.Music == "1")
			{
				Music_1.Pause();
			}
			if(banco.Option.Music == "2")
			{
				Music_2.Pause();
			}
		    if(banco.Option.Music == "3")
			{
				Music_3.Pause();
			}				
		}
		private void StopMusic()
		{
			if(banco.Option.Music == "1")
			{
				Music_1.Stop();
			}
			if(banco.Option.Music == "2")
			{
				Music_2.Stop();
			}
		    if(banco.Option.Music == "3")
			{
				Music_3.Stop();
			}				
		}

		private void Munu_New_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			ControlSave=false;
			Click_sound.Play();
			StopMusic();
			controlsound=ControlMute.Playing;
			MP_Status.Items.Clear();
		}

		private void MP_Play_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Click_sound.Play();
			totalTimer.Start();
			if(banco.currPlayer == Player.Human && banco.End== Player.None)
			{
				timer1.Start();
			}
            if (banco.currPlayer == Player.Com && banco.End == Player.None)
			{
				timer2.Start();
			}
			if(controlsound==ControlMute.Playing)
				PlayMusic();
		}

		private void Munu_Exit_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Click_sound.Play();
			Exit_BeginStoryboard.Storyboard.Completed+=delegate
			{
				this.Close();
			};
		}

		private void btnMute_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Click_sound.Play();
			if(controlsound==ControlMute.Playing)	
			{
				PauseMusic();
				controlsound=ControlMute.Muting;
				AddText("- Tắt âm thanh");
			}
			else				
			{
				PlayMusic();
				controlsound=ControlMute.Playing;
				AddText("- Mở âm thanh");
			}
		}

		private void Win_LaughSound_MediaEnded(object sender, System.Windows.RoutedEventArgs e)
		{
			Win_LaughSound.Position=new TimeSpan(0,0,0);
			Win_LaughSound.Stop();
		}

		private void Music_1_MediaEnded(object sender, System.Windows.RoutedEventArgs e)
		{
			Music_1.Position=new TimeSpan(0,0,0);
		}

		private void Music_2_MediaEnded(object sender, System.Windows.RoutedEventArgs e)
		{
			Music_2.Position=new TimeSpan(0,0,0);
		}

		private void Music_3_MediaEnded(object sender, System.Windows.RoutedEventArgs e)
		{
			Music_3.Position=new TimeSpan(0,0,0);
		}

		private void Lose_CrySound_MediaEnded(object sender, System.Windows.RoutedEventArgs e)
		{
			Lose_CrySound.Position=new TimeSpan(0,0,0);
			Lose_CrySound.Stop();
		}

		private void btnLoseContinous_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			controlplay=ControlPlay.Pause;
			if(controlsound==ControlMute.Playing)
			PlayMusic();
		}

		private void Click_sound_MediaEnded(object sender, System.Windows.RoutedEventArgs e)
		{
			Click_sound.Position=new TimeSpan(0,0,0);
			Click_sound.Stop();
		}

		private void ClickSound(object sender, System.Windows.RoutedEventArgs e)
		{
			Click_sound.Play();
			if(ControlSave==true)
			{
				Storyboard storyboard = (Storyboard)FindResource("Menu_to_Save");
				storyboard.Begin();
			}
		}
	}
}