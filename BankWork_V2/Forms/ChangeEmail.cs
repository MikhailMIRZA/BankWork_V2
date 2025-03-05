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
    public partial class ChangeEmail : Form
    {
        DataBaseConnection dataBase = new DataBaseConnection();
        public ChangeEmail()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBoxButtons btn = MessageBoxButtons.OK;
            MessageBoxIcon icon = MessageBoxIcon.Information;
            string save = "Дата сохранения";

            if (!Regex.IsMatch(textBox1.Text, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
            {
                MessageBox.Show("Пожалуйста,ввведите вашу почту", save, btn, icon);
                textBox1.Select();
                return;
            }

            var email = textBox1.Text;
            var changeEmail = $"UPDATE client SET client_email = '{email}' WHERE id_client = '{DataStorage1.idClient}'";
            var command = new SqlCommand(changeEmail, dataBase.getConnection());
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

        private void Close_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
