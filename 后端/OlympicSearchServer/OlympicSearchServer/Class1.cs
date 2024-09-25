using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Web.Http;
using static OlympicSearchServer.DateMatchDetails;
using static OlympicSearchServer.MatchNameData.Props.PageProps.InitialFilterDisciplines;

namespace OlympicSearchServer
{
    /// <summary>
    /// 创建数据使用，非服务器运行时调用
    /// </summary>
    public class DataPraser
    {
        public static string CrawlerDataPath = "C:/Users/nigulasirui/Desktop/软工实践作业/222200415/第四次作业/crawlerData.json";
        public static string MatchNameDataPath = "C:/Users/nigulasirui/Desktop/软工实践作业/222200415/第四次作业/matchNameData.json";
        public static string NationalMedalsPath = "C:/Users/nigulasirui/Desktop/软工实践作业/222200415/第四次作业/nationalMedals.json";

        public static string TestDayResultPath = "C:/Users/nigulasirui/Desktop/软工实践作业/222200415/第四次作业/testDayResult.json";

        public static string ResourcesPath = "C:/Users/nigulasirui/Desktop/软工实践作业/222200415/第四次作业/OlympicSearchServer/Resources";
        public static string DayResultSavePath = "C:/Users/nigulasirui/Desktop/软工实践作业/222200415/第四次作业/OlympicSearchServer/Resources/DayResult";

        public static string GetBracketTableHttp(string matchID)
        {
            return $"https://olympics.com/OG2024/data/GLO_Bracket~comp=OG2024~rsc={matchID}~lang=CHI.json";
        }
        public static string GetDailyFixturesHttp(string date)
        {
            return $"https://sph-s-api.olympics.com/summer/schedules/api/CHI/schedule/day/{date}";
        }
        public static string GetBracketHttp(string id)
        {
            return $"https://olympics.com/OG2024/data/GLO_Bracket~comp=OG2024~rsc={id}~lang=CHI.json";
        }
        public void ReadAllMatchData()
        {
            MatchNameData data = JsonConvert.DeserializeObject<MatchNameData>(File.ReadAllText(CrawlerDataPath));
            Dictionary<string, Disciplines> allMatchData = new();
            foreach (var a in data.props.pageProps.initialFilterDisciplines.disciplines)
            {
                allMatchData.Add(a.name, a);
            }
            File.WriteAllText(MatchNameDataPath, JsonConvert.SerializeObject(allMatchData));
        }
        public void ReadAllNationalMedalDetails()
        {
            NationalMedalDetails data = JsonConvert.DeserializeObject<NationalMedalDetails>(File.ReadAllText(CrawlerDataPath));
            List<NationalMeadls> allNationalData = new();
            foreach (var a in data.props.pageProps.initialMedals.medalStandings.medalsTable)
            {
                foreach (var b in a.medalsNumber)
                {
                    if (b.type.Equals("Total"))
                    {
                        NationalMeadls mid = new();
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


    }
    public class DataGetController : ApiController
    {
        public List<NationalMeadls> allNationalMedals = new();
        Dictionary<string, Disciplines> allMatchData = new();
        public List<string> allMatchFirstName = new();
        public DataGetController()
        {
            allNationalMedals = JsonConvert.DeserializeObject<List<NationalMeadls>>(File.ReadAllText(DataPraser.NationalMedalsPath));
            allMatchData = JsonConvert.DeserializeObject<Dictionary<string, Disciplines>>(File.ReadAllText(DataPraser.MatchNameDataPath));
            if (allMatchData != null) allMatchFirstName = allMatchData.Keys.ToList();
            else Console.WriteLine("数据初始化错误！");
        }

        /// <summary>
        /// 获取所有比赛名称
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public Result<List<string>> GetAllMatchName()
        {
            Result<List<string>> result = new();
            if (allMatchFirstName.Count == 0)
            {
                result.code = 0;
                result.message = "比赛名称数据为空，为服务器数据出错！";
            }
            else result.data = allMatchData.Keys.ToList();
            return result;
        }
        /// <summary>
        /// 获取所有比赛详细名称
        /// </summary>
        /// <param name="firstName"></param>
        /// <returns></returns>
        [HttpGet]
        public Result<List<string>> GetAllMatchDetailName(string firstName)
        {
            Result<List<string>> result = new();
            if (allMatchData.ContainsKey(firstName))
            {
                result.data = allMatchData[firstName].events.Select(x => x.name).ToList();
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
            Result<List<NationalMeadls>> result = new();
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
        Dictionary<string, Result<List<Units>>> saveDayResult=new();
        List<StringIntPair> useCheck = new();
        [HttpGet]
        public Result<List<Units>> GetDayResult(string Date)
        {
            string savePath = Path.Combine(DataPraser.DayResultSavePath, Date + ".json");
            Result<List<Units>> result = new();
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
                    useCheck.Add(new StringIntPair(Date,5));
                }
                result.data = data.units;

            }
            //清理
            foreach(var a in useCheck)
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

    }

    public class BattleTable
    {




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
                /// 根据这个的数量判断是半决赛，四分之一决赛还是奖牌赛
                /// </summary>
                public List<BracketItems> bracketItems;



            }
            public List<BracketPhases> bracketPhases;




        }
        public List<Bracket> bracket;

    }



}
