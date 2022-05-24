namespace sobaco {
    partial class ChartPaint {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChartPaint));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.印刷ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.bPrint = new System.Windows.Forms.ToolStripButton();
            this.bUndo = new System.Windows.Forms.ToolStripButton();
            this.bRedo = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.bNone = new System.Windows.Forms.ToolStripButton();
            this.bPen = new System.Windows.Forms.ToolStripButton();
            this.bLine = new System.Windows.Forms.ToolStripButton();
            this.bCircle = new System.Windows.Forms.ToolStripButton();
            this.bSquare = new System.Windows.Forms.ToolStripButton();
            this.bSquareFill = new System.Windows.Forms.ToolStripButton();
            this.bText = new System.Windows.Forms.ToolStripButton();
            this.bEraser = new System.Windows.Forms.ToolStripButton();
            this.bColor = new System.Windows.Forms.ToolStripButton();
            this.bBlack = new System.Windows.Forms.ToolStripButton();
            this.bRed = new System.Windows.Forms.ToolStripButton();
            this.bGreen = new System.Windows.Forms.ToolStripButton();
            this.bBlue = new System.Windows.Forms.ToolStripButton();
            this.bYellow = new System.Windows.Forms.ToolStripButton();
            this.bWhite = new System.Windows.Forms.ToolStripButton();
            this.bWidth = new System.Windows.Forms.ToolStripDropDownButton();
            this.極太ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.太ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.中ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.細ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.極細ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printDialog1 = new System.Windows.Forms.PrintDialog();
            this.printDocument1 = new System.Drawing.Printing.PrintDocument();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(631, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.印刷ToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
            this.fileToolStripMenuItem.Text = "ファイル";
            // 
            // 印刷ToolStripMenuItem
            // 
            this.印刷ToolStripMenuItem.Name = "印刷ToolStripMenuItem";
            this.印刷ToolStripMenuItem.Size = new System.Drawing.Size(104, 22);
            this.印刷ToolStripMenuItem.Text = "印刷";
            this.印刷ToolStripMenuItem.Click += new System.EventHandler(this.印刷ToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(101, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(104, 22);
            this.exitToolStripMenuItem.Text = "閉じる";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.MnuFileExit_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 548);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(631, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bPrint,
            this.bUndo,
            this.bRedo,
            this.toolStripSeparator1,
            this.bNone,
            this.bPen,
            this.bLine,
            this.bCircle,
            this.bSquare,
            this.bSquareFill,
            this.bText,
            this.bEraser,
            this.bColor,
            this.bBlack,
            this.bRed,
            this.bGreen,
            this.bBlue,
            this.bYellow,
            this.bWhite,
            this.bWidth});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(631, 40);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // bPrint
            // 
            this.bPrint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bPrint.Image = global::sobaco.Properties.Resources.FilePrint;
            this.bPrint.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bPrint.Name = "bPrint";
            this.bPrint.Size = new System.Drawing.Size(23, 37);
            this.bPrint.Text = "印刷";
            this.bPrint.Click += new System.EventHandler(this.ToolStripButton1_Click);
            // 
            // bUndo
            // 
            this.bUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bUndo.Image = global::sobaco.Properties.Resources.undo;
            this.bUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bUndo.Name = "bUndo";
            this.bUndo.Size = new System.Drawing.Size(23, 37);
            this.bUndo.Text = "元に戻る";
            this.bUndo.Click += new System.EventHandler(this.BtnUndo_Click);
            // 
            // bRedo
            // 
            this.bRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bRedo.Image = global::sobaco.Properties.Resources.redo;
            this.bRedo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bRedo.Name = "bRedo";
            this.bRedo.Size = new System.Drawing.Size(23, 37);
            this.bRedo.Text = "やり直し";
            this.bRedo.Click += new System.EventHandler(this.BtnRedo_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 40);
            // 
            // bNone
            // 
            this.bNone.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bNone.Image = global::sobaco.Properties.Resources.hand;
            this.bNone.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bNone.Name = "bNone";
            this.bNone.Size = new System.Drawing.Size(23, 37);
            this.bNone.Text = "拡大・縮小";
            this.bNone.Click += new System.EventHandler(this.BtnNone_Click);
            // 
            // bPen
            // 
            this.bPen.BackColor = System.Drawing.SystemColors.Control;
            this.bPen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bPen.Image = global::sobaco.Properties.Resources.Edit_grey_64x;
            this.bPen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bPen.Name = "bPen";
            this.bPen.Size = new System.Drawing.Size(23, 37);
            this.bPen.Text = "鉛筆";
            this.bPen.Click += new System.EventHandler(this.BtnPen_Click);
            // 
            // bLine
            // 
            this.bLine.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bLine.Image = global::sobaco.Properties.Resources.Line;
            this.bLine.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bLine.Name = "bLine";
            this.bLine.Size = new System.Drawing.Size(23, 37);
            this.bLine.Text = "直線";
            this.bLine.Click += new System.EventHandler(this.BtnLine_Click);
            // 
            // bCircle
            // 
            this.bCircle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bCircle.Image = global::sobaco.Properties.Resources.circle;
            this.bCircle.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bCircle.Name = "bCircle";
            this.bCircle.Size = new System.Drawing.Size(23, 37);
            this.bCircle.Text = "楕円";
            this.bCircle.Click += new System.EventHandler(this.BtnCircle_Click);
            // 
            // bSquare
            // 
            this.bSquare.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bSquare.Image = global::sobaco.Properties.Resources.square;
            this.bSquare.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bSquare.Name = "bSquare";
            this.bSquare.Size = new System.Drawing.Size(23, 37);
            this.bSquare.Text = "長方形";
            this.bSquare.Click += new System.EventHandler(this.BSquare_Click);
            // 
            // bSquareFill
            // 
            this.bSquareFill.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bSquareFill.Image = global::sobaco.Properties.Resources.squarefill;
            this.bSquareFill.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bSquareFill.Name = "bSquareFill";
            this.bSquareFill.Size = new System.Drawing.Size(23, 37);
            this.bSquareFill.Text = "塗りつぶし長方形";
            this.bSquareFill.Click += new System.EventHandler(this.BSquareFill_Click);
            // 
            // bText
            // 
            this.bText.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bText.Image = global::sobaco.Properties.Resources.letter_512;
            this.bText.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bText.Name = "bText";
            this.bText.Size = new System.Drawing.Size(23, 37);
            this.bText.Text = "文字";
            this.bText.Click += new System.EventHandler(this.BtnText_Click);
            // 
            // bEraser
            // 
            this.bEraser.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bEraser.Image = global::sobaco.Properties.Resources.eraser;
            this.bEraser.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bEraser.Name = "bEraser";
            this.bEraser.Size = new System.Drawing.Size(23, 37);
            this.bEraser.Text = "消しゴム";
            this.bEraser.Click += new System.EventHandler(this.BtnEraser_Click);
            // 
            // bColor
            // 
            this.bColor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bColor.Image = global::sobaco.Properties.Resources.ColorDialog_64x;
            this.bColor.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bColor.Name = "bColor";
            this.bColor.Size = new System.Drawing.Size(23, 37);
            this.bColor.Text = "色選択";
            this.bColor.Click += new System.EventHandler(this.BtnColor_Click);
            // 
            // bBlack
            // 
            this.bBlack.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bBlack.Image = global::sobaco.Properties.Resources.Black;
            this.bBlack.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bBlack.Name = "bBlack";
            this.bBlack.Size = new System.Drawing.Size(23, 37);
            this.bBlack.Text = "黒";
            this.bBlack.Click += new System.EventHandler(this.BtnBlack_Click);
            // 
            // bRed
            // 
            this.bRed.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bRed.Image = global::sobaco.Properties.Resources.Red;
            this.bRed.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bRed.Name = "bRed";
            this.bRed.Size = new System.Drawing.Size(23, 37);
            this.bRed.Text = "赤";
            this.bRed.Click += new System.EventHandler(this.BtnRed_Click);
            // 
            // bGreen
            // 
            this.bGreen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bGreen.Image = global::sobaco.Properties.Resources.Green;
            this.bGreen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bGreen.Name = "bGreen";
            this.bGreen.Size = new System.Drawing.Size(23, 37);
            this.bGreen.Text = "緑";
            this.bGreen.Click += new System.EventHandler(this.BtnGreen_Click);
            // 
            // bBlue
            // 
            this.bBlue.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bBlue.Image = global::sobaco.Properties.Resources.Blue;
            this.bBlue.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bBlue.Name = "bBlue";
            this.bBlue.Size = new System.Drawing.Size(23, 37);
            this.bBlue.Text = "青";
            this.bBlue.Click += new System.EventHandler(this.BtnBlue_Click);
            // 
            // bYellow
            // 
            this.bYellow.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bYellow.Image = global::sobaco.Properties.Resources.Orange;
            this.bYellow.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bYellow.Name = "bYellow";
            this.bYellow.Size = new System.Drawing.Size(23, 37);
            this.bYellow.Text = "黄";
            this.bYellow.ToolTipText = "オレンジ";
            this.bYellow.Click += new System.EventHandler(this.BtnYellow_Click);
            // 
            // bWhite
            // 
            this.bWhite.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bWhite.Image = global::sobaco.Properties.Resources.White;
            this.bWhite.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bWhite.Name = "bWhite";
            this.bWhite.Size = new System.Drawing.Size(23, 37);
            this.bWhite.Text = "白";
            this.bWhite.Click += new System.EventHandler(this.BtnWhite_Click);
            // 
            // bWidth
            // 
            this.bWidth.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bWidth.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.極太ToolStripMenuItem,
            this.太ToolStripMenuItem,
            this.中ToolStripMenuItem,
            this.細ToolStripMenuItem,
            this.極細ToolStripMenuItem});
            this.bWidth.Image = global::sobaco.Properties.Resources.w3;
            this.bWidth.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bWidth.Name = "bWidth";
            this.bWidth.Size = new System.Drawing.Size(29, 37);
            this.bWidth.Text = "線の太さ";
            // 
            // 極太ToolStripMenuItem
            // 
            this.極太ToolStripMenuItem.Image = global::sobaco.Properties.Resources.w5;
            this.極太ToolStripMenuItem.Name = "極太ToolStripMenuItem";
            this.極太ToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.極太ToolStripMenuItem.Text = "極太";
            this.極太ToolStripMenuItem.Click += new System.EventHandler(this.極太ToolStripMenuItem_Click);
            // 
            // 太ToolStripMenuItem
            // 
            this.太ToolStripMenuItem.Image = global::sobaco.Properties.Resources.w4;
            this.太ToolStripMenuItem.Name = "太ToolStripMenuItem";
            this.太ToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.太ToolStripMenuItem.Text = "太";
            this.太ToolStripMenuItem.Click += new System.EventHandler(this.太ToolStripMenuItem_Click);
            // 
            // 中ToolStripMenuItem
            // 
            this.中ToolStripMenuItem.Image = global::sobaco.Properties.Resources.w3;
            this.中ToolStripMenuItem.Name = "中ToolStripMenuItem";
            this.中ToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.中ToolStripMenuItem.Text = "中";
            this.中ToolStripMenuItem.Click += new System.EventHandler(this.中ToolStripMenuItem_Click);
            // 
            // 細ToolStripMenuItem
            // 
            this.細ToolStripMenuItem.Image = global::sobaco.Properties.Resources.w2;
            this.細ToolStripMenuItem.Name = "細ToolStripMenuItem";
            this.細ToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.細ToolStripMenuItem.Text = "細";
            this.細ToolStripMenuItem.Click += new System.EventHandler(this.細ToolStripMenuItem_Click);
            // 
            // 極細ToolStripMenuItem
            // 
            this.極細ToolStripMenuItem.Image = global::sobaco.Properties.Resources.w1;
            this.極細ToolStripMenuItem.Name = "極細ToolStripMenuItem";
            this.極細ToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.極細ToolStripMenuItem.Text = "極細";
            this.極細ToolStripMenuItem.Click += new System.EventHandler(this.極細ToolStripMenuItem_Click);
            // 
            // printDialog1
            // 
            this.printDialog1.UseEXDialog = true;
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 64);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(631, 484);
            this.panel1.TabIndex = 4;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(631, 484);
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PictureBox1_MouseDown);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PictureBox1_MouseMove);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PictureBox1_MouseUp);
            // 
            // ChartPaint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(631, 570);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ChartPaint";
            this.Text = "SoBaCo - ChartPaint";
            this.Load += new System.EventHandler(this.ChartPaint_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ChartPaint_KeyDown);
            this.Resize += new System.EventHandler(this.ChartPaint_Resize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolStripButton bPrint;
        private System.Windows.Forms.ToolStripButton bPen;
        private System.Windows.Forms.ToolStripButton bLine;
        private System.Windows.Forms.ToolStripButton bCircle;
        private System.Windows.Forms.ToolStripButton bText;
        private System.Windows.Forms.ToolStripButton bColor;
        private System.Windows.Forms.ToolStripDropDownButton bWidth;
        private System.Windows.Forms.ToolStripMenuItem 極太ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 太ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 中ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 細ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 極細ToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton bEraser;
        private System.Windows.Forms.PrintDialog printDialog1;
        private System.Drawing.Printing.PrintDocument printDocument1;
        private System.Windows.Forms.ToolStripButton bUndo;
        private System.Windows.Forms.ToolStripButton bRedo;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripButton bBlack;
        private System.Windows.Forms.ToolStripButton bRed;
        private System.Windows.Forms.ToolStripButton bGreen;
        private System.Windows.Forms.ToolStripButton bBlue;
        private System.Windows.Forms.ToolStripButton bWhite;
        private System.Windows.Forms.ToolStripButton bYellow;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton bNone;
        private System.Windows.Forms.ToolStripButton bSquare;
        private System.Windows.Forms.ToolStripButton bSquareFill;
        private System.Windows.Forms.ToolStripMenuItem 印刷ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    }
}