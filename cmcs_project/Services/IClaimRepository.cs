
using CMCS.Models;
namespace CMCS.Services;
public interface IClaimRepository
{
    IEnumerable<Claim> GetAll();
    IEnumerable<Claim> GetByLecturer(string lecturer);
    Claim? Get(Guid id);
    void Add(Claim claim);
    bool UpdateStatus(Guid id, ClaimStatus status);
    void Clear();
}
