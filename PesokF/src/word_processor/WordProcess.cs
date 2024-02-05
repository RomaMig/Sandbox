using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Office.Interop.Word;

namespace PesokF.src.w
{
    class WordProcess
    {
        //private static readonly string PATH = @"C:\Users\pc\source\repos\PesokF\PesokF\data\doc\";
        private static readonly string PATH = Program.PathExe() + @"\data\doc\";
        private static readonly string[] NAMES = { "RS ИЛИ-НЕ", "RS И-НЕ", "T", "ADD-SUB", "REV", "SF" };
        private delegate void DocCreator(SolutionInfo si, string path);
        private readonly DocCreator[] create;
        private Microsoft.Office.Interop.Word.Application app;

        public WordProcess()
        {
            app = new Microsoft.Office.Interop.Word.Application();
            create = new DocCreator[]{ create_doc_autors, create_doc_countrev, create_doc_autot, create_doc_switchfun };
        }

        public void CreateDoc(SolutionInfo si, string path)
        {
            try
            {
                switch (si.taskInfo.Type)
                {
                    case TaskType.AUTOMAT:
                        if (si.trigger == "RS")
                            create[0](si, path);
                        else
                            create[2](si, path);
                        break;
                    case TaskType.COUNTER_ADD:
                    case TaskType.COUNTER_SUB:
                        create[2](si, path);
                        break;
                    case TaskType.COUNTER_REV:
                        create[1](si, path);
                        break;
                    case TaskType.SWITCH_FUNC:
                        create[3](si, path);
                        break;
                }
            }
            catch (Exception)
            {
                app.Quit(false, false, false);
            }
        }

        private void create_doc_switchfun(SolutionInfo si, string path)
        {
            Object missing = Type.Missing;
            Object fileName = PATH + NAMES[5] + ".docx";
            app.Documents.Open(ref fileName);
            ReplacePatternToText("@task", si.taskInfo.Task);
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 1; j++)
                {
                    ReplacePatternToText("@n", si.mainTable[i, j]);
                }
            }
            
            foreach (int[][] m1 in si.veichTables)
            {
                foreach (int[] m2 in m1)
                {
                    foreach (int m in m2)
                    {
                        ReplacePatternToText("@n", m == -1 ? "-" : m.ToString());
                    }
                }
            }
            
            string imgPath = path + @"\imgs";
            ReplacePatternToImage("@img", imgPath + @"\_S" + 0 + ".png");
            ReplacePatternToImage("@img", imgPath + @"\_F" + 0 + ".png");
            
            ReplacePatternToImage("@img", imgPath + @"\_E" + 0 + ".png");
            
            for (int i = 1; i < si.taskInfo.Ones.Length + 1; i++)
            {
                ReplacePatternToText("@s", "");
                ReplacePatternToImage("@img", imgPath + @"\_E" + i + ".png");
            }

            ReplacePatternToText("@s @img", "Наборы, на которых функция принимает значение 0");

            for (int i = si.taskInfo.Ones.Length + 1; i < si.taskInfo.Ones.Length + si.taskInfo.Zeros.Length + 1; i++)
            {
                ReplacePatternToText("@s", "");
                ReplacePatternToImage("@img", imgPath + @"\_E" + i + ".png");
            }

            for (int i = si.taskInfo.Ones.Length + si.taskInfo.Zeros.Length + 1; i < 17; i++)
            {
                RemoveParagraphByPattern("@s");
            }

