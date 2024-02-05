using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PesokF.src.s
{

    public class LogicalElement
    {
        public delegate void SignPattern(params KeyValuePair<int, bool>[] input);
        public delegate void Ready();
        public event SignPattern SendingSignal;
        public event Ready ready;
        public bool Signal { get; set; }
        public bool SelfSustained { get; }
        public int Adress { get; set; }
        public bool isReady { get; private set; }
        public List<LogicalElement> Parents { get; }
        public int HashCode { get; }
        public long HashCodeP { get; private set; }
        public Rectangle bound { get; set; }
        public Point outLocation { get; set; }
        public List<Point> inLocations { get; set; }
        public bool isLogicSignal { get; }
        public bool isLine { get; }
        public bool Resulting { get; set; }
        SignPattern SignProcess;
        List<KeyValuePair<int, bool>> input;

        public LogicalElement(Pattern pattern, bool selfSustained, int layer, int numInLayer, int sourceLayer)
        {
            isLine = false;
            switch (pattern)
            {
                case Pattern.Q:
                    SignProcess = QPattern;
                    break;
                case Pattern.NQ:
                    SignProcess = NQPattern;
                    break;
                case Pattern.OR_NOT:
                    SignProcess = OrNotPattern;
                    break;
                case Pattern.AND_NOT:
                    SignProcess = AndNotPattern;
                    break;
                case Pattern.TRIGGER:
                    SignProcess = TriggerPattern;
                    break;
                case Pattern.LINE:
                    SignProcess = LinePattern;
                    isLine = true;
                    break;
                default:
                    break;
            }
            isLogicSignal = false;
            SelfSustained = selfSustained;
            input = new List<KeyValuePair<int, bool>>();
            Parents = new List<LogicalElement>();
            inLocations = new List<Point>();
            Resulting = false;
            HashCode = numInLayer * 10 + layer;
            HashCodeP = layer == sourceLayer ? HashCode : 0;
        }

        public LogicalElement(bool signal, int l, int i)
        {
            isLine = false;
            isLogicSignal = true;
            SignProcess = LogicSignalPattern;
            Signal = signal;
            SelfSustained = false;
            input = new List<KeyValuePair<int, bool>>();
            Parents = new List<LogicalElement>();
            Resulting = false;
            HashCode = i * 10 + l;
            HashCodeP = l == 1 ? HashCode : 0;
        }

        public void Add(LogicalElement le)
        {
            le.SendingSignal += Recept;
            le.Adress = Parents.Count;
            Parents.Add(le);
            HashCodeP = ((HashCodeP / 10) * 1000 + le.HashCode) * 10 + Parents.Count;
        }

        private void Recept(params KeyValuePair<int, bool>[] input)
        {
            this.input.AddRange(input);
            if (isReady = this.input.Count == Parents.Count)
            {
                if (Resulting)
                {
                    if (SelfSustained)
                    {
                        TakeInput();
                    }
                    ready?.Invoke();
                }
                else
                {
                    if (SelfSustained)
                    {
                        TakeInput();
                        ForcedSending();
                    }
                }
            }
        }

        private void SendSignal()
        {
            SendingSignal?.Invoke(new KeyValuePair<int, bool>(Adress, Signal));
        }

        public void TakeInput()
        {
            SignProcess?.Invoke(input.ToArray());
            input.Clear();
        }

        public void ForcedSending()
        {
            SendSignal();
        }

        public void Start()
        {
            SendSignal();
        }

        public enum Pattern
        {
            Q,
            NQ,
            OR_NOT,
            AND_NOT,
            TRIGGER,
            LOGIC_SIGNAL,
            LINE
        }

        public void QPattern(params KeyValuePair<int, bool>[] input)
        {
            Signal = input[0].Value;
        }
        public void NQPattern(params KeyValuePair<int, bool>[] input)
        {
            Signal = !input[0].Value;
        }

        public void OrNotPattern(params KeyValuePair<int, bool>[] input)
        {
            bool signal = input[0].Value;
            for (int i = 1; i < input.Length; i++)
            {
                signal |= input[i].Value;
            }
            Signal = !signal;
        }

        public void AndNotPattern(params KeyValuePair<int, bool>[] input)
        {
            bool signal = input[0].Value;
            for (int i = 1; i < input.Length; i++)
            {
                signal &= input[i].Value;
            }
            Signal = !signal;
        }

        public void TriggerPattern(params KeyValuePair<int, bool>[] input)
        {
            if (input[0].Value == input[1].Value)
            {
                if (input[0].Value) Signal = !Signal;
            }
            else
            {
                Signal = input[input[0].Key].Value;
            }
        }
        public void LinePattern(params KeyValuePair<int, bool>[] input)
        {
            bool signal = false;
            for (int i = 0; i < input.Length; i++)
            {
                signal |= input[i].Value;
            }
            Signal = signal;
        }

        public void LogicSignalPattern(params KeyValuePair<int, bool>[] input)
        {

        }

        public override bool Equals(object obj)
        {
            return obj is LogicalElement element &&
                   HashCode == element.HashCode;
        }

        public override int GetHashCode()
        {
            return HashCode;
        }
    }
}
