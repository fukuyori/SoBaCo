using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sobaco {

    public partial class CalendarDialog : Form {


        private DateTime MaxDate { get; set; }
        private DateTime MinDate { get; set; }
        public DateTime SelectedDate { get; private set; }
    

        public CalendarDialog(DateTime minDate, DateTime maxDate, DateTime currentDate) {
            InitializeComponent();

            MinDate = minDate;
            MaxDate = maxDate;
            SelectedDate = currentDate;
        }

        private void CalendarDialog_Load(object sender, EventArgs e) {
            this.monthCalendar1.MinDate = this.MinDate;
            this.monthCalendar1.MaxDate = this.MaxDate;
            this.monthCalendar1.SelectionStart = SelectedDate;
            this.monthCalendar1.SelectionEnd = SelectedDate;
        }

        private void MonthCalendar1_DateSelected(object sender, DateRangeEventArgs e) {
            SelectedDate = this.monthCalendar1.SelectionStart;
            this.Close();
        }
    }
}
