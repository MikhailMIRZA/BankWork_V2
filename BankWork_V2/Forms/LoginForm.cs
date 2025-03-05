using BankWork_V2.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace BankWork_V2.Forms
{
    public partial class LoginForm : Form
    {
        DataBaseConnection dataBase = new DataBaseConnection();
        public LoginForm()
        {
            InitializeComponent();
        }

 


        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void CloseProgramm_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            PhoneNumber.Select();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Registration registration = new Registration();
            registration.ShowDialog();
        }

        private void ShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            if(ShowPassword.Checked == true )
            {
                Password.UseSystemPasswordChar = false;
            }
            else
            {
                Password.UseSystemPasswordChar= true;
            }
        }

        private void Password_TextChanged(object sender, EventArgs e)
        {

        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(PhoneNumber.Text) && !string.IsNullOrEmpty(Password.Text))
            {
                var querySelectClient = $"SELECT * FROM client WHERE client_phone_number = '{PhoneNumber.Text}' AND client_password = '{Password.Text}'";
                var queryGetId = $"SELECT id_client FROM client WHERE client_phone_number = '{PhoneNumber.Text}'";
                var commandGetId = new SqlCommand(queryGetId, dataBase.getConnection());

                dataBase.openConnection();
                SqlDataReader reader = commandGetId.ExecuteReader();
                while (reader.Read())
                {
                    DataStorage1.idClient = reader[0].ToString();
                }
                reader.Close();
                
                SqlDataAdapter adapter = new SqlDataAdapter();
                DataTable table = new DataTable();

                SqlCommand command = new SqlCommand(querySelectClient,dataBase.getConnection());

                adapter.SelectCommand = command;
                adapter.Fill(table);

                if(table.Rows.Count > 0 )
                {
                    PhoneNumber.Clear();
                    Password.Clear();
                    ShowPassword.Checked = false;

                    Hide();

                    Main main = new Main();
                    main.ShowDialog();
                    main = null;

                    Show();

                    PhoneNumber.Select();
                }
                else
                {
                    MessageBox.Show("Имя пользователя или пароль неверны.Попробуйте еще раз!", "Ошикба!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    PhoneNumber.Focus();
                    PhoneNumber.SelectAll();
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста введите имя пользователя и пароль!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                PhoneNumber.Select();
            }
        }
    }
}
