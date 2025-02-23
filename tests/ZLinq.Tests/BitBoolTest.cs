using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZLinq.Internal;

namespace ZLinq.Tests;

public class BitBoolTest
{
    [Fact]
    public void Test()
    {
        BitBool b = default;

        b.IsZero.ShouldBeTrue();
        b.IsBit1.ShouldBeFalse();
        b.IsBit2.ShouldBeFalse();
        b.IsBit3.ShouldBeFalse();
        b.IsBit4.ShouldBeFalse();
        b.IsBit5.ShouldBeFalse();
        b.IsBit6.ShouldBeFalse();
        b.IsBit7.ShouldBeFalse();
        b.IsBit8.ShouldBeFalse();

        b.SetFalseToBit1();
        b.IsZero.ShouldBeTrue();

        b.SetTrueToBit1();
        b.IsZero.ShouldBeFalse();
        b.IsBit1.ShouldBeTrue();

        b.SetTrueToBit2();
        b.IsBit2.ShouldBeTrue();

        b.SetTrueToBit3();
        b.IsBit3.ShouldBeTrue();

        b.SetTrueToBit4();
        b.IsBit4.ShouldBeTrue();

        b.SetTrueToBit5();
        b.IsBit5.ShouldBeTrue();

        b.SetTrueToBit6();
        b.IsBit6.ShouldBeTrue();

        b.SetTrueToBit7();
        b.IsBit7.ShouldBeTrue();

        b.SetTrueToBit8();
        b.IsBit8.ShouldBeTrue();

        b.SetFalseToBit1();
        b.IsBit1.ShouldBeFalse();

        b.SetFalseToBit2();
        b.IsBit2.ShouldBeFalse();

        b.SetFalseToBit3();
        b.IsBit3.ShouldBeFalse();

        b.SetFalseToBit4();
        b.IsBit4.ShouldBeFalse();

        b.SetFalseToBit5();
        b.IsBit5.ShouldBeFalse();

        b.SetFalseToBit6();
        b.IsBit6.ShouldBeFalse();

        b.SetFalseToBit7();
        b.IsBit7.ShouldBeFalse();

        b.SetFalseToBit8();
        b.IsBit8.ShouldBeFalse();

        b.IsZero.ShouldBeTrue();

        b.SetTrueToBit4();
        b.IsBit4.ShouldBeTrue();
    }
}
