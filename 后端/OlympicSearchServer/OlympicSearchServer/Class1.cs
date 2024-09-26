using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Http;
using static OlympicSearchServer.DateMatchDetails;
using static OlympicSearchServer.DateMatchDetails.Units.Competitors;
using static OlympicSearchServer.MatchNameData.Props.PageProps.InitialFilterDisciplines;

namespace OlympicSearchServer
{
    /// <summary>
    /// 创建数据使用，非服务器运行时调用
    /// </summary>
    public class DataPraser
    {
        //服务器使用
        //C:\Users\Administrator\Documents\GitHub\222200413_222200415\后端\OlympicSearchServer\Resources
        public static string CrawlerDataPath = "C:\\Users\\Administrator\\Documents\\GitHub\\222200413_222200415\\后端\\OlympicSearchServer\\Resources\\CrawlerData\\crawlerData.json";
        public static string MatchNameDataPath = "C:\\Users\\Administrator\\Documents\\GitHub\\222200413_222200415\\后端\\OlympicSearchServer\\Resources\\CrawlerData\\matchNameData.json";
        public static string NationalMedalsPath = "C:\\Users\\Administrator\\Documents\\GitHub\\222200413_222200415\\后端\\OlympicSearchServer\\Resources\\CrawlerData\\nationalMedals.json";

        //public static string TestDayResultPath = "C:/Users/nigulasirui/Desktop/软工实践作业/222200415/第四次作业/testDayResult.json";

        public static string ResourcesPath = "C:\\Users\\Administrator\\Documents\\GitHub\\222200413_222200415\\后端\\OlympicSearchServer\\Resources";
        public static string DayResultSavePath = "C:\\Users\\Administrator\\Documents\\GitHub\\222200413_222200415\\后端\\OlympicSearchServer\\Resources\\DayResult";



        //调试使用
        //public static string CrawlerDataPath = "C:/Users/nigulasirui/Desktop/软工实践作业/222200415/第四次作业/crawlerData.json";
        //public static string MatchNameDataPath = "C:/Users/nigulasirui/Desktop/软工实践作业/222200415/第四次作业/matchNameData.json";
        //public static string NationalMedalsPath = "C:/Users/nigulasirui/Desktop/软工实践作业/222200415/第四次作业/nationalMedals.json";

        //public static string TestDayResultPath = "C:/Users/nigulasirui/Desktop/软工实践作业/222200415/第四次作业/testDayResult.json";

        //public static string ResourcesPath = "C:/Users/nigulasirui/Desktop/软工实践作业/222200415/第四次作业/OlympicSearchServer/Resources";
        //public static string DayResultSavePath = "C:/Users/nigulasirui/Desktop/软工实践作业/222200415/第四次作业/OlympicSearchServer/Resources/DayResult";

        public static string GetBracketTableHttp(string matchID)
        {
            return $"https://olympics.com/OG2024/data/GLO_Bracket~comp=OG2024~rsc={matchID}~lang=CHI.json";
        }
        public static string GetDailyFixturesHttp(string date)
        {
            return $"https://sph-s-api.olympics.com/summer/schedules/api/CHI/schedule/day/{date}";
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
                                matchResult.competitor1 = new BattleTableData.BattleResult.MatchResult.Competitor(bracketCompetitors.participant.organisation.code, bracketCompetitors.participant.organisation.description, bracketCompetitors.cp_result, isWinner);
                            else matchResult.competitor1 = new BattleTableData.BattleResult.MatchResult.Competitor("", "轮空", "", false);
                            //参赛二
                            bracketCompetitors = c.bracketCompetitors[1];
                            isWinner = (bracketCompetitors.cp_wlt.Equals("W"));
                            if (bracketCompetitors.participant != null)
                                matchResult.competitor2 = new BattleTableData.BattleResult.MatchResult.Competitor(bracketCompetitors.participant.organisation.code, bracketCompetitors.participant.organisation.description, bracketCompetitors.cp_result, isWinner);
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
        public Result<List<string>> GetAllMatchName()
        {
            Result<List<string>> result = new Result<List<string>>();
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
        public Result<List<MatchDetailName>> GetAllMatchDetailName(string firstName)
        {
            Result<List<MatchDetailName>> result = new Result<List<MatchDetailName>>();
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
        public Result<List<NationalMeadls>> GetAllNationalMedals()
        {
            Result<List<NationalMeadls>> result = new Result<List<NationalMeadls>>();
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
        Dictionary<string, Result<List<Units>>> saveDayResult = new Dictionary<string, Result<List<Units>>>();
        List<StringIntPair> useCheck = new List<StringIntPair>();
        [HttpGet]
        public Result<List<Units>> GetDayResult(string Date)
        {
            string savePath = Path.Combine(DataPraser.DayResultSavePath, Date + ".json");
            Result<List<Units>> result = new Result<List<Units>>();
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
                    string jsonData = DataPraser.GetHttpJson(httpPath);
                    data = JsonConvert.DeserializeObject<DateMatchDetails>(jsonData);

                    foreach (var a in data.units)
                    {
                        a.startDate = a.startDate.Substring(11, 5);
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
        public Result<BattleTableData> GetBattleTable(string id)
        {
            Result<BattleTableData> result = new Result<BattleTableData>();
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
                    public string countryCN;
                    public string score;
                    public bool isWinner;
                    public Competitor(string countryEN, string countryCN, string score, bool isWinner)
                    {
                        this.countryEN = countryEN;
                        this.countryCN = countryCN;
                        this.score = score;
                        this.isWinner = isWinner;
                    }
                    public void Show()
                    {
                        Console.WriteLine($"{countryCN} {score} {isWinner}");
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
    public class Result<T>
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



}
