using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.SelfHost;
using System.Web.Http;
using System.Web.Http.Cors;
using OlympicSearchServer;

namespace StudentDB_Scripts
{
    internal class Program
    {
        static void Main(string[] args)
        {

            //// 7595 是端口，可以改成喜欢的
            //var config = new HttpSelfHostConfiguration("http://localhost:7595");
            //config.EnableCors();
            //config.EnableCors(new EnableCorsAttribute("*", "*", "*"));
            //config.Formatters.XmlFormatter.SupportedMediaTypes.Clear();

            //// 定义路由，这里不需要改
            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",  // 可以改成喜欢的名字，xxxApi
            //    routeTemplate: "api/{controller}/{action}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

            //var server = new HttpSelfHostServer(config);
            //server.OpenAsync().Wait();  // 启动服务器

            //Console.WriteLine("服务器已启动，按回车停止。");

            //Console.ReadLine();


            //DataPraser mid=new DataPraser();
            //mid.ReadAllMatchData();
            //mid.ReadAllNationalMedalDetails();
            ///测试获取http上的json数据
            DataGetController test = new();
            //test.GetDayResult("2024-08-01");
            //test.GetDayResult("2024-08-02");
            //test.GetDayResult("2024-08-03");
            //test.GetDayResult("2024-08-04");
            //test.GetDayResult("2024-08-05");
            //test.GetDayResult("2024-07-24");
            //test.GetDayResult("2024-08-04");
            //test.TestBracketValue();
            DataPraser.GetBattleTable("FBLMTEAM11------------------------");
        }

    }
}
