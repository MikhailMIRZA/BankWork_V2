using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BankWork_V2.Classes;
using Microsoft.Data.SqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace BankWork_V2.Forms
{
    public partial class Credit : Form
    {
        DataBaseConnection database = new DataBaseConnection();
        Random rnd = new Random();
        DataTable dt = new DataTable();
        PinCode pinCode = new PinCode();
        SqlDataAdapter adapter = new SqlDataAdapter();

        public Credit()
        {
            InitializeComponent();

            System.Globalization.CultureInfo culture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            culture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = culture;
        }

        private void Credit_Load(object sender, EventArgs e)
        {
            textBox1.Text = trackBar1.Value.ToString();
            textBox2.Text = trackBar2.Value.ToString();
            panel3.Visible = false;
            button2.Visible = false;

            var totalSum = "";
            var sum = "";
            DateTime date = new DateTime();
            var idCredit = "";

            double creditTotalSumToChek = 0;
            double creditSumToChek = 0;

            var query = $"SELECT credit_total_sum, credit_sum FROM credits WHERE id_bank_card = (SELECT id_bank_card FROM bank_card WHERE bank_card_number = '{DataStorage1.cardNumber}') ";
            SqlCommand command = new SqlCommand(query,database.getConnection());
            database.openConnection();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                creditTotalSumToChek = Convert.ToDouble(reader[0]);
                creditSumToChek = Convert.ToDouble(reader[1]);
            }
            reader.Close();

            if(creditSumToChek >= creditTotalSumToChek)
            {
                var query1 = $"DELETE FROM credits WHERE id_bank_card = (SELECT id_bank_card FROM bank_card WHERE bank_card_number = '{DataStorage1.cardNumber}')";
                SqlCommand command1 = new SqlCommand(query1,database.getConnection());
                command1.ExecuteNonQuery();
            }
            var query2 = $"SELECT credits.id_bank_card, credits.credit_total_sum, credits.credit_date, credits.id_credit FROM credits INNER JOIN bank_card ON credits.id_bank_card = bank_card.id_bank_card WHERE bank_card.bank_card_number = '{DataStorage1.cardNumber}'";
            SqlCommand command2 = new SqlCommand(query2,database.getConnection());
            SqlDataReader reader2 = command2.ExecuteReader();
            while (reader2.Read())
            {
                totalSum = reader2[1].ToString();
                sum = reader2[2].ToString();
                date = Convert.ToDateTime(reader2[3]);
                idCredit = reader2[4].ToString();

            }
            reader2.Close();

            SqlCommand command3 = new SqlCommand(query2 ,database.getConnection());
            adapter.SelectCommand = command3;
            adapter.Fill(dt);
            if(dt.Rows.Count > 0)
            {
                panel3.Visible = true;
                button2.Visible = true;

                label14.Text = Math.Round(Convert.ToDouble(sum), 2).ToString();
                label15.Text = Math.Round(Convert.ToDouble(totalSum), 2).ToString();
                label10.Text = date.ToShortDateString();

                double toPaySum = 0;
                DateTime dateTime = new DateTime();

                var query3 = $"SELECT repayment_date, repayment_sum FROM credits WHERE id_credit ='{idCredit}'";
                SqlCommand command1 = new SqlCommand(query3,database.getConnection());
                SqlDataReader reader1 = command1.ExecuteReader();
                while(reader1.Read())
                {
                    dateTime = Convert.ToDateTime(reader1[0].ToString());
                    toPaySum = Convert.ToDouble(reader1[1].ToString());
                }
                reader1.Close();
                database.closeConnection();

                label18.Text = Math.Round(toPaySum, 2).ToString();
                label17.Text = date.ToShortDateString();

            }
            
        }
        private void CalculateCredit()
        {
            double sumMonth = 0.01;
            double sum = Convert.ToDouble(textBox1.Text);
            int numberOfMonths = Convert.ToInt32(textBox2.Text);
            double result = sum * (sumMonth + (sumMonth / (Math.Pow(1 + sumMonth,numberOfMonths) -1 )));
            label4.Text = Math.Round(result, 2).ToString();
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            trackBar1.Value = Convert.ToInt32(textBox1.Text);
            CalculateCredit();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            trackBar2.Value = Convert.ToInt32(textBox2.Text);
            CalculateCredit();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            textBox1.Text = trackBar1.Value.ToString();

        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            textBox2.Text = trackBar2.Value.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            trackBar1.Value = Convert.ToInt32(textBox1.Text);
            trackBar2.Value = Convert.ToInt32(textBox2.Text);

            DataStorage1.bankCard = DataStorage1.cardNumber;
            pinCode.ShowDialog();

            if(DataStorage1.attemps > 0)
            {
                var totalSum = Convert.ToDouble(label4.Text) * Convert.ToDouble(textBox2.Text);
                DateTime dateTime = DateTime.Now;
                var repDay = dateTime.AddMonths(1);
                var pay = label4.Text;

                database.openConnection();
                var query = $"INSERT INTO credits(credit_total_sum, credit_sum, credit_date, id_bank_card) VALUES ('{totalSum}', 0, '{dateTime}', (SELECT id_bank_card FROM bank_card WHERE bank_card_number = '{DataStorage1.cardNumber}'))";
                var command1 = new SqlCommand(query,database.getConnection());
                command1.ExecuteNonQuery();

                var idCredit = "";
                var query1 = $"SELECT id_credit FROM credits WHERE id_bank_card = (SELECT id_bank_card FROM bank_card WHERE bank_card_number = '{DataStorage1.cardNumber}')";
                SqlCommand command = new SqlCommand(query1,database.getConnection());
                SqlDataReader reader = command.ExecuteReader();
                while(reader.Read())
                {
                    idCredit = reader[0].ToString();
                }
                reader.Close();

                var sum = textBox1.Text;
                var query2 = $"UPDATE credits SET repayment_date = '{repDay}', repayment_money = '{pay}' WHERE id_credit = '{idCredit}'";
                var query3 = $"UPDATE bank_card SET bank_card_balance = bank_card_balance + '{sum}' WHERE bank_card_number = '{DataStorage1.cardNumber}'";

                var command4 = new SqlCommand(query3,database.getConnection());
                var command2 = new SqlCommand(query2,database.getConnection());

                command4.ExecuteNonQuery();
                command2.ExecuteNonQuery();

                MessageBox.Show("Кредит оформлен ","Успех",MessageBoxButtons.OK,MessageBoxIcon.Information);

                DateTime PayDay = new DateTime();
                DateTime CreditTake = new DateTime();
                double creditSum = 0;
                double creditTotalSum = 0;
                double creditToPay = 0;

                var querySelect = $"SELECT credit_date, credit_sum, credit_total_sum, repayment_date, repayment_money FROM credits WHERE id_bank_card = (SELECT id_bank_card FROM bank_card WHERE bank_card_number = '{DataStorage1.cardNumber}')";
                SqlCommand command3 = new SqlCommand(querySelect,database.getConnection());
                SqlDataReader dataReader = command3.ExecuteReader();
                while (dataReader.Read())
                {
                    CreditTake = Convert.ToDateTime(dataReader[0].ToString());
                    creditSum = Convert.ToDouble(dataReader[1].ToString());
                    PayDay = Convert.ToDateTime(dataReader[2].ToString());
                    creditTotalSum = Convert.ToDouble(dataReader[3].ToString());
                    creditToPay = Convert.ToDouble(dataReader[4].ToString());
                }
                dataReader.Close();
                database.closeConnection();

                label10.Text = CreditTake.ToShortDateString();
                label14.Text = Math.Round(creditSum, 2).ToString();
                label15.Text = Math.Round(creditTotalSum, 2).ToString();
                label17.Text = PayDay.ToShortDateString();
                label18.Text = Math.Round(creditToPay, 2).ToString();

                button2.Visible = true;
                panel3.Visible = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DateTime ToPay = new DateTime();
            ToPay = ToPay.AddMonths(1);
            var sumToPay = label18.Text;
            double toPaySum = 0;
            DateTime rePay = new DateTime();
            bool error = false;

            database.openConnection();

            double cardBalanceCheck = 0;
            var queruCheckBalance = $"SELECT bank_card_balance FROM bank_card WHERE bank_card_number = '{DataStorage1.cardNumber}'";
            SqlCommand check = new SqlCommand(queruCheckBalance,database.getConnection());
            database.openConnection();
            SqlDataReader reader = check.ExecuteReader();
            while (reader.Read())
            {
                cardBalanceCheck = Convert.ToDouble(reader[0].ToString());
            }
            reader.Close();
            database.closeConnection();

            double checkSum = Convert.ToDouble(label14.Text);
            double checkTotalSum = Convert.ToDouble(label15.Text);
            bool checkStatus = false;

            if(checkSum >= checkTotalSum)
            {
                MessageBox.Show("Кредит погашен", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                checkStatus = true;
            }
            if(checkStatus == false)
            {
                double paymentSum = Convert.ToDouble(label18.Text);

                if(paymentSum > cardBalanceCheck)
                {
                    MessageBox.Show("Недостаточно средств ", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    error = true;
                }
                if(error == false)
                {
                    database.openConnection();
                    var queryPayCredit = $"UPDATE credits SET repayment_date = '{ToPay}', credit_sum = credit_sum + repayment_sum WHERE id_bank_card = (SELECT id_bank_card FROM bank_card WHERE bank_card_number = '{DataStorage1.cardNumber}')";
                    var queryPay = $"UPDATE bank_card SET bank_card_balance = bank_card_balance - '{sumToPay}' WHERE bank_card_number = '{DataStorage1.cardNumber}'";

                    DateTime transactionDate = DateTime.Now;
                    var transactionNumber = "P";

                    for (int i = 0; i < 10; i++)
                    {
                        transactionNumber += Convert.ToString(rnd.Next(0, 10));
                    }

                    var queryTrans1 = $"INSERT INTO transactions(transaction_type, transaction_destination, transaction_date, transaction_number, transaction_value, id_bank_card) VALUES('Кредит ', '{transactionDate}', '{transactionNumber}','{sumToPay}', (SELECT id_bank_card FROM bank_card WHERE bank_card_number = '{DataStorage1.cardNumber}'))";

                    var com1 = new SqlCommand(queryPayCredit, database.getConnection());
                    var com2 = new SqlCommand(queryPay, database.getConnection());
                    var com3 = new SqlCommand(queryTrans1, database.getConnection());

                    database.openConnection();

                    com1.ExecuteNonQuery();
                    com2.ExecuteNonQuery();
                    com3.ExecuteNonQuery();

                    var queryRepayment = $"SELECT repayment_date, credit_sum FROM credits WHERE id_bank_card = id_bank_card = (SELECT id_bank_card FROM bank_card WHERE bank_card_number = '{DataStorage1.cardNumber}')";
                    SqlCommand sqlCommand  = new SqlCommand(queryRepayment, database.getConnection());
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    while (sqlDataReader.Read())
                    {
                        rePay = Convert.ToDateTime(sqlDataReader[0].ToString());
                        toPaySum = Convert.ToDouble(sqlDataReader[1].ToString());
                    }
                    sqlDataReader.Close();
                    database.closeConnection();

                    label18.Text = Math.Round(toPaySum, 2).ToString();
                    label17.Text = rePay.ToShortDateString();
                }
            }

        }

        private void Close_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
