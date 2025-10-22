using System;
using Xunit;
using CMCS.Services;
using CMCS.Models;
using System.Linq;

namespace CMCSUnitTests.Tests;

public class InMemoryClaimRepositoryTests
{
    [Fact]
    public void AddAndGet_Workflow()
    {
        var repo = new InMemoryClaimRepository();
        repo.Clear();
        var c = new Claim { Lecturer = "l1", HoursWorked = 2, HourlyRate = 10m };
        repo.Add(c);

        var fetched = repo.Get(c.Id);
        Assert.NotNull(fetched);
        Assert.Equal(c.Lecturer, fetched?.Lecturer);
        Assert.Equal(20m, fetched?.Amount);

        var byLecturer = repo.GetByLecturer("l1").ToList();
        Assert.Contains(byLecturer, x => x.Id == c.Id);

        Assert.True(repo.UpdateStatus(c.Id, ClaimStatus.Approved));
        var updated = repo.Get(c.Id);
        Assert.Equal(ClaimStatus.Approved, updated?.Status);

        repo.Clear();
        Assert.Empty(repo.GetAll());
    }
}