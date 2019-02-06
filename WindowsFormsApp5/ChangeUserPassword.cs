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

        // Создаем поля класса orderlist - форма со списком заказов, userid - идентификатор пользователя
        Orders_list orderslist;
        int userid;

        // Конструктор класса с инициализацией полей класса
        public ChangeUserPassword(Orders_list o, int id)
        {
            InitializeComponent();
            this.orderslist = o;
            this.userid = id;
        }

        private void ChangeUserPassword_Load(object sender, EventArgs e)
        {

        }

        // Обработка закрытия формы
        private void ChangeUserPassword_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Показываем форму со списком заказа пользователя
            orderslist.Show();
        }

        // Метод для генерации хеша пароля. В идеале надо его вынести в отдельный класс.
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

        // Обработка события Click по кнопке Save
        private void button2_Click(object sender, EventArgs e)
        {
            // Создаем новое соединение с SQL сервером
            var connection = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            // Открываем его
            connection.Open();
            // Формируем запрос на проверку действующего пароля
            SqlCommand cmd = new SqlCommand("SELECT id,admin FROM users WHERE id = '" + userid + "' AND pass = '" + md5(textBox1.Text) + "'", connection);
            // Значение идентификатора пользователя, при условии что запрос выполнен  
            int authed = Convert.ToInt32(cmd.ExecuteScalar());
            // Если id пользователя существует
            if(authed > 0)
            {
                // Проверка паролей
                if(textBox2.Text != textBox3.Text)
                {
                    MessageBox.Show("Пароли не совпадают");
                } else
                {
                    // Формируем запрос на изменение пароля
                    SqlCommand cmd2 = new SqlCommand("UPDATE users WHERE SET pass = '"+md5(textBox2.Text)+"' WHERE id = " + userid, connection);
                    int changed = Convert.ToInt32(cmd2.ExecuteNonQuery());
                    MessageBox.Show("Пароль успешно изменен!");
                    // Сбрасываем все значения в полях
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
