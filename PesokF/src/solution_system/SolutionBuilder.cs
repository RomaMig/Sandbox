using PesokF.src.solution_system.evema;
using PesokF.src.w;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static PesokF.MinimizeSystem;

namespace PesokF.src.s
{
    class SolutionBuilder
    {
        public States State { get => state; }
        public delegate void Completion();
        private Completion completion;
        private States state;
        private int configNum;
        private bool trigger;
        private bool bazis;
        private TaskInfo taskInfo;
        private Thread draw;
        private MinimizeSystem minSystem;
        private Tables table;
        private EvemaBased evemaB;
        private EvemaWithTriggers evemaT;
        private Images images;
        private SplitContainer[] containers;
        private PictureBox[] picturesS;
        private PictureBox[] picturesF;
        private PictureBox circuit;
        private List<PictureBox> picturesE;
        private PictureBox[] picturesEtmp;
        private TabPage background;
        private List<Element>[][] solution;
        private DataGridView dataGridView;
        private TextBox configView;
        private ToolStripProgressBar progress;
        private ToolStripLabel text;
        private Label weight;

        public SolutionBuilder(DataGridView dataGridView, SplitContainer[] containers, PictureBox[] picturesS, PictureBox[] picturesF, PictureBox circuit, PictureBox[] picturesEtmp, TabPage background, Completion completion, TextBox configView, ToolStripProgressBar progress, ToolStripLabel text, Label weight)
        {
            this.ConfigNum = 0;
            table = new Tables(dataGridView);
            minSystem = new MinimizeSystem(progress, text, false);
            minSystem.Resolve += takeSolution;
            images = new Images();
            picturesE = new List<PictureBox>();
            state = States.FREE;
            this.containers = containers;
            this.picturesS = picturesS;
            this.picturesF = picturesF;
            this.circuit = circuit;
            this.picturesEtmp = picturesEtmp;
            this.background = background;
            this.completion = completion;
            this.dataGridView = dataGridView;
            this.configView = configView;
            this.progress = progress;
            this.text = text;
            this.weight = weight;
        }

        public enum States
        {
            SEARCH,
            DRAWNING,
            EVEMA,
            SAVE,
            REPORT,
            FREE
        }

        public void BuildSolution(string task, Tables.Trigger trigger, bool bazis, int deep, float diversity, out string name)
        {
            name = "";
            try
            {
                if (state == States.FREE)
                    BuildAutomat(task, trigger, bazis, deep, diversity, out name);
            }
            catch (ThreadAbortException)
            {

            }
            catch (Exception e)
            {
                MessageBox.Show("Solution Builder:\n" + e.Source + "\n----------\n" + e.Message + "\n----------\n" + e.StackTrace + "\n----------\n" + e.Data);
            }
        }

        public void BuildSolution(string task, int k1, int k2, Tables.Trigger trigger, bool bazis, int deep, float diversity, out string name)
        {
            name = "";
            try
            {
                if (state == States.FREE)
                    BuildCounter(task, k1, k2, trigger, bazis, deep, diversity, out name);
            }
            catch (ThreadAbortException)
            {

            }
            catch (Exception e)
            {
                MessageBox.Show("Solution Builder:\n" + e.Source + "\n----------\n" + e.Message + "\n----------\n" + e.StackTrace + "\n----------\n" + e.Data);
            }
        }

        public void BuildSolution(string task1, string task2, Tables.Trigger trigger, bool bazis, int deep, float diversity, out string name)
        {
            name = "";
            try
            {
                if (state == States.FREE)
                    BuildSwitchFunc(task1, task2, trigger, bazis, deep, diversity, out name);
            }
            catch (ThreadAbortException)
            {

            }
            catch (Exception e)
            {
                MessageBox.Show("Solution Builder:\n" + e.Message + "\n----------\n" + e.StackTrace + "\n----------\n" + e.Source + "\n----------\n" + e.Data);
            }
        }

        private void BuildAutomat(string task, Tables.Trigger trigger, bool bazis, int deep, float diversity, out string name)
        {
            state = States.SEARCH;
            name = "";
            taskInfo = new TaskInfo(task);
            this.trigger = trigger == Tables.Trigger.RS;
            this.bazis = bazis;

            table.changeTrigger(trigger);
            minSystem.bazis = bazis;
            evemaT = null;
            table.Build(task);
            for (int i = 0; i < picturesEtmp.Length; i++)
            {
                picturesEtmp[i].Image = null;
            }
            int[][][] tmp = table.Generate();
            name = (this.trigger ? "RS" : "T") + @"\" + (bazis ? "ИЛИ-НЕ" : "И-НЕ") + @"\" + table.ToString();

            minSystem.SearchMinSolution(tmp, deep, diversity);
        }

