using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Text;
using System.Web.Http;
using static OlympicSearchServer.BattleTableData.BattleResult;
using static OlympicSearchServer.BattleTableData.BattleResult.MatchResult;
using static OlympicSearchServer.DateMatchDetails;
using static OlympicSearchServer.DateMatchDetails.Units.Competitors;
using static OlympicSearchServer.MatchNameData.Props.PageProps.InitialFilterDisciplines;
using static OlympicSearchServer.MatchNameData.Props.PageProps.InitialFilterDisciplines.Disciplines;
using static OlympicSearchServer.ResultCombine;

namespace OlympicSearchServer
{
    /// <summary>
    /// 创建数据使用，非服务器运行时调用
    /// </summary>
    public class DataPraser
    {
        public static string CrawlerDataPath = "Resources\\CrawlerData\\crawlerData.json";
        public static string MatchNameDataPath = "Resources\\CrawlerData\\matchNameData.json";
        public static string NationalMedalsPath = "Resources\\CrawlerData\\nationalMedals.json";

        public static string ResourcesPath = "Resources";
        public static string DayResultSavePath = "Resources\\DayResult";


        public static string GetBracketTableHttp(string matchID)
        {
            return $"https://olympics.com/OG2024/data/GLO_Bracket~comp=OG2024~rsc={matchID}~lang=CHI.json";
        }
        public static string GetDailyFixturesHttp(string date)
        {
            return $"https://sph-s-api.olympics.com/summer/schedules/api/CHI/schedule/day/{date}";
        }
        public static string GetSchedulesHttp(string disciplineCode)
        {
            return $"https://olympics.com/OG2024/data/SCH_StartList~comp=OG2024~disc={disciplineCode}~lang=CHI.json";
        }
        public static string GetScheduleResultsHttp(string disciplineCode, string id)
        {
            return $"https://olympics.com/OG2024/data/RES_ByRSC_H2H~comp=OG2024~disc={disciplineCode}~rscResult={id}~lang=CHI.json";
        }
        public static void ReadAllMatchData()
        {
            MatchNameData data = JsonConvert.DeserializeObject<MatchNameData>(File.ReadAllText(CrawlerDataPath));
            Dictionary<string, Disciplines> allMatchData = new Dictionary<string, Disciplines>();
            foreach (var a in data.props.pageProps.initialFilterDisciplines.disciplines)
            {
                allMatchData.Add(a.name, a);
            }
            File.WriteAllText(MatchNameDataPath, JsonConvert.SerializeObject(allMatchData));
        }
        public static void ReadAllNationalMedalDetails()
        {
            NationalMedalDetails data = JsonConvert.DeserializeObject<NationalMedalDetails>(File.ReadAllText(CrawlerDataPath));
            List<NationalMeadls> allNationalData = new List<NationalMeadls>();
            foreach (var a in data.props.pageProps.initialMedals.medalStandings.medalsTable)
            {
                foreach (var b in a.medalsNumber)
                {
                    if (b.type.Equals("Total"))
                    {
                        NationalMeadls mid = new NationalMeadls();
                        mid.description = a.description;
                        mid.organisation = a.organisation;
                        mid.rank = a.rank;
                        mid.total = b.total;
                        mid.gold = b.gold;
                        mid.silver = b.silver;
                        mid.bronze = b.bronze;
                        allNationalData.Add(mid);
                    }
                }
            }
            allNationalData.Sort((x, y) => { return x.rank.CompareTo(y.rank); });
            File.WriteAllText(NationalMedalsPath, JsonConvert.SerializeObject(allNationalData));
        }

        public static string GetHttpJson(string httpPath)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(httpPath);
            request.Proxy = null;
            request.KeepAlive = false;
            request.Method = "GET";
            request.ContentType = "application/json; charset=UTF-8";
            request.AutomaticDecompression = DecompressionMethods.GZip;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
            string retString = myStreamReader.ReadToEnd();

