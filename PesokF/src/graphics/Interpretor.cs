using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PesokF.MinimizeSystem;

namespace PesokF
{
    static class Interpretor
    {
        public static List<SSymbol>[] Interpret(List<Element> arg, string[] symbols, bool bazis)
        {
            List<SSymbol>[] res = new List<SSymbol>[2];
            res[0] = new List<SSymbol>();
            res[1] = new List<SSymbol>();
            bool flag = false;
            foreach (Element e in arg)
            {
                if (e.type == Element.Type.SIXTEEN)
                {
                    if (!bazis)
                    {
                        SSymbol s = new SSymbol((Bitmap)Bitmap.FromFile(Images.PATHS[1] + Images.SYMBOLS_DIGITS[e.origin] + ".png"), Images.colors[0]);
                        flag = true;
                        res[0].Add(s);
                    }
                }
                else
                {
                    int n = getOrigin(e);
                    while (n > 0)
                    {
                        int x = n % 10;
                        SSymbol s = new SSymbol((Bitmap)Bitmap.FromFile(Images.PATHS[1] + symbols[x % 5 + x / 5 - 1] + ".png"), Images.colors[e.Group]);
                        if (x / 5 > 0) s.Inverse(0);
                        n /= 10;
                        res[0].Add(s);
                    }
                }
                res[0].Add(new SSymbol((Bitmap)Bitmap.FromFile(Images.PATHS[1] + Images.SIGNS[1] + ".png"), Color.Black));
            }
            res[0].Remove(res[0].Last());

            if (arg.Count == 1)
            {
                Element e = arg.First();
                if (e.type == Element.Type.EIGHT_H || e.type == Element.Type.EIGHT_V)
                {
                    if (bazis)
                    {
                        int n = 0;
                        SSymbol tmp = res[0].Last();
                        SSymbol s1 = new SSymbol(tmp.Symbol, tmp.Color);
                        SSymbol s2 = new SSymbol(tmp.Symbol, tmp.Color);
                        bool isInverse = false;

                        for (int i = 0; i < 3; i++)
                        {
                            if (tmp.Inverses[i] != null)
                            {
                                n++;
                                s1.Inverse(i);
                                s2.Inverse(i);
                            }
                            else
                            {
                                if (!isInverse)
                                {
                                    isInverse = true;
                                    n++;
                                    s1.Inverse(i);
                                    s2.Inverse(i);
                                }
                            }
                        }
                        res[1].Add(s1);
                        if (n > 1)
                        {
                            for (int i = 0, j = 0; i < 3 && j < 2; i++)
                            {
                                if (s2.Inverses[i] != null)
                                {
                                    s2.Inverses[i] = null;
                                    j++;
                                }
                            }
                            res[1].Add(new SSymbol((Bitmap)Bitmap.FromFile(Images.PATHS[1] + Images.SIGNS[0] + ".png"), Color.Black));
                            res[1].Add(s2);
                        }
                    }
                    flag = true;
                }
            }

            if (!flag)
            {
                if (bazis)
                {
                    foreach (Element e in arg)
                    {
                        if (e.type == Element.Type.SIXTEEN)
                        {
                            SSymbol s = new SSymbol((Bitmap)Bitmap.FromFile(Images.PATHS[1] + Images.SYMBOLS_DIGITS[e.origin] + ".png"), Images.colors[0]);
                            res[1].Add(s);
                        }
                        else
                        {
                            int n = getOrigin(e);
                            while (n > 0)
                            {
                                int x = n % 10;
                                SSymbol s = new SSymbol((Bitmap)Bitmap.FromFile(Images.PATHS[1] + symbols[x % 5 + x / 5 - 1] + ".png"), Images.colors[e.Group]);
                                s.Inverse(2);
                                if (e.origin > 10)
                                {
                                    s.Inverse(1);
                                    if (x / 5 == 0)
                                    {
                                        s.Inverse(0);
                                    }
                                }
                                else
                                {
                                    if (x / 5 > 0)
                                    {
                                        s.Inverse(0);
                                    }
                                }

                                n /= 10;
                                res[1].Add(s);
                                if (n > 0)
                                {
                                    SSymbol sss = new SSymbol((Bitmap)Bitmap.FromFile(Images.PATHS[1] + Images.SIGNS[1] + ".png"), Images.colors[e.Group]);
                                    sss.Inverse(2);
                                    sss.Inverse(1);
                                    res[1].Add(sss);
                                }
                            }
                        }
                        SSymbol ss = new SSymbol((Bitmap)Bitmap.FromFile(Images.PATHS[1] + Images.SIGNS[1] + ".png"), Color.Black);
                        ss.Inverse(2);
                        res[1].Add(ss);
                    }
                }
                else
                {
                    foreach (Element e in arg)
                    {
                        if (e.type == Element.Type.SIXTEEN)
                        {
                            SSymbol s = new SSymbol((Bitmap)Bitmap.FromFile(Images.PATHS[1] + Images.SYMBOLS_DIGITS[e.origin] + ".png"), Images.colors[0]);
                            res[1].Add(s);
                        }
                        else
                        {
                            int n = getOrigin(e);
                            while (n > 0)
                            {
                                int x = n % 10;
                                SSymbol s = new SSymbol((Bitmap)Bitmap.FromFile(Images.PATHS[1] + symbols[x % 5 + x / 5 - 1] + ".png"), Images.colors[e.Group]);
                                if (e.origin > 10)
                                {
                                    s.Inverse(1);
                                    if (x / 5 > 0) s.Inverse(0);
                                }
                                else
                                {
                                    if (x / 5 == 0) s.Inverse(0);
                                }
                                s.Inverse(2);
                                n /= 10;
                                res[1].Add(s);
                            }
                        }
                        SSymbol ss = new SSymbol((Bitmap)Bitmap.FromFile(Images.PATHS[1] + Images.SIGNS[2] + ".png"), Color.Black);
                        ss.Inverse(2);
                        res[1].Add(ss);
                    }
                }
                res[1].Remove(res[1].Last());
            }
            return res;
        }

        private static int getOrigin(Element e)
        {
            int n = e.origin;
            int nn = 0;
            while (n > 0)
            {
                nn *= 10;
                nn += n % 10;
                n /= 10;
            }
            return nn;
        }

        public struct SSymbol
        {
            public Bitmap[] Inverses { get; }
            public Bitmap Symbol { get; }
            public Color Color { get; }

            public SSymbol(Bitmap symbol, Color color)
            {
                Inverses = new Bitmap[3];
                Symbol = symbol;
                Color = color;
            }

            public void Inverse(int i)
            {
                if (Inverses[i] != null) Inverses[i] = null;
                else Inverses[i] = (Bitmap)Bitmap.FromFile(Images.PATHS[1] + Images.INVERSES[i] + ".png");
            }
        }
    }
}
