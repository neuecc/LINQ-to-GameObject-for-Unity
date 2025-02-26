namespace ZLinq.Tests;

public static class TestUtil
{
    public static void Throws<T>(Action expectedTestCode, Action actualTestCode)
        where T : Exception
    {
        var expectedException = Assert.Throws<T>(expectedTestCode);
        var actualException = Assert.Throws<T>(actualTestCode);
        expectedException.Message.ShouldBe(actualException.Message);
    }

    public static void NoThrow(Action expectedTestCode, Action actualTestCode)
    {
        expectedTestCode();
        actualTestCode();
    }

    public static IEnumerable<int> Empty()
    {
        yield break;
    }
}