        private void BuildCounter(string task, int k1, int k2, Tables.Trigger trigger, bool bazis, int deep, float diversity, out string name)
        {
            state = States.SEARCH;
            name = "";
            this.trigger = trigger == Tables.Trigger.RS;
            this.bazis = bazis;

            table.changeTrigger(trigger);
            minSystem.bazis = bazis;

            evemaT = null;
            switch (task)
            {
                case "Суммирующий":
                    taskInfo = new TaskInfo(k1, TaskType.COUNTER_ADD);
                    name += @"Сум\" + (this.trigger ? "RS" : "T") + " " + (bazis ? "ИЛИ-НЕ" : "И-НЕ") + @"\" + k1;
                    table.Build(false, k1 - 1);
                    break;
                case "Вычитающий":
                    taskInfo = new TaskInfo(k1, TaskType.COUNTER_SUB);
                    name += @"Выч\" + (this.trigger ? "RS" : "T") + " " + (bazis ? "ИЛИ-НЕ" : "И-НЕ") + @"\" + k1;
                    table.Build(true, k1 - 1);
                    break;
                case "Реверсивный":
                    taskInfo = new TaskInfo(k1, k2);
                    name += @"Рев\" + (this.trigger ? "RS" : "T") + " " + (bazis ? "ИЛИ-НЕ" : "И-НЕ") + @"\" + k1 + " " + k2;
                    table.Build(k1 - 1, k2 - 1);
                    break;
                default:
                    break;
            }
            for (int i = 0; i < picturesEtmp.Length; i++)
            {
                picturesEtmp[i].Image = null;
            }
            minSystem.SearchMinSolution(table.Generate(), deep, diversity);
        }

        private void BuildSwitchFunc(string task1, string task2, Tables.Trigger trigger, bool bazis, int deep, float diversity, out string name)
        {
            state = States.SEARCH;
            name = "";
            this.trigger = trigger == Tables.Trigger.RS;
            this.bazis = bazis;

            table.changeTrigger(trigger);
            minSystem.bazis = bazis;

            evemaB = null;
            table.Build(task1, task2);
            taskInfo = new TaskInfo(table.ones, table.zeros);
            for (int i = 0; i < picturesEtmp.Length; i++)
            {
                picturesEtmp[i].Image = null;
            }
            name += "(" + task1 + ")v(" + task2 + @")\" + (bazis ? "ИЛИ-НЕ" : "И-НЕ");
            minSystem.SearchMinSolution(table.Generate(), deep, diversity);
        }

        private void takeSolution(MinimizeSystem ms, ResolveArgs args)
        {
            solution = args.solution;
            Element.Weight minWeight = args.minWeight;
            weight.Text = String.Format("Вес конфигурации: {0} Песков, {1} Эвем, {2} Кушек", minWeight.Pesok, minWeight.Evema, args.Q);
            buildImageResources(Solution);
        }
        private void buildImageResources(List<Element>[] solution)
        {
            state = States.DRAWNING;
            draw?.Abort();
            draw = new Thread(() =>
            {
                try
                {
                    text.Text = "Подготовка изображений";
                    progress.Value = 0;
                    PictureBox[] picsS = new PictureBox[solution.Length];
                    PictureBox[] picsF = new PictureBox[solution.Length];
                    for (int i = 0; i < containers.Length; i++)
                    {
                        containers[i].Visible = false;
                    }
                    for (int i = 0; i < picsS.Length; i++)
                    {
                        containers[i].Visible = true;
                        picsS[i] = picturesS[i];
                        picsF[i] = picturesF[i];
                    }
                    text.Text = "Рисование склеек";
                    progress.Value = 10;
                    images.DrawSplices(table.Mrx, solution, taskInfo.Type == TaskType.SWITCH_FUNC ? 1 : 0, minSystem.bazis, picsS);
                    text.Text = "Рисование формул";
                    progress.Value = 60;
                    images.DrawFormulae(solution, taskInfo.Type == TaskType.SWITCH_FUNC ? 1 : 0, minSystem.bazis, trigger, picsF);
                    text.Text = "Рисование схемы";
                    progress.Value = 70;
                    picturesE.Clear();
                    picturesE.Add(circuit);
                    if (taskInfo.Type != TaskType.SWITCH_FUNC)
                    {
                        evemaT = new EvemaWithTriggers(Solution.Length / (trigger ? 2 : 1));
                        evemaT.BuildCircuit(solution, bazis, trigger, taskInfo.Type);
                        progress.Value = 80;
                        images.DrawCircuit(evemaT, bazis, trigger, taskInfo.Type, circuit);
                    }
                    else
                    {
                        evemaB = new EvemaBased(4);
                        evemaB.BuildCircuit(solution, bazis, trigger, taskInfo.Type);
                        progress.Value = 80;
                        images.DrawCircuit(evemaB, bazis, trigger, taskInfo.Type, circuit);
                    }
                    progress.Value = 90;
                    background.Invalidate();
                    progress.Value = 99;
                    state = States.FREE;
                    completion();
                    text.Text = "Готово";
                    progress.Value = 100;
                    state = States.FREE;
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message + "\nПроблема произошла во время рисования" + e.StackTrace);
                }
            });
            draw.Start();
        }

