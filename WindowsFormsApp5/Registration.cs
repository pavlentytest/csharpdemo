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
        // Поля класса Registration
        private Authform rrr;
        SqlConnection connection = new SqlConnection(Properties.Settings.Default.dbConnectionString);
        // Конструктор класс Registration
        public Registration()
        {
            InitializeComponent();
        }
        // Обработчик закрытия окна
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        // Обрабочтик события Click по кнопке Save
        private void button1_Click(object sender, EventArgs e)
        {
            // Проверки
            // Если все проверки пройдены, то errors = 0, в противном случае errors > 0
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
                // Если ошибок нет, открываем SQL соединение
                connection.Open();
                try
                {
                    // Формируем строку запроса, на добавление пользователя
                    rrr = new Authform();
                    SqlCommand cmd = new SqlCommand("INSERT INTO users (name,login,pass) VALUES (@name,@login,@password)", connection);
                    cmd.Parameters.AddWithValue("@name", textBox1.Text);
                    cmd.Parameters.AddWithValue("@login", textBox2.Text);
                    cmd.Parameters.AddWithValue("@password", rrr.md5(textBox3.Text));
                    int regged = Convert.ToInt32(cmd.ExecuteNonQuery());
                    // Закрываем соединение
                    connection.Close();
                    MessageBox.Show("You have successfully registered!");
                }
                catch
                {
                    // Если такой пользователь существует, то выбрасываем исключение
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
