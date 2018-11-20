namespace PL0WindowsFormsApp
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.textBoxfile = new System.Windows.Forms.TextBox();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.choosefile = new System.Windows.Forms.Button();
            this.textBoxCode = new System.Windows.Forms.TextBox();
            this.listView1 = new System.Windows.Forms.ListView();
            this.col1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.col2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.col3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.buttonsavefile = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.listView2 = new System.Windows.Forms.ListView();
            this.label1 = new System.Windows.Forms.Label();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // textBoxfile
            // 
            this.textBoxfile.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBoxfile.Location = new System.Drawing.Point(46, 34);
            this.textBoxfile.Name = "textBoxfile";
            this.textBoxfile.Size = new System.Drawing.Size(678, 30);
            this.textBoxfile.TabIndex = 1;
            this.textBoxfile.TextChanged += new System.EventHandler(this.textBoxfile_TextChanged);
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(1042, 696);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(200, 25);
            this.dateTimePicker1.TabIndex = 2;
            // 
            // choosefile
            // 
            this.choosefile.Location = new System.Drawing.Point(810, 33);
            this.choosefile.Name = "choosefile";
            this.choosefile.Size = new System.Drawing.Size(88, 30);
            this.choosefile.TabIndex = 3;
            this.choosefile.Text = "选择文件";
            this.choosefile.UseVisualStyleBackColor = true;
            this.choosefile.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBoxCode
            // 
            this.textBoxCode.Font = new System.Drawing.Font("宋体", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBoxCode.Location = new System.Drawing.Point(46, 125);
            this.textBoxCode.Multiline = true;
            this.textBoxCode.Name = "textBoxCode";
            this.textBoxCode.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxCode.Size = new System.Drawing.Size(470, 491);
            this.textBoxCode.TabIndex = 4;
            this.textBoxCode.TextChanged += new System.EventHandler(this.textBoxCode_TextChanged);
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.col1,
            this.col2,
            this.col3});
            this.listView1.GridLines = true;
            this.listView1.Location = new System.Drawing.Point(538, 125);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(360, 342);
            this.listView1.TabIndex = 6;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // col1
            // 
            this.col1.Text = "单词";
            this.col1.Width = 100;
            // 
            // col2
            // 
            this.col2.Text = "类型";
            this.col2.Width = 100;
            // 
            // col3
            // 
            this.col3.Text = "值";
            this.col3.Width = 100;
            // 
            // buttonsavefile
            // 
            this.buttonsavefile.Location = new System.Drawing.Point(46, 88);
            this.buttonsavefile.Name = "buttonsavefile";
            this.buttonsavefile.Size = new System.Drawing.Size(90, 31);
            this.buttonsavefile.TabIndex = 7;
            this.buttonsavefile.Text = "保存文件";
            this.buttonsavefile.UseVisualStyleBackColor = true;
            this.buttonsavefile.Click += new System.EventHandler(this.buttonsavefile_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(538, 88);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(104, 31);
            this.button1.TabIndex = 8;
            this.button1.Text = "词法分析";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // listView2
            // 
            this.listView2.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listView2.GridLines = true;
            this.listView2.Location = new System.Drawing.Point(538, 510);
            this.listView2.Name = "listView2";
            this.listView2.Size = new System.Drawing.Size(360, 106);
            this.listView2.TabIndex = 9;
            this.listView2.UseCompatibleStateImageBehavior = false;
            this.listView2.View = System.Windows.Forms.View.Details;
            this.listView2.SelectedIndexChanged += new System.EventHandler(this.listView2_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(534, 470);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 20);
            this.label1.TabIndex = 10;
            this.label1.Text = "错误";
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "行数";
            this.columnHeader1.Width = 140;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "错误";
            this.columnHeader2.Width = 140;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1001, 700);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listView2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.buttonsavefile);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.textBoxCode);
            this.Controls.Add(this.choosefile);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.textBoxfile);
            this.Name = "Form1";
            this.Text = "PL0词法分析器";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxfile;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Button choosefile;
        private System.Windows.Forms.TextBox textBoxCode;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Button buttonsavefile;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListView listView2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ColumnHeader col1;
        private System.Windows.Forms.ColumnHeader col2;
        private System.Windows.Forms.ColumnHeader col3;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
    }
}

