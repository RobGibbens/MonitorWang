using System;
using System.Configuration;
using System.Windows.Forms;
using MonitorWang.Core.AppStats;

namespace MonitorWang.AppStats.Demo
{
    public partial class AppStatsDemoForm : Form
    {
        private AppStatsEngine.AppStatsEventTimer myTimer;

        public AppStatsDemoForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            uiDestinationQueue.Text = ConfigurationManager.AppSettings["queue"];
            uiTimerStatId.Text = "TestTimerKpi";
            uiCountStatId.Text = "TestCounterKpi";
        }

        private void uiTimerStartStop_Click(object sender, EventArgs e)
        {
            if (uiTimerStartStop.Text == "Timer Start")
            {
                // starts timing
                // usually you would time something inline like this
                using (var timer = AppStatsEngine.Time("SomeOperation"))
                {
                    // some operation to time
                    // the appstat is automatically published
                    // when it is disposed
                }              
                myTimer = AppStatsEngine.Time(uiTimerStatId.Text);
                uiTimerStartStop.Text = "Timer Stop";
            }
            else
            {
                // stops timing
                // this will flush the timing data to the bus
                myTimer.Dispose();
                myTimer = null;
                uiTimerStartStop.Text = "Timer Start";
            }
        }

        private void uiCount_Click(object sender, EventArgs e)
        {
            // example of a simple count
            AppStatsEngine.One(uiCountStatId.Text);
        }

        private void uiCountTen_Click(object sender, EventArgs e)
        {
            // example of a simple count with a context, this 
            // appears in the tag property of the healthcheck
            AppStatsEngine.Count(10, uiCountStatId.Text, "@{0}", DateTime.UtcNow);
        }

        private void uiVote_Click(object sender, EventArgs e)
        {
            var segmentId = "Don't know";

            if (uiVoteA.Checked)
                segmentId = uiVoteA.Text;
            else if (uiVoteB.Checked)
                segmentId = uiVoteB.Text;
            else if (uiVoteC.Checked)
                segmentId = uiVoteC.Text;

            // using the piechart extension allows us to set up the
            // underlying result properties in a more natural way.
            // We can then use the Geckoboard Data Service to visualise
            // this data with a Geckoboard piechart widget.
            //
            // The MonitorWang Geckoboard Data Service url to use in 
            // your geckoboard widget is...
            // 
            AppStatsEngine.Publish(new AppStatsEvent()
                                       .PieChart("MonitorWangPoll")
                                       .Segment(segmentId)
                                       .One());
        }
    }
}