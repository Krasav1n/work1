using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using organizer.Properties;
using System.Collections;

namespace organizer
{
    public partial class AddForm : Form
    {
        OrganizerForm organizerForm;

        public void setOrganizerForm(OrganizerForm of)
        {
            this.organizerForm = of;
        }
        public AddForm()
        {
            InitializeComponent();
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            string repeater;
            switch (comboBox1.SelectedIndex)
            {
                case 0: repeater = "no";
                    break;
                case 1: repeater = "daily";
                    break;
                default: repeater = "no";
                    break;
            }

            ArrayList al = new ArrayList();
            al.Add(dateTimePicker1.Value);
            al.Add(textBox1.Text);
            al.Add(repeater);
            organizerForm.addTask(al);
            this.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void AddForm_Load(object sender, EventArgs e)
        {

        }

    }
}
