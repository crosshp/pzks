using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections;

namespace PZKS2
{
    public partial class Form1 : Form
    {
        int Active = -1; int GA = -1;  // номер активного елементу
        int First = -1, Second = -1;   // вх. та вих. звязок, тип звязку
        int MoveNode = -1;             //номер елементу, що пересуваємо
        Point MousePos, MouseInNode;

        NodeT[] Nodes = new NodeT[10];
        int[,] Lines = new int[10, 10];
        int NumNode = 0;

        int[] TaskList;

        Queue queue13;
        Queue queue3;
        Queue queue4;

        public Form1()
        {
            InitializeComponent();
            assignAlgCmb.Items.AddRange(new object[] { "2", "5" });
            assignAlgCmb.SelectedIndex = 0;
            queeBuildAlg.Items.AddRange(new object[] { "1", "9", "13" });
            queeBuildAlg.SelectedIndex = 0;
        }

        ////////////////////////////////////////////////////////////////////////////////////
        ///////////////////        TASK        /////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////

        //Метод обробки натискання клавіші миші на поле ГЗ
        private void GTask1_MouseUp(object sender, MouseEventArgs e)
        {
            MousePos.X = e.X; MousePos.Y = e.Y;
            if (toolStripButton3.Checked) AddNode(MousePos.X, MousePos.Y, Convert.ToInt16(elemTextTxt.Text));
            First = -1; Second = -1;
        }

        //Метод створення елемента
        public void AddNode(int x, int y, int mass)
        {
            if (NumNode >= Nodes.Count())
            {
                NodeT[] Nodes_ = new NodeT[Nodes.Length + 5];
                for (int i = 0; i < Nodes.Length; i++) Nodes_[i] = Nodes[i];
                Nodes = Nodes_;

                int[,] Lines_ = new int[Nodes.Length + 5, Nodes.Length + 5];
                for (int i = 0; i < Math.Sqrt(Lines.Length); i++)
                    for (int j = 0; j < Math.Sqrt(Lines.Length); j++)
                        Lines_[i, j] = Lines[i, j];
                Lines = Lines_;
            }

            Nodes[NumNode] = new NodeT(NumNode, mass);
            Nodes[NumNode].Click += new System.EventHandler(this.Node_Click);
            Nodes[NumNode].KeyDown += new KeyEventHandler(this.Node_KeyDown);
            Nodes[NumNode].MouseDown += new MouseEventHandler(Node_MouseDown);
            Nodes[NumNode].MouseUp += new MouseEventHandler(Node_MouseUp);
            Nodes[NumNode].MouseMove += new MouseEventHandler(Node_MouseMove);

            Nodes[NumNode].Location = new System.Drawing.Point(x, y);
            Nodes[NumNode].Size = new System.Drawing.Size(40, 40);
            Nodes[NumNode].Color = Color.Black;

            this.gTask1.Controls.Add(Nodes[NumNode]);
            NumNode++;
        }

        //Метод обробки натискання клавіші миші на будь-який елемент
        private void Node_Click(object sender, EventArgs e)
        {
            toolStripButton3.Checked = false;
            Active = ((NodeT)sender).number; GA = 1;
            gTask1.Refresh();
            if (toolStripButton4.Checked)
            {
                if (First == -1) First = ((NodeT)sender).number;
                else
                {
                    Second = ((NodeT)sender).number;
                    Form2 F2 = new Form2();
                    F2.label1.Text = "Введите вес связи между вершинами" + First.ToString() + " и " + Second.ToString() + " :";
                    F2.ShowDialog();
                    if (DialogResult.OK == F2.DialogResult) Lines[First, Second] = Convert.ToInt16(F2.textBox1.Text);
                    else Lines[First, Second] = 1; F2.Dispose();

                    gTask1.LinesUpdate(Nodes, Lines, NumNode);
                    First = -1; Second = -1;
                    gTask1.Refresh();
                }
            }
            if (toolStripButton5.Checked)
            {
                if (First == -1) First = ((NodeT)sender).number;
                else
                {
                    Second = ((NodeT)sender).number;
                    Lines[First, Second] = 0;
                    gTask1.LinesUpdate(Nodes, Lines, NumNode);
                    First = -1; Second = -1;
                    gTask1.Refresh();
                }
            }
            int buf = ((NodeT)sender).mass;
            elemTextTxt.Text = buf.ToString();
        }

        //Метод виділення елемента
        private void Node_MouseDown(object sender, MouseEventArgs e)
        {
            ((NodeT)sender).Focus(); Refresh();
            MoveNode = ((NodeT)sender).number;
            MousePos = e.Location;
        }

        //Методи зміни положення елемента
        private void Node_MouseUp(object sender, MouseEventArgs e)
        {
            if (MoveNode != -1)
            {
                int X_pos = Math.Max(1, Nodes[MoveNode].Location.X + e.X - MousePos.X);
                int Y_pos = Math.Max(1, Nodes[MoveNode].Location.Y + e.Y - MousePos.Y);
                Nodes[MoveNode].Location = new Point(X_pos, Y_pos);
            }
            MoveNode = -1;
        }
        private void Node_MouseMove(object sender, MouseEventArgs e)
        {
            MouseInNode.X = e.X; MouseInNode.Y = e.Y;
            if (MoveNode != -1)
            {
                int X_pos = Math.Max(1, Nodes[MoveNode].Location.X + e.X - MousePos.X);
                int Y_pos = Math.Max(1, Nodes[MoveNode].Location.Y + e.Y - MousePos.Y);
                Nodes[MoveNode].Location = new Point(X_pos, Y_pos);
                gTask1.Invalidate();
            }
        }

