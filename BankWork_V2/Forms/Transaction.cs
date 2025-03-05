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
    public partial class Transaction : Form
    {
        DataBaseConnection dataBase = new DataBaseConnection();
        public Transaction()
        {
            InitializeComponent();
        }

        private void Close_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Transaction_Load(object sender, EventArgs e)
        {

            listView1.View = View.Details;


            if (listView1.Columns.Count == 0)
            {
                listView1.Columns.Add("Тип транзакции", 150, HorizontalAlignment.Left);
                listView1.Columns.Add("Назначение", 200, HorizontalAlignment.Left);
                listView1.Columns.Add("Дата", 100, HorizontalAlignment.Center);
                listView1.Columns.Add("Номер", 80, HorizontalAlignment.Right);
                listView1.Columns.Add("Сумма", 100, HorizontalAlignment.Right);
            }


            var history = $"SELECT transaction_type, transaction_destination, transaction_date, transaction_number, transaction_value FROM transactions INNER JOIN bank_card ON transactions.id_bank_card = bank_card.id_bank_card INNER JOIN client ON client.id_client = bank_card.id_client WHERE client.id_client = '{DataStorage1.idClient}'";
            SqlCommand sqlCommand = new SqlCommand(history, dataBase.getConnection());
            dataBase.openConnection();
            SqlDataReader reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                ListViewItem item = new ListViewItem(reader[0].ToString());
                item.SubItems.Add(reader[1].ToString());
                item.SubItems.Add(reader[2].ToString());
                item.SubItems.Add(reader[3].ToString());
                item.SubItems.Add(reader[4].ToString());
                listView1.Items.Add(item);
            }
            reader.Close();
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent); 
            listView1.Sort();
        }
    }
}
