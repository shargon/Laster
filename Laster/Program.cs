using Laster.Core.Classes.Collections;
using Laster.Core.Classes.RaiseMode;
using Laster.Core.Helpers;
using Laster.Inputs.DB;
using Laster.Inputs.Http;
using Laster.Outputs;
using Laster.Process;
using System;
using System.ServiceProcess;
using System.Windows.Forms;

namespace Laster
{
    /*
     Input  -> Process -> Process -> Output
            -> Process -> Output
    */
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        static void Main()
        {
            DataVariableCollection vars = new DataVariableCollection();
            //vars.Add("", "");

            RSSInput rss1 = new RSSInput(new DataInputInterval(TimeSpan.FromSeconds(1)))
            {
                Url = new Uri("http://feeds.feedburner.com/cuantarazon?format=xml"),
                Name = "RSSInput"
            };

            DBInput db1 = new DBInput(new DataInputInterval(TimeSpan.FromSeconds(1)))
            {
                ExecuteMode = DBInput.EExecuteMode.ArrayWithHeader,
                ConnectionString = "Server=localhost;Uid=root;Pwd=885551;Port=3306",
                SqlQuery = "SHOW PROCESSLIST"
            };

            /*
            TwitterHomeInput twiiter1 = new TwitterHomeInput(TimeSpan.FromSeconds(10))
            {
                ConsumerKey = "1",
                ConsumerSecret = "2",

                AccessToken = "3",
                AccessTokenSecret = "4",
                Name = "TwitterHomeInput",
            };
            TwitterSearchInput twiiter2 = new TwitterSearchInput(TimeSpan.FromSeconds(10))
            {
                ConsumerKey = twiiter1.ConsumerKey,
                ConsumerSecret = twiiter1.ConsumerSecret,

                AccessToken = twiiter1.AccessToken,
                AccessTokenSecret = twiiter1.AccessTokenSecret,
                Name = "TwitterSearchInput",

                Query = "Clash of clans"
            };
            TwitterFollowersInput twiiter3 = new TwitterFollowersInput(TimeSpan.FromSeconds(10))
            {
                ConsumerKey = twiiter1.ConsumerKey,
                ConsumerSecret = twiiter1.ConsumerSecret,

                AccessToken = twiiter1.AccessToken,
                AccessTokenSecret = twiiter1.AccessTokenSecret,
                Name = "TwitterSearchInput",

                ScreenName = "@VenezuelaClan",
                IncludeUserEntities = true,
                Type = ETwitterFollowType.Him
            };*/


            rss1.Variables.AddAll(vars);
            //twiiter1.Variables.AddAll(vars);
            //twiiter2.Variables.AddAll(vars);
            //twiiter3.Variables.AddAll(vars);

            EmptyProcess emptyByReg = new EmptyProcess() { Name = "EmptyProcess", WaitForFull = true };

            emptyByReg.Variables.AddAll(vars);

            emptyByReg.Out.Add(new FileOutput() { FileName = "E:\\test_by_json.txt", Format = SerializationHelper.EFormat.Json });
            emptyByReg.Out.Add(new FileOutput() { FileName = "E:\\test_by_txt.txt", Format = SerializationHelper.EFormat.ToString });
            emptyByReg.Out.Add(new HttpRestOutput()
            {
                Prefixes = new string[] { "http://127.0.0.1:8080/index/" },
            });

            rss1.Process.Add(emptyByReg);
            db1.Process.Add(emptyByReg);

            //twiiter1.Process.Add(emptyByReg);
            //twiiter2.Process.Add(emptyByReg);
            //twiiter3.Process.Add(emptyByReg);

            DataInputCollection inputs = new DataInputCollection(rss1, db1/*twiiter1,twiiter2,twiiter3*/);
            //inputs.Add(rss1);
            //inputs.Add(twiiter1);
            //inputs.Add(twiiter2);
            //inputs.Add(twiiter3);

            //            inputs.Start();

            Application.Run(new FEditTopology());

            ServiceBase[] ServicesToRun = new ServiceBase[] { new LasterService() };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
