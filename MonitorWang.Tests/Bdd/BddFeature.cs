using StoryQ;

namespace MonitorWang.Tests.Bdd
{
    /// <summary>
    /// The purpose of this class is to provide a single feature per spec class - it saves having
    /// to repeat the feature per scenario
    /// </summary>
    public abstract class BddFeature
    {
        private Feature myFeature;

        protected Feature Feature
        {
            get
            {
                return (myFeature ?? (myFeature = DescribeFeature()));
            }
        }

        protected abstract Feature DescribeFeature();
    }
}