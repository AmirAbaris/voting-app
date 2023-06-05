using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PresidentCandidateController : ControllerBase
{
    private readonly IMongoCollection<PresidentCandidate> _collection;

    // Dependency Injection
    public PresidentCandidateController(IMongoClient client, IMongoDbSettings dbSettings)
    {
        var dbName = client.GetDatabase(dbSettings.DatabaseName);
        _collection = dbName.GetCollection<PresidentCandidate>("president-candidates");
    }

    // CRUD
    [HttpPost("register-candidate")]
    public async Task<ActionResult<PresidentCandidate>> RegisterCandidate(PresidentCandidate candidate)
    {
        // Check if candidate exists
        var candidateExists = await _collection.FindAsync<PresidentCandidate>(c =>
        c.CandidateNationalId == candidate.CandidateNationalId.Trim());

        if (candidateExists.Any())
        {
            return BadRequest("Candidate already exist");
        }

        // Create president candidate
        var newCandidate = new PresidentCandidate(
            CandidateId: null,
            CandidateNationalId: candidate.CandidateNationalId,
            FirstName: candidate.FirstName,
            LastName: candidate.LastName,
            Age: candidate.Age,
            Gender: candidate.Gender,
            Party: candidate.Party,
            Address: new Address(
                City: candidate.Address.City,
                State: candidate.Address.State,
                Street: candidate.Address.Street,
                ZipCode: candidate.Address.ZipCode
            )
        );

        // Insert candidate into database
        await _collection.InsertOneAsync(newCandidate);

        // Return created candidate
        return Created("", newCandidate);
    }

    [HttpGet("get-candidate-by-national-id/{nationalId}")]
    public async Task<ActionResult<PresidentCandidate>> GetCandidateByNationalId(string nationalId)
    {
        // Check if candidate exists
        var candidate = await _collection.Find(c =>
        c.CandidateNationalId == nationalId.Trim()).FirstOrDefaultAsync();

        if (candidate == null)
        {
            return NotFound("no matching candidate found");
        }

        // Return candidate
        return Ok(candidate);
    }

    [HttpGet("get-candidate-by-id/{id}")]
    public async Task<ActionResult<PresidentCandidate>> GetCandidateById(string id)
    {
        // Check if candidate exists
        var candidate = await _collection.Find(c => c.CandidateId == id.Trim()).FirstOrDefaultAsync();

        if (candidate == null)
        {
            return NotFound("no matching candidate found");
        }

        // Return candidate
        return Ok(candidate);
    }

    [HttpGet("get-candidates")]
    public async Task<ActionResult<IEnumerable<PresidentCandidate>>> GetAllCandidates()
    {
        // Creating a list for candidates
        List<PresidentCandidate> candidates = await _collection.Find(new BsonDocument()).ToListAsync();

        // Returning candidates
        return Ok(candidates);
    }

    [HttpPatch("update-candidate-by-national-id/{nationalId}")]
    public async Task<ActionResult<UpdateResult>> UpdateCandidate(string nationalId, PresidentCandidate candidate)
    {
        // Check if candidate exists
        var candidateExists = await _collection.Find(c => c.CandidateNationalId == nationalId.Trim()).FirstOrDefaultAsync();

        if (candidateExists == null)
        {
            return NotFound("no matching candidate found");
        }

        // Update candidate 
        var updateResult = Builders<PresidentCandidate>.Update
        .Set(c => c.FirstName, candidate.FirstName)
        .Set(c => c.LastName, candidate.LastName)
        .Set(c => c.Age, candidate.Age)
        .Set(c => c.Gender, candidate.Gender)
        .Set(c => c.Party, candidate.Party)
        .Set(c => c.Address, candidate.Address);

        // Update candidate in database
        await _collection.UpdateOneAsync(p => p.CandidateNationalId == nationalId, updateResult);

        // Return update result
        return Ok(updateResult);
    }

    [HttpDelete("delete-candidate-by-national-id/{nationalId}")]
    async public Task<ActionResult<DeleteResult>> DeleteCandidate(string nationalId)
    {
        // Valid input
        if (string.IsNullOrWhiteSpace(nationalId))
        {
            return BadRequest("National ID must not be null or empty");
        }

        // Delete candidate
        DeleteResult result = await _collection.DeleteOneAsync<PresidentCandidate>(c => c.CandidateNationalId == nationalId.Trim());

        // Check if candidate exists
        if (result.DeletedCount == 0)
        {
            return NotFound("no matching candidate found");
        }

        // Return delete result
        return Ok(result);
    }

}
