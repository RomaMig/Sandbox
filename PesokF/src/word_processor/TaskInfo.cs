using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PesokF.src.w
{
    struct TaskInfo
    {
        public TaskType Type { get; }
        public string Task { get; }
        public int[] Ones { get; }
        public int[] Zeros { get; }
        public int k1 { get; }
        public int k2 { get; }

        public TaskInfo(string task)
        {
            this.Type = TaskType.AUTOMAT;
            this.Task = task;
            this.Ones = null;
            this.Zeros = null;
            this.k1 = 0;
            this.k2 = 0;
        }
        public TaskInfo(int[] ones, int[] zeros)
        {
            this.Type = TaskType.SWITCH_FUNC;
            this.Task = null;
            this.Ones = ones;
            this.Zeros = zeros;
            this.k1 = 0;
            this.k2 = 0;
        }
        public TaskInfo(int k1, TaskType type)
        {
            this.Type = type;
            this.Task = null;
            this.Ones = null;
            this.Zeros = null;
            this.k1 = k1;
            this.k2 = 0;
        }
        public TaskInfo(int k1, int k2)
        {
            this.Type = TaskType.COUNTER_REV;
            this.Task = null;
            this.Ones = null;
            this.Zeros = null;
            this.k1 = k1;
            this.k2 = k2;
        }
    }

    public enum TaskType
    {
        AUTOMAT,
        COUNTER_ADD,
        COUNTER_SUB,
        COUNTER_REV,
        COUNTER_REV_INVERSE,
        SWITCH_FUNC
    }
}
