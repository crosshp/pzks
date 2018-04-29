using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace PZKS2
{
    public class PlaningLogic
    {
        private int[][] procesors; //0-number 1-mass
        private int[][] incidenceProc;
        private int[][] tasks;
        private int[][] incidenceTask;
        private Queue quee;
        private int algNumber;
        private Random random = new Random();

        public Dictionary<Int32,Task> taskMap;
        public Dictionary<Int32, Processor> procMap;
        public List<Transmission> transmMap=new List<Transmission>();

        public PlaningLogic(NodeS[] nodes, int[,] lines, Queue quee, NodeT[] tasksIn, int[,] taskIncIn, int algNumber)
        {
            this.quee = quee;
            this.algNumber = algNumber;
            int vertexCount = 0;
            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i] != null)
                {
                    vertexCount++;
                }
            }

            procesors = new int[vertexCount][];
            incidenceProc = new int[vertexCount][];
            int p = 0;
            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i] != null)
                {
                    procesors[p] = new int[] { nodes[i].number, nodes[i].mass };
                    p++;
                }
            }
            for (int i = 0; i < incidenceProc.Length; i++)
            {
                incidenceProc[i] = new int[incidenceProc.Length];
            }
            for (int i = 0; i < Math.Sqrt(lines.Length); i++)
            {
                for (int j = 0; j < Math.Sqrt(lines.Length); j++)
                {
                    if (lines[i, j] != 0)
                    {
                        incidenceProc[i][j] = lines[i, j];
                    }
                }
            }

            vertexCount = 0;
            for (int i = 0; i < tasksIn.Length; i++)
            {
                if (tasksIn[i] != null)
                {
                    vertexCount++;
                }
            }

            tasks = new int[vertexCount][];
            incidenceTask = new int[vertexCount][];
            p = 0;
            for (int i = 0; i < tasksIn.Length; i++)
            {
                if (tasksIn[i] != null)
                {
                    tasks[p] = new int[] { tasksIn[i].number, tasksIn[i].mass };
                    p++;
                }
            }
            for (int i = 0; i < incidenceTask.Length; i++)
            {
                incidenceTask[i] = new int[incidenceTask.Length];
            }
            for (int i = 0; i < Math.Sqrt(taskIncIn.Length); i++)
            {
                for (int j = 0; j < Math.Sqrt(taskIncIn.Length); j++)
                {
                    if (taskIncIn[i, j] != 0)
                    {
                        incidenceTask[getVertexIndexInArrayByNumber(i, tasks)][getVertexIndexInArrayByNumber(j, tasks)] = taskIncIn[i, j];
                    }
                }
            }
        }

        private int getVertexIndexInArrayByNumber(int number, int[][] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i][0] == number)
                {
                    return i;
                }
            }
            return Int32.MinValue;
        }

        private void buildTasks()
        {
            taskMap = new Dictionary<Int32,Task>();
            for (int i = 0; i < tasks.Length; i++)
            {
                Task task = new Task(tasks[i][0], tasks[i][1]);
                taskMap.Add(task.number, task);
            }
            for (int i = 0; i < incidenceTask.Length; i++)
            {
                for (int j = 0; j < incidenceTask.Length; j++)
                {
                    if (incidenceTask[i][j] != 0)
                    {
                        Task task = taskMap[j];
                        Transmission transm = new Transmission(i, j);
                        transmMap.Add(transm);
                        task.requiredTransmission.Add(transm);
                    }
                }
            }
        }

        private void buildProc()
        {
            procMap = new Dictionary<int, Processor>();
            for (int i = 0; i < procesors.Length; i++)
            {
                procMap.Add(procesors[i][0], new Processor(procesors[i][0]));
            }
            for (int i = 0; i < incidenceProc.Length; i++)
            {
                for (int j = 0; j < incidenceProc.Length; j++)
                {
                    if (incidenceProc[i][j] != 0)
                    {
                        procMap[i].relationCount++;
                    }
                }
            }
        }

        private bool isAllTaskFinished()
        {
            for (int i = 0; i < taskMap.Keys.Count; i++)
            {
                if (taskMap[taskMap.Keys.ElementAt(i)].state != TaskState.FINISHED)
                {
                    return false;
                }
            }
            return true;
        }

        private List<Processor> getAllFreeProcOnStep(int stepNumber, List<DiagramRow> rows)
        {
            List<Processor> freeProcessors = new List<Processor>();
            DiagramRow row = rows.ElementAt(stepNumber);
            for (int i = 0; i < row.states.Length; i++)
            {
                if (row.states[i].task == null)
                {
                    freeProcessors.Add(procMap[i]);
                }
            }
            return freeProcessors;
        }

        // step, proc
        private int[] plannAlg2Task(Task task, int stepNumber, List<DiagramRow> rows)
        {
            int step = stepNumber;
            List<Processor> freeProc = null;
            while (true)
            {
                freeProc = getAllFreeProcOnStep(step, rows);
                if (freeProc.Count != 0)
                {
                    break;
                }
                step++;
            }
         
           List<Processor> orderedProc= freeProc
                .OrderBy(processor => processor.freeStep)
                .ToList();
            Processor proc=orderedProc.First();
            return new int[] { step, proc.number };
        }

        private int[] plannAlg5Task(Task task, int stepNumber, List<DiagramRow> rows)
        {
            int currentStep=stepNumber;
            List<Processor> freeProc = null;
            while(true){
                freeProc = getAllFreeProcOnStep(currentStep, rows);
                if(freeProc.Count>0){
                    break;
                }
                currentStep++;
            }
            List<Tuple<Processor, Int32>> relatedProc = new List<Tuple<Processor, Int32>>();
            for (int i = 0; i < task.requiredTransmission.Count; i++)
            {
                Transmission transm=task.requiredTransmission.ElementAt(i);
                relatedProc.Add(new Tuple<Processor, Int32>(procMap[taskMap[transm.taskFromNumber].procNumber], incidenceTask[transm.taskFromNumber][transm.taskToNumber]));
            }
            List<Processor> minProc = new List<Processor>();
            int minCharValue = Int32.MaxValue;
            for (int i = 0; i < freeProc.Count; i++)
            {
                Processor currentProc = freeProc.ElementAt(i);
                int charValue=0;
                for (int j = 0; j < relatedProc.Count; j++)
                {
                    charValue+=getShortestWayForTransm(currentProc.number, relatedProc.ElementAt(j).Item1.number, relatedProc.ElementAt(j).Item2).Count;
                }
                if (charValue == minCharValue)
                {
                    if (minProc == null)
                    {
                        minProc = new List<Processor>();
                    }
                    minProc.Add(currentProc);
                }
                else if (charValue < minCharValue)
                {
                    minProc = new List<Processor>();
                    minProc.Add(currentProc);
                    minCharValue = charValue;
                }
            }
            if (minProc == null || minProc.Count == 0)
            {
                return null;
            }
            if (minProc.Count == 1)
            {
                return new int[]{currentStep, minProc.ElementAt(0).number};
            }
            int maxRelCount = minProc.ElementAt(0).relationCount;
            List<Processor> maxProcList = new List<Processor>();
            maxProcList.Add(minProc.ElementAt(0));
            for (int i = 1; i < minProc.Count; i++)
            {
                if (minProc.ElementAt(i).relationCount == maxRelCount)
                {
                    maxProcList.Add(minProc.ElementAt(i));
                }
                if (minProc.ElementAt(i).relationCount > maxRelCount)
                {
                    maxProcList = new List<Processor>();
                    maxProcList.Add(minProc.ElementAt(i));
                    maxRelCount = minProc.ElementAt(i).relationCount;
                }
            }
            Random random = new Random();
            Processor proc = maxProcList.ElementAt(random.Next(maxProcList.Count));
            return new int[]{currentStep, proc.number};
        }

        // step number, proc number
        public int[] plann(Task task, int stepNumber, List<DiagramRow> rows)
        {
            int[] result = null;
            if (algNumber == 2)
            {
                result=plannAlg2Task(task, stepNumber, rows);
            }
            else
            {
                result = plannAlg5Task(task, stepNumber, rows);
            }
            return result;
        }

        private int getStepForDeployTask(Task task, List<DiagramRow> rows)
        {
            if (task.requiredTransmission.Count == 0)
            {
                return 0;
            }
            int maxStep = 0;
            for (int i = 0; i < task.requiredTransmission.Count; i++)
            {
                Transmission transmission = task.requiredTransmission.ElementAt(i);
                Task requiredTask = taskMap[transmission.taskFromNumber];
                if (requiredTask.state!=TaskState.FINISHED)
                {
                    return Int32.MinValue;
                }
                if (requiredTask.finishStepNumber > maxStep)
                {
                    maxStep = requiredTask.finishStepNumber;
                }
            }
            return maxStep+1;
        }

        private List<DiagramRow> initRows()
        {
            List<DiagramRow> rows = new List<DiagramRow>();
            for (int i = 0; i < 10000; i++)
            {
                DiagramRow row = new DiagramRow();
                row.number = i;
                row.states = new DiagramState[procMap.Count];
                for (int j = 0; j < row.states.Length; j++)
                {
                    row.states[j] = new DiagramState();
                }
                rows.Add(row);
            }
            return rows;
        }

        private List<ProcLink> getShortestWayForTransm(int procSource, int procDest, int transmWeight)
        {
            if (procSource == procDest)
            {
                return new List<ProcLink>();
            }
            int[] ways = new int[procesors.Length];
            int[] prevVertex = new int[procesors.Length];
            for (int i = 0; i < ways.Length; i++)
            {
                ways[i] = Int32.MaxValue / 2;
                prevVertex[i] = Int32.MaxValue / 2;
            }
            ways[procSource] = 0;
            while (true)
            {
                bool recalc = false;
                for (int i = 0; i < incidenceProc.Length; i++)
                {
                    for (int j = 0; j < incidenceProc.Length; j++)
                    {
                        if (incidenceProc[i][j] != 0 && ways[j] > ways[i] + incidenceProc[i][j])
                        {
                            ways[j] = ways[i] + incidenceProc[i][j];
                            prevVertex[j] = i;
                            recalc = true;
                        }
                    }
                }
                if (!recalc)
                {
                    break;
                }
            }
            List<ProcLink> links = new List<ProcLink>();
            int currentVertex=procDest;
            while(currentVertex!=procSource)
            {
                int prev=prevVertex[currentVertex];
                links.Add(new ProcLink(prev, currentVertex));
                currentVertex = prev;
            }
            links.Reverse();
            List<ProcLink> weightedLinks = new List<ProcLink>();
            for (int i = 0; i < links.Count; i++)
            {
                int weight = incidenceProc[links.ElementAt(i).procFromNumber][links.ElementAt(i).procToNumber]*transmWeight;
                for (int j = 0; j < weight; j++)
                {
                    weightedLinks.Add(links.ElementAt(i));
                }
            }
            return weightedLinks;
        }

        private int getCriticalTimeForVertex(int taskSource)
        {
            int[] ways = new int[taskMap.Keys.Count];
            for (int i = 0; i < ways.Length; i++)
            {
                ways[i] = 0;
            }
            ways[taskSource] = taskMap[taskSource].weight;
            int max = 0;
            while (true)
            {
                bool recalc = false;
                for (int i = 0; i < incidenceTask.Length; i++)
                {
                    for (int j = 0; j < incidenceTask.Length; j++)
                    {
                        if (incidenceTask[i][j] != 0 && ways[j] < ways[i] + taskMap[j].weight)
                        {
                            ways[j] = ways[i] + taskMap[j].weight;
                            recalc = true;
                            if (ways[j] > max)
                            {
                                max = ways[j];
                            }
                        }
                    }
                }
                if (!recalc)
                {
                    break;
                }
            }
            return max;
        }

        public double[] makeStatistic(int executionTime)
        {
            double[] res = new double[3];
            int t1 = 0;
            for(int i=0;i<taskMap.Keys.Count;i++){
                Task task = taskMap[taskMap.Keys.ElementAt(i)];
                t1 += task.weight;
            }
            res[0] = ((double)t1) / executionTime;

            res[1] = res[0] / procMap.Keys.Count;

            int max = 0;
            for (int i = 0; i < taskMap.Keys.Count; i++)
            {
                int criticalTimeForV = getCriticalTimeForVertex(i);
                if (criticalTimeForV > max)
                {
                    max = criticalTimeForV;
                }
            }

            res[2] = ((double)max) / executionTime;
            return res;
        }

        private void addTaskToDiagram(Task task, int[] planInfo, List<DiagramRow> rows)
        {
            // add transmission
            int deployTaskStep=planInfo[0];
            for (int i = 0; i < task.requiredTransmission.Count; i++) {
                Transmission transm = task.requiredTransmission.ElementAt(i);
                Task taskFrom=taskMap[transm.taskFromNumber];
                transm.links = getShortestWayForTransm(taskFrom.procNumber, planInfo[1], incidenceTask[transm.taskFromNumber][transm.taskToNumber]);
                int stepNumber = taskFrom.finishStepNumber+1;
                for (int j = 0; j < transm.links.Count; j++)
                {
                    while (true)
                    {
                        if (rows.ElementAt(stepNumber).canUseLink(transm.links.ElementAt(j)))
                        {
                            rows.ElementAt(stepNumber).useLink(transm.links.ElementAt(j), transm);
                            stepNumber++;
                            break;
                        }
                        stepNumber++;
                    }
                }
                if (stepNumber > deployTaskStep)
                {
                    deployTaskStep = stepNumber;
                }
            }
            // add task
            task.procNumber = planInfo[1];
            task.state = TaskState.FINISHED;
            task.finishStepNumber = task.weight + deployTaskStep-1;
            Processor processor= procMap[task.procNumber];
            processor.freeStep = task.finishStepNumber;
            while (rows.ElementAt(deployTaskStep).states[planInfo[1]].task != null)
            {
                deployTaskStep++;
            }
            for (int i = 0; i < task.weight; i++)
            {
                rows.ElementAt(deployTaskStep + i).states[planInfo[1]].task = task;
            }
        }

        public String[][] buildPlan()
        {
            buildProc();
            buildTasks();
            List<DiagramRow> rows = initRows();

            while (!isAllTaskFinished())
            {
                for (int i = 0; i < quee.getQueue().Count; i++)
                {
                    Task task=taskMap[quee.getQueue().ElementAt(i)];
                    if (task.state == TaskState.FINISHED)
                    {
                        continue;
                    }
                    int stepForDeploy = getStepForDeployTask(task, rows);
                    if (stepForDeploy == Int32.MinValue)
                    {
                        continue;
                    }
                    int[] planTaskInfo = plann(task, stepForDeploy, rows);
                    addTaskToDiagram(task, planTaskInfo, rows);
                }
            }
            int maxRowCount=0;
            for (int i = 0; i < rows.Count; i++)
            {
                DiagramRow row = rows.ElementAt(i);
                if (row.usedProc.Count != 0)
                {
                    continue;
                }
                bool isUseState=false;
                for (int j = 0; j < row.states.Length; j++)
                {
                    DiagramState state = row.states[j];
                    if (state.transm.Count != 0 || state.task != null)
                    {
                        isUseState = true;
                        break;
                    }
                }
                if (isUseState)
                {
                    continue;
                }
                maxRowCount = i;
                break;
            }
            String[][] result=new String[maxRowCount][];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new String[procesors.Length];
                for (int j = 0; j < procesors.Length; j++)
                {
                    if (rows.ElementAt(i).states[j].task != null)
                    {
                        Task task = rows.ElementAt(i).states[j].task;
                        result[i][j] = "Task " + task.number+";";
                    }
                }
            }
            for (int i = 0; i < result.Length; i++)
            {
                for (int j = 0; j < procesors.Length; j++)
                {
                    DiagramState diagramState=rows.ElementAt(i).states[j];
                    for(int k=0;k<diagramState.transm.Count;k++)
                    {
                        Transmission transm = diagramState.transm.ElementAt(k);
                        Task taskFrom = taskMap[transm.taskFromNumber];
                        Task taskTo = taskMap[transm.taskToNumber];
                        if (j == taskTo.procNumber)
                        {
                            result[i][j] += "Receive from ";
                        }else{
                            result[i][j] += "Send from ";
                        }
                        result[i][j] += taskFrom.number + " to " + taskTo.number+";";
                    }
                }
            }
            return result;
        }

    }

    public enum TaskState{
        WAIT_TASK, FINISHED
    }

    public class Processor
    {
        public int number;
        public int relationCount=0;
        public int freeStep; // номер шага на котором процессор стал свободен
        public Processor(int number)
        {
            this.number = number;
        }
    }

    public class ProcLink
    {
        public int procFromNumber;
        public int procToNumber;
        public ProcLink(int procFromNumber, int procToNumber)
        {
            this.procFromNumber = procFromNumber;
            this.procToNumber = procToNumber;
        }
    }

    public class Transmission{
        public int taskFromNumber;
        public int taskToNumber;
        public List<ProcLink> links;
        public Transmission(int nodeFromNumber, int nodeToNumber)
        {
            this.taskFromNumber = nodeFromNumber;
            this.taskToNumber = nodeToNumber;
        }
    }

    public class Task
    {
        public TaskState state=TaskState.WAIT_TASK;
        public int number;
        public int weight;
        public int procNumber = Int32.MinValue;
        public int finishStepNumber = Int32.MinValue;
        public List<Transmission> requiredTransmission=new List<Transmission>();
        public Task(int number, int weight)
        {
            this.number = number;
            this.weight = weight;
        }
    }

    public class DiagramState
    {
        public List<Transmission> transm=new List<Transmission>();
        public Task task;
    }

    public class DiagramRow
    {
        public DiagramState[] states;
        public List<int> usedProc = new List<int>();
        public int number;
        public bool canUseLink(ProcLink link)
        {
            for (int i = 0; i < usedProc.Count; i++)
            {
                if (usedProc.ElementAt(i)== link.procFromNumber|| usedProc.ElementAt(i) == link.procToNumber)
                {
                    return false;
                }
            }
            return true;
        }

        public void useLink(ProcLink link, Transmission transm)
        {
            usedProc.Add(link.procFromNumber);
            usedProc.Add(link.procToNumber);
            
            states[link.procFromNumber].transm.Add(transm);
            states[link.procToNumber].transm.Add(transm);
        }
    }
}
