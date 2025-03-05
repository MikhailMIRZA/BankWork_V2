using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BankWork_V2.Classes;
using Microsoft.Data.SqlClient;

namespace BankWork_V2.Forms
{
    public partial class Main : Form
    {
        DataBaseConnection dataBase = new DataBaseConnection();
        DataTable dataTable = new DataTable();
        SqlDataAdapter adapter = new SqlDataAdapter();
        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            NumberCard.BringToFront();
            NumberCard.Text = "";
            CVV.BringToFront();
            label12.BringToFront();
            Date.BringToFront();
            label15.BringToFront();
            Value.BringToFront();
            Const.BringToFront();


            var queryMyCards = $"SELECT id_bank_card, bank_card_number FROM bank_card WHERE id_client = '{DataStorage1.idClient}'";

            SqlDataAdapter sqlData = new SqlDataAdapter(queryMyCards, dataBase.getConnection());
            dataBase.openConnection();
            DataTable cards = new DataTable();
            sqlData.Fill(cards);
            comboBox1.DataSource = cards;
            comboBox1.ValueMember = "id_bank_card";
            comboBox1.DisplayMember = "bank_card_number";
            dataBase.closeConnection();

            SelectBankCard();
        }
        private void SelectBankCard()
        {
            NumberCard.Text = "";
            string query = $"SELECT bank_card_number, bank_card_cvv_code, CONCAT(FORMAT(bank_card_date, '%M'), '/', FORMAT(bank_card_date, '%y')) , bank_card_balance, bank_card_currency FROM bank_card WHERE bank_card_number = '{comboBox1.GetItemText(comboBox1.SelectedItem)}' "; 
            SqlCommand command = new SqlCommand(query, dataBase.getConnection());
            dataBase.openConnection();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                var CardNumber = reader[0].ToString();
                int tmp = 0;
                int tmp1 = 4;

                for(int i = 0;i < 4;i++)
                {
                    for(int j = tmp;j < tmp1;j++)
                    {
                        NumberCard.Text += CardNumber[j].ToString();
                    }
                    NumberCard.Text += " ";
                    tmp += 4;
                    tmp1 += 4;

                }

                label15.Text = reader[1].ToString();
                Date.Text = reader[2].ToString();
                Const.Text = Math.Round(Convert.ToDouble(reader[3]), 2).ToString();
                Value.Text = reader[4].ToString();
                DataStorage1.cardCVV = label15.Text;
                label15.Text = "***";

            }
            reader.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddCard addCard = new AddCard();
            addCard.ShowDialog();
        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void CVV_Click(object sender, EventArgs e)
        {
            if(label15.Text == "***")
            {
                label15.Text = DataStorage1.cardCVV;
            }
            else
            {
                label15.Text = "***";
            }
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            var queryMyCards = $"SELECT id_bank_card, bank_card_number FROM bank_card WHERE id_client = '{DataStorage1.idClient}'";

            SqlDataAdapter sqlData = new SqlDataAdapter(queryMyCards, dataBase.getConnection());
            dataBase.openConnection();
            DataTable cards = new DataTable();
            sqlData.Fill(cards);
            comboBox1.DataSource = cards;
            comboBox1.ValueMember = "id_bank_card";
            comboBox1.DisplayMember = "bank_card_number";

            dataBase.closeConnection();

            SelectBankCard();

        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SendTo sendTo = new SendTo();
            
            DataStorage1.cardNumber = comboBox1.GetItemText(comboBox1.SelectedItem);
            comboBox1.Text = "";
            sendTo.ShowDialog();
        }

        private void Close_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Profile_Click(object sender, EventArgs e)
        {
            Profile profile = new Profile();
            profile.Show();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void TransactionList_Click(object sender, EventArgs e)
        {
            Transaction transaction = new Transaction();
            transaction.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AddPhone addPhone = new AddPhone();
            DataStorage1.cardNumber = comboBox1.GetItemText(comboBox1.SelectedItem);
            addPhone.Show();
        }

        private void Communical_Click(object sender, EventArgs e)
        {
            HomePayment homePayment = new HomePayment();
            homePayment.Show();
        }

        private void internet_Click(object sender, EventArgs e)
        {
            IntAndTVPayment internet = new IntAndTVPayment();
            internet.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var cardCurrency = "";
            var query = $"SELECT bank_card_currency FROM bank_card WHERE bank_card_number = '{DataStorage1.cardNumber}'";
            SqlCommand command = new SqlCommand(query,dataBase.getConnection());
            SqlDataReader dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                cardCurrency = dataReader[0].ToString();
            }
            dataReader.Close();

            Credit credit = new Credit();
            credit.Show();

        }
    }
}