            myStreamReader.Close();
            myResponseStream.Close();

            if (response != null)
            {
                response.Close();
            }
            if (request != null)
            {
                request.Abort();
            }

            return retString;
        }

        public static BattleTableData GetBattleTable(string id)
        {
            BattleTableData result = new BattleTableData();
            string savePath = Path.Combine(ResourcesPath, "BracketData", id + ".json");



            //获取已经存在的数据
            if (File.Exists(savePath))
            {
                string json = File.ReadAllText(savePath);
                result = JsonConvert.DeserializeObject<BattleTableData>(json);
                return result;
            }
            //若不存在则获取

            BattleTable data = new BattleTable();
            string httpPath = GetBracketTableHttp(id);
            var jsonResult = Communicable(httpPath);
            if (!jsonResult.Item1) return null;
            data = JsonConvert.DeserializeObject<BattleTable>(jsonResult.Item2);
            try
            {
                foreach (var a in data.bracket)
                {
                    result.id = a.documentCode;
                    Console.WriteLine(a.documentCode);
                    if (!a.bracketCode.Equals("BRN") && !a.bracketCode.Equals("FNL")) continue;
                    foreach (var b in a.bracketPhases)
                    {
                        BattleTableData.BattleResult target;
                        if (b.phaseCode.Equals("QFNL"))
                        {
                            target = result.qFinal;

                        }
                        else if (b.phaseCode.Equals("SFNL"))
                        {
                            target = result.halfFinal;
                        }
                        else if (b.bracketItems.Count == 1)
                        {
                            if (a.bracketCode.Equals("BRN")) target = result.final2;
                            else if (b.phaseCode.Equals("FNL-")) target = result.final;
                            else continue;
                        }//若超过四分之一决赛，舍弃
                        else continue;


                        foreach (var c in b.bracketItems)
                        {
                            target.description = c.eventUnit.description;
                            BattleTableData.BattleResult.MatchResult matchResult = new BattleTableData.BattleResult.MatchResult();

                            bool isWinner;
                            BattleTable.Bracket.BracketPhases.BracketItems.BracketCompetitors bracketCompetitors;
                            //参赛一
                            bracketCompetitors = c.bracketCompetitors[0];
                            isWinner = (bracketCompetitors.cp_wlt.Equals("W"));
                            if (bracketCompetitors.participant != null)
                                matchResult.competitor1 = new BattleTableData.BattleResult.MatchResult.Competitor(bracketCompetitors.participant.organisation.code, bracketCompetitors.participant.shortName, bracketCompetitors.cp_result, isWinner);
                            else matchResult.competitor1 = new BattleTableData.BattleResult.MatchResult.Competitor("", "轮空", "", false);
                            //参赛二
                            bracketCompetitors = c.bracketCompetitors[1];
                            isWinner = (bracketCompetitors.cp_wlt.Equals("W"));
                            if (bracketCompetitors.participant != null)
                                matchResult.competitor2 = new BattleTableData.BattleResult.MatchResult.Competitor(bracketCompetitors.participant.organisation.code, bracketCompetitors.participant.shortName, bracketCompetitors.cp_result, isWinner);
                            else matchResult.competitor2 = new BattleTableData.BattleResult.MatchResult.Competitor("", "轮空", "", false);
                            target.allMatch.Add(matchResult);
                        }


                    }

                }
                File.WriteAllText(savePath, JsonConvert.SerializeObject(result));
                Console.WriteLine("success");
                Console.WriteLine(savePath);
                Console.WriteLine(httpPath);

            }
            catch (Exception e)
            {
                Console.WriteLine("false");
                Console.WriteLine("false");
                Console.WriteLine("false");
                Console.WriteLine(savePath);
                Console.WriteLine(httpPath);
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();


            }
            return result;

        }
        public static void UpdateResultCombine()
        {
            string[] allFiles = Directory.GetFiles(Path.Combine(ResourcesPath, "ResultCombine"));
            foreach(var a in allFiles)
            {
                string json = File.ReadAllText(a);
                ResultCombineJsonUse result = JsonConvert.DeserializeObject<ResultCombineJsonUse>(json);
                //处理
                List<ResultCombine> allResults = result.resultCombines;
                //比赛阶段
                allResults.Sort((x, y) => TestCompare(x,y));


                File.WriteAllText(a, JsonConvert.SerializeObject(result));
            }

        }

