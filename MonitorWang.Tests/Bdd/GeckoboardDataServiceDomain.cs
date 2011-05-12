using MonitorWang.Core.Geckoboard;
using MonitorWang.Core.Geckoboard.DataProvider;

namespace MonitorWang.Tests.Bdd
{
    public abstract class GeckoboardDataServiceDomain : BddTestDomain
    {
        protected IGeckoboardDataServiceImpl myActivity;
        protected IGeckoboardDataProvider myDataProvider;
        protected IColourPicker myColourPicker;

        protected GeckoboardDataServiceDomain()
        {
            myColourPicker = new DefaultColourPicker();
        }
    }
}