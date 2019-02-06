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

        // Обработчик события Сlick по кнопке "Закрыть"
        private void button2_Click(object sender, EventArgs e)
        {
            // Закрываем текущее окно авторизации
            this.Close();
        }

        // Обработчик события Click по ссылке "Регистрация"
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Создаем объект класса Registration 
            Registration reg = new Registration();
            // Вызываем метод Show класс Registration
            reg.Show();
            // Закрываем текущую форму
            this.Hide();
        }
        
        // Метод, для получения хеша пароля в md5
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

        // Обработка события Click по кнопке Enter
        private void button1_Click(object sender, EventArgs e)
        {
            // Формируем строку соединения с SQL сервером
            var connection = new SqlConnection(Properties.Settings.Default.dbConnectionString);
            // Открываем соединения
            connection.Open();
            // Присваиваем значения полей ввода в соотстветствующие переменные
            string login = textBox1.Text;
            string password = textBox2.Text;
            // Формируем запрос к БД
            SqlCommand cmd = new SqlCommand("SELECT id,admin FROM users WHERE login = '"+login+"' AND pass = '"+md5(password)+"'", connection);
            // Чтение результатов запроса
            SqlDataReader reader = cmd.ExecuteReader();
            // Начальные значения для переменных id, admin
            // id - идентификатор пользователя
            // admin - флаг, показывающий роль пользователя
            // admin = 0 - пользователь, admin = 1 - администратор
            int id = 0;
            int admin = 0;
            
            while(reader.Read())
            {
               // Получаем значения id и admin - из БД и присваиваем их
                id = Convert.ToInt32(reader[0]);
               admin = Convert.ToInt32(reader[1]);

            }
               // Если пользователь, то показываем форму со списком заказов
            if (id > 0 && admin == 0)
            {
                // Форма, для отображения списка заказов пользователя + бронирование
                Orders_list odform = new Orders_list(this,id);
                odform.Show();
                // Сбрасываем поля на форме авторизации
                textBox1.Text = "";
                textBox2.Text = "";
                // Закрываем форму авторизации
                this.Hide();
            }
            else if (id > 0 && admin == 1)
            {
                // Форма, для отображения списка заказов всех пользователей + подтверждение заказа
                Adminlist adform = new Adminlist();
                // Сбрасываем поля на форме авторизации
                textBox1.Text = "";
                textBox2.Text = "";
                // Отображаем форму
                adform.Show();
                // Закрываем форму авторизации
                this.Hide();
            }
            else
                MessageBox.Show("Wrong data");
        }
    }
}
