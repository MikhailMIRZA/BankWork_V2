using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BankWork_V2.Classes;
using Microsoft.Data.SqlClient;

namespace BankWork_V2.Forms
{
    public partial class AddCard : Form
    {
        DataBaseConnection dataBase = new DataBaseConnection();
        Random random = new Random();
        SqlDataAdapter adapter = new SqlDataAdapter();
        DataTable dt = new DataTable();
        public AddCard()
        {
            InitializeComponent();
        }

        public void AddCard_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var cardType = comboBox1.GetItemText(comboBox1.SelectedItem);
            var currency = comboBox2.GetItemText(comboBox2.SelectedItem);
            var paymentSystem = comboBox3.GetItemText(comboBox3.SelectedItem);
            var cardNumber = "";
            var cardPin = numericUpDown1.Value;
            var cvvCode = "";
            bool isCardFree = false;
            DateTime dateTime = DateTime.Now;
            var CardDate = dateTime.AddYears(10);

            for(int i = 0;i < 3; i++)
            {
                cvvCode += Convert.ToString(random.Next(0, 10));
            }
            do {

                if (paymentSystem == "Виза")
                {
                    cardNumber += "4";
                    for (int i = 0; i < 15; i++)
                    {
                        cardNumber += Convert.ToString(random.Next(0, 10));
                    }
                }
                else
                {
                    cardNumber += "5";
                    for (int i = 0; i < 15; i++)
                    {
                        cardNumber += Convert.ToString(random.Next(0, 10));
                    }
                }
                var CheckNumber = $"SELECT * FROM  bank_card WHERE bank_card_number = '{cardNumber}'";

                SqlCommand sqlCommand = new SqlCommand(CheckNumber, dataBase.getConnection());

                adapter.SelectCommand = sqlCommand;
                adapter.Fill(dt);
                if (dt.Rows.Count == 0)
                {
                    isCardFree = true;
                }

            } while (isCardFree == false);
            var AddNewCard = $"INSERT INTO bank_card(bank_card_type, bank_card_number, bank_card_cvv_code, bank_card_currency, bank_card_paymentSystem, bank_card_date, id_client, bank_card_pin) values ('{cardType}', '{cardNumber}', '{cvvCode}','{currency}', '{paymentSystem}' , '{CardDate}', {DataStorage1.idClient}, '{cardPin}')";
            SqlCommand sql = new SqlCommand(AddNewCard,dataBase.getConnection());
            dataBase.openConnection();
            sql.ExecuteNonQuery();
            dataBase.closeConnection();

            MessageBox.Show("Карта успшно создана!", "Данные сохранены", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
