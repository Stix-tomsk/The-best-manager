using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.Json;
using System.Net;
using System.Security.Cryptography;

namespace AdminApp
{
    public partial class ConfigForm : Form
    {
        public PublicJson? usersData { get;set; }
        public string url { get;set; }

        /* daily norms:
         * this database architecture includes 3 responsibilities, 
         * so the array is filled with the daily norms of each employee like 
         * {*1 norm of 1 employee*, *2 norm of 1 employee*, *3 norm of 1 employee*, *1 norm of 2 employee*, ...}
        */
        public double[] resp = new double[12] { 100, 20, 30, 150, 15, 40, 200, 30, 40, 200, 15, 30 };
        public ConfigForm()
        {
            InitializeComponent();

            url = new StreamReader("url.config").ReadLine();
            usersData = GetData();
            if (usersData != null)
                for (int i = 0; i < usersData.Names.Length; i++)
                    UsersBox.Items.Add(usersData.Names[i]);
            else
                infoLabel.Text = "Cannot upload data";
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            if (UsersBox.SelectedIndex == -1) infoLabel.Text = "Please choose an employees";
            else if (ProjLText.Text != "0" && DealsText.Text != "0" && CallsText.Text != "0")
            {
                infoLabel.Text = "Successfull!";
                CheckAchievements(UsersBox.SelectedIndex);
                MakeScore(UsersBox.SelectedIndex);
            }
            else infoLabel.Text = "Please input all fields";
        }


        private void CheckAchievements(int i)
        {
            /*
             * Confidential
            */
        }
        private void MakeScore(int i)
        {
            double score = 0;
            double[] tmpCoef = { 0.5, 0.3, 0.2 };
            TextBox[] tmpConf = { ProjLText, DealsText, CallsText };
            for (int j = i*3; j < 3*i+3; j++)
            {
                score += tmpCoef[j-(i*3)] * (Convert.ToDouble(tmpConf[j-(i*3)].Text) / resp[j]);
            }

            SendRequest(string.Format("{0}edit_data/id={1}&t={2}&v={3}", url, UsersBox.SelectedIndex, "score", Convert.ToInt32(score*100)));
        }

        
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
                    infoLabel.Text = " " + "Unable to connect to remote server";
                    return null;
                }
            }
            return response;
        }
            
        private PublicJson? GetData()
        {
            var response = SendRequest(url + "get_data/t=its_me_admin");

            if (response == "Denied.") return null;

            try
            {
                return JsonSerializer.Deserialize<PublicJson>(response)!;
            }
            catch (System.Text.Json.JsonException e)
            {
                infoLabel.Text += " " + e.Message;
                return null;
            }
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
