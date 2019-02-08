using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp5
{
    public partial class Adminlist : Form
    {

        SqlConnection connection = new SqlConnection(Properties.Settings.Default.dbConnectionString);

        public Adminlist()
        {
            InitializeComponent();
            connection.Open();
        }
        private void refreshGrid(int idcolor)
        {
            // Формируем запрос на выборку из таблицы заказов orders
            string sql = "SELECT id,userid,date,type,counter,delivery,total,confirmed FROM orders";
            // Открываем соединение
           
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

            if(idcolor>0)
            {
                foreach (DataGridViewRow Myrow in dataGridView1.Rows)
                {           
                    if (Convert.ToInt32(Myrow.Cells[0].Value) == idcolor) 
                    {
                        Myrow.DefaultCellStyle.BackColor = Color.LightGreen;
                        Task.Run(() => this.getColorBack(idcolor));
                    }
                   
                }
            }

        }

        private void getColorBack(int idcolor)
        {
            
            Thread.Sleep(2000);
            if (idcolor > 0)
            {
                foreach (DataGridViewRow Myrow in dataGridView1.Rows)
                {
                    if (Convert.ToInt32(Myrow.Cells[0].Value) == idcolor)
                    {
                        Myrow.DefaultCellStyle.BackColor = Color.White;
                       
                    }

                }
            }

        }


        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if(dataGridView1.SelectedRows.Count>0)
            {
                // Присваиваем значение order number из dataGridView1 => label1
                label1.Text = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();           
                string sql = "SELECT login,name,address FROM users WHERE id = " + dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
                SqlCommand sqlcommand = new SqlCommand(sql, connection);
                SqlDataReader sqlreader = sqlcommand.ExecuteReader();
                while (sqlreader.Read())
                {     
            
                    label2.Text = Convert.ToString(sqlreader[0]);
                    label3.Text = Convert.ToString(sqlreader[1]);
                    label4.Text = Convert.ToString(sqlreader[2]);

                }
                checkBox1.Checked = false;
                if(Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[7].Value) == 1) {
                    checkBox1.Checked = true;
                } else
                {
                    checkBox1.Checked = false;
                }

                sqlreader.Close();
            }        

        }

        private void Adminlist_Load(object sender, EventArgs e)
        {
            refreshGrid(0);
        }

        private void checkBox1_Click(object sender, EventArgs e)
        {
            string sql;
            if(checkBox1.Checked) {
                sql = "UPDATE orders SET confirmed = 1 WHERE id = " + dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
            } else
            {
                sql = "UPDATE orders SET confirmed = 0 WHERE id = " + dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
            }
            SqlCommand cmd2 = new SqlCommand(sql, connection);
            int changed = Convert.ToInt32(cmd2.ExecuteNonQuery());         
            MessageBox.Show("Заявка успешно подтверждена!");
            refreshGrid(Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[0].Value.ToString()));
        }
    }
}
