using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TaskSchelder
{
    public partial class Form1 : Form
    {
        TaskCharp task = new TaskCharp();
        public Form1()
        {
            InitializeComponent();
        }
        private void EnumAllTasks(string path)
        {
            listView1.Items.Clear();
            imageList1.Images.Clear();

            task.ActOnStart += () =>
            {
                listView1.Items.Clear();
                imageList1.Images.Clear();
                
            };
            task.ActOnFolder += (e) =>
            {
                ListViewItem lvi = new ListViewItem($" {e.Name}", listView1.Items.Count);
                imageList1.Images.Add(Properties.Resources.folder);
                lvi.SubItems.Add(e.Path);
                lvi.SubItems.Add("Папка");
                listView1.Items.Add(lvi);
            };
            task.ActOnTask += (t) =>
            {
                ListViewItem lvi = new ListViewItem($"{t.Name}",listView1.Items.Count);
                imageList1.Images.Add(Properties.Resources.file);
                lvi.SubItems.Add(t.Path);
                lvi.SubItems.Add("Задача");
                listView1.Items.Add(lvi);
                AutoSizeColumnList(listView1);
            };
            task.ActOnProgress += () => {
                textBox1.Text = task.current.Path;
            };
            task.EnumAllTasks(path);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            EnumAllTasks("\\");
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems.Count != 0)
            {
                if (listView1.SelectedItems[0].Text.Contains(" "))
                {
                    task.EnumAllTasks(listView1.SelectedItems[0].SubItems[1].Text);
                    
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите удалить данную задачу?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                task.current.DeleteTask(listView1.SelectedItems[0].Text, 0);
                task.EnumAllTasks(task.current.Path);
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            task.EnumAllTasks(task.parent.Path);
        }

        private void AutoSizeColumnList(ListView listView)
        {
            listView.BeginUpdate();
            Dictionary<int, int> columnSize = new Dictionary<int, int>();
            listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            foreach (ColumnHeader colHeader in listView.Columns)
                columnSize.Add(colHeader.Index, colHeader.Width);
            listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            foreach (ColumnHeader colHeader in listView.Columns)
            {
                int nColWidth;
                if (columnSize.TryGetValue(colHeader.Index, out nColWidth))
                    colHeader.Width = Math.Max(nColWidth, colHeader.Width);
                else
                    colHeader.Width = Math.Max(50, colHeader.Width);
            }
            listView.EndUpdate();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            AutoSizeColumnList(listView1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            task.EnumAllTasks(textBox1.Text);
        }

        private void dfghjkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите удалить данную задачу?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                task.current.DeleteTask(listView1.SelectedItems[0].Text, 0);
                task.EnumAllTasks(task.current.Path);
            }
        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            if (listView1.SelectedItems.Count > 0&& listView1.SelectedItems[0].SubItems[2].Text=="Задача")
            {
                e.Cancel = false;
            }
            
        }

        private void подробностиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TaskInfo task1 =  task.GetTask(listView1.SelectedItems[0].Text, listView1.SelectedItems[0].SubItems[1].Text);
            Form f = new Info(task1.Name, task1.Path, task1.State, task1.LastRunTime,task1.Enabled);
            f.ShowDialog();
            Console.WriteLine(task1.State);
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) EnumAllTasks(textBox1.Text);
        }
    }
}
