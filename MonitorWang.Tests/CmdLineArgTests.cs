using System.Linq;
using MonitorWang.Agent;
using NUnit.Framework;

namespace MonitorWang.Tests
{
    [TestFixture]
    public class WhenTheInstallArgIsSupplied
    {
        [TestFixtureSetUp]
        public void Given()
        {
            CmdLine.Init(new[]
                                 {
                                     "/install"
                                 });
        }

        [Test]
        public void ThenTheNumberOfTopshelfArgsShouldBeCorrect()
        {
            Assert.That(CmdLine.Expanded.Count(), Is.EqualTo(1));
        }

        [Test]
        public void ThenTheTopshelfArgsShouldContainTheExpectedItems()
        {
            var tsArgs = CmdLine.Expanded;
            var expectedArg = "install";
            Assert.That(tsArgs.Contains(expectedArg), Is.True, "{0} expanded arg missing", expectedArg);
        }

        [Test]
        public void ThenTheTotalNumberOfArgsShouldBeCorrect()
        {
            Assert.That(CmdLine.All.Count(), Is.EqualTo(2));
        }
    }

    [TestFixture]
    public class WhenTheUninstallArgIsSupplied
    {
        [TestFixtureSetUp]
        public void Given()
        {
            CmdLine.Init(new[]
                                 {
                                     "/uninstall"
                                 });
        }

        [Test]
        public void ThenTheNumberOfTopshelfArgsShouldBeCorrect()
        {
            Assert.That(CmdLine.Expanded.Count(), Is.EqualTo(1));
        }

        [Test]
        public void ThenTheTopshelfArgsShouldContainTheExpectedItems()
        {
            var tsArgs = CmdLine.Expanded;
            var expectedArg = "uninstall";
            Assert.That(tsArgs.Contains(expectedArg), Is.True, "{0} expanded arg missing", expectedArg);
        }

        [Test]
        public void ThenTheTotalNumberOfArgsShouldBeCorrect()
        {
            Assert.That(CmdLine.All.Count(), Is.EqualTo(2));
        }
    }

    [TestFixture]
    public class WhenNoCustomArgsAreSupplied
    {
        [TestFixtureSetUp]
        public void Given()
        {
            CmdLine.Init(new string[0]);
        }

        [Test]
        public void ThenTheNumberOfTopshelfArgsShouldBeZero()
        {
            Assert.That(CmdLine.Expanded.Count(), Is.EqualTo(0));
        }
    }
    
    [TestFixture]
    public class WhenTheProfileCustomArgIsSupplied
    {
        [TestFixtureSetUp]
        public void Given()
        {
            CmdLine.Init(new[]
                                 {
                                     "/profile:testing"
                                 });
        }

        [Test]
        public void ThenTheNumberOfTopshelfArgsShouldBeZero()
        {
            Assert.That(CmdLine.Expanded.Count(), Is.EqualTo(0));
        }

        [Test]
        public void ThenTheNumberOfCustomArgsSuppliedShouldBeCorrect()
        {
            Assert.That(CmdLine.All.Count(), Is.EqualTo(1));
        }

        [Test]
        public void ThenTheProfileShouldBeCorrect()
        {
            string profile;
            var found = CmdLine.Value(CmdLine.SwitchNames.Profile, out profile);

            Assert.That(found, Is.True);
            Assert.That(profile, Is.EqualTo("testing"));
        }
    }
}