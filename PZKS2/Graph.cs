using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PZKS2
{
    public class Graph
    {
        private int[][] incidenceMatrix;

        public int[][] IncidenceMatrix
        {
            get { return incidenceMatrix; }
            set { incidenceMatrix = value; }
        }
        private int[] nodes;

        public int[] Nodes
        {
            get { return nodes; }
            set { nodes = value; }
        }

        public Graph(int[] nodes, int[][] incidenceMatrix)
        {
            this.incidenceMatrix = incidenceMatrix;
            this.nodes = nodes;
        }
    }
}
