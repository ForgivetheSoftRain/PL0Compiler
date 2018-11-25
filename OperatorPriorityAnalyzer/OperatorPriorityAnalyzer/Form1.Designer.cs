namespace OperatorPriorityAnalyzer
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxGrammar = new System.Windows.Forms.TextBox();
            this.buttonmatrix = new System.Windows.Forms.Button();
            this.textBoxSentence = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonprocess = new System.Windows.Forms.Button();
            this.listView2 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(38, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "文法输入";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // textBoxGrammar
            // 
            this.textBoxGrammar.Location = new System.Drawing.Point(42, 83);
            this.textBoxGrammar.Multiline = true;
            this.textBoxGrammar.Name = "textBoxGrammar";
            this.textBoxGrammar.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxGrammar.Size = new System.Drawing.Size(315, 285);
            this.textBoxGrammar.TabIndex = 1;
            this.textBoxGrammar.UseWaitCursor = true;
            // 
            // buttonmatrix
            // 
            this.buttonmatrix.Location = new System.Drawing.Point(403, 22);
            this.buttonmatrix.Name = "buttonmatrix";
            this.buttonmatrix.Size = new System.Drawing.Size(149, 43);
            this.buttonmatrix.TabIndex = 4;
            this.buttonmatrix.Text = "生成算符优先矩阵";
            this.buttonmatrix.UseVisualStyleBackColor = true;
            this.buttonmatrix.Click += new System.EventHandler(this.buttonmatrix_Click);
            // 
            // textBoxSentence
            // 
            this.textBoxSentence.Location = new System.Drawing.Point(42, 421);
            this.textBoxSentence.Multiline = true;
            this.textBoxSentence.Name = "textBoxSentence";
            this.textBoxSentence.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxSentence.Size = new System.Drawing.Size(315, 285);
            this.textBoxSentence.TabIndex = 6;
            this.textBoxSentence.UseWaitCursor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(38, 382);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 20);
            this.label2.TabIndex = 5;
            this.label2.Text = "句子输入";
            // 
            // buttonprocess
            // 
            this.buttonprocess.Location = new System.Drawing.Point(403, 372);
            this.buttonprocess.Name = "buttonprocess";
            this.buttonprocess.Size = new System.Drawing.Size(149, 43);
            this.buttonprocess.TabIndex = 8;
            this.buttonprocess.Text = "移进规约";
            this.buttonprocess.UseVisualStyleBackColor = true;
            this.buttonprocess.Click += new System.EventHandler(this.button2_Click);
            // 
            // listView2
            // 
            this.listView2.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6});
            this.listView2.GridLines = true;
            this.listView2.Location = new System.Drawing.Point(403, 421);
            this.listView2.Name = "listView2";
            this.listView2.Size = new System.Drawing.Size(512, 285);
            this.listView2.TabIndex = 10;
            this.listView2.UseCompatibleStateImageBehavior = false;
            this.listView2.View = System.Windows.Forms.View.Details;
            this.listView2.SelectedIndexChanged += new System.EventHandler(this.listView2_SelectedIndexChanged_1);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "步骤";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "栈";
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "优先关系";
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "当前符号";
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "剩余符号";
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "动作";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(403, 83);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(512, 285);
            this.textBox1.TabIndex = 11;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(943, 720);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.listView2);
            this.Controls.Add(this.buttonprocess);
            this.Controls.Add(this.textBoxSentence);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonmatrix);
            this.Controls.Add(this.textBoxGrammar);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "算符优先分析器";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxGrammar;
        private System.Windows.Forms.Button buttonmatrix;
        private System.Windows.Forms.TextBox textBoxSentence;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonprocess;
        private System.Windows.Forms.ListView listView2;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.TextBox textBox1;
    }
}