        public static int TestCompare(ResultCombine x, ResultCombine y)
        {
            if (x.stateName.Contains("金") || x.stateName.Equals("决赛")) return 1000;
            if (y.stateName.Contains("金") || y.stateName.Equals("决赛")) return -1000;
            if (x.stateName.Contains("铜")) return 500;
            if (y.stateName.Contains("铜")) return -500;
            if (x.stateName.Contains("半决赛")) return 200;
            if (y.stateName.Contains("半决赛")) return -200;
            if (x.stateName.Contains("1/4")) return 100;
            if (y.stateName.Contains("1/4")) return -100;
            if (x.stateName.Substring(0, 2).Equals(y.stateName.Substring(0, 2)) )
            {
                return x.stateName.CompareTo(y.stateName);
            }
            if (x.stateName.Length.Equals(y.stateName.Length))
            {
                return y.results.Count.CompareTo(x.results.Count);
            }
            return x.results.Count.CompareTo(y.results.Count);

        }

        public static ResultCombineJsonUse GetResultCombine(string disciplineCode, string eventId)
        {
            string savePath = Path.Combine(ResourcesPath, "ResultCombine", eventId.Substring(0,11)+".json");
            if (File.Exists(savePath))
            {
                string json=File.ReadAllText(savePath);
                ResultCombineJsonUse result = JsonConvert.DeserializeObject<ResultCombineJsonUse>(json);
                return result;

            }

            string http = DataPraser.GetSchedulesHttp(disciplineCode);
            var jsonResult = DataPraser.Communicable(http);
            if (!jsonResult.Item1|| !eventId.Substring(0,3).Equals(disciplineCode))
            {
                Console.WriteLine($"{http}   数据获取错误");
                return null;
            }
            GetSchelusClass getSchelusClass;
            getSchelusClass = JsonConvert.DeserializeObject<GetSchelusClass>(jsonResult.Item2);
            eventId = eventId.Substring(0, 11);
            getSchelusClass.schedules.RemoveAll(x => !x.code.Contains(eventId)||!x.status.code.Equals("FINISHED"));

            List<ResultCombine> allResults = new List<ResultCombine>();
            foreach (var a in getSchelusClass.schedules)
            {
                string resultHttp = DataPraser.GetScheduleResultsHttp(disciplineCode, a.code);
                var jsonResult_mid = DataPraser.Communicable(resultHttp);
                if (!jsonResult_mid.Item1)
                {
                    Console.WriteLine("");
                    Console.WriteLine("");
                    Console.WriteLine("");
                    Console.WriteLine(resultHttp);
                    Console.WriteLine("");
                    Console.WriteLine("");
                    Console.WriteLine("");
                    continue;
                }
                GetResultClass mid = JsonConvert.DeserializeObject<GetResultClass>(jsonResult_mid.Item2);

                if (!mid.results.schedule.status.code.Equals("FINISHED")) continue;
                if (mid.results.items.Count > 2)
                {
                    Console.WriteLine($"{http}   该项目数据结构不支持期望");
                    return null;
                }


                ResultCombine target = allResults.Find(x => x.stateName.Equals(mid.results.eventUnit.shortDescription));
                if (target == null || target.Equals(default(ResultCombine)))
                {
                    target = new ResultCombine() { stateName = mid.results.eventUnit.shortDescription };
                    allResults.Add(target);
                }
                NewMatchResult matchResult = new NewMatchResult();
                GetResultClass.Result.Item item;  
                item = mid.results.items[0];
                matchResult.competitor1 = new MatchResult.Competitor(item.participant.organisation.code, item.participant.shortName, item.resultData, !string.IsNullOrEmpty(item.resultWLT) &&item.resultWLT.Equals("W"));
                item = mid.results.items[1];
                matchResult.competitor2 = new MatchResult.Competitor(item.participant.organisation.code, item.participant.shortName, item.resultData, !string.IsNullOrEmpty(item.resultWLT) && item.resultWLT.Equals("W"));
                matchResult.startDate = mid.results.schedule.startDate.Substring(0,16);
                target.results.Add(matchResult);
            }
            //排序 需要修改，当前方法排序不对
            allResults.Sort((x, y) => x.results.Count.CompareTo(y.results.Count));
            foreach (var a in allResults)
            {
                a.results.Sort((x, y) => x.startDate.CompareTo(y.startDate));
            }
            ResultCombineJsonUse resultCombineJsonUse = new ResultCombineJsonUse();
            resultCombineJsonUse.resultCombines = allResults;
            File.WriteAllText(savePath, JsonConvert.SerializeObject(resultCombineJsonUse));
            return resultCombineJsonUse;

        }



