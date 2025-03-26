using Soenneker.Tests.FixturedUnit;
using Xunit;

namespace Soenneker.Cron.Builder.Tests;

[Collection("Collection")]
public class CronBuilderTests : FixturedUnitTest
{
    public CronBuilderTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
    }

    [Fact]
    public void Default()
    {

    }
}
