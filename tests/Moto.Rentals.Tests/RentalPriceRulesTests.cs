using Moto.Rentals.Api.Domain;
using Xunit;

public class RentalPriceRulesTests
{
    [Fact]
    public void Early_Return_7_Days_Has_20Percent_Penalty()
    {
        var start = new DateOnly(2025, 1, 2);
        var expected = start.AddDays(6);
        var ret = start.AddDays(2);
        var (used, remaining, extra, total) = RentalPricingRules.CalculateReturnTotal(start, expected, ret, 7, 30m);
        Assert.Equal(3, used);
        Assert.Equal(4, remaining);
        // base: 3*30=90; multa: 4*30*0.2=24 => total 114
        Assert.Equal(114m, total);
        Assert.Equal(0, extra);
    }
}
