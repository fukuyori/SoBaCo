using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace sobaco {
    public partial class SetHanbunForm : Form {

        public double[] Point = new double[4];
        public List<List<int>> Idouheikins = new List<List<int>>();

        public SetHanbunForm(double[] _point, List<List<int>> _idouheikins) {
            InitializeComponent();

            trackBar1.Value = (int)_point[0];
            trackBar2.Value = (int)_point[1];
            trackBar3.Value = (int)_point[2];
            trackBar4.Value = (int)_point[3];

            Idouheikins = _idouheikins;
        }

        private void Form1_Load(object sender, EventArgs e) {
            ReDraw();

            daily1.Value = Idouheikins[0][0];
            daily2.Value = Idouheikins[0][1];
            daily3.Value = Idouheikins[0][2];
            daily4.Value = Idouheikins[0][3];
            daily5.Value = Idouheikins[0][4];
            weekly1.Value = Idouheikins[1][0];
            weekly2.Value = Idouheikins[1][1];
            weekly3.Value = Idouheikins[1][2];
            weekly4.Value = Idouheikins[1][3];
            weekly5.Value = Idouheikins[1][4];
            monthly1.Value = Idouheikins[2][0];
            monthly2.Value = Idouheikins[2][1];
            monthly3.Value = Idouheikins[2][2];
            monthly4.Value = Idouheikins[2][3];
            monthly5.Value = Idouheikins[2][4];
        }

        private void ReDraw() {

            label1.Text = $"{trackBar1.Value,3}%";
            label2.Text = $"{trackBar2.Value,3}%";
            label3.Text = $"{trackBar3.Value,3}%";
            label4.Text = $"{trackBar4.Value,3}%";

            //描画先とするImageオブジェクトを作成する
            Bitmap canvas = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            //ImageオブジェクトのGraphicsオブジェクトを作成する
            Graphics g = Graphics.FromImage(canvas);
            //Penオブジェクトの作成(幅1の黒色)
            //(この場合はPenを作成せずに、Pens.Blackを使っても良い)
            Pen p = new Pen(Color.Black, 2);
            //位置(10, 20)に100x80の長方形を描く
            g.DrawRectangle(p, 10, 2, 20, 200);
            p.Dispose();
            //Brushオブジェクトの作成
            SolidBrush b = new SolidBrush(Color.DarkGray);
            g.FillRectangle(b, 50, 2, 20, 200);

            p = new Pen(Color.Red, 4);
            g.DrawLine(p, 2, 102, 38, 102);
            g.DrawLine(p, 42, 102, 80, 102);

            p = new Pen(Color.Blue, 2);
            g.DrawLine(p, 2, 202 - trackBar1.Value * 2, 38, 202 - trackBar1.Value * 2);
            g.DrawLine(p, 2, 202 - trackBar2.Value * 2, 38, 202 - trackBar2.Value * 2);
            g.DrawLine(p, 42, 202 - trackBar3.Value * 2, 80, 202 - trackBar3.Value * 2);
            g.DrawLine(p, 42, 202 - trackBar4.Value * 2, 80, 202 - trackBar4.Value * 2);            //g.DrawLine(p, 42, line[2], 80, line[2]);
            //g.DrawLine(p, 42, line[3], 80, line[3]);

            //リソースを解放する
            p.Dispose();
            g.Dispose();

            //PictureBox1に表示する
            pictureBox1.Image = canvas;
        }

        private void TrackBar1_ValueChanged(object sender, EventArgs e) {
            ReDraw();
        }

        private void TrackBar2_ValueChanged(object sender, EventArgs e) {
            ReDraw();
        }

        private void TrackBar3_ValueChanged(object sender, EventArgs e) {
            ReDraw();
        }

        private void TrackBar4_ValueChanged(object sender, EventArgs e) {
            ReDraw();
        }

        private void Button2_Click(object sender, EventArgs e) {
            Point[0] = trackBar1.Value;
            Point[1] = trackBar2.Value;
            Point[2] = trackBar3.Value;
            Point[3] = trackBar4.Value;
            Idouheikins = new List<List<int>>() {
                new List<int>() {
                    (int)daily1.Value,
                    (int)daily2.Value,
                    (int)daily3.Value,
                    (int)daily4.Value,
                    (int)daily5.Value
                },
                new List<int>() {
                    (int)weekly1.Value,
                    (int)weekly2.Value,
                    (int)weekly3.Value,
                    (int)weekly4.Value,
                    (int)weekly5.Value
                },
                new List<int>() {
                    (int)monthly1.Value,
                    (int)monthly2.Value,
                    (int)monthly3.Value,
                    (int)monthly4.Value,
                    (int)monthly5.Value
                }
            };

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void Button1_Click(object sender, EventArgs e) {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
    }
}
