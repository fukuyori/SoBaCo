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

    /// <summary>
    /// ChartPaintの文字入力用フォーム
    /// </summary>
    public partial class InputText : Form {
        public string ReturnValue;
        public TextFont myTextFont;

        public InputText() {
            InitializeComponent();
        }

        private void InputText_Load(object sender, EventArgs e) {
            comboBox1.DrawMode = DrawMode.OwnerDrawFixed;
            comboBox1.ItemHeight = 16;
            foreach (FontFamily item in FontFamily.Families) {
                if (item.IsStyleAvailable(FontStyle.Regular)) {
                    comboBox1.Items.Add(item.Name);
                }
            }
            comboBox1.Text = "ＭＳ Ｐゴシック";

            comboBox2.DrawMode = DrawMode.OwnerDrawFixed;
            comboBox2.ItemHeight = 16;
            comboBox2.Items.Add("標準");
            comboBox2.Items.Add("太字");
            comboBox2.Items.Add("斜体");
            comboBox2.SelectedIndex = 0;
        }

        private void ComboBox1_DrawItem(object sender, DrawItemEventArgs e) {
            if (e.Index == -1) return;

            e.Graphics.DrawString(comboBox1.Items[e.Index].ToString(),
                new Font(comboBox1.Items[e.Index].ToString(), 12),
                new SolidBrush(Color.Black),
                e.Bounds.X, e.Bounds.Y);
        }

        private void ComboBox2_DrawItem(object sender, DrawItemEventArgs e) {
            if (e.Index == -1) return;

            e.Graphics.DrawString(comboBox2.Items[e.Index].ToString(),
                new Font("ＭＳ Ｐゴシック", 12, (FontStyle)e.Index),
                new SolidBrush(Color.Black),
                e.Bounds.X, e.Bounds.Y);
        }

        private void Button2_Click(object sender, EventArgs e) {
            //キャンセルボタンが押された時はDialogResult.Cancelを設定する。
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            //ShowDialog()で表示されているので閉じないといけない
            this.Close();
        }

        private void Button1_Click(object sender, EventArgs e) {
            Font myFont = new Font(comboBox1.SelectedText, 
                                (int)numericUpDown1.Value, 
                                (FontStyle)comboBox2.SelectedIndex);
            myTextFont = new TextFont(textBox1.Text, myFont);

            //OKボタンが押された時はDialogResult.OKを設定する。
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            //ShowDialog()で表示されているので閉じないといけない
            this.Close();
        }
    }
}
