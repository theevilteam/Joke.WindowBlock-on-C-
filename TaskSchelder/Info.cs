using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TaskScheduler;

namespace TaskSchelder
{
    public partial class Info : Form
    {
        public Info(string _name,string _path, string _state,DateTime _last_time,bool enabled)
        {
            InitializeComponent();
            name.Text = _name;
            path.Text = _path;
            state.Text = _state.ToString();
            last_time.Text = _last_time.ToString();
            enabled_.Text = enabled ? "Включена" : "Выключена";
        }

        private void Info_Load(object sender, EventArgs e)
        {

        }
    }
}
