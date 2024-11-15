using System;
using System.Windows.Forms;

namespace danilamobile
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.FormClosing += Form1_FormClosing;
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Завершаем приложение при закрытии формы
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();

            // Открываем вторую форму
            form2.Show();

            // Прячем первую форму, если нужно оставить её в фоне
            this.Hide();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();

            // Открываем вторую форму
            form2.Show();

            // Прячем первую форму, если нужно оставить её в фоне
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3();
            form3.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form4 form4 = new Form4();
            form4.Show();
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form5 form5 = new Form5();
            form5.Show();
            this.Hide();
        }
    }
}
