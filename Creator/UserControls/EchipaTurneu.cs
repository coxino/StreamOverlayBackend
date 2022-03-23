using CreatorDeprecated;
using DataLayer;
using LocalDatabaseManager;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Creator.UserControls
{
    public partial class EchipaTurneu : UserControl
    {
        public Meci Meci { get; set; }

        public void UpdateWhenWinner()
        {
            try
            {
                comboBox1.Text = Meci?.Team1?.Nume;
                comboBox2.Text = Meci?.Team2?.Nume;

                UpdateStats();
            }
            catch
            {

            }
        }

        int meciuriDirecte = 1;
        public EchipaTurneu(string text, string text1)
        {
            InitializeComponent();

            int.TryParse(text1, out meciuriDirecte);
            Meci = new Meci(meciuriDirecte);

            int aux = 0;
            if (int.TryParse(text, out aux))
            {
                textBox7.Text = text;
                textBox8.Text = text;
                Meci.Team1.BuyCost = aux;
                Meci.Team2.BuyCost = aux;
            }

            comboBox1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            comboBox2.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

            comboBox1.AutoCompleteSource = AutoCompleteSource.ListItems;
            comboBox2.AutoCompleteSource = AutoCompleteSource.ListItems;

            comboBox1.AutoCompleteCustomSource.AddRange(Database.AllGamesNames);
            comboBox2.AutoCompleteCustomSource.AddRange(Database.AllGamesNames);

            comboBox1.Items.AddRange(Database.AllGamesNames);
            comboBox2.Items.AddRange(Database.AllGamesNames);

            dataGridView1.DataSource = null;
            dataGridView2.DataSource = null;

            dataGridView1.DataSource = Meci.Team1.Payout;
            dataGridView2.DataSource = Meci.Team2.Payout;

            Form1.Instance.PropertyChanged += Instance_PropertyChanged;
        }

        private void Instance_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            comboBox1.AutoCompleteCustomSource.Clear();
            comboBox2.AutoCompleteCustomSource.Clear();

            comboBox1.AutoCompleteCustomSource.AddRange(Database.AllGamesNames);
            comboBox2.AutoCompleteCustomSource.AddRange(Database.AllGamesNames);

            comboBox1.Items.Clear();
            comboBox2.Items.Clear();

            comboBox1.Items.AddRange(Database.AllGamesNames);
            comboBox2.Items.AddRange(Database.AllGamesNames);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateStats();
            }
            catch
            {
                MessageBox.Show("Check Imnput");
            }
        }

        private void UpdateStats()
        {
            try
            {
                double scor1 = Math.Round(Meci.Team1.Payout.Sum(x => x.Payout) / Meci.Team1.BuyCost, 2);
                double scor2 = Math.Round(Meci.Team2.Payout.Sum(x => x.Payout) / Meci.Team2.BuyCost, 2);
                Meci.Team1.Scor = scor1;
                Meci.Team2.Scor = scor2;
            }
            catch
            {
                //do nothing is ok
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //SetWinner
            if (Meci.Team1.Scor > Meci.Team2.Scor)
            {
                Meci.Team1.HasWon = true;
                Meci.Team1.PrevX = Meci.Team1.Scor;

                if (Database.LiveTournament.IsQuarter == true)
                {
                    var nextBattle = Database.LiveTournament.MeciuriSemiFinale
                         .Where(x => string.IsNullOrWhiteSpace(x.Team1.Nume) || string.IsNullOrWhiteSpace(x.Team2.Nume)).FirstOrDefault();

                    if (string.IsNullOrWhiteSpace(nextBattle.Team1.Nume))
                    {
                        nextBattle.Team1 = Meci.Team1;
                    }

                    else if (string.IsNullOrWhiteSpace(nextBattle.Team2.Nume))
                    {
                        nextBattle.Team2 = Meci.Team1;
                    }
                }

                if (Database.LiveTournament.IsSemis == true && Database.LiveTournament.IsQuarter == false)
                {
                    if (string.IsNullOrWhiteSpace(Database.LiveTournament.MeciFinal.Team1.Nume))
                    {
                        Database.LiveTournament.MeciFinal.Team1 = Meci.Team1;
                    }

                    else if (string.IsNullOrWhiteSpace(Database.LiveTournament.MeciFinal.Team2.Nume))
                    {
                        Database.LiveTournament.MeciFinal.Team2 = Meci.Team1;
                    }
                }
            }
            else
            {
                Meci.Team2.HasWon = true;
                Meci.Team2.PrevX = Meci.Team2.Scor;
                if (Database.LiveTournament.IsQuarter == true)
                {
                    
                    var nextBattle = Database.LiveTournament.MeciuriSemiFinale
                         .Where(x => string.IsNullOrWhiteSpace(x.Team1.Nume) || string.IsNullOrWhiteSpace(x.Team2.Nume)).FirstOrDefault();

                    if (string.IsNullOrWhiteSpace(nextBattle.Team1.Nume))
                    {
                        nextBattle.Team1 = Meci.Team2;
                    }

                    else if (string.IsNullOrWhiteSpace(nextBattle.Team2.Nume))
                    {
                        nextBattle.Team2 = Meci.Team2;
                    }
                }

                if (Database.LiveTournament.IsSemis == true && Database.LiveTournament.IsQuarter == false)
                {
                    if (string.IsNullOrWhiteSpace(Database.LiveTournament.MeciFinal.Team1.Nume))
                    {
                        Database.LiveTournament.MeciFinal.Team1 = Meci.Team2;
                    }

                    else if (string.IsNullOrWhiteSpace(Database.LiveTournament.MeciFinal.Team2.Nume))
                    {
                        Database.LiveTournament.MeciFinal.Team2 = Meci.Team2;
                    }
                }
            }

            button2.Text = "!INCHIS!";
            this.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UpdateStats();                   
        }

        private void textBox7_KeyPress(object sender, KeyPressEventArgs e)
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

            double buyAmount = 0;
            if (double.TryParse(textBox7.Text, out buyAmount))
            {
                Meci.Team1.BuyCost = buyAmount;
                UpdateStats();
            }
        }

        private void textBox8_KeyPress(object sender, KeyPressEventArgs e)
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

            double buyAmount = 0;
            if (double.TryParse(textBox8.Text, out buyAmount))
            {
                Meci.Team2.BuyCost = buyAmount;
                UpdateStats();
            }
        }
                
        private void dataGridView2_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            UpdateStats();
        }
        
        public void UpdateCmb()
        {
            if (!string.IsNullOrWhiteSpace(Meci.Team1.Nume))
            {
                comboBox1.Text = Meci.Team1.Nume;
                Meci.Team1.Scor = 0;
                dataGridView1.DataSource = Meci.Team1.Payout;

                foreach (var team in Meci.Team1.Payout)
                {
                    team.Payout = 0;
                }
            }

            if (!string.IsNullOrWhiteSpace(Meci.Team2.Nume))
            {
                comboBox2.Text = Meci.Team2.Nume;
                Meci.Team2.Scor = 0;
                dataGridView2.DataSource = Meci.Team2.Payout;
                foreach (var team in Meci.Team2.Payout)
                {
                    team.Payout = 0;
                }
            }
        }

        internal void DisableCMB()
        {
            comboBox1.Enabled = false;
            comboBox2.Enabled = false;
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var nume = comboBox1.Text;
            AddGameToTournament(nume);
            Meci.Team1.Nume = nume;
            Meci.Team1.HasWon = false;
            UpdateStats();
        }

        private void comboBox2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var nume = comboBox2.Text;
            AddGameToTournament(nume);
            Meci.Team2.Nume = nume;
            Meci.Team2.HasWon = false;
            UpdateStats();
        }

        private void AddGameToTournament(string name)
        {
            if (Database.Games.Any(x => x.Name == name) == false)
            {
                Add_Game add_Game = new Add_Game(name);
                if (add_Game.ShowDialog() != DialogResult.OK)
                    return;
            }            
        }

        private void comboBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                var nume = comboBox1.Text;
                AddGameToTournament(nume);
                Meci.Team1.Nume = nume;
                Meci.Team1.HasWon = false;
                UpdateStats();
            }
        }

        private void comboBox2_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                var nume = comboBox2.Text;
                AddGameToTournament(nume);
                Meci.Team2.Nume = nume;
                Meci.Team2.HasWon = false;
                UpdateStats();
            }
        }

        private void EchipaTurneu_Load(object sender, EventArgs e)
        {

        }
    }
}
