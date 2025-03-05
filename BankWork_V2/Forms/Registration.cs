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
    public partial class Registration : Form
    {
        DataBaseConnection dataBase = new DataBaseConnection();
        public Registration()
        {
            InitializeComponent();
        }

        private void Registration_Load(object sender, EventArgs e)
        {
            textBox1.Select();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBoxButtons btn = MessageBoxButtons.OK;
            MessageBoxIcon icon = MessageBoxIcon.Information;

            string caption = "Дата сохранения";

            if (!Regex.IsMatch(textBox1.Text, "[А-Яа-я]+$"))
            {
                MessageBox.Show("Пожалуйста,введите фамилию повторно!",caption,btn,icon);
                textBox1.Select();
                return;
            }
            if(!Regex.IsMatch(textBox2.Text, "[А-Яа-я]+$"))
            {
                MessageBox.Show("Пожалуйста,введите имя повторно!",caption, btn,icon);
                textBox2.Select(); 
                return;
            }
            if(!Regex.IsMatch(textBox3.Text, "[А-Яа-я]+$"))
            {
                MessageBox.Show("Пожалуйста,введите отчество повторно!", caption, btn, icon);
                textBox3.Select();
                return;
            }
            if(string.IsNullOrEmpty(comboBox1.SelectedItem.ToString()))
            {
                MessageBox.Show("Пожалуйста,выберите пол!", caption, btn, icon);
                comboBox1.Select();
                return;
            }
            if(!Regex.IsMatch(textBox4.Text, "^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@s**&*-]).{8,}$"))
            {
                MessageBox.Show("Пожалуйста,ввведите пароль", caption, btn, icon);
                textBox4.Select();
                return;
            }
            if(!Regex.IsMatch(textBox5.Text, "^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@s**&*-]).{8,}$"))
            {
                MessageBox.Show("Пожалуйста,ввведите потверждение пароля", caption, btn, icon);
                textBox5.Select();
                return;
            }
            if(textBox4.Text != textBox5.Text)
            {
                MessageBox.Show("Пароли не совпадают!",caption,btn, icon);
                textBox5.SelectAll();
                return;
            }
            if (!Regex.IsMatch(textBox6.Text, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
            {
                MessageBox.Show("Пожалуйста,ввведите вашу почту", caption, btn, icon);
                textBox6.Select();
                return;
            }
            if(!Regex.IsMatch(textBox7.Text,"^[+][7][0-9]{7,14}$"))
            {
                MessageBox.Show("Пожалуйста,ввведите ваш номер телефона", caption, btn, icon);
                textBox7.Select();
                return;
            }
            string mcSQL = "SELECT client_phone_number FROM client WHERE client_phone_number = ' " + textBox7.Text + "'";
            SqlDataAdapter adapter = new SqlDataAdapter();
            DataTable table = new DataTable();

            SqlCommand command = new SqlCommand(mcSQL, dataBase.getConnection());

            adapter.SelectCommand = command;
            adapter.Fill(table);

            if(table.Rows.Count > 0)
            {
                MessageBox.Show("Номер телефона уже существует.Невозможно зарегистрировать аккаунт","Ошибка",MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox7.SelectAll();
                return;
            }

            DialogResult result;
            result = MessageBox.Show("Вы хотите сохранить данные?","Сохранение данных",MessageBoxButtons.YesNo,MessageBoxIcon.Question);

            if(result == DialogResult.Yes)
            {
                string mySQL = string.Empty;

                mySQL += "INSERT INTO client (client_last_name, client_first_name, client_middle_name, client_gender, client_password, client_email, client_phone_number) ";
                mySQL += "VALUES ('" + textBox1.Text + "', '" + textBox2.Text + "', '" + textBox3.Text + "', ";
                mySQL += "'" + comboBox1.SelectedItem.ToString() + "', '" + textBox4.Text + "', '" + textBox6.Text + "', '" + textBox7.Text + "')";

                dataBase.openConnection();
                SqlCommand commandAddNewUser = new SqlCommand(mySQL, dataBase.getConnection());
                commandAddNewUser.ExecuteNonQuery();

                MessageBox.Show("Запсь успешно произведена","Данные сохранены",MessageBoxButtons.OK, MessageBoxIcon.Information);
                dataBase.closeConnection();
                Close();
            }

        }

        private void clearControls()
        {
            foreach(TextBox textBox in Controls.OfType<TextBox>())
            {
                textBox.Text = string.Empty;
            }
            foreach(ComboBox comboBox in Controls.OfType<ComboBox>())
            {
                comboBox.SelectedItem = null;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Select();
            clearControls();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                textBox4.UseSystemPasswordChar = false;
                textBox5.UseSystemPasswordChar = false;
            }
            else
            {
                textBox4.UseSystemPasswordChar = true;
                textBox5.UseSystemPasswordChar = true;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
