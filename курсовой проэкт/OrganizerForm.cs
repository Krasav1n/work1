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
using System.Media;

namespace organizer
{
    public partial class OrganizerForm : Form
    {
        private Settings settings = new Settings();
        public List<ArrayList> todoList = new List<ArrayList>();
        public List<ArrayList> doneList = new List<ArrayList>();
        public List<ArrayList> allList = new List<ArrayList>();
        public List<ArrayList> actualList = new List<ArrayList>();
        private int nextTask;

        protected bool updated = false;

        public OrganizerForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            /**
             * Получаем значения имен файлов из файла конфигурации программы
             */
            String todoFile = settings.todoFile;
            String doneFile = settings.doneFile;

            /**
             * Загружаем текущие задачи
             */
            ArrayList array = readFileToArray(todoFile);
            /**
             * Если массив не пустой, 
             * то разбиваем каждую строку в массив и заводим в списки
             */
            if ((array.Count != 0) && array[0] != null)
            {
                for (int i = 0; i < array.Count; i++)
                {
                    todoList.Add(getArrayFromString(Convert.ToString(array[i])));
                }
            }

            /**
             * Загружаем выполненные задачи
             */
            array = readFileToArray(doneFile);
            /**
             * Если массив не пустой, 
             * то разбиваем каждую строку в массив и заводим в списки
             */
            if ((array.Count != 0) && array[0] != null)
            {
                for (int i = 0; i < array.Count; i++)
                {
                    doneList.Add(getArrayFromString(Convert.ToString(array[i])));
                }
            }

            /**
             * Устанавливаем в календаре текущую дату.
             */
            tasksCalendar.SetDate(DateTime.Today);
            /**
             * Прорисовываем ViewList
             */
            repainListView(DateTime.Today);

            getNextTask();
            timer1.Start();
        }