            app.ActiveDocument.SaveAs(path + @"\Отчет.docx", missing, missing, missing, missing,
                    missing, missing, missing, missing, missing, missing,
                    missing, missing, missing, missing, missing);
            app.Quit(false, ref missing, ref missing);
        }

        private void create_doc_countrev(SolutionInfo si, string path)
        {
            Object missing = Type.Missing;
            Object fileName = PATH + NAMES[4] + ".docx";
            app.Documents.Open(ref fileName);
            ReplacePatternToText("@task", si.taskInfo.Task);
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    ReplacePatternToText("@n", si.mainTable[i, j]);
                }
            }
            
            foreach (int[][] m1 in si.veichTables)
            {
                foreach (int[] m2 in m1)
                {
                    foreach (int m in m2)
                    {
                        ReplacePatternToText("@n", m == -1 ? "-" : m.ToString());
                    }
                }
            }
            
            string imgPath = path + @"\imgs";
            for (int i = 0; i < 3; i++)
            {
                ReplacePatternToImage("@img", imgPath + @"\_S" + i + ".png");
                ReplacePatternToImage("@img", imgPath + @"\_F" + i + ".png");
            }
            
            ReplacePatternToImage("@img", imgPath + @"\_E" + 0 + ".png");
            ReplacePatternToImage("@img", imgPath + @"\_E" + si.taskInfo.k1 + ".png");
            for (int i = 1; i <= si.taskInfo.k1; i++)
            {
                ReplacePatternToText("@s", "");
                ReplacePatternToImage("@img", imgPath + @"\_E" + i + ".png");
            }
            ReplacePatternToText("@s @img", "Работа схемы в режиме вычитания");

            for (int i = si.taskInfo.k1; i <= si.taskInfo.k1 + si.taskInfo.k2; i++)
            {
                ReplacePatternToText("@s", "");
                ReplacePatternToImage("@img", imgPath + @"\_E" + i + ".png");
            }

            for (int i = si.transitions.Length; i < 23; i++)
            {
                RemoveParagraphByPattern("@s");
            }
            
            app.ActiveDocument.SaveAs(path + @"\Отчет.docx", missing, missing, missing, missing,
                    missing, missing, missing, missing, missing, missing,
                    missing, missing, missing, missing, missing);
            app.Quit(false, ref missing, ref missing);
        }


        private void create_doc_autot(SolutionInfo si, string path)
        {
            Object missing = Type.Missing;
            Object fileName = PATH + NAMES[2] + ".docx";
            app.Documents.Open(ref fileName);
            ReplacePatternToText("@task", si.taskInfo.Task);
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    ReplacePatternToText("@n", si.mainTable[i, j]);
                }
            }

            foreach (int[][] m1 in si.veichTables)
            {
                foreach (int[] m2 in m1)
                {
                    foreach (int m in m2)
                    {
                        ReplacePatternToText("@n", m == -1 ? "-" : m.ToString());
                    }
                }
            }

            string imgPath = path + @"\imgs";
            for (int i = 0; i < 4; i++)
            {
                ReplacePatternToImage("@img", imgPath + @"\_S" + i + ".png");
                ReplacePatternToImage("@img", imgPath + @"\_F" + i + ".png");
            }

            ReplacePatternToImage("@img", imgPath + @"\_E" + 0 + ".png");
            ReplacePatternToImage("@img", imgPath + @"\_E" + (si.transitions.Length - 1) + ".png");
            for (int i = 1; i < si.transitions.Length; i++)
            {
                ReplacePatternToText("@s", "");
                ReplacePatternToImage("@img", imgPath + @"\_E" + i + ".png");
            }

            for (int i = si.transitions.Length; i < 17; i++)
            {
                RemoveParagraphByPattern("@s");
            }

            app.ActiveDocument.SaveAs(path + @"\Отчет.docx", missing, missing, missing, missing,
                    missing, missing, missing, missing, missing, missing,
                    missing, missing, missing, missing, missing);
            app.Quit(false, ref missing, ref missing);
        }

        private void create_doc_autors(SolutionInfo si, string path)
        {
            Object missing = Type.Missing;
            Object fileName = PATH + NAMES[(si.bazis ? 0 : 1)] + ".docx";
            app.Documents.Open(ref fileName);
            ReplacePatternToText("@task", si.taskInfo.Task);
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    ReplacePatternToText("@n", si.mainTable[i, j]);
                }
            }
            foreach (int[][] m1 in si.veichTables)
            {
                foreach (int[] m2 in m1)
                {
                    foreach (int m in m2)
                    {
                        ReplacePatternToText("@n", m == -1 ? "-" : m.ToString());
                    }
                }
            }
            foreach (string s in si.transitions)
            {
                ReplacePatternToText("@k", s);
                ReplacePatternToText("@k", s);
            }
            string imgPath = path + @"\imgs";
            for (int i = 0; i < 8; i++)
            {
                ReplacePatternToImage("@img", imgPath + @"\_S" + i + ".png");
                ReplacePatternToImage("@img", imgPath + @"\_F" + i + ".png");
            }
            int numPages = app.ActiveDocument.ComputeStatistics(WdStatistic.wdStatisticPages, false);
            int endPage = (numPages - (17 - si.transitions.Length) / 2) - 1;
            for (int i = numPages - 10; i < endPage; i++)
            {
                ReplacePatternToText("@p", i.ToString());
                ReplacePatternToText("@p", i.ToString());
            }
            for (int i = si.transitions.Length; i < 17; i++)
            {
                RemoveParagraphByPattern("@p");
            }
            ReplacePatternToText("@p", endPage.ToString());
            ReplacePatternToImage("@img", imgPath + @"\_E" + 0 + ".png");
            ReplacePatternToImage("@img", imgPath + @"\_E" + (si.transitions.Length - 1) + ".png");
            for (int i = 1; i < si.transitions.Length; i++)
            {
                if (i > 1)
                {
                    ReplacePatternToText("@s", "");
                    ReplacePatternToText("@s", "");
                    ReplacePatternToText("@s", "");
                }
                ReplacePatternToImage("@img", imgPath + @"\_E" + i + ".png");
            }
            for (int i = si.transitions.Length; i < 17; i++)
            {
                RemoveParagraphByPattern("@s");
                RemoveParagraphByPattern("@s");
                RemoveParagraphByPattern("@s");
            }
            app.ActiveDocument.SaveAs(path + @"\Отчет.docx", missing, missing, missing, missing,
                    missing, missing, missing, missing, missing, missing,
                    missing, missing, missing, missing, missing);
            app.Quit(false, ref missing, ref missing);
        }

        private void RemoveParagraphByPattern(string pattern)
        {
            Object missing = Type.Missing;
            Range range = app.ActiveDocument.Content;

            range.Find.Text = pattern;
            range.Find.ClearFormatting();
            if (range.Find.Execute(ref missing, ref missing, ref missing, ref missing, ref missing,
                  ref missing, ref missing, ref missing, ref missing, ref missing,
                  ref missing, ref missing, ref missing, ref missing, ref missing)
                )
            {
                range.Expand(WdUnits.wdParagraph);
                range.Delete(ref missing, ref missing);
            }
        }

        private void ReplacePatternToText(string pattern, string text)
        {
            Object missing = Type.Missing;
            Object wrap = WdFindWrap.wdFindContinue;
            Object replace = WdReplace.wdReplaceOne;
            Find find = app.Selection.Find;
            find.Text = pattern;
            find.Replacement.Text = text;
            find.Execute(FindText: Type.Missing,
                MatchCase: false,
                MatchWholeWord: false,
                MatchWildcards: false,
                MatchSoundsLike: missing,
                MatchAllWordForms: false,
                Forward: true,
                Wrap: wrap,
                Format: false,
                ReplaceWith: missing,
                Replace: replace);
        }

        private void ReplacePatternToImage(string pattern, string fileName)
        {
            Object missing = Type.Missing;
            Range range = app.ActiveDocument.Content;
            Find find = range.Find;

            find.Text = pattern;
            find.ClearFormatting();
            if (find.Execute(ref missing, ref missing, ref missing, ref missing, ref missing,
                  ref missing, ref missing, ref missing, ref missing, ref missing,
                  ref missing, ref missing, ref missing, ref missing, ref missing)
                )
            {
                range.InlineShapes.AddPicture(fileName, ref missing, ref missing, ref missing);

                find.Replacement.ClearFormatting();
                find.Replacement.Text = "";
                object replaceOne = WdReplace.wdReplaceOne;
                find.Execute(ref missing, ref missing, ref missing, ref missing, ref missing,
                   ref missing, ref missing, ref missing, ref missing, ref missing,
                    ref replaceOne, ref missing, ref missing, ref missing, ref missing);
            }
            else
            {
                throw new Exception("The text could not be located.");
            }
        }
    }
}
