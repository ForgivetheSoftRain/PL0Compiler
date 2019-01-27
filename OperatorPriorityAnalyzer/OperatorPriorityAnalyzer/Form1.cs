using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OperatorPriorityAnalyzer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //文法类
        public class WF
        {
            public string left;//文法左边的字符串
            public List<string> right=new List<string>();
            public WF(string str)
            {
                left = str;
            }
            public void insert(string str)
            {
                right.Add(str);
            }
        }

        public static int MAX = 1001;//矩阵最大大小
        public List<char> VT;//终结符号VT集
        public char[,] relation;//算符优先矩阵
        public List<WF> VN_set;//VN集合
        public Dictionary<string, int> VN_dic;//VN映射关系，便于生成产生式
        public HashSet<char>[] first;//first集合
        public HashSet<char>[] last;//last集合
        public int[] used;//判断该非终结符号是否打印过
        public int[] visited;//搜索过程中判断是否遍历过
        public List<string> symbolstatus;//符号栈的状态过程
        public List<string> prioritystatus;//存储优先关系的过程
        public List<string> cursym;//当前符号的过程
        public List<string> remsym;//剩余符号的过程
        public List<string> action;//动作符号的过程

        public bool isupper(char c)
        {
            return c - 'A' >= 0 && c - 'Z' <= 0;
        }
        //根据输入的文法，得到产生式，终结符号集VT
        public void process(string[] programer)
        {
            for (int i = 0; i < MAX; i++)
                used[i] = 0;
            foreach(string s in programer)
            {
                int len = s.Length,j;
                for (j = 0; j < len; j++)
                    if (s[j] == '-')
                        break;
                string vn = s.Substring(0, j);//左边的VN
                //得到产生式
                //将形如E->T,E->E+T转换成E->T|E+T
                if(!VN_dic.Keys.Contains(vn)||VN_dic[vn]==0)
                {
                    VN_set.Add(new WF(vn));
                    VN_dic[vn] = VN_set.Count();//对于一个非终结符号，有几个映射关系
                }
                //得到VN的映射关系。例如E->E+T,E->T可以生成{E,[E+T,E]}
                VN_set[VN_dic[vn] - 1].insert(s.Substring(j+2,s.Length-j-2));
                //得到终结符号集VT
                for (int k=0;k<j;k++)
                {
                    if(!isupper(s[k]))
                    {
                        if(used[s[k]]==1)
                            continue;
                        used[s[k]] = 1;
                        VT.Add(s[k]);
                    }
                }
                for(int k=j+2;k<len;k++)
                {
                    if (!isupper(s[k]))
                    {
                        if (used[s[k]] == 1)
                            continue;
                        VT.Add(s[k]);
                        used[s[k]] = VT.Count();
                    }
                }

            }
        }
        public void dfsFirst(int t)
        {
            if (visited[t]==1)
                return;
            visited[t] = 1;
            string left = VN_set[t].left;
            first[t] = new HashSet<char>();
            for(int i=0;i<VN_set[t].right.Count();i++)
            {
                string right = VN_set[t].right[i];//第t个非终结符号映射的第i个右部
                if (isupper(right[0]))
                {
                    //对于E->Vb的情况
                    if (right.Length > 1 && !isupper(right[1]))
                        first[t].Add(right[1]);
                    int y = VN_dic[right.Substring(0, 1)] - 1;//对index为y的VN进行搜索
                    dfsFirst(y);
                    //若b在first(V)集合内，那么它也在first(E)集合内
                    foreach (var item in first[y])
                        first[t].Add(item);
                }
                else
                    first[t].Add(right[0]);//对于E->b的情况
            }
        }
        //求First集合
        public void First()
        {
            for (int i = 0; i < MAX; i++)
                visited[i] = 0;
            for(int i=0;i<VN_set.Count();i++)
            {
                if (visited[i] == 1)
                    continue;
                else
                    dfsFirst(i);
            }
        }

        public void dfsLast(int t)
        {
            if (visited[t] == 1)
                return;
            visited[t] = 1;
            string left = VN_set[t].left;
            last[t] = new HashSet<char>();
            for (int i = 0; i < VN_set[t].right.Count(); i++)
            {
                string right = VN_set[t].right[i];//第t个非终结符号映射的第i个右部
                int n = right.Length - 1;
                //E->aV的情况
                if (isupper(right[n]))
                {
                    //将a加入last集合
                    if (n > 0 && !isupper(right[n-1]))
                        last[t].Add(right[1]);
                    //a在last(v)中，那么也在last(E)中
                    int y = VN_dic[right.Substring(n, 1)] - 1;//对index为y的VN进行搜索
                    dfsLast(y);
                    foreach (var item in last[y])
                        last[t].Add(item);
                }
                else
                    last[t].Add(right[n]);//E->……a的情况
            }
        }
        //求Last集合
        public void Last()
        {
            for (int i = 0; i < MAX; i++)
                visited[i] = 0;
            for (int i = 0; i < VN_set.Count(); i++)
            {
                if (visited[i] == 1)
                    continue;
                else
                    dfsLast(i);
            }
        }
        public void matrix()
        {
            //初始化，将算符优先矩阵置空
            for (int i = 0; i < MAX; i++)
                for (int j = 0; j < MAX; j++)
                    relation[i,j] = ' ';
            for(int i=0;i<VN_set.Count();i++)
            {
                for(int j=0;j<VN_set[i].right.Count();j++)//遍历每条规则
                {
                    string right = VN_set[i].right[j];
                    for(int k=0;k<right.Length-1;k++)
                    {
                        //xi和xi+1都是终结符，置为=
                        if (!isupper(right[k]) && !isupper(right[k + 1]))
                        {
                            if (relation[right[k], right[k + 1]] == ' ' || relation[right[k], right[k + 1]] == '=')
                                relation[right[k], right[k + 1]] = '=';
                            else//已经被赋值过，会产生冲突
                                relation[right[k], right[k + 1]] = 'E';
                        }
                        //xi和xi+2为终结符，xi+1为非终结符，置为=

                        if (k <right.Length - 2 && !isupper(right[k]) && !isupper(right[k + 2]) && isupper(right[k+1]))
                        {
                            if (relation[right[k], right[k + 2]] == ' '||relation[right[k],right[k+2]]=='=')
                                relation[right[k], right[k + 2]] = '=';
                            else
                                relation[right[k], right[k + 2]] = 'E';
                        }
                        //xi为终结符号，xi+1为非终结符号，对于first(xi+1)的每一个b，xi+1<b
                        if(!isupper(right[k])&&isupper(right[k+1]))
                        {
                            int x = VN_dic[right.Substring(k + 1, 1)] - 1;
                            foreach (var item in first[x])
                            {
                                if (relation[right[k], item] == '>')
                                    relation[right[k], item] = 'E';
                                else
                                    relation[right[k], item] = '<';
                            }
                        }
                        //xi为非终结符号，xi+1为终结符号，对于last(xi)的每一个a，a>xi+1
                        if(isupper(right[k])&&!isupper(right[k+1]))
                        {
                            int x = VN_dic[right.Substring(k, 1)] - 1;
                            foreach (var item in last[x])
                            {
                                if (relation[item, right[k + 1]] == '<')
                                    relation[item, right[k + 1]] = 'E';
                                else
                                    relation[item, right[k + 1]] = '>';
                            }
                        }
                    }
                }
            }
        }
        
        //规约
        public string guiyuel(string sym)
        {
            string result="";
            char t;
            if (!isupper(sym[sym.Length - 1])&&sym[sym.Length-1]!=')'&&sym[sym.Length-1]!='>'&&sym[sym.Length-1]!=']'&&sym[sym.Length-1]!='}')
            {
                t = sym[sym.Length - 1];
                if (t>='a'&&t<='z')
                {
                    foreach (var i in VN_set)
                    {
                        for (int j = 0; j < i.right.Count(); j++)
                        {
                            if (i.right[j].Contains(t))
                            {
                                result = sym.Substring(0, sym.Length - 1) + i.left;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    return "error";
                }
            }
            else
            {
                t = sym[sym.Length - 2];
                foreach (var i in VN_set)
                {
                    for (int j = 0; j < i.right.Count(); j++)
                    {
                        if (i.right[j].Contains(t))
                        {
                            result = sym.Substring(0, sym.Length - 3) + i.left;
                            break;
                        }
                    }
                }
            }
            
            return result;
        }
        //得到当前符号和栈顶符号的关系
        char compare(string symst,string cu)
        {
            for(int i=symst.Length-1;i>=0;i--)
            {
                if (!isupper(symst[i]))
                    return relation[symst[i], cu[0]];
            }
            return ' ';
        }

        public void  step()
        {
            symbolstatus = new List<string>();
            prioritystatus = new List<string>();
            cursym = new List<string>();
            remsym = new List<string>();
            action = new List<string>();

            string sentence = textBoxSentence.Text.ToString()+"#";//得到要处理的句子;
            string symstack = "#";
            bool error = false;
            //程序结束条件为符号栈中只有两个元素并且输入串长度为1，或者出现错误
            //即，栈顶的符号和当前输入串第一个符号没有大小关系或者关系冲突
            int ans = 1;//步骤
            string cur=sentence[0].ToString();
            string rem=sentence.Substring(1,sentence.Length-1);
            while (!(symstack.Length==2&&rem.Length==0)&&!(compare(symstack,cur)==' '|| compare(symstack, cur) == 'E'))
            {
                prioritystatus.Add(compare(symstack, cur).ToString());
                cursym.Add(cur);
                remsym.Add(rem);
                if (compare(symstack, cur) == '>')
                {
                    symbolstatus.Add(symstack);
                    string temp=symstack;
                    symstack = guiyuel(symstack);
                    action.Add("规约");
                    if(symstack=="error")
                    {
                        
                        error = true;
                        symstack = temp;
                        break;
                    }
                    Console.WriteLine(symstack+"规约");
                }
                else if(compare(symstack, cur) == 'E')
                {
                    error = true;
                    break;
                }
                else
                {
                    action.Add("移进");
                    symbolstatus.Add(symstack);
                    symstack += cur;
                    Console.WriteLine(symstack+"移进");
                    if (rem.Length >= 1)
                    {
                        cur = rem[0].ToString();
                        rem = rem.Substring(1, rem.Length - 1);
                    }
                    else
                        rem = "";
                }
                ans++;
                Console.WriteLine(cur + "当前符号");
                Console.WriteLine(rem + "剩余符号");
                //sentence = rem;
            }
            symbolstatus.Add(symstack);
            prioritystatus.Add(compare(symstack, cur).ToString());
            cursym.Add(cur);
            remsym.Add(rem);
            if (compare(symstack, cur) == ' ' || compare(symstack, cur) == 'E')
                error = true;
            if (error)
            {
                action.Add("错误");
            }
            else
            {
                action.Add("接受");
            }
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            step();
            listView2.Items.Clear();
            string[][] process = new string[symbolstatus.Count()][];
            for(int i=0;i<symbolstatus.Count();i++)
            {
                process[i] = new string[6];
                process[i][0] = (i + 1).ToString();
                process[i][1] = symbolstatus[i];
                process[i][2] = prioritystatus[i];
                process[i][3] = cursym[i];
                process[i][4] = remsym[i];
                process[i][5] = action[i];
            }
            for (int i = 0; i < process.Length; i++)
            {
                ListViewItem item = new ListViewItem(process[i]);
                listView2.Items.Add(item);
            }
        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void buttonmatrix_Click(object sender, EventArgs e)
        {

            VN_dic = new Dictionary<string, int>();
            VT = new List<char>();
            VN_set = new List<WF>();
            first = new HashSet<char>[MAX];
            relation = new char[MAX, MAX];
            last = new HashSet<char>[MAX];
            used = new int[MAX];
            visited = new int[MAX];

            textBox1.Text = "";
            
            HashSet<string> textgram = new HashSet<string>();
            for (int i = 0; i < textBoxGrammar.Lines.Length; i++)
            {
                if(textBoxGrammar.Lines[i]!="")
                {
                    textgram.Add(textBoxGrammar.Lines[i]);
                }
                if(!(textBoxGrammar.Lines[0][3]=='#'&& textBoxGrammar.Lines[0][5] == '#'))
                {
                    textgram.Add("S->#" + textBoxGrammar.Lines[0][0].ToString() + "#");
                }
            }
            process(textgram.ToArray());
            
            First();
            Last();
            matrix();
            VT.Distinct().ToList();//去重
            HashSet<char> temp = new HashSet<char>();
            foreach(char i in VT)
            {
                temp.Add(i);
            }
            VT = temp.ToList();
            int count = VT.Count() + 1;
            string [][] prioritymatrix = new string[count][];
            for (int i = 0; i < count; i++)
                prioritymatrix[i] = new string[count];
            for(int i=0;i<count;i++)
            {
                for (int j = 0; j < count; j++)
                    prioritymatrix[i][j] = " ";
            }

            for (int i=1;i < count;i++)
            {
                prioritymatrix[i][0] = VT[i-1].ToString();
                prioritymatrix[0][i]= VT[i-1].ToString();
            }
            for (int i = 1; i < count; i++)
            {
                for (int j = 1; j < count; j++)
                {
                    prioritymatrix[i][j] = relation[VT[i - 1], VT[j - 1]].ToString();
                }
            }
            for(int i=0;i<count;i++)
            {
                for(int j=0;j<count;j++)
                {
                    Console.Write(prioritymatrix[i][j] + " ");
                    textBox1.Text += prioritymatrix[i][j] + "\t";
                }
                textBox1.Text += "\r\n\r\n\r\n";
                Console.WriteLine();
            }

            //DataTable dt = new DataTable();
            //for (int i = 0; i < prioritymatrix[0].Length; i++)
            //    dt.Columns.Add(prioritymatrix[0][i], typeof(string));
            //for(int i=1;i<prioritymatrix.Length;i++)
            //{
            //    DataRow dr = dt.NewRow();
            //    for(int j=0;j<prioritymatrix[0].Length;j++)
            //    {
            //        dr[j] = prioritymatrix[i][j];
            //        dt.Rows.Add(dr.ItemArray);
            //    }
            //}
            //dataGridView1.DataSource = prioritymatrix;
            //for (int i = 0; i < count; i++)
            //{
            //    ListViewItem item = new ListViewItem(prioritymatrix[i]);
            //    listViewmatrix.Items.Add(item);
            //}

        }

        private void listViewmatrix_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listView2_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}