        public void StartUp()
        {
            try
            {
                if (picturesE.Count < 2 && state == States.FREE)
                {
                    state = States.EVEMA;
                    text.Text = "Запуск эвемы";
                    progress.Value = 0;
                    switch (taskInfo.Type)
                    {
                        case TaskType.AUTOMAT:
                        case TaskType.COUNTER_ADD:
                        case TaskType.COUNTER_SUB:
                            StartUpStandartWithTriggers();
                            break;
                        case TaskType.COUNTER_REV:
                        case TaskType.COUNTER_REV_INVERSE:
                            StartUpREV();
                            break;
                        case TaskType.SWITCH_FUNC:
                            StartUpStandartBase();
                            break;
                        default:
                            break;
                    }
                    progress.Value = 99;
                    state = States.FREE;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message + "\n\nПроблема произошла во время работы эвемы" + e.StackTrace);
            }
        }

        private void StartUpStandartBase()
        {
            EvemaBased evema = evemaB;
            background.AutoScroll = true;
            int i = 0;
            foreach (int set in taskInfo.Ones)
            {
                evema.Start(set);
                while (!evema.isComplete) ;
                images.DrawCircuitTick(evema.Layers, picturesEtmp[i], taskInfo.Type);
                picturesE.Add(picturesEtmp[i]);
                i++;
            }
            foreach (int set in taskInfo.Zeros)
            {
                evema.Start(set);
                while (!evema.isComplete) ;
                images.DrawCircuitTick(evema.Layers, picturesEtmp[i], taskInfo.Type);
                picturesE.Add(picturesEtmp[i]);
                i++;
            }
        }

        private void StartUpREV()
        {
            EvemaWithTriggers evema = evemaT;
            evema.Start(table.Start);
            images.DrawCircuitTick(evema.Layers, picturesEtmp[0], taskInfo.Type);
            background.AutoScroll = true;
            int i = TicTacEvema(evema, 1, taskInfo.Type);

            progress.Value = 55;
            EvemaWithTriggers secEvtmp = new EvemaWithTriggers(Solution.Length / (trigger ? 2 : 1));
            secEvtmp.BuildCircuit(Solution, bazis, trigger, taskInfo.Type);
            images.DrawCircuit(secEvtmp, bazis, trigger, TaskType.COUNTER_REV_INVERSE, new PictureBox());
            secEvtmp.SwitchAlpha.Signal = true;
            secEvtmp.Start(table.Start);
            images.DrawCircuitTick(secEvtmp.Layers, picturesEtmp[i], TaskType.COUNTER_REV_INVERSE);
            progress.Value = 70;
            TicTacEvema(secEvtmp, i + 1, TaskType.COUNTER_REV_INVERSE);
        }

        private void StartUpStandartWithTriggers()
        {
            EvemaWithTriggers evema = evemaT;
            evema.Start(table.Start);
            images.DrawCircuitTick(evema.Layers, picturesEtmp[0], taskInfo.Type);
            progress.Value = 70;
            background.AutoScroll = true;
            TicTacEvema(evema, 1, taskInfo.Type);
        }

        private int TicTacEvema(EvemaWithTriggers evema, int i, TaskType taskType)
        {
            while (!evema.isComplete)
            {
                if (evema.isReady)
                {
                    evema.Tick();
                    images.DrawCircuitTick(evema.Layers, picturesEtmp[i], taskType);
                    picturesE.Add(picturesEtmp[i]);
                    i++;
                }
            }
            return i;
        }

        public void Save(string path, bool flag)
        {
            try
            {
                if (state == States.FREE && solution != null && draw != null && (draw.ThreadState == System.Threading.ThreadState.Stopped) || flag)
                {
                    state = States.SAVE;
                    text.Text = "Сохранение";
                    progress.Value = 0;
                    path += @"\imgs";
                    Directory.CreateDirectory(path);
                    for (int i = 0; i < table.Mrx.Length; i++)
                    {
                        picturesS[i].Image.Save(path + @"\_S" + i + ".png", ImageFormat.Png);
                        picturesF[i].Image.Save(path + @"\_F" + i + ".png", ImageFormat.Png);
                    }
                    progress.Value = 40;
                    for (int i = 0; i < picturesE.Count; i++)
                    {
                        picturesE[i].Image.Save(path + @"\_E" + i + ".png", ImageFormat.Png);
                    }
                    progress.Value = 99;
                    state = States.FREE;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message + "\n\nПроблема произошла во время сохранения" + e.StackTrace);
            }
        }

