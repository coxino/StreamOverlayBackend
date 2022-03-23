using Creator.UserControls;
using CreatorDeprecated;
using DataLayer;
using LocalDatabaseManager;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Creator
{
    public partial class Form1 : Form, INotifyPropertyChanged
    {
        private List<Provider> providers { get => Database.Providers; }
        private List<Game> games { get => Database.Games; }
        public SetariTurneu SetariOptimi { get; set; } = new SetariTurneu();
        public SetariTurneu SetariSferturi { get; set; } = new SetariTurneu();
        public SetariTurneu SetariSemifinala { get; set; } = new SetariTurneu();
        public SetariTurneu SetariFinala { get; set; } = new SetariTurneu();
        public SetariTurneu SetariCastigator { get; set; } = new SetariTurneu();
        public string CurrentTurnament { get; internal set; }

        public static Form1 Instance { get; internal set; }

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Database.Games.Add(new Game()
            {
                Name = textBox1.Text,
                Provider = comboBox1.Text,
                Potential = textBox6.Text,
                Volatility = textBox7.Text,
                Rounds = new List<Round>()
            });

            ReloadGames();
        }

        public void ReloadGames()
        {
            var _gamesList = Database.Games.Select(x => x.Name).ToArray();

            comboBox2.Items.Clear();
            comboBox2.Items.AddRange(_gamesList);
            comboBox2.AutoCompleteSource = AutoCompleteSource.ListItems;
            comboBox2.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            comboBox2.AutoCompleteCustomSource.AddRange(_gamesList);

            gamesList.Items.Clear();
            gamesList.Items.AddRange(_gamesList);
        }

        public void ReloadProviders()
        {
            var _providerList = Database.Providers.Select(x => x.Name).ToArray();

            comboBox1.Items.Clear();
            comboBox3.Items.Clear();

            comboBox1.Items.AddRange(_providerList);
            comboBox3.Items.AddRange(_providerList);


            providerList.Items.Clear();
            providerList.Items.AddRange(_providerList);
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsNumber(e.KeyChar) && !Char.IsControl(e.KeyChar))
            {
                if (e.KeyChar != '.')
                {
                    e.Handled = true;
                    //Get out of there - make it safe to add stuff after the if statement
                    return;
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                Database.BonusHuntInformations.BonusHuntId = int.Parse(textBox4.Text);
                var editedgame = Database.Games.Where(x => x.Name == comboBox2.Text).FirstOrDefault();

                var theRound = new Round()
                {
                    BonusHuntId = int.Parse(textBox4.Text),
                    BetSize = double.Parse(textBox3.Text)
                };

                editedgame.Rounds.Add(theRound);

                Database.BonusHuntInformations.Bonuses.Add(new BonusHuntGridObject()
                {
                    GameName = editedgame.Name,
                    ProviderName = editedgame.Provider,
                    BetSize = theRound.BetSize,
                    BonusRound = Database.BonusHuntInformations.BonusHuntId,
                    Payed = theRound.PayAmount
                });

                dataGridView1.DataSource = null;
                dataGridView1.DataSource = Database.BonusHuntInformations.Bonuses;


            }
            catch
            {
                MessageBox.Show("Check Input!");
            }
        }

        internal void CloseMeci(TeamStats team1)
        {
            throw new NotImplementedException();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            UpdateBonusHuntLiveView();
        }

        CustomAnimations CustomAnimations = new CustomAnimations();

        public void UpdateBonusHuntLiveView()
        {
            int auxInt = 0;
            if (int.TryParse(textBox17.Text, out auxInt))
            {
                Database.BonusHuntInformations.StartMoney = auxInt;
            }

            if (int.TryParse(textBox4.Text, out auxInt))
            {
                Database.BonusHuntInformations.BonusHuntId = auxInt;
            }

            UpdateInPlayView();
        }

        private void PlayCustomAnimation(int v)
        {
            System.Timers.Timer t = new System.Timers.Timer();
            t.Interval = v * 1000;
            t.Elapsed += T_Elapsed;
            t.Start();
            File.WriteAllText(Settings.ProjectSettings.DatabaseFolder + "/profitAnimations.json", JsonConvert.SerializeObject(CustomAnimations));
        }

        private void T_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            (sender as System.Timers.Timer).Stop();
            CustomAnimations = new CustomAnimations();
            File.WriteAllText(Settings.ProjectSettings.DatabaseFolder + "/profitAnimations.json", JsonConvert.SerializeObject(CustomAnimations));
        }

        private void UpdateInPlayView()
        {
            Game gameInfo = new Game();
            int contor = 0;
            foreach (var game in Database.BonusHuntInformations.Bonuses)
            {
                contor++;
                if (game.Payed == 0)
                {
                    gameInfo = Database.Games.Where(x => x.Name == game.GameName).FirstOrDefault();
                    Database.InPlayGame.InHuntNumber = contor;
                    Database.InPlayGame.Game = gameInfo;
                    return;
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            RefreshGrud();
        }

        private void RefreshGrud()
        {
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = Database.BonusHuntInformations.Bonuses;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.CellEndEdit += DataGridView1_CellEndEdit;
            Instance = this;
            Database.Initialize();
            checkBox1.Checked = Database.BonusHuntInformations.IsScrolling;
            textBox17.Text = Database.BonusHuntInformations.StartMoney.ToString();
            ReloadProviders();
            ReloadGames();
            WebBrowser webBrowser = new WebBrowser();
        }

        private void DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                bool aux = false;
                int contor = 0;
                foreach (var bonus in Database.BonusHuntInformations.Bonuses)
                {
                    if (bonus.Payed == 0 && aux == false)
                    {
                        aux = true;
                        bonus.IsCurrent = true;
                        Database.InPlayGame = new InPlayGame() { Game = Database.Games.Where(x => x.Name == bonus.GameName).FirstOrDefault(), InHuntNumber = contor + 1 };
                    }
                    else
                    {
                        bonus.IsCurrent = false;
                    }

                    if (aux == false)
                        contor++;
                }

                Database.BonusHuntInformations.SliceIndex = contor;

                dataGridView1.DataSource = null;
                dataGridView1.DataSource = Database.BonusHuntInformations.Bonuses;
            }
            catch
            {

            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                var to_remove = dataGridView1.SelectedRows[0].DataBoundItem as BonusHuntGridObject;
                if (to_remove != null)
                    Database.BonusHuntInformations.Bonuses.Remove(to_remove);

                UpdateBonusHuntLiveView();
                RefreshGrud();
            }
            catch
            {

            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            foreach (var game in Database.BonusHuntInformations.Bonuses)
            {
                Game currentGame = Database.Games.Where(x => x.Name == game.GameName).FirstOrDefault();

                if (currentGame is null)
                {
                    currentGame = new Game()
                    {
                        Name = game.GameName,
                        Provider = game.ProviderName,
                        Potential = "10 000",
                        Volatility = "10",
                        Rounds = new List<Round>()
                    };
                }

                if (currentGame.Rounds is null)
                {
                    currentGame.Rounds = new List<Round>();
                }

                currentGame.Rounds.Add(new Round()
                {
                    BonusHuntId = Database.BonusHuntInformations.BonusHuntId,
                    PayAmount = game.Payed,
                    BetSize = game.BetSize
                });
            }
            Database.BonusHuntInformations.BonusHuntEnd = true;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Nu am implementat ca poate gasesc un site sa le iau direct de acolo.");
        }

        private void gamesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var gameName = gamesList.SelectedItem as string;

            var gameToEdit = Database.Games.Where(x => x.Name == gameName).FirstOrDefault();
            textBox8.Text = gameToEdit.Name;
            comboBox3.Text = gameToEdit.Provider;
            textBox9.Text = gameToEdit.Volatility;
            textBox10.Text = gameToEdit.Potential;

            dataGridView2.DataSource = null;
            dataGridView2.DataSource = gameToEdit.Rounds;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            var gameName = gamesList.SelectedItem as string;

            var gameToEdit = Database.Games.Where(x => x.Name == gameName).FirstOrDefault();
            gameToEdit.Name = textBox8.Text;
            gameToEdit.Provider = comboBox3.Text;
            gameToEdit.Volatility = textBox9.Text;
            gameToEdit.Potential = textBox10.Text;

            ReloadGames();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            Process httpserver = new Process();
            ProcessStartInfo startInfox = new System.Diagnostics.ProcessStartInfo();
            startInfox.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
            startInfox.FileName = "cmd.exe";
            startInfox.Arguments = string.Format("/C cd C:\\StreamOverlay\\&angular-http-server");
            httpserver.StartInfo = startInfox;
            httpserver.Start();
        }

        private void button17_Click(object sender, EventArgs e)
        {
            Process httpserver = new Process();
            ProcessStartInfo startInfox = new System.Diagnostics.ProcessStartInfo();
            startInfox.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
            startInfox.FileName = "cmd.exe";
            startInfox.Arguments = string.Format("/C cd..&cd RequiredSoftware&node-v14.17.6-x64.msi&npm install -g angular-http-server");
            httpserver.StartInfo = startInfox;
            httpserver.Start();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Creeaza_Click(object sender, EventArgs e)
        {

            Database.LiveTournament.IsQuarter = true;
            Database.LiveTournament.IsSemis = true;
            Database.LiveTournament.IsFinal = false;

            meciFinala.Controls.Clear();
            meciuriSemifinale.Controls.Clear();
            meciuriSferturi.Controls.Clear();

            for (int i = 0; i < 4; i++)
            {
                var control = new EchipaTurneu(textBox18.Text, textBox16.Text);
                Database.LiveTournament.MeciuriSferturi[i] = control.Meci;
                meciuriSferturi.Controls.Add(control);
            }

            for (int i = 0; i < 2; i++)
            {
                var control = new EchipaTurneu(textBox18.Text, textBox16.Text);
                control.DisableCMB();
                Database.LiveTournament.MeciuriSemiFinale[i] = control.Meci;
                meciuriSemifinale.Controls.Add(control);
            }

            var controlfinal = new UserControls.EchipaTurneu(textBox18.Text, textBox16.Text);
            controlfinal.DisableCMB();
            Database.LiveTournament.MeciFinal = controlfinal.Meci;
            meciFinala.Controls.Add(controlfinal);        }

        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                foreach (var control in meciuriSemifinale.Controls)
                {
                    (control as EchipaTurneu).UpdateCmb();
                }
                (meciFinala.Controls[0] as EchipaTurneu).UpdateCmb();
            }
            catch
            {

            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Database.BonusHuntInformations.IsScrolling = checkBox1.Checked;
        }

        private void button19_Click(object sender, EventArgs e)
        {

        }

        private void button18_Click(object sender, EventArgs e)
        {
            Settings.ProjectSettings.DatabaseFolder = @"D:\StreamTheme\StreamOverlay\dist\StreamOverlay\assets\database\";
            Database.Initialize();
        }

        private void textBox17_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsNumber(e.KeyChar) && !Char.IsControl(e.KeyChar))
            {
                if (e.KeyChar != '.')
                {
                    e.Handled = true;
                    //Get out of there - make it safe to add stuff after the if statement
                    return;
                }
            }

            int buyAmount = 0;
            if (int.TryParse(textBox8.Text, out buyAmount))
            {
                Database.BonusHuntInformations.StartMoney = buyAmount;

            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            Database.Providers.Add(new Provider()
            {
                Name = textBox2.Text
            });

            ReloadProviders();
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            if (providerList.SelectedItem != null)
            {
                if (MessageBox.Show("Esti sigur?", "Esti sigur?", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    Database.Providers.Remove(Database.Providers.Where(x => x.Name == providerList.SelectedItem.ToString()).FirstOrDefault());
                    ReloadProviders();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (gamesList.SelectedItem != null)
            {
                if (MessageBox.Show("Esti sigur?", "Esti sigur?", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    Database.Games.Remove(Database.Games.Where(x => x.Name == gamesList.SelectedItem.ToString()).FirstOrDefault());
                    ReloadGames();
                }
            }
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            Database.LiveTournament.IsPaused = !Database.LiveTournament.IsPaused;

            if (Database.LiveTournament.IsPaused == true)
            {
                button7.Text = "Reia";
            }
            if (Database.LiveTournament.IsPaused == false)
            {
                button7.Text = "Pauza";
            }
        }

        private void button21_Click(object sender, EventArgs e)
        {
            if (checkBox2.Checked == false)
            {
                if (int.TryParse(textBox21.Text, out int aux))
                {
                    if (int.TryParse(textBox22.Text, out int maxBet))
                    {
                        Database.BettingModel = new BettingModel[aux].Populate(maxBet);
                        dataGridView3.DataSource = null;
                        dataGridView3.DataSource = Database.BettingModel;
                        dataGridView3.Refresh();
                    }
                }
                else
                {
                    MessageBox.Show("Invalid input!");
                }
            }
            else
            {
                if (int.TryParse(textBox22.Text, out int maxBet))
                {
                    Database.BettingModel = new BettingModel[8].Populate(maxBet,true);
                    dataGridView3.DataSource = null;
                    dataGridView3.DataSource = Database.BettingModel;
                    dataGridView3.Refresh();
                }
            }


        }

        Timer Timer = new Timer();
        private void StartBettingUpdate()
        {
            //delete C:\Users\Luca\AppData\Roaming\Streamlabs\Streamlabs Chatbot\Services\Scripts\bets_yt.txt
            File.Delete(Settings.YoutubeBotFiles.BetsFile);
            File.Delete(Settings.TwitchBotFiles.BetsFile);


            Timer.Interval = 3000;
            Timer.Tick += Timer_Tick;
            Timer.Start();
            //start timer 1m
            //Update interface
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            ReadBotFiles();
        }

        private void ReadBotFiles()
        {
            Dictionary<string, int?> betsTW = new Dictionary<string, int?>();
            Dictionary<string, int?> betsYT = new Dictionary<string, int?>();
            if (File.Exists(Settings.TwitchBotFiles.BetsFile))
            {
                betsTW = JsonConvert.DeserializeObject<Dictionary<string, int?>>(File.ReadAllText(Settings.TwitchBotFiles.BetsFile));                
            }
            if (File.Exists(Settings.YoutubeBotFiles.BetsFile))
            {
                betsYT = JsonConvert.DeserializeObject<Dictionary<string, int?>>(File.ReadAllText(Settings.YoutubeBotFiles.BetsFile));
            }


            var totalBets = (betsTW.Values.Sum() ?? 0) + (betsYT.Values?.Sum() ?? 0);

            foreach (var bet in Database.BettingModel)
            {
                try
                {
                    bet.Voturi = 0;

                    if (betsTW.ContainsKey(bet.Key))
                    bet.Voturi = betsTW[bet.Key] ?? 0;
                    if(betsYT.ContainsKey(bet.Key))
                    bet.Voturi += betsYT[bet.Key] ?? 0;

                    var q = (bet.Voturi / (double)totalBets * 100d);
                    if(q > 0)
                    bet.Progress = (int)q;
                }
                catch
                {
                    //get past this
                }
            }
        }

        private void button22_Click(object sender, EventArgs e)
        {
            StartBettingUpdate();
            button22.Enabled = false;
        }

        private void button20_Click(object sender, EventArgs e)
        {
            button22.Enabled = true;
            textBox21.Enabled = true;
            Timer.Stop();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            textBox21.Enabled = false;
        }

        private void button23_Click(object sender, EventArgs e)
        {
            Database.BonusHuntInformations.Bonuses.Clear();
            RefreshGrud();
        }

        private void comboBox4_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //SetInPlay(comboBox4.Text);
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetInPlay(comboBox4.Text);
        }

        void SetInPlay(string text)
        {
            if (Database.Games.Where(x => x.Name == text).FirstOrDefault() != null)
            {
                Database.InPlayGame = new InPlayGame
                {
                    Game = Database.Games.Where(x => x.Name == text).FirstOrDefault()
                };
            }
            else
            {
                Add_Game add_Game = new Add_Game(text);
                if(add_Game.ShowDialog() == DialogResult.OK)
                SetInPlay(text);
            }
        }

        private void comboBox4_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SetInPlay(comboBox4.Text);
            }
        }

        private void button24_Click_1(object sender, EventArgs e)
        {
            var _gamesList = Database.Games.Select(x => x.Name).ToArray();

            comboBox4.Items.Clear();
            comboBox4.Items.AddRange(_gamesList);
            comboBox4.AutoCompleteSource = AutoCompleteSource.ListItems;
            comboBox4.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            comboBox4.AutoCompleteCustomSource.AddRange(_gamesList);
        }

        private void button25_Click(object sender, EventArgs e)
        {
            int.TryParse(textBox23.Text, out int aux1);
            int.TryParse(textBox24.Text, out int aux2);
            var game = Database.Games.Where(x => x.Name == comboBox4.Text).FirstOrDefault();
            game.Rounds.Add(new Round()
            {
                PayAmount = aux1,
                BetSize = aux2,
                BonusHuntId = new Random().Next(9999999,int.MaxValue)
            });
        }

        private void textBox24_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox2_KeyUp(object sender, KeyEventArgs e)
        {
            if (Database.Games.Where(x => x.Name == comboBox2.Text).FirstOrDefault() != null)
            {
                Database.InPlayGame = new InPlayGame
                {
                    Game = Database.Games.Where(x => x.Name == comboBox2.Text).FirstOrDefault()
                };
            }
            else
            {
                Add_Game add_Game = new Add_Game(comboBox2.Text);
                add_Game.ShowDialog();
            }
        }
    }
}
