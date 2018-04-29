using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PZKS2
{
    public class Queue
    {
        private int type;
        private IList<int> queue;
        private IList<int> weights;

        public Queue(int type)
        {
            this.type = type;
            queue = new List<int>();
            weights=new List<int>();
        }

        public IList<int> getQueue()
        {
            return queue;
        }

        public void addVertexToQueue(int vertex)
        {
            queue.Add(vertex);
        }

        public void addVertexToQueue(int vertex, int weight)
        {
            queue.Add(vertex);
            weights.Add(weight);
        }

        public String ToString()
        {
            StringBuilder buffer = new StringBuilder();
            for (int i = 0; i < queue.Count; i++)
            {
                buffer.Append(queue.ElementAt(i));
                if (weights != null && weights.Count!=0)
                {
                    buffer.Append("(" + weights.ElementAt(i) + ")");
                }
                if (i != queue.Count - 1)
                {
                    buffer.Append(", ");
                }
            }
            return buffer.ToString();
        }
    }
}
