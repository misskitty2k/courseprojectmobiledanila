using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;

namespace danilamobile
{
    public partial class Form4 : Form
    {
        private string connectionString = "Server=localhost;Database=moby;Uid=root;Pwd=a0#$f43JF@Ejk#_cwp[!323tFGed4%$%@z67cmrg-+356G^;";

        public Form4()
        {
            InitializeComponent();
            LoadRepairs(); // Загружаем список ремонтов в ComboBox1
            LoadComponents(); // Загружаем комплектующие в ComboBox2
            this.FormClosing += Form4_FormClosing;
        }
        private void Form4_FormClosing(object sender, FormClosingEventArgs e)
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

        // Загружаем ремонты
        private void LoadRepairs()
        {
            string query = "SELECT ID_Ремонта, Модель_Телефона, Описание_Проблемы FROM Ремонты WHERE Статус_Ремонта = 'В процессе'";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataReader reader = command.ExecuteReader();

                    // Очищаем ComboBox перед загрузкой новых данных
                    comboBox1.Items.Clear();

                    while (reader.Read())
                    {
                        // Создаем новый объект RepairItem и добавляем его в ComboBox
                        var repairItem = new RepairItem
                        {
                            RepairId = Convert.ToInt32(reader["ID_Ремонта"]),
                            Model = reader["Модель_Телефона"].ToString(),
                            ProblemDescription = reader["Описание_Проблемы"].ToString()
                        };

                        comboBox1.Items.Add(repairItem);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            }
        }

        // Загружаем комплектующие
        private void LoadComponents()
        {
            string query = "SELECT ID_Комплектующего, Название, Количество FROM Комплектующие";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataReader reader = command.ExecuteReader();

                    // Очищаем ComboBox перед загрузкой новых данных
                    comboBox2.Items.Clear();

                    while (reader.Read())
                    {
                        // Создаем новый объект ComponentItem и добавляем его в ComboBox
                        var componentItem = new ComponentItem
                        {
                            ComponentId = Convert.ToInt32(reader["ID_Комплектующего"]),
                            Name = reader["Название"].ToString(),
                            Quantity = Convert.ToInt32(reader["Количество"])
                        };

                        comboBox2.Items.Add(componentItem);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            }
        }

        // Обработчик для добавления расхода
        private void button4_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null || comboBox2.SelectedItem == null || string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }

            // Преобразуем выбранный элемент в тип RepairItem и ComponentItem
            var selectedRepair = (RepairItem)comboBox1.SelectedItem;
            var selectedComponent = (ComponentItem)comboBox2.SelectedItem;

            // Используем свойства объектов, а не Value
            int repairId = selectedRepair.RepairId;
            int componentId = selectedComponent.ComponentId;
            int requiredQuantity = int.Parse(textBox1.Text);
            int availableQuantity = selectedComponent.Quantity;

            // Проверка на наличие достаточного количества комплектующего на складе
            if (availableQuantity < requiredQuantity)
            {
                MessageBox.Show("Недостаточно комплектующих на складе.");
                return;
            }

            // Обновление количества комплектующих на складе
            UpdateComponentQuantity(componentId, availableQuantity - requiredQuantity);

            // Добавление расхода в базу данных
            AddExpense(repairId, componentId, requiredQuantity);

            // Обновление статуса ремонта на "Выполнено"
            UpdateRepairStatus(repairId);

            MessageBox.Show("Расход добавлен успешно, статус ремонта обновлен.");
        }

        // Обновление количества комплектующих на складе
        private void UpdateComponentQuantity(int componentId, int newQuantity)
        {
            string query = "UPDATE Комплектующие SET Количество = @newQuantity WHERE ID_Комплектующего = @componentId";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@newQuantity", newQuantity);
                    command.Parameters.AddWithValue("@componentId", componentId);
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            }
        }

        // Добавление расхода в базу данных
        private void AddExpense(int repairId, int componentId, int quantity)
        {
            string query = "INSERT INTO РасходМатериалов (ID_Ремонта, ID_Комплектующего, Количество) VALUES (@repairId, @componentId, @quantity)";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@repairId", repairId);
                    command.Parameters.AddWithValue("@componentId", componentId);
                    command.Parameters.AddWithValue("@quantity", quantity);
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            }
        }

        // Обновление статуса ремонта
        private void UpdateRepairStatus(int repairId)
        {
            string query = "UPDATE Ремонты SET Статус_Ремонта = 'Выполнено' WHERE ID_Ремонта = @repairId";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@repairId", repairId);
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Запрос SQL для получения необходимых данных
            string query = @"
        SELECT 
            r.Модель_Телефона, 
            r.Описание_Проблемы, 
            c.Название, 
            cm.Количество, 
            rm.Количество AS Количество_Использованное
        FROM 
            РасходМатериалов rm
        JOIN 
            Ремонты r ON rm.ID_Ремонта = r.ID_Ремонта
        JOIN 
            Комплектующие c ON rm.ID_Комплектующего = c.ID_Комплектующего
        JOIN 
            Комплектующие cm ON rm.ID_Комплектующего = cm.ID_Комплектующего
    ";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataReader reader = command.ExecuteReader();

                    // Очищаем DataGridView перед загрузкой новых данных
                    dataGridView1.Rows.Clear();

                    // Добавляем столбцы в DataGridView, если они еще не добавлены
                    if (dataGridView1.Columns.Count == 0)
                    {
                        dataGridView1.Columns.Add("Model", "Модель Телефона");
                        dataGridView1.Columns.Add("ProblemDescription", "Описание Проблемы");
                        dataGridView1.Columns.Add("ComponentName", "Название Комплектующего");
                        dataGridView1.Columns.Add("StockQuantity", "Количество на складе");
                        dataGridView1.Columns.Add("UsedQuantity", "Количество Использованное");
                    }

                    while (reader.Read())
                    {
                        // Чтение данных из базы
                        string model = reader["Модель_Телефона"].ToString();
                        string problemDescription = reader["Описание_Проблемы"].ToString();
                        string component = reader["Название"].ToString();
                        int quantity = Convert.ToInt32(reader["Количество"]);
                        int usedQuantity = Convert.ToInt32(reader["Количество_Использованное"]);

                        // Добавляем строку с данными в DataGridView
                        dataGridView1.Rows.Add(model, problemDescription, component, quantity, usedQuantity);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            }
        }







        public class RepairItem
        {
            public int RepairId { get; set; }
            public string Model { get; set; }
            public string ProblemDescription { get; set; }

            // Переопределяем ToString() для отображения как "Модель Телефона - Описание Проблемы"
            public override string ToString()
            {
                return $"{Model} - {ProblemDescription}";
            }
        }

        public class ComponentItem
        {
            public int ComponentId { get; set; }
            public string Name { get; set; }
            public int Quantity { get; set; }

            // Переопределяем ToString() для отображения как "Название - Количество"
            public override string ToString()
            {
                return $"{Name} - {Quantity}";
            }
        }

























    }
}
