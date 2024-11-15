using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace danilamobile
{
    public partial class Form5 : Form
    {
        private string connectionString = "Server=localhost;Database=moby;Uid=root;Pwd=a0#$f43JF@Ejk#_cwp[!323tFGed4%$%@z67cmrg-+356G^;";

        public Form5()
        {
            InitializeComponent();
            this.FormClosing += Form5_FormClosing;

            // Загружаем данные на форму
            LoadReports();
        }

        private void Form5_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Завершаем приложение при закрытии формы
            Application.Exit();
        }

        private void LoadReports()
        {
            // SQL-запросы для каждой таблицы
            string repairsQuery = @"
                SELECT 
                    r.ID_Ремонта, 
                    r.Модель_Телефона, 
                    r.Описание_Проблемы, 
                    r.Статус_Ремонта 
                FROM Ремонты r";

            string stockQuery = @"
                SELECT 
                    c.Название AS Комплектующее, 
                    c.Количество AS Количество_на_складе 
                FROM Комплектующие c";

            string expensesQuery = @"
                SELECT 
                    r.Модель_Телефона, 
                    r.Описание_Проблемы, 
                    c.Название AS Комплектующее, 
                    rm.Количество AS Количество_Использованное, 
                    (rm.Количество * c.Цена) AS Затраты
                FROM РасходМатериалов rm
                JOIN Ремонты r ON rm.ID_Ремонта = r.ID_Ремонта
                JOIN Комплектующие c ON rm.ID_Комплектующего = c.ID_Комплектующего";

            string incomeQuery = @"
                SELECT 
                    r.Модель_Телефона, 
                    r.Описание_Проблемы, 
                    r.Стоимость_Ремонта AS Доход
                FROM Ремонты r";

            // Выполняем запросы
            DataTable repairsTable = ExecuteQuery(repairsQuery);
            DataTable stockTable = ExecuteQuery(stockQuery);
            DataTable expensesTable = ExecuteQuery(expensesQuery);
            DataTable incomeTable = ExecuteQuery(incomeQuery);

            // Создаем элементы на форме
            int yOffset = 10; // Отступ сверху
            yOffset = CreateSection("Ремонты", repairsTable, yOffset);
            yOffset = CreateSection("Состояние склада", stockTable, yOffset);
            yOffset = CreateSection("Затраты по ремонту", expensesTable, yOffset);
            yOffset = CreateSection("Доходы по ремонту", incomeTable, yOffset);
        }

        private int CreateSection(string labelText, DataTable table, int yOffset)
        {
            // Создаем метку
            Label label = new Label
            {
                Text = labelText,
                Font = new Font("Arial", 10, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(10, yOffset) // Расположение сверху
            };
            this.Controls.Add(label);
            yOffset += label.Height + 5; // Обновляем отступ для следующего элемента

            // Создаем DataGridView
            DataGridView gridView = new DataGridView
            {
                DataSource = table,
                Location = new Point(10, yOffset),
                Size = new Size(760, 200),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true
            };
            this.Controls.Add(gridView);

            // Обновляем отступ для следующего блока
            yOffset += gridView.Height + 20;
            return yOffset;
        }

        private DataTable ExecuteQuery(string query)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    return dataTable;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                    return null;
                }
            }
        }
    }
}
