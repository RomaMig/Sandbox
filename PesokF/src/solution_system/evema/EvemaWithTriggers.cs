
using PesokF.src.s;
using PesokF.src.solution_system.evema;
using PesokF.src.w;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static PesokF.MinimizeSystem;

namespace PesokF
{
    class EvemaWithTriggers : EvemaBased
    {
        public LogicalElement SwitchAlpha { get; }
        private readonly int SOURCE_LAYER = 1;
        private int startNum;
        private int count;

        public EvemaWithTriggers(int size) : base(size)
        {
            SwitchAlpha = new LogicalElement(false, -1, 0);
        }

        private new void Dump()
        {
            count = 0;
            base.Dump();
        }

        public new void Start(int start)
        {
            startNum = start;
            isReady = false;
            isComplete = false;
            for (int i = 0; i < size; i++)
            {
                Layers[0][i].Signal = (startNum >> (size - i - 1)) % 2 == 1;
                Layers[0][i].Resulting = true;
                Layers[0][i].ready += Finish;
            }
            for (int i = 0; i < size; i++)
            {
                Layers[0][i].Start();
            }
            StartAroundLayers();
        }

        private new void StartAroundLayers()
        {
            SwitchAlpha.Start();
            base.StartAroundLayers();
        }

        private new void Finish()
        {
            count++;
            bool comp = true;
            for (int i = 0; i < size; i++)
            {
                comp &= Layers[0][i].Signal == ((startNum >> (size - i - 1)) % 2 == 1);
            }
            isComplete = comp && (count > size);
            if (count % size == 0)
            {
                isReady = true;
            }
        }

        public new void Reset(int start)
        {
            count = 0;
            startNum = start;
            base.Reset(start);
        }

        public void Tick()
        {
            isReady = false;
            for (int i = 0; i < size; i++)
            {
                Layers[0][i].TakeInput();
            }

            for (int i = 0; i < size; i++)
            {
                Layers[0][i].ForcedSending();
            }
            TickAroundLayers();
        }

        private void TickAroundLayers()
        {
            SwitchAlpha.ForcedSending();
            for (int i = 1; i < Layers.Length; i++)
            {
                foreach (LogicalElement lr in Layers[i])
                {
                    if (!lr.SelfSustained)
                    {
                        lr.TakeInput();
                        lr.ForcedSending();
                    }
                }
            }
        }

        public override sealed void BuildCircuit(List<Element>[] solution, bool bazis, bool trigger, TaskType taskType)
        {
            Dump();
            for (int i = 0; i < size; i++)
            {
                LogicalElement le = new LogicalElement(LogicalElement.Pattern.TRIGGER, false, 0, Layers[0].Count, SOURCE_LAYER);
                Layers[0].Add(le);
            }
            if (taskType == TaskType.COUNTER_REV)
            {
                for (int i = 0; i < 2; i++)
                {
                    LogicalElement le = new LogicalElement(i % 2 == 0 ? LogicalElement.Pattern.Q : LogicalElement.Pattern.NQ, true, 1, Layers[1].Count, SOURCE_LAYER);
                    le.Add(SwitchAlpha);
                    Layers[1].Add(le);
                }
            }
            for (int i = 0; i < size * 2; i++)
            {
                LogicalElement le = new LogicalElement(i % 2 == 0 ? LogicalElement.Pattern.Q : LogicalElement.Pattern.NQ, true, 1, Layers[1].Count, SOURCE_LAYER);
                le.Add(Layers[0][i / 2]);
                Layers[1].Add(le);
            }
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
                        l3.Add(Layers[1][x]);
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
                                l2.Add(Layers[1][x]);
                            }
                            else
                            {
                                x = bazis ? x : x + ((x % 2) * (-2) + 1);
                                l3.Add(Layers[1][x]);
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
                Layers[3].Add(l3);
            }
            for (int i = 0; i < solution.Length / (trigger ? 2 : 1); i++)
            {
                if (trigger)
                {
                    Layers[0][i].Add(Layers[3][i * 2 + 1]);
                    Layers[0][i].Add(Layers[3][i * 2]);
                }
                else
                {
                    Layers[0][i].Add(Layers[3][i]);
                    Layers[0][i].Add(Layers[3][i]);
                }
            }
        }

    }
}
