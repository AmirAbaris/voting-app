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

    [HttpPost("register-candidate")]
    public async Task<ActionResult<PresidentCandidate>> RegisterCandidate(PresidentCandidate candidate)
    {
        // Check if candidate already exists
        var existingCandidates = await _collection.FindAsync<PresidentCandidate>(c =>
            c.CandidateNationalId == candidate.CandidateNationalId.Trim());
        if (existingCandidates.Any())
        {
            return BadRequest("Candidate already exist");
        }

        // Create a new president candidate
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

        // Insert created candidate into database
        await _collection.InsertOneAsync(newCandidate);

        // Return created candidate
        return Created("", newCandidate);
    }

    [HttpGet("get-candidate-by-national-id/{nationalId}")]
    public async Task<ActionResult<PresidentCandidate>> GetCandidateByNationalId(string nationalId)
    {
        // Check if candidate exists
        var candidate = await _collection.Find<PresidentCandidate>(c =>
            c.CandidateNationalId == nationalId.Trim()).FirstOrDefaultAsync();
        if (candidate is null)
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
        var candidate = await _collection.Find<PresidentCandidate>(c =>
            c.CandidateId == id.Trim()).FirstOrDefaultAsync();
        if (candidate is null)
        {
            return NotFound("no matching candidate found");
        }

        // Return candidate
        return Ok(candidate);
    }

    [HttpGet("get-candidates")]
    public async Task<ActionResult<IEnumerable<PresidentCandidate>>> GetAllCandidates()
    {
        // Retrive candidates
        List<PresidentCandidate> candidates = await _collection.Find(new BsonDocument()).ToListAsync();

        // Return all candidates
        return Ok(candidates);
    }

    [HttpPut("update-candidate-by-national-id/{nationalId}")]
    public async Task<ActionResult<UpdateResult>> UpdateCandidate(string nationalId, PresidentCandidate candidateIn)
    {
        // Check if candidate exists
        List<PresidentCandidate> candidateExists = await _collection.Find(c =>
            c.CandidateNationalId == nationalId.Trim()).ToListAsync();
        if (!candidateExists.Any())
        {
            return NotFound("No matching candidate found");
        }

        // Update candidate 
        var updateCanidate = Builders<PresidentCandidate>.Update
            .Set(c => c.FirstName, candidateIn.FirstName)
            .Set(c => c.LastName, candidateIn.LastName)
            .Set(c => c.Age, candidateIn.Age)
            .Set(c => c.Gender, candidateIn.Gender)
            .Set(c => c.Party, candidateIn.Party)
            .Set(c => c.Address, candidateIn.Address);

        // Remove CandidateNationalId from updateCanidate
        // * users can't update national id due to data security
        updateCanidate = updateCanidate.Unset(c => c.CandidateNationalId);

        // Update candidate in database
        UpdateResult result = await _collection.UpdateOneAsync(candidate =>
            candidate.CandidateNationalId == nationalId, updateCanidate);
        if (result.ModifiedCount > 0)
        {
            return Ok(result);
        }

        return BadRequest("Failed to update candidate");
    }

    [HttpDelete("delete-candidate-by-national-id/{nationalId}")]
    async public Task<ActionResult<DeleteResult>> DeleteCandidate(string nationalId)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(nationalId))
        {
            return BadRequest("National ID should not be null or empty");
        }

        // Delete candidate
        DeleteResult result = await _collection.DeleteOneAsync<PresidentCandidate>(c =>
        c.CandidateNationalId == nationalId.Trim());

        // Check if candidate exists
        if (result.DeletedCount == 0)
        {
            return NotFound("no matching candidate found");
        }

        // Return delete result
        return Ok(result);
    }
}
