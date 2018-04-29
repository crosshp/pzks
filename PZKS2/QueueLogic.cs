using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace PZKS2
{
    public class QueueLogic
    {
        private int[][] vertexes; //0-number 1-mass
        private int[][] incidenceMatrix;
        private int[] depthTree; // кількость вершин до кінця графа 
        private int[] maxTimeFromStart; // критическое время от начала графа 

        static int[][] invertMatrix(int[][] matrix)
        {
            int[][] invert=new int[matrix.Length][];
       
            for (int i = 0; i < matrix.Length; i++) {
                invert[i] = new int[matrix.Length];
                for (int j = 0; j < matrix.Length; j++) { 
                    invert[i][j]=matrix[i][j];
                }
            }
            return invert;
        }

        public QueueLogic(NodeT[] nodes, int[,] lines)
        {
            int vertexCount = 0;
            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i] != null)
                {
                    vertexCount++;
                }
            }

            vertexes = new int[vertexCount][];
            incidenceMatrix = new int[vertexCount][];
            depthTree=new int[vertexCount];
            maxTimeFromStart = new int[vertexCount];
            
            int p = 0;
            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i] != null)
                {
                    vertexes[p] = new int[] { nodes[i].number, nodes[i].mass };
                    p++;
                }
            }
            for (int i = 0; i < incidenceMatrix.Length; i++)
            {
                incidenceMatrix[i] = new int[incidenceMatrix.Length];
            }
            for (int i = 0; i < Math.Sqrt(lines.Length); i++)
            {
                for (int j = 0; j < Math.Sqrt(lines.Length); j++)
                {
                    if (lines[i, j] != 0)
                    {
                        incidenceMatrix[getVertexIndexInArrayByNumber(i)][getVertexIndexInArrayByNumber(j)] = lines[i, j];
                    }
                }
            }
        }

        private int getVertexIndexInArrayByNumber(int number)
        {
            for (int i = 0; i < vertexes.Length; i++)
            {
                if (vertexes[i][0] == number)
                {
                    return i;
                }
            }
            return Int32.MinValue;
        }


        public Queue buildQueue1()
        {
            Queue queue = new Queue(4);
            double[][] wayLength = new double[vertexes.Length][];
            for (int i = 0; i < wayLength.Length; i++)
            {
                // Первый - елемент - номер вершины,
                // Второй - критическое растояние от начала графа
                // Третий - критичний час
                // Четвертий - пронормоване значення з завдання вар1
                wayLength[i] = new double[] { i, Int32.MaxValue, 0, 0 };
            }

            // Заполняем значения для конечных вершин
            for (int i = 0; i < incidenceMatrix.Length; i++)
            {
                bool isAllZero = true;
                for (int j = 0; j < incidenceMatrix.Length; j++)
                {
                    if (incidenceMatrix[i][j] != 0)
                    {
                        isAllZero = false;
                        break;
                    }
                }
                if (isAllZero)
                {
                    wayLength[i][2] = vertexes[i][1];
                }
            }
            bool isChanged = true;
            while (isChanged)
            {
                isChanged = false;
                for (int i = 0; i < incidenceMatrix.Length; i++)
                {
                    for (int j = 0; j < incidenceMatrix.Length; j++)
                    {
                        if (incidenceMatrix[i][j] != 0 && wayLength[i][2] < wayLength[j][2] + vertexes[i][1])
                        {
                            wayLength[i][2] = wayLength[j][2] + vertexes[i][1];
                            isChanged = true;
                        }
                    }
                }
            }

            // Для каждой вершины находим максимальный по длине путь к концу графа.
            // Конец графа - вершина неимеющая исходящих ребер.
            for (int i = 0; i < wayLength.Length; i++)
            {
                wayLength[i][1] = getMaxDepthTree(i);
            }

            double maxLength = findMax(wayLength, 1);
            double maxTime = findMax(wayLength, 2);

            for (int i = 0; i < wayLength.Length; i++)
            {
                wayLength[i][3] = wayLength[i][1] / maxLength + wayLength[i][2] / maxTime;
            }

  /*          Array.Sort(wayLength, new Var1Comparer());
            for (int i = 0; i < wayLength.Length; i++)
            {
                queue.addVertexToQueue(wayLength[i][0], wayLength[i][3]);
            }*/
            return queue;
        }

        //1-lenght
        //2-time
        private double findMax(double[][] array, int index)
        {
            double max = Double.MinValue;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i][index] > max)
                {
                    max = array[i][index];
                }
            }
            return max;
        }

        public Queue buildQueue15()
        {
            Queue queue = new Queue(15);
            int[][] _vertex = new int[vertexes.Length][];
            for (int i = 0; i < vertexes.Length; i++)
            {
                _vertex[i] = new int[] { vertexes[i][0], vertexes[i][1] };
            }
            Array.Sort(_vertex, new VertexComparer(true));
            for (int i = 0; i < _vertex.Length; i++)
            {
                queue.addVertexToQueue(_vertex[i][0]);
            }
            return queue;
        }



        public Queue buildQueue13()
        {
            Queue queue = new Queue(15);
            int[][] _vertex = new int[vertexes.Length][];
            for (int i = 0; i < vertexes.Length; i++)
            {
                _vertex[i] = new int[] { vertexes[i][0], vertexes[i][1] };
            }
            _vertex = Shuffle<int>(_vertex);
            for (int i = 0; i < _vertex.Length; i++)
            {
                queue.addVertexToQueue(_vertex[i][0]);
            }
            return queue;
        }

        public T[][] Shuffle<T>(T[][] array)
        {
            Random _random = new Random();
            var random = _random;
            for (int i = array.Length; i > 1; i--)
            {
                // Pick random element to swap.
                int j = random.Next(i); // 0 <= j <= i-1
                                        // Swap.
                T tmp = array[j][0];
                array[j][0] = array[i - 1][0];
                array[i - 1][0] = tmp;
            }
            return array;
        }

        public Queue buildQueue3()
        {
            Queue queue = new Queue(3);
            int[][] wayLength = new int[vertexes.Length][];
            for (int i = 0; i < wayLength.Length; i++)
            {
                // Первый елемент - номер вершины
                // Второй - критичнон время
                wayLength[i] = new int[] { i, 0 };
            }

            // Заполняем значения для конечных вершин
            for (int i = 0; i < incidenceMatrix.Length; i++)
            {
                bool isAllZero = true;
                for (int j = 0; j < incidenceMatrix.Length; j++)
                {
                    if (incidenceMatrix[i][j] != 0)
                    {
                        isAllZero = false;
                        break;
                    }
                }
                if (isAllZero)
                {
                    wayLength[i][1] = vertexes[i][1];
                }
            }
            bool isChanged = true;
            while (isChanged)
            {
                isChanged = false;
                for (int i = 0; i < incidenceMatrix.Length; i++)
                {
                    for (int j = 0; j < incidenceMatrix.Length; j++)
                    {
                        if (incidenceMatrix[i][j] != 0 && wayLength[i][1] < wayLength[j][1] + vertexes[i][1])
                        {
                            wayLength[i][1] = wayLength[j][1] + vertexes[i][1];
                            isChanged = true;
                        }
                    }
                }
            }
            Array.Sort(wayLength, new VertexComparer(false));
            for (int i = 0; i < wayLength.Length; i++)
            {
                queue.addVertexToQueue(wayLength[i][0], wayLength[i][1]);
            }
            return queue;
        }

        

         public Queue buildQueue4()
        {
            Queue queue = new Queue(4);
            int[][] wayLength = new int[vertexes.Length][];
            for (int i = 0; i < wayLength.Length; i++)
            {
                // Первый елемент - номер вершины,
                // Второй - критическое растояние от начала графа
                // Третий - связность вершины
                wayLength[i] = new int[] { i, Int32.MaxValue, 0 };
            }

            // Подсчитываем связность
            for (int i = 0; i < incidenceMatrix.Length; i++)
            {
                for (int j = 0; j < incidenceMatrix.Length; j++)
                {
                    if (incidenceMatrix[j][i] != 0)
                    {
                        wayLength[i][2]++;
                    }
                    if (incidenceMatrix[i][j] != 0 && i != j)
                    {
                        
                        wayLength[i][2]++;
                    }
                    
                }
             
            }
             // Для каждой вершины находим максимальный по длине путь к концу графа.
             // Конец графа - вершина неимеющая исходящих ребер.
            for (int i = 0; i < wayLength.Length; i++)
            {
                wayLength[i][1] = getMaxDepthTree(i);
            }

          
            Array.Sort(wayLength, new Var4Comparer());
            for (int i = 0; i < wayLength.Length; i++)
            {
                queue.addVertexToQueue(wayLength[i][0]);
            }
            return queue;
        }

         int getMaxDepthTree(int vertex) {
             if (depthTree[vertex] != 0) {
                 return depthTree[vertex];
             }

             if (isEndVertex(vertex)) {
                 depthTree[vertex] = 1;
                 return depthTree[vertex];
             }

             int maxDepth = 0;
             for (int i = 0; i < incidenceMatrix.Length; i++) {
                 if ((i == vertex) || (incidenceMatrix[vertex][i] == 0)) {
                     continue;
                 }
                 int depthI = getMaxDepthTree(i);
                 if (depthI > maxDepth) {
                     maxDepth = depthI;
                 }
             }
             depthTree[vertex] = maxDepth + 1;// уитываем  в высоте саму вершину

             return depthTree[vertex];
         }

         Boolean isEndVertex(int v) {
             for (int i = 0; i < incidenceMatrix.Length; i++)
             {
                 if (incidenceMatrix[v][i] != 0) {
                     return false;
                 }
             }
             return true;
         }

         Boolean isStartVertex(int v)
         {
             for (int i = 0; i < incidenceMatrix.Length; i++)
             {
                 if (incidenceMatrix[i][v] != 0)
                 {
                     return false;
                 }
             }
             return true;
         }

         class Var4Comparer : IComparer
         {
             int IComparer.Compare(Object x, Object y)
             {
                 int[] o1 = (int[])x;
                 int[] o2 = (int[])y;
                 int res = o2[1] - o1[1];
                 if (res != 0)
                 {
                     return res;
                 }
                 else
                 {
                     return o2[2] - o1[2];
                 }
             }
         }


         int getMaxtimeFromStart(int v) {
             if (maxTimeFromStart[v] != 0) {
                 return maxTimeFromStart[v];
             }
             
             if (isStartVertex(v)) {
                 maxTimeFromStart[v] = vertexes[v][1];
                 return maxTimeFromStart[v];
             }

             int maxTime = 0;
             for (int i = 0; i < incidenceMatrix.Length; i++)
             {
                 // Для всех вершин из которых можно попасть в текущую находим время от начала
                 if ((i == v) || (incidenceMatrix[i][v] == 0))
                 {
                     continue;
                 }
                 int timeI = getMaxtimeFromStart(i);
                 if (timeI > maxTime)
                 {
                     maxTime = timeI;
                 }
             }
             maxTimeFromStart[v] =maxTime+ vertexes[v][1]; // учитываем время текщей вершины
             return maxTimeFromStart[v];
         }

         public Queue buildQueue16()
         {
             Queue queue = new Queue(16);
             int[][] wayLength = new int[vertexes.Length][];
             for (int i = 0; i < wayLength.Length; i++)
             {
                 // Первый елемент - номер вершины
                 // Второй - критичнон время
                 wayLength[i] = new int[] { i, 0 };
             }

             // Заполняем значения для начальных
             for (int i = 0; i < incidenceMatrix.Length; i++)
             {
                     wayLength[i][1] = getMaxtimeFromStart(i);
             }

             Array.Sort(wayLength, new VertexComparer(true));
             for (int i = 0; i < wayLength.Length; i++)
             {
                 queue.addVertexToQueue(wayLength[i][0], wayLength[i][1]);
             }
             return queue;
         }

        public Queue buildQueue11()
        {
            Queue queue = new Queue(11);
            int[][] wayLength = new int[vertexes.Length][];
            for (int i = 0; i < wayLength.Length; i++)
            {
                // Первый елемент - номер вершины,
                // Второй - критическое растояние от начала графа
                // Третий - связность вершины
                wayLength[i] = new int[] { i, Int32.MaxValue, 0 };
            }

            // Подсчитываем связность и находим начальные вершины
            for (int i = 0; i < incidenceMatrix.Length; i++)
            {
                bool isAllZero = true;
                for (int j = 0; j < incidenceMatrix.Length; j++)
                {
                    if (incidenceMatrix[j][i] != 0)
                    {
                        isAllZero = false;
                        wayLength[i][2]++;
                    }
                    if (incidenceMatrix[i][j] != 0 && i != j)
                    {
                        wayLength[i][2]++;
                    }
                }
                if (isAllZero)
                {
                    wayLength[i][1] = 1;
                }
            }
            bool isChanged = true;
            while (isChanged)
            {
                isChanged = false;
                for (int i = 0; i < incidenceMatrix.Length; i++)
                {
                    for (int j = 0; j < incidenceMatrix.Length; j++)
                    {
                        if (incidenceMatrix[i][j] != 0)
                            if (wayLength[j][1] != Int32.MaxValue)
                            {
                                if (wayLength[j][1] > wayLength[i][1] + 1)
                                {
                                    wayLength[j][1] = wayLength[i][1] + 1;
                                    isChanged = true;
                                }
                            }
                            else {
                                wayLength[j][1] = wayLength[i][1] + 1;
                            }
                    }
                }
            }

            Array.Sort(wayLength, new Var11Comparer());
            for (int i = 0; i < wayLength.Length; i++)
            {
                queue.addVertexToQueue(wayLength[i][0]);
            }
            return queue;
        }
    }

    class VertexComparer : IComparer
    {
        private bool order;
        public VertexComparer(bool order)
        {
            this.order = order;
        }

        int IComparer.Compare(Object x, Object y)
        {
            int[] o1 = (int[])x;
            int[] o2 = (int[])y;
            int res = o1[1] - o2[1];
            if (order)
            {
                return res;
            }
            else
            {
                return -res;
            }
        }
    }

    class Var11Comparer : IComparer
    {
        int IComparer.Compare(Object x, Object y)
        {
            int[] o1 = (int[])x;
            int[] o2 = (int[])y;
            int res = o2[2] - o1[2];
            if (res != 0)
            {
                return res;
            }
            else
            {
                return o1[1] - o2[1];
            }
        }
    }


}
