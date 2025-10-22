
using CMCS.Models;
using System.Collections.Concurrent;
namespace CMCS.Services;
public class InMemoryClaimRepository : IClaimRepository
{
    private readonly ConcurrentDictionary<Guid, Claim> _claims = new();
    public IEnumerable<Claim> GetAll() => _claims.Values.OrderByDescending(c => c.SubmittedAt);
    public IEnumerable<Claim> GetByLecturer(string lecturer) => _claims.Values.Where(c => c.Lecturer.Equals(lecturer, StringComparison.OrdinalIgnoreCase));
    public Claim? Get(Guid id) => _claims.TryGetValue(id, out var c) ? c : null;
    public void Add(Claim claim) { _claims[claim.Id] = claim; }
    public bool UpdateStatus(Guid id, ClaimStatus status) { if (_claims.TryGetValue(id, out var claim)) { claim.Status = status; _claims[id] = claim; return true; } return false; }
    public void Clear() => _claims.Clear();
}
