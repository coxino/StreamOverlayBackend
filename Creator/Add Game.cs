using Creator;
using DataLayer;
using LocalDatabaseManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CreatorDeprecated
{
    public partial class Add_Game : Form
    {
        public Add_Game(string text)
        {
            InitializeComponent();
            textBox1.Text = text;
        }

        private void Add_Game_Load(object sender, EventArgs e)
        {
            ReloadProviders();
        }


        private void ReloadProviders()
        {
            var _providerList = Database.Providers.Select(x => x.Name).ToArray();
            comboBox1.Items.Clear();           
            comboBox1.Items.AddRange(_providerList);            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(Database.Providers.Any(x=>x.Name == comboBox1.Text))
            {
                CreateGame();
            }
            else
            {
                Database.Providers.Add(new DataLayer.Provider() { Name = comboBox1.Text });
                CreateGame();
            }

            Form1.Instance.ReloadGames();
            Form1.Instance.ReloadProviders();

            Close();
            DialogResult = DialogResult.OK;
        }

        private void CreateGame()
        {
            Database.Games.Add(new Game()
            {
                Name = textBox1.Text,
                Provider = comboBox1.Text,
                Potential = textBox6.Text,
                Volatility = textBox7.Text,
                Rounds = new List<Round>()
            });
        }
    }
}
