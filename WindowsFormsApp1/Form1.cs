using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMPLib;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        PictureBox[] stars;
        int backgroundspeed;
        Random rnd;
        int playerspeed = 5;
        int MunitionSpeed = 20;
        PictureBox[] munitions;
        Image munition;
        WindowsMediaPlayer gamemedia;
        WindowsMediaPlayer shootmedia;
        WindowsMediaPlayer explosion;

        PictureBox[] enemies;
        int enemiesspeed;

        //khai báo một số biến cho game
        int score;
        int level;
        int difficulty;
        bool pause;
        bool gameisover;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //đầu game sẽ không pause và gameisover
            pause = false;
            gameisover = false;
            //Level và điểm đầu trò chơi:
            level = 1;
            score = 0;
            

            enemiesspeed = 4;
            // load image

            Image apple = Properties.Resources.apple;
            Image bag = Properties.Resources.bag;
            Image can = Properties.Resources.can;
            enemies = new PictureBox[15];
            for (int i = 0; i < enemies.Length; i++)
            {
                // Khởi tạo PictureBox
                enemies[i] = new PictureBox();

                // Đặt kích thước cho PictureBox
                enemies[i].Size = new Size(40, 40);

                // Chế độ hiển thị hình ảnh trong PictureBox
                enemies[i].SizeMode = PictureBoxSizeMode.Zoom;

                // Loại bỏ đường viền của PictureBox
                enemies[i].BorderStyle = BorderStyle.None;

                // Đặt thuộc tính Visible của PictureBox thành false, nghĩa là các đối tượng PictureBox không hiển thị ban đầu
                enemies[i].Visible = false;

                // Thêm PictureBox vào form
                this.Controls.Add(enemies[i]);

                // Đặt vị trí của PictureBox trên form
                enemies[i].Location = new Point((i + 1) * 50, -50);
            }
            enemies[0].Image = apple;
            enemies[1].Image = bag;
            enemies[2].Image = can;
            try
            {
                munition = Image.FromFile(@"C:\assert\munition.png"); // Tải ảnh đạn
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải hình ảnh đạn: " + ex.Message);
                return;
            }
            
            munitions = new PictureBox[3];
            for (int i = 0; i < munitions.Length; i++)
            {
                munitions[i] = new PictureBox();
                munitions[i].Size = new Size(8, 8);
                munitions[i].Image = munition;
                munitions[i].SizeMode = PictureBoxSizeMode.Zoom;
                munitions[i].BorderStyle = BorderStyle.None;
                this.Controls.Add(munitions[i]);
            }
            this.Controls.Add(Player);
            backgroundspeed = 4;
            stars = new PictureBox[10];
            rnd = new Random();
            for (int i = 0; i < stars.Length; i++)
            {
                stars[i] = new PictureBox();
                stars[i].BorderStyle = BorderStyle.None;
                stars[i].Location = new Point(rnd.Next(20, 580), rnd.Next(-10, 400));

                if (i % 2 == 1)
                {
                    stars[i].Size = new Size(2, 2);
                    stars[i].BackColor = Color.Wheat;
                }
                else
                {
                    stars[i].Size = new Size(3, 3);
                    stars[i].BackColor = Color.DarkGray;
                }

                this.Controls.Add(stars[i]);
            }
            gamemedia = new WindowsMediaPlayer();
            shootmedia = new WindowsMediaPlayer();
            explosion = new WindowsMediaPlayer();
            ///Load all song
            shootmedia.URL = "C:\\assert\\shoot.mp3";
            shootmedia.settings.setMode("loop", true);
            gamemedia.URL = "C:\\assert\\GameSong.mp3";
            gamemedia.controls.play();
        }
 

        /// <summary>
        /// Background moving
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < stars.Length / 2; i++)
            {
                stars[i].Top += backgroundspeed;

                if (stars[i].Top >= this.Height)
                {
                    stars[i].Top = -stars[i].Height;
                }
            }
            for (int i = stars.Length / 2; i < stars.Length; i++)
            {
                stars[i].Top += backgroundspeed - 2;

                if (stars[i].Top >= this.Height)
                {
                    stars[i].Top = -stars[i].Height;
                }
                this.Controls.Add(stars[i]);
            }
            
        }

        private void LeftMoveTimer_Tick(object sender, EventArgs e)
        {
            if (Player.Left > 10)
            {
                Player.Left -= playerspeed;
            }
        }
        private void RightMoveTimer_Tick(object sender, EventArgs e)
        {
            if (Player.Right < 580)
            {
                Player.Left += playerspeed;
            }
        }

        // Hàm di chuyển xuống dưới
        private void DownMoveTimer_Tick(object sender, EventArgs e)
        {
            if (Player.Top < 400)
            {
                Player.Top += playerspeed;
            }
        }

        // Hàm di chuyển lên trên
        private void UpMoveTimer_Tick(object sender, EventArgs e)
        {
            if (Player.Top > 10)
            {
                Player.Top -= playerspeed;
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!pause)
            {

                if (e.KeyCode == Keys.Right)
                {
                    RightMoveTimer.Start();
                }
                if (e.KeyCode == Keys.Left)
                {
                    LeftMoveTimer.Start();
                }
                if (e.KeyCode == Keys.Down)
                {
                    DownMoveTimer.Start();
                }
                if (e.KeyCode == Keys.Up)
                {
                    UpMoveTimer.Start();
                }
            }    

        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            RightMoveTimer.Stop();
            LeftMoveTimer.Stop();
            DownMoveTimer.Stop();
            UpMoveTimer.Stop();
            if (e.KeyCode == Keys.Space) // Kiểm tra phím Space
            {
                if (!gameisover) // Kiểm tra xem trò chơi đã kết thúc chưa
                {
                    if (pause) // Nếu đang tạm dừng
                    {
                        StartTimers(); // Tiếp tục các bộ đếm thời gian
                        label.Visible = false; // Ẩn nhãn thông báo
                        gamemedia.controls.play(); // Tiếp tục phát nhạc
                        pause = false; // Chuyển trạng thái thành không tạm dừng
                    }
                    else // Nếu đang chạy
                    {
                        // Đặt vị trí nhãn "PAUSED" ra giữa màn hình
                        label.Location = new Point(this.Width / 2 - 120, 150);
                        label.Text = "PAUSED"; // Hiển thị chữ "PAUSED"
                        label.Visible = true; // Hiển thị nhãn
                        gamemedia.controls.pause(); // Tạm dừng nhạc nền
                        StopTimers(); // Dừng các bộ đếm thời gian
                        pause = true; // Chuyển trạng thái thành tạm dừng
                    }
                }
            }
        }

        private void Player_Click(object sender, EventArgs e)
        {

        }

        private void timer2_Tick(object sender, EventArgs e)
        {

        }

        private void MoveMunitionTimer_Tick(object sender, EventArgs e)
        {
            shootmedia.controls.play();
            Collision();
            for (int i = 0; i < munitions.Length; i++)
            {
                if (munitions[i].Top > 0)
                {
                    munitions[i].Visible = true;
                    munitions[i].Top -= MunitionSpeed;
                }
                else
                {
                    munitions[i].Visible = false;
                    munitions[i].Location = new Point(Player.Location.X + 20, Player.Location.Y - i * 30);
                }
            }

        }
        /// <summary>
        /// Di chuyển các đối tượng PictureBox (kẻ địch) xuống dưới màn hình.
        /// Nếu kẻ địch vượt quá màn hình, vị trí sẽ được đặt lại về phía trên.
        /// </summary>
        /// <param name="array">Mảng chứa các PictureBox đại diện cho kẻ địch.</param>
        /// <param name="speed">Tốc độ di chuyển của kẻ địch.</param>
        private void MoveEnemies(PictureBox[] array, int speed)
        {
            // Duyệt qua từng kẻ địch trong mảng
            for (int i = 0; i < array.Length; i++)
            {
                // Hiển thị kẻ địch (nếu đang bị ẩn)
                array[i].Visible = true;

                // Di chuyển kẻ địch xuống dưới màn hình theo tốc độ
                array[i].Top += speed;

                // Kiểm tra nếu kẻ địch đã vượt qua chiều cao của cửa sổ
                if (array[i].Top > this.Height)
                {
                    // Đặt lại vị trí kẻ địch về phía trên màn hình
                    // Vị trí X cách nhau 50 pixels và Y là -200 (phía trên cửa sổ)
                    array[i].Location = new Point((i + 1) * 50, -200);
                }
            }
        }

        private void MoveEnemiesTimer_Tick(object sender, EventArgs e)
        {
            MoveEnemies(enemies, enemiesspeed);
        }
        private void Collision()
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                if (munitions[0].Bounds.IntersectsWith(enemies[i].Bounds)
                    || munitions[1].Bounds.IntersectsWith(enemies[i].Bounds)
                    || munitions[2].Bounds.IntersectsWith(enemies[i].Bounds))
                {
                    score += 1;
                    scorelabel.Text =  $"score: {score}";
                    explosion.controls.play();
                    enemies[i].Location = new Point((i + 1) * 50, -100);
                }

                if (Player.Bounds.IntersectsWith(enemies[i].Bounds))
                {
                    explosion.settings.volume = 30;
                    explosion.controls.play();
                    Player.Visible = false;
                    GameOver("Game OVer");
                }
            }
        }
        private void GameOver(String str)
        {
            gamemedia.controls.stop();
            shootmedia.controls.stop();
            StopTimers(); // Dừng toàn bộ các Timer
            EXIT.Visible = true;
            REPLAY.Visible = true;
            
        }

        // Stop Timers
        private void StopTimers()
        {
            timer1.Stop();
            MoveEnemiesTimer.Stop();
            MoveMunitionTimer.Stop();
        }

        // Start Timers
        private void StartTimers()
        {
            timer1.Start();
            MoveEnemiesTimer.Start();
            MoveMunitionTimer.Start();
        }

        private void REPLAY_Click(object sender, EventArgs e)
        {
            this.Controls.Clear();
            InitializeComponent();
            Application.Restart();
        }

        private void EXIT_Click(object sender, EventArgs e)
        {
            Environment.Exit(1);
        }
    }
}
