using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using BankWork_V2.Classes;
using Microsoft.Data.SqlClient;

namespace BankWork_V2.Forms
{
    public partial class IntAndTVPayment : Form
    {
        DataBaseConnection database = new DataBaseConnection();
        Random rnd = new Random();
        DataTable dt = new DataTable();
        PinCode PinCode = new PinCode();
        public IntAndTVPayment()
        {
            InitializeComponent();
        }

        private void IntAndTVPayment_Load(object sender, EventArgs e)
        {
            textBox4.Text = DataStorage1.cardNumber;

            var query = $"SELECT id_service, serviceName FROM clientServices WHERE serviceType = 'Internet'";
            SqlDataAdapter command = new SqlDataAdapter(query, database.getConnection());
            database.openConnection();
            command.Fill(dt);
            comboBox1.DataSource = dt;
            comboBox1.ValueMember = "id_service";
            comboBox1.DisplayMember = "serviceName";
            database.closeConnection();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBoxButtons btn = MessageBoxButtons.OK;
            MessageBoxIcon icon = MessageBoxIcon.Information;
            string save = "Дата сохранения";

            var account = textBox1.Text;
            double sum = Convert.ToDouble(textBox5.Text);
            var cardNumber = textBox4.Text;
            var cardCVV = textBox3.Text;
            var cardDate = textBox2.Text;
            var cardCVVCheck = "";
            double cardBalanceCheck = 0;
            bool error = false;
            string cardCurrency = "";

            if (!Regex.IsMatch(textBox1.Text, "^[0-9]{10}$"))
            {
                MessageBox.Show("Пожалуйста,ввведите ваш номер лицевого счета корректно", save, btn, icon);
                textBox1.Select();
                return;
            }

            string query = $"SELECT bank_card_cvv_code, CONCAT(FORMAT(bank_card_date, '%M'), '/', FORMAT(bank_card_date, '%y')) , bank_card_balance, bank_card_currency FROM bank_card WHERE bank_card_number = '{cardNumber}' ";
            SqlCommand command = new SqlCommand(query, database.getConnection());
            database.openConnection();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                cardCVVCheck = reader[0].ToString();
                cardBalanceCheck = Convert.ToDouble(reader[2].ToString());
                cardCurrency = reader[3].ToString();
            }
            reader.Close();

            if (cardCVV != cardCVVCheck)
            {
                MessageBox.Show("Ошибка,СVV неправильный", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                error = true;
            }

            if (sum > cardBalanceCheck)
            {
                MessageBox.Show("Ошибка,недостаточно средств", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                error = true;
            }
            if (error == false)
            {
                DataStorage1.bankCard = textBox4.Text;
                PinCode pinCode = new PinCode();
                PinCode.ShowDialog();

                if (DataStorage1.attemps > 0)
                {
                    DateTime transactionDate = DateTime.Now;
                    var transactionNumber = "P";

                    for (int i = 0; i < 10; i++)
                    {
                        transactionNumber += Convert.ToString(rnd.Next(0, 10));
                    }

                    var queryTrans = $"UPDATE bank_card SET bank_card_balance = bank_card_balance - '{sum}' WHERE bank_card_number = '{cardNumber}'";
                    var queryTrans1 = $"INSERT INTO transactions(transaction_type, transaction_destination, transaction_date, transaction_number, transaction_value, id_bank_card) VALUES('Оплата Интернета и ТВ', '{comboBox1.GetItemText(comboBox1.SelectedItem)}', '{transactionDate}', '{transactionNumber}','{sum}', (SELECT id_bank_card FROM bank_card WHERE bank_card_number = '{cardNumber}'))  ";
                    var queryTrans2 = $"UPDATE clientServices SET serviceBalance = serviceBalance + '{sum}' WHERE serviceName = '{comboBox1.GetItemText(comboBox1.SelectedItem)}' AND serviceType = 'Internet'";
                    var queryTrans3 = $"INSERT INTO clientPersonalAccount(personal_account, id_service, id_client) VALUES('{textBox1.Text}', (SELECT id_service FROM clientServices WHERE serviceName = '{comboBox1.GetItemText(comboBox1.SelectedItem)}'), '{DataStorage1.idClient}')";

                    var com1 = new SqlCommand(queryTrans, database.getConnection());
                    var com2 = new SqlCommand(queryTrans1, database.getConnection());
                    var com3 = new SqlCommand(queryTrans2, database.getConnection());
                    var com4 = new SqlCommand(queryTrans3, database.getConnection());

                    database.openConnection();

                    com1.ExecuteNonQuery();
                    com2.ExecuteNonQuery();
                    com3.ExecuteNonQuery();
                    com4.ExecuteNonQuery();
                    database.closeConnection();

                    Close();
                }
            }
        }

        private void Close_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
