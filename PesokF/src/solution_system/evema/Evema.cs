using PesokF.src.w;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PesokF.MinimizeSystem;

namespace PesokF.src.solution_system.evema
{
    interface Evema
    {
        void BuildCircuit(List<Element>[] solution, bool bazis, bool trigger, TaskType taskType);
        void Start(int start);
        void Finish();
    }
}
