using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PL0WindowsFormsApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public string[] key = { "const", "var", "procedure", "odd", "if", "then", "else", "while", "do", "call", "begin", "end", "read", "write", "repeat", "until" };
        public string[] op = { "+", "-", "*", "/", "=", "odd", "<", "<=", ">", ">=", ":=", "<>" };
        public string file, code;
        public List<List<string>> wordresult = new List<List<string>>();//保存词法分析结果
        public List<List<string>> errorlist = new List<List<string>>();
        public int line = 1;
        public int error = 0;
        public char ch;//每次读取的字符
        public char temp;//缓冲字符
        public string token;//每次读取的单词
        public int sym;//每次读取的单词的类别
        public string value;//每次读取的单词的值
        public FileStream fs;
        public StreamReader sr;
        public bool flag = false;

        //将单词加入到结果中
        public void addresult(string t, int s, string v)
        {
            List<string> tk;
            switch (s)
            {
                case 1:
                    tk = new List<string> { t, "关键字", v };
                    break;
                case 2:
                    tk = new List<string> { t, "标识符", v };
                    break;
                case 3:
                    tk = new List<string> { t, "运算符", v };
                    break;
                case 4:
                    tk = new List<string> { t, "常数", v };
                    break;
                default:
                    tk = new List<string> { t, "分界符", v };
                    break;
            }
            wordresult.Add(tk);
        }
        //加入错误列表
        public void adderror(int id)
        {
            List<string> el;
            switch (id)
            {
                case 1:
                    el = new List<string> { line.ToString(), ":后面必须为=" };
                    break;
                case 2:
                    el = new List<string> { line.ToString(), "其它错误" };
                    break;
                default:
                    el = new List<string> { line.ToString(), "error" };
                    break;
            }
            errorlist.Add(el);
        }
        //判断是否为关键字
        public bool iskey(string v)
        {
            foreach (string i in key)
            {
                if (i == v)
                    return true;
            }
            return false;
        }
        //每次读取一个字符
        public int getch()
        {
            if (flag)
            {
                flag = false;
                // Console.Write(ch);
                return 1;
            }
            else
            {
                int content = sr.Read();
                if (content != -1)
                {
                    ch = Convert.ToChar(content);
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        //+,-,*,/，;,等符号
        public void addop(char c, int s)
        {
            token = Convert.ToString(c);
            value = token;
            //  Console.WriteLine(token, s, value);
            addresult(token, s, value);
        }

        //判断字符是数字字母还是都不是
        public int judge(char ch)
        {
            int t = ch - '0';
            if (t >= 0 && t <= 9)
                return 1;
            else if ((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z'))
                return 2;
            else
                return 0;
        }

        //每次读取一个单词
        public int getsym()
        {
            token = "";
            value = "";
            getch();
            while (ch == ' ' || ch == '\n' || ch == '\t')
            {
                if (ch == '\n')
                {
                    line++;
                }
                getch();
            }
            if (sr.Peek() == -1)
            {
                return 0;
            }
            if (judge(ch) == 2)
            {
                while (judge(ch) != 0)
                {
                    token = token + ch;
                    getch();
                }
                value = token;
                temp = ch;
                flag = true;
                fs.Seek(-1, SeekOrigin.Current);//回退鼠标，因为已经将光标移到前面一个了
                if (iskey(token))
                    sym = 1;//1是关键字
                else
                    sym = 2;//2是标识符
                addresult(token, sym, value);
            }
            else if (judge(ch) == 1)
            {
                while (judge(ch) == 1)
                {
                    token = token + ch;
                    getch();
                }
                Console.WriteLine(ch);
                if (ch == '.')
                {
                    token = token + '.';
                    getch();
                    while (judge(ch) == 1)
                    {
                        token = token + ch;
                        getch();
                    }
                    value = token;
                }
                else
                {
                    value = Convert.ToString(int.Parse(token), 2);
                }
                sym = 4;
                temp = ch;
                flag = true;
                fs.Seek(-1, SeekOrigin.Current);
                addresult(token, sym, value);
            }
            else if (ch == ':')
            {
                getch();
                if (ch == '=')
                {
                    sym = 3;
                    token = ":=";
                    value = ":=";
                    addresult(token, sym, value);

                }
                else
                {
                    sym = 3;
                    token = ":";
                    value = ":";
                    addresult(token, sym, value);
                    flag = true;
                    adderror(1);
                }
            }
            else if (ch == '<')
            {
                sym = 3;
                getch();
                if (ch != '=')
                {
                    flag = true;
                    fs.Seek(-1, SeekOrigin.Current);
                    token = "<";
                    value = "<";
                    temp = ch;
                }
                else
                {
                    token = "<=";
                    value = "<=";
                }
                addresult(token, sym, value);
            }
            else if (ch == '>')
            {
                sym = 3;
                getch();
                if (ch != '=')
                {
                    flag = true;
                    fs.Seek(-1, SeekOrigin.Current);
                    token = ">";
                    value = ">";
                    temp = ch;
                }
                else
                {
                    token = ">=";
                    value = ">=";
                }
                addresult(token, sym, value);
            }
            else if (ch == '+' || ch == '-' || ch == '*' || ch == '/'||ch=='=')
            {
                token = Convert.ToString(ch);
                value = token;
                sym = 3;
                addresult(token, sym, value);
                // Console.WriteLine(token, sym, value);
            }
            else if (ch == ';' || ch == ',' || ch == '.' || ch == '(' || ch == ')')
            {
                token = Convert.ToString(ch);
                value = token;
                sym = 5;
                addresult(token, sym, value);
                // Console.WriteLine(token, sym, value);
            }
            return 1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Title = "请选择文件";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                file = fileDialog.FileName;//返回文件的完整路径  

                textBoxfile.Text = file.ToString();
                textBoxCode.Text = File.ReadAllText(file, Encoding.Default);

            }

        }

        private void textBoxfile_TextChanged(object sender, EventArgs e)
        {
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            fs = new FileStream(file, FileMode.Open);
            sr = new StreamReader(fs, Encoding.ASCII);
            wordresult.Clear();//清空之前运行的结果
            this.listView1.Items.Clear();
            this.listView1.Scrollable = true;//有滚动条
            errorlist.Clear();
            this.listView2.Items.Clear();
            this.listView2.Scrollable = true;//有滚动条
            while (getsym() == 1)
            {
            }
            //将最后的.加入
            if (textBoxCode.Text[textBoxCode.Text.Length - 1] == '.')
            {
                List<string> end = new List<string> { ".", "分界符", "." };
                wordresult.Add(end);
            }
            for (int i = 0; i < wordresult.Count(); i++)
            {
                ListViewItem item = new ListViewItem(wordresult[i].ToArray());
                listView1.Items.Add(item);
            }
            if (errorlist.Count() > 0)
            {
                for (int i = 0; i < errorlist.Count(); i++)
                {
                    ListViewItem item = new ListViewItem(errorlist[i].ToArray());
                    listView2.Items.Add(item);
                }
            }

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBoxCode_TextChanged(object sender, EventArgs e)
        {

        }

        private void buttonsavefile_Click(object sender, EventArgs e)
        {
            File.WriteAllText(file, textBoxCode.Text, Encoding.Default);

        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
