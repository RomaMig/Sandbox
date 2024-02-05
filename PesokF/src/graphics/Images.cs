using PesokF.src.s;
using PesokF.src.solution_system.evema;
using PesokF.src.w;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static PesokF.EvemaWithTriggers;
using static PesokF.Interpretor;
using static PesokF.MinimizeSystem;

namespace PesokF
{
    class Images
    {
        public static readonly Color[] colors = new Color[] { Color.Black, Color.Red, Color.DarkBlue, Color.DarkGreen, Color.Orange, Color.Aqua, Color.Yellow, Color.Purple, Color.Brown, Color.HotPink, Color.LightGreen };
        //public static readonly string[] PATHS = { @"C:\Users\pc\source\repos\PesokF\PesokF\data\img\veiche\", @"C:\Users\pc\source\repos\PesokF\PesokF\data\img\formulae\", @"C:\Users\pc\source\repos\PesokF\PesokF\data\img\evema\" };
        public static readonly string[] PATHS = { Program.PathExe() + @"\data\img\veiche\", Program.PathExe() + @"\data\img\formulae\", Program.PathExe() + @"\data\img\evema\" };
        public static readonly string[] SYNBOLS_ABC = { "a", "b", "c", "d" };
        public static readonly string[] SYMBOLS_Q = { "q1", "q2", "q3", "q4" };
        public static readonly string[] SYMBOLS_RES = { "t1", "t2", "t3", "t4", "r1", "s1", "r2", "s2", "r3", "s3", "r4", "s4" };
        public static readonly string[] SYMBOLS_RES_F = { "f" };
        public static readonly string[] SYMBOLS_DIGITS = { "0", "1" };
        public static readonly string[] SIGNS = { "=", "v", "&" };
        public static readonly string[] INVERSES = { "1!", "2!", "3!" };
        static readonly string[] VEICHE = { "-", "0", "1", "Veiche", "VeicheABC" };
        static readonly string[] SPLICES = { "O", "THL", "THR", "TVU", "TVB", "FSLU", "FSRU", "FSLB", "FSRB", "FH", "FV", "EHU", "EHB", "EVL", "EVR", "S" };
        static readonly string[] EVEMA = { "Circuit", "&", "1", "in", "out", "s0", "s1", "ls0", "ls1" };
        static readonly string[] POSTFIX = { "REV0", "REV1", "empty" };
        static readonly int OFFSETX = 57;
        static readonly int OFFSETY = 42;
        static readonly int WIDTH = 175;
        static readonly int HEIGHT = 145;
        static readonly int E_L0_OFFSETX = 1023;
        static readonly int E_L0_OFFSETY = 67;
        static readonly int E_L2_OFFSETX = 280;
        static readonly int E_L2_OFFSETY = 50;
        static readonly int E_L3_OFFSETX = 900;
        static readonly int E_L3_OFFSETY = 57;
        static readonly int E_I_OFFSETX = 1335;
        static readonly int E_I_OFFSETY = 180;
        static readonly int E_O_L0_OFFSETX = 5;
        static readonly int E_O_L0_OFFSETY = 20;
        static readonly int E_O_L1_OFFSETX = 50;
        static readonly int E_O_L1_OFFSETY = 10;
        static readonly int E_O_L2_OFFSETX = 280;
        static readonly int E_O_L2_OFFSETY = 190;
        static readonly int E_O_L3_OFFSETX = 500;
        static readonly int E_O_L3_OFFSETY = 200;
        static readonly int ROOF = 12;
        static readonly int ROOF_O = 195;
        static readonly int[][] inLocations;
        static readonly int[] inLocationsT;
        static readonly int[] inLocationsArg;
        static readonly int[] locationsInvArgY;
        static readonly int[] outLocationsT;
        List<int> skipQPos;
        Bitmap circuit;
        Bitmap cloneCircuit;
        Bitmap[] nums;
        Bitmap[] data;
        Bitmap input;
        Bitmap output;
        Bitmap s0, s1;
        Bitmap logicSignal0, logicSignal1;

        public Images()
        {
            Bitmap d1 = (Bitmap)Bitmap.FromFile(PATHS[0] + VEICHE[0] + ".png");
            Bitmap d2 = (Bitmap)Bitmap.FromFile(PATHS[0] + VEICHE[1] + ".png");
            Bitmap d3 = (Bitmap)Bitmap.FromFile(PATHS[0] + VEICHE[2] + ".png");
            data = new Bitmap[] { d1, d2, d3 };
            circuit = (Bitmap)Bitmap.FromFile(PATHS[2] + EVEMA[0] + ".png");
            input = (Bitmap)Bitmap.FromFile(PATHS[2] + EVEMA[3] + ".png");
            output = (Bitmap)Bitmap.FromFile(PATHS[2] + EVEMA[4] + ".png");
            s0 = (Bitmap)Bitmap.FromFile(PATHS[2] + EVEMA[5] + ".png");
            s1 = (Bitmap)Bitmap.FromFile(PATHS[2] + EVEMA[6] + ".png");
            logicSignal0 = (Bitmap)Bitmap.FromFile(PATHS[2] + EVEMA[7] + ".png");
            logicSignal1 = (Bitmap)Bitmap.FromFile(PATHS[2] + EVEMA[8] + ".png");
            nums = new Bitmap[10];
            for (int i = 0; i < 10; i++)
            {
                nums[i] = (Bitmap)Bitmap.FromFile(PATHS[2] + @"nums\" + i + ".png");
            }
        }

        static Images()
        {
            inLocations = new int[8][];
            inLocations[0] = new int[] { 5, 25 };
            inLocations[1] = new int[] { 5, 15, 25 };
            inLocations[2] = new int[] { 5, 10, 20, 25 };
            inLocations[3] = new int[] { 5, 10, 15, 20, 25 };
            inLocations[4] = new int[] { 5, 10, 15, 25, 30, 35 };
            inLocations[5] = new int[] { 5, 10, 15, 20, 25, 30, 35 };
            inLocations[6] = new int[] { 5, 10, 15, 20, 30, 35, 40, 45 };
            inLocations[7] = new int[] { 5, 10, 15, 20, 25, 30, 35, 40, 45 };
            inLocationsT = new int[2] { 5, 35 };
            inLocationsArg = new int[8] { 83, 103, 143, 163, 198, 213, 238, 258 };
            locationsInvArgY = new int[4] { 35, 75, 115, 155 };
            outLocationsT = new int[8] { 68, 88, 143, 163, 218, 238, 293, 313 };
        }

        public void DrawCircuit(EvemaBased evema, bool bazis, bool trigger, TaskType taskType, PictureBox pb)
        {
            List<LogicalElement>[] Layers = evema.Layers;
            circuit = (Bitmap)Bitmap.FromFile(PATHS[2] + EVEMA[0] + "_" + POSTFIX[2] + ".png");
            Bitmap baz = (Bitmap)Bitmap.FromFile(PATHS[2] + EVEMA[bazis ? 2 : 1] + ".png");
            skipQPos = new List<int>();

            PrepareAndDraw(E_O_L0_OFFSETX, E_O_L0_OFFSETY, circuit, baz, Layers[0], false, 40);
            PrepareAndDraw(E_O_L1_OFFSETX, E_O_L1_OFFSETY, circuit, baz, Layers[1], true, 30);
            PrepareAndDraw(E_O_L2_OFFSETX, E_O_L2_OFFSETY, circuit, baz, Layers[2], Layers[3], 0, 1);
            PrepareAndDraw(E_O_L3_OFFSETX, E_O_L3_OFFSETY, circuit, baz, Layers[3], false, 30);

            DrawArgumentsTrace(circuit, Layers[0], Layers[1]);
            DrawLinksFromInverseToArgs(circuit, Layers[0], Layers[1]);
            DrawLinksToArgs(circuit, Layers[2], false);
            DrawLinksBetweenLayersOnTop(circuit, Layers[3], false, ROOF_O);
            cloneCircuit = (Bitmap)circuit.Clone();
            pb.Image = circuit;
        }

        public void DrawCircuit(EvemaWithTriggers evema, bool bazis, bool trigger, TaskType taskType, PictureBox pb)
        {
            List<LogicalElement>[] Layers = evema.Layers;
            circuit = (Bitmap)Bitmap.FromFile(
                PATHS[2] +
                EVEMA[0] +
                (taskType == TaskType.COUNTER_REV || taskType == TaskType.COUNTER_REV_INVERSE ?
                "_" + POSTFIX[1 - (TaskType.COUNTER_REV_INVERSE - taskType)] :
                "") +
                ".png");
            Bitmap baz = (Bitmap)Bitmap.FromFile(PATHS[2] + EVEMA[bazis ? 2 : 1] + ".png");
            skipQPos = new List<int>();

            PrepareAndDraw(E_L2_OFFSETX, E_L2_OFFSETY, circuit, baz, Layers[2], Layers[3], 1);
            PrepareAndDraw(E_L3_OFFSETX, E_L3_OFFSETY, circuit, baz, Layers[3], false, 70);

            DrawLinksToArgs(circuit, Layers[2], true);
            DrawLinksToTriggers(circuit, Layers[3], trigger);
            DrawLinksBetweenLayersOnTop(circuit, Layers[3], true, ROOF);
            cloneCircuit = (Bitmap)circuit.Clone();
            pb.Image = circuit;
        }

        private void DrawIndicator(Bitmap circuit, int num)
        {
            DrawDigitOnIndicator(circuit, num / 10, 0);
            DrawDigitOnIndicator(circuit, num % 10, 13);
        }

        private void DrawDigitOnIndicator(Bitmap circuit, int digit, int offset)
        {
            for (int x = E_I_OFFSETX + offset; x < E_I_OFFSETX + offset + 13; x++)
            {
                for (int y = E_I_OFFSETY; y < E_I_OFFSETY + 14; y++)
                {
                    circuit.SetPixel(x, y, nums[digit].GetPixel(x - E_I_OFFSETX - offset, y - E_I_OFFSETY));
                }
            }
        }

        private Bitmap DrawSignals(List<LogicalElement>[] layers)
        {
            Bitmap circuit_with_signals = (Bitmap)cloneCircuit.Clone();
            PrepareAndDraw(E_O_L0_OFFSETX, E_O_L0_OFFSETY, circuit_with_signals, null, layers[0], false, 40);

            for (int i = 1; i < 4; i++)
            {
                foreach (LogicalElement le in layers[i])
                {
                    if (!le.isLogicSignal && !le.isLine)
                        DrawSignalsOnCircuit(circuit_with_signals, le.Signal ? s1 : s0, le.outLocation.X - 1, le.outLocation.Y - 9);
                }
            }
            return circuit_with_signals;
        }

        private Bitmap DrawSignalsWithTriggers(List<LogicalElement>[] layers)
        {
            Bitmap circuit_with_signals = (Bitmap)cloneCircuit.Clone();

            for (int i = 2; i < 4; i++)
            {
                foreach (LogicalElement le in layers[i])
                {
                    if (!le.isLogicSignal && !le.isLine)
                        DrawSignalsOnCircuit(circuit_with_signals, le.Signal ? s1 : s0, le.outLocation.X - 1, le.outLocation.Y - 9);
                }
            }
            int size = layers[0].Count > 4 ? 8 : layers[0].Count * 2;
            for (int i = 0; i < size; i++)
            {
                DrawSignalsOnCircuit(circuit_with_signals, layers[1][8 - size + i].Signal ? s1 : s0, E_L0_OFFSETX + 30, outLocationsT[i]);   //Аккуратно, костыль
            }
            return circuit_with_signals;
        }

        private void DrawSignalsOnCircuit(Bitmap circuit, Bitmap signal, int x, int y)
        {
            for (int dx = x; dx < x + signal.Width; dx++)
            {
                for (int dy = y; dy < y + signal.Height; dy++)
                {
                    circuit.SetPixel(dx, dy, signal.GetPixel(dx - x, dy - y));
                }
            }
        }

        private void DrawArgumentsTrace(Bitmap circuit, params List<LogicalElement>[] args)
        {
            foreach (List<LogicalElement> arg in args)
            {
                foreach (LogicalElement le in arg)
                {
                    DrawArgumentTrace(circuit, le.outLocation);
                }
            }
        }

        private void DrawArgumentTrace(Bitmap circuit, params Point[] inputs)
        {
            foreach (Point input in inputs)
            {
                Point corner = new Point(inLocationsArg[(input.Y - 30) / 20], input.Y);
                Point contact = new Point(corner.X, 360);
                DrawLine(circuit, input, corner);
                DrawLine(circuit, corner, contact);
                DrawPunctContact(circuit, contact);
            }
        }

        private void DrawLinksBetweenLayersOnTop(Bitmap circuit, List<LogicalElement> outL, bool isWithTriggers, int roof)
        {
            List<long> hashes = new List<long>();
            List<int> offsets = new List<int>();
            int offset = 10;
            int skipQ = 0;
            foreach (LogicalElement ole in outL)
            {
                for (int i = 0; i < ole.Parents.Count; i++)
                {
                    LogicalElement pole = ole.Parents[i];
                    //MessageBox.Show(pole.HashCode.ToString()+"\n"+ pole.HashCodeP.ToString());
                    if (!hashes.Contains(pole.HashCodeP))
                    {
                        //новая связь
                        hashes.Add(pole.HashCodeP);
                        offsets.Add(roof + offset);
                        Point np1 = new Point(pole.outLocation.X + offset, pole.outLocation.Y);
                        if (pole.HashCode % 10 == 1)
                        {
                            np1 = new Point(E_L2_OFFSETX + 20 + offset, skipQPos[skipQ++]);
                            DrawLinkToLogicArg(circuit, pole, np1, isWithTriggers);
                        }
                        else
                        {
                            DrawLine(circuit, pole.outLocation, np1);
                        }
                        Point np2 = new Point(np1.X, roof + offset);
                        Point np3 = new Point(ole.inLocations[i].X - offset, np2.Y);
                        Point np4 = new Point(np3.X, ole.inLocations[i].Y);
                        DrawLine(circuit, np1, np2);
                        DrawLine(circuit, np2, np3);
                        DrawLine(circuit, np3, np4);
                        DrawLine(circuit, np4, ole.inLocations[i]);
                    }
                    else
                    {
                        //подсоединение к уже существующей связи
                        Point np1 = new Point(ole.inLocations[i].X - offset, ole.inLocations[i].Y);
                        Point np2 = new Point(np1.X, offsets[hashes.IndexOf(pole.HashCodeP)]);
                        DrawLine(circuit, ole.inLocations[i], np1);
                        DrawLine(circuit, np1, np2);
                        DrawContact(circuit, np2);
                    }
                    offset += 5;
                }
            }
        }

        private void DrawLinksFromInverseToArgs(Bitmap circuit, List<LogicalElement> args, List<LogicalElement> inverses)
        {
            foreach (LogicalElement le in inverses)
            {
                Point corner = new Point(le.inLocations[0].X - 10, le.inLocations[0].Y);
                Point node = new Point(corner.X, args[le.HashCode / 10].outLocation.Y);
                DrawLine(circuit, le.inLocations[0], corner);
                DrawLine(circuit, corner, node);
                DrawContact(circuit, node);
            }
        }

        private void DrawLinksToArgs(Bitmap circuit, List<LogicalElement> outL, bool isWithTriggers)
        {
            foreach (LogicalElement le in outL)
            {
                for (int i = 0; i < le.Parents.Count; i++)
                {
                    DrawLinkToLogicArg(circuit, le.Parents[i], le.inLocations[i], isWithTriggers);
                }
            }
        }

        private void DrawLinkToLogicArg(Bitmap circuit, LogicalElement Arg, Point inle, bool isWithTriggers)
        {
            Point contact = new Point(inLocationsArg[isWithTriggers ? Arg.HashCode / 10 : (Arg.HashCode / 10) * 2 + Arg.HashCode % 10], inle.Y);
            DrawLine(circuit, contact, inle);
            DrawContact(circuit, contact);
        }

        private void DrawLinkToNo(Bitmap circuit, Point inle)
        {
            Point contact = new Point(inle.X - 10, inle.Y - 20);
            Point corner = new Point(contact.X, inle.Y);
            DrawLine(circuit, inle, corner);
            DrawLine(circuit, corner, contact);
            DrawContact(circuit, contact);
        }

        private void DrawLinksToTriggers(Bitmap circuit, List<LogicalElement> inL, bool trigger)
        {
            int[] offsetX = new int[2] { 20, 30 };
            for (int i = 0; i < inL.Count; i++)
            {
                Point nPos = new Point(inL[i].outLocation.X + offsetX[i % 2], inL[i].outLocation.Y);
                Point nPos2 = trigger ?
                    new Point(nPos.X, E_L0_OFFSETY + (i / 2) * 75 + inLocationsT[1 - i % 2]) :
                    new Point(nPos.X, E_L0_OFFSETY + i * 75 + inLocationsT[0]);
                DrawLine(circuit, inL[i].outLocation, nPos);
                DrawLine(circuit, nPos, nPos2);
                DrawLine(circuit, nPos2, new Point(E_L0_OFFSETX, nPos2.Y));
                if (!trigger)
                {
                    Point nPos3 = new Point(nPos2.X, nPos2.Y - inLocationsT[0] + inLocationsT[1]);
                    Point contact = new Point(nPos3.X, nPos2.Y);
                    DrawLine(circuit, nPos3, new Point(E_L0_OFFSETX, nPos3.Y));
                    DrawLine(circuit, nPos3, contact);
                    DrawContact(circuit, contact);
                }
            }
        }

        private void DrawContact(Bitmap circuit, Point contact)
        {
            for (int x = contact.X - 2; x < contact.X + 3; x++)
            {
                for (int y = contact.Y - 2; y < contact.Y + 3; y++)
                {
                    Color col = input.GetPixel(x - contact.X + 2, y - contact.Y + 2);
                    if (col.R < 20 && col.G < 20 && col.B < 20)
                        circuit.SetPixel(x, y, col);
                }
            }
        }

        private void DrawPunctContact(Bitmap circuit, Point contact)
        {
            for (int i = 0; i < output.Height; i++)
            {
                for (int j = 0; j < output.Width; j++)
                {
                    circuit.SetPixel(contact.X - 2 + j, contact.Y - 2 + i, output.GetPixel(j, i));
                }
            }
        }

        private void DrawLine(Bitmap circuit, Point s, Point f)
        {
            int X = Math.Min(s.X, f.X);
            int W = Math.Abs(f.X - s.X) + 1;
            int Y = Math.Min(s.Y, f.Y);
            int H = Math.Abs(f.Y - s.Y) + 1;
            for (int x = X; x < X + W; x++)
            {
                for (int y = Y; y < Y + H; y++)
                {
                    circuit.SetPixel(x, y, Color.Black);
                }
            }
        }

        private void PrepareAndDraw(int startX, int startY, Bitmap circuit, Bitmap baz, List<LogicalElement> layer, bool isOrderImportant, int lse_gape)
        {
            for (int i = 0; i < layer.Count; i++)
            {
                if (layer[i].isLogicSignal)
                {
                    prepareLogicSignalElement(ref startX, ref startY, circuit, layer, i, lse_gape);
                }
                else
                {
                    if (layer[i].isLine)
                    {
                        prepareLineElement(ref startX, ref startY, layer, i);
                    }
                    else
                    {
                        int h = 30;
                        if (layer[i].Parents.Count > 5)
                            h += 10;
                        if (layer[i].Parents.Count > 7)
                            h += 10;

                        if (isOrderImportant) startY = locationsInvArgY[layer[i].HashCode / 10];
                        int ox = startX + 17;
                        int oy = startY + h / 2 - 2;
                        layer[i].outLocation = new Point(ox + 4, oy + 2);
                        for (int j = 0; j < layer[i].Parents.Count; j++)
                        {
                            //ОСТОРОЖНО! ЛЮТЫЙ КОСТЫЛЬ
                            if (layer[i].Parents.Count == 1) //если был только один родитель
                            {
                                layer[i].inLocations.Add(new Point(startX, startY + inLocations[0][j]));   //инициализируем верхнее подключение
                                DrawLinkToNo(circuit, new Point(startX, startY + inLocations[0][j + 1])); //рисуем линк из нижнего подключения к верхнему
                            }
                            else
                            {
                                layer[i].inLocations.Add(new Point(startX, startY + inLocations[layer[i].Parents.Count - 2][j]));
                            }
                        }
                        DrawElementOnCircuit(circuit, baz, startX, startY, h, ox, oy);

                        startY += h + 5 + 10 * (layer.Count > 4 ? (i % 2) : 3);
                    }
                }
            }
        }

        private void PrepareAndDraw(int startX, int startY, Bitmap circuit, Bitmap baz, List<LogicalElement> layer, List<LogicalElement> children, params int[] args)
        {
            for (int i = 0; i < layer.Count; i++)
            {
                if (layer[i].isLogicSignal)
                {
                    prepareLogicSignalElement(ref startX, ref startY, circuit, layer, i, 70);
                }
                else
                {
                    if (layer[i].isLine)
                    {
                        prepareLineElement(ref startX, ref startY, layer, i);
                    }
                    else
                    {
                        prepareLogicElement(ref startX, ref startY, circuit, baz, layer, children, i, args);
                    }
                }
            }
            CheckOnQ(layer.Count == 0 ? null : layer.Last(), null, children, ref startY, args);
        }

        private void prepareLogicSignalElement(ref int startX, ref int startY, Bitmap circuit, List<LogicalElement> layer, int i, int gape)
        {
            int ox = startX + 26;
            int oy = startY + 10;
            layer[i].outLocation = new Point(ox, oy);

            DrawLogicSignalOnCircuit(circuit, layer[i].Signal, startX, startY);
            startY += gape;
        }


        private void prepareLineElement(ref int startX, ref int startY, List<LogicalElement> layer, int i)
        {
            layer[i].outLocation = new Point(startX, startY);
            layer[i].inLocations.Add(new Point(startX, startY));
            startY += 80;
        }

        private void prepareLogicElement(ref int startX, ref int startY, Bitmap circuit, Bitmap baz, List<LogicalElement> layer, List<LogicalElement> children, int i, params int[]args)
        {
            CheckOnQ(i == 0 ? null : layer[i - 1], layer[i], children, ref startY, args);
            int h = 30;
            if (layer[i].Parents.Count > 5)
                h += 10;
            if (layer[i].Parents.Count > 7)
                h += 10;

            int ox = startX + 17;
            int oy = startY + h / 2 - 2;
            layer[i].outLocation = new Point(ox + 4, oy + 2);
            for (int j = 0; j < layer[i].Parents.Count; j++)
            {
                //ОСТОРОЖНО! ЛЮТЫЙ КОСТЫЛЬ
                if (layer[i].Parents.Count == 1) //если был только один родитель
                {
                    layer[i].inLocations.Add(new Point(startX, startY + inLocations[0][j]));
                    DrawLinkToNo(circuit, new Point(startX, startY + inLocations[0][j + 1])); //рисуем линк из нижнего подключения к верхнему
                }
                else
                {
                    layer[i].inLocations.Add(new Point(startX, startY + inLocations[layer[i].Parents.Count - 2][j]));
                }
            }

            DrawElementOnCircuit(circuit, baz, startX, startY, h, ox, oy);
            startY += h + 5;
        }

        private void CheckOnQ(LogicalElement after, LogicalElement before, List<LogicalElement> children, ref int y, params int[] args)
        {
            List<LogicalElement> unicQ = new List<LogicalElement>();
            bool arrive = false;
            foreach (LogicalElement le in children)
            {
                for (int i = 0; i < le.Parents.Count; i++)
                {
                    if (after != null)
                    {
                        arrive |= le.Parents[i].HashCode == after.HashCode;
                        if (!arrive && le.Parents[i].HashCode % 10 == 1 && !unicQ.Contains(le.Parents[i]))
                            unicQ.Add(le.Parents[i]);
                        if (!arrive) continue;
                    }
                    if (before == null || before.HashCode != le.Parents[i].HashCode)
                    {
                        if (args.Contains(le.Parents[i].HashCode % 10) && !unicQ.Contains(le.Parents[i]))
                        {
                            unicQ.Add(le.Parents[i]);
                            skipQPos.Add(y + 5);
                            y += 10;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }

        public void DrawElementOnCircuit(Bitmap circuit, Bitmap baz, int x, int y, int h, int ox, int oy)
        {
            for (int i = 0; i < baz.Height; i++)
            {
                for (int j = 0; j < baz.Width; j++)
                {
                    Color col = baz.GetPixel(j, i);
                    if (col.A > 0)
                        circuit.SetPixel(x + j, y + i, col);
                }
            }
            for (int dy = y; dy < y + h; dy++)
            {
                circuit.SetPixel(x, dy, Color.Black);
                circuit.SetPixel(x + 19, dy, Color.Black);
            }
            for (int dx = x; dx < x + 20; dx++)
            {
                circuit.SetPixel(dx, y, Color.Black);
                circuit.SetPixel(dx, y + h - 1, Color.Black);
            }
            DrawPunctContact(circuit, new Point(ox + 2, oy + 2));
        }

        public void DrawLogicSignalOnCircuit(Bitmap circuit, bool signal, int x, int y)
        {
            Bitmap logicSignal = signal ? logicSignal1 : logicSignal0;
            for (int i = 0; i < logicSignal.Height; i++)
            {
                for (int j = 0; j < logicSignal.Width; j++)
                {
                    Color col = logicSignal.GetPixel(j, i);
                    if (col.A > 0)
                        circuit.SetPixel(x + j, y + i, col);
                }
            }
        }

        private void DrawNumsOnIndicator(Bitmap circuit, List<LogicalElement>[] Layers)
        {
            int num = 0;
            for (int i = 0; i < Layers[0].Count; i++)
            {
                num += (Layers[0][i].Signal ? 1 : 0) * (int)Math.Pow(2, Layers[0].Count - i - 1);
            }
            DrawIndicator(circuit, num);
        }

        public void DrawCircuitTick(List<LogicalElement>[] Layers, PictureBox pb, TaskType taskType)
        {
            Bitmap cir = null;
            switch (taskType)
            {
                case TaskType.AUTOMAT:
                case TaskType.COUNTER_ADD:
                case TaskType.COUNTER_SUB:
                    cir = DrawSignalsWithTriggers(Layers);
                    DrawNumsOnIndicator(cir, Layers);
                    break;
                case TaskType.COUNTER_REV:
                    cir = DrawSignalsWithTriggers(Layers);
                    DrawSignalsOnCircuit(cir, s1, 119, 12);
                    DrawNumsOnIndicator(cir, Layers);
                    break;
                case TaskType.COUNTER_REV_INVERSE:
                    cir = DrawSignalsWithTriggers(Layers);
                    DrawSignalsOnCircuit(cir, s0, 119, 12);
                    DrawNumsOnIndicator(cir, Layers);
                    break;
                case TaskType.SWITCH_FUNC:
                    cir = DrawSignals(Layers);
                    break;
                default:
                    break;
            }
            pb.Image = cir;
        }

        public void DrawFormulae(List<Element>[] solution, int symbolsStyle, bool bazis, bool trigger, params PictureBox[] pb)
        {
            for (int i = 0; i < solution.Length; i++)
            {
                List<SSymbol>[] formulae = Interpretor.Interpret(solution[i], symbolsStyle == 0 ? SYMBOLS_Q : SYNBOLS_ABC, bazis);
                SSymbol equal = new SSymbol((Bitmap)Bitmap.FromFile(PATHS[1] + SIGNS[0] + ".png"), Color.Black);
                SSymbol res1 = new SSymbol((Bitmap)Bitmap.FromFile(PATHS[1] + (symbolsStyle == 0 ? SYMBOLS_RES[(trigger ? 4 : 0) + i] : SYMBOLS_RES_F[(trigger ? 4 : 0) + i]) + ".png"), Color.Black);
                SSymbol res2 = new SSymbol((Bitmap)Bitmap.FromFile(PATHS[1] + (symbolsStyle == 0 ? SYMBOLS_RES[(trigger ? 4 : 0) + i] : SYMBOLS_RES_F[(trigger ? 4 : 0) + i]) + ".png"), Color.Black);
                if (formulae[1].Count != 0)
                {
                    formulae[1].Insert(0, equal);
                    formulae[1].Insert(0, res1);
                }
                if (formulae[0].Count != 0)
                {
                    formulae[0].Insert(0, equal);
                    if (bazis) res2.Inverse(0);
                    formulae[0].Insert(0, res2);
                }
                Bitmap img = new Bitmap(Math.Max(pb[i].Width, Math.Max(formulae[0].Count, formulae[1].Count) * 38), pb[i].Height);
                for (int k = 0; k < 2; k++)
                {
                    for (int x = 0; x < formulae[k].Count * 38; x++)
                    {
                        SSymbol s = formulae[k][x / 38];
                        for (int y = k * 58 + k; y < k * 59 + 58; y++)
                        {
                            Color col;
                            int dx = x % 38;
                            int dy = (y - k) % 58;
                            col = s.Symbol.GetPixel(dx, dy);
                            col = Color.FromArgb(255, Math.Max(col.R, s.Color.R), Math.Max(col.G, s.Color.G), Math.Max(col.B, s.Color.B));
                            for (int inv = 0; inv < 3; inv++)
                            {
                                if (s.Inverses[inv] != null)
                                {
                                    Color p = s.Inverses[inv].GetPixel(dx, dy);
                                    if (p.A > 0)
                                    {
                                        if (inv < 2)
                                        {
                                            float A = p.A / 255f;
                                            int R = (int)((1 - A) * 255 + s.Color.R);
                                            int G = (int)((1 - A) * 255 + s.Color.G);
                                            int B = (int)((1 - A) * 255 + s.Color.B);
                                            col = Color.FromArgb(255, R > 255 ? 255 : R, G > 255 ? 255 : G, B > 255 ? 255 : B);
                                        }
                                        else
                                        {
                                            col = p;
                                        }
                                    }
                                }
                            }
                            img.SetPixel(x, y, col);
                        }
                    }
                }
                pb[i].Image = (System.Drawing.Image)img.Clone();
            }
        }

        public void DrawSplices(int[][][] m, List<Element>[] solution, int symbolsStyle, bool bazis, params PictureBox[] pb)
        {
            int WB = data[0].Width;
            int HB = data[0].Height;

            for (int k = 0; k < pb.Length; k++)
            {
                ElementView view = new ElementView();
                if (solution[k] != null)
                    for (int ind = 0; ind < solution[k].Count; ind++)
                    {
                        view.Reflect(solution[k][ind], bazis);
                    }
                int i;
                int j;
                Bitmap v = (Bitmap)Bitmap.FromFile(PATHS[0] + VEICHE[symbolsStyle == 0 ? 3 : 4] + ".png");
                bool state = false;
                for (int y = OFFSETY; y < HEIGHT + OFFSETY; y++)
                {
                    for (int x = OFFSETX; x < WIDTH + OFFSETX; x++)
                    {
                        int nx = x - OFFSETX;
                        int ny = y - OFFSETY;
                        Color cur = v.GetPixel(x, y);
                        Color newCol = cur;
                        i = ny / HB;
                        j = nx / WB;
                        i = i > 3 ? 3 : i;
                        j = j > 3 ? 3 : j;
                        state = cur.R == 255;
                        if (state)
                        {
                            newCol = data[m[k][i][j] + 1].GetPixel(nx % WB, ny % HB);
                        }
                        foreach (KeyValuePair<Rectangle, List<KeyValuePair<Color, Bitmap>>> elwc in view.elements)
                        {
                            int bx = nx - WB * elwc.Key.X;
                            int by = ny - HB * elwc.Key.Y;
                            bx = bx < 0 ? 0 : bx;
                            by = by < 0 ? 0 : by;
                            bx = bx > 172 ? 172 : bx;
                            by = by > 143 ? 143 : by;
                            foreach (KeyValuePair<Color, Bitmap> kvp in elwc.Value)
                            {
                                float A = kvp.Value.GetPixel(bx, by).A / 255f;
                                if (A > 0)
                                {
                                    int R = (int)(kvp.Key.R + (1 - A) * newCol.R);
                                    int G = (int)(kvp.Key.G + (1 - A) * newCol.G);
                                    int B = (int)(kvp.Key.B + (1 - A) * newCol.B);
                                    newCol = Color.FromArgb(255, R > 255 ? 255 : R, G > 255 ? 255 : G, B > 255 ? 255 : B);
                                }
                            }
                        }
                        v.SetPixel(x, y, newCol);
                    }
                }
                pb[k].Image = (System.Drawing.Image)v.Clone();
            }
        }

        class ElementView
        {
            public Dictionary<Rectangle, List<KeyValuePair<Color, Bitmap>>> elements { get; }

            public ElementView()
            {
                elements = new Dictionary<Rectangle, List<KeyValuePair<Color, Bitmap>>>();
            }

            public void Reflect(Element e, bool bazis)
            {
                switch (e.type)
                {
                    case Element.Type.ONE:
                        AddToDic(elements, new Rectangle(e.j, e.i, 1, 1), new KeyValuePair<Color, Bitmap>(colors[e.Group], (Bitmap)Bitmap.FromFile(PATHS[0] + SPLICES[0] + ".png")));
                        break;
                    case Element.Type.TWO_H:
                        AddToDic(elements, new Rectangle(e.j, e.i, 1, 1), new KeyValuePair<Color, Bitmap>(colors[e.Group], (Bitmap)Bitmap.FromFile(PATHS[0] + SPLICES[1] + ".png")));
                        AddToDic(elements, new Rectangle((e.j + 1) % 4 - 1, e.i, 2, 1), new KeyValuePair<Color, Bitmap>(colors[e.Group], (Bitmap)Bitmap.FromFile(PATHS[0] + SPLICES[2] + ".png")));
                        break;
                    case Element.Type.TWO_V:
                        AddToDic(elements, new Rectangle(e.j, e.i, 1, 1), new KeyValuePair<Color, Bitmap>(colors[e.Group], (Bitmap)Bitmap.FromFile(PATHS[0] + SPLICES[3] + ".png")));
                        AddToDic(elements, new Rectangle(e.j, (e.i + 1) % 4 - 1, 1, 2), new KeyValuePair<Color, Bitmap>(colors[e.Group], (Bitmap)Bitmap.FromFile(PATHS[0] + SPLICES[4] + ".png")));
                        break;
                    case Element.Type.FOUR_S:
                        AddToDic(elements, new Rectangle(e.j, e.i, 1, 1), new KeyValuePair<Color, Bitmap>(colors[e.Group], (Bitmap)Bitmap.FromFile(PATHS[0] + SPLICES[5] + ".png")));
                        AddToDic(elements, new Rectangle((e.j + 1) % 4 - 1, e.i, 2, 1), new KeyValuePair<Color, Bitmap>(colors[e.Group], (Bitmap)Bitmap.FromFile(PATHS[0] + SPLICES[6] + ".png")));
                        AddToDic(elements, new Rectangle(e.j, (e.i + 1) % 4 - 1, 1, 2), new KeyValuePair<Color, Bitmap>(colors[e.Group], (Bitmap)Bitmap.FromFile(PATHS[0] + SPLICES[7] + ".png")));
                        AddToDic(elements, new Rectangle((e.j + 1) % 4 - 1, (e.i + 1) % 4 - 1, 2, 2), new KeyValuePair<Color, Bitmap>(colors[e.Group], (Bitmap)Bitmap.FromFile(PATHS[0] + SPLICES[8] + ".png")));
                        break;
                    case Element.Type.FOUR_H:
                        AddToDic(elements, new Rectangle(e.j, e.i, 4, 1), new KeyValuePair<Color, Bitmap>(colors[e.Group], (Bitmap)Bitmap.FromFile(PATHS[0] + SPLICES[9] + ".png")));
                        break;
                    case Element.Type.FOUR_V:
                        AddToDic(elements, new Rectangle(e.j, e.i, 1, 4), new KeyValuePair<Color, Bitmap>(colors[e.Group], (Bitmap)Bitmap.FromFile(PATHS[0] + SPLICES[10] + ".png")));
                        break;
                    case Element.Type.EIGHT_H:
                        AddToDic(elements, new Rectangle(e.j, e.i, 4, 1), new KeyValuePair<Color, Bitmap>(colors[e.Group], (Bitmap)Bitmap.FromFile(PATHS[0] + SPLICES[11] + ".png")));
                        AddToDic(elements, new Rectangle(e.j, (e.i + 1) % 4 - 1, 4, 2), new KeyValuePair<Color, Bitmap>(colors[e.Group], (Bitmap)Bitmap.FromFile(PATHS[0] + SPLICES[12] + ".png")));
                        break;
                    case Element.Type.EIGHT_V:
                        AddToDic(elements, new Rectangle(e.j, e.i, 1, 4), new KeyValuePair<Color, Bitmap>(colors[e.Group], (Bitmap)Bitmap.FromFile(PATHS[0] + SPLICES[13] + ".png")));
                        AddToDic(elements, new Rectangle((e.j + 1) % 4 - 1, e.i, 2, 4), new KeyValuePair<Color, Bitmap>(colors[e.Group], (Bitmap)Bitmap.FromFile(PATHS[0] + SPLICES[14] + ".png")));
                        break;
                    case Element.Type.SIXTEEN:
                        if (e.origin == 0 ? bazis : !bazis)
                            AddToDic(elements, new Rectangle(0, 0, 4, 4), new KeyValuePair<Color, Bitmap>(colors[e.Group], (Bitmap)Bitmap.FromFile(PATHS[0] + SPLICES[15] + ".png")));
                        break;
                    default:
                        break;
                }
            }

            private void AddToDic<TKey, TValue>(Dictionary<TKey, List<TValue>> dic, TKey key, TValue value)
            {
                List<TValue> tmp;
                if (dic.TryGetValue(key, out tmp))
                {
                    tmp.Add(value);
                }
                else
                {
                    tmp = new List<TValue>();
                    tmp.Add(value);
                    dic.Add(key, tmp);
                }
            }
        }
    }
}
