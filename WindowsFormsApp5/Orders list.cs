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
   
    public partial class Orders_list : Form
    {

        // Внутренний класс Prices (структура). Для вывода данных в ComboBox.
        private class Prices
        {
            // Поля внутреннего класса 
            // Фактически - это поля из таблицы товаров
            public int id { get; set; }
            public string name { get; set; }
            public double price { get; set; }
            // Конструктор класса
            public Prices(int i, string n, double p)
            {
                this.id = i;
                this.name = n;
                this.price = p;
            }
        }

        // Поля класса Orders_list
        Authform authform;
        int userid;
        SqlConnection connection = new SqlConnection(Properties.Settings.Default.dbConnectionString);
        double total = 0;
        // Конструктор с инициализацией начальных значений
        public Orders_list(Authform a, int cid)
        {
            InitializeComponent();
            this.authform = a;
            this.userid = cid;
        }
        // Метод, обновляющий содержимое DataGridView
        private void refreshGrid()
        {
            // Формируем запрос на выборку из таблицы заказов orders
            string sql = "SELECT id,date,type,counter,delivery,total,confirmed FROM orders WHERE userid=" + this.userid;
            // Открываем соединение
            connection.Open();
            // Выполняем запрос на выборку
            SqlCommand sqlcommand = new SqlCommand(sql, connection);
            // Формируем адаптер из запроса
            SqlDataAdapter sqladapter = new SqlDataAdapter(sqlcommand);
            // Формируем builder и далее датасет, для отображения всех полей в DataGridView
            SqlCommandBuilder sqlbuilder = new SqlCommandBuilder(sqladapter);
            DataSet ds = new DataSet();
            sqladapter.Fill(ds, "orders");
            DataTable dt = ds.Tables["orders"];         
            // Выставляем источник данных - датасет, с данными из таблицы orders
            dataGridView1.DataSource = ds.Tables["orders"];          
        }
        // Метод расчета стоимости заказа
        private void calculate()
        {   
            // current - выбранное значение в ComboBox1 в виде объекта
            Prices current = comboBox1.SelectedItem as Prices;
            // cnt - выбранное значение в ComboBox2 в int
            int cnt = comboBox2.SelectedIndex;
            if (cnt == -1) cnt = 1;
            // Считаем total значение
            total = current.price * cnt;
            // checkBox1 - доставка. Хардкод, лучше вынести в таблицу.
            if(checkBox1.Checked)
            {
                total = total + 250;
            }  
            label4.Text = total + "RUB";
        }
        // Метод, для заполнения ComboBox1
        private void fillLists()
        {
            // Формируем строку запроса
            string sql = "SELECT id,name,price FROM types";
            // Выполняем запрос
            SqlCommand sqlcommand = new SqlCommand(sql, connection);
            // Чтение данных 
            SqlDataReader sqlreader = sqlcommand.ExecuteReader();
            // Список прайсов
            List<Prices> listofprices = new List<Prices>();           
            // Заполнение listofprices объектами Prices
            while (sqlreader.Read())
            {
                listofprices.Add(new Prices(Convert.ToInt32(sqlreader["id"]), sqlreader["name"].ToString(), Convert.ToDouble(sqlreader["price"])));
            }
            // Источник данных для ComboBox1 
            comboBox1.DataSource = listofprices;
            // Наименование поля, которое будет отображаться в списке
            comboBox1.DisplayMember = "name";
            // Закрываем reader
             sqlreader.Close();
            // Заполнение ComboBox2 числовыми значения от 0 до 9
            System.Object[] ItemObject = new System.Object[10];
            for (int i = 0; i <= 9; i++)
            {
                ItemObject[i] = i;
            }
            comboBox2.Items.AddRange(ItemObject);       
      
        }
        // Метод отрабатывающий при отображении формы Orders_list
        private void Orders_list_Load(object sender, EventArgs e)
        {
            // Последовательно вызываем методы: загружающий данные по заявкйе, заполнябщий комбобоксы
            refreshGrid();
            fillLists();           
        }
        // При изменении индекса в списках товаров - пересчитать стоимость заказа
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            calculate();
        }
        // При изменении индекса в списках количества - пересчитать стоимость заказа
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            calculate();
        }
        // Пересчет при выборе доставки - checkbox
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            calculate();
        }
        // Метод сохранения заказа
        private void button1_Click(object sender, EventArgs e)
        { 
            SqlCommand cmd = new SqlCommand("INSERT INTO orders (userid,date,type, counter,delivery, confirmed,total)" +
                " VALUES (@userid,@date,@type,@counter,@delivery,@confirmed,@total)", connection);
            cmd.Parameters.AddWithValue("@userid", this.userid);
            cmd.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd h:m:s"));
            Prices current = comboBox1.SelectedItem as Prices;
            cmd.Parameters.AddWithValue("@type", current.id);
            int cnt = comboBox2.SelectedIndex;
            cmd.Parameters.AddWithValue("@counter", cnt);                
            cmd.Parameters.AddWithValue("@delivery", 250);
            cmd.Parameters.AddWithValue("@confirmed", 0);
            cmd.Parameters.AddWithValue("@total", total);
            int regged = Convert.ToInt32(cmd.ExecuteNonQuery());
            connection.Close();
            MessageBox.Show("Your orders booked successeful!");
            refreshGrid();     
        }
         // Обработчик закрытия окна с заказами
        private void Orders_list_FormClosed(object sender, FormClosedEventArgs e)
        {
             // При закрытии формы с заказами открывается форма авторизации
            authform.Show();
        }
        // Обрабочик события Click по пункту в меню - изменения пароля
        private void changePasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Создание объекта класс ChangeUserPassword
            ChangeUserPassword changeuserpassword = new ChangeUserPassword(this, userid);
            // Отображение формы изменения пароля
            changeuserpassword.Show();
            // Закрытие текущей формы
            this.Hide();

        }
    }
}