        /// <summary>
        /// 判断是否通信正常，并返回通信结果
        /// </summary>
        /// <param name="http"></param>
        /// <returns></returns>
        public static (bool, string) Communicable(string http)
        {
            Console.WriteLine(http);

            string jsonData = GetHttpJson(http);
            //Console.WriteLine(http);
            //if (!jsonData[1].Equals('!')) Console.WriteLine(http);
            //else
            //{
            //    Console.WriteLine("错误http");
            //    Console.WriteLine(http);
            //    Console.WriteLine();
            //}
            return (!jsonData[1].Equals('!') && !jsonData[1].Equals('}'), jsonData);
        }
    }

    public class DataGetController : ApiController
    {
        public class BracketFirstNamePackge
        {
            public List<string> allHasBracketMatchFirstName = new List<string>();
        }

        public static List<NationalMeadls> allNationalMedals = new List<NationalMeadls>();
        static Dictionary<string, Disciplines> allMatchData = new Dictionary<string, Disciplines>();
        public static List<string> allHasBracketMatchFirstName = new List<string>();
        static DataGetController()
        {
            if (!File.Exists(DataPraser.MatchNameDataPath)) DataPraser.ReadAllMatchData();
            if (!File.Exists(DataPraser.NationalMedalsPath)) DataPraser.ReadAllNationalMedalDetails();
            allNationalMedals = JsonConvert.DeserializeObject<List<NationalMeadls>>(File.ReadAllText(DataPraser.NationalMedalsPath));
            allMatchData = JsonConvert.DeserializeObject<Dictionary<string, Disciplines>>(File.ReadAllText(DataPraser.MatchNameDataPath));

            string bracketFirstNamePackgePath = Path.Combine(DataPraser.ResourcesPath, "BracketFirstNamePackge.json");
            if (File.Exists(bracketFirstNamePackgePath))
            {
                allHasBracketMatchFirstName = JsonConvert.DeserializeObject<BracketFirstNamePackge>(File.ReadAllText(bracketFirstNamePackgePath)).allHasBracketMatchFirstName;
            }
            else
            {
                if (allMatchData != null)
                {
                    allHasBracketMatchFirstName = allMatchData.Keys.ToList();
                    //删除所有无对阵表的
                    allHasBracketMatchFirstName.RemoveAll(x => !DataPraser.Communicable(DataPraser.GetBracketTableHttp(allMatchData[x].events[0].id)).Item1);
                    BracketFirstNamePackge save = new BracketFirstNamePackge();
                    save.allHasBracketMatchFirstName = allHasBracketMatchFirstName;
                    File.WriteAllText(bracketFirstNamePackgePath, JsonConvert.SerializeObject(save));
                }
                else Console.WriteLine("数据初始化错误！");
            }

            foreach (var a in allHasBracketMatchFirstName) Console.WriteLine(a);
        }

        /// <summary>
        /// 获取所有比赛名称
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public GetResult<List<string>> GetAllMatchName()
        {
            GetResult<List<string>> result = new GetResult<List<string>>();
            if (allHasBracketMatchFirstName.Count == 0)
            {
                result.code = 0;
                result.message = "比赛名称数据为空，为服务器数据出错！";
            }
            else result.data = allHasBracketMatchFirstName;

            return result;
        }
        /// <summary>
        /// 获取所有比赛详细名称
        /// </summary>
        /// <param name="firstName"></param>
        /// <returns></returns>
        [HttpGet]
        public GetResult<List<MatchDetailName>> GetAllMatchDetailName(string firstName)
        {
            GetResult<List<MatchDetailName>> result = new GetResult<List<MatchDetailName>>();
            if (allMatchData.ContainsKey(firstName))
            {
                result.data = allMatchData[firstName].events.Select(x => new MatchDetailName() { id = x.id, description = x.name }).ToList();
            }
            else
            {
                result.code = 0;
                result.message = $"{firstName}比赛不存在相关数据！";
            }
            return result;
        }

        /// <summary>
        /// 获取国家排名数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public GetResult<List<NationalMeadls>> GetAllNationalMedals()
        {
            GetResult<List<NationalMeadls>> result = new GetResult<List<NationalMeadls>>();
            if (allNationalMedals.Count == 0)
            {
                result.code = 0;
                result.message = "国家奖牌数量数据为空，为服务器数据出错！";
            }
            else result.data = allNationalMedals;
            return result;
        }
        public class StringIntPair
        {
            public string value1;
            public int value2;
            public StringIntPair(string value1, int value2)
            {
                this.value1 = value1;
                this.value2 = value2;
            }
        }
        public class MatchDetailName
        {
            public string id;
            public string description;
        }
        Dictionary<string, GetResult<List<Units>>> saveDayResult = new Dictionary<string, GetResult<List<Units>>>();
        List<StringIntPair> useCheck = new List<StringIntPair>();
        [HttpGet]
        public GetResult<List<Units>> GetDayResult(string Date)
        {
            
            GetResult <List<Units>> result = new GetResult<List<Units>>();

            string[] strings = Date.Split('-');
            int month= int.Parse(strings[1]), day= int.Parse(strings[2]);
      
            if (string.IsNullOrEmpty(Date) || strings.Length != 3 || !strings[0].Equals("2024") || month < 7|| month>8||(month==7&& (day <24|| day >31))||(month==8&&(day<1||day>11)))
            {
                result.code = 0;
                result.message = "日期错误！";
                Console.WriteLine($"参数  {Date} 的数据获取错误!");
                return result;
            }

            string savePath = Path.Combine(DataPraser.DayResultSavePath, Date + ".json");
            
            DateMatchDetails data;
            if (saveDayResult.ContainsKey(Date))
            {
                result.data = saveDayResult[Date].data;
            }
            else
            {
                if (File.Exists(savePath))
                {
                    string jsonData = File.ReadAllText(savePath);
                    data = JsonConvert.DeserializeObject<DateMatchDetails>(jsonData);
                }
                else
                {
                    string httpPath = DataPraser.GetDailyFixturesHttp(Date);
                    var jsonResult = DataPraser.Communicable(httpPath);
                    //if (!jsonResult.Item1)
                    //{
                    //    result.code = 0;
                    //    result.message = "日期错误！";
                    //    Console.WriteLine($"参数  {Date} 的数据获取错误!");
                    //    return result;
                    //}
                    data = JsonConvert.DeserializeObject<DateMatchDetails>(jsonResult.Item2);

                    foreach (var a in data.units)
                    {
                        a.startDate = a.startDate.Substring(11, 5);
                        if (a.competitors == null) continue;
                        if (a.competitors.Count > 3)
                        {
                            a.competitors.RemoveRange(3, a.competitors.Count - 3);
                        }
                        //存在参赛者但是不存在比赛数据，则不返回具体比赛情况
                        if (a.competitors.Count > 0 && a.competitors[0].results == null) a.competitors = null;
                    }
                    ////进行筛选 个人赛保留三位competitor的数据

                    File.WriteAllText(savePath, JsonConvert.SerializeObject(data));
                    saveDayResult.Add(Date, result);
                    useCheck.Add(new StringIntPair(Date, 5));
                }
                result.data = data.units;

            }
            //清理
            foreach (var a in useCheck)
            {
                a.value2--;
                if (a.value2 < 0) saveDayResult.Remove(a.value1);
                Console.WriteLine($"{a.value1}   {a.value2}");
            }
            useCheck.RemoveAll(x => x.value2 < 0);

            return result;
        }


        //测试对阵表数据获取
        //public void TestBracketValue()
        //{
        //    BattleTable data = new();
        //    string httpPath = DataPraser.GetBracketHttp("FBLMTEAM11------------------------");
        //    string jsonData = DataPraser.GetHttpJson(httpPath);
        //    data = JsonConvert.DeserializeObject<BattleTable>(jsonData);
        //    File.WriteAllText(Path.Combine(DataPraser.ResourcesPath,"TestBracket.json"), JsonConvert.SerializeObject(data,Formatting.Indented));
        //}
        [HttpGet]
        public GetResult<BattleTableData> GetBattleTable(string id)
        {
            GetResult<BattleTableData> result = new GetResult<BattleTableData>();
            BattleTableData mid = DataPraser.GetBattleTable(id);
            if (mid == null)
            {
                result.code = 0;
                result.message = $"对阵表通信未成功！大概率是该比赛：{id} 为个人赛不支持对阵表";
            }
            else
            {
                result.data = mid;
            }
            return result;
        }


        [HttpGet]
        public GetResult<List<ResultCombine>> GetResultCombine(string disciplineCode, string eventId)
        {
            GetResult<List<ResultCombine>> result = new GetResult<List<ResultCombine>>();
            ResultCombineJsonUse resultCombineJsonUse = DataPraser.GetResultCombine(disciplineCode, eventId);
            if (resultCombineJsonUse == null)
            {
                result.code = 0;
                result.message = "获取数据失败,或该项目数据结构不支持期望";
                return result;
            }
            result.data = resultCombineJsonUse.resultCombines;
            return result;

        }


    }
    [Serializable]
    public class BattleTableData
    {
        [Serializable]
        public class BattleResult
        {
            [Serializable]
            public class MatchResult
            {
                [Serializable]
                public class Competitor
                {
                    public string countryEN;
                    public string name;
                    public string score;
                    public bool isWinner;
                    public Competitor(string countryEN, string name, string score, bool isWinner)
                    {
                        this.countryEN = countryEN;
                        this.name = name;
                        this.score = score;
                        this.isWinner = isWinner;
                    }
                    public void Show()
                    {
                        Console.WriteLine($"{name} {score} {isWinner}");
                    }
                }
                public Competitor competitor1, competitor2;
            }
            /// <summary>
            /// 当前阶段的名称
            /// </summary>
            public string description;
            /// <summary>
            /// 所有当前阶段的比赛
            /// </summary>
            public List<MatchResult> allMatch = new List<MatchResult>();
        }
        /// <summary>
        /// 表示是哪一个比赛
        /// </summary>
        public string id;
        public BattleResult final = new BattleResult(), final2 = new BattleResult(), halfFinal = new BattleResult(), qFinal = new BattleResult();



        public void ShowResult()
        {
            Console.WriteLine("=============================Begin===============================");
            Console.WriteLine($"============================={final.description}===============================");
            final.allMatch[0].competitor1.Show();
            final.allMatch[0].competitor2.Show();
            if (final2.allMatch.Count > 0)
            {
                Console.WriteLine($"============================={final2.description}===============================");
                final2.allMatch[0].competitor1.Show();
                final2.allMatch[0].competitor2.Show();
            }
            for (int i = 0; i < halfFinal.allMatch.Count; i++)
            {
                Console.WriteLine($"============================={halfFinal.description}  {i + 1}===============================");
                halfFinal.allMatch[i].competitor1.Show();
                halfFinal.allMatch[i].competitor2.Show();
            }
            for (int i = 0; i < qFinal.allMatch.Count; i++)
            {
                Console.WriteLine($"============================={qFinal.description}  {i + 1}===============================");
                qFinal.allMatch[i].competitor1.Show();
                qFinal.allMatch[i].competitor2.Show();
            }
            Console.WriteLine("(\"============================End===============================");


        }

    }




    public class NationalMeadls
    {
        /// <summary>
        /// 英文缩写
        /// </summary>
        public string organisation;
        /// <summary>
        /// 中文名
        /// </summary>
        public string description;
        public int rank;
        public int gold, silver, bronze, total;
    }
    public class GetResult<T>
    {
        /// <summary>
        /// 1成功，0失败
        /// </summary>
        public int code = 1;

        public string message;

        public T data;

    }
    /// <summary>
    /// 用于序列化比赛名称数据
    /// </summary>
    [Serializable]
    public class MatchNameData
    {
        [Serializable]
        public class Props
        {
            [Serializable]
            public class PageProps
            {
                [Serializable]
                public class InitialFilterDisciplines
                {
                    [Serializable]
                    public class Disciplines
                    {
                        [Serializable]
                        public class Events
                        {
                            public string id;
                            public string name;
                        }
                        public List<Events> events;
                        public string id;
                        public string name;
                    }
                    public List<Disciplines> disciplines;
                }
                public InitialFilterDisciplines initialFilterDisciplines;
            }
            public PageProps pageProps;
        }
        public Props props;
    }



    /// <summary>
    /// 用于序列化国家奖牌数据
    /// </summary>
    [Serializable]
    public class NationalMedalDetails
    {
        [Serializable]
        public class Props
        {
            [Serializable]
            public class PageProps
            {
                [Serializable]
                public class InitialMedals
                {
                    [Serializable]
                    public class MedalStandings
                    {
                        [Serializable]
                        public class MedalsTable
                        {
                            [Serializable]
                            public class Disciplines
                            {
                                [Serializable]
                                public class MedalWinners
                                {
                                    public string competitorDisplayName;
                                    public string date;
                                    public string medalType;
                                    public string competitorType;


                                }
                                public string name;
                                public int gold;
                                public int silver;
                                public int bronze;
                                public int total;
                                public MedalWinners[] medalWinners;
                            }
                            [Serializable]
                            public class MedalsNumber
                            {
                                public string type;
                                public int gold;
                                public int silver;
                                public int bronze;
                                public int total;

                            }
                            public Disciplines[] disciplines;
                            public string organisation;
                            public string description;
                            public string longDescription;
                            public int rank;
                            public MedalsNumber[] medalsNumber;

                        }
                        public MedalsTable[] medalsTable;
                    }
                    public MedalStandings medalStandings;
                }
                public InitialMedals initialMedals;
            }
            public PageProps pageProps;
        }
        public Props props;

    }

    /// <summary>
    /// 用于获取详细的日期数据
    /// </summary>
    [Serializable]
    public class DateMatchDetails
    {
        [Serializable]
        public class Units
        {
            public string disciplineName;
            public string eventUnitName;
            public string id;
            public string disciplineCode;
            public string eventId;
            public string startDate;
            public List<Competitors> competitors;
            [Serializable]
            public class Competitors
            {
                public string noc;
                public string name;
                public Result results;
                [Serializable]
                public class Result
                {
                    public string mark;
                }
            }
        }
        public List<Units> units;
    }
    [Serializable]
    public class BattleTable
    {
        [Serializable]
        public class Bracket
        {
            [Serializable]
            public class BracketPhases
            {
                [Serializable]
                public class BracketItems
                {
                    [Serializable]
                    public class EventUnit
                    {
                        public string code;
                        public string description;
                    }
                    [Serializable]
                    public class BracketCompetitors
                    {
                        [Serializable]
                        public class Participant
                        {
                            [Serializable]
                            public class Athlete
                            {
                                [Serializable]
                                public class Person
                                {
                                    public string name;
                                }
                                public int order;
                                public Person person;

                            }
                            [Serializable]
                            public class Organisation
                            {
                                /// <summary>
                                /// 国家缩写
                                /// </summary>
                                public string code;
                                /// <summary>
                                /// 国家中文
                                /// </summary>
                                public string description;
                            }
                            public string __typename;
                            public string shortName;
                            public List<Athlete> athletes;
                            /// <summary>
                            /// 获取参赛队伍或人员国籍数据
                            /// </summary>
                            public Organisation organisation;
                        }
                        public string cp_pos;
                        /// <summary>
                        /// 输赢
                        /// </summary>
                        public string cp_wlt;
                        /// <summary>
                        /// 得分
                        /// </summary>
                        public string cp_result;
                        public Participant participant;
                    }
                    /// <summary>
                    /// 其中的description存放比赛名称，金牌铜牌，半决赛，1/4决赛
                    /// </summary>
                    public EventUnit eventUnit;
                    public List<BracketCompetitors> bracketCompetitors;

                }
                public string code;
                /// <summary>
                /// 判断比赛类型
                /// </summary>
                public string phaseCode;

                public List<BracketItems> bracketItems;
            }
            public string documentCode;
            public string bracketCode;
            public List<BracketPhases> bracketPhases;




        }
        public List<Bracket> bracket;

    }
    [Serializable]
    public class GetSchelusClass
    {
        [Serializable]
        public class Schedules
        {
            //用来确定比赛类型，比赛阶段
            public string code;
            public GetResultClass.Result.Schedule.Status status;
        }  
        public List<Schedules> schedules = new List<Schedules>();

    }

    /// <summary>
    /// 获取比分
    /// </summary>
    [Serializable]
    public class GetResultClass
    {
        [Serializable]
        public class Result
        {
            [Serializable]
            public class EventUnit
            {
                public string shortDescription;
            }
            [Serializable]
            public class Schedule
            {
                [Serializable]
                public class Status
                {
                    public string code;
                }

                public string startDate;
                public Status status;
            }
            [Serializable]
            public class Item
            {
                [Serializable]
                public class Organisation
                {
                    /// <summary>
                    /// 国家英文官方缩写
                    /// </summary>
                    public string code;
                    public string description;
                }
                [Serializable]
                public class Participant
                {
                    public string shortName;
                    public Organisation organisation;
                }
                public string resultWLT;
                public string resultData;
                public Participant participant;
            }
            
            
            public EventUnit eventUnit;
            public Schedule schedule;
            public List<Item> items = new List<Item>();
        }
        public Result results=new Result();


    }
    [Serializable]
    public class ResultCombineJsonUse
    {
        public List<ResultCombine> resultCombines = new List<ResultCombine>();
    }
    [Serializable]
    public class ResultCombine
    {
        [Serializable]
        public class NewMatchResult
        {
            public string startDate;
            public Competitor competitor1, competitor2;
        }
        public string stateName;
        public List<NewMatchResult> results = new List<NewMatchResult>();

    }


}
