using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VoterController : ControllerBase
{
    private readonly IMongoCollection<Voter> _voterCollection;
    private readonly IMongoCollection<PresidentCandidate> _candidateCollection;

    // Dependency Injection
    public VoterController(IMongoClient client, IMongoDbSettings dbSettings)
    {
        var dbName = client.GetDatabase(dbSettings.DatabaseName);
        _voterCollection = dbName.GetCollection<Voter>("voters");
        _candidateCollection = dbName.GetCollection<PresidentCandidate>("president-candidates");
    }

    [HttpPost("register-voter")]
    async public Task<ActionResult<Voter>> RegisterVoter(Voter voter)
    {
        // Check if the voter already exists
        var existingVoter = await _voterCollection.FindAsync<Voter>(v =>
        v.VoterNationalId == voter.VoterNationalId.Trim());

        if (existingVoter.Any())
        {
            return BadRequest("Voter already exist");
        }

        // Check if the candidate exists by Id
        var existingCandidateById = await _candidateCollection.FindAsync<PresidentCandidate>(c =>
        c.CandidateId == voter.SelectedPresidentById);

        if (!existingCandidateById.Any())
        {
            return BadRequest("found no candidate by this ID");
        }

        // Check if candidate exist by national Id
        var existingCandidateByNationalId = await _candidateCollection.FindAsync<PresidentCandidate>(c =>
        c.CandidateNationalId == voter.SelectedPresidentNationalId);

        if (!existingCandidateByNationalId.Any())
        {
            return BadRequest("found no candidate by this National ID");
        }

        // Create a new voter
        var newVoter = new Voter(
            VoterId: null,
            VoterNationalId: voter.VoterNationalId,
            SelectedPresidentById: voter.SelectedPresidentById,
            SelectedPresidentNationalId: voter.SelectedPresidentNationalId,
            FirstName: voter.FirstName,
            LastName: voter.LastName,
            Gender: voter.Gender,
            Age: voter.Age,
            Address: new Address(
                City: voter.Address.City,
                State: voter.Address.State,
                Street: voter.Address.Street,
                ZipCode: voter.Address.ZipCode
            )
        );

        // Insert voter into database
        await _voterCollection.InsertOneAsync(newVoter);

        // Return created voter
        return Created("", newVoter);
    }

    [HttpGet("get-voters-by-candidate-national-id/{nationalId}")]
    public async Task<ActionResult<IEnumerable<Voter>>> GetVoterByCandidateNationalId(string nationalId)
    {
        // Check if voters exist
        List<Voter> voters = await _voterCollection.Find(v => v.SelectedPresidentNationalId == nationalId.Trim()).ToListAsync();

        if (!voters.Any())
        {
            return NotFound("No matching voters found");
        }

        // Return voters
        return Ok(voters);
    }

    [HttpGet("get-voters-by-candidate-id/{id}")]
    public async Task<ActionResult<IEnumerable<Voter>>> GetVoterByCandidateId(string id)
    {
        // Check if voters exist
        List<Voter> voters = await _voterCollection.Find(v => v.SelectedPresidentById == id.Trim()).ToListAsync();

        if (!voters.Any())
        {
            return NotFound("No matching voters found");
        }

        // Return voters with updated counter
        return Ok(voters);
    }

    [HttpGet("get-all-voters")]
    public async Task<ActionResult<IEnumerable<Voter>>> GetAllVoters()
    {
        // Retrieve all voters from database
        List<Voter> voters = await _voterCollection.Find<Voter>(new BsonDocument()).ToListAsync();

        // Return voters
        return Ok(voters);
    }

}
