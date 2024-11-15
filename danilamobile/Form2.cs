using MySql.Data.MySqlClient;
using System;
using System.Globalization;
using System.Windows.Forms;

namespace danilamobile
{
    public partial class Form2 : Form
    {
        private string connectionString = "Server=localhost;Database=moby;Uid=root;Pwd=a0#$f43JF@Ejk#_cwp[!323tFGed4%$%@z67cmrg-+356G^;";

        public Form2()
        {
            InitializeComponent();
            this.FormClosing += Form2_FormClosing;
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Завершаем приложение при закрытии формы
            Application.Exit();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Проверка на заполненность всех полей
            if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text) ||
                string.IsNullOrEmpty(textBox3.Text) || string.IsNullOrEmpty(textBox4.Text) ||
                string.IsNullOrEmpty(textBox5.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля!");
                return; // Прерываем выполнение метода, если одно из полей пустое
            }

            // Получаем данные из текстовых полей
            string fio = textBox1.Text;  // ФИО заказчика
            string phone = textBox2.Text;  // Телефон заказчика
            string phoneModel = textBox3.Text;  // Модель телефона
            string problemDescription = textBox4.Text;  // Описание проблемы
            DateTime deadline = dateTimePicker1.Value;  // Срок выполнения
            decimal repairCost;

            // Проверка на ввод корректной стоимости
            if (!decimal.TryParse(textBox5.Text, out repairCost))
            {
                MessageBox.Show("Пожалуйста, введите правильную стоимость ремонта.");
                return;
            }

            // Шаг 1: Проверяем, существует ли заказчик с таким номером телефона
            string checkCustomerQuery = "SELECT ID_Заказчика FROM Заказчики WHERE Телефон = @phone";
            int customerId = 0;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Пытаемся получить ID заказчика
                    using (MySqlCommand command = new MySqlCommand(checkCustomerQuery, connection))
                    {
                        command.Parameters.AddWithValue("@phone", phone);
                        object result = command.ExecuteScalar(); // Получаем результат запроса

                        if (result != null)  // Если заказчик найден
                        {
                            customerId = Convert.ToInt32(result);  // Присваиваем ID заказчика
                        }
                        else  // Если заказчик не найден
                        {
                            // Шаг 2: Создаем нового заказчика
                            string insertCustomerQuery = "INSERT INTO Заказчики (ФИО, Телефон) VALUES (@fio, @phone)";
                            using (MySqlCommand insertCommand = new MySqlCommand(insertCustomerQuery, connection))
                            {
                                insertCommand.Parameters.AddWithValue("@fio", fio);
                                insertCommand.Parameters.AddWithValue("@phone", phone);
                                insertCommand.ExecuteNonQuery();  // Добавляем нового заказчика

                                // Получаем ID нового заказчика
                                customerId = (int)insertCommand.LastInsertedId;
                            }
                        }
                    }

                    // Шаг 3: Добавляем ремонт с найденным/созданным ID_Заказчика
                    string repairQuery = "INSERT INTO Ремонты (ID_Заказчика, Модель_Телефона, Описание_Проблемы, Срок_Выполнения, Стоимость_Ремонта) " +
                                         "VALUES (@customerId, @phoneModel, @problemDescription, @deadline, @repairCost)";

                    using (MySqlCommand repairCommand = new MySqlCommand(repairQuery, connection))
                    {
                        repairCommand.Parameters.AddWithValue("@customerId", customerId);
                        repairCommand.Parameters.AddWithValue("@phoneModel", phoneModel);
                        repairCommand.Parameters.AddWithValue("@problemDescription", problemDescription);
                        repairCommand.Parameters.AddWithValue("@deadline", deadline);
                        repairCommand.Parameters.AddWithValue("@repairCost", repairCost);

                        repairCommand.ExecuteNonQuery();  // Добавляем новый ремонт
                        MessageBox.Show("Ремонт добавлен успешно!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Устанавливаем столбцы в DataGridView (если они не заданы заранее в дизайнере)
            if (dataGridView1.Columns.Count == 0)
            {
                dataGridView1.Columns.Add("fio", "ФИО");
                dataGridView1.Columns.Add("phone", "Телефон");
                dataGridView1.Columns.Add("phoneModel", "Модель телефона");
                dataGridView1.Columns.Add("problemDescription", "Описание проблемы");
                dataGridView1.Columns.Add("deadline", "Срок выполнения");
                dataGridView1.Columns.Add("repairCost", "Стоимость ремонта");
                dataGridView1.Columns.Add("repairStatus", "Статус ремонта");
            }

            // Создаем SQL-запрос для получения всех ремонтов с нужными полями
            string query = @"
        SELECT Заказчики.ФИО, Заказчики.Телефон, Ремонты.Модель_Телефона, Ремонты.Описание_Проблемы, 
               Ремонты.Срок_Выполнения, Ремонты.Стоимость_Ремонта, Ремонты.Статус_Ремонта
        FROM Ремонты
        INNER JOIN Заказчики ON Ремонты.ID_Заказчика = Заказчики.ID_Заказчика";

            // Очищаем DataGridView перед заполнением
            dataGridView1.Rows.Clear();

            // Подключаемся к базе данных и выполняем запрос
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            // Читаем все строки данных и заполняем DataGridView
                            while (reader.Read())
                            {
                                // Добавляем строку в DataGridView
                                dataGridView1.Rows.Add(
                                    reader["ФИО"].ToString(),
                                    reader["Телефон"].ToString(),
                                    reader["Модель_Телефона"].ToString(),
                                    reader["Описание_Проблемы"].ToString(),
                                    Convert.ToDateTime(reader["Срок_Выполнения"]).ToShortDateString(),
                                   Convert.ToDecimal(reader["Стоимость_Ремонта"]).ToString("C2", new CultureInfo("ru-BY")),
                                    reader["Статус_Ремонта"].ToString()
                                );
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при получении данных: " + ex.Message);
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox1.Clear();  // ФИО
            textBox2.Clear();  // Телефон
            textBox3.Clear();  // Модель телефона
            textBox4.Clear();  // Описание проблемы
            textBox5.Clear();  // Стоимость ремонта

            // Сбрасываем значение DateTimePicker на текущую дату
            dateTimePicker1.Value = DateTime.Now;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();

            // Открываем вторую форму
            form1.Show();

            // Прячем первую форму, если нужно оставить её в фоне
            this.Hide();
        }
    }
}
