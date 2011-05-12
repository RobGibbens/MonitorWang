using System;
using System.Configuration;
using System.Windows.Forms;
using Castle.Windsor;
using MonitorWang.Core.AppStats;
using MonitorWang.Core.Bus;
using NServiceBus;

namespace MonitorWang.AppStats.Demo
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // these are a pretend bus & container
            // to demo where you could use them if you already
            // have these in your application (otherwise AppStats
            // will create it's own instances)
            IBus myBus = null;
            IWindsorContainer myContainer = null;

            AppStatsEngine.Initialise(AppStatsConfigBuilder.For("AppStatsDemo")
                                          // if you already have a bus
                                          //.PublishWith(myBus)

                                          // if you need a bus...
                                          // then use the busbuilder
                                          .PublishWith(BusBuilder.ForApplication()
                                                           // use this if you already have a container 
                                                           // (otherwise the default NSB container is used)
                                                           //.UseContainer(myContainer)
                                                           
                                                           // if you want to customise the msmq settings
                                                           // then use this method to do it otherwise the
                                                           // default queues (non transactional) are used
                                                           //.Msmq("AlternateInput", "AlternateError")

                                                           // that's it - start it!
                                                           .FireItUp(),
                                                       // specify the destination queue for messages
                                                       // here otherwise AppStats will assume you 
                                                       // have configured this routing in external
                                                       // configuration for the Send() method
                                                       ConfigurationManager.AppSettings["queue"])
                                          .Build());

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new AppStatsDemoForm());
        }
    }
}
