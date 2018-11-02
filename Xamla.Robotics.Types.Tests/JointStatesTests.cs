using Xunit;

namespace Xamla.Robotics.Types.Tests
{
    public class JointStatesTests
    {
        [Fact]
        public void TestInit()
        {

            var pos = new JointValues(new JointSet("a", "b", "c"), new double[] { 2, 3, 4 });
            var vel = new JointValues(new JointSet("a", "b", "c"), new double[] { 2, 3, 4 });
            var eff = new JointValues(new JointSet("a", "b", "c"), new double[] { 2, 3, 4 });

            var js = new JointStates(pos, vel, eff);
            Assert.NotNull(js.JointSet);

            Assert.Null((new JointStates(null, null, null)).JointSet);
            // all jointset must match
            var badEff = new JointValues(new JointSet("f", "b", "c"), new double[] { 2, 3, 4 });
            Assert.Throws<System.Exception>(() => new JointStates(pos, vel, badEff));

        }
    }
}