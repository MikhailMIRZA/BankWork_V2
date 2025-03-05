using BankWork_V2.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace BankWork_V2.Forms
{
    public partial class SendTo : Form
    {
        DataBaseConnection dataBase = new DataBaseConnection();
        Random rnd = new Random(); 
        SqlDataAdapter adapter = new SqlDataAdapter();
        DataTable dt = new DataTable();


        public SendTo()
        {
            InitializeComponent();
        }
        private void SendTo_Load(object sender, EventArgs e)
        {
            textBox4.Text = DataStorage1.bankCard;
            textBox1.Text = DataStorage1.cardNumber;
        }

        private void Close_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            double doll = 1.0;
            double eur = 1.0;
            
            var cardNumber = textBox1.Text;
            var cardCVV = textBox3.Text;
            var cardDate  = textBox2.Text;
            var destinationCard =textBox4.Text;
            double sum = Convert.ToDouble(textBox5.Text);
            var cardCurrency = "";
            var cardCurrency2 = "";
            var cardCVVChek = "";
            var cardDateCheck = "";
            double cardBalanceCheck = 0;
            bool error = false;

            var queryCheckCard = $"SELECT bank_card_cvv_code, CONCAT(FORMAT(bank_card_date, '%M'), '/', FORMAT(bank_card_date, '%Y')), bank_card_balance, bank_card_currency FROM bank_card WHERE bank_card_number = '{cardNumber}'"; 
            SqlCommand checkCard = new SqlCommand(queryCheckCard,dataBase.getConnection());
            dataBase.openConnection();
            SqlDataReader reader = checkCard.ExecuteReader();

            while (reader.Read()) 
            {
                cardCVVChek = reader[0].ToString();
                cardDateCheck = reader[1].ToString();
                cardBalanceCheck = Convert.ToDouble(reader[2].ToString());
                cardCurrency = reader[3].ToString();

            }
            reader.Close();

            if(cardCVV != cardCVVChek)
            {
                MessageBox.Show("Ошибка,Неправильный CVV код", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                error = true;
            }
            if(cardDate != cardDateCheck)
            {
                MessageBox.Show("Ошибка,даты не совпадают","Ошибка",MessageBoxButtons.OK, MessageBoxIcon.Information);
                
            }

            var queryCheckCardNumber = $"SELECT id_bank_card, bank_card_currency FROM bank_card WHERE bank_card_number = '{destinationCard}'";
            SqlCommand sqlCommand = new SqlCommand(queryCheckCardNumber,dataBase.getConnection());
            
            adapter.SelectCommand = sqlCommand;
            adapter.Fill(dt);
            SqlDataReader dataReader = sqlCommand.ExecuteReader();
            while (dataReader.Read())
            {
                cardCurrency2 = dataReader[1].ToString();
            }
            dataReader.Close();

            if(dt.Rows.Count == 0)
            {
                MessageBox.Show("Ошибка,Неккоретные данные получателя","Ошибка",MessageBoxButtons.OK,MessageBoxIcon.Information);
                error= true;
            }
            if(Convert.ToDouble(sum) < 15 )
            {
                MessageBox.Show("Ошибка,минимальная сумма перевода 15 рублей вата","Ошибка",MessageBoxButtons.OK,MessageBoxIcon.Information );
                error= true;
            }
            if(cardNumber == destinationCard)
            {
                MessageBox.Show("Ошибка,нельзя перевести на свою же карту", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            if(sum > cardBalanceCheck)
            {
                MessageBox.Show("Ошибка,недостаточно средств на вашей карте","Ошибка",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
                if(error == false)
            {
                DataStorage1.bankCard = textBox1.Text;
                PinCode pinCode = new PinCode();
                pinCode.ShowDialog();


                    if(DataStorage1.attemps > 0)
                {
                    DateTime transactionDate = DateTime.Now;
                    var transactionNum = "P";
                    for(int i = 0;i < 10;i++)
                    {
                        transactionNum += Convert.ToString(rnd.Next(0, 10));
                    }
                    var queryTrans = $"";
                    var queryTrans1 = $"";

                    if(cardCurrency == "RUB" && cardCurrency2 == "USD")
                    {
                        queryTrans = $"UPDATE bank_card SET bank_card_balance = bank_card_balance - '{sum}' WHERE bank_card_number = '{cardNumber}'";
                        queryTrans1 = $"UPDATE bank_card SET bank_card_balance = bank_card_balance + '{sum /= doll}' WHERE bank_card_number = '{destinationCard}'";
                    }
                    else if(cardCurrency == "RUB" && cardCurrency2 == "EUR")
                    {
                        queryTrans = $"UPDATE bank_card SET bank_card_balance = bank_card_balance - '{sum}' WHERE bank_card_number = '{cardNumber}'";
                        queryTrans1 = $"UPDATE bank_card SET bank_card_balance = bank_card_balance + '{sum /= eur}' WHERE bank_card_number = '{destinationCard}'";
                    }
                    else if (cardCurrency == "USD" && cardCurrency2 == "RUB")
                    {
                        queryTrans = $"UPDATE bank_card SET bank_card_balance = bank_card_balance - '{sum}' WHERE bank_card_number = '{cardNumber}'";
                        queryTrans1 = $"UPDATE bank_card SET bank_card_balance = bank_card_balance + '{sum *= doll}' WHERE bank_card_number = '{destinationCard}'";
                    }
                    else if (cardCurrency == "EUR" && cardCurrency2 == "RUB")
                    {
                        queryTrans = $"UPDATE bank_card SET bank_card_balance = bank_card_balance - '{sum}' WHERE bank_card_number = '{cardNumber}'";
                        queryTrans1 = $"UPDATE bank_card SET bank_card_balance = bank_card_balance + '{sum *= eur}' WHERE bank_card_number = '{destinationCard}'";
                    }
                    else if (cardCurrency == "USD" && cardCurrency2 == "EUR")
                    {
                        queryTrans = $"UPDATE bank_card SET bank_card_balance = bank_card_balance - '{sum}' WHERE bank_card_number = '{cardNumber}'";
                        queryTrans1 = $"UPDATE bank_card SET bank_card_balance = bank_card_balance + '{sum *= 0.88}' WHERE bank_card_number = '{destinationCard}'";
                    }
                    else 
                    {
                        queryTrans = $"UPDATE bank_card SET bank_card_balance = bank_card_balance - '{sum * 1.55}' WHERE bank_card_number = '{cardNumber}'";
                        queryTrans1 = $"UPDATE bank_card SET bank_card_balance = bank_card_balance + '{sum}' WHERE bank_card_number = '{destinationCard}'";
                    }

                    var queryTrans2 = $"INSERT INTO transactions(transaction_type, transaction_destination, transaction_date, transaction_number, transaction_value, id_bank_card) VALUES('Перевод','{destinationCard}','{transactionDate}','{transactionNum}','{sum}', (SELECT id_bank_card FROM bank_card WHERE bank_card_number = '{cardNumber}'))";
                    var command1 = new SqlCommand(queryTrans,dataBase.getConnection());
                    var command2 = new SqlCommand(queryTrans1,dataBase.getConnection());
                    var command3 = new SqlCommand(queryTrans2,dataBase.getConnection());
                    dataBase.openConnection();
                    command1.ExecuteNonQuery();
                    command2.ExecuteNonQuery();
                    command3.ExecuteNonQuery();
                    dataBase.closeConnection();

                    Close();
                }
            }



        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel7_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
