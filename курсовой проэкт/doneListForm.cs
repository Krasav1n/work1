using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace organizer
{
    public partial class doneListForm : Form
    {
        public doneListForm( )
        {
            InitializeComponent( );
        }

        private void doneListForm_Load( object sender, EventArgs e )
        {

        }

        /**
         * Функция перерисовки данных в ListView
         */
        public void repainListView( DateTime dateTime, List<ArrayList> todoList )
        {
            //  Чистим actualList
            List<ArrayList> actualList = new List<ArrayList>();
            foreach ( ArrayList taskString in todoList )
            {
                try
                {
                    if ( Convert.ToDateTime(Convert.ToString(taskString[0])).Date == dateTime.Date )
                    {
                        actualList.Add(taskString);
                    }
                }
                catch ( Exception e )
                {
                    MessageBox.Show(e.Message);
                }
            }

            //  Чистим ListView
            listView1.Clear( );

            //  Создаем заголовки ListView
            listView1.Columns.Add("Дата", 120);
            listView1.Columns.Add("Задача", 200);
            listView1.Columns.Add("Повторять", 90);

            foreach ( ArrayList actual in actualList )
            {
                ListViewItem lvi = new ListViewItem(Convert.ToString(actual[0]));
                lvi.SubItems.Add(Convert.ToString(actual[1]));
                lvi.SubItems.Add(Convert.ToString(actual[2]));
                lvi.BackColor = Color.Red;
                listView1.Items.Add(lvi);
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
