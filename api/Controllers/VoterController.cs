namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VoterController : ControllerBase
{
    private readonly IMongoCollection<Voter> _voterCollection;
    private readonly IMongoCollection<PresidentCandidate> _candidateCollection;

    public VoterController(IMongoClient client, IMongoDbSettings dbSettings)
    {
        var dbName = client.GetDatabase(dbSettings.DatabaseName);
        _voterCollection = dbName.GetCollection<Voter>("voters");
        _candidateCollection = dbName.GetCollection<PresidentCandidate>("president-candidates");
    }

    // Register a new voter
    [HttpPost("register-voter")]
    async public Task<ActionResult<Voter>> RegisterVoter(Voter voterToRegister)
    {
        // Check if the voter already exists
        var existingVoter = await _voterCollection.FindAsync<Voter>(v =>
            v.VoterNationalId == voterToRegister.VoterNationalId.Trim());
        if (existingVoter.Any())
        {
            return BadRequest("Voter already exists");
        }

        // Check if candidate ID is valid
        var existingCandidateById = await _candidateCollection.FindAsync<PresidentCandidate>(c =>
            c.CandidateId == voterToRegister.SelectedPresidentById);
        if (!existingCandidateById.Any())
        {
            return BadRequest("No candidate found with this ID");
        }

        // Check if candidate national ID is valid
        var existingCandidateByNationalId = await _candidateCollection.FindAsync<PresidentCandidate>(c =>
            c.CandidateNationalId == voterToRegister.SelectedPresidentNationalId);
        if (!existingCandidateByNationalId.Any())
        {
            return BadRequest("No candidate found with this National ID");
        }

        // Create a new voter object
        var newVoter = new Voter(
            VoterId: null,
            VoterNationalId: voterToRegister.VoterNationalId,
            SelectedPresidentById: voterToRegister.SelectedPresidentById,
            SelectedPresidentNationalId: voterToRegister.SelectedPresidentNationalId,
            FirstName: voterToRegister.FirstName,
            LastName: voterToRegister.LastName,
            Gender: voterToRegister.Gender,
            Age: voterToRegister.Age,
            Address: new Address(
                City: voterToRegister.Address.City,
                State: voterToRegister.Address.State,
                Street: voterToRegister.Address.Street,
                ZipCode: voterToRegister.Address.ZipCode
            )
        );

        // Insert the new voter into the database
        await _voterCollection.InsertOneAsync(newVoter);

        // Return the created voter
        return Created("", newVoter);
    }

    // Get a list of voters who voted for a candidate with the specified national ID
    [HttpGet("get-voters-by-candidate-national-id/{nationalId}")]
    public async Task<ActionResult<IEnumerable<Voter>>> GetVotersByCandidateNationalId(string nationalId)
    {
        // Check if any voters exist for the given candidate national ID
        List<Voter> voters = await _voterCollection.Find(v => v.SelectedPresidentNationalId == nationalId.Trim()).ToListAsync();
        if (!voters.Any())
        {
            return NotFound("No matching voters found");
        }

        // Return the list of voters
        return Ok(voters);
    }

    // Get a list of voters who voted for a candidate with the specified ID
    [HttpGet("get-voters-by-candidate-id/{id}")]
    public async Task<ActionResult<IEnumerable<Voter>>> GetVotersByCandidateId(string id)
    {
        // Check if any voters exist for the given candidate ID
        List<Voter> voters = await _voterCollection.Find(v => v.SelectedPresidentById == id.Trim()).ToListAsync();
        if (!voters.Any())
        {
            return NotFound("No matching voters found");
        }

        // Return the list of voters
        return Ok(voters);
    }

    // Get a list of all voters in the database
    [HttpGet("get-all-voters")]
    public async Task<ActionResult<IEnumerable<Voter>>> GetAllVoters()
    {
        // Retrieve all voters from the database
        List<Voter> voters = await _voterCollection.Find(new BsonDocument()).ToListAsync();

        // Return the list of voters
        return Ok(voters);
    }
}
