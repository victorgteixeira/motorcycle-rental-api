namespace Moto.Rentals.Api.Domain;

public static class RentalPricingRules
{
    public static decimal DailyPriceFor(int planDays) => planDays switch
    {
        7 => 30m,
        15 => 28m,
        30 => 22m,
        45 => 20m,
        50 => 18m,
        _ => throw new ArgumentOutOfRangeException(nameof(planDays), "Invalid plan")
    };

    public static decimal EarlyPenaltyRateFor(int planDays) => planDays switch
    {
        7 => 0.20m,
        15 => 0.40m,
        _ => 0m
    };

    public static (int usedDays, int remainingDays, int extraDays, decimal total) 
        CalculateReturnTotal(DateOnly start, DateOnly expected, DateOnly returnDate, int planDays, decimal daily)
    {
        if (returnDate < start) throw new ArgumentException("Return before start");

        var usedDays = (returnDate.DayNumber - start.DayNumber) + 1;
        var baseUsed = usedDays * daily;

        if (returnDate < expected)
        {
            var remaining = expected.DayNumber - returnDate.DayNumber;
            var penalty = remaining * daily * EarlyPenaltyRateFor(planDays);
            return (usedDays, remaining, 0, Math.Round(baseUsed + penalty, 2));
        }
        else if (returnDate > expected)
        {
            var extra = returnDate.DayNumber - expected.DayNumber;
            var basePlan = planDays * daily;
            var extraFee = extra * 50m;
            return (usedDays, 0, extra, Math.Round(basePlan + extraFee, 2));
        }
        else
        {
            var exact = planDays * daily;
            return (usedDays, 0, 0, Math.Round(exact, 2));
        }
    }
}
