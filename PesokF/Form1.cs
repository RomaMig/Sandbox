using PesokF.src.s;
using PesokF.src.w;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using static PesokF.EvemaWithTriggers;
using static PesokF.MinimizeSystem;

namespace PesokF
{
    public partial class Form1 : Form
    {
        private static readonly string PATH_SAVE = Program.PathExe() + @"\Отчеты\";
        //private static readonly string PATH_SAVE = @"C:\Users\" + System.Environment.GetEnvironmentVariable("USERPROFILE") + @"\Documents\ТА\";
        private static readonly string ERROR_MASSAGE = "Ошибка:\n";
        private static readonly string[] PATH_PREFIX = { "Автоматы", "Счетчики", "Переключательные Функции" };
        private string fileName;
        private SolutionBuilder builder;
        private Tables.Trigger trigger;

        public Form1()
        {
            InitializeComponent();
            ToolStripProgressBar progress = new ToolStripProgressBar();
            ToolStripLabel text = new ToolStripLabel();
            statusStrip1.Items.Add(progress);
            statusStrip1.Items.Add(text);
            dataGridView1.AllowUserToAddRows = false;
            PictureBox[] picturesEtmp = new PictureBox[18];
            for (int i = 0; i < picturesEtmp.Length; i++)
            {
                picturesEtmp[i] = new PictureBox();
                picturesEtmp[i].Location = new Point(0, 6 + 471 * (i + 1));
                picturesEtmp[i].SizeMode = PictureBoxSizeMode.Zoom;
                picturesEtmp[i].Size = new Size(830, 465);
                tabPage3.Controls.Add(picturesEtmp[i]);
            }

            builder = new SolutionBuilder(
                dataGridView1,
                new SplitContainer[8] { splitContainer1, splitContainer2, splitContainer3, splitContainer4, splitContainer5, splitContainer6, splitContainer7, splitContainer8 },
                new PictureBox[8] { pictureBox1, pictureBox2, pictureBox3, pictureBox4, pictureBox5, pictureBox6, pictureBox7, pictureBox8 },
                new PictureBox[8] { pictureBox9, pictureBox10, pictureBox11, pictureBox12, pictureBox13, pictureBox14, pictureBox15, pictureBox16 },
                pictureBox17,
                picturesEtmp,
                tabPage3,
                Completion,
                textBox2,
                progress,
                text,
                label5);
        }

        private void toNullObjects()
        {
            label5.Text = "";
            tabPage3.AutoScroll = false;
            dataGridView1.Rows.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            toNullObjects();
            string name;
            builder.BuildSolution(
                textBox1.Text,
                trigger,
                comboBox2.Text == "ИЛИ-НЕ",
                (int)numericUpDown1.Value,
                (float)numericUpDown2.Value,
                out name);
            fileName = PATH_PREFIX[0] + @"\" + name;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            toNullObjects();
            string name;
            builder.BuildSolution(
                comboBox3.Text,
                (int)numericUpDown3.Value,
                (int)numericUpDown4.Value,
                trigger,
                comboBox2.Text == "ИЛИ-НЕ",
                (int)numericUpDown1.Value,
                (float)numericUpDown2.Value, out name);
            fileName = PATH_PREFIX[1] + @"\" + name;

        }

        private void button5_Click(object sender, EventArgs e)
        {
            toNullObjects();
            string name;
            builder.BuildSolution(
                textBox4.Text,
                textBox3.Text,
                trigger,
                comboBox2.Text == "ИЛИ-НЕ",
                (int)numericUpDown1.Value,
                (float)numericUpDown2.Value, out name);
            fileName = PATH_PREFIX[2] + @"\" + name;

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            builder.Abort();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            builder.ConfigNum = Convert.ToInt32(textBox2.Text) - 1;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            builder.ConfigNum++;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            builder.ConfigNum--;
        }

        private void Completion()
        {
            string path = PATH_SAVE + fileName;
            if (автозапускToolStripMenuItem.Checked) пускToolStripMenuItem_Click(this, null);
            if (автосохранениеToolStripMenuItem.Checked) сохранитьToolStripMenuItem_Click(path, null);
            if (автосозданиеToolStripMenuItem1.Checked) создатьToolStripMenuItem_Click(path, null);
        }

        private void пускToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                builder.StartUp();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ERROR_MASSAGE + ex.Message);
            }
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                builder.Save(PATH_SAVE + fileName, sender is string);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ERROR_MASSAGE + ex.Message);
            }
        }

        private void создатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                builder.CreateReport(PATH_SAVE + fileName, автосохранениеToolStripMenuItem.Checked);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ERROR_MASSAGE + ex.Message);
            }
        }

        private void показатьВПроводникеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(PATH_SAVE))
                Process.Start(PATH_SAVE);
            else
                MessageBox.Show("Папка с сохранениями отсутствует");
        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (comboBox1.Text == "RS")
                numericUpDown1.Maximum = 8;
            else
                numericUpDown1.Maximum = 4;
            switch (comboBox1.Text)
            {
                case "RS":
                    trigger = Tables.Trigger.RS;
                    break;
                case "T":
                    trigger = Tables.Trigger.T;
                    break;
                case "D":
                    trigger = Tables.Trigger.D;
                    break;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            builder.Abort();
        }
    }
}