        /**
         * Функция перерисовки данных в ListView
         */
        public void repainListView(DateTime dateTime)
        {
            //  Чистим actualList
            actualList.Clear();
            /**
             * Создаем список актуальныз на заданный день событий
             */
            foreach (ArrayList taskString in todoList)
            {
                try
                {
                    if (Convert.ToDateTime(Convert.ToString(taskString[0])).Date == dateTime.Date)
                    {
                        actualList.Add(taskString);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }

            //  Чистим ListView
            listView1.Clear();

            //  Создаем заголовки ListView
            listView1.Columns.Add("Дата", 120);
            listView1.Columns.Add("Задача", 200);
            listView1.Columns.Add("Повторять",90);

            foreach (ArrayList actual in actualList)
            {
                ListViewItem lvi = new ListViewItem(Convert.ToString(actual[0]));
                lvi.SubItems.Add(Convert.ToString(actual[1]));
                lvi.SubItems.Add(Convert.ToString(actual[2]));
                lvi.BackColor = Color.AliceBlue;
                listView1.Items.Add(lvi);
            }
        }

        /**
         * Функция разбивки строки на массив вида [дата время][заголовок][режим повторений]
         */
        public ArrayList getArrayFromString(string src)
        {
            ArrayList al = new ArrayList();
            /**
             * Если файлы были пустыми, то в строке может оказатья null
             */
            string delimiter = "&";
            if (src != null)
            {
                string[] dest = src.Split(delimiter.ToCharArray());
                foreach (string d in dest)
                {
                    al.Add(d);
                }
                return al;
            }
            else
            {
                return al;
            }
        }


        /**
         * Функция чтения файлов
         */
        public ArrayList readFileToArray(string fileName)
        {
            //  Переменная, в которую мы будем заносить результат помтрочного чтения из файла
            ArrayList result = new ArrayList();
            //  Строка, в которую читаем файл
            string line;

            //  Проверяем, существует ли файл.
            if (File.Exists(fileName))
            {
                //  Открываем поток для чтения из файла
                StreamReader file = new StreamReader(@fileName);

                //  Пока не достигнем конца файла
                //  построчно читаем в строку и заносим в массив
                while ((line = file.ReadLine()) != null) {
                    result.Add(line);
                }
                file.Close();
            }
            else
            {
                DialogResult dr = MessageBox.Show("Файл " + fileName + " не найден. Создать?", "Ошибка чтения файла!", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                if (dr == DialogResult.OK)
                {
                    //  Открываем поток для записи в файл.
                    StreamWriter file = new StreamWriter(@fileName);
                    file.Write("");
                    file.Close();
                    result = readFileToArray(fileName);
                }
            }

            return result;
        }

       
        /**
         * При двойном щелчке по списку выводим форму изменения выбранного события
         */
        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            ListViewItem item = new ListViewItem( );
            item = listView1.SelectedItems[0];
            string[] str = new string[4];
            /**
            str[0] = item.SubItems[0].Text;
            str[1] = item.SubItems[1].Text;
            str[2] = item.SubItems[2].Text;
            */

            for (int i = 0; i < todoList.Count; i++)
            {
                if (Convert.ToDateTime(todoList[i][0]) == Convert.ToDateTime(item.SubItems[0].Text))
                {
                    str[0] = Convert.ToString( todoList[i][0] );
                    str[1] = Convert.ToString( todoList[i][1] );
                    str[2] = Convert.ToString( todoList[i][2] );
                    str[3] = Convert.ToString(i + 1);
                    break;
                }
            }

           

            int selectRepeat = 0;
            if ( str[2] == "daily" )
            {
                selectRepeat = 1;
            }

           
        }

        /**
         * Функция изменения задач
         */
        public void updateTask(int nom, ArrayList task)
        {
            //  Обновляем в списке
            //todoList.RemoveAt(nom);
            //todoList.Add(task);
            todoList[nom] = task;
            repainListView(DateTime.Today);
            updated = true;
        }

        /**
         * Функция добавления задач
         */
        public void addTask(ArrayList task)
        {
            todoList.Add(task);
            updated = true;
        }

        /**
         * Функция сохранения задач
         */
        public void saveTasks(List<ArrayList> tasksList, string fileName) {
            string stringToWrite = "";
            StreamWriter writer = new StreamWriter(@fileName);
            try
            {
                for (int i = 0; i < tasksList.Count; i++)
                {
                    stringToWrite = Convert.ToString(tasksList[i][0]) + "&" +
                    Convert.ToString(tasksList[i][1]) + "&" +
                    Convert.ToString(tasksList[i][2]);

                    writer.WriteLine(stringToWrite);
                }
                updated = false;
            }
            catch (IOException e)
            {
                MessageBox.Show("Ошибка записи данных в файл. Программа будет закрыта.\n" + e.Message);
            }
            writer.Close();
        }

        /**
         * Изменение даты в календаре
         */
        private void tasksCalendar_DateChanged(object sender, DateRangeEventArgs e)
        {
        }

        /**
         * Получаем ближайшую активную задачу
         */
        private void getNextTask()
        {
            int nom = this.nextTask;
            if ( todoList.Count == 0 ) {
                nextTask = -1;
                return;
            }
            if (todoList.Count == 1)
            {
                nextTask = 0;
                return;
            }

            /**
             * Ищем минимальное значение даты-времени
             */
            for (int i = 0; i < todoList.Count - 1; i++)
            { 
                if (Convert.ToDateTime(todoList[i][0]) < Convert.ToDateTime(todoList[nom][0]))
                {
                    nom = i + 1;
                }
            }
            this.nextTask = nom;
        }

        /**
         * Удаление задачи из активных в выполненные
         */
        protected void removeTask(int nom)
        {
            if (todoList.Count > 0)
            {
                timer1.Stop();
                doneList.Add(todoList[nom]);
                todoList.RemoveAt(nom);
                repainListView(DateTime.Now);
                timer1.Start();
            }
            else
            {
                /**
                 * Здесь можно определить 
                 * какой-либо дополнительный функционал программы
                 * при случае, если задач больше нет.
                 * 
                 * Мы остановим отсчет времени.
                 */
                timer1.Stop();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (nextTask != -1)
            {
                /**
                 * Ведем обратный отсчет
                 */
                int comparition = DateTime.Compare( DateTime.Now, Convert.ToDateTime(todoList[nextTask][0]));
                if ( comparition > 0)
                {
                    showMessage();
                }
            }
        }

        /**
         * Вывод сообщения о событии
         */

        private void showMessage()
        {
            AlarmForm alarm = new AlarmForm();
            alarm.label1.Text = Convert.ToString(todoList[nextTask][1]);
            if ( Convert.ToString(todoList[nextTask][2]) == "daily" )
            {
                DateTime nextDay = Convert.ToDateTime(todoList[nextTask][0]).AddDays(1);
                ArrayList al = new ArrayList( );
                al.Add(nextDay);
                al.Add(todoList[nextTask][1]);
                al.Add(todoList[nextTask][2]);
                removeTask(nextTask);
                todoList.Add(al);
            }
            else
            {
                removeTask(nextTask);
            }
            updated = true;
            alarm.Show();
            getNextTask();
        }

        
        /**
         * При изменении даты в календаре
         * Прорисовываем задачи данного дня.
         */
        private void tasksCalendar_DateSelected( object sender, DateRangeEventArgs e )
        {
            if ( updated )
            {
                saveTasks(todoList, settings.todoFile);
                updated = false;
            }

            repainListView(tasksCalendar.SelectionEnd);
        }

        /**
         * Перед закрытием формы спрашиваем, нужно ли сохранить изменения.
         */
        private void OrganizerForm_FormClosing( object sender, FormClosingEventArgs e )
        {
            if ( updated == true )
            {
                DialogResult dr = MessageBox.Show("Данные были изменены. Сохранить?", "Закрытие программы.", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if ( dr == DialogResult.Yes )
                {
                    saveTasks(todoList, settings.todoFile);
                    saveTasks(doneList, settings.doneFile);
                    updated = false;
                }
            }
        }

        

        /**
         * Удаление задачи
         */

        private void toolStripMenuItem5_Click( object sender, EventArgs e )
        {
            ListViewItem item = new ListViewItem( );
            item = listView1.SelectedItems[0];
            string[] str = new string[1];
            /**
            str[0] = item.SubItems[0].Text;
            str[1] = item.SubItems[1].Text;
            str[2] = item.SubItems[2].Text;
            */

            for ( int i = 0; i < todoList.Count; i++ )
            {
                if ( Convert.ToDateTime(todoList[i][0]) == Convert.ToDateTime(item.SubItems[0].Text) )
                {
                    str[0] = Convert.ToString(todoList[i][0]);
                    removeTask(i);
                    break;
                }
            }
            updated = true;
            getNextTask( );
            repainListView(tasksCalendar.SelectionEnd);
        }

        /**
         * Открываем форму с выполненными задачами за день, указанный в календаре
         */
        private void button1_Click( object sender, EventArgs e )
        {
            doneListForm doneForm = new doneListForm( );
            doneForm.repainListView(tasksCalendar.SelectionEnd.Date, doneList);
            doneForm.Show( );
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void tasksCalendar_DateChanged_1(object sender, DateRangeEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
              AddForm form2 = new AddForm();
            form2.setOrganizerForm(this);
            form2.Show();
        
        }

    }
}
