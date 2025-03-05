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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace BankWork_V2.Forms
{
    public partial class AddPhone : Form
    {
        DataBaseConnection dataBase = new DataBaseConnection();
        Random rnd = new Random(); 
        SqlDataAdapter adapter = new SqlDataAdapter();
        DataTable dt = new DataTable();

        public AddPhone()
        {
            InitializeComponent();
        }

        private void AddPhone_Load(object sender, EventArgs e)
        {
            PhoneNumber.Text = DataStorage1.phoneNumber;
            textBox4.Text = DataStorage1.cardNumber;

            var query = $"SELECT id_service, serviceName FROM clientServices WHERE serviceType = 'Mobile'";
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query,dataBase.getConnection());
            dataBase.openConnection();
            DataTable dt2 = new DataTable();
            sqlDataAdapter.Fill(dt2);
            comboBox1.DataSource = dt2;
            comboBox1.ValueMember = "id_service";
            comboBox1.DisplayMember = "serviceName";
            dataBase.closeConnection();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBoxButtons btn = MessageBoxButtons.OK;
            MessageBoxIcon icon = MessageBoxIcon.Information;
            string save = "Дата сохранения";

            string tmp = PhoneNumber.Text;
            string phoneNumCheck = String.Concat(tmp[0], tmp[1]);

            string NumOperator = comboBox1.GetItemText(comboBox1.SelectedItem);

            bool check = false;

            if(NumOperator == "МТС")
            {
                if(phoneNumCheck != "98" && phoneNumCheck != "97")
                {
                    MessageBox.Show("Введите корректный номер!", save,btn,icon);
                    check = true;
                }
            }
            else if (NumOperator == "Мегафон")
            {
                if (phoneNumCheck != "92" && phoneNumCheck != "93")
                {
                    MessageBox.Show("Введите корректный номер!", save, btn, icon);
                    check = true;
                }
            }
            else if (NumOperator == "Билайн")
            {
                if (phoneNumCheck != "90" && phoneNumCheck != "95")
                {
                    MessageBox.Show("Введите корректный номер!", save, btn, icon);
                    check = true;
                }
            }
            else if (NumOperator == "Теле2")
            {
                if (phoneNumCheck != "91" && phoneNumCheck != "94")
                {
                    MessageBox.Show("Введите корректный номер!", save, btn, icon);
                    check = true;
                }
            }
            if(check == false)
            {
                var phoneNumber = PhoneNumber.Text;
                double sum = Convert.ToDouble(textBox1.Text);
                var cardNumber = textBox4.Text;
                var cardCVV = textBox3.Text;
                var cardDate = textBox2.Text;
                var cardCVVCheck = "";
                double cardBalanceCheck = 0;
                bool error = false;
                string cardCurrency = "";

                double commision = ((Convert.ToDouble(sum) * 2) / 100);
                double totalsum =  commision + Convert.ToDouble(sum);

                if (!Regex.IsMatch(PhoneNumber.Text, "^[0-9]{10}$"))
                {
                    MessageBox.Show("Пожалуйста,ввведите ваш номер телефона", save, btn, icon);
                    PhoneNumber.Select();
                    return;
                }
                string query = $"SELECT bank_card_cvv_code, CONCAT(FORMAT(bank_card_date, '%M'), '/', FORMAT(bank_card_date, '%y')) , bank_card_balance, bank_card_currency FROM bank_card WHERE bank_card_number = '{cardNumber}' ";
                SqlCommand CheckCard = new SqlCommand(query,dataBase.getConnection());
                dataBase.openConnection();
                SqlDataReader dataReader = CheckCard.ExecuteReader();

                while(dataReader.Read())
                {
                    cardCVVCheck = dataReader[0].ToString();
                    cardBalanceCheck = Convert.ToDouble(dataReader[2].ToString());
                    cardCurrency = dataReader[3].ToString();
                }
                dataReader.Close();

               

                if (cardCVV != cardCVVCheck)
                {
                    MessageBox.Show("Ошибка,СVV неправильный", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    error = true;
                }

                if (Convert.ToDouble(sum) < 15.00)
                {
                    MessageBox.Show("Ошибка,минимальная сумма пополнения 15 рублей", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    error = true;
                }
                if (sum > cardBalanceCheck)
                {
                    MessageBox.Show("Ошибка,недостаточно средств неправильный", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    error = true;
                }



                if (error ==  false)
                {
                    DataStorage1.bankCard = textBox4.Text;
                    PinCode pinCode = new PinCode();
                    pinCode.ShowDialog();

                    if(DataStorage1.attemps > 0)
                    {
                        DateTime transactionDate = DateTime.Now;
                        var transactionNumber = "P";

                        for(int i = 0;i < 10;i++)
                        {
                            transactionNumber += Convert.ToString(rnd.Next(0, 10));
                        }

                        var queryTrans = $"UPDATE bank_card SET bank_card_balance = bank_card_balance - '{totalsum}' WHERE bank_card_number = '{cardNumber}'";
                        var queryTrans1 = $"INSERT INTO transactions(transaction_type, transaction_destination, transaction_date, transaction_number, transaction_value, id_bank_card) VALUES('Пополнение мобильного', '{PhoneNumber.Text}', '{transactionDate}', '{transactionNumber}','{totalsum}', (SELECT id_bank_card FROM bank_card WHERE bank_card_number = '{cardNumber}'))";
                        var queryTrans2 = $"UPDATE clientServices SET serviceBalance = '{sum}' WHERE serviceName = '{comboBox1.GetItemText(comboBox1.Text)}' AND serviceType = 'Mobile'";

                        var com1 = new SqlCommand(queryTrans, dataBase.getConnection());
                        var com2 = new SqlCommand(queryTrans1, dataBase.getConnection());
                        var com3 = new SqlCommand(queryTrans2, dataBase.getConnection());

                        dataBase.openConnection();

                        com1.ExecuteNonQuery();
                        com2.ExecuteNonQuery();
                        com3.ExecuteNonQuery();

                        dataBase.closeConnection();

                        Close();
                    }
                }
            }

        }

        private void Close_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if(textBox1.Text == string.Empty)
            {
                textBox1.Text = null;
                label8.Text = "0";
                label11.Text = "0";
            }
            else
            {
                double sum = Convert.ToDouble(textBox1.Text);
                label8.Text = Convert.ToString((sum * 2) / 100);
                label11.Text = Convert.ToString(((sum * 2) / 100) + sum);
            }
        }
    }
}
