using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PZKS2
{
    public partial class Form4 : Form
    {
        private String[][] data;
        public Form4(String[][] data)
        {
            InitializeComponent();
            this.data = data;
            graph.Columns.Clear();
            DataGridViewTextBoxColumn[] Data = new DataGridViewTextBoxColumn[data[0].Length];
            for (int i = 0; i < data[0].Length; i++)
            {
                Data[i] = new DataGridViewTextBoxColumn();
                Data[i].FillWeight = 40F;
                Data[i].Name = i.ToString();
                Data[i].ReadOnly = true;
                Data[i].Width = 150;
            }
            graph.Columns.AddRange(Data);
            graph.RowCount = data.Length;
            foreach (DataGridViewColumn C in graph.Columns)
                C.DataGridView.Font = new Font("Arial", 9.75F, FontStyle.Italic, GraphicsUnit.Pixel, ((byte)(204)));
            for (int i = 0; i < data.Length; i++) { 
                graph.Rows[i].HeaderCell.Value = i.ToString(); 
            }
            for (int i = 0; i < data.Length; i++)
            {
                for (int j = 0; j < data[0].Length; j++)
                {
                    graph.Rows[i].Cells[j].Value = data[i][j]!=null?data[i][j]:"";
                    //if (Data[i, j] != null) Grid.Rows[i].Cells[j].Style.BackColor = Color.LightGreen;
                }
            }
        }
    }
}
