using Soenneker.Tests.HostedUnit;

namespace Soenneker.Cron.Builder.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public class CronBuilderTests : HostedUnitTest
{
    public CronBuilderTests(Host host) : base(host)
    {
    }

    [Test]
    public void Default()
    {

    }
}
