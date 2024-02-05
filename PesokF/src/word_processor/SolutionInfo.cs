using System;
using System.Drawing;

namespace PesokF.src.w
{
    struct SolutionInfo
    {
        public TaskInfo taskInfo { get; }
        public bool bazis { get; }
        public string trigger { get; }
        public string[,] mainTable { get; }
        public int[][][] veichTables { get; }
        public string [] transitions { get; }
        public Image[] splices { get; }
        public Image[] formulae { get; }
        public Image[] evema { get; }

        public SolutionInfo(TaskInfo taskInfo, bool bazis, string trigger, string[,] mainTable, int[][][] veichTables, string[] transitions, Image[] splices, Image[] formulae, Image[] evema)
        {
            this.taskInfo = taskInfo;
            this.bazis = bazis;
            this.trigger = trigger;
            this.mainTable = mainTable;
            this.veichTables = veichTables;
            this.transitions = transitions;
            this.splices = splices;
            this.formulae = formulae;
            this.evema = evema;
        }
    }
}