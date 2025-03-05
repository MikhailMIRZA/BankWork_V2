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
    public partial class PinCode : Form
    {
        DataBaseConnection connection = new DataBaseConnection();

        public PinCode()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int attemps = 3;
            int cardPin = Convert.ToInt32(numericUpDown1.Value);
            int pin = 0;

            var CheckPin = $"SELECT bank_card_pin FROM bank_card WHERE bank_card_number = '{DataStorage1.bankCard}'";
            SqlCommand command = new SqlCommand(CheckPin, connection.getConnection());
            connection.openConnection();
            SqlDataReader reader = command.ExecuteReader();
            while(reader.Read())
            {
                pin = Convert.ToInt32(reader[0]);
            }
            reader.Close();

           
                MessageBox.Show("Операция потверждена","Ок",MessageBoxButtons.OK,MessageBoxIcon.Information);
                Close();
                DataStorage1.attemps = attemps;
            
            


        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
