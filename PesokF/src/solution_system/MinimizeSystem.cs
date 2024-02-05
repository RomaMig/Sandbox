using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PesokF
{

    class MinimizeSystem
    {
        public bool bazis { get; set; }
        public delegate void GetSolution(MinimizeSystem ms, ResolveArgs args);
        public event GetSolution Resolve;
        delegate Element dltmp(int[][] m, int i, int j);
        delegate void ChangeProgress();
        ToolStripProgressBar progress;
        ToolStripLabel text;
        Thread search;
        CancellationTokenSource cancelToken;
        ElementEqualComparer comparerE;

        public MinimizeSystem(ToolStripProgressBar progress, ToolStripLabel text, bool bazis)
        {
            this.progress = progress;
            this.text = text;
            this.bazis = bazis;
            comparerE = new ElementEqualComparer();
            cancelToken = new CancellationTokenSource();
        }

        public void Abort()
        {
            cancelToken?.Cancel();
            search?.Abort();
        }

        public void SearchMinSolution(int[][][] mrx, int deep, float diversity)
        {
            search = new Thread(() =>
            {
                changeProgress("Поиск", () =>
                {
                    progress.Value = 0;
                });
                SortedDictionary<Element.Weight, List<List<Element>[]>> map = new SortedDictionary<Element.Weight, List<List<Element>[]>>(new Element.WeightComparer());

                changeProgress("Подготовка карт Вейча", () =>
                {
                    progress.Value = 5;
                });
                List<int> exceptional = new List<int>();
                int premordialSize = mrx.Length;
                mrx = prepareMatrix(mrx, exceptional);
                List<List<Element>>[] configs = getAllCombinations(mrx);

                changeProgress("Поиск минимумов каждой карты", () =>
                {
                    progress.Value = 37;
                });
                List<Element>[][] eachMin = getAllMinCombinationOf(mrx, configs);
                for (int i = 0; i < eachMin.Length; i++)
                {
                    AddToDic(map, getWeightWithCommon(eachMin[i]), eachMin[i]);
                }

                changeProgress("Поиск общих элементов", () =>
                {
                    progress.Value = 46;
                });
                Dictionary<int[], List<Element>> listCommons = getAllCombinationsOfCommon(mrx);
                searchWhileFoundMin(mrx, listCommons, map);

                changeProgress("Глубокий поиск общих элементов", () =>
                {
                    progress.Value = 50;
                });
                RemoveDublicateFromMap(ref map);
                for (int i = 1; i <= ((deep > mrx.Length) ? mrx.Length : deep); i++)
                {
                    Element.Weight minTmpW;
                    do
                    {
                        minTmpW = map.First().Key;
                        searchDeepCommon(mrx, listCommons, map, i, diversity);
                    } while (Element.comparer.Compare(map.First().Key, minTmpW) < 0);
                }

                changeProgress("Получение решения", () =>
                {
                    progress.Value = 99;
                });
                List<List<Element>[]> minimums = map.First().Value;
                List<Element>[][] sol = new List<Element>[minimums.Count][];
                List<List<Element>[]> solution = new List<List<Element>[]>();
                int minCountQ = 9999;
                for (int j = 0; j < sol.Length; j++)
                {
                    sol[j] = minimums[j];
                    markCommonsOf(sol[j]);
                    int countQ = 0;
                    for (int i = 0; i < sol[j].Length; i++)
                    {
                        sol[j][i].Sort(new ElementComparer());
                        sol[j][i].ForEach((Element el) =>
                        {
                            switch (el.type)
                            {
                                case Element.Type.ONE:
                                    countQ += 4;
                                    break;
                                case Element.Type.TWO_H:
                                    countQ += 3;
                                    break;
                                case Element.Type.TWO_V:
                                    countQ += 3;
                                    break;
                                case Element.Type.FOUR_S:
                                    countQ += 2;
                                    break;
                                case Element.Type.FOUR_H:
                                    countQ += 2;
                                    break;
                                case Element.Type.FOUR_V:
                                    countQ += 2;
                                    break;
                                case Element.Type.EIGHT_H:
                                    countQ += 1;
                                    break;
                                case Element.Type.EIGHT_V:
                                    countQ += 1;
                                    break;
                                default:
                                    break;
                            }
                        });
                    }
                    if (countQ == minCountQ)
                    {
                        solution.Add(getClone(sol[j]));
                    }
                    if (countQ < minCountQ)
                    {
                        minCountQ = countQ;
                        solution = new List<List<Element>[]>();
                        solution.Add(getClone(sol[j]));
                    }
                    if (exceptional.Count > 0)
                    {
                        List<List<Element>> finalSol = new List<List<Element>>();
                        int delt = 0;
                        for (int r = 0; r < premordialSize; r++)
                        {
                            bool exceptExist = false;
                            foreach (int except in exceptional)
                            {
                                if (except % 10 == r)
                                {
                                    List<Element> tmpLElements = new List<Element>();
                                    tmpLElements.Add(new Element(Element.Type.SIXTEEN, except / 10, 0, 0));
                                    finalSol.Add(tmpLElements);
                                    delt++;
                                    exceptExist = true;
                                }
                            }
                            if (!exceptExist)
                            {
                                finalSol.Add(solution.Last()[r - delt]);
                            }
                        }
                        solution.Remove(solution.Last());
                        solution.Add(finalSol.ToArray());
                    }
                }

                Resolve(this, new ResolveArgs(solution.ToArray(), map.First().Key, minCountQ));

                changeProgress("Готово", () =>
                {
                    progress.Value = 100;
                });

            });
            search.Start();
        }

        private int[][][] prepareMatrix(int[][][] mrx, List<int> exceptional)
        {
            List<int[][]> tmp = new List<int[][]>();
            for (int i = 0; i < mrx.Length; i++)
            {
                int num = 0;
                int sum = 0;
                for (int n = 0; n < mrx[i].Length; n++)
                {
                    for (int m = 0; m < mrx[i][n].Length; m++)
                    {
                        if (mrx[i][n][m] != -1)
                        {
                            num++;
                            sum += mrx[i][n][m];
                        }
                    }
                }
                if (num != 0 && sum != 0 && (sum - num) != 0)
                    tmp.Add(mrx[i]);
                else
                    exceptional.Add((sum == 0 ? 0 : 10) + i);
            }
            return tmp.ToArray();
        }

        private void RemoveDublicateFromMap(ref SortedDictionary<Element.Weight, List<List<Element>[]>> map)
        {
            SortedDictionary<Element.Weight, List<List<Element>[]>> newMap = new SortedDictionary<Element.Weight, List<List<Element>[]>>(Element.comparer);
            foreach (KeyValuePair<Element.Weight, List<List<Element>[]>> kvp in map)
            {
                List<List<Element>[]> configs = kvp.Value;
                RemoveDublicate(ref configs);
                newMap.Add(kvp.Key, configs);
            }
            map = newMap;
        }

        private void RemoveDublicate(ref List<List<Element>[]> configs)
        {
            List<long[]> hashes = new List<long[]>();
            List<List<Element>[]> newConfigs = new List<List<Element>[]>();
            foreach (List<Element>[] config in configs)
            {
                long[] hash = getHashes(config);
                if (!hashes.Contains(hash, new LongArrayEqualComparer()))
                {
                    hashes.Add(hash);
                    newConfigs.Add(config);
                }
            }
            configs = newConfigs;
        }

        class LongArrayEqualComparer : IEqualityComparer<long[]>
        {
            public bool Equals(long[] x, long[] y)
            {
                if (x.Length == y.Length)
                {
                    bool res = true;
                    for (int i = 0; i < x.Length; i++)
                    {
                        res &= x[i] == y[i];
                    }
                    return res;
                }
                else
                {
                    return false;
                }
            }

            public int GetHashCode(long[] obj)
            {
                throw new NotImplementedException();
            }
        }

        private long[] getHashes(List<Element>[] config)
        {
            long[] hashes = new long[config.Length];
            for (int i = 0; i < config.Length; i++)
            {
                hashes[i] = 0;
                foreach (Element e in config[i])
                {
                    hashes[i] *= 100;
                    hashes[i] += e.HashCode;
                }
            }
            return hashes;
        }

        private List<Element>[] getClone(List<Element>[] l)
        {
            List<Element>[] res = new List<Element>[l.Length];
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = new List<Element>();
                foreach (Element e in l[i])
                {
                    res[i].Add(e);
                }
            }
            return res;
        }

        public class ResolveArgs
        {
            public List<Element>[][] solution { get; }
            public Element.Weight minWeight { get; }
            public int Q { get; }

            public ResolveArgs(List<Element>[][] solution, Element.Weight minWeight, int Q)
            {
                this.solution = solution;
                this.minWeight = minWeight;
                this.Q = Q;
            }
        }

        private void searchWhileFoundMin(int[][][] m, Dictionary<int[], List<Element>> listCommons, SortedDictionary<Element.Weight, List<List<Element>[]>> map)
        {
            List<List<Element>[]> list = new List<List<Element>[]>();
            list.AddRange(map.First().Value);
            Element.Weight minWeight = map.First().Key;
            for (int i = 0; i < list.Count; i++)
            {
                markCommonsOf(list[i]);
                findAllCombinationsOfMinWithCommon(m, list[i], listCommons, map);
            }
            if (Element.comparer.Compare(map.First().Key, minWeight) < 0)
                searchWhileFoundMin(m, listCommons, map);
        }

        private void searchDeepCommon(int[][][] m, Dictionary<int[], List<Element>> listCommons, SortedDictionary<Element.Weight, List<List<Element>[]>> map, int deep, float diversity)
        {
            List<List<Element>[]> configs = map.First().Value;
            ParallelOptions options = new ParallelOptions();
            options.CancellationToken = cancelToken.Token;
            options.MaxDegreeOfParallelism = System.Environment.ProcessorCount;
            int configCount = (int)(configs.Count * diversity);
            int countCheckConfigs = 0;
            int countStartConfigs = 0;
            float countReady = 0;
            try
            {
                Parallel.For(0, configCount, i =>
                {
                    countStartConfigs++;
                    Dictionary<int, List<List<List<Dictionary<int[], Element>>>>> maps = searchCommonAmongUnic(m, (List<Element>[])configs[i].Clone(), listCommons);
                    List<List<List<List<Dictionary<int[], Element>>>>> combinations = new List<List<List<List<Dictionary<int[], Element>>>>>();
                    PrepareCombinationsToDeepCommonSearch(deep > maps.Keys.Count ? maps.Keys.Count : deep, maps, combinations);
                    Parallel.ForEach(combinations, options, (List<List<List<Dictionary<int[], Element>>>> comb) =>
                    {
                        enumerateCommonInDeep(m, 0, comb, new List<Dictionary<int[], Element>>[comb.Count], (List<Element>[])configs[i].Clone(), map);
                        countReady++;
                        changeProgress(
                            String.Format("Глубина: {0} | Конфигураций проверено: {1}/{2} | Конфигураций проверяется в данный момент: {3}",
                            deep,
                            countCheckConfigs,
                            configCount,
                            countStartConfigs - countCheckConfigs),
                            () => { progress.Value = (int)(50 + 48 * (countReady / (combinations.Count / countStartConfigs * configCount))); });
                        options.CancellationToken.ThrowIfCancellationRequested();
                    });
                    countCheckConfigs++;
                });
            }
            catch (Exception) { }
        }
        private void searchDeepCommonForDebug(int[][][] m, Dictionary<int[], List<Element>> listCommons, SortedDictionary<Element.Weight, List<List<Element>[]>> map, int deep, float diversity)
        {
            List<List<Element>[]> configs = map.First().Value;
            ParallelOptions options = new ParallelOptions();
            options.CancellationToken = cancelToken.Token;
            options.MaxDegreeOfParallelism = System.Environment.ProcessorCount;
            int configCount = (int)(configs.Count * diversity);
            int countCheckConfigs = 0;
            int countStartConfigs = 0;
            float countReady = 0;
            try
            {
                for (int i = 0; i < configCount; i++)
                {
                    countStartConfigs++;
                    Dictionary<int, List<List<List<Dictionary<int[], Element>>>>> maps = searchCommonAmongUnic(m, (List<Element>[])configs[i].Clone(), listCommons);
                    List<List<List<List<Dictionary<int[], Element>>>>> combinations = new List<List<List<List<Dictionary<int[], Element>>>>>();
                    PrepareCombinationsToDeepCommonSearch(deep > maps.Keys.Count ? maps.Keys.Count : deep, maps, combinations);
                    Parallel.ForEach(combinations, options, (List<List<List<Dictionary<int[], Element>>>> comb) =>
                    {
                        enumerateCommonInDeep(m, 0, comb, new List<Dictionary<int[], Element>>[comb.Count], (List<Element>[])configs[i].Clone(), map);
                        countReady++;
                        changeProgress(
                            String.Format("Глубина: {0} | Конфигураций проверено: {1}/{2} | Конфигураций проверяется в данный момент: {3} | Оценка готовности: {4:F6}",
                            deep,
                            countCheckConfigs,
                            configCount,
                            countStartConfigs - countCheckConfigs,
                            countReady / (combinations.Count / countStartConfigs * configCount)), null);
                        options.CancellationToken.ThrowIfCancellationRequested();
                    });
                    countCheckConfigs++;
                }
            }
            catch (Exception) { }
        }


        private void PrepareCombinationsToDeepCommonSearch(int deep, Dictionary<int, List<List<List<Dictionary<int[], Element>>>>> maps, List<List<List<List<Dictionary<int[], Element>>>>> combinations)
        {
            List<List<int>> keys = new List<List<int>>();
            KeysPrepare(0, deep, maps.Keys.ToList(), new List<int>(), keys);
            foreach (List<int> key in keys)
                MainPrepareToDeepSearch(0, maps, new List<List<List<Dictionary<int[], Element>>>>(deep), combinations, key);
        }

        private void KeysPrepare(int level, int deep, List<int> keys, List<int> combKey, List<List<int>> res)
        {
            foreach (int key in keys)
            {
                if (level < combKey.Count)
                    combKey[level] = key;
                else
                    combKey.Add(key);
                if (level + 1 < deep)
                {
                    KeysPrepare(level + 1, deep, keys, combKey, res);
                }
                else
                {
                    List<int> tmp = new List<int>();
                    tmp.AddRange(combKey);
                    res.Add(tmp);
                }
            }
        }

        private void MainPrepareToDeepSearch(int level, Dictionary<int, List<List<List<Dictionary<int[], Element>>>>> maps, List<List<List<Dictionary<int[], Element>>>> comb, List<List<List<List<Dictionary<int[], Element>>>>> combinations, List<int> keys)
        {
            List<List<List<Dictionary<int[], Element>>>> value;
            maps.TryGetValue(keys[level], out value);
            foreach (List<List<Dictionary<int[], Element>>> v in value)
            {
                if (level < comb.Count)
                    comb[level] = v;
                else
                    comb.Add(v);
                if (level + 1 < keys.Count)
                    MainPrepareToDeepSearch(level + 1, maps, comb, combinations, keys);
                else
                {
                    List<List<List<Dictionary<int[], Element>>>> tmp = new List<List<List<Dictionary<int[], Element>>>>();
                    tmp.AddRange(comb);
                    combinations.Add(tmp);
                }
            }
        }

        private void enumerateCommonInDeep(int[][][] m, int deep, List<List<List<Dictionary<int[], Element>>>> comb, List<Dictionary<int[], Element>>[] config, List<Element>[] mainConfig, SortedDictionary<Element.Weight, List<List<Element>[]>> map)
        {
            foreach (List<Dictionary<int[], Element>> c in comb[deep])
            {
                config[deep] = c;
                if (deep + 1 < comb.Count)
                {
                    enumerateCommonInDeep(m, deep + 1, comb, (List<Dictionary<int[], Element>>[])config.Clone(), mainConfig, map);
                }
                else
                {
                    List<Element>[] tmpConfig = new List<Element>[mainConfig.Length];
                    for (int i = 0; i < mainConfig.Length; i++)
                    {
                        tmpConfig[i] = new List<Element>();
                        tmpConfig[i].AddRange(mainConfig[i]);
                    }
                    foreach (List<Dictionary<int[], Element>> el in config)
                    {
                        foreach (Dictionary<int[], Element> e in el)
                        {
                            foreach (int i in e.Keys.First())
                            {
                                Element arg = e.Values.First();
                                arg.Group = -1;
                                tmpConfig[i].Add(arg);
                            }
                        }
                    }
                    for (int i = 0; i < mainConfig.Length; i++)
                    {
                        //tmpConfig[i] = RemoveDuplicate(tmpConfig[i]);
                        int countConfig = tmpConfig[i].Count;
                        tmpConfig[i] = ExcessWithIgnore(m[i], tmpConfig[i]);
                        if (countConfig == tmpConfig[i].Count)
                            tmpConfig[i] = Excess(m[i], tmpConfig[i]);
                    }
                    Element.Weight curWeight = getWeightWithCommon(tmpConfig);
                    if (Element.comparer.Compare(curWeight, map.First().Key) < 0)
                        AddToDic(map, curWeight, tmpConfig);
                }
            }
        }

        private List<Element> RemoveDuplicate(List<Element> e)
        {
            return (List<Element>)e.Distinct(comparerE);
        }

        private List<Element> ExcessWithIgnore(int[][] m, List<Element> e)
        {
            if (!isReady(m, e)) return null;
            for (int i = 0; i < e.Count; i++)
            {
                if (e[i].Group != -1)
                {
                    List<Element> tmp = new List<Element>();
                    for (int j = 0; j < e.Count; j++)
                    {
                        if (i != j) tmp.Add(e[j]);
                    }
                    if (isReady(m, tmp)) return ExcessWithIgnore(m, tmp);
                }
            }
            return e;
        }

        private Dictionary<int, List<List<List<Dictionary<int[], Element>>>>> searchCommonAmongUnic(int[][][] m, List<Element>[] config, Dictionary<int[], List<Element>> listCommons)
        {
            markCommonsOf(config);
            Dictionary<int, List<List<List<Dictionary<int[], Element>>>>> res = new Dictionary<int, List<List<List<Dictionary<int[], Element>>>>>();
            for (int i = 0; i < config.Length; i++)
            {
                for (int j = 0; j < config[i].Count; j++)
                {
                    Element e = config[i][j];
                    if (e.Group == 0 && e.type != Element.Type.EIGHT_H && e.type != Element.Type.EIGHT_V)
                    {
                        Dictionary<int[], List<Element>> commons;
                        if (isSubstitute(m[i], config, i, e, listCommons, out commons))
                        {
                            AddToDic(res, i, getAllCombinationsOfCommonAmongUnci(m[i], e, commons));
                        }
                    }
                }
            }
            return res;
        }

        private bool isSubstitute(int[][] m, List<Element>[] config, int i, Element e, Dictionary<int[], List<Element>> listCommons, out Dictionary<int[], List<Element>> res)
        {
            Dictionary<int[], List<Element>> tmp = new Dictionary<int[], List<Element>>();
            foreach (KeyValuePair<int[], List<Element>> kvp in listCommons)
            {
                if (kvp.Key.Contains(i))
                    tmp.Add(kvp.Key, kvp.Value);
            }
            Dictionary<int[], List<Element>>.KeyCollection keys = tmp.Keys;
            List<int[]> sortedKeys = keys.ToList();
            sortedKeys.Sort((int[] a1, int[] a2) => { return a2.Length - a1.Length; });
            List<Element> con = new List<Element>();
            List<Element> commons = new List<Element>();
            res = new Dictionary<int[], List<Element>>();
            foreach (int[] key in sortedKeys)
            {
                List<Element> le;
                tmp.TryGetValue(key, out le);

                foreach (Element c in le)
                {
                    if (!commons.Contains(c) && isApplied(m, config, key, i, c))
                    {
                        commons.Add(c);
                        AddToDic(res, key, c);
                    }
                }
            }
            con.AddRange(config[i]);
            con.Remove(e);
            con.AddRange(commons);
            return isReady(m, con);
        }

        private bool isApplied(int[][] m, List<Element>[] config, int[] nums, int i, Element common)
        {
            bool res = false;
            foreach (int num in nums)
            {
                if (num != i)
                {
                    IEnumerable<Element> enumer = config[num]
                        .Where(e => e.type == Element.Type.EIGHT_H || e.type == Element.Type.EIGHT_V);
                    if (enumer == null) return true;
                    res |= enumer
                        .Any(e => isExcessElement2(m, e, common));
                }
            }
            return !res;
        }

        private List<List<Dictionary<int[], Element>>> getAllCombinationsOfCommonAmongUnci(int[][] m, Element e, Dictionary<int[], List<Element>> listCommons)
        {
            int[][] intersect = getIntersectionOf(m, e);
            List<List<Element>> configs = getAllCombinationsOf(intersect);
            List<List<Dictionary<int[], Element>>> res = new List<List<Dictionary<int[], Element>>>();
            foreach (List<Element> config in configs)
            {
                List<Dictionary<int[], Element>> resConfig = new List<Dictionary<int[], Element>>();
                bool fail = true;
                foreach (Element c in config)
                {
                    Dictionary<int[], Element> tmp = new Dictionary<int[], Element>();
                    foreach (KeyValuePair<int[], List<Element>> kvp in listCommons)
                    {
                        if (kvp.Value.Contains(c))
                        {
                            tmp.Add(kvp.Key, c);
                        }
                    }
                    if (fail = tmp.Count == 0) break;
                    else resConfig.Add(tmp);
                }
                if (!fail)
                {
                    res.Add(resConfig);
                }
            }
            return res;
        }

        private int[][] getIntersectionOf(int[][] m, Element e)
        {
            int[][] tmp = new int[m.Length][];
            for (int i = 0; i < tmp.Length; i++)
            {
                tmp[i] = new int[m[i].Length];
                for (int j = 0; j < tmp[i].Length; j++)
                {
                    if (!(m[i][j] == 0 && bazis || m[i][j] == 1 && !bazis) || (i < e.i + e.h && i >= e.i && j < e.j + e.w && j >= e.j))
                    {
                        tmp[i][j] = m[i][j];
                    }
                    else
                    {
                        tmp[i][j] = -1;
                    }
                }
            }
            return tmp;
        }

        private void markCommonsOf(List<Element>[] config)
        {
            int group = 1;
            for (int i = 0; i < config.Length; i++)
            {
                if (config[i] != null)
                {
                    for (int j = 0; j < config[i].Count; j++)
                    {
                        Element tmp = config[i][j];
                        tmp.Group = 0;
                        config[i][j] = tmp;
                    }
                }
            }
            Dictionary<int, int> repeated = new Dictionary<int, int>();
            List<Element> unic = new List<Element>();
            for (int i = 0; i < config.Length; i++)
            {
                if (config[i] != null)
                {
                    for (int j = 0; j < config[i].Count; j++)
                    {
                        if (!unic.Contains(config[i][j]))
                        {
                            unic.Add(config[i][j]);
                        }
                        else
                        {
                            if (!repeated.ContainsKey(config[i][j].origin))
                                repeated.Add(config[i][j].origin, group++);
                        }
                    }
                }
            }
            for (int i = 0; i < config.Length; i++)
            {
                if (config[i] != null)
                {
                    for (int j = 0; j < config[i].Count; j++)
                    {
                        if (repeated.TryGetValue(config[i][j].origin, out group))
                        {
                            Element tmp = config[i][j];
                            tmp.Group = group;
                            config[i][j] = tmp;
                        }
                    }
                }
            }
        }

        private List<List<Element>>[] convert(List<List<Element>[]> m)
        {
            List<List<Element>>[] res = new List<List<Element>>[m.First().Length];

            for (int i = 0; i < res.Length; i++)
            {
                res[i] = new List<List<Element>>();
            }
            foreach (List<Element>[] em in m)
            {
                for (int i = 0; i < res.Length; i++)
                {
                    if (!res[i].Contains(em[i]))
                        res[i].Add(em[i]);
                }
            }
            return res;
        }

        private List<List<Element>>[] getAllCombinations(int[][][] m)
        {
            int countVMaps = 0;
            List<List<Element>>[] res = new List<List<Element>>[m.Length];
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = getAllCombinationsOf(m[i]);
                countVMaps++;
                changeProgress((i + 1) + " карта Вейче готова. Подготовлено " + countVMaps + "/" + m.Length + " карт Вейче", () => { progress.Value += 32 / res.Length; });
            }
            return res;
        }

        private List<List<Element>> getAllCombinationsOf(int[][] m)
        {
            List<List<Element>> tmp = new List<List<Element>>();
            List<List<Element>> res = new List<List<Element>>();
            enumLists(m, ref tmp, new List<Element>(), getAllElementsInSystem(m));
            foreach (List<Element> l in tmp)
            {
                if (!isExcess(m, l)) res.Add(l);
            }
            return res;
        }

        Element elementForTest = new Element(Element.Type.ONE, 0, 0, 0);
        ElementEqualityToEightesComaparer eecForTest = new ElementEqualityToEightesComaparer();
        class ElementEqualityToEightesComaparer : IEqualityComparer<Element>
        {
            public bool Equals(Element e1, Element e2)
            {
                return e1.type == Element.Type.EIGHT_H ||
                    e1.type == Element.Type.EIGHT_V ||
                    e2.type == Element.Type.EIGHT_H ||
                    e2.type == Element.Type.EIGHT_V;
            }

            public int GetHashCode(Element obj)
            {
                throw new NotImplementedException();
            }
        }

        private void enumLists(int[][] m, ref List<List<Element>> res, List<Element> step, List<Element> input)
        {
            try
            {
                if (res.Count > 1000000)
                    return;
                if (step.Count > 0 && isReady(m, step))
                {
                    res.Add(new List<Element>(step));
                    return;
                }
                for (int i = 0; i < input.Count; i++)
                {
                    if (!isExcessElement2(m, step, input[i]))
                    {
                        step.Add(input[i]);
                        List<Element> tmp = new List<Element>();
                        tmp.AddRange(input.Skip(i + 1));
                        if (input.Contains(elementForTest, eecForTest))
                        {
                            if (step.Contains(elementForTest, eecForTest) || tmp.Contains(elementForTest, eecForTest))
                                enumLists(m, ref res, step, tmp);
                            else 
                                return;
                        }
                        else
                        {
                            enumLists(m, ref res, step, tmp);
                        }
                        step.RemoveAt(step.Count - 1);
                    }
                }
            }
            catch (OutOfMemoryException oome)
            {
                MessageBox.Show(oome.Message);
            }
        }

        private bool isExcessElement(int[][] m, List<Element> els, Element e)
        {
            List<Element> tmp = new List<Element>();
            tmp.AddRange(els);
            tmp.Add(e);
            return isExcess(m, tmp);
        }

        private bool isExcessElement2(int[][] m, Element origin, Element e)
        {
            bool[][] tmp = new bool[4][];
            bool[][] tmp2 = new bool[4][];
            for (int i = 0; i < 4; i++)
            {
                tmp[i] = new bool[4];
                tmp2[i] = new bool[4];
            }
            Prepare(tmp, origin);
            Prepare(tmp2, e);
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (!tmp[i][j] && tmp2[i][j] && (m[i][j] == 0 && bazis || m[i][j] == 1 && !bazis)) return false;
                }
            }
            return true;
        }

        private bool isExcessElement2(int[][] m, List<Element> els, Element e)
        {
            bool[][] tmp = new bool[4][];
            bool[][] tmp2 = new bool[4][];
            for (int i = 0; i < 4; i++)
            {
                tmp[i] = new bool[4];
                tmp2[i] = new bool[4];
            }
            foreach (Element el in els)
            {
                Prepare(tmp, el);
            }
            Prepare(tmp2, e);
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (!tmp[i][j] && tmp2[i][j] && (m[i][j] == 0 && bazis || m[i][j] == 1 && !bazis)) return false;
                }
            }
            return true;
        }

        private void EnumerationEverything(List<List<Element>>[] configs, SortedDictionary<Element.Weight, List<List<Element>[]>> map, Element.WeightComparer comparer, int deep, int bottom, List<Element>[] system)
        {
            //--------------------------------------------------
            //
            // Перебор вариантов конфигурации отдельной карты Вейче
            // Заполнение config пока не будет готов isReady(config)
            // Получение всех возможных готовых вариантов config
            //
            //--------------------------------------------------
            //  Begin
            foreach (List<Element> config in configs[deep])
            {
                system[deep] = config;
                if (deep + 3 > bottom)
                {
                    Element.Weight curWeight = getWeightWithCommon(system);
                    Element.Weight minWeight = map.First().Key;
                    if (comparer.Compare(curWeight, minWeight) <= 0)
                    {
                        if (deep + 1 == bottom)
                            AddToDic(map, curWeight, (List<Element>[])system.Clone());
                        else
                            EnumerationEverything(configs, map, comparer, deep + 1, bottom, system);
                    }
                }
                else
                {
                    EnumerationEverything(configs, map, comparer, deep + 1, bottom, system);
                }
            }
            system[deep] = null;
            //  End
        }

        private void EnumerationEverythingParallel(List<List<Element>>[] configs, SortedDictionary<Element.Weight, List<List<Element>[]>> map, Element.WeightComparer comparer, int deep, int bottom)
        {
            ParallelQuery<List<Element>> pq = configs[deep].AsParallel();
            pq.ForAll((List<Element> config) =>
            {
                List<Element>[] system = new List<Element>[bottom];
                system[deep] = config;
                Element.Weight curWeight = getWeightWithCommon(system);
                Element.Weight minWeight = map.First().Key;

                if (comparer.Compare(curWeight, minWeight) <= 0)
                {
                    if (deep + 1 == bottom)
                        AddToDic(map, curWeight, (List<Element>[])system.Clone());
                    else
                        EnumerationEverything(configs, map, comparer, deep + 1, bottom, (List<Element>[])system.Clone());
                }
            });
        }

        private void AddToDic<TKey, TValue>(SortedDictionary<TKey, List<TValue>> dic, TKey key, TValue value)
        {
            lock (dic)
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
        private void AddToDic<TKey, TValue>(Dictionary<TKey, List<TValue>> dic, TKey key, TValue value)
        {
            lock (dic)
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

        private List<Element> getAllElementsInSystem(int[][] m)
        {
            dltmp[] funcs = { One, TwoH, TwoV, FourS, FourH, FourV, EightH, EightV };
            Element tmp;
            List<Element> res = new List<Element>();

            for (int k = 0; k < funcs.Length; k++)
            {
                for (int i = 0; i < m.Length; i++)
                {
                    for (int j = 0; j < m[i].Length; j++)
                    {
                        tmp = funcs[k](m, i, j);
                        if (tmp.origin != -1 && !res.Contains(tmp, comparerE))
                            res.Add(tmp);
                    }
                }
            }
            res.Sort((Element e1, Element e2) => { return e1.weight.Pesok - e2.weight.Pesok; });
            return res;
        }
        private List<Element>[] getMinCombinationOf(int[][][] m, List<List<Element>>[] configs)
        {
            List<Element>[] res = new List<Element>[m.Length];
            Element.WeightComparer comparer = new Element.WeightComparer();
            for (int i = 0; i < configs.Length; i++)
            {
                Element.Weight cur;
                Element.Weight min = new Element.Weight(9999, 999);
                foreach (List<Element> c in configs[i])
                {
                    if (comparer.Compare(cur = getWeight(c), min) < 0)
                    {
                        min = cur;
                        res[i] = c;
                    }
                }
            }
            return res;
        }
        private List<Element>[][] getAllMinCombinationOf(int[][][] m, List<List<Element>>[] configs)
        {
            List<List<Element>>[] preRes = new List<List<Element>>[m.Length];
            List<Element>[] minConfig = getMinCombinationOf(m, configs);
            Element.WeightComparer comparer = new Element.WeightComparer();
            for (int i = 0; i < configs.Length; i++)
            {
                changeProgress("Поиск минимума " + (i + 1) + " карты Вейча", () => { progress.Value += 8 / configs.Length; });
                Element.Weight min = getWeight(minConfig[i]);
                foreach (List<Element> c in configs[i])
                {
                    if (comparer.Compare(getWeight(c), min) <= 0)
                    {
                        if (preRes[i] == null) preRes[i] = new List<List<Element>>();
                        preRes[i].Add(c);
                    }
                }
            }
            int length = 1;
            foreach (List<List<Element>> r in preRes)
            {
                length *= r.Count;
            }
            List<Element>[][] res = new List<Element>[length][];

            for (int i = 0; i < length; i++)
            {
                res[i] = new List<Element>[m.Length];
                for (int j = 0; j < m.Length; j++)
                {
                    res[i][j] = preRes[j][i % preRes[j].Count];
                }
            }
            return res;
        }

        private List<Element> getMinCombinationOf_wrong(int[][] m)
        {
            List<Element> all = getAllElementsInSystem(m);
            //all.Sort((Element e1, Element e2) => { if (e1.type < e2.type) return -1; if (e1.type > e2.type) return 1; return 0; });
            List<Element> tmp = new List<Element>();
            for (int i = all.Count - 1; i > -1; i--)
            {
                tmp.Add(all[i]);
                if (isReady(m, tmp)) break;
            }
            List<Element> res = new List<Element>();
            Element[] t = Excess(m, tmp.ToArray());
            if (t != null)
            {
                res.AddRange(t);
                return res;
            }
            return tmp;
        }

        private void findAllCombinationsOfMinWithCommon(int[][][] m, List<Element>[] config, Dictionary<int[], List<Element>> commons, SortedDictionary<Element.Weight, List<List<Element>[]>> map)
        {
            Element.WeightComparer comparer = new Element.WeightComparer();
            Element.Weight minWeight = getWeightWithCommon(config);
            Parallel.ForEach(commons, (KeyValuePair<int[], List<Element>> com) =>
            {
                int[] nums = com.Key;
                List<Element> common = com.Value;
                Parallel.ForEach(common, (Element c) =>
                {
                    List<List<Element>[]> collectMin = new List<List<Element>[]>();
                    findCommonAmongMin(m, nums, 0, nums.Length - 1, (List<Element>[])config.Clone(), c, collectMin);
                    Parallel.ForEach(collectMin, (List<Element>[] min) =>
                    {
                        Element.Weight w = getWeightWithCommon(min);
                        if (comparer.Compare(w, minWeight) <= 0)
                            AddToDic(map, w, min);
                    });
                });
            });
        }

        private void findCommonAmongMin(int[][][] m, int[] nums, int deep, int bottom, List<Element>[] config, Element common, List<List<Element>[]> res)
        {
            List<Element> tmp = new List<Element>();
            tmp.AddRange(config[nums[deep]]);
            for (int j = 0; j < tmp.Count; j++)
            {
                if (Element.Equals(tmp[j], common) && tmp[j].Group == 0)
                {
                    if (deep == bottom)
                        res.Add(config);
                    else
                        findCommonAmongMin(m, nums, deep + 1, bottom, config, common, res);
                }
                else
                {
                    if (tmp[j].type != Element.Type.EIGHT_H && tmp[j].type != Element.Type.EIGHT_V)
                    {
                        List<Element> min = new List<Element>();
                        min.AddRange(tmp);
                        min.RemoveAt(j);
                        min.Add(common);
                        if (isReady(m[nums[deep]], min))
                        {
                            config[nums[deep]] = min;
                            if (deep == bottom)
                                res.Add(config);
                            else
                                findCommonAmongMin(m, nums, deep + 1, bottom, config, common, res);
                        }
                    }
                }
            }
        }

        private Dictionary<int[], List<Element>> getAllCombinationsOfCommon(int[][][] m)
        {
            Dictionary<int[], List<Element>> map = new Dictionary<int[], List<Element>>();
            for (int n = 2; n <= m.Length; n++)
            {
                int[] arr = new int[n];
                Enumeration(m, map, arr, 0, m.Length - n + 1, 0, m.Length);
            }
            return map;
        }

        private void Enumeration(int[][][] m, Dictionary<int[], List<Element>> d, int[] arr, int l, int r, int b, int n)
        {
            for (int i = l; i < r; i++)
            {
                arr[b] = i;
                if (r < n)
                    Enumeration(m, d, arr, i + 1, r + 1, b + 1, n);
                else
                {
                    List<Element> tmp = getCommonOf(m, arr);
                    if (tmp != null && tmp.Count > 0)
                    {
                        d.Add((int[])arr.Clone(), tmp);
                    }
                }
            }
        }

        private Element.Weight getWeightWithCommon(params List<Element>[] e)
        {
            int pesok, evema;
            pesok = evema = 0;
            List<Element> repeated = new List<Element>();
            for (int i = 0; i < e.Length; i++)
            {
                if (e[i] != null)
                    foreach (Element el in e[i])
                    {
                        if (!repeated.Contains(el))
                        {
                            pesok += el.weight.Pesok;
                            evema += el.weight.Evema;
                            repeated.Add(el);
                        }
                    }
            }
            return new Element.Weight(pesok, evema);
        }

        private Element.Weight getWeight(List<Element> commons, params List<Element>[] e)
        {
            int pesok, evema;
            pesok = evema = 0;
            List<Element> repeated = new List<Element>();
            for (int i = 0; i < e.Length; i++)
            {
                if (e[i] != null && commons != null)
                {
                    pesok += getWeight(commons, e[i].ToArray()).Pesok;
                    evema += getWeight(commons, e[i].ToArray()).Evema;
                }
            }
            foreach (Element el in commons)
            {
                pesok += el.weight.Pesok;
                evema += el.weight.Evema;
            }
            return new Element.Weight(pesok, evema);
        }

        private Element.Weight getWeight(List<Element> commons, params Element[] e)
        {
            int pesok, evema;
            pesok = evema = 0;
            for (int i = 0; i < e.Length; i++)
            {
                if (!commons.Contains(e[i]))
                {
                    pesok += e[i].weight.Pesok;
                    evema += e[i].weight.Evema;
                }
            }
            return new Element.Weight(pesok, evema);
        }

        private Element.Weight getWeight(params List<Element>[] e)
        {
            int pesok, evema;
            pesok = evema = 0;
            for (int i = 0; i < e.Length; i++)
            {
                pesok += getWeight(e[i]).Pesok;
                evema += getWeight(e[i]).Evema;
            }
            return new Element.Weight(pesok, evema);
        }

        private Element.Weight getWeight(List<Element> e)
        {
            return getWeight(e.ToArray());
        }

        private Element.Weight getWeight(params Element[] e)
        {
            int pesok, evema;
            pesok = evema = 0;
            for (int i = 0; i < e.Length; i++)
            {
                pesok += e[i].weight.Pesok;
                evema += e[i].weight.Evema;
            }
            return new Element.Weight(pesok, evema);
        }

        /*private Element.Weight getWeight(params Element.Weight[] w)*/

        private List<Element> getCommonOf(int[][][] m, params int[] n)
        {
            List<Element>[] all = new List<Element>[n.Length];
            for (int k = 0; k < n.Length; k++)
            {
                all[k] = getAllElementsInSystem(m[n[k]]);
            }
            List<Element> res = new List<Element>();
            for (int i = 0; i < all[0].Count; i++)
            {
                bool contains = false;
                bool allContains = true;
                Element tmp = all[0].ElementAt(i);
                for (int j = 1; j < n.Length; j++)
                {
                    allContains &= all[j].Contains(tmp);
                    contains |= res.Contains(tmp);
                }
                if (!contains && allContains)
                    res.Add(tmp);
            }
            return res;
        }

        private List<Element> getCommonOfSystem(List<Element>[] system)
        {
            List<Element> commons = new List<Element>();
            for (int i = 0; i < system.Length - 1; i++)
            {
                if (system[i] != null)
                {
                    foreach (Element e in system[i])
                    {
                        for (int j = i + 1; j < system.Length; j++)
                        {
                            if (system[j] != null && system[j].Contains(e) && !commons.Contains(e))
                            {
                                commons.Add(e);
                            }
                        }
                    }
                }
            }
            return commons;
        }

        private Element[] Excess(int[][] m, params Element[] e)
        {
            if (!isReady(m, e)) return null;
            Array.Sort(e, (Element e1, Element e2) => { if (e1.weight.Pesok > e2.weight.Pesok) return -1; if (e1.weight.Pesok < e2.weight.Pesok) return 1; return 0; });
            for (int i = 0; i < e.Length; i++)
            {
                List<Element> tmp = new List<Element>();
                for (int j = 0; j < e.Length; j++)
                {
                    if (i != j) tmp.Add(e[j]);
                }
                if (isReady(m, tmp)) return Excess(m, tmp.ToArray());
            }
            return e;
        }
        private List<Element> Excess(int[][] m, List<Element> e)
        {
            if (!isReady(m, e)) return null;
            e.Sort((Element e1, Element e2) => { if (e1.weight.Pesok > e2.weight.Pesok) return -1; if (e1.weight.Pesok < e2.weight.Pesok) return 1; return 0; });
            for (int i = 0; i < e.Count; i++)
            {
                List<Element> tmp = new List<Element>();
                for (int j = 0; j < e.Count; j++)
                {
                    if (i != j) tmp.Add(e[j]);
                }
                if (isReady(m, tmp)) return Excess(m, tmp);
            }
            return e;
        }

        private bool isExcess(int[][] m, params Element[] e)
        {
            if (!isReady(m, e)) return false;
            Array.Sort(e, (Element e1, Element e2) => { if (e1.weight.Pesok > e2.weight.Pesok) return -1; if (e1.weight.Pesok < e2.weight.Pesok) return 1; return 0; });
            for (int i = 0; i < e.Length; i++)
            {
                List<Element> tmp = new List<Element>();
                for (int j = 0; j < e.Length; j++)
                {
                    if (i != j) tmp.Add(e[j]);
                }
                if (isReady(m, tmp)) return true;
            }
            return false;
        }

        private bool isExcess(int[][] m, List<Element> e)
        {
            if (!isReady(m, e)) return false;
            for (int i = 0; i < e.Count; i++)
            {
                List<Element> tmp = new List<Element>();
                for (int j = 0; j < e.Count; j++)
                {
                    if (i != j) tmp.Add(e[j]);
                }
                if (isReady(m, tmp)) return true;
            }
            return false;
        }

        private bool isReady(int[][] m, List<Element> e)
        {
            bool[][] tmp = new bool[4][];
            for (int i = 0; i < 4; i++)
            {
                tmp[i] = new bool[4];
            }
            foreach (Element el in e)
            {
                Prepare(tmp, el);
            }
            for (int i = 0; i < m.Length; i++)
            {
                for (int j = 0; j < m[i].Length; j++)
                {
                    if (!tmp[i][j] && (m[i][j] == 0 && bazis || m[i][j] == 1 && !bazis)) return false;
                }
            }
            return true;
        }

        private bool isReady(int[][] m, params Element[] e)
        {
            bool[][] tmp = new bool[4][];
            for (int i = 0; i < 4; i++)
            {
                tmp[i] = new bool[4];
            }
            for (int i = 0; i < e.Length; i++)
            {
                Prepare(tmp, e[i]);
            }
            for (int i = 0; i < m.Length; i++)
            {
                for (int j = 0; j < m[i].Length; j++)
                {
                    if (!tmp[i][j] && (m[i][j] == 0 && bazis || m[i][j] == 1 && !bazis)) return false;
                }
            }
            return true;
        }

        private void Prepare(bool[][] m, Element e)
        {
            for (int i = e.i; i < e.i + e.h; i++)
            {
                for (int j = e.j; j < e.j + e.w; j++)
                {
                    m[i < 4 ? i : 0][j < 4 ? j : 0] = true;
                }
            }
        }

        public struct Element
        {
            public static WeightComparer comparer { get; }
            public Type type { get; }
            public int origin { get; }
            public int i { get; }
            public int j { get; }
            public int w { get; }
            public int h { get; }
            public Weight weight { get; }
            public int Group { get; set; }
            public long HashCode { get; }

            static Element()
            {
                comparer = new WeightComparer();
            }

            public Element(Type type, int origin, int i, int j)
            {
                this.type = type;
                this.origin = origin;
                this.Group = 0;
                this.i = i;
                this.j = j;
                int pesok, evema = 1;
                HashCode = 0;
                switch (type)
                {
                    case Element.Type.ONE:
                        w = h = 1;
                        pesok = 4;
                        HashCode = i * 4 + j;
                        break;
                    case Element.Type.TWO_H:
                        w = 2; h = 1;
                        pesok = 3;
                        HashCode += 16;
                        HashCode = i * 4 + j;
                        break;
                    case Element.Type.TWO_V:
                        w = 1; h = 2;
                        pesok = 3;
                        HashCode += 32;
                        HashCode = i * 4 + j;
                        break;
                    case Element.Type.FOUR_S:
                        w = 2; h = 2;
                        pesok = 2;
                        HashCode += 48;
                        HashCode = i * 4 + j;
                        break;
                    case Element.Type.FOUR_H:
                        w = 4; h = 1;
                        pesok = 2;
                        HashCode += 64;
                        HashCode += i;
                        break;
                    case Element.Type.FOUR_V:
                        w = 1; h = 4;
                        pesok = 2;
                        HashCode += 68;
                        HashCode += j;
                        break;
                    case Element.Type.EIGHT_H:
                        w = 4; h = 2;
                        pesok = 0;
                        evema = 0;
                        HashCode += 72;
                        HashCode += i;
                        break;
                    case Element.Type.EIGHT_V:
                        w = 2; h = 4;
                        pesok = 0;
                        evema = 0;
                        HashCode += 76;
                        HashCode += j;
                        break;
                    case Element.Type.SIXTEEN:
                        w = 4; h = 4;
                        pesok = -1;
                        evema = 0;
                        break;
                    default:
                        w = h = 0;
                        pesok = 0;
                        break;
                }
                weight = new Weight(pesok, evema);
            }

            public bool Equals(Element e)
            {
                return origin == e.origin;
            }

            public struct Weight
            {
                public int Pesok { get; set; }
                public int Evema { get; set; }

                public Weight(int pesok, int evema)
                {
                    Pesok = pesok;
                    Evema = evema;
                }
            }

            public class WeightComparer : IComparer<Weight>
            {
                public int Compare(Weight x, Weight y)
                {
                    if (x.Pesok == y.Pesok && x.Evema == y.Evema) return 0;
                    if (x.Pesok <= y.Pesok && x.Evema <= y.Evema) return -1;
                    if (x.Pesok >= y.Pesok && x.Evema >= y.Evema) return 1;
                    return 0;
                }
            }

            public enum Type
            {
                ONE, TWO_H, TWO_V, FOUR_S, FOUR_H, FOUR_V, EIGHT_H, EIGHT_V, SIXTEEN
            }
        }
        class ElementEqualComparer : IEqualityComparer<Element>
        {
            public bool Equals(Element x, Element y)
            {
                return x.origin == y.origin;
            }

            public int GetHashCode(Element obj)
            {
                throw new NotImplementedException();
            }
        }

        class ElementComparer : IComparer<Element>
        {
            public int Compare(Element x, Element y)
            {
                return x.origin - y.origin;
            }
        }

        int[,] oneQ = {
            { 5678, 5674, 5634, 5638},
            { 5278, 5274, 5234, 5238},
            { 1278, 1274, 1234, 1238},
            { 1678, 1674, 1634, 1638},
        };
        int[,] twoHQ = {
            { 567, 564, 563, 568},
            { 527, 524, 523, 528},
            { 127, 124, 123, 128},
            { 167, 164, 163, 168},
        };
        int[,] twoVQ = {
            { 578, 574, 534, 538},
            { 278, 274, 234, 238},
            { 178, 174, 134, 138},
            { 678, 674, 634, 638},
        };
        int[,] fourSQ = {
            { 57, 54, 53, 58},
            { 27, 24, 23, 28},
            { 17, 14, 13, 18},
            { 67, 64, 63, 68},
        };
        int[,] fourHQ = {
            { 56, 56, 56, 56},
            { 52, 52, 52, 52},
            { 12, 12, 12, 12},
            { 16, 16, 16, 16},
        };
        int[,] fourVQ = {
            { 78, 74, 34, 38},
            { 78, 74, 34, 38},
            { 78, 74, 34, 38},
            { 78, 74, 34, 38},
        };
        int[,] eightHQ = {
            { 5, 5, 5, 5},
            { 2, 2, 2, 2},
            { 1, 1, 1, 1},
            { 6, 6, 6, 6},
        };
        int[,] eightVQ = {
            { 7, 4, 3, 8},
            { 7, 4, 3, 8},
            { 7, 4, 3, 8},
            { 7, 4, 3, 8},
        };

        private bool isWrongElExist(params int[] n)
        {
            bool f = false;
            for (int i = 0; i < n.Length; i++)
            {
                if (n[i] == 1 && bazis || n[i] == 0 && !bazis) return true;
                f |= n[i] == 0 && bazis || n[i] == 1 && !bazis;
            }
            return !f;
        }

        private int pattern(bool success, int n)
        {
            return success ? -1 : n;
        }

        private Element One(int[][] m, int i, int j)
        {
            return new Element(Element.Type.ONE, pattern(isWrongElExist(m[i][j]), oneQ[i, j]), i, j);
        }
        private Element TwoH(int[][] m, int i, int j)
        {
            return new Element(Element.Type.TWO_H, pattern(isWrongElExist(m[i][j], m[i][j < 3 ? j + 1 : 0]), twoHQ[i, j]), i, j);
        }
        private Element TwoV(int[][] m, int i, int j)
        {
            return new Element(Element.Type.TWO_V, pattern(isWrongElExist(m[i][j], m[i < 3 ? i + 1 : 0][j]), twoVQ[i, j]), i, j);
        }
        private Element FourS(int[][] m, int i, int j)
        {
            return new Element(Element.Type.FOUR_S, pattern(isWrongElExist(m[i][j], m[i][j < 3 ? j + 1 : 0], m[i < 3 ? i + 1 : 0][j], m[i < 3 ? i + 1 : 0][j < 3 ? j + 1 : 0]), fourSQ[i, j]), i, j);
        }
        private Element FourH(int[][] m, int i, int j)
        {
            return new Element(Element.Type.FOUR_H, pattern(isWrongElExist(m[i][0], m[i][1], m[i][2], m[i][3]), fourHQ[i, j]), i, j);
        }
        private Element FourV(int[][] m, int i, int j)
        {
            return new Element(Element.Type.FOUR_V, pattern(isWrongElExist(m[0][j], m[1][j], m[2][j], m[3][j]), fourVQ[i, j]), i, j);
        }
        private Element EightH(int[][] m, int i, int j)
        {
            return new Element(Element.Type.EIGHT_H, pattern(isWrongElExist(m[i][0], m[i][1], m[i][2], m[i][3],
                m[i < 3 ? i + 1 : 0][0], m[i < 3 ? i + 1 : 0][1], m[i < 3 ? i + 1 : 0][2], m[i < 3 ? i + 1 : 0][3]), eightHQ[i, j]), i, j);
        }
        private Element EightV(int[][] m, int i, int j)
        {
            return new Element(Element.Type.EIGHT_V, pattern(isWrongElExist(m[0][j], m[1][j], m[2][j], m[3][j],
                m[0][j < 3 ? j + 1 : 0], m[1][j < 3 ? j + 1 : 0], m[2][j < 3 ? j + 1 : 0], m[3][j < 3 ? j + 1 : 0]), eightVQ[i, j]), i, j);
        }


        private void changeProgress(string text, ChangeProgress cp)
        {
            this.text.Text = text;
            cp?.Invoke();
        }
    }
}
