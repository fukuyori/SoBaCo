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

    public struct TextFont {
        public string Text { get; }
        public Font Font { get; }

        public TextFont(string s, Font fon) {
            Text = s;
            Font = fon;
        }
    }

    public partial class ChartPaint : Form {

        private Graphics Gra;
        private Bitmap BitmapChart;
        private Bitmap Buffer;
        // マウスダウンフラグ
        private bool IsMouseDown = false;
        // マウスをクリックした位置の保持用
        private PointF OldPoint;
        // アフィン変換行列
        private System.Drawing.Drawing2D.Matrix DrawMatrix;

        enum Tools { None, Pencil, Line, Circle, Square, SquareFill, Text, Eraser };
        Tools Tool;

        int PictureWidth;
        int PictureHeight;
        private Color PenColor;
        private int LineWidth;
        private Pen DrawPen;
        private SolidBrush DrawBrush;

        bool IsDrawing = false;  // true = 描画中
        int DrawStartX;     // Line X 起点
        int DrawStartY;     // Line Y 起点

        public ChartPaint(Bitmap _bitmap) {
            InitializeComponent();

            BitmapChart = new Bitmap(_bitmap);
            //Canvas = _bitmap;
            PictureWidth = _bitmap.Width;
            PictureHeight = _bitmap.Height;

            // ホイールイベントの追加
            this.pictureBox1.MouseWheel
                += new System.Windows.Forms.MouseEventHandler(this.PictureBox1_MouseWheel);
            // リサイズイベントを強制的に実行（Graphicsオブジェクトの作成のため）
            ChartPaint_Resize(null, null);
        }

        private void ChartPaint_Load(object sender, EventArgs e) {

            this.Width = PictureWidth + 40;
            this.Height = PictureHeight + 110;

            Tool = Tools.None;
            PenColor = Color.Black;
            LineWidth = 3;
            DrawPen = new Pen(PenColor, LineWidth);
            DrawBrush = new SolidBrush(PenColor);


            bUndo.Enabled = false;
            bRedo.Enabled = false;

            // Graphics オブジェクトの取得
            pictureBox1.Image = new Bitmap(PictureWidth, PictureHeight);
            Gra = Graphics.FromImage(pictureBox1.Image);
            Gra.DrawImage(BitmapChart, 0, 0);

            // アフィン変換行列の初期化
            if (DrawMatrix != null) {
                DrawMatrix.Dispose();
            }
            DrawMatrix = new System.Drawing.Drawing2D.Matrix();

            // 画像の描画
            DrawImage();

            Buffer = new Bitmap(pictureBox1.Image);

            bNone.BackColor = SystemColors.Highlight;
        }

        private void MnuFileExit_Click(object sender, EventArgs e) {
            // 終了
            this.Close();
        }

        private void ChartPaint_Resize(object sender, EventArgs e) {
            if (Gra != null) {
                DrawMatrix = Gra.Transform;
                Gra.Dispose();
                Gra = null;
            }

            // PictureBoxと同じ大きさのBitmapクラスを作成する。
            Bitmap _bmpPicBox = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            // 空のBitmapをPictureBoxのImageに指定する。
            pictureBox1.Image = _bmpPicBox;
            // Graphicsオブジェクトの作成(FromImageを使う)
            Gra = Graphics.FromImage(pictureBox1.Image);
            // アフィン変換行列の設定
            if (DrawMatrix != null) {
                Gra.Transform = DrawMatrix;
            }

            // 補間モードの設定（このサンプルではNearestNeighborに設定）
            Gra.InterpolationMode
                = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            // 画像の描画
            DrawImage();
        }

        // ビットマップの描画
        private void DrawImage() {
            if (BitmapChart == null) return;
            // アフィン変換行列の設定
            if ((DrawMatrix != null)) {
                Gra.Transform = DrawMatrix;
            }
            // ピクチャボックスのクリア
            Gra.Clear(pictureBox1.BackColor);
            // 描画
            Gra.DrawImage(BitmapChart, 0, 0);
            // 再描画
            pictureBox1.Refresh();

            pictureBox1.Focus();
        }


        private void ToolStripButton1_Click(object sender, EventArgs e) {
            PrintProc();
        }


        private void 印刷ToolStripMenuItem_Click(object sender, EventArgs e) {
            PrintProc();
        }

        private void PrintProc() {
            printDocument1.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(Pd_PrintPage);
            printDocument1.DefaultPageSettings.Landscape = true;
            // 余白設定 左上右下
            System.Drawing.Printing.Margins margins = new System.Drawing.Printing.Margins(25, 25, 100, 25);
            printDocument1.DefaultPageSettings.Margins = margins;
            printDialog1.Document = printDocument1;
            if (printDialog1.ShowDialog() == DialogResult.OK) {
                printDocument1.Print();
            }
        }

        /// <summary>
        /// 印刷
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Pd_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e) {
            Bitmap bmpChart = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.DrawToBitmap(bmpChart, new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height));
            e.Graphics.DrawImage(bmpChart, e.MarginBounds);
            e.HasMorePages = false;
            bmpChart.Dispose();
        }

        /// <summary>
        /// カラーピッカー
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnColor_Click(object sender, EventArgs e) {
            //ColorDialogクラスのインスタンスを作成
            ColorDialog chartDialog = new ColorDialog() {
                //はじめに選択されている色を設定
                Color = (Color)PenColor,
                //色の作成部分を表示可能にする
                //デフォルトがTrueのため必要はない
                AllowFullOpen = true,
                //純色だけに制限しない
                //デフォルトがFalseのため必要はない
                SolidColorOnly = false,
                //[作成した色]に指定した色（RGB値）を表示する
                CustomColors = new int[] {
                ColorTranslator.ToWin32(Color.Black),
                ColorTranslator.ToWin32(Color.Red),
                ColorTranslator.ToWin32(Color.Green),
                ColorTranslator.ToWin32(Color.Blue),
                ColorTranslator.ToWin32(Color.Purple),
                ColorTranslator.ToWin32(Color.Orange) }
            };

            //ダイアログを表示する
            if (chartDialog.ShowDialog() == DialogResult.OK) {
                //選択された色の取得
                PenColor = chartDialog.Color;
                DrawPen = new Pen(PenColor, LineWidth);
                DrawBrush = new SolidBrush(PenColor);
                if (PenColor == Color.Black)
                    bColor.BackColor = SystemColors.Control;
                else
                    bColor.BackColor = PenColor;
            }
        }

        private void PictureBox1_MouseDown(object sender, MouseEventArgs e) {

            // 右ボタンがクリックされたとき
            if (e.Button == System.Windows.Forms.MouseButtons.Right) {
                // アフィン変換行列に単位行列を設定する
                DrawMatrix.Reset();
                // 画像の描画
                DrawImage();

                return;
            }
            if (Tool == Tools.None) {
                // フォーカスの設定
                //（クリックしただけではMouseWheelイベントが有効にならない）
                pictureBox1.Focus();
                // マウスをクリックした位置の記録
                OldPoint.X = e.X;
                OldPoint.Y = e.Y;
                // マウスダウンフラグ
                IsMouseDown = true;
                return;
            }

            UndoStackPush();
            RedoStackClear();

            switch (Tool) {
                case Tools.Pencil:
                    IsDrawing = true;
                    DrawStartX = e.X;
                    DrawStartY = e.Y;
                    break;
                case Tools.Eraser:
                    IsDrawing = true;
                    DrawStartX = e.X;
                    DrawStartY = e.Y;
                    break;
                case Tools.Line:
                    IsDrawing = true;
                    DrawStartX = e.X;
                    DrawStartY = e.Y;
                    Buffer = new Bitmap(pictureBox1.Image);
                    break;
                case Tools.Circle:
                    IsDrawing = true;
                    DrawStartX = e.X;
                    DrawStartY = e.Y;
                    Buffer = new Bitmap(pictureBox1.Image);
                    break;
                case Tools.Square:
                    IsDrawing = true;
                    DrawStartX = e.X;
                    DrawStartY = e.Y;
                    Buffer = new Bitmap(pictureBox1.Image);
                    break;
                case Tools.SquareFill:
                    IsDrawing = true;
                    DrawStartX = e.X;
                    DrawStartY = e.Y;
                    Buffer = new Bitmap(pictureBox1.Image);
                    break;
                case Tools.Text:
                    IsDrawing = true;
                    DrawStartX = e.X;
                    DrawStartY = e.Y;
                    break;
            }
        }

        private void PictureBox1_MouseUp(object sender, MouseEventArgs e) {
            switch (Tool) {
                case Tools.Pencil:
                    IsDrawing = false;
                    break;
                case Tools.Eraser:
                    IsDrawing = false;
                    break;
                case Tools.Line:
                    Gra.DrawImage(Buffer, 0, 0);
                    Gra.DrawLine(DrawPen, DrawStartX, DrawStartY, e.X, e.Y);
                    pictureBox1.Refresh();
                    IsDrawing = false;
                    break;
                case Tools.Circle:
                    Gra.DrawImage(Buffer, 0, 0);
                    Gra.DrawEllipse(DrawPen, DrawStartX, DrawStartY, Math.Abs(e.X - DrawStartX), Math.Abs(e.Y - DrawStartY));
                    pictureBox1.Refresh();
                    IsDrawing = false;
                    break;
                case Tools.Square:
                    Gra.DrawImage(Buffer, 0, 0);
                    Gra.DrawRectangle(DrawPen, DrawStartX, DrawStartY, Math.Abs(e.X - DrawStartX), Math.Abs(e.Y - DrawStartY));
                    pictureBox1.Refresh();
                    IsDrawing = false;
                    break;
                case Tools.SquareFill:
                    Gra.DrawImage(Buffer, 0, 0);
                    Gra.FillRectangle(DrawBrush, DrawStartX, DrawStartY, Math.Abs(e.X - DrawStartX), Math.Abs(e.Y - DrawStartY));
                    pictureBox1.Refresh();
                    IsDrawing = false;
                    break;
                case Tools.Text:
                    InputText _inputText = new InputText();
                    if (System.Windows.Forms.DialogResult.OK == _inputText.ShowDialog()) {
                        TextFont _textFont = _inputText.myTextFont;
                        Gra.DrawString(_textFont.Text, _textFont.Font, DrawBrush, DrawStartX, DrawStartY);
                        pictureBox1.Refresh();
                    }
                    _inputText.Dispose();
                    IsDrawing = false;
                    break;
                default:
                    // マウスダウンフラグ
                    IsMouseDown = false;
                    Gra.DrawImage(Buffer, 0, 0);
                    break;
            }
        }

        private void PictureBox1_MouseMove(object sender, MouseEventArgs e) {
            // マウスをクリックしながら移動中のとき
            if (IsMouseDown) {

                var _cursor = this.Cursor;
                this.Cursor = Cursors.SizeAll;

                // 画像の移動
                DrawMatrix.Translate(e.X - OldPoint.X, e.Y - OldPoint.Y,
                    System.Drawing.Drawing2D.MatrixOrder.Append);
                // 画像の描画
                DrawImage();

                // ポインタ位置の保持
                OldPoint.X = e.X;
                OldPoint.Y = e.Y;

                this.Cursor = _cursor;
            } else {
                switch (Tool) {
                    case Tools.Pencil:
                        if (!IsDrawing) return;
                        Gra.DrawLine(DrawPen, DrawStartX, DrawStartY, e.X, e.Y);
                        DrawStartX = e.X;
                        DrawStartY = e.Y;
                        pictureBox1.Refresh();
                        break;
                    case Tools.Eraser:
                        if (!IsDrawing) return;
                        Gra.DrawLine(ErasePen, DrawStartX, DrawStartY, e.X, e.Y);
                        DrawStartX = e.X;
                        DrawStartY = e.Y;
                        pictureBox1.Refresh();
                        break;
                    case Tools.Line:
                        if (!IsDrawing) return;
                        Gra.DrawImage(Buffer, 0, 0);
                        Gra.DrawLine(DrawPen, DrawStartX, DrawStartY, e.X, e.Y);
                        pictureBox1.Refresh();
                        break;
                    case Tools.Circle:
                        if (!IsDrawing) return;
                        Gra.DrawImage(Buffer, 0, 0);
                        Gra.DrawEllipse(DrawPen, DrawStartX, DrawStartY, Math.Abs(e.X - DrawStartX), Math.Abs(e.Y - DrawStartY));
                        pictureBox1.Refresh();
                        break;
                    case Tools.Square:
                        if (!IsDrawing) return;
                        Gra.DrawImage(Buffer, 0, 0);
                        Gra.DrawRectangle(DrawPen, DrawStartX, DrawStartY, Math.Abs(e.X - DrawStartX), Math.Abs(e.Y - DrawStartY));
                        pictureBox1.Refresh();
                        break;
                    case Tools.SquareFill:
                        if (!IsDrawing) return;
                        Gra.DrawImage(Buffer, 0, 0);
                        Gra.FillRectangle(DrawBrush, DrawStartX, DrawStartY, Math.Abs(e.X - DrawStartX), Math.Abs(e.Y - DrawStartY));
                        pictureBox1.Refresh();
                        break;
                }
            }
        }

        // マウスホイールイベント
        private void PictureBox1_MouseWheel(object sender, MouseEventArgs e) {
            var _cursor = this.Cursor;
            this.Cursor = Cursors.SizeAll;

            // ポインタの位置→原点へ移動
            DrawMatrix.Translate(-e.X, -e.Y,
                System.Drawing.Drawing2D.MatrixOrder.Append);
            if (e.Delta > 0) {
                // 拡大
                if (DrawMatrix.Elements[0] < 100)  // X方向の倍率を代表してチェック
                {
                    DrawMatrix.Scale(1.5f, 1.5f,
                        System.Drawing.Drawing2D.MatrixOrder.Append);
                }
            } else {
                // 縮小
                if (DrawMatrix.Elements[0] > 0.01)  // X方向の倍率を代表してチェック
                {
                    DrawMatrix.Scale(1.0f / 1.5f, 1.0f / 1.5f,
                        System.Drawing.Drawing2D.MatrixOrder.Append);
                }
            }
            // 原点→ポインタの位置へ移動(元の位置へ戻す)
            DrawMatrix.Translate(e.X, e.Y,
                System.Drawing.Drawing2D.MatrixOrder.Append);
            // 画像の描画
            DrawImage();

            this.Cursor = _cursor;
        }

        private void BtnNone_Click(object sender, EventArgs e) {
            ChangeButton(Tools.None);
            this.pictureBox1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.PictureBox1_MouseWheel);
        }

        private void BtnPen_Click(object sender, EventArgs e) {
            ChangeButton(Tools.Pencil);
            this.pictureBox1.MouseWheel -= new System.Windows.Forms.MouseEventHandler(this.PictureBox1_MouseWheel);
        }

        private void BtnLine_Click(object sender, EventArgs e) {
            ChangeButton(Tools.Line);
            this.pictureBox1.MouseWheel -= new System.Windows.Forms.MouseEventHandler(this.PictureBox1_MouseWheel);
        }

        private void BtnCircle_Click(object sender, EventArgs e) {
            ChangeButton(Tools.Circle);
            this.pictureBox1.MouseWheel -= new System.Windows.Forms.MouseEventHandler(this.PictureBox1_MouseWheel);
        }

        private void bSquare_Click(object sender, EventArgs e) {
            ChangeButton(Tools.Square);
            this.pictureBox1.MouseWheel -= new System.Windows.Forms.MouseEventHandler(this.PictureBox1_MouseWheel);
        }

        private void bSquareFill_Click(object sender, EventArgs e) {
            ChangeButton(Tools.SquareFill);
            this.pictureBox1.MouseWheel -= new System.Windows.Forms.MouseEventHandler(this.PictureBox1_MouseWheel);
        }

        private Pen ErasePen = new Pen(Color.White, 20);

        private void BtnEraser_Click(object sender, EventArgs e) {
            ChangeButton(Tools.Eraser);
            this.pictureBox1.MouseWheel -= new System.Windows.Forms.MouseEventHandler(this.PictureBox1_MouseWheel);
        }

        private void BtnText_Click(object sender, EventArgs e) {
            ChangeButton(Tools.Text);
            this.pictureBox1.MouseWheel -= new System.Windows.Forms.MouseEventHandler(this.PictureBox1_MouseWheel);
        }

        private void ChangeButton(Tools t) {
            switch (Tool) {
                case Tools.Pencil:
                    bPen.BackColor = SystemColors.Control;
                    break;
                case Tools.Line:
                    bLine.BackColor = SystemColors.Control;
                    break;
                case Tools.Circle:
                    bCircle.BackColor = SystemColors.Control;
                    break;
                case Tools.Square:
                    bSquare.BackColor = SystemColors.Control;
                    break;
                case Tools.SquareFill:
                    bSquareFill.BackColor = SystemColors.Control;
                    break;
                case Tools.Eraser:
                    bEraser.BackColor = SystemColors.Control;
                    break;
                case Tools.Text:
                    bText.BackColor = SystemColors.Control;
                    break;
                case Tools.None:
                    DrawMatrix.Reset();
                    DrawImage();
                    bNone.BackColor = SystemColors.Control;
                    break;
            }
            switch (t) {
                case Tools.Pencil:
                    bPen.BackColor = SystemColors.Highlight;
                    break;
                case Tools.Line:
                    bLine.BackColor = SystemColors.Highlight;
                    break;
                case Tools.Circle:
                    bCircle.BackColor = SystemColors.Highlight;
                    break;
                case Tools.Square:
                    bSquare.BackColor = SystemColors.Highlight;
                    break;
                case Tools.SquareFill:
                    bSquareFill.BackColor = SystemColors.Highlight;
                    break;
                case Tools.Eraser:
                    bEraser.BackColor = SystemColors.Highlight;
                    break;
                case Tools.Text:
                    bText.BackColor = SystemColors.Highlight;
                    break;
                case Tools.None:
                    BitmapChart = new Bitmap(pictureBox1.Image);
                    bNone.BackColor = SystemColors.Highlight;
                    pictureBox1.Focus();
                    break;
            }
            Tool = t;
        }

        private void 極細ToolStripMenuItem_Click(object sender, EventArgs e) {
            LineWidth = 1;
            DrawPen = new Pen(PenColor, LineWidth);
            bWidth.Image = sobaco.Properties.Resources.w1;
        }

        private void 細ToolStripMenuItem_Click(object sender, EventArgs e) {
            LineWidth = 2;
            DrawPen = new Pen(PenColor, LineWidth);
            bWidth.Image = sobaco.Properties.Resources.w2;
        }

        private void 中ToolStripMenuItem_Click(object sender, EventArgs e) {
            LineWidth = 3;
            DrawPen = new Pen(PenColor, LineWidth);
            bWidth.Image = sobaco.Properties.Resources.w3;
        }

        private void 太ToolStripMenuItem_Click(object sender, EventArgs e) {
            LineWidth = 5;
            DrawPen = new Pen(PenColor, LineWidth);
            bWidth.Image = sobaco.Properties.Resources.w4;
        }

        private void 極太ToolStripMenuItem_Click(object sender, EventArgs e) {
            LineWidth = 8;
            DrawPen = new Pen(PenColor, LineWidth);
            bWidth.Image = sobaco.Properties.Resources.w5;
        }


        /// <summary>
        /// Undo Redo
        /// </summary>
        private Stack<Bitmap> _undoStack = new Stack<Bitmap>();
        private Stack<Bitmap> _redoStack = new Stack<Bitmap>();

        private readonly object _undoRedoLocker = new object();

        private void Undo() {
            lock (_undoRedoLocker) {
                if (_undoStack.Count > 0) {
                    RedoStackPush();
                    bRedo.Enabled = true;

                    //_redoStack.Push(_undoStack.Pop());

                    //OnUndo();
                    Gra.DrawImage(_undoStack.Pop(), 0, 0);
                    pictureBox1.Refresh();
                }
                if (_undoStack.Count == 0)
                    bUndo.Enabled = false;
            }
        }

        private void Redo() {
            lock (_undoRedoLocker) {
                if (_redoStack.Count > 0) {
                    UndoStackPush();
                    bUndo.Enabled = true;

                    //_undoStack.Push(_redoStack.Pop());

                    //OnRedo();
                    Gra.DrawImage(_redoStack.Pop(), 0, 0);
                    pictureBox1.Refresh();
                }
                if (_redoStack.Count == 0)
                    bRedo.Enabled = false;
            }
        }

        private void StackPeek() {
            lock (_undoRedoLocker) {
                if (_undoStack.Count > 0) {
                    Gra.DrawImage(_undoStack.Peek(), 0, 0);
                    pictureBox1.Refresh();
                }
            }
        }

        private void UndoStackPush() {
            lock (_undoRedoLocker) {
                _undoStack.Push(new Bitmap(pictureBox1.Image));//image);
            }
            bUndo.Enabled = true;
        }
        private void RedoStackPush() {
            lock (_undoRedoLocker) {
                _redoStack.Push(new Bitmap(pictureBox1.Image));//image);
            }
            bUndo.Enabled = true;
        }
        private void RedoStackClear() {
            _redoStack.Clear();
            bRedo.Enabled = false;
        }

        private void UpdateImageData(Action updateImage) {
            lock (_undoRedoLocker) {
                _undoStack.Push(new Bitmap(pictureBox1.Image));//image);

                try {
                    updateImage();
                } catch {
                    _undoStack.Pop();//because of exception remove the last added frame from stack
                    throw;
                }
            }
        }

        private void BtnUndo_Click(object sender, EventArgs e) {
            Undo();
        }

        private void BtnRedo_Click(object sender, EventArgs e) {
            Redo();
        }

        /// <summary>
        /// ペンの色を変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnBlack_Click(object sender, EventArgs e) {
            PenColor = Color.Black;
            DrawPen = new Pen(PenColor, LineWidth);
            DrawBrush = new SolidBrush(PenColor);
            bColor.BackColor = SystemColors.Control;
        }

        private void BtnRed_Click(object sender, EventArgs e) {
            PenColor = Color.Red;
            DrawPen = new Pen(PenColor, LineWidth);
            DrawBrush = new SolidBrush(PenColor);
            bColor.BackColor = Color.Red;
        }

        private void BtnGreen_Click(object sender, EventArgs e) {
            PenColor = Color.Green;
            DrawPen = new Pen(PenColor, LineWidth);
            DrawBrush = new SolidBrush(PenColor);
            bColor.BackColor = Color.Green;
        }

        private void BtnBlue_Click(object sender, EventArgs e) {
            PenColor = Color.Blue;
            DrawPen = new Pen(PenColor, LineWidth);
            DrawBrush = new SolidBrush(PenColor);
            bColor.BackColor = Color.Blue;
        }

        private void BtnWhite_Click(object sender, EventArgs e) {
            PenColor = Color.White;
            DrawPen = new Pen(PenColor, LineWidth);
            DrawBrush = new SolidBrush(PenColor);
            bColor.BackColor = Color.White;
        }

        private void BtnYellow_Click(object sender, EventArgs e) {
            PenColor = Color.Yellow;
            DrawPen = new Pen(PenColor, LineWidth);
            DrawBrush = new SolidBrush(PenColor);
            bColor.BackColor = Color.Yellow;
        }

        /// <summary>
        /// ショートカットキー
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChartPaint_KeyDown(object sender, KeyEventArgs e) {
            switch (e.KeyCode) {
                case Keys.Z:
                    Undo();
                    break;
                case Keys.Y:
                    Redo();
                    break;
            }
        }
    }
}