        public void CreateReport(string path, bool flag)
        {
            try
            {
                if (state == States.FREE)
                {
                    state = States.REPORT;
                    if (!flag)
                        Save(path, false);
                    text.Text = "Создание отчета";
                    progress.Value = 0;
                    WordProcess wp = new WordProcess();
                    progress.Value = 15;
                    wp.CreateDoc(traceSolution(), path);
                    progress.Value = 95;
                    state = States.FREE;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message + "\n\nПроблема произошла во время создания отчета" + e.StackTrace);
            }
        }

        public void Abort()
        {
            text.Text = "Остановлено";
            progress.Value = 0;
            minSystem?.Abort();
            draw?.Abort();
            state = States.FREE;
        }

        public int ConfigNum
        {
            get => configNum;
            set
            {
                try
                {
                    if (state == States.FREE)
                    {
                        configNum = value;
                        if (configNum < 0)
                        {
                            configNum = 0;
                            configView.Text = "1";
                        }
                        if (solution != null)
                        {
                            if (configNum >= solution.Length)
                            {
                                configNum = solution.Length - 1;
                                configView.Text = solution.Length.ToString();
                            }
                            buildImageResources(Solution);
                        }
                        else
                        {
                            configNum = 0;
                            configView.Text = "1";
                        }
                    }
                }
                catch { }
            }
        }

        private SolutionInfo traceSolution()
        {
            Image[] splices = new Image[picturesS.Length];
            Image[] formulae = new Image[picturesF.Length];
            Image[] evema = new Image[picturesE.Count];
            string[,] mainTable;

            switch (taskInfo.Type)
            {
                case TaskType.AUTOMAT:
                    if (trigger)
                        mainTable = getMainTableRS();
                    else
                        mainTable = getMainTableT();
                    break;
                case TaskType.COUNTER_ADD:
                case TaskType.COUNTER_SUB:
                    mainTable = getMainTableT();
                    break;
                case TaskType.COUNTER_REV:
                    mainTable = getMainTableREV();
                    break;
                case TaskType.SWITCH_FUNC:
                    mainTable = getMainTableSF();
                    break;
                default:
                    mainTable = null;
                    break;
            }
            for (int i = 0; i < picturesS.Length; i++)
            {
                splices[i] = picturesS[i].Image;
                formulae[i] = picturesF[i].Image;
            }
            for (int i = 0; i < picturesE.Count; i++)
            {
                evema[i] = picturesE[i].Image;
            }
            return new SolutionInfo(taskInfo, bazis, trigger ? "RS" : "T", mainTable, table.Mrx, table.numsS, splices, formulae, evema);
        }

        private string[,] getMainTableSF()
        {
            string[,] mainTable = new string[16, 1];
            for (int i = 0; i < 16; i++)
            {
                mainTable[i, 0] = dataGridView.Rows[i].Cells[9].Value.ToString();
            }
            return mainTable;
        }


        private string[,] getMainTableREV()
        {
            string[,] mainTable = new string[16, 6];
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    mainTable[i, j] = dataGridView.Rows[i].Cells[j + 6].Value.ToString();
                }
                for (int j = 3; j < 6; j++)
                {
                    mainTable[i, j] = dataGridView.Rows[i].Cells[j + 7].Value.ToString();
                }
            }
            return mainTable;
        }

        private string[,] getMainTableT()
        {
            string[,] mainTable = new string[16, 8];
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    mainTable[i, j] = dataGridView.Rows[i].Cells[j + 5].Value.ToString();
                }
            }
            return mainTable;
        }

        private string[,] getMainTableRS()
        {
            string[,] mainTable = new string[16, 9];
            for (int i = 0; i < 16; i++)
            {
                string tmp = "";
                for (int j = 5; j < 9; j++)
                {
                    tmp += dataGridView.Rows[i].Cells[j].Value.ToString() + (j == 8 ? "" : " ");
                }
                mainTable[i, 0] = tmp;
                for (int j = 1; j < 9; j++)
                {
                    mainTable[i, j] = dataGridView.Rows[i].Cells[8 + j].Value.ToString();
                }
            }
            return mainTable;
        }

        public List<Element>[] Solution => solution[configNum];
    }
}
