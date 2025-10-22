using Xunit;
using CMCS.Models;
namespace CMCSUnitTests.Tests;

public class ClaimTests
{
    [Fact]
    public void Amount_Computes_HoursTimesRate()
    {
        var c = new Claim { HoursWorked = 10, HourlyRate = 15.50m };
        Assert.Equal(155.00m, c.Amount);
    }

    [Fact]
    public void Amount_Zero_When_NoHoursOrRate()
    {
        var c = new Claim { HoursWorked = 0, HourlyRate = 0m };
        Assert.Equal(0m, c.Amount);
    }
}