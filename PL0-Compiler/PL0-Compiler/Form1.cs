using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace PL0_Compiler
{
    public struct pCode
    {
        public int f;//虚拟机代码指令
        public int l;//层次差
        public int a;//指令参数
    };
    //符号表
    public struct Item
    {
        public string name;//名字
        public int type;//类型
        public int val;//值
        public int level;//所处层
        public int addr;//地址
        public int Size;//需要分配的数据空间
        public bool ret;
        public int paarNum;
    };
    public struct words//单词的名字，类型，行号
    {
        public string name;
        public int type;
        public int line;
    }
    public partial class Form1 : MetroFramework.Forms.MetroForm
    {
        /*0到15表示保留字，16表示标识符，17表示常数，18表示+，19表示-，20表示*，21表示
        * /、22表示：=23表示<=,24表示>=，25表示=，26表示<，27表示>,28表示.,29表示， 30表示；
        * 31表示（，32表示）
        */
        private string[] codeF;
        private words[] wordList;
        private pCode[] code;
        private Item[] sign;
        private int usedNum;//去单词时已经取了的单词
        public static char ch;//当前读取的字符
        public  enum symbol
        {
            nul, ident, number, pluss, minuss, times, slash, oddsym, eql, neq, lss,
            leq, gtr, geq, lparen, rparen, comma, semicolon, period, becomes,
            beginsym, endsym, ifsym, thensym, whilesym, dosym, callsym, constsym,
            varsym, procsym, readsym, writesym, elsesym, repeatsym, untilsym
        };//保留字类型
       // public static symbol sym;//当前单词的类型
        public static string id;//当前读取的数字
        public static int num;//当前读取的数字
        public static int cc;//当前读取的位置
        public static int ll;//每行代码长度
        public static int lc;//当前第几行
        public static int err;//错误计数
        //public static int cx;//Pcode指令索引指针
        public static string line;//缓存一行代码
        public static string openfile;//导入的文件
        public static FileStream fs;//读入的文件流
        public static StreamReader sr;//从字节流中读取字符
        //关键字
        public static string[] key = { "" ,"begin","call","const","do","else","end","if","odd","procedure","read","repeat","then","until","var","while","write"};
        public static symbol[] ksym = new symbol[18]; //判断属于哪个关键字
        public static symbol[] ssym = new symbol[256];//判断单字符类型，比如+,-,*,/等
        public static string []errmsg = {
        "",
        "常数说明中应是'='而不是':='",
        "常数说明中'='后应为数字",
        "常数说明中标识符后应为'='",
        "const,var,procedure后应为标识符",
        "漏掉逗号或分号",
        "过程说明后的符号不正确",
        "应为语句开始符号",
        "程序体内语句部分后的符号不正确",
        "程序结尾应为句号",
        "语句之间漏了分号",
        "标识符未说明",
        "不可向常量或过程赋值",
        "赋值语句中应为赋值运算符':='",
        "call后应为标识符",
        "call后标识符属性应为过程,不可调用常量或变量",
        "条件语句中缺失then",
        "应为分号或end",
        "while型循环语句中缺失do",
        "语句后的符号不正确",
        "应为关系运算符",
        "表达式内不可有过程标识符",
        "缺失右括号",
        "因子后不可为此符号",
        "表达式不能以此符号开始",
        "repeat循环语句中缺失until",
        "代码太长，无法编译",
        "RuntimeError，地址偏移越界",
	    "Read语句括号内不是标识符",
        };
        public static HashSet<symbol> declbegsys;//声明开始的符号集
        public static HashSet<symbol> statbegsys;//语句开始的符号集
        public static HashSet<symbol> facbegsys;//因子开始的符号集
        
        private string[] keyWord;
        private string[] wordFlow;
        private string[] kindFlow;
        private string[] valueFlow;
        private int total;
        //first集合
        private bool[] decPre;
        private bool[] statePre;
        private bool[] factorPre;
        //程序是否有错误
        private bool isError;//是否有错误
        private bool isCompile;
        private int pcodeNum;//pcode数量
        private int wordNum;//单词数量
        private int sym;//标识符
        private string ss;
        private int symNum;
        private bool[] follow;
        private int cx;
        private int tx;
        private int errorNum;//错误数量
        private int ttop;
        private string[] input;
        private int linePos;
        public void init()//初始化
        {
            keyWord = new string[20];
            wordFlow = new string[20005];
            kindFlow = new string[20005];
            valueFlow = new string[20005];
            codeF = new string[15];
            wordList = new words[2005];
            sign = new Item[2005];
            code = new pCode[2005];
            follow = new bool[55];
            decPre = new bool[55];
            statePre = new bool[55];
            factorPre = new bool[55];
            
            total = 0;
            pcodeNum = 0;
            wordNum = 0;
            usedNum = 0;
            cx = 0;
            errorNum = 0;
            symNum = 55;
            tx = 0;
            ttop = 0;
            isError = false;
            isCompile = false;
            //
            keyWord[0] = "const";
            keyWord[1] = "var";
            keyWord[2] = "procedure";
            keyWord[3] = "odd";
            keyWord[4] = "if";
            keyWord[5] = "then";
            keyWord[6] = "else";
            keyWord[7] = "while";
            keyWord[8] = "do";
            keyWord[9] = "call";
            keyWord[10] = "begin";
            keyWord[11] = "end";
            keyWord[12] = "repeat";
            keyWord[13] = "until";
            keyWord[14] = "read";
            keyWord[15] = "write";

            codeF[0] = "LIT";
            codeF[1] = "OPR";
            codeF[2] = "LOD";
            codeF[3] = "STO";
            codeF[4] = "CAL";
            codeF[5] = "INT";
            codeF[6] = "JMP";
            codeF[7] = "JPC";
            codeF[8] = "WRT";
            codeF[9] = "RED";

            //codeF[10] = "loa";
            for (int i = 0; i < 55; i++)
                decPre[i] = false;
            decPre[0] = true;
            decPre[1] = true;
            decPre[2] = true;
            for (int i = 0; i < 55; i++)
                factorPre[i] = false;
            factorPre[16] = true;
            factorPre[17] = true;
            factorPre[31] = true;
            for (int i = 0; i < 55; i++)
                statePre[i] = false;
            statePre[4] = true;
            statePre[7] = true;
            statePre[9] = true;
            statePre[10] = true;
            statePre[12] = true;
            statePre[16] = true;
            for (int i = 0; i < symNum; i++)
                follow[i] = false;
            for (int i = 0; i < 55; i++)
            {
                if (decPre[i])
                    follow[i] = true;
                if (statePre[i])
                    follow[i] = true;
            }
            follow[28] = true;
        }
        private void insertWordList(int type, string name, int line)//保存单词的类型，名字，行号
        {
            wordList[wordNum].type = type;
            wordList[wordNum].name = name;
            wordList[wordNum++].line = line;
        }
        void dealError(int id, int lineId, string name)//处理各种错误，将错误的id，行号，名字加入错误列表中
        {
            textBox2.Text += errorNum+" 第"+lineId.ToString()+"行： 字符"+name+": ";
            switch (id)
            {
                case 1:
                    textBox2.Text += "应为:=而不是=";
                                       break;
                case 2:
                    textBox2.Text += "\"=\"后应为整数.";
                                       break;
                case 3:
                    textBox2.Text += "标识符后缺少\"=\".  ";
                    break;
                case 4:
                    textBox2.Text += "const ，var ，procedure后应为标识符.";
                                       break;
                case 5:
                    textBox2.Text += "漏掉了\",\"或者是\";\".  ";
                                       break;
                case 6:
                    textBox2.Text += "过程说明后的符号不正确";
                                       break;
                case 7:
                    textBox2.Text += "声明顺序有误，应为[<变量说明部分>][<常量说明部分>] [<过程说明部分>]<语句>。 ";
                                       break;
                case 8:
                    textBox2.Text += "程序体内语句部分字符不正确";
                                       break;
                case 9:
                    textBox2.Text += "程序的末尾缺少句号\".\" ";
                                       break;
                case 10:
                    textBox2.Text += "句子之间缺少\";\"。 ";
                                       break;
                case 11:
                    textBox2.Text += "标识符未说明";
                                       break;
                case 12:
                    textBox2.Text += "赋值号左端应为变量";
                                       break;
                case 13:
                    textBox2.Text += "应为赋值运算符\":=\"";
                                       break;
                case 14:
                    textBox2.Text += "call 后应为标识符 ";
                                       break;
                case 15:
                    textBox2.Text += "call不能调用常量或者变量 ";
                                       break;
                case 16:
                    textBox2.Text += "应为\"then\"。 ";
                                       break;
                case 17:
                    textBox2.Text += "缺少\"end\"或\";\"。 ";
                                       break;
                case 18:
                    textBox2.Text += "do while 型循环语句缺少while 。 ";
                                       break;
                case 19:
                    textBox2.Text += "语句后的标号错误。 ";
                                       break;
                case 20:
                    textBox2.Text += "应为关系运算符 ";
                                       break;
                case 21:
                    textBox2.Text += "表达式中不能有过程标识符 ";
                                       break;
                case 22:
                    textBox2.Text += "表达式中缺少右括号";
                                       break;
                case 23:
                    textBox2.Text += "非法符号。 ";
                                       break;
                case 24:
                    textBox2.Text += "表达式不能以这个符号开头 ";
                                       break;
                case 25:
                    textBox2.Text += "运算符后边是常量。 ";
                                       break;
                case 26:
                    textBox2.Text += "不存在此操作符 ";
                                       break;
                case 27:
                    textBox2.Text += "变量定义的长度应为常量或者是常数 ";
                                       break;
                case 28:
                    textBox2.Text += "变量定义重复 ";
                                       break;
                case 29:
                    textBox2.Text += "未找到对应过程名 ";
                                       break;
                case 30:
                    textBox2.Text += "不支持过程的判断 ";
                                       break;
                case 31:
                    textBox2.Text += "这个数太大";
                                       break;
                case 32:
                    textBox2.Text += "read 语句括号中的标识符不是变量。 ";
                                       break;
                case 33:
                    textBox2.Text += "缺少标识符，无法进行条件判断";
                                       break;
                case 34:
                    textBox2.Text += "read内应为变量值 ";
                                       break;
                case 35:
                    textBox2.Text += "此处不应出现过程说明标识符";
                                       break;
                case 36:
                    textBox2.Text += "缺少until";
                    break;
                case 37:
                    textBox2.Text += "read括号内应为变量标识符";
                                       break;
                case 40:
                    textBox2.Text += "应为左括号";
                    break;
                case 41:
                    textBox2.Text += "此处应为标识符 ";
                                       break;
                case 42:
                    textBox2.Text += "常量说明部分位置错误 ";
                                       break;
                case 43:
                    textBox2.Text += "常量说明部分多于一个 ";
                                       break;
                case 44:
                    textBox2.Text += "变量说明部分位置错误 ";
                                       break;
                case 45:
                    textBox2.Text += "变量说明部分多于一个 ";
                                       break;
                default:
                    textBox2.Text += " ";
                                       break;
            }
            textBox2.Text += Environment.NewLine;
            isError = true;
            errorNum++;
        }
        private void enter(int type, ref int tx, int level, ref int dx, int val, string name)//登记符号表
        {
            for (int i = tx; i >= 1; i--)//判断是否重复登记符号表
            {
                if (sign[i].name == name && level == sign[i].level)
                {
                    dealError(28, linePos, ss);
                    break;
                }
            }
            tx++;//符号表数量加一
            sign[tx].name = name;
            sign[tx].type = type;
            switch (type)
            {
                case 0://整数
                    if (val > 20000005)//数字太大
                    {
                        dealError(31, linePos, ss);
                        val = 0;
                    }
                    sign[tx].val = val;
                    sign[tx].level = level;
                    break;
                case 1://变量
                    sign[tx].addr = dx;
                    sign[tx].level = level;
                    dx++;
                    break;
                case 2://过程
                    sign[tx].level = level;
                    break;
            }
        }
        private int position(string ss, int tx)//查询符号表
        {
            for (int i = tx; i >= 1; i--)
            {
                if (ss == sign[i].name)
                    return i;
            }
            return -1;
        }
        private bool wordJudge(string str, int lineNum)//判断单词的类型
        {
            int len = str.Length;
            bool numFlag = false;//包含数字
            bool chFlag = false;//包含字母
            bool dotFlag = false;//包含小数点
            for (int i = 0; i < len; i++)
            {
                if (str[i] >= '0' && str[i] <= '9')
                    numFlag = true;
                else if (str[i] >= 'a' && str[i] <= 'z')
                    chFlag = true;
                else if (str[i] >= 'A' && str[i] <= 'Z')
                    chFlag = true;
                else if (str[i] == '.')
                    dotFlag = true;
            }
            if (chFlag)//如果包含字母
            {
                if (str[0] >= '0' && str[0] <= '9')//首位为数字，比如1adas,2da之类的不合法字符
                {
                    textBox2.Text += "第"+lineNum.ToString()+ " 行字符"+str+"不合法";
                    textBox2.Text += Environment.NewLine;
                    return false;
                }
                else if (dotFlag)//如果同时包含字母和小数点
                {
                    textBox2.Text += "第" + lineNum.ToString() + " 行字符" + str + "不合法";
                    textBox2.Text += Environment.NewLine;
                    return false;
                }
                else//关键字或标识符
                {
                    bool tt = false;
                    for (int i = 0; i < 16; i++)//判断是哪个关键字
                    {
                        if (string.Compare(keyWord[i], str) == 0)
                        {
                            tt = true;
                            if (str.Length <= 12)
                            {
                                int count = 12 - str.Length;
                            }
                            insertWordList(i, str, lineNum);//第i个关键字
                            return true;
                        }
                    }
                    insertWordList(16, str, lineNum);//标识符
                    return true;
                }
            }
            else//不包含字母，即数字和小数点
            {
                if (dotFlag)//如果有小数点，就将其转化为2进制
                {
                    byte[] bArray = BitConverter.GetBytes(Double.Parse(str));
                    string sBin = string.Empty;
                    foreach (byte b in bArray)
                    {
                        sBin += Convert.ToString(b, 2);
                    }
                }//不含小数点直接插入
                else
                {
                    insertWordList(17, str, lineNum);
                }
                return true;
            }
        }
        //编译每一行，判断每一行中的单词类型，并加入到词汇表中
        private bool wordCompile(string str, int lineNum)
        {
            bool check = true;//是否要检查该单词的类别了
            string tmp = "";//单词
            int len = str.Length;//每行代码的长度
            bool assign = false;//赋值运算符
            bool operatorFlag = false;//普通运算符
            bool greatFlag = false;
            for (int i = 0; i < len; i++)
            {
                char ch = str[i];//第i个字符
                //过滤空行
                if (ch == ' ' || ch == '\n' || ch == '\r' || ch == '\t')
                {
                    assign = false;
                    greatFlag = false;
                    if (!check && tmp != "")
                    {
                        bool cnt = wordJudge(tmp, lineNum);//检查该单词
                        check = true;//已经检查过了
                        tmp = "";
                    }
                }
                //运算符
                else if (ch == '+' || ch == '-' || ch == '*' || ch == '/')
                {
                    assign = false;
                    greatFlag = false;
                    operatorFlag = true;
                    if (!check && tmp != "")
                    {
                        bool cnt = wordJudge(tmp, lineNum);
                        check = true;
                        tmp = "";
                    }
                    int tt = 21;
                    if (ch == '+')
                        tt = 18;
                    else if (ch == '-')
                        tt = 19;
                    else if (ch == '*')
                        tt = 20;
                    insertWordList(tt, ch.ToString(), lineNum);//插入单词列表中，18-21代表加减乘除
                }
                else if (ch == '=')
                {
                    if (!check && tmp != "")
                    {
                        bool cnt = wordJudge(tmp, lineNum);
                        check = true;
                        tmp = "";
                    }
                    if (i > 0 && str[i - 1] == ':')//判断:=
                    {
                        assign = false;
                        insertWordList(22, ":=", lineNum);
                    }
                    else if (i > 0 && (str[i - 1] == '<' || str[i - 1] == '>'))//判断<=,>=
                    {
                        greatFlag = false;
                        if (str[i - 1] == '<')
                            insertWordList(23, "<=", lineNum);
                        else if (str[i - 1] == '>')
                            insertWordList(24, ">=", lineNum);
                    }
                    else//判断=
                    {
                        insertWordList(25, "=", lineNum);
                    }
                    operatorFlag = true;
                }
                else if (ch == ':')
                {
                    if (i < len - 2 && str[i + 1] == '=')//判断:=
                    {
                        assign = true;
                    }
                    else//判断不存在的字符
                    {
                        textBox2.Text += "第"+lineNum.ToString()+ " 行字符 :  输入不存在的字符";
                        textBox2.Text += Environment.NewLine;
                    }
                    operatorFlag = true;
                }
                else if (ch == '>' || ch == '<')
                {
                    operatorFlag = true;
                    if (!check && tmp != "")
                    {
                        bool cnt = wordJudge(tmp, lineNum);
                        check = true;
                        tmp = "";
                    }
                    greatFlag = true;
                    if (i > 0 && str[i - 1] == '<' && str[i] == '>')//判断<>
                    {
                        insertWordList(33, "<>", lineNum);
                    }
                    else if (i == len - 1 || (str[i + 1] != '=' && (str[i] == '>' || (str[i] == '<' && str[i + 1] != '>'))))//判断>和<
                    {
                        //textBox2.Text = str[i + 1].ToString();
                        if (ch == '<')
                            insertWordList(26, "<", lineNum);
                        else
                            insertWordList(27, ">", lineNum);
                    }
                }
                else if (ch == '.')//判断.
                {
                    if (!check && tmp != "")
                    {
                        bool cnt = wordJudge(tmp, lineNum);
                        check = true;
                        tmp = "";
                    }
                    insertWordList(28, ".", lineNum);
                }
                else if (ch == ',' || ch == ';' || ch == '(' || ch == ')')//判断,;,(,)等操作符
                {
                    operatorFlag = true;
                    if (!check && tmp != "")
                    {
                        bool cnt = wordJudge(tmp, lineNum);
                        check = true;
                        tmp = "";
                    }
                    if (ch == ',')
                        insertWordList(29, ",", lineNum);
                    else if (ch == ';')
                        insertWordList(30, ";", lineNum);
                    else if (ch == '(')
                        insertWordList(31, "(", lineNum);
                    else
                        insertWordList(32, ")", lineNum);
                }
                else if (ch >= '0' && ch <= '9')//判断数字
                {
                    tmp += ch.ToString();
                    check = false;
                    assign = false;
                    operatorFlag = false;
                    greatFlag = false;
                }
                else if (ch >= 'a' && ch <= 'z')//小写字母
                {
                    tmp += ch.ToString();
                    check = false;
                    assign = false;
                    operatorFlag = false;
                    greatFlag = false;
                }
                else if (ch >= 'A' && ch <= 'Z')//大写字母
                {
                    tmp += ch.ToString();
                    check = false;
                    assign = false;
                    operatorFlag = false;
                    greatFlag = false;
                }
                else//其它错误字符
                {
                    textBox2.Text += "第" + lineNum.ToString() + " 行字符"+ch.ToString()+"不存在!";
                    textBox2.Text += Environment.NewLine;
                }
                if (i == len - 1)
                {
                    if (!check && tmp != "")
                    {
                        bool cnt = wordJudge(tmp, lineNum);
                        check = true;
                        tmp = "";
                    }
                }
            }
            return true;
        }
        private bool gen(int x, int y, int z)//保存pcode
        {
            if (pcodeNum >= 1000)//pcode数量太大
            {
                textBox2.Text = "程序太长";
                return false;
            }
            code[pcodeNum].f = x;
            code[pcodeNum].l = y;
            code[pcodeNum].a = z;
            pcodeNum++;
            cx++;
            return true;
        }
        private bool test(ref bool[] a, ref bool[] b, int id)//检查当前符号是否属于调用该语法单位时应有的后跟符号集合
        {
            if (!a[sym])
            {
                //当检测不通过时，不停地获取符号，直到它属于需要的集合
                dealError(id, linePos, ss);
                while (!b[sym])
                {
                    if (!getsym())
                        return false;
                }
            }
            return true;
        }
        private bool expression(ref bool[] fs, int level, ref int tx)//处理表达式
        {
            bool[] foll = new bool[symNum];
            if (sym == 18 || sym == 19)
            {
                int tmp = sym;
                if (!getsym())
                    return false;
                for (int i = 0; i < 55; i++)
                    foll[i] = fs[i];
                foll[18] = true;
                foll[19] = true;
                term(ref foll, ref tx, level);
                if (tmp == 19)
                    gen(1, 0, 1);
            }
            else
            {
                for (int i = 0; i < 55; i++)
                    foll[i] = fs[i];
                foll[18] = true;
                foll[19] = true;
                term(ref foll, ref tx, level);
            }
            while (sym == 18 || sym == 19)
            {
                int tmp = sym;
                if (!getsym())
                    return false;
                for (int i = 0; i < 55; i++)
                    foll[i] = fs[i];
                foll[18] = true;
                foll[19] = true;
                term(ref foll, ref tx, level);
                if (tmp == 18)
                    gen(1, 0, 2);
                else
                    gen(1, 0, 3);
            }
            return true;
        }
        private bool condition(ref bool[] fs, ref int tx, int level)//处理condition
        {
            bool[] foll = new bool[symNum];
            if (sym == 3)
            {
                if (!getsym())
                    return false;
                expression(ref fs, level, ref tx);
                if (!gen(1, 0, 6))
                    return false;
            }
            else
            {
                for (int i = 0; i < symNum; i++)
                    foll[i] = fs[i];
                foll[23] = true;
                foll[24] = true;
                foll[25] = true;
                foll[26] = true;
                foll[27] = true;
                foll[33] = true;
                expression(ref foll, level, ref tx);
                if (sym != 23 && sym != 24 && sym != 25 && sym != 26 && sym != 27 && sym != 33)
                {
                    dealError(20, linePos, ss);
                }
                else
                {
                    int temp = sym;
                    if (!getsym())
                        return false;
                    expression(ref fs, level, ref tx);
                    switch (temp)
                    {
                        case 23:
                            gen(1, 0, 13);
                            break;
                        case 24:
                            gen(1, 0, 11);
                            break;
                        case 25:
                            gen(1, 0, 8);
                            break;
                        case 26:
                            gen(1, 0, 10);
                            break;
                        case 27:
                            gen(1, 0, 12);
                            break;
                        case 33:
                            gen(1, 0, 9);
                            break;
                    }
                }
            }
            return true;
        }
        private bool term(ref bool[] fs, ref int tx, int level)//处理term
        {
            bool[] foll = new bool[symNum];
            for (int i = 0; i < 55; i++)
                foll[i] = fs[i];
            foll[20] = true;
            foll[21] = true;
            foll[17] = true;
            factor(ref foll, ref tx, level);
            while (sym == 20 || sym == 21)
            {
                if (sym == 20)
                {
                    if (!getsym())
                        return false;
                    factor(ref foll, ref tx, level);
                    gen(1, 0, 4);
                }
                else
                {
                    if (!getsym())
                        return false;
                    factor(ref foll, ref tx, level);
                    gen(1, 0, 5);
                }
            }
            if (sym != 30 && linePos == 8)
            {
                dealError(26, linePos, ss);
                if (!getsym())
                    return false;
            }
            return true;
        }
        private bool factor(ref bool[] fs, ref int tx, int level)//处理factor
        {
            bool[] foll = new bool[symNum];
            test(ref factorPre, ref fs, 24);
            if (sym == 16)//标识符
            {
                int pos = position(ss, tx);
                if (pos == -1)
                    dealError(11, linePos, ss);
                else
                {
                    switch (sign[pos].type)
                    {
                        case 0:
                            gen(0, 0, sign[pos].val);
                            break;
                        case 1:
                            gen(2, level - sign[pos].level, sign[pos].addr);
                            break;
                        case 2:
                            dealError(21, linePos, ss);
                            break;
                    }
                }
                if (!getsym())
                    return false;
            }
            else if (sym == 17)//无符号整数
            {
                int num = int.Parse(ss);
                if (num > 20000005)
                {
                    dealError(31, linePos, ss);
                }
                gen(0, 0, num);
                if (!getsym())
                    return false;
            }
            else if (sym == 31)
            {
                if (!getsym())
                    return false;
                for (int i = 0; i < symNum; i++)
                    foll[i] = fs[i];
                foll[32] = true;
                expression(ref foll, level, ref tx);
                if (sym == 32)
                {
                    if (!getsym())
                        return false;
                }
                else
                {
                    dealError(22, linePos, ss);//表达式缺失右括号
                    if (linePos == 17)
                    {
                        usedNum += 8;
                        getsym();
                    }
                }
            }
            else
            {
                for (int i = 0; i < symNum; i++)
                    foll[i] = fs[i];
                test(ref fs, ref foll, 23);
            }
            return true;
        }
        private bool statement(ref bool[] fs, ref int tx, int level)//处理statement
        {
            bool[] foll = new bool[symNum];
            if (sym == 16)
            {
                int pos = position(ss, tx);
                if (pos == -1)
                {
                    dealError(11, linePos, ss);
                }
                else if (sign[pos].type != 1)
                {
                    dealError(12, linePos, ss);
                    pos = -1;
                }
                else
                {
                    if (!getsym())
                        return false;
                    if (sym == 22)
                    {
                    }
                    else
                        dealError(13, linePos, ss);
                    if (!getsym())
                        return false;
                    for (int i = 0; i < symNum; i++)
                        foll[i] = fs[i];
                    expression(ref foll, level, ref tx);
                    gen(3, level - sign[pos].level, sign[pos].addr);
                }
            }
            else if (sym == 15)
            {
                //write(tx, level);
                if (!getsym())
                    return false;
                for (int i = 0; i < symNum; i++)
                    foll[i] = fs[i];
                foll[32] = true;
                foll[29] = true;
                write(ref foll, ref tx, level);
            }
            else if (sym == 14)
                read(tx, level);
            else if (sym == 9)//call
            {
                if (!getsym())
                    return false;
                if (sym == 16)
                {
                    int pos = position(ss, tx);
                    if (pos == -1)
                        dealError(29, linePos, ss);
                    else if (sign[pos].type != 2)
                    {
                        dealError(15, linePos, ss);
                    }
                    else
                    {
                        gen(4, level - sign[pos].level, sign[pos].addr);
                    }
                    if (!getsym())
                        return false;
                }
                else
                {
                    dealError(14, linePos, ss);
                    if (!getsym())
                        return false;
                }
            }
            else if (sym == 7)//while
            {
                int t1 = cx;
                if (!getsym())
                    return false;
                for (int i = 0; i < symNum; i++)
                    foll[i] = fs[i];
                foll[8] = true;
                condition(ref fs, ref tx, level);
                int t2 = cx;
                gen(7, 0, 0);
                if (sym == 8)
                {
                    if (!getsym())
                        return false;
                }
                else
                    dealError(18, linePos, ss);
                statement(ref foll, ref tx, level);
                gen(6, 0, t1);
                code[t2].a = cx;
            }
            else if (sym == 4)//if
            {
                if (!getsym())
                    return false;
                for (int i = 0; i < symNum; i++)
                    foll[i] = fs[i];
                foll[5] = true;
                condition(ref foll, ref tx, level);
                if (sym == 5)
                {
                    if (!getsym())
                        return false;
                }
                else
                {
                    dealError(16, linePos, ss);
                    if (!getsym())
                        return false;
                }
                int cur = cx;
                gen(7, 0, 0);
                statement(ref foll, ref tx, level);
                code[cur].a = cx;
                if (sym == 6)
                {
                    code[cur].a++;
                    if (!getsym())
                        return false;
                    gen(6, 0, 0);
                    int temp = cx - 1;
                    statement(ref foll, ref tx, level);
                    code[temp].a = cx;
                }
            }
            else if (sym == 10)//begin
            {
                if (!getsym())
                    return false;
                for (int i = 0; i < symNum; i++)
                    foll[i] = fs[i];
                foll[30] = true;
                foll[11] = true;
                statement(ref foll, ref tx, level);
                while (statePre[sym] || sym == 30)
                {
                    if (sym == 30)
                    {
                        if (!getsym())
                            return false;
                    }
                    else
                    {
                        dealError(10, linePos, ss);
                    }
                    statement(ref foll, ref tx, level);
                }
                if (sym == 11)
                {
                    if (!getsym())
                        return false;
                }
                else
                    dealError(17, linePos, ss);
            }
            else if (sym == 12)//repeat
            {
                if (!getsym())
                    return false;
                int cur = cx;
                for (int i = 0; i < symNum; i++)
                    foll[i] = fs[i];
                foll[30] = true;
                foll[13] = true;
                statement(ref foll, ref tx, level);
                while (statePre[sym] || sym == 30)
                {
                    if (sym == 30)
                    {
                        if (!getsym())
                            return false;
                    }
                    else
                        dealError(10, linePos, ss);
                    statement(ref foll, ref tx, level);
                }
                if (sym == 13)
                {
                    if (!getsym())
                        return false;
                    condition(ref foll, ref tx, level);
                    gen(7, 0, cur);
                }
                else
                    dealError(36, linePos, ss);
            }
            return true;
        }
        private bool vardeclaration(ref int tx, int level, ref int dx)//处理var声明语句
        {
            if (sym == 16)
            {
                enter(1, ref tx, level, ref dx, 1, ss);//登记符号表
            }
            else
                dealError(4, linePos, ss);
            if (!getsym())
                return false;
            return true;
        }
        private bool constdeclaration(ref int tx, int level, ref int dx)//处理const声明语句
        {
            if (sym == 16)//标识符
            {
                string sTmp = ss;
                if (!getsym())//只有 const 标识符
                    return false;
                if (sym == 22 || sym == 25)
                {
                    if (sym == 22)//：=
                        dealError(1, linePos, ss);
                    if (!getsym())
                        return false;
                    if (sym == 17)//整数
                    {
                        enter(0, ref tx, level, ref dx, int.Parse(ss), sTmp);
                    }
                    else//const后面不是整数
                    {
                        dealError(2, linePos, ss);
                    }
                }
                else
                    dealError(3, linePos, ss);
            }
            else//后面不是
                dealError(4, linePos, ss);
            if (!getsym())
                return false;
            return true;
        }

        private bool write(ref bool []fs, ref int tx, int level)//处理write语句
        {
            if (sym ==31)//左括号
            {
                do
                {
                    getsym();
                    expression(ref fs, level, ref tx);
                    //expression(['(', ','] + fsys);
                    gen(8, 0, 0);
                } while (sym == 29);
                if (sym != 32)
                    dealError(22,linePos,ss);
                getsym();
            }
            else
                dealError(40, linePos, ss);
            return true;
        }
        private bool read(int tx, int level)//处理read语句
        {
            if (!getsym())
                return false;
            if (sym != 31)
                dealError(40, linePos, ss);
            do
            {
                getsym();
                if (sym != 16)
                {
                    dealError(41, linePos, ss);
                }
                int pos = position(ss, tx);
                if (pos == -1)
                    dealError(11, linePos, ss);
                else if (sign[pos].type != 1)
                    dealError(37, linePos, ss);
                else
                {
                    int addr = sign[pos].addr;
                    gen(9, level - sign[pos].level, addr);
                }
                getsym();
            } while (sym == 29);
            if (sym == 32)//不是右括号
            {
                if (!getsym())
                    return false;
            }
            else
                dealError(22, linePos, ss);
            return true;
        }
        //子程序语法分析
        private bool block(ref bool[] fs, int level, ref int tx)
        {
            int dx = 3;
            int ttmp = tx;
            sign[tx].addr = cx;
            if (!gen(6, 0, 0))
                return false;
            bool[] foll = new bool[symNum];
            if (level > 5)//符号表层次超过5
            {
                dealError(32, linePos, ss);
                return false;
            }
            int varNum = 0, conNum = 0, proNum = 0;//var的声明，const的声明，procedure的声明
            do
            {
                if (sym == 1)//变量类型声明处理
                {
                    if (proNum > 0)
                        dealError(44, linePos, ss);
                    if (varNum > 0)
                        dealError(45, linePos, ss);
                    varNum++;
                    if (!getsym())//只有var，后面没有单词
                        return false;
                    vardeclaration(ref tx, level, ref dx);
                    while (sym == 29)
                    {
                        if (!getsym())
                            return false;
                        vardeclaration(ref tx, level, ref dx);
                    }
                    if (sym == 30)
                    {
                        if (!getsym())
                            return false;
                    }
                    else
                        dealError(5, linePos, ss);
                }
                else if (sym == 0)//const类型声明处理
                {
                    if (varNum > 0 || proNum > 0)
                        dealError(42, linePos, ss);
                    if (conNum > 0)
                        dealError(43, linePos, ss);
                    conNum++;
                    if (!getsym())//只有const，后面没有单词了
                        return false;
                    constdeclaration(ref tx, level, ref dx);
                    while (sym == 29)
                    {
                        if (!getsym())
                            return false;
                        constdeclaration(ref tx, level, ref dx);
                    }
                    if (sym == 30)
                    {
                        if (!getsym())
                            return false;
                    }
                    else
                        dealError(5, linePos, ss);
                }
                while (sym == 2)//过程声明处理
                {
                    proNum++;
                    if (!getsym())//只有procedure，后面没有单词
                        return false;
                    int tdx = -1;
                    if (sym == 16)
                    {
                        enter(2, ref tx, level, ref dx, 1, ss);//登记符号表
                        tdx = tx;
                        if (!getsym())
                            return false;
                    }
                    else
                        dealError(4, linePos, ss);
                    if (sym == 30)
                    {
                        if (!getsym())
                            return false;
                    }
                    else
                        dealError(5, linePos, ss);
                    for (int i = 0; i < symNum; i++)
                        foll[i] = fs[i];
                    foll[30] = true;
                    if (!block(ref foll, level + 1, ref tx))
                        return false;
                    if (sym == 30)
                        getsym();
                    else
                        dealError(5, linePos, ss);
                    for (int i = 0; i < symNum; i++)
                        foll[i] = fs[i];
                    foll[16] = true;
                    foll[2] = true;
                    if (!test(ref foll, ref fs, 6))
                        return false;
                }
            }while (decPre[sym]);
            code[sign[ttmp].addr].a = cx;
            sign[ttmp].addr = cx;
            sign[ttmp].Size = dx;
            gen(5, 0, dx);

            for (int i = 0; i < symNum; i++)
                foll[i] = fs[i];
            foll[30] = true;
            foll[11] = true;
            statement(ref foll, ref tx, level);
            gen(1, 0, 0);
            for (int i = 0; i < symNum; i++)
                foll[i] = false;
            if (!test(ref fs, ref foll, 8))
                return false;
            
            tx = ttmp;
            return true;
        }
        private bool getsym()//取单词
        {
            if (usedNum > wordNum - 1)//单词取完了
                return false;
            ss = wordList[usedNum].name;//下一个单词名
            linePos = wordList[usedNum].line;//下一个单词的行号
            sym = wordList[usedNum++].type;//下一个单词的类型
            return true;
        }
        private int Base(int l, int[] sta, int bp)
        {
            while (l > 0)
            {
                bp = sta[bp];
                l--;
            }
            return bp;
        }

        //解释程序
        private void interpret()
        {
            int[] sta = new int[305];
            for (int i = 0; i < 305; i++)
                sta[i] = 0;
            int pc = 0, bp = 1, sp = 0;
            do
            {
                pCode currentCode = code[pc++];
                switch (currentCode.f)
                {
                    case 0://取常量与栈顶
                        sp++;
                        sta[sp] = currentCode.a;
                        break;
                    case 1://计算
                        switch (currentCode.a)
                        {
                            case 0://函数调用返回
                                sp = bp-1;
                                pc = sta[sp + 3];
                                bp = sta[sp + 2];
                                break;
                            //负号，加减乘除
                            case 1:
                                sta[sp] = -sta[sp];
                                break;
                            case 2:
                                sp--;
                                sta[sp] += sta[sp+1];
                                break;
                            case 3:
                                sp--;
                                sta[sp] -= sta[sp+1];
                                break;
                            case 4:
                                sp--;
                                sta[sp]*=sta[sp+1];
                                break;
                            case 5:
                                sp--;
                                sta[sp] /= sta[sp+1];
                                break;
                            case 6://奇偶判断
                                sta[sp] %= 2;
                                break;
                            case 8:  //判断相等                                                          
                                sp--;
                                sta[sp] = (sta[sp] == sta[sp+1] ? 1 : 0);
                                break;
                            case 9:  //判断不等                                                              
                                sp--;
                                sta[sp] = (sta[sp] != sta[sp +1] ? 1 : 0);
                                break;
                            case 10: //判断小于                                                             
                                sp--;
                                sta[sp] = (sta[sp] < sta[sp+1] ? 1 : 0);
                                break;
                            case 11:  //判断>=                                                     
                                sp--;
                                sta[sp] = (sta[sp] >= sta[sp+1] ? 1 : 0);
                                break;
                            case 12: //判断>                                                               
                                sp--;
                                sta[sp] = (sta[sp] > sta[sp+1] ? 1 : 0);
                                break;
                            case 13:   //判断<=                                                             
                                sp--;
                                sta[sp] = (sta[sp] <= sta[sp+1] ? 1 : 0);
                                break;
                            default:
                                break;
                        }
                        break;
                    case 2:     //取变量值于栈顶                                    
                        sp++;
                        sta[sp] = sta[Base(currentCode.l, sta, bp) + currentCode.a];
                        break;
                    case 3://栈顶值存在变量
                        sta[Base(currentCode.l, sta, bp) + currentCode.a] = sta[sp];
                        sp--;
                        break;
                    case 4://调用过程
                        sta[sp+1] = Base(currentCode.l, sta, bp);
                        sta[sp + 2] = bp;
                        sta[sp + 3] = pc;
                        bp = sp+1;
                        pc = currentCode.a;
                        break;
                    case 5://分配空间，指针+a
                        sp += currentCode.a;
                        break;
                    case 6://无条件跳转a
                        pc = currentCode.a;
                        break;
                    case 7://条件跳转a
                        if (sta[sp] == 0)
                        {
                            pc = currentCode.a;
                        }
                        sp--;
                        break;
                    case 8://写数据
                        textBox2.Text += sta[sp].ToString();
                        textBox2.Text += Environment.NewLine;
                        sp++;
                        break;
                    case 9://读数据
                        if(input.Length>ttop)
                        {
                            int tmp = int.Parse(input[ttop]);
                            ttop++;
                            sta[sp] = tmp;
                            sta[Base(currentCode.l, sta, bp) + currentCode.a] = sta[sp];
                        }
                        else
                        {
                            MessageBox.Show("请输入");
                            return;
                        }
                        break;
                }
            } while (pc != 0);

        }
        public Form1()
        {
            InitializeComponent();
        }
        
        public static void printError(int n)
        {
            string str = "Error(" + (lc - 1).ToString() + "," +cc.ToString()
                + "): 错误编号：" + n.ToString()
                + "，错误信息：" + errmsg[n];
            //fprintf(fallerror, "%s\n", str.c_str());输出错误信息文件
            err++;
        }
        private void label1_Click(object sender, EventArgs e)
        {
        }
        private void Form1_Load(object sender, EventArgs e)
        {
        }
        private void buttoncompile_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            File.WriteAllText("incode.txt", textBoxCode.Text.ToString());//将代码框的代码保存
            listView1.Items.Clear();//清空listview
            textBox2.Text = "";//清空输出框
            init();

            int lineNum = 1;//第几行
            bool cnt = true;
            if(textBoxCode.Text.ToString()=="")
            {
                MessageBox.Show("请输入代码或导入代码文件！");
                return;
            }
            foreach (string nextLine in textBoxCode.Lines)//对代码框里的每一行代码进行词法分析
            {
                cnt = wordCompile(nextLine, lineNum);
                lineNum++;
            }
            if (getsym())//取下一个单词，如果没有取完
                block(ref follow, 0, ref tx);
            if (sym != 28)
                dealError(9, linePos, ss);
            Console.WriteLine(cx);
            //将pcode结果显示
            string[][] itemresult = new string[cx + 1][];
            for(int i=0;i<cx;i++)
            {
                itemresult[i] = new string[4];
                itemresult[i][0] = (i + 1).ToString();
                itemresult[i][1] = codeF[code[i].f].ToString();
                itemresult[i][2] = code[i].l.ToString();
                itemresult[i][3] = code[i].a.ToString();
                ListViewItem item = new ListViewItem(itemresult[i]);
                listView1.Items.Add(item);
            }
            for (int i = 0; i < cx; i++)
            {
                if (code[i].f == 20)
                    continue;
                Console.WriteLine((i + 1).ToString() + " " + codeF[code[i].f] + " " + code[i].l.ToString() + " " + code[i].a.ToString());
            }
            if (!isError)
            {
                isCompile = true;
                textBox2.Text += "编译成功!";
                textBox2.Text += Environment.NewLine;
                textBox2.Text += "若有输入，请输入之后再运行";
            }
            else
                isCompile = false;
        }
        private void buttonimport_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Title = "请选择文件";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                openfile = fileDialog.FileName;//返回文件的完整路径  
                textBoxCode.Text = openfile.ToString();
                textBoxCode.Text = File.ReadAllText(openfile, Encoding.Default);
                //code = textBoxCode.Lines;
            }
        }
        private void buttonrun_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";
            if (isCompile)
            {
                string str = textBox1.Text;
                ttop = 0;
                input = str.Split(new char[] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                interpret();
            }
            else
                MessageBox.Show("请先正确编译");
        }
        private void textBoxCode_TextChanged(object sender, EventArgs e)
        {
        }
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
        }
    }
}