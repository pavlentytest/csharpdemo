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
    public partial class Authform : Form
    {
        public Authform()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Registration reg = new Registration();
            reg.Show();
            this.Hide();
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
    

    private void Authform_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var connection = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            connection.Open();
            string login = textBox1.Text;
            string password = textBox2.Text;
            SqlCommand cmd = new SqlCommand("SELECT id,admin FROM users WHERE login = '"+login+"' AND pass = '"+md5(password)+"'", connection);
            //int authed = Convert.ToInt32(cmd.ExecuteScalar());
            SqlDataReader reader = cmd.ExecuteReader();
            int id = 0;
            int admin = 0;
            while(reader.Read())
            {
                id = Convert.ToInt32(reader[0]);
               admin = Convert.ToInt32(reader[1]);

            }
            if (id > 0 && admin == 0)
            {
                Orders_list odform = new Orders_list(this,id);
                odform.Show();
                textBox1.Text = "";
                textBox2.Text = "";
                this.Hide();
            }
            else if (id > 0 && admin == 1)
            {
                Adminlist adform = new Adminlist();
                textBox1.Text = "";
                textBox2.Text = "";
                adform.Show();
                this.Hide();
            }
            else
                MessageBox.Show("Wrong data");
        }
    }
}
