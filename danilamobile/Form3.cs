using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace danilamobile
{
    public partial class Form3 : Form
    {

        private string connectionString = "Server=localhost;Database=moby;Uid=root;Pwd=a0#$f43JF@Ejk#_cwp[!323tFGed4%$%@z67cmrg-+356G^;";

        public Form3()
        {
            InitializeComponent();
            this.FormClosing += Form3_FormClosing;
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Завершаем приложение при закрытии формы
            Application.Exit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Получаем данные из текстовых полей
            string название = textBox1.Text;
            string колвоText = textBox2.Text;
            string ценаText = textBox5.Text;
            int колво;
            decimal цена;

            // Проверяем, что все поля заполнены корректно
            if (string.IsNullOrEmpty(название) || string.IsNullOrEmpty(колвоText) || string.IsNullOrEmpty(ценаText) ||
                !int.TryParse(колвоText, out колво) || !decimal.TryParse(ценаText, out цена))
            {
                MessageBox.Show("Пожалуйста, заполните все поля корректно!");
                return;
            }

            // SQL-запрос для добавления данных в таблицу Комплектующие
            string query = "INSERT INTO Комплектующие (Название, Количество, Цена) VALUES (@название, @количество, @цена)";

            // Подключение к базе данных и выполнение запроса
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@название", название);
                    command.Parameters.AddWithValue("@количество", колво);
                    command.Parameters.AddWithValue("@цена", цена);

                    // Выполняем запрос
                    command.ExecuteNonQuery();
                    MessageBox.Show("Комплектующее добавлено успешно!");

                    // Очистка текстовых полей после добавления
                    textBox1.Clear();
                    textBox2.Clear();
                    textBox5.Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            {
                // SQL-запрос для получения всех комплектующих
                string query = "SELECT Название, Количество, Цена FROM Комплектующие";

                // Создаем подключение к базе данных
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();

                        // Выполняем запрос
                        MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        // Присваиваем данные DataTable в DataGridView
                        dataGridView1.DataSource = dataTable;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка: " + ex.Message);
                    }
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox1.Clear();  // ФИО
            textBox2.Clear();  // Телефон
            textBox5.Clear();  // Стоимость ремонта
        }
    }
}