        //Метод видалення виділеного елемента
        private void Node_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 46)
            {
                if (((NodeT)sender).number == NumNode - 1)
                {
                    ((NodeT)sender).Dispose();
                    for (int i = 0; i < Math.Sqrt(Lines.Length); i++)
                    {
                        Lines[i, NumNode - 1] = 0;
                        Lines[NumNode - 1, i] = 0;
                    }
                    NumNode--;
                }
                else
                {
                    Nodes[((NodeT)sender).number] = Nodes[NumNode - 1];
                    Nodes[NumNode - 1] = null;
                    Nodes[((NodeT)sender).number].number = ((NodeT)sender).number;
                    for (int i = 0; i < Math.Sqrt(Lines.Length); i++)
                        Lines[((NodeT)sender).number, i] = Lines[NumNode - 1, i];
                    for (int i = 0; i < Math.Sqrt(Lines.Length); i++)
                        Lines[i, ((NodeT)sender).number] = Lines[i, NumNode - 1];
                    for (int i = 0; i < Math.Sqrt(Lines.Length); i++)
                    {
                        Lines[i, NumNode - 1] = 0; Lines[NumNode - 1, i] = 0;
                    }
                    ((NodeT)sender).Dispose(); NumNode--;
                }
                gTask1.LinesUpdate(Nodes, Lines, NumNode);
                gTask1.Invalidate();
            }
        }

        //Метод скидання виділення блоку
        private void gTask1_Click(object sender, EventArgs e)
        {
            gTask1.Focus(); gTask1.Refresh();
        }

        //Метод варифікації введення данних у блоки.
        private void elemTextTxt_KeyDown(object sender, KeyEventArgs e)
        {
            string text;
            string input = "[0-9]";
            Regex regex = new Regex(input);
            MatchCollection matches = null;

            if (Active >= 0)
            {
                if (e.KeyValue == 13)
                {
                    text = elemTextTxt.Text;
                    matches = regex.Matches(text);

                    if (matches.Count != text.Length || matches.Count == 0)
                    {
                        MessageBox.Show("Данные не верны. Возможно лишь числовое значение", "Ошибка");
                        elemTextTxt.Text = "";
                    }
                    else
                    {
                        if (GA == 1)
                        {
                            Nodes[Active].mass = Convert.ToInt16(elemTextTxt.Text);
                            Nodes[Active].Refresh(); gTask1.Invalidate();
                        }
                        if (GA == 2)
                        {
                            NodeSys[Active].mass = Convert.ToInt16(elemTextTxt.Text);
                            NodeSys[Active].Refresh(); gTask1.Invalidate();
                        }
                    }
                }
            }
        }

        //Метод кнопки "Новий ГЗ"

        //Методи збереження та відновлення ГЗ
        private void графЗадачіToolStripMenuItem1_Click(object sender, EventArgs e)
        { openTask.ShowDialog(); }
        private void графЗадачіToolStripMenuItem_Click(object sender, EventArgs e)
        { saveTask.ShowDialog(); }

        private void openTask_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                int count;
                StreamReader SR = new StreamReader(openTask.FileName);
                String[] Ln = SR.ReadLine().Split('/'); count = Ln.Length;
                Lines = new int[count, count]; Nodes = new NodeT[count];

                for (int i = 0; i < count; i++)
                    Lines[0, i] = Convert.ToInt32(Ln[i]);

                for (int i = 1; i < count; i++)
                {
                    Ln = SR.ReadLine().Split('/');
                    for (int j = 0; j < count; j++)
                        Lines[i, j] = Convert.ToInt32(Ln[j]);
                }
                SR.ReadLine();

                for (int i = 0; i < count; i++)
                {
                    Ln = SR.ReadLine().Split('/');
                    AddNode(Convert.ToInt32(Ln[2]), Convert.ToInt32(Ln[3]), Convert.ToInt32(Ln[1]));
                    Nodes[i].number = i;
                }
                gTask1.LinesUpdate(Nodes, Lines, NumNode);
                gTask1.Update(); gTask1.Invalidate();
            }
            catch (Exception) { MessageBox.Show("Файл поврежден!", "Error"); }
        }
        private void saveTask_FileOk(object sender, CancelEventArgs e)
        {
            StreamWriter SW = new StreamWriter(saveTask.FileName);
            for (int i = 0; i < NumNode; i++)
            {
                for (int j = 0; j < NumNode - 1; j++)
                    SW.Write(Lines[i, j] + "/");
                SW.WriteLine(Lines[i, NumNode - 1]);
            }
            SW.WriteLine();
            for (int i = 0; i < NumNode; i++)
                SW.WriteLine(Nodes[i].number + "/" + Nodes[i].mass + "/" + Nodes[i].Location.X + "/" + Nodes[i].Location.Y);
            SW.Close();
        }

        int Fault = 0;
        private Boolean CheckTask()
        {
            Boolean InfExsist = false;
            for (int i = 0; i < NumNode; i++)
            {
                int[] check = new int[NumNode];
                InfExsist = DFS(Nodes[i].number, check, Lines);

                if (InfExsist) { Fault = i; return false; }
            }
            return true;
        }

        //Алгоритм пошуку в глибину.
        private Boolean DFS(int v, int[] check, int[,] Lines)
        {
            if (check[v] == 1) return true;
            if (check[v] == 2) return false;
            check[v] = 1;
            for (int i = 0; i < NumNode; i++)
            {
                if (Lines[v, i] != 0)
                    if (DFS(Nodes[i].number, check, Lines))
                        return true;
            }
            check[v] = 2;
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////
        ///////////////////       SYSTEM       /////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////
        NodeS[] NodeSys = new NodeS[10];
        int[,] LineSys = new int[10, 10];
        int NumNodeSys = 0;

        //Метод обробки натискання клавіші миші на поле ГC
        private void gSystem1_MouseUp(object sender, MouseEventArgs e)
        {
            MousePos.X = e.X; MousePos.Y = e.Y;
            if (toolStripButton3.Checked) AddNodeSys(MousePos.X, MousePos.Y, Convert.ToInt16(elemTextTxt.Text));
            First = -1; Second = -1;
        }

        //Метод створення елемента
        public void AddNodeSys(int x, int y, int mass)
        {
            if (NumNodeSys >= NodeSys.Count())
            {
                NodeS[] NodeSys_ = new NodeS[NodeSys.Length + 2];
                for (int i = 0; i < NodeSys.Length; i++) NodeSys_[i] = NodeSys[i];
                NodeSys = NodeSys_;

                int[,] LineSys_ = new int[NodeSys.Length + 2, NodeSys.Length + 2];
                for (int i = 0; i < Math.Sqrt(LineSys.Length); i++)
                    for (int j = 0; j < Math.Sqrt(LineSys.Length); j++)
                        LineSys_[i, j] = LineSys[i, j];
                LineSys = LineSys_;
            }

            NodeSys[NumNodeSys] = new NodeS(NumNodeSys, mass);
            NodeSys[NumNodeSys].Click += new System.EventHandler(this.NodeS_Click);
            NodeSys[NumNodeSys].KeyDown += new KeyEventHandler(this.NodeS_KeyDown);
            NodeSys[NumNodeSys].MouseDown += new MouseEventHandler(NodeS_MouseDown);
            NodeSys[NumNodeSys].MouseUp += new MouseEventHandler(NodeS_MouseUp);
            NodeSys[NumNodeSys].MouseMove += new MouseEventHandler(NodeS_MouseMove);

            NodeSys[NumNodeSys].Location = new System.Drawing.Point(x, y);
            NodeSys[NumNodeSys].Size = new System.Drawing.Size(40, 40);
            NodeSys[NumNodeSys].Color = Color.Black;

            this.gSystem1.Controls.Add(NodeSys[NumNodeSys]);
            NumNodeSys++;
        }

        //Метод обробки натискання клавіші миші на будь-який елемент
        private void NodeS_Click(object sender, EventArgs e)
        {
            toolStripButton3.Checked = false;
            Active = ((NodeS)sender).number; GA = 2;
            gSystem1.Refresh();
            if (toolStripButton4.Checked)
            {
                if (First == -1) First = ((NodeS)sender).number;
                else
                {
                    Second = ((NodeS)sender).number;
                    Form2 F2 = new Form2();
                    F2.label1.Text = "Введите вес связи между вершинами " + First.ToString() + " и " + Second.ToString() + " :";
                    F2.ShowDialog();
                    if (DialogResult.OK == F2.DialogResult)
                    {
                        LineSys[First, Second] = Convert.ToInt16(F2.textBox1.Text);
                        LineSys[Second, First] = Convert.ToInt16(F2.textBox1.Text);
                    }
                    else { LineSys[First, Second] = 1; LineSys[Second, First] = 1; };
                    F2.Dispose();
                    gSystem1.LinesUpdate(NodeSys, LineSys, NumNodeSys);
                    First = -1; Second = -1;
                    gSystem1.Refresh();
                }
            }
            if (toolStripButton5.Checked)
            {
                if (First == -1) First = ((NodeS)sender).number;
                else
                {
                    Second = ((NodeS)sender).number;
                    LineSys[First, Second] = 0; LineSys[Second, First] = 0;
                    gSystem1.LinesUpdate(NodeSys, LineSys, NumNodeSys);
                    First = -1; Second = -1;
                    gSystem1.Refresh();
                }
            }
            int buf = ((NodeS)sender).mass;
            elemTextTxt.Text = buf.ToString();
        }

        //Метод виділення елемента
        private void NodeS_MouseDown(object sender, MouseEventArgs e)
        {
            ((NodeS)sender).Focus(); Refresh();
            MoveNode = ((NodeS)sender).number;
            MousePos = e.Location;
        }

        //Методи зміни положення елемента
        private void NodeS_MouseUp(object sender, MouseEventArgs e)
        {
            if (MoveNode != -1)
            {
                int X_pos = Math.Max(1, NodeSys[MoveNode].Location.X + e.X - MousePos.X);
                int Y_pos = Math.Max(1, NodeSys[MoveNode].Location.Y + e.Y - MousePos.Y);
                NodeSys[MoveNode].Location = new Point(X_pos, Y_pos);
            }
            MoveNode = -1;
        }
        private void NodeS_MouseMove(object sender, MouseEventArgs e)
        {
            MouseInNode.X = e.X; MouseInNode.Y = e.Y;
            if (MoveNode != -1)
            {
                int X_pos = Math.Max(1, NodeSys[MoveNode].Location.X + e.X - MousePos.X);
                int Y_pos = Math.Max(1, NodeSys[MoveNode].Location.Y + e.Y - MousePos.Y);
                NodeSys[MoveNode].Location = new Point(X_pos, Y_pos);
                gSystem1.Invalidate();
            }
        }

        //Метод видалення виділеного елемента
        private void NodeS_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 46)
            {
                if (((NodeS)sender).number == NumNodeSys - 1)
                {
                    ((NodeS)sender).Dispose();
                    for (int i = 0; i < Math.Sqrt(LineSys.Length); i++)
                    {
                        LineSys[i, NumNodeSys - 1] = 0;
                        LineSys[NumNodeSys - 1, i] = 0;
                    }
                    NumNodeSys--;
                }
                else
                {
                    NodeSys[((NodeS)sender).number] = NodeSys[NumNodeSys - 1];
                    NodeSys[NumNodeSys - 1] = null;
                    NodeS[] temp = new NodeS[NodeSys.Length];
                    for (int j = 0; j < temp.Length; j++)
                    {
                        temp[j] = NodeSys[j];
                    }
                    NodeSys = temp;
                    NodeSys[((NodeS)sender).number].number = ((NodeS)sender).number;
                    for (int i = 0; i < Math.Sqrt(LineSys.Length); i++)
                        LineSys[((NodeS)sender).number, i] = LineSys[NumNodeSys - 1, i];
                    for (int i = 0; i < Math.Sqrt(LineSys.Length); i++)
                        LineSys[i, ((NodeS)sender).number] = LineSys[i, NumNodeSys - 1];
                    for (int i = 0; i < Math.Sqrt(LineSys.Length); i++)
                    {
                        LineSys[i, NumNodeSys - 1] = 0; LineSys[NumNodeSys - 1, i] = 0;
                    }
                    ((NodeS)sender).Dispose(); NumNodeSys--;
                }
                gSystem1.LinesUpdate(NodeSys, LineSys, NumNodeSys);
                gSystem1.Invalidate();
            }
        }

        //Метод скидання виділення блоку
        private void gSystem1_Click(object sender, EventArgs e)
        {
            gSystem1.Focus(); gSystem1.Refresh();
        }

        //Метод кнопки "Новий ГC"
        private void toolStripButton8_Click(object sender, EventArgs e)
        {

        }

        //Методи збереження та відновлення ГЗ
        private void графСистемиToolStripMenuItem_Click(object sender, EventArgs e)
        { saveSys.ShowDialog(); }
        private void графСистемиToolStripMenuItem1_Click(object sender, EventArgs e)
        { openSys.ShowDialog(); }

        private void openSys_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                toolStripButton8_Click(sender, e); int count = 0;
                StreamReader SR = new StreamReader(openSys.FileName);
                String[] Ln = SR.ReadLine().Split('/'); count = Ln.Length;
                LineSys = new int[count, count]; NodeSys = new NodeS[count];

                for (int i = 0; i < count; i++)
                    LineSys[0, i] = Convert.ToInt32(Ln[i]);

                for (int i = 1; i < count; i++)
                {
                    Ln = SR.ReadLine().Split('/');
                    for (int j = 0; j < count; j++)
                        LineSys[i, j] = Convert.ToInt32(Ln[j]);
                }
                SR.ReadLine();

                for (int i = 0; i < count; i++)
                {
                    Ln = SR.ReadLine().Split('/');
                    AddNodeSys(Convert.ToInt32(Ln[2]), Convert.ToInt32(Ln[3]), Convert.ToInt32(Ln[1]));
                    NodeSys[i].number = i;
                }
                gSystem1.LinesUpdate(NodeSys, LineSys, NumNodeSys);
                gSystem1.Update(); gSystem1.Invalidate();
            }
            catch (Exception) { MessageBox.Show("Файл поврежден", "Ошибка"); }
        }
        private void saveSys_FileOk(object sender, CancelEventArgs e)
        {
            StreamWriter SW = new StreamWriter(saveSys.FileName);
            for (int i = 0; i < NumNodeSys; i++)
            {
                for (int j = 0; j < NumNodeSys - 1; j++)
                    SW.Write(LineSys[i, j] + "/");
                SW.WriteLine(LineSys[i, NumNodeSys - 1]);
            }
            SW.WriteLine();
            for (int i = 0; i < NumNodeSys; i++)
                SW.WriteLine(NodeSys[i].number + "/" + NodeSys[i].mass + "/" + NodeSys[i].Location.X + "/" + NodeSys[i].Location.Y);
            SW.Close();
        }

        //Метод перевірки звязності ГС
        int[,] Des;

        ////////////////////////////////////////////////////////////////////////////////////
        ////////////    Робочі методи знаходження КП    ////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////

        //Фомування вектору КП по часу до кінця
        int[] Tkr;
        private void TkrEND()
        {
            Tkr = new int[NumNode];
            for (int i = 0; i < NumNode; i++)
                Tkr[i] = DFSmass(i);
        }
        //Пошук сумарної ваги вниз
        private int DFSmass(int v)
        {
            int T = 0;
            for (int i = 0; i < NumNode; i++)
            {
                int buf = 0;
                if (Lines[v, i] != 0)
                    buf = DFSmass(i) + Nodes[v].mass;
                else buf = Nodes[v].mass;
                if (buf > T) T = buf;
            }
            return T;
        }

        //Формування вектору КП по вершинам (DFSnum(i, false) - вниз, DFSnum(i, true) - вгору)
        int[] NkrE, NkrB;
        private void Nkr()
        {
            NkrE = new int[NumNode]; NkrB = new int[NumNode];
            for (int i = 0; i < NumNode; i++)
            {
                NkrE[i] = DFSnum(i, false);
                NkrB[i] = DFSnum(i, true) - 1;
            }
        }
        //Пошук сумарної кількості вершин
        private int DFSnum(int v, Boolean Rev)
        {
            int L, R, N = 0;
            for (int i = 0; i < NumNode; i++)
            {
                if (Rev) { L = i; R = v; }
                else { L = v; R = i; }
                int buf = 0;
                if (Lines[L, R] != 0)
                    buf = DFSnum(i, Rev) + 1;
                else buf = 1;
                if (buf > N) N = buf;
            }
            return N;
        }

        //Формування КП граничного по кількості вершин
        int[] Ng; int CNg;
        private void NkrG()
        {
            Ng = new int[NumNode];
            int buf = 0; CNg = NkrE[0];
            for (int i = 1; i < NumNode; i++)
                if (CNg < NkrE[i]) { CNg = NkrE[i]; buf = i; }
            Ng = new int[CNg];
            Ng[0] = buf;
            for (int i = 1; i < CNg; i++)
                Ng[i] = FindNext(Ng[i - 1]);
        }

        private int FindNext(int v)
        {
            for (int i = 0; i < NumNode; i++)
                if (Lines[v, i] != 0)
                    if (NkrE[v] == NkrE[i] + 1) return i;
            return v;
        }

        private void формуванняЧергиЗадачToolStripMenuItem_Click(object sender, EventArgs e)
        { TkrEND(); Nkr(); NkrG(); }

        ////////////////////////////////////////////////////////////////////////////////////
        ////////////             Алгоритм 2             ////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////
        string str;
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            int[] Queue = new int[NumNode];
            for (int i = 0; i < NumNode; i++) Queue[i] = i;
            int[] bufT = new int[NumNode];
            bufT = Tkr; str = null;

            for (int i = 0; i < NumNode; i++)
                for (int j = i; j < NumNode; j++)
                    if (bufT[i] < bufT[j])
                    {
                        int t = bufT[i]; bufT[i] = bufT[j]; bufT[j] = t;
                        t = Queue[i]; Queue[i] = Queue[j]; Queue[j] = t;
                    }
            for (int i = 0; i < NumNode; i++)
                str += Nodes[Queue[i]].number + " (" + bufT[i] + ")  ";

            TaskList = Queue;
        }

        ////////////////////////////////////////////////////////////////////////////////////
        ////////////             Алгоритм 4             ////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////
        int[] List, bufer;
        int count = 0;
        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            int[] Queue = new int[NumNode];
            List = new int[NumNode];
            for (int i = 0; i < NumNode; i++) { Queue[i] = -1; List[i] = i; }
            int[] bufN = new int[NumNode];
            bufer = new int[NumNode];
            bufer = NkrE; str = null; count = NumNode;

            if (CNg == NumNode)
            {
                Queue = Ng;
                for (int i = 0; i < CNg; i++)
                    bufN[i] = NkrE[Queue[i]];
            }
            else
            {
                //Поставили в чергу Ngr
                for (int i = 0; i < CNg; i++)
                {
                    Queue[i] = Ng[i];
                    bufN[i] = NkrE[Queue[i]];
                    List[Queue[i]] = -1;
                }
                Last();

                //Інші вершини
                for (int i = 0; i < count; i++)
                    for (int j = i; j < count; j++)
                        if (bufer[i] < bufer[j])
                        {
                            int t = bufer[i]; bufer[i] = bufer[j]; bufer[j] = t;
                            t = List[i]; List[i] = List[j]; List[j] = t;
                        }
                for (int i = 0; i < count; i++)
                {
                    Queue[i + CNg] = List[i];
                    bufN[i + CNg] = bufer[i];
                }
            }

            for (int i = 0; i < NumNode; i++)
                str += Nodes[Queue[i]].number + " (" + bufN[i] + ")  ";
            TaskList = Queue;
        }

        private void Last()
        {
            Boolean ck = true;
            while (ck)
            {
                ck = false;
                for (int i = 0; i < count; i++)
                    if (List[i] == -1)
                    {
                        for (int j = i + 1; j < count; j++)
                        {
                            List[j - 1] = List[j];
                            bufer[j - 1] = bufer[j];
                        }
                        count--; ck = true;
                    }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////
        ////////////             Алгоритм 10            ////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////
        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            int[] Queue = new int[NumNode];
            for (int i = 0; i < NumNode; i++) Queue[i] = i;
            int[] bufN = new int[NumNode];
            bufN = NkrB; str = null;
            int[] LCount = new int[NumNode];

            for (int i = 0; i < NumNode; i++)
                for (int j = 0; j < NumNode; j++)
                {
                    if (i == j) continue;
                    if (Lines[i, j] != 0) LCount[i]++;
                    if (Lines[j, i] != 0) LCount[i]++;
                }

            for (int i = 0; i < NumNode; i++)
                for (int j = i; j < NumNode; j++)
                    if (LCount[i] < LCount[j])
                    {
                        int t = bufN[i]; bufN[i] = bufN[j]; bufN[j] = t;
                        t = Queue[i]; Queue[i] = Queue[j]; Queue[j] = t;
                        t = LCount[i]; LCount[i] = LCount[j]; LCount[j] = t;
                    }

            for (int i = 0; i < NumNode; i++)
                for (int j = i; j < NumNode; j++)
                    if ((LCount[i] == LCount[j]) && (bufN[i] > bufN[j]))
                    {
                        int t = bufN[i]; bufN[i] = bufN[j]; bufN[j] = t;
                        t = Queue[i]; Queue[i] = Queue[j]; Queue[j] = t;
                        t = LCount[i]; LCount[i] = LCount[j]; LCount[j] = t;
                    }
            for (int i = 0; i < NumNode; i++)
                str += Nodes[Queue[i]].number + " (" + LCount[i] + "," + bufN[i] + ")  ";
            TaskList = Queue;
        }



        ////////////////////////////////////////////////////////////////////////////////////
        ////////////              Шлях між вершинами    ////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////
        private void формуванняЧергиПризначенняToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Des = new int[NumNodeSys, NumNodeSys];
            for (int i = 0; i < NumNodeSys; i++)
                for (int j = 0; j < NumNodeSys; j++)
                    if (i == j) Des[i, j] = 0;
                    else
                        if (LineSys[i, j] != 0) Des[i, j] = 1;
                    else Des[i, j] = 99999;

            for (int i = 0; i < NumNodeSys; i++)
                for (int j = 0; j < NumNodeSys; j++)
                    for (int l = 0; l < NumNodeSys; l++)
                        if (Des[j, l] > Des[j, i] + Des[i, l]) Des[j, l] = Des[j, i] + Des[i, l];
        }

        int[] Path; int CPath;
        private void GetPath(int beg, int end)
        {
            CPath = Des[beg, end];
            Path = new int[CPath + 1];
            int buf = CPath;
            Path[0] = beg;
            Boolean ck = false;

            for (int i = 0; i < CPath; i++)
            {
                ck = false;
                for (int j = 0; j < NumNodeSys; j++)
                {
                    if (ck) continue;
                    if ((Des[j, end] + 1 == Des[beg, end]) && (Des[beg, j] == 1))
                    {
                        beg = j; ck = true; continue;
                    }
                }
                Path[i + 1] = beg;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////
        ////////////              Призначення 1         ////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////
        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            GetDeep1();
            // CreateTable(dataGridView1);
            //  CreateTable(dataGridView2);
            //  FillTable(dataGridView1, Proc);
            //  FillTable(dataGridView2, Send);
        }

        string[,] Proc, Send;
        int Tik;
        int[,] W;
        private void GetDeep1()
        {
            Proc = new string[1000, NumNodeSys];
            Send = new string[1000, NumNodeSys];
            Random r = new Random();
            int Done = 0;
            W = new int[3, NumNode]; //Num W, Tik Done, Proc
            int[] Process = new int[NumNodeSys];


            for (int i = 0; i < NumNode; i++)
            {
                W[0, i] = i;
                W[1, i] = -1; //W not prepared
            }

            int Wt = 0, Pt = 0, buf;
            while (Done != NumNode)
            {
                //Choose Task
                Wt = -1;
                for (int i = 0; i < NumNode; i++)
                    if (W[1, TaskList[i]] == -1)
                    {
                        buf = TaskList[i];
                        //Check preparation
                        if (CheckW(buf)) { Wt = buf; break; }
                    }
                if (Wt == -1)
                {
                    Tik++;
                    for (int i = 0; i < NumNodeSys; i++)
                        if (Process[i] < Tik) Process[i] = Tik;
                    continue;

                }
                //Choose Proc
                while (true)
                {
                    buf = r.Next(NumNodeSys);
                    if (Process[buf] <= Tik)
                    { Pt = buf; break; }
                }

                //Check send (знаходимо вершини-попередники)
                CheckData(Wt);
                int Time;

                for (int i = 0; i < DataT.Length; i++)
                    for (int j = i; j < DataT.Length; j++)
                        if (Lines[DataT[i], Wt] > Lines[DataT[j], Wt])
                        {
                            buf = DataT[i]; DataT[i] = DataT[j]; DataT[j] = buf;
                        }
                int StartTransfer = 0;
                int TimeWithP = 0; double a = 0;
                for (int i = 0; i < DataT.Length; i++)
                    if (StartTransfer < W[1, DataT[i]]) StartTransfer = W[1, DataT[i]];
                //SENDING DATA!!!!
                for (int i = 0; i < DataT.Length; i++)
                    if (W[2, DataT[i]] != Pt)
                    {
                        GetPath(W[2, DataT[i]], Pt); //шлях між процесорами
                        //Sending...
                        Time = Lines[DataT[i], Wt];
                        string DT = W[2, DataT[i]].ToString() + " (" + DataT[i].ToString() + ") -> " + Pt.ToString() + " (" + Wt.ToString() + ")";

                        int STbuf = StartTransfer;
                        //Check free path
                        while (!CheckFreePath(Time, STbuf)) STbuf++; //чи шлях пересилки вільний

                        a = Math.Round(Time / Convert.ToDouble(LineSys[Path[0], Path[1]]), 3);
                        if (Math.Round(a) < a) TimeWithP = Convert.ToInt16(Math.Round(a)) + 1;
                        else TimeWithP = Convert.ToInt16(Math.Round(a));

                        for (int j = 0; j < Path.Length; j++)
                        {
                            for (int k = 0; k < TimeWithP; k++)
                                Send[STbuf + k, Path[j]] = DT;
                            STbuf += TimeWithP;
                            try
                            {
                                a = Math.Round(Time / Convert.ToDouble(LineSys[Path[j], Path[j + 1]]), 3);
                                if (Math.Round(a) < a) TimeWithP = Convert.ToInt16(Math.Round(a)) + 1;
                                else TimeWithP = Convert.ToInt16(Math.Round(a));
                            }
                            catch { }
                        }
                        if (Process[Pt] < STbuf) Process[Pt] = STbuf;
                    }

                //Processing...
                int Wmass = 0;
                a = Math.Round(Nodes[Wt].mass / Convert.ToDouble(NodeSys[Pt].mass), 3);
                if (Math.Round(a) < a) Wmass = Convert.ToInt16(Math.Round(a)) + 1;
                else Wmass = Convert.ToInt16(Math.Round(a));
                for (int i = 0; i < Wmass; i++)
                {
                    Proc[Process[Pt], Pt] = Wt.ToString();
                    Process[Pt]++;
                }
                W[1, Wt] = Process[Pt];
                W[2, Wt] = Pt;

                if ((Tik < Process.Min()) && (Process.Min() != 0)) Tik = Process.Min();
                Done++;
            }
            //Max Tic
            Tik = Process.Max();
            Analys();
        }

        //Перевірка, чи шлях пересилки вільний
        private Boolean CheckFreePath(int time, int Start)
        {
            int buf = Start;

            double aaa = Math.Round(time / Convert.ToDouble(LineSys[Path[0], Path[1]]), 3);
            int TimeBuf;
            if (Math.Round(aaa) < aaa) TimeBuf = Convert.ToInt16(Math.Round(aaa)) + 1;
            else TimeBuf = Convert.ToInt32(Math.Round(aaa));

            for (int i = 0; i < Path.Length; i++)
            {
                for (int j = 0; j < TimeBuf; j++)
                    if (Send[buf + j, Path[i]] != null) return false;
                buf += TimeBuf;
                try
                {
                    aaa = Math.Round(time / Convert.ToDouble(LineSys[Path[i], Path[i + 1]]), 3);
                    if (Math.Round(aaa) < aaa) TimeBuf = Convert.ToInt16(Math.Round(aaa)) + 1;
                    else TimeBuf = Convert.ToInt16(Math.Round(aaa));
                }
                catch { }
            }
            return true;
        }

        //Перевірка готовності вершини
        private Boolean CheckW(int Wt)
        {
            for (int i = 0; i < NumNode; i++)
                if (Lines[i, Wt] != 0)
                    if ((W[1, i] > Tik) || (W[1, i] == -1))
                        return false;
            return true;
        }

        //Знаходження всіх вершин-предків
        int[] DataT;
        private void CheckData(int Wt)
        {
            int buf = 0;
            for (int i = 0; i < NumNode; i++)
                if (Lines[i, Wt] != 0) buf++;
            DataT = new int[buf];
            buf = 0;
            for (int i = 0; i < NumNode; i++)
                if (Lines[i, Wt] != 0) { DataT[buf] = i; buf++; }
        }

        //Створення будь-якої таблиці
        private void CreateTable(DataGridView Grid)
        {
            Grid.Columns.Clear();
            DataGridViewTextBoxColumn[] Data = new DataGridViewTextBoxColumn[NumNodeSys];
            for (int i = 0; i < NumNodeSys; i++)
            {
                Data[i] = new DataGridViewTextBoxColumn();
                Data[i].FillWeight = 40F;
                Data[i].Name = i.ToString();
                Data[i].ReadOnly = true;
                Data[i].Width = 100;
            }
            Grid.Columns.AddRange(Data);
            Grid.RowCount = Tik;
            foreach (DataGridViewColumn C in Grid.Columns)
                C.DataGridView.Font = new Font("Comic Sans MS", 9.75F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(204)));
            for (int i = 0; i < Tik; i++) { Grid.Rows[i].HeaderCell.Value = i.ToString(); }
        }
        //Заповнення будь-якої таблиці
        private void FillTable(DataGridView Grid, string[,] Data)
        {
            for (int i = 0; i < Tik; i++)
                for (int j = 0; j < NumNodeSys; j++)
                {
                    Grid.Rows[i].Cells[j].Value = Data[i, j];
                    if (Data[i, j] != null) Grid.Rows[i].Cells[j].Style.BackColor = Color.LightGreen;
                }
        }

        ////////////////////////////////////////////////////////////////////////////////////
        ////////////              Призначення 4         ////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////
        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            GetDeep4();
            //  CreateTable(dataGridView1);
            // CreateTable(dataGridView2);
            //  FillTable(dataGridView1, Proc);
            //  FillTable(dataGridView2, Send);
        }

        private void GetDeep4()
        {
            Proc = new string[1000, NumNodeSys];
            Send = new string[1000, NumNodeSys];
            Random r = new Random();
            int Done = 0;
            W = new int[3, NumNode]; //Num W, Tik Done, Proc
            int[] Process = new int[NumNodeSys];


            for (int i = 0; i < NumNode; i++)
            {
                W[0, i] = i;
                W[1, i] = -1; //W not prepared
            }

            int Wt = 0, Pt = 0, buf;

            //Погружаємо перший ярус на процесори (пріорітет по звязності)
            int[] FirstLine = new int[NumNode];
            int FLC = 0;
            Boolean FL = true;
            for (int i = 0; i < NumNode; i++)
            {
                FL = true;
                for (int j = 0; j < NumNode; j++)
                    if (Lines[j, TaskList[i]] != 0) { FL = false; break; }
                if (FL)
                {
                    FirstLine[FLC] = TaskList[i];
                    FLC++;
                }
            }

            int[,] ListProc = new int[2, NumNodeSys];
            for (int i = 0; i < NumNodeSys; i++)
            {
                ListProc[0, i] = i;
                //count connection
                for (int j = 0; j < NumNodeSys; j++)
                    if (LineSys[i, j] != 0) ListProc[1, i]++;
            }

            for (int i = 0; i < NumNodeSys; i++)
                for (int j = i; j < NumNodeSys; j++)
                    if (ListProc[1, i] < ListProc[1, j])
                    {
                        buf = ListProc[0, i]; ListProc[0, i] = ListProc[0, j]; ListProc[0, j] = buf;
                        buf = ListProc[1, i]; ListProc[1, i] = ListProc[1, j]; ListProc[1, j] = buf;
                    }
            //Download first line
            buf = 0; double a = 0;
            for (int i = 0; i < FLC; i++)
            {
                int Wmass = 0;
                a = Math.Round(Nodes[FirstLine[i]].mass / Convert.ToDouble(NodeSys[ListProc[0, buf]].mass), 3);
                if (Math.Round(a) < a) Wmass = Convert.ToInt16(Math.Round(a)) + 1;
                else Wmass = Convert.ToInt16(Math.Round(a));
                for (int j = 0; j < Wmass; j++)
                {
                    Proc[Process[ListProc[0, buf]], ListProc[0, buf]] = FirstLine[i].ToString();
                    Process[ListProc[0, buf]]++;
                }
                W[1, FirstLine[i]] = Process[ListProc[0, buf]];
                W[2, FirstLine[i]] = ListProc[0, buf];

                if (buf + 1 < NumNodeSys) buf++;
                else buf = 0;
                Done++;
            }
            if ((Tik < Process.Min()) && (Process.Min() != 0)) Tik = Process.Min();

            while (Done != NumNode)
            {
                //Choose Task
                Wt = -1;
                for (int i = 0; i < NumNode; i++)
                    if (W[1, TaskList[i]] == -1)
                    {
                        buf = TaskList[i];
                        //Check preparation
                        if (CheckW(buf)) { Wt = buf; break; }
                    }
                if (Wt == -1)
                {
                    Tik++;
                    for (int i = 0; i < NumNodeSys; i++)
                        if (Process[i] < Tik) Process[i] = Tik;
                    continue;
                }

                //Check send (знаходимо вершини-попередники)
                CheckData(Wt);
                int Time;

                for (int i = 0; i < DataT.Length; i++)
                    for (int j = i; j < DataT.Length; j++)
                        if (Lines[DataT[i], Wt] > Lines[DataT[j], Wt])
                        {
                            buf = DataT[i]; DataT[i] = DataT[j]; DataT[j] = buf;
                        }

                //Choose Proc
                Pt = W[2, DataT[DataT.Length - 1]];

                int StartTransfer = 0;
                int TimeWithP = 0; a = 0;
                for (int i = 0; i < DataT.Length; i++)
                    if (StartTransfer < W[1, DataT[i]]) StartTransfer = W[1, DataT[i]];
                //SENDING DATA!!!!
                for (int i = 0; i < DataT.Length; i++)
                    if (W[2, DataT[i]] != Pt)
                    {
                        GetPath(W[2, DataT[i]], Pt); //шлях між процесорами
                        //Sending...
                        Time = Lines[DataT[i], Wt];
                        string DT = W[2, DataT[i]].ToString() + " (" + DataT[i].ToString() + ") -> " + Pt.ToString() + " (" + Wt.ToString() + ")";
                        int STbuf = StartTransfer;
                        //Check free path
                        while (!CheckFreePath(Time, STbuf)) STbuf++; //чи шлях пересилки вільний

                        a = Math.Round(Time / Convert.ToDouble(LineSys[Path[0], Path[1]]), 3);
                        if (Math.Round(a) < a) TimeWithP = Convert.ToInt16(Math.Round(a)) + 1;
                        else TimeWithP = Convert.ToInt16(Math.Round(a));

                        for (int j = 0; j < Path.Length; j++)
                        {
                            for (int k = 0; k < TimeWithP; k++)
                                Send[STbuf + k, Path[j]] = DT;
                            STbuf += TimeWithP;
                            try
                            {
                                a = Math.Round(Time / Convert.ToDouble(LineSys[Path[j], Path[j + 1]]), 3);
                                if (Math.Round(a) < a) TimeWithP = Convert.ToInt16(Math.Round(a)) + 1;
                                else TimeWithP = Convert.ToInt16(Math.Round(a));
                            }
                            catch { }
                        }
                        if (Process[Pt] < STbuf) Process[Pt] = STbuf;
                    }

                //Processing...
                int Wmass = 0;
                a = Math.Round(Nodes[Wt].mass / Convert.ToDouble(NodeSys[Pt].mass), 3);
                if (Math.Round(a) < a) Wmass = Convert.ToInt16(Math.Round(a)) + 1;
                else Wmass = Convert.ToInt16(Math.Round(a));

                for (int i = 0; i < Wmass; i++)
                {
                    Proc[Process[Pt], Pt] = Wt.ToString();
                    Process[Pt]++;
                }
                W[1, Wt] = Process[Pt];
                W[2, Wt] = Pt;

                if ((Tik < Process.Min()) && (Process.Min() != 0)) Tik = Process.Min();
                Done++;
            }
            //Max Tic
            Tik = Process.Max();
            Analys();
        }

        //Analys
        private void Analys()
        {
            double[] Analys = new double[4]; int buf = 0;
            Analys[0] = Tik - 1;

            for (int i = 0; i < NumNode; i++)
                buf += Nodes[i].mass;
            Analys[1] = Math.Round(buf / Analys[0], 3);
            Analys[2] = Math.Round(Analys[1] / NumNodeSys, 3);

            NkrG(); buf = 0;
            for (int i = 0; i < CNg; i++)
                buf += Nodes[Ng[i]].mass;
            Analys[3] = Math.Round(buf / Analys[0], 3);

            //  textBox2.Text = Analys[0].ToString();
            //  textBox3.Text = Analys[1].ToString();
            //  textBox4.Text = Analys[2].ToString();
            //  textBox5.Text = Analys[3].ToString();
        }

        private void newGraphTaskBtn_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < NumNode; i++)
                Nodes[i].Dispose();
            Lines = null; Lines = new int[10, 10];
            Nodes = new NodeT[10]; NumNode = 0;
            gTask1.LinesUpdate(Nodes, Lines, NumNode);
            gTask1.Invalidate();
            toolStripButton2.Checked = false;
            toolStripButton3.Checked = false;
            toolStripButton4.Checked = false;
            toolStripButton5.Checked = false;
        }

        private void checkGraphTaskBtn_Click(object sender, EventArgs e)
        {
            if (!CheckTask()) { MessageBox.Show("Граф задачи содержит циклы", "Ошибка"); Nodes[Fault].Focus(); gTask1.Refresh(); return; }
            else MessageBox.Show("Граф задачи не имеет циклов", "Результат");
        }

        private void newGraphSystBtn_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < NumNodeSys; i++)
                NodeSys[i].Dispose();
            LineSys = null; LineSys = new int[10, 10];
            NodeSys = new NodeS[10]; NumNodeSys = 0;
            gSystem1.LinesUpdate(NodeSys, LineSys, NumNodeSys);
            gSystem1.Invalidate();
            toolStripButton2.Checked = false;
            toolStripButton3.Checked = false;
            toolStripButton4.Checked = false;
            toolStripButton5.Checked = false;
        }

        private void checkGraphSysBtn_Click(object sender, EventArgs e)
        {
            Des = new int[NumNodeSys, NumNodeSys];
            //Алгоритм Флойда-Уолшера
            for (int i = 0; i < NumNodeSys; i++)
                for (int j = 0; j < NumNodeSys; j++)
                    if (i == j) Des[i, j] = 0;
                    else
                        if (LineSys[i, j] != 0) Des[i, j] = 1;
                    else Des[i, j] = 99999;

            for (int i = 0; i < NumNodeSys; i++)
                for (int j = 0; j < NumNodeSys; j++)
                    for (int l = 0; l < NumNodeSys; l++)
                        if (Des[j, l] > Des[j, i] + Des[i, l]) Des[j, l] = Des[j, i] + Des[i, l];

            for (int i = 0; i < NumNodeSys; i++)
                for (int j = 0; j < NumNodeSys; j++)
                    if (Des[i, j] == 99999)
                    {
                        MessageBox.Show("Элемент " + j.ToString() + " недостижим из вершины " + i.ToString(), "Ошибка"); NodeSys[j].Focus();
                        gSystem1.Refresh(); return;
                    }
            MessageBox.Show("Граф системы не содержит ошибок", "Результат");
        }

        private void selectElemBtn_Click(object sender, EventArgs e)
        {
            toolStripButton2.Checked = true;
            toolStripButton3.Checked = false;
            toolStripButton4.Checked = false;
            toolStripButton5.Checked = false;
            elemTextTxt.Text = "1";
        }

        private void addNodeBtn_Click(object sender, EventArgs e)
        {
            toolStripButton2.Checked = false;
            toolStripButton3.Checked = true;
            toolStripButton4.Checked = false;
            toolStripButton5.Checked = false;
            elemTextTxt.Text = "1";
        }

        private void removeNodeBtn_Click(object sender, EventArgs e)
        {

        }

        private void addLinkBtn_Click(object sender, EventArgs e)
        {
            toolStripButton2.Checked = false;
            toolStripButton3.Checked = false;
            toolStripButton4.Checked = true;
            toolStripButton5.Checked = false;
            elemTextTxt.Text = "1";
        }

        private void removeLinkBtn_Click(object sender, EventArgs e)
        {
            toolStripButton2.Checked = false;
            toolStripButton3.Checked = false;
            toolStripButton4.Checked = false;
            toolStripButton5.Checked = true;
            elemTextTxt.Text = "1";
        }

        private void elemTextTxt_TextChanged(object sender, EventArgs e)
        {
            //elemTextTxt_KeyDown(sender, e);
            /*if (First != -1 && Second != -1)
            {
                Lines[First, Second] = int.Parse(elemTextTxt.Text);
            }
            if (Active != -1)
            {
                int value = Int16.MinValue;
                try
                {
                    value = Int16.Parse(elemTextTxt.Text);
                }catch(Exception e1){
                }
                if (value == Int16.MinValue)
                {
                    return;
                }
                Nodes[Active].mass = value;
                Nodes[Active].Refresh();
            }*/
        }

        private void elemTextTxt_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void elemTextTxt_KeyDown_1(object sender, KeyEventArgs e)
        {
            elemTextTxt_KeyDown(sender, e);
        }

        private void buildQueuesBtn_Click(object sender, EventArgs e)
        {
            buildQueue();
        }

        private void buildQueue()
        {
            QueueLogic queueLogic = new QueueLogic(Nodes, Lines);
            String value = ((String)(queeBuildAlg.SelectedItem));
            if (value.Equals("1"))
            {
                
            }
            if (value.Equals("9"))
            {
                
            }
            if (value.Equals("13"))
            {
                queue13 = queueLogic.buildQueue13();
                this.queue15Txt.Text = queue13.ToString();
            }
            queue3 = queueLogic.buildQueue3();
            this.queue3Txt.Text = queue3.ToString();

            queue4 = queueLogic.buildQueue4();
            this.queue11Txt.Text = queue4.ToString();
        }

        private void generateGraphBtn_Click(object sender, EventArgs e)
        {
            /*
            {
                int vertexCount = int.Parse(genNodeCountTxt.Text);
                GraphRandomGeneratorLogic logic = new GraphRandomGeneratorLogic(int.Parse(genMinWeightTxt.Text), int.Parse(genMaxWeightTxt.Text), vertexCount, double.Parse(genCorrelationTxt.Text), double.Parse(genVarianceTxt.Text));
                Graph graph = logic.generateRandomGraph();
                newGraphTaskBtn_Click(null, null);
                int centerX = 290;
                int centerY = 210;
                int r = 180;
                double angle = 2 * 3.14 / vertexCount;
                double currPos = 0;
                for (int i = 0; i < graph.Nodes.Length; i++)
                {
                    int x = (int)(r * Math.Cos(currPos)) + centerX;
                    int y = (int)(r * Math.Sin(currPos)) + centerY;
                    currPos += angle;
                    AddNode(x, y, graph.Nodes[i]);
                }

                Lines = new int[vertexCount, vertexCount];
                for (int i = 0; i < vertexCount; i++)
                {
                    for (int j = 0; j < vertexCount; j++)
                    {
                        Lines[i, j] = graph.IncidenceMatrix[i][j];
                    }
                }
                gTask1.LinesUpdate(Nodes, Lines, vertexCount);
                gTask1.Update();
                gTask1.Invalidate();
                if (!CheckTask())
                {
                }
            }*/

            генеруватиГЗToolStripMenuItem_Click(null, null);
        }

        ////////////////////////////////////////////////////////////////////////////////////
        ////////////              Генератор             ////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////
        int MinScale, MaxScale;
        double Koef;

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void queeBuildAlg_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void генеруватиГЗToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Считуемо введені данні
            //Form3 F3 = new Form3();
            //F3.ShowDialog();
            //if (DialogResult.OK == F3.DialogResult)
            {
                NumNode = Convert.ToInt16(genNodeCountTxt.Text);
                MinScale = Convert.ToInt16(genMinWeightTxt.Text);
                MaxScale = Convert.ToInt16(genMaxWeightTxt.Text);
                Koef = Convert.ToDouble(genCorrelationTxt.Text);
            }
            /*else return;
            if (NumNode > 10) { Nodes = new NodeT[NumNode]; Lines = new int[NumNode, NumNode]; }
            */
            //Генерируемо вершини та їх вагу
            Point center = new Point(gTask1.Width / 2 - 80, gTask1.Height / 2 - 20);
            int radius = Math.Min(gTask1.Width / 2, gTask1.Height / 2) - 20;
            int j = 0;
            Random r = new Random();
            int buf = NumNode; NumNode = 0;
            while (j < buf)
            {
                AddNode((int)Math.Round(center.X - radius * Math.Cos(2 * Math.PI * j / buf) + 60),
                        (int)Math.Round(center.Y - radius * Math.Sin(2 * Math.PI * j / buf)),
                        r.Next(MinScale, MaxScale));
                j++;
            }

            //Генерація зв'язків
            int LineCount = r.Next(NumNode, 2 * NumNode - 1); //кількість зв'язків
            buf = 0; int beg, end;
            int[,] LP = new int[LineCount, 2];
            while (buf < LineCount)
            {
                beg = r.Next(0, NumNode); end = r.Next(0, NumNode);
                if ((beg != end) && (Lines[beg, end] == 0) && (Lines[end, beg] == 0))
                {
                    Lines[beg, end] = 1;
                    if (!CheckTask()) { Lines[beg, end] = 0; }
                    else { LP[buf, 0] = beg; LP[buf, 1] = end; buf++; }
                }
            }

            //Відповідність коефіцієнту связності
            buf = 0;
            for (int i = 0; i < NumNode; i++) buf += Nodes[i].mass;
            int WC = Convert.ToInt16(Math.Round(buf * (1 - Koef) / Koef));

            buf = LineCount;
            int L, Inc;
            if (buf > WC)
            {
                while (buf > WC)
                {
                    L = r.Next(LineCount);
                    Lines[LP[L, 0], LP[L, 1]] = 0;
                    if (!CheckTask()) { Lines[LP[L, 0], LP[L, 1]] = 1; }
                    else buf--;
                }
            }
            else
            {
                while (buf < WC)
                {
                    L = r.Next(LineCount);
                    Inc = r.Next(1, 3);
                    if (buf + Inc <= WC)
                    {
                        Lines[LP[L, 0], LP[L, 1]] += Inc;
                    }
                    buf += Inc;
                }
            }
            gTask1.LinesUpdate(Nodes, Lines, NumNode);
            gTask1.Update(); gTask1.Invalidate();
        }

        private void planBtn_Click(object sender, EventArgs e)
        {
            buildQueue();
            Queue queue = getSelectedQueue();
            int algorithm = Int32.Parse((String)(assignAlgCmb.SelectedItem));
            PlaningLogic planningLogic = new PlaningLogic(NodeSys, LineSys, queue, Nodes, Lines, algorithm);
            String[][] planResult = planningLogic.buildPlan();
            Form4 graphForm = new Form4(planResult);
            graphForm.Show();

        }

        // lab 8
        private void calculateStatistic()
        {
            buildQueue();
            Queue queue = getSelectedQueue();
            double[] avStat = new double[] { 0, 0, 0 };
            int algorithm = Int32.Parse((String)(assignAlgCmb.SelectedItem));
            for (int i = 0; i < 10; i++)
            {
                PlaningLogic planningLogic = new PlaningLogic(NodeSys, LineSys, queue, Nodes, Lines, algorithm);
                String[][] planResult = planningLogic.buildPlan();
                double[] statistic = planningLogic.makeStatistic(planResult.Length);
                avStat[0] += statistic[0];
                avStat[1] += statistic[1];
                avStat[2] += statistic[2];
            }
            avStat[0] /= 10;
            avStat[1] /= 10;
            avStat[2] /= 10;
            this.kuTxt.Text = format(avStat[0]);
            this.keTxt.Text = format(avStat[1]);
            this.keaTxt.Text = format(avStat[2]);
        }


        private Queue getSelectedQueue()
        {
            String value = ((String)(queeBuildAlg.SelectedItem));
            if (value.Equals("1"))
            {
                return queue3;
            }
            if (value.Equals("9"))
            {
                return queue4;
            }
            if (value.Equals("13"))
            {
                return queue13;
            }
            return null;
        }

        private String format(double value)
        {
            String s = "" + value;
            if (s.Length > 7)
            {
                s = s.Substring(0, 7);
            }
            return s;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            calculateStatistic();
        }
    }
}
