using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp5
{
    public partial class Registration : Form
    {
        private Authform rrr;
        SqlConnection connection = new SqlConnection(Properties.Settings.Default.dbConnectionString);
        public Registration()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int errors = 0;
            string outMessage = "";
            if (textBox1.Text == "")
            {
                errors++;
                outMessage = outMessage + errors.ToString() + " )Enter name\n";
            }
            if (textBox2.Text == "")
            {
                errors++;
                outMessage = outMessage + errors.ToString() + " )Enter login\n";
            }
            if (textBox3.Text == "")
            {
                errors++;
                outMessage = outMessage + errors.ToString() + " )Enter password\n";
            }
            if (textBox4.Text == "")
            {
                errors++;
                outMessage = outMessage + errors.ToString() + " )Enter password\n";
            }
            if (!textBox3.Text.Equals(textBox4.Text))
            {
                errors = errors + 1;
                outMessage = outMessage + errors.ToString() + " )Password do not match\n";
            }

            if (errors == 0)
            {
                
                connection.Open();
                try
                {
                    rrr = new Authform();
                    SqlCommand cmd = new SqlCommand("INSERT INTO users (name,login,pass) VALUES (@name,@login,@password)", connection);
                    cmd.Parameters.AddWithValue("@name", textBox1.Text);
                    cmd.Parameters.AddWithValue("@login", textBox2.Text);
                    cmd.Parameters.AddWithValue("@password", rrr.md5(textBox3.Text));
                    int regged = Convert.ToInt32(cmd.ExecuteNonQuery());
                    connection.Close();
                    MessageBox.Show("You have successfully registered!");
                }
                catch
                {
                    MessageBox.Show("User exists!");
                }

            }
            else
            {
                MessageBox.Show("There are mistakes!\n" + outMessage);
            }
                }

        private void Registration_Load(object sender, EventArgs e)
        {

        }
    }
    }
