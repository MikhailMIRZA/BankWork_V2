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
    public partial class ChangePhone : Form
    {
        DataBaseConnection dataBase = new DataBaseConnection();
        public ChangePhone()
        {
            InitializeComponent();
        }

        private void Close_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBoxButtons btn = MessageBoxButtons.OK;
            MessageBoxIcon icon = MessageBoxIcon.Information;
            string save = "Дата сохранения";

            if(!Regex.IsMatch(textBox1.Text, "^[+][7][0-9]{7,14}$"))
            {
                MessageBox.Show("Пожалуйста введите коректный номер телефона",save,btn,icon);
                textBox1.Select();
                return;
            }

            var phoneNumber = textBox1.Text;
            var ChangeData = $"UPDATE client SET client_phone_number = '{phoneNumber}' WHERE id_client = '{DataStorage1.idClient}'";
            var command = new SqlCommand(ChangeData, dataBase.getConnection());
            dataBase.openConnection();
            if (command.ExecuteNonQuery() == 1)
            {
                MessageBox.Show("Данные сохранены");
                Close();
            }
            else
            {
                MessageBox.Show("Ошибка");
            }
            dataBase.closeConnection();
        }

        private void ChangePhone_Load(object sender, EventArgs e)
        {

        }
    }
}
