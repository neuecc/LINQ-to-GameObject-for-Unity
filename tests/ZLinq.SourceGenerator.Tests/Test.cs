using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZLinq.Tests;

public class Test(ITestOutputHelper testOutputHelper)
{
    VerifyHelper verifier = new VerifyHelper(testOutputHelper, ""); 

    [Fact]
    public void Foo()
    {
        verifier.Ok("""
using System.Linq;

_ = Enumerable.Range(1, 100).AsValueEnumerable().Select(x => x);
""");



    }
}
