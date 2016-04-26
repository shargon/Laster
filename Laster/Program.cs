using Laster.Core.Classes.Collections;
using Laster.Core.Classes.RaiseMode;
using Laster.Inputs;
using Laster.Inputs.Http;
using Laster.Inputs.Twitter;
using Laster.Inputs.Twitter.Enums;
using Laster.Outputs;
using Laster.Process;
using System;
using System.ServiceProcess;
using System.Windows.Forms;

namespace Laster
{
    /*
     Input  -> Process -> Process -> Output
            -> Output  -> Process
                       -> Exit
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


            DataInputCollection inputs = new DataInputCollection();
            RSSInput rss1 = new RSSInput()
            {
                Credentials = null,
                Url = new Uri("http://feeds.feedburner.com/cuantarazon?format=xml"),
                Name = "RSSInput"
            };


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
            };


            rss1.Variables.AddAll(vars.Items);
            twiiter1.Variables.AddAll(vars.Items);
            twiiter2.Variables.AddAll(vars.Items);
            twiiter3.Variables.AddAll(vars.Items);

            ((DataInputInterval)rss1.RaiseMode).Interval = TimeSpan.FromSeconds(1);
            ((DataInputInterval)twiiter1.RaiseMode).Interval = TimeSpan.FromSeconds(0.1);
            ((DataInputInterval)twiiter2.RaiseMode).Interval = TimeSpan.FromSeconds(0.5);
            ((DataInputInterval)twiiter3.RaiseMode).Interval = TimeSpan.FromSeconds(0.2);

            EmptyProcess emptyByReg = new EmptyProcess() { Name = "EmptyProcess" };

            emptyByReg.Variables.AddAll(vars.Items);

            emptyByReg.Out.Add(new FileOutput() { FileName = "D:\\test_by_reg.txt" });
            emptyByReg.Out.Add(new FileOutput() { FileName = "D:\\test_by_all.txt" });

            rss1.Process.Add(emptyByReg);
            twiiter1.Process.Add(emptyByReg);
            twiiter2.Process.Add(emptyByReg);
            twiiter3.Process.Add(emptyByReg);

            inputs.Add(rss1);
            inputs.Add(twiiter1);
            inputs.Add(twiiter2);
            inputs.Add(twiiter3);


            inputs.Start();

            Application.Run();

            ServiceBase[] ServicesToRun = new ServiceBase[] { new LasterService() };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
