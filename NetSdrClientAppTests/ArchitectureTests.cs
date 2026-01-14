using NetArchTest.Rules;
using Xunit;
using NetSdrClientApp;

namespace NetSdrClientAppTests
{
    public class ArchitectureTests
    {
        [Fact]
        public void Interfaces_Should_Start_With_I()
        {

            var result = Types.InAssembly(typeof(NetSdrClient).Assembly)
                .That().AreInterfaces()
                .Should().HaveNameStartingWith("I")
                .GetResult();


            Xunit.Assert.True(result.IsSuccessful);
        }

        [Fact]
        public void Networking_Should_Not_Depend_On_Client()
        {

            var result = Types.InAssembly(typeof(NetSdrClient).Assembly)
                .That().ResideInNamespace("NetSdrClientApp.Networking")

                .Should().NotHaveDependencyOn("NetSdrClientApp.NetSdrClient")
                .GetResult();


            Xunit.Assert.True(result.IsSuccessful, "Networking classes should not depend on NetSdrClient class directly.");
        }
    }
}