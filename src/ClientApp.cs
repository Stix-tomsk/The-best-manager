using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Text.Json;
using System.Security.Cryptography;
using System.IO;

namespace tbwClient
{
    public partial class MainForm : Form
    {
        public PublicJson? data { get; set; }
        public int userId { get; set; }
        public Button enableFilter { get; set; }
        public string url { get; set; }
        public string accessToken { get; set; }
        public MainForm()
        {
            InitializeComponent();
            url = new StreamReader("url.config").ReadLine();
            AuthPanel.Location = new Point(235, 175);
            MainInfoPanel.Location = new Point(83, 101);
            profilePanel.Location = new Point(60, 87);
            loadingGIF.Location = new Point(240, 160);
            achievesPanel.Location = new Point(99, 140);
            errorLabel.Location = new Point(118, 146);


            string[] response = SendRequest(url + "autoAuth").Split(',');
            userId = Convert.ToInt32(response[0]);
            if (userId == -2) errorLabel.Visible = true;
            else if (userId == -1) AuthPanel.Visible = true;
            else {
                InitializeApp(response);
            }
            enableFilter = byDayFilter;
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            if (data != null)
            {
                string response = SendRequest(url + "end_session/t=" + accessToken);
            }
            this.Close();
        }

        private void hideButton_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void restoreButton_Click(object sender, EventArgs e)
        {
            data = GetData();
            SetMainWindowData();
            if (profilePanel.Visible == true) SetProfileWindowData();
        }

        private void hideButton_MouseEnter(object sender, EventArgs e)
        {
            hideButton.Image = iconsList.Images[4];
        }

        private void hideButton_MouseLeave(object sender, EventArgs e)
        {
            hideButton.Image = iconsList.Images[1];
        }

        private void exitButton_MouseEnter(object sender, EventArgs e)
        {
            exitButton.Image = iconsList.Images[3];
        }

        private void exitButton_MouseLeave(object sender, EventArgs e)
        {
            try
            {
                exitButton.Image = iconsList.Images[0];
            } catch (ArgumentOutOfRangeException er)
            {
                return;
            }
        }

        private void restoreButton_MouseEnter(object sender, EventArgs e)
        {
            restoreButton.Image = iconsList.Images[5];
        }

        private void restoreButton_MouseLeave(object sender, EventArgs e)
        {
            restoreButton.Image = iconsList.Images[2];
        }

        private bool isMousePress = false;
        private Point _clickPoint;
        private Point _formStartPoint;

        private void mainForm_MouseDown(object sender, MouseEventArgs e)
        {
            isMousePress = true;
            _clickPoint = Cursor.Position;
            _formStartPoint = Location;
        }

        private void mainForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMousePress)
            {
                var cursorOffsetPoint = new Point(
                    Cursor.Position.X - _clickPoint.X,
                    Cursor.Position.Y - _clickPoint.Y);

                Location = new Point(
                    _formStartPoint.X + cursorOffsetPoint.X,
                    _formStartPoint.Y + cursorOffsetPoint.Y);
            }
        }

        private void mainForm_MouseUp(object sender, MouseEventArgs e)
        {
            isMousePress = false;
            _clickPoint = Point.Empty;
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {

            if (EmailtextBox.Text == "") {
                authErrorLabel.Visible = true;
                authErrorLabel.Text = "Please fill the email field";
                return;
            }
            if (PasTextBox.Text == "")
            {
                authErrorLabel.Visible = true;
                authErrorLabel.Text = "Please fill the password field";
                return;
            }
            
            string passDigest = String.Empty;
            byte[] crypto = new SHA256Managed().ComputeHash(Encoding.ASCII.GetBytes(PasTextBox.Text));
            foreach (byte theByte in crypto)
            {
                passDigest += theByte.ToString("x2");
            }

            string[] response = SendRequest(url + "auth/e=" + EmailtextBox.Text + "&p=" + passDigest).Split(',');
            userId = Convert.ToInt32(response[0]);

            switch (userId)
            {
                case -2: // Unable to connect to remote server
                    errorLabel.Visible = true;
                    AuthPanel.Visible = false;
                    break;
                case -3:
                    authErrorLabel.Visible = true;
                    authErrorLabel.Text = "There is no such email";
                    break;
                case -1:
                    authErrorLabel.Visible = true;
                    authErrorLabel.Text = "Password mismatch";
                    break;
                default:
                    if (authErrorLabel.Visible == true) authErrorLabel.Visible = false;
                    AuthPanel.Visible = false;
                    InitializeApp(response);
                    break;
            }
            
        }

        private void InitializeApp(string[] response)
        {
            MainInfoPanel.Visible = true;
            mainWindowBut.Visible = true;
            profileButton.Visible = true;
            AchievesButton.Visible = true;
            accessToken = response[1];
            data = GetData();
            if (data == null)
            {
                errorLabel.Visible = true;
                errorLabel.Text += Environment.NewLine + "Data is null";
                return;
            }
            string name = SetMainWindowData();
            for (int i = 0; i < data.Names.Length; i++)
                if (data.Names[i] == name) { userId = i; return; }
        }

        private PublicJson? GetData()
        {
            var response = SendRequest(url + "get_data/t=" + accessToken);

            if (response == "Denied.") return null;

            try
            {
                return JsonSerializer.Deserialize<PublicJson>(response)!;
            } catch (System.Text.Json.JsonException e)
            {
                errorLabel.Text += " " + e.Message;
                return null;
            }
        }

        private string SetMainWindowData()
        {
            string userIdName = data.Names[userId];
            int d = data.Names.Length / 2;
            while (d >= 1)
            {

                for (int i = d; i < data.Names.Length; i++)
                {
                    int j = i;
                    while ((j >= d) && (data.Score[j - d] > data.Score[j]))
                    {
                        int tmpScore = data.Score[j];
                        data.Score[j] = data.Score[j - d];
                        data.Score[j - d] = tmpScore;
                        string tmpName = data.Names[j];
                        data.Names[j] = data.Names[j-d];
                        data.Names[j - d] = tmpName;
                        j -= d;
                    }
                }
                d /= 2;
            }
            
            firstPlaceText.Text = data.Names[3];
            secondPlaceText.Text = data.Names[2];
            thirdPlaceText.Text = data.Names[1];
            fourthPlaceText.Text = data.Names[0];

            firstPlaceScoreText.Text = data.Score[3].ToString();
            secondPlaceScoreText.Text = data.Score[2].ToString();
            thirdPlaceScoreText.Text = data.Score[1].ToString();
            fourthPlaceScoreText.Text = data.Score[0].ToString();

            return userIdName;
        }

        private void SetProfileWindowData()
        {
           
            string[] users = { "User1", "User2", "User3", "User4" };
            profileNameText.Text = "Name: " + data.Names[userId].ToString();
            profileScoreText.Text = "Score: " + data.Score[userId].ToString();
            switch (data.Names[userId])
            {
                case "Name 1":
                    profileAvatar.Image = avatarsList.Images[0];
                    break;
                case "Name 2":
                    profileAvatar.Image = avatarsList.Images[1];
                    break;
                case "Name 3":
                    profileAvatar.Image = avatarsList.Images[2];
                    break;
                case "Name 4":
                    profileAvatar.Image = avatarsList.Images[3];
                    break;
            }
            SetAchievements();
            SetProgressData();
        }

        private void SetAchievements()
        {
            int achievements = 0;
            switch (data.Names[userId])
            {
                case "Name 1":
                    achievements = data.User1.achievements;
                    break;
                case "Name 2":
                    achievements = data.User2.achievements;
                    break;
                case "Name 3":
                    achievements = data.User3.achievements;
                    break;
                case "Name 4":
                    achievements = data.User4.achievements;
                    break;
            }

            char[] achList = Convert.ToString(achievements, 2).ToCharArray();
            PictureBox[] icons = { zeroAchIcon, firstAchIcon, secondAchIcon, thirdAchIcon, fourthAchIcon, fifthAchIcon, sixthAchIcon, seventhAchIcon };
            for (int i = achList.Length-1; i > -1; i--) //1110
            {
                if (achList[i] == '1') icons[achList.Length-1-i].Image = achievesList.Images[achList.Length-i + 7];
                else icons[achList.Length-1-i].Image = achievesList.Images[achList.Length-1-i];
            }    
        }

        private void SetProgressData()
        {
            int[] score = null;
            switch (data.Names[userId])
            {
                case "Name 1":
                    score = data.User1.score;
                    break;
                case "Name 2":
                    score = data.User2.score;
                    break;
                case "Name 3":
                    score = data.User3.score;
                    break;
                case "Name 4":
                    score = data.User4.score;
                    break;
            }

            if (enableFilter == byDayFilter)
            {
                if (score.Length > 7) ProgressDataText.ScrollBars = ScrollBars.Vertical;
                else ProgressDataText.ScrollBars = ScrollBars.None;
                for (int i = 0; i < score.Length; i++)
                {
                    string postfix = score[i].ToString() + Environment.NewLine;
                    if (i == 0) { ProgressDataText.Text = "1st: + " + postfix; continue; }
                    if (i == 1) { ProgressDataText.Text += "2nd: + " + postfix; continue; }
                    if (i == 2) { ProgressDataText.Text += "3rd: + " + postfix; continue; }
                    ProgressDataText.Text += (i + 1).ToString() + "th: + " + postfix;
                }
            }
            else if (enableFilter == byWeekFilter)
            {
                int amountOfWeeks = score.Length / 5;
                if (amountOfWeeks > 6) ProgressDataText.ScrollBars = ScrollBars.Vertical;
                else ProgressDataText.ScrollBars = ScrollBars.None;
                for (int i = 0; i < amountOfWeeks; i++)
                {
                    int weekData = 0;
                    for (int j = 0; j < 5; j++)
                        weekData += score[i * 5 + j];
                    //weekData /= 5;
                    string postfix = weekData.ToString() + Environment.NewLine;
                    if (i == 0) { ProgressDataText.Text = "1st: " + postfix; continue; }
                    if (i == 1) { ProgressDataText.Text += "2nd: " + postfix; continue; }
                    if (i == 2) { ProgressDataText.Text += "3rd: " + postfix; continue; }
                    ProgressDataText.Text += (i + 1).ToString() + "th: " + postfix;
                }
                if (score.Length % 5 != 0)
                {
                    int weekData = 0;
                    for (int i = 0; i < score.Length % 5; i++)
                        weekData += score[(amountOfWeeks * 5) + i];
                    //weekData /= score.Length % 5;
                    string postfix = weekData.ToString() + Environment.NewLine;
                    if (amountOfWeeks == 0) ProgressDataText.Text = "1st: " + postfix; 
                    else if (amountOfWeeks == 1) ProgressDataText.Text += "2nd: " + postfix;
                    else if (amountOfWeeks == 2) ProgressDataText.Text += "3rd: " + postfix;
                    else ProgressDataText.Text += (amountOfWeeks + 1).ToString() + "th: " + postfix;
                }
            }
            else if (enableFilter == byMonthFilter)
            {
                int amountOfMonths = score.Length / 20;
                if (amountOfMonths > 6) ProgressDataText.ScrollBars = ScrollBars.Vertical;
                else ProgressDataText.ScrollBars = ScrollBars.None;
                for (int i = 0; i < amountOfMonths; i++)
                {
                    int monthData = 0;
                    for (int j = 0; j < 20; j++)
                        monthData += score[i * 20 + j];
                    //monthData /= 20;
                    string postfix = monthData.ToString() + Environment.NewLine;
                    if (i == 0) { ProgressDataText.Text = "1st: " + postfix; continue; }
                    if (i == 1) { ProgressDataText.Text += "2nd: " + postfix; continue; }
                    if (i == 2) { ProgressDataText.Text += "3rd: " + postfix; continue; }
                    ProgressDataText.Text += (i + 1).ToString() + "th: " + postfix;
                }
                if (score.Length % 20 != 0)
                {
                    int monthData = 0;
                    for (int i = 0; i < score.Length % 20; i++)
                        monthData += score[(amountOfMonths * 20) + i];
                    //monthData /= score.Length % 20;
                    string postfix = monthData.ToString() + Environment.NewLine;
                    if (amountOfMonths == 0) ProgressDataText.Text = "1st: " + postfix;
                    else if (amountOfMonths == 1) ProgressDataText.Text += "2nd: " + postfix;
                    else if (amountOfMonths == 2) ProgressDataText.Text += "3rd: " + postfix;
                    else ProgressDataText.Text += (amountOfMonths + 1).ToString() + "th: " + postfix;
                }
            }
            else if (enableFilter == byQuarterFilter)
            {
                int amountOfQuarters = score.Length / 20;
                if (amountOfQuarters > 6) ProgressDataText.ScrollBars = ScrollBars.Vertical;
                else ProgressDataText.ScrollBars = ScrollBars.None;
                for (int i = 0; i < amountOfQuarters; i++)
                {
                    int quarterData = 0;
                    for (int j = 0; j < 20; j++)
                        quarterData += score[i * 20 + j];
                    //quarterData /= 20;
                    string postfix = quarterData.ToString() + Environment.NewLine;
                    if (i == 0) { ProgressDataText.Text = "1st: " + postfix; continue; }
                    if (i == 1) { ProgressDataText.Text += "2nd: " + postfix; continue; }
                    if (i == 2) { ProgressDataText.Text += "3rd: " + postfix; continue; }
                    ProgressDataText.Text += (i + 1).ToString() + "th: " + postfix;
                }
                if (score.Length % 20 != 0)
                {
                    int quarterData = 0;
                    for (int i = 0; i < score.Length % 20; i++)
                        quarterData += score[(amountOfQuarters * 20) + i];
                    //quarterData /= score.Length % 20;
                    string postfix = quarterData.ToString() + Environment.NewLine;
                    if (amountOfQuarters == 0) ProgressDataText.Text = "1st: " + postfix;
                    else if (amountOfQuarters == 1) ProgressDataText.Text += "2nd: " + postfix;
                    else if (amountOfQuarters == 2) ProgressDataText.Text += "3rd: " + postfix;
                    else ProgressDataText.Text += (amountOfQuarters + 1).ToString() + "th: " + postfix;
                }
            }
        }

        private void mainWindowBut_Click(object sender, EventArgs e)
        {
            if (MainInfoPanel.Visible == true) return;
            if (profilePanel.Visible == true)
                profilePanel.Visible = false;
            else if (achievesPanel.Visible == true)
                achievesPanel.Visible = false;
            MainInfoPanel.Visible = true;

            SetMainWindowData();
        }

        private void mainWindowBut_MouseLeave(object sender, EventArgs e)
        {
            mainWindowBut.Image = menuIconList.Images[0];
        }

        private void mainWindowBut_MouseEnter(object sender, EventArgs e)
        {
            mainWindowBut.Image = menuIconList.Images[3];
        }

        private void profileButton_Click(object sender, EventArgs e)
        {
            if (profilePanel.Visible == true) return;
            if (MainInfoPanel.Visible == true)
                MainInfoPanel.Visible = false;
            else if (achievesPanel.Visible == true)
                achievesPanel.Visible = false;
            profilePanel.Visible = true;

            SetProfileWindowData();
        }

        // -----------

        private void profileButton_MouseEnter(object sender, EventArgs e)
        {
            profileButton.Image = menuIconList.Images[4];
        }

        private void profileButton_MouseLeave(object sender, EventArgs e)
        {
            profileButton.Image = menuIconList.Images[1];
        }

        private void AchievesButton_Click(object sender, EventArgs e)
        {
            if (achievesPanel.Visible == true) return;
            if (MainInfoPanel.Visible == true)
                MainInfoPanel.Visible = false;
            else if (profilePanel.Visible == true)
                profilePanel.Visible = false;
            achievesPanel.Visible = true;
        }

        private void AchievesButton_MouseEnter(object sender, EventArgs e)
        {
            AchievesButton.Image = menuIconList.Images[5];
        }

        private void AchievesButton_MouseLeave(object sender, EventArgs e)
        {
            AchievesButton.Image = menuIconList.Images[2];
        }

        
        // -----------
        
        private void byWeekFilter_Click(object sender, EventArgs e)
        {
            ChangeFilter(byWeekFilter);
            SetProgressData();
            ProgressDataText.Location = new Point(371, 278);
        }

        private void byDayFilter_Click(object sender, EventArgs e)
        {
            ChangeFilter(byDayFilter);
            SetProgressData();
            ProgressDataText.Location = new Point(278, 278);
        }

        private void byMonthFilter_Click(object sender, EventArgs e)
        {
            ChangeFilter(byMonthFilter);
            SetProgressData();
            ProgressDataText.Location = new Point(464, 278);
        }

        private void byQuarterFilter_Click(object sender, EventArgs e)
        {
            ChangeFilter(byQuarterFilter);
            SetProgressData();
            ProgressDataText.Location = new Point(557, 278);
        }

        private void ChangeFilter(Button but)
        {
            if (enableFilter == but) return;
            but.ForeColor = Color.White;
            but.BackColor = Color.FromArgb(0, 124, 255);
            but.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 124, 255);
            but.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 124, 255);

            enableFilter.ForeColor = Color.Black;
            enableFilter.BackColor = Color.Transparent;
            enableFilter.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 214, 255);
            enableFilter.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 214, 255);
            enableFilter = but;
        }

        // -----------

        private string SendRequest(string strUrl)
        {
            var response = "";
            using (var webClient = new WebClient())
            {
                try
                {
                    response = webClient.DownloadString(strUrl);
                }
                catch (System.Net.WebException e)
                {
                    errorLabel.Text += " " + "Unable to connect to remote server";
                    return null;
                }
            }
            return response;
        }


    }

    public class PublicJson
    {
        public string[]? Names { get; set; }
        public int[] Score { get; set; }
        public User User1 { get; set; }
        public User User2 { get; set; }
        public User User3 { get; set; }
        public User User4 { get; set; }
    }

    public class User
    {
        public int avatarId { get; set; }
        public int achievements { get; set; }
        public int[] score { get; set; }
        public string[]? ip { get; set; }
    }
}
