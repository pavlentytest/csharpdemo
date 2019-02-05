using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp5
{
    public partial class ChangeUserPassword : Form
    {

        Orders_list orderslist;
        int userid;
        public ChangeUserPassword(Orders_list o, int id)
        {
            InitializeComponent();
            this.orderslist = o;
            this.userid = id;
        }

        private void ChangeUserPassword_Load(object sender, EventArgs e)
        {

        }

        private void ChangeUserPassword_FormClosed(object sender, FormClosedEventArgs e)
        {
            orderslist.Show();
        }

        public string md5(string input)
        {
            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {

            var connection = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            connection.Open();
            SqlCommand cmd = new SqlCommand("SELECT id,admin FROM users WHERE id = '" + userid + "' AND pass = '" + md5(textBox1.Text) + "'", connection);
            int authed = Convert.ToInt32(cmd.ExecuteScalar());
            if(authed > 0)
            {
                if(textBox2.Text != textBox3.Text)
                {
                    MessageBox.Show("Пароли не совпадают");
                } else
                {
                    SqlCommand cmd2 = new SqlCommand("UPDATE users WHERE SET pass = '"+md5(textBox2.Text)+"' WHERE id = " + userid, connection);
                    int changed = Convert.ToInt32(cmd2.ExecuteNonQuery());
                    MessageBox.Show("Пароль успешно изменен!");
                    textBox1.Text = ""; textBox2.Text = ""; textBox3.Text = "";
                    this.Close();
                }
            } else
            {
                MessageBox.Show("Старый пароль введен не верно!");
            }
        }       
    }
}
