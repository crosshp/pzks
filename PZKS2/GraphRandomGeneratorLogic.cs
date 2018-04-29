using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PZKS2
{
    public class GraphRandomGeneratorLogic
    {
        private int minVertexWeight;
        private int maxVertexWeight;
        private int vertexCount;
        private double correlation;
        private double variance;

        public GraphRandomGeneratorLogic(int minVertexWeight, int maxVertexWeight, int vertexCount, double correlation, double variance)
        {
            this.minVertexWeight = minVertexWeight;
            this.maxVertexWeight = maxVertexWeight;
            this.vertexCount = vertexCount;
            this.correlation = correlation;
            this.variance = variance;
        }

        public Graph generateRandomGraph()
        {
            int[] vertexes = new int[vertexCount];
            Random random=new Random();
            int dW=maxVertexWeight-minVertexWeight;
            int vertexSum = 0;
            for (int i = 0; i < vertexCount; i++)
            {
                vertexes[i] = minVertexWeight + random.Next(dW);
                vertexSum += vertexes[i];
            }
            double nodeSum = ((double)vertexSum) * (1 - correlation) / correlation;
            int[][] incidence = new int[vertexCount][];
            for (int i = 0; i < incidence.Length; i++)
            {
                incidence[i]=new int[vertexCount];
            }

            int minNodeCount = 1 * vertexCount;
            double averageNodeWeight = nodeSum / minNodeCount;
            double minBorder = averageNodeWeight * (1 - variance);
            double dB = averageNodeWeight * 2 * variance;
            double realNodeSum = 0;
            for (int i = 0; i < minNodeCount; i++)
            {
                int sourceIndex = i % vertexCount;
                int destIndex = random.Next(vertexCount);
                int nodeWeight = (int)(minBorder + random.NextDouble() * dB);
                incidence[sourceIndex][destIndex] = nodeWeight;
                realNodeSum += nodeWeight;
                if (realNodeSum > nodeSum)
                {
                    break;
                }
            }
            bool isExistOutNodeForAllVertexes = true;
            for (int i = 0; i < incidence.Length; i++)
            {
                bool isExistOutNode = false;
                for (int j = 0; j < incidence.Length; j++)
                {
                    if (incidence[i][j] != 0)
                    {
                        isExistOutNode = true;
                        break;
                    }
                }
                if (!isExistOutNode)
                {
                    isExistOutNodeForAllVertexes = false;
                }
            }
            if (isExistOutNodeForAllVertexes)
            {
                return new Graph(vertexes, incidence);
            }
            else
            {
                return generateRandomGraph();
            }
        }
    }
}
