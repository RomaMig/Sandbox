using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PesokF
{
    class Tables
    {
        delegate int[][][] Dl();
        Dl genTableWithRule;
        DataGridView dataGridView1;
        String task;
        int k1, k2;
        public int[] ones;
        public int[] zeros;
        string textTask;
        public int[][][] Mrx { get; private set; }
        public string[] numsS { get; private set; }
        public int Start { get; private set; }
        private Trigger trigger;

        public enum Trigger
        {
            T, RS, D
        }

        public override string ToString()
        {
            return textTask;
        }

        public Tables(DataGridView dataGridView1)
        {
            this.dataGridView1 = dataGridView1;
            trigger = Trigger.T;
        }

        public void Build(String task)
        {
            this.task = task;
            genTableWithRule = gridAutoTrans;
        }

        public void Build(bool config, int k)
        {
            k1 = k;
            if (config)
                genTableWithRule = gridCountSub;
            else
                genTableWithRule = gridCountAdd;
        }

        public void Build(int k1, int k2)
        {
            this.k1 = k1;
            this.k2 = k2;
            genTableWithRule = gridCountRevers;
        }

        public void Build(string ones, string zeros)
        {

            string[] OnesNum = ones.Split(' ');
            int[] Ones = new int[OnesNum.Length];
            for (int i = 0; i < Ones.Length; i++)
            {
                Ones[i] = Convert.ToInt32(OnesNum[i]);
            }
            this.ones = Ones;
            string[] ZerosNum = zeros.Split(' ');
            int[] Zeros = new int[ZerosNum.Length];
            for (int i = 0; i < Zeros.Length; i++)
            {
                Zeros[i] = Convert.ToInt32(ZerosNum[i]);
            }
            this.zeros = Zeros;
            genTableWithRule = gridSwitchFun;
        }

        public int[][][] Generate()
        {
            return (Mrx = genTableWithRule());
        }

        public void changeTrigger(Tables.Trigger trigger)
        {
            this.trigger = trigger;
        }

        string prepare(string s)
        {
            int n = s.Length;
            for (int i = 0; i < 4 - n; i++)
            {
                s = "0" + s;
            }
            return s;
        }

        string[] printTable(int[] Qt, int[] T, ref string output)
        {
            string[] Ts = new string[T.Length];
            for (int i = 0; i < Qt.Length; i++)
            {
                output += string.Format("{0}", prepare(Convert.ToString(i, 2)));
                if (T[i] >= 0)
                    output += string.Format("{0}{1}\n", prepare(Convert.ToString(Qt[i], 2)), Ts[i] = prepare(Convert.ToString(T[i], 2)));
                else
                {
                    output += string.Format("--------\n");
                    Ts[i] = "----";
                }
            }
            return Ts;
        }


        string[] truthTable(int[] F, ref string output)
        {
            string[] Ts = new string[F.Length];
            for (int i = 0; i < F.Length; i++)
            {
                output += string.Format("0000" + prepare(Convert.ToString(i, 2)));
                if (F[i] >= 0)
                    output += string.Format("{0}\n", Ts[i] = "" + F[i]);
                else
                {
                    output += string.Format("-\n");
                    Ts[i] = "-";
                }
            }
            return Ts;
        }

        string[] printTable(int[] Qt, int[][] T, ref string output)
        {
            string[] Ts = new string[T.Length];
            for (int i = 0; i < Qt.Length; i++)
            {
                output += string.Format("{0}", prepare(Convert.ToString(i, 2)));
                if (Qt[i] >= 0)
                {
                    output += string.Format("{0}", prepare(Convert.ToString(Qt[i], 2)));
                    for (int j = 0; j < T[i].Length; j++)
                        if (T[i][j] == -1)
                            Ts[i] += "-";
                        else
                            Ts[i] += T[i][j];
                    output += string.Format("{0}\n", Ts[i]);
                }
                else
                {
                    output += string.Format("------------\n");
                    Ts[i] = "--------";
                }
            }
            return Ts;
        }

        void printVeichechacha(string v, ref string output)
        {
            for (int i = 0; i < 4; i++, output += "\n")
            {
                for (int j = 0; j < 4; j++)
                {
                    output += v[i * 4 + j];
                }
            }
        }

        string Swap(string s, int n)
        {
            string s12 = s.Substring(0, 2 * n);
            string s3 = s.Substring(2 * n, n);
            string s4 = s.Substring(3 * n, n);
            return s12 + s4 + s3;
        }

        // n - номер столбца Т (n = 2 => T2)
        string Veichechacha(string[] T, int n)
        {
            int k = T.Length / 4;
            string VTn = "";
            for (int j = 0; j < 4; j++)
            {
                string Ttmp = "";
                // i - номер строки
                for (int i = j * k; i < j * k + k; i++)
                {
                    Ttmp += T[i][n];
                }
                VTn += Swap(Ttmp, 1);
            }
            return Swap(VTn, 4);
        }

        public void T(string input, out string output1, out string output2)
        {
            output1 = "";
            output2 = "";
            char separator = input.Contains("–") ? '–' : '-';
            numsS = input.Split(separator);
            int[] nums = new int[numsS.Length];
            int[] Qt = new int[16];
            int[] T = new int[16];
            textTask = "";
            for (int i = 0; i < nums.Length; i++)
            {
                nums[i] = Convert.ToInt32(numsS[i]);
                textTask += nums[i] + ".";
            }
            Start = nums[0];
            for (int i = 0; i < Qt.Length; i++)
            {
                Qt[i] = -1;
            }
            for (int i = 0; i < nums.Length - 1; i++)
            {
                Qt[nums[i]] = nums[i + 1];
            }
            for (int i = 0; i < T.Length; i++)
            {
                if (Qt[i] == -1) T[i] = -1;
                else T[i] = Qt[i] ^ i;
            }
            string[] Ts = printTable(Qt, T, ref output1);
            for (int i = 0; i < 4; i++)
            {
                printVeichechacha(Veichechacha(Ts, i), ref output2);
            }
        }
        public void D(string input, out string output1, out string output2)
        {
            output1 = "";
            output2 = "";
            char separator = input.Contains("–") ? '–' : '-';
            numsS = input.Split(separator);
            int[] nums = new int[numsS.Length];
            int[] Qt = new int[16];
            int[] D = new int[16];
            textTask = "";
            for (int i = 0; i < nums.Length; i++)
            {
                nums[i] = Convert.ToInt32(numsS[i]);
                textTask += nums[i] + ".";
            }
            Start = nums[0];
            for (int i = 0; i < Qt.Length; i++)
            {
                Qt[i] = -1;
            }
            for (int i = 0; i < nums.Length - 1; i++)
            {
                Qt[nums[i]] = nums[i + 1];
            }
            for (int i = 0; i < D.Length; i++)
            {
                if (Qt[i] == -1) D[i] = -1;
                else D[i] = Qt[i];
            }
            string[] Ts = printTable(Qt, D, ref output1);
            for (int i = 0; i < 4; i++)
            {
                printVeichechacha(Veichechacha(Ts, i), ref output2);
            }
        }

        private int getBitAt(int a, int i)
        {
            return (a >> i) & 1;
        }

        public void RS(string input, out string output1, out string output2)
        {
            output1 = "";
            output2 = "";
            char separator = input.Contains("–") ? '–' : '-';
            numsS = input.Split(separator);
            int[] nums = new int[numsS.Length];
            int[] Qt = new int[16];
            int[][] T = new int[16][];
            int[,,] m = { { { -1, 0 }, { 0, 1 } }, { { 1, 0 }, { 0, -1 } } };
            for (int i = 0; i < T.Length; i++)
            {
                T[i] = new int[8];
            }
            textTask = "";
            for (int i = 0; i < nums.Length; i++)
            {
                nums[i] = Convert.ToInt32(numsS[i]);
                textTask += nums[i] + ".";
            }
            Start = nums[0];
            for (int i = 0; i < Qt.Length; i++)
            {
                Qt[i] = -1;
            }
            for (int i = 0; i < nums.Length - 1; i++)
            {
                Qt[nums[i]] = nums[i + 1];
            }
            for (int i = 0; i < T.Length; i++)
            {
                for (int j = 0; j < T[i].Length / 2; j++)
                {
                    if (Qt[i] == -1)
                    {
                        T[i][j * 2] = -1;
                        T[i][j * 2 + 1] = -1;
                    }
                    else
                    {
                        int index = T[i].Length / 2 - j - 1;
                        T[i][j * 2] = m[getBitAt(i, index), getBitAt(Qt[i], index), 0];
                        T[i][j * 2 + 1] = m[getBitAt(i, index), getBitAt(Qt[i], index), 1];
                    }
                }
            }
            string[] Ts = printTable(Qt, T, ref output1);
            for (int i = 0; i < 8; i++)
            {
                printVeichechacha(Veichechacha(Ts, i), ref output2);
            }
        }


        public void kostulT(string input, out string output1, out string output2)
        {
            output1 = "";
            output2 = "";
            char separator = input.Contains("–") ? '–' : '-';
            numsS = input.Split(separator);
            int[] nums = new int[numsS.Length];
            int[] Qt = new int[16];
            int[] T = new int[16];
            textTask = "";
            for (int i = 0; i < nums.Length; i++)
            {
                nums[i] = Convert.ToInt32(numsS[i]);
                textTask += nums[i] + ".";
            }
            Start = nums[0];
            for (int i = 0; i < Qt.Length; i++)
            {
                Qt[i] = -1;
            }
            for (int i = 0; i < nums.Length - 1; i++)
            {
                Qt[nums[i]] = nums[i + 1];
            }
            Qt[0] = 1;
            for (int i = 0; i < T.Length; i++)
            {
                if (Qt[i] == -1) T[i] = -1;
                else T[i] = Qt[i] ^ i;
            }
            string[] Ts = printTable(Qt, T, ref output1);
            for (int i = 0; i < 4; i++)
            {
                printVeichechacha(Veichechacha(Ts, i), ref output2);
            }
        }

        public void kostulD(string input, out string output1, out string output2)
        {
            output1 = "";
            output2 = "";
            char separator = input.Contains("–") ? '–' : '-';
            numsS = input.Split(separator);
            int[] nums = new int[numsS.Length];
            int[] Qt = new int[16];
            int[] D = new int[16];
            textTask = "";
            for (int i = 0; i < nums.Length; i++)
            {
                nums[i] = Convert.ToInt32(numsS[i]);
                textTask += nums[i] + ".";
            }
            Start = nums[0];
            for (int i = 0; i < Qt.Length; i++)
            {
                Qt[i] = -1;
            }
            for (int i = 0; i < nums.Length - 1; i++)
            {
                Qt[nums[i]] = nums[i + 1];
            }
            Qt[0] = 1;
            for (int i = 0; i < D.Length; i++)
            {
                if (Qt[i] == -1) D[i] = -1;
                else D[i] = Qt[i];
            }
            string[] Ts = printTable(Qt, D, ref output1);
            for (int i = 0; i < 4; i++)
            {
                printVeichechacha(Veichechacha(Ts, i), ref output2);
            }
        }


        public void kostulRS(string input, out string output1, out string output2)
        {
            output1 = "";
            output2 = "";
            char separator = input.Contains("–") ? '–' : '-';
            numsS = input.Split(separator);
            int[] nums = new int[numsS.Length];
            int[] Qt = new int[16];
            int[][] T = new int[16][];
            int[,,] m = { { { -1, 0 }, { 0, 1 } }, { { 1, 0 }, { 0, -1 } } };
            for (int i = 0; i < T.Length; i++)
            {
                T[i] = new int[8];
            }
            textTask = "";
            for (int i = 0; i < nums.Length; i++)
            {
                nums[i] = Convert.ToInt32(numsS[i]);
                textTask += nums[i] + ".";
            }
            Start = nums[0];
            for (int i = 0; i < Qt.Length; i++)
            {
                Qt[i] = -1;
            }
            for (int i = 0; i < nums.Length - 1; i++)
            {
                Qt[nums[i]] = nums[i + 1];
            }
            Qt[0] = 1;
            for (int i = 0; i < T.Length; i++)
            {
                for (int j = 0; j < T[i].Length / 2; j++)
                {
                    if (Qt[i] == -1)
                    {
                        T[i][j * 2] = -1;
                        T[i][j * 2 + 1] = -1;
                    }
                    else
                    {
                        int index = T[i].Length / 2 - j - 1;
                        T[i][j * 2] = m[getBitAt(i, index), getBitAt(Qt[i], index), 0];
                        T[i][j * 2 + 1] = m[getBitAt(i, index), getBitAt(Qt[i], index), 1];
                    }
                }
            }
            string[] Ts = printTable(Qt, T, ref output1);
            for (int i = 0; i < 8; i++)
            {
                printVeichechacha(Veichechacha(Ts, i), ref output2);
            }
        }


        public int[][][] gridCountAdd()
        {
            String tmp = "0";
            for (int i = 1; i <= k1; i++)
            {
                tmp += "-";
                tmp += i;
            }
            tmp += "-";
            tmp += 0;
            task = tmp;
            return gridAutoTrans();
        }

        public int[][][] gridCountSub()
        {
            String tmp = k1.ToString();
            for (int i = k1 - 1; i >= 0; i--)
            {
                tmp += "-";
                tmp += i;
            }
            tmp += "-";
            tmp += k1.ToString();
            task = tmp;
            return gridAutoTrans();
        }

        public int[][][] gridCountRevers()
        {
            String tmp = "0";
            //for (int i = k1; i >= 0; i--)
            //{
            //    tmp += "-";
            //    tmp += i;
            //}
            //tmp += "-";
            //tmp += 0;
            //for (int i = 0b1000; i <= (0b1000 | k2); i++)
            //{
            //    tmp += "-";
            //    tmp += i;
            //}
            //tmp += "-";
            //tmp += 0b1000;
            for (int i = 0; i <= k1; i++)
            {
                tmp += "-";
                tmp += i;
            }
            tmp += "-";
            tmp += 0;
            tmp += "-";
            tmp += 0b1000;
            for (int i = (0b1000 | k2); i >= 0b1000; i--)
            {
                tmp += "-";
                tmp += i;
            }
            task = tmp;

            switch (trigger)
            {
                case Trigger.T:
                    return kostulgridT();
                case Trigger.RS:
                    return kostulgridRS();
                case Trigger.D:
                    return kostulgridD();
                default:
                    return null;
            }
        }

        public int[][][] gridSwitchFun()
        {
            string[] heads = { "F" };
            while (dataGridView1.Columns.Count > 9)
                dataGridView1.Columns.RemoveAt(dataGridView1.Columns.Count - 1);
            for (int i = 0; i < heads.Length; i++)
            {
                DataGridViewColumn T = new DataGridViewTextBoxColumn();
                T.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
                T.HeaderText = heads[i];
                T.Name = heads[i];
                T.ReadOnly = true;
                T.Width = 45;
                dataGridView1.Columns.Add(T);
            }

            string s1, s2;
            switchFunc(out s1, out s2);
            int len = 9;
            for (int i = 0; i < 16; ++i)
            {
                //Добавляем строку, указывая значения колонок поочереди слева направо
                char[] c = s1.ToCharArray(i * len + i, len);
                object[] o = new object[len + 1];
                o[0] = i;
                for (int j = 1; j < o.Length; j++)
                {
                    o[j] = c[j - 1];
                }
                dataGridView1.Rows.Add(o);
            }

            len = 4;
            int count = 1;
            int[][][] mrx = new int[count][][];
            for (int i = 0; i < mrx.Length; i++)
            {
                mrx[i] = new int[len][];
                for (int j = 0; j < mrx[i].Length; j++)
                {
                    mrx[i][j] = new int[len];
                }
            }
            for (int i = 0; i < len * count; ++i)
            {
                char[] c = s2.ToCharArray(i * len + i, len);
                object[] o = new object[len];
                for (int j = 0; j < o.Length; j++)
                {
                    o[j] = c[j];
                    switch (c[j])
                    {
                        case '-':
                            mrx[i / len][i % len][j] = -1;
                            break;
                        case '0':
                            mrx[i / len][i % len][j] = 0;
                            break;
                        case '1':
                            mrx[i / len][i % len][j] = 1;
                            break;
                    }
                }
                if (i % len == 0) dataGridView1.Rows.Add(heads[i / len]);
                dataGridView1.Rows.Add(o);
            }
            return mrx;
        }


        public int[][][] gridAutoTrans()
        {
            switch (trigger)
            {
                case Trigger.T:
                    return gridT();
                case Trigger.RS:
                    return gridRS();
                case Trigger.D:
                    return gridD();
                default:
                    return null;
            }
        }

        public void switchFunc(out string output1, out string output2)
        {
            output1 = "";
            output2 = "";
            int[] F = new int[16];
            for (int i = 0; i < F.Length; i++)
            {
                F[i] = -1;
            }
            for (int i = 0; i < ones.Length; i++)
            {
                F[ones[i]] = 1;
            }
            for (int i = 0; i < zeros.Length; i++)
            {
                F[zeros[i]] = 0;
            }
            string[] Ts = truthTable(F, ref output1);
            printVeichechacha(Veichechacha(Ts, 0), ref output2);
        }

        public int[][][] gridT()
        {
            string[] heads = { "T1", "T2", "T3", "T4" };
            while (dataGridView1.Columns.Count > 9)
                dataGridView1.Columns.RemoveAt(dataGridView1.Columns.Count - 1);
            for (int i = 0; i < heads.Length; i++)
            {
                DataGridViewColumn T = new DataGridViewTextBoxColumn();
                T.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
                T.HeaderText = heads[i];
                T.Name = heads[i];
                T.ReadOnly = true;
                T.Width = 45;
                dataGridView1.Columns.Add(T);
            }

            string s1, s2;
            T(task, out s1, out s2);
            int len = 12;
            for (int i = 0; i < 16; ++i)
            {
                //Добавляем строку, указывая значения колонок поочереди слева направо
                char[] c = s1.ToCharArray(i * len + i, len);
                object[] o = new object[len + 1];
                o[0] = i;
                for (int j = 1; j < o.Length; j++)
                {
                    o[j] = c[j - 1];
                }
                dataGridView1.Rows.Add(o);
            }
            len = 4;
            int count = 4;
            int[][][] mrx = new int[count][][];
            for (int i = 0; i < mrx.Length; i++)
            {
                mrx[i] = new int[len][];
                for (int j = 0; j < mrx[i].Length; j++)
                {
                    mrx[i][j] = new int[len];
                }
            }
            for (int i = 0; i < len * count; ++i)
            {
                char[] c = s2.ToCharArray(i * len + i, len);
                object[] o = new object[len];
                for (int j = 0; j < o.Length; j++)
                {
                    o[j] = c[j];
                    switch (c[j])
                    {
                        case '-':
                            mrx[i / len][i % len][j] = -1;
                            break;
                        case '0':
                            mrx[i / len][i % len][j] = 0;
                            break;
                        case '1':
                            mrx[i / len][i % len][j] = 1;
                            break;
                    }
                }
                if (i % len == 0) dataGridView1.Rows.Add(heads[i / len]);
                dataGridView1.Rows.Add(o);
            }
            return mrx;
        }

        public int[][][] gridRS()
        {
            string[] heads = { "R1", "S1", "R2", "S2", "R3", "S3", "R4", "S4" };
            while (dataGridView1.Columns.Count > 9)
                dataGridView1.Columns.RemoveAt(dataGridView1.Columns.Count - 1);
            for (int i = 0; i < heads.Length; i++)
            {
                DataGridViewColumn T = new DataGridViewTextBoxColumn();
                T.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
                T.HeaderText = heads[i];
                T.Name = heads[i];
                T.ReadOnly = true;
                T.Width = 45;
                dataGridView1.Columns.Add(T);
            }
            string s1, s2;
            RS(task, out s1, out s2);
            int len = 16;
            for (int i = 0; i < 16; ++i)
            {
                //Добавляем строку, указывая значения колонок поочереди слева направо
                char[] c = s1.ToCharArray(i * len + i, len);
                object[] o = new object[len + 1];
                o[0] = i;
                for (int j = 1; j < o.Length; j++)
                {
                    o[j] = c[j - 1];
                }
                dataGridView1.Rows.Add(o);
            }
            len = 4;
            int count = 8;
            int[][][] mrx = new int[count][][];
            for (int i = 0; i < mrx.Length; i++)
            {
                mrx[i] = new int[len][];
                for (int j = 0; j < mrx[i].Length; j++)
                {
                    mrx[i][j] = new int[len];
                }
            }
            for (int i = 0; i < len * count; ++i)
            {
                char[] c = s2.ToCharArray(i * len + i, len);
                object[] o = new object[len];
                for (int j = 0; j < o.Length; j++)
                {
                    o[j] = c[j];
                    switch (c[j])
                    {
                        case '-':
                            mrx[i / len][i % len][j] = -1;
                            break;
                        case '0':
                            mrx[i / len][i % len][j] = 0;
                            break;
                        case '1':
                            mrx[i / len][i % len][j] = 1;
                            break;
                    }
                }
                if (i % len == 0) dataGridView1.Rows.Add(heads[i / len]);
                dataGridView1.Rows.Add(o);
            }
            return mrx;
        }


        public int[][][] gridD()
        {
            string[] heads = { "D1", "D2", "D3", "D4" };
            while (dataGridView1.Columns.Count > 9)
                dataGridView1.Columns.RemoveAt(dataGridView1.Columns.Count - 1);
            for (int i = 0; i < heads.Length; i++)
            {
                DataGridViewColumn T = new DataGridViewTextBoxColumn();
                T.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
                T.HeaderText = heads[i];
                T.Name = heads[i];
                T.ReadOnly = true;
                T.Width = 45;
                dataGridView1.Columns.Add(T);
            }

            string s1, s2;
            D(task, out s1, out s2);
            int len = 12;
            for (int i = 0; i < 16; ++i)
            {
                //Добавляем строку, указывая значения колонок поочереди слева направо
                char[] c = s1.ToCharArray(i * len + i, len);
                object[] o = new object[len + 1];
                o[0] = i;
                for (int j = 1; j < o.Length; j++)
                {
                    o[j] = c[j - 1];
                }
                dataGridView1.Rows.Add(o);
            }
            len = 4;
            int count = 4;
            int[][][] mrx = new int[count][][];
            for (int i = 0; i < mrx.Length; i++)
            {
                mrx[i] = new int[len][];
                for (int j = 0; j < mrx[i].Length; j++)
                {
                    mrx[i][j] = new int[len];
                }
            }
            for (int i = 0; i < len * count; ++i)
            {
                char[] c = s2.ToCharArray(i * len + i, len);
                object[] o = new object[len];
                for (int j = 0; j < o.Length; j++)
                {
                    o[j] = c[j];
                    switch (c[j])
                    {
                        case '-':
                            mrx[i / len][i % len][j] = -1;
                            break;
                        case '0':
                            mrx[i / len][i % len][j] = 0;
                            break;
                        case '1':
                            mrx[i / len][i % len][j] = 1;
                            break;
                    }
                }
                if (i % len == 0) dataGridView1.Rows.Add(heads[i / len]);
                dataGridView1.Rows.Add(o);
            }
            return mrx;
        }


        public int[][][] kostulgridT()
        {
            string[] heads = { "T1", "T2", "T3", "T4" };
            while (dataGridView1.Columns.Count > 9)
                dataGridView1.Columns.RemoveAt(dataGridView1.Columns.Count - 1);
            for (int i = 0; i < heads.Length; i++)
            {
                DataGridViewColumn T = new DataGridViewTextBoxColumn();
                T.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
                T.HeaderText = heads[i];
                T.Name = heads[i];
                T.ReadOnly = true;
                T.Width = 45;
                dataGridView1.Columns.Add(T);
            }

            string s1, s2;
            kostulT(task, out s1, out s2);
            int len = 12;
            for (int i = 0; i < 16; ++i)
            {
                //Добавляем строку, указывая значения колонок поочереди слева направо
                char[] c = s1.ToCharArray(i * len + i, len);
                object[] o = new object[len + 1];
                o[0] = i;
                for (int j = 1; j < o.Length; j++)
                {
                    o[j] = c[j - 1];
                }
                dataGridView1.Rows.Add(o);
            }
            len = 4;
            int count = 4;
            int[][][] mrx = new int[count][][];
            for (int i = 0; i < mrx.Length; i++)
            {
                mrx[i] = new int[len][];
                for (int j = 0; j < mrx[i].Length; j++)
                {
                    mrx[i][j] = new int[len];
                }
            }
            for (int i = 0; i < len * count; ++i)
            {
                char[] c = s2.ToCharArray(i * len + i, len);
                object[] o = new object[len];
                for (int j = 0; j < o.Length; j++)
                {
                    o[j] = c[j];
                    switch (c[j])
                    {
                        case '-':
                            mrx[i / len][i % len][j] = -1;
                            break;
                        case '0':
                            mrx[i / len][i % len][j] = 0;
                            break;
                        case '1':
                            mrx[i / len][i % len][j] = 1;
                            break;
                    }
                }
                if (i % len == 0) dataGridView1.Rows.Add(heads[i / len]);
                dataGridView1.Rows.Add(o);
            }
            int[][][] res = new int[count - 1][][];

            for (int i = 0; i < res.Length; i++)
            {
                res[i] = mrx[i + 1];
            }
            return res;
        }

        public int[][][] kostulgridD()
        {
            string[] heads = { "D1", "D2", "D3", "D4" };
            while (dataGridView1.Columns.Count > 9)
                dataGridView1.Columns.RemoveAt(dataGridView1.Columns.Count - 1);
            for (int i = 0; i < heads.Length; i++)
            {
                DataGridViewColumn T = new DataGridViewTextBoxColumn();
                T.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
                T.HeaderText = heads[i];
                T.Name = heads[i];
                T.ReadOnly = true;
                T.Width = 45;
                dataGridView1.Columns.Add(T);
            }

            string s1, s2;
            kostulD(task, out s1, out s2);
            int len = 12;
            for (int i = 0; i < 16; ++i)
            {
                //Добавляем строку, указывая значения колонок поочереди слева направо
                char[] c = s1.ToCharArray(i * len + i, len);
                object[] o = new object[len + 1];
                o[0] = i;
                for (int j = 1; j < o.Length; j++)
                {
                    o[j] = c[j - 1];
                }
                dataGridView1.Rows.Add(o);
            }
            len = 4;
            int count = 4;
            int[][][] mrx = new int[count][][];
            for (int i = 0; i < mrx.Length; i++)
            {
                mrx[i] = new int[len][];
                for (int j = 0; j < mrx[i].Length; j++)
                {
                    mrx[i][j] = new int[len];
                }
            }
            for (int i = 0; i < len * count; ++i)
            {
                char[] c = s2.ToCharArray(i * len + i, len);
                object[] o = new object[len];
                for (int j = 0; j < o.Length; j++)
                {
                    o[j] = c[j];
                    switch (c[j])
                    {
                        case '-':
                            mrx[i / len][i % len][j] = -1;
                            break;
                        case '0':
                            mrx[i / len][i % len][j] = 0;
                            break;
                        case '1':
                            mrx[i / len][i % len][j] = 1;
                            break;
                    }
                }
                if (i % len == 0) dataGridView1.Rows.Add(heads[i / len]);
                dataGridView1.Rows.Add(o);
            }
            int[][][] res = new int[count - 1][][];

            for (int i = 0; i < res.Length; i++)
            {
                res[i] = mrx[i + 1];
            }
            return res;
        }
        public int[][][] kostulgridRS()
        {
            string[] heads = { "R1", "S1", "R2", "S2", "R3", "S3", "R4", "S4" };
            while (dataGridView1.Columns.Count > 9)
                dataGridView1.Columns.RemoveAt(dataGridView1.Columns.Count - 1);
            for (int i = 0; i < heads.Length; i++)
            {
                DataGridViewColumn T = new DataGridViewTextBoxColumn();
                T.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
                T.HeaderText = heads[i];
                T.Name = heads[i];
                T.ReadOnly = true;
                T.Width = 45;
                dataGridView1.Columns.Add(T);
            }
            string s1, s2;
            kostulRS(task, out s1, out s2);
            int len = 16;
            for (int i = 0; i < 16; ++i)
            {
                //Добавляем строку, указывая значения колонок поочереди слева направо
                char[] c = s1.ToCharArray(i * len + i, len);
                object[] o = new object[len + 1];
                o[0] = i;
                for (int j = 1; j < o.Length; j++)
                {
                    o[j] = c[j - 1];
                }
                dataGridView1.Rows.Add(o);
            }
            len = 4;
            int count = 8;
            int[][][] mrx = new int[count][][];
            for (int i = 0; i < mrx.Length; i++)
            {
                mrx[i] = new int[len][];
                for (int j = 0; j < mrx[i].Length; j++)
                {
                    mrx[i][j] = new int[len];
                }
            }
            for (int i = 0; i < len * count; ++i)
            {
                char[] c = s2.ToCharArray(i * len + i, len);
                object[] o = new object[len];
                for (int j = 0; j < o.Length; j++)
                {
                    o[j] = c[j];
                    switch (c[j])
                    {
                        case '-':
                            mrx[i / len][i % len][j] = -1;
                            break;
                        case '0':
                            mrx[i / len][i % len][j] = 0;
                            break;
                        case '1':
                            mrx[i / len][i % len][j] = 1;
                            break;
                    }
                }
                if (i % len == 0) dataGridView1.Rows.Add(heads[i / len]);
                dataGridView1.Rows.Add(o);
            }
            int[][][] res = new int[count - 1][][];

            for (int i = 0; i < res.Length; i++)
            {
                res[i] = mrx[i + 1];
            }
            return res;
        }
    }
}
