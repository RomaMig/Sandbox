using PesokF.src.s;
using PesokF.src.w;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static PesokF.MinimizeSystem;

namespace PesokF.src.solution_system.evema
{
    class EvemaBased : Evema
    {
        public bool isReady { get; protected set; }
        public bool isComplete { get; protected set; }
        public List<LogicalElement>[] Layers { get => layers; }
        public List<LogicalElement>[] layers;
        private readonly int SOURCE_LAYER = 0;
        protected int size;

        public EvemaBased(int size)
        {
            this.size = size;
            layers = new List<LogicalElement>[4];
        }

        protected void Dump()
        {
            for (int i = 0; i < Layers.Length; i++)
            {
                Layers[i] = new List<LogicalElement>();
            }
        }

        public void Start(int start)
        {
            isReady = false;
            isComplete = false;
            for (int i = 0; i < size; i++)
            {
                Layers[0][i].Signal = (start >> (size - i - 1)) % 2 == 1;
            }
            for (int i = 0; i < size; i++)
            {
                Layers[0][i].Start();
            }
            StartAroundLayers();
        }

        protected void StartAroundLayers()
        {
            for (int i = 1; i < Layers.Length; i++)
            {
                foreach (LogicalElement lr in Layers[i])
                {
                    if (!lr.SelfSustained)
                    {
                        lr.Start();
                    }
                }
            }
        }

        public void Finish()
        {
            isComplete = true;
        }

        public void Reset(int start)
        {
            isReady = false;
            isComplete = false;
            for (int i = 0; i < size; i++)
            {
                Layers[0][i].Signal = (start >> (size - i - 1)) % 2 == 1;
                Layers[0][i].Start();
            }
            StartAroundLayers();
        }

        public virtual void BuildCircuit(List<Element>[] solution, bool bazis, bool trigger, TaskType taskType)
        {
            Dump();
            for (int i = 0; i < 4; i++) //кол-во переменных
            {
                LogicalElement l0 = new LogicalElement(false, 0, Layers[0].Count);
                Layers[0].Add(l0);
            }
            for (int i = 0; i < 4; i++) //кол-во переменных
            {
                LogicalElement l1 = new LogicalElement(bazis ? LogicalElement.Pattern.OR_NOT : LogicalElement.Pattern.AND_NOT, true, 1, Layers[1].Count, SOURCE_LAYER);
                l1.Add(Layers[0][i]);
                Layers[1].Add(l1);
            }
            int unused = 15;
            Dictionary<long, LogicalElement> unicL2 = new Dictionary<long, LogicalElement>();
            for (int i = 0; i < solution.Length; i++)
            {
                LogicalElement l3 = null;

                if (solution[i].Count == 1)
                {
                    Element e = solution[i][0];
                    if (solution[i][0].type == Element.Type.EIGHT_H || solution[i][0].type == Element.Type.EIGHT_V)
                    {
                        l3 = new LogicalElement(LogicalElement.Pattern.LINE, true, 3, Layers[3].Count, SOURCE_LAYER);
                        int x = (e.origin % 10) - 1;
                        if (x > 3) x = (x % 4) * 2 + 1;
                        else x *= 2;
                        x = !bazis ? x : x + ((x % 2) * (-2) + 1);
                        unused &= ~(1 << (x / 2));
                        l3.Add(Layers[x % 2][x / 2]);
                    }
                    else
                    {
                        if (e.type == Element.Type.SIXTEEN)
                        {
                            l3 = new LogicalElement(e.origin == 1, 3, Layers[3].Count);
                        }
                    }
                }
                if (l3 == null)
                {
                    l3 = new LogicalElement(bazis ? LogicalElement.Pattern.OR_NOT : LogicalElement.Pattern.AND_NOT, true, 3, Layers[3].Count, SOURCE_LAYER);
                    foreach (Element e in solution[i])
                    {
                        LogicalElement l2;
                        l2 = new LogicalElement(bazis ? LogicalElement.Pattern.OR_NOT : LogicalElement.Pattern.AND_NOT, true, 2, Layers[2].Count, SOURCE_LAYER);
                        int n = e.origin;
                        while (n > 0)
                        {
                            int x = (n % 10) - 1;
                            if (x > 3) x = (x % 4) * 2 + 1;
                            else x *= 2;
                            if (e.origin > 10)
                            {
                                x = !bazis ? x : x + ((x % 2) * (-2) + 1);
                                unused &= ~((x % 2) << (x / 2));
                                l2.Add(Layers[x % 2][x / 2]);
                            }
                            else
                            {
                                x = bazis ? x : x + ((x % 2) * (-2) + 1);
                                unused &= ~(1 << (x / 2));
                                l3.Add(Layers[x % 2][x / 2]);
                            }
                            n /= 10;
                        }
                        if (l2.Parents.Count > 0)
                        {
                            if (!unicL2.ContainsKey(l2.HashCodeP))
                            {
                                unicL2.Add(l2.HashCodeP, l2);
                                Layers[2].Add(l2);
                            }
                            else
                            {
                                unicL2.TryGetValue(l2.HashCodeP, out l2);
                            }
                            l3.Add(l2);
                        }
                    }
                }
                l3.ready += Finish;
                l3.Resulting = true;
                Layers[3].Add(l3);
            }
            for (int i = 0, d = 0; i < 4; i++)
            {
                if (unused % 2 == 1)
                {
                    Layers[1].RemoveAt(i - d);
                    d++;
                }
                unused /= 2;
            }
        }
    }
}
