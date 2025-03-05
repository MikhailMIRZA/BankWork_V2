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
    public partial class Profile : Form
    {
        DataBaseConnection database = new DataBaseConnection();
        public Profile()
        {
            InitializeComponent();
        }

        private void Close_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Profile_Load(object sender, EventArgs e)
        {
            RefreshData();
        }
        private void RefreshData()
        {
            var queryCheck = $"SELECT CONCAT(client_last_name, ' ' , client_first_name, ' ' , client_middle_name),client_phone_number, client_email FROM client WHERE id_client = '{DataStorage1.idClient}'";
            SqlCommand command = new SqlCommand(queryCheck,database.getConnection());
            database.openConnection();
            SqlDataReader dataReader = command.ExecuteReader();
            while(dataReader.Read())
            {
                label4.Text += dataReader[0].ToString();
                label5.Text += dataReader[1].ToString();
                label6.Text += dataReader[2].ToString();
            }
            dataReader.Close();
        }
        private void ClearData()
        {
            label4.Text = string.Empty;
            label5.Text = string.Empty;
            label6.Text = string.Empty;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ClearData();
            RefreshData();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ChangeEmail changeEmail = new ChangeEmail();
            changeEmail.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ChangePhone changePhone = new ChangePhone();
            changePhone.Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            ChangePassword changePassword = new ChangePassword();
            changePassword.Show();
        }
    }
}
