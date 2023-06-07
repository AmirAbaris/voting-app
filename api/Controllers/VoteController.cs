namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VoteController : ControllerBase
{
    private readonly IMongoCollection<PresidentCandidate> _candidateCollection;
    private readonly IMongoCollection<Voter> _voterCollection;

    // Dependency Injection
    public VoteController(IMongoClient client, IMongoDbSettings dbSettings)
    {
        var dbName = client.GetDatabase(dbSettings.DatabaseName);
        _candidateCollection = dbName.GetCollection<PresidentCandidate>("president-candidates");
        _voterCollection = dbName.GetCollection<Voter>("voters");
    }

    [HttpGet("count-votes-by-candidate-national-id/{candidateNationalId}")]
    public async Task<ActionResult<IEnumerable<int>>> GetCandidateVoteCount(string candidateNationalId)
    {
        // Retrieve the candidate
        var candidate = await _candidateCollection.Find<PresidentCandidate>(candidate =>
        candidate.CandidateNationalId == candidateNationalId.Trim()).FirstOrDefaultAsync();
        if (candidate == null)
        {
            return NotFound("no matching candidate found");
        }

        List<Voter> voters = await _voterCollection.Find(v =>
        v.SelectedPresidentNationalId == candidate.CandidateNationalId).ToListAsync();

        // Count the number of voters
        var voteCount = voters.Count();

        // Return the vote count
        return Ok(voteCount);
    }

    [HttpGet("count-all-votes")]
    public async Task<ActionResult<IEnumerable<int>>> GetCountOfAllVotes()
    {
        // Retrieve all voters
        List<Voter> voters = await _voterCollection.Find(new BsonDocument()).ToListAsync();

        // Count the number of voters
        var totalVoteCount = voters.Count();

        // Return the vote count
        return Ok(totalVoteCount);
    }

    [HttpGet("select-winner-candidate")]
    public async Task<ActionResult<object>> GetWinnerCandidate()
    {
        // Retrieve candidates and voters
        var candidates = await _candidateCollection.Find(new BsonDocument()).ToListAsync();
        var voters = await _voterCollection.Find(new BsonDocument()).ToListAsync();

        if (!candidates.Any())
        {
            return BadRequest("No candidates found");
        }

        // Count votes for each candidate
        var voteCounts = candidates
            .Where(c => !string.IsNullOrEmpty(c.CandidateNationalId))
            .ToDictionary(c => c.CandidateNationalId, c => voters.Count(voter => string.Equals(voter.SelectedPresidentNationalId, c.CandidateNationalId)));

        // Find candidates with the most votes
        var maxVoteCount = voteCounts.Values.Max();
        var winnerCandidates = voteCounts.Where(kv => kv.Value == maxVoteCount)
        .Select(kv => kv.Key).ToList();

        if (winnerCandidates.Count != 1)
        {
            // Handle ties by returning all tied candidates
            var tiedCandidates = await _candidateCollection.Find(candidate =>
                winnerCandidates.Contains(candidate.CandidateNationalId))
                .ToListAsync();

            return Ok(new { Winners = tiedCandidates, VoteCount = maxVoteCount });
        }

        // Retrieve the winner's information
        var winnerCandidate = await _candidateCollection.Find<PresidentCandidate>(candidate =>
            candidate.CandidateNationalId == winnerCandidates[0]).FirstOrDefaultAsync();

        // Return the winner's information and vote count
        return Ok(new { Winner = winnerCandidate, VoteCount = maxVoteCount });
    }

    [HttpGet("list-candidates-by-vote-count")]
    public async Task<ActionResult<IEnumerable<object>>> GetWinnersByVoteCount()
    {
        // Get voters
        List<Voter> voters = await _voterCollection.Find(new BsonDocument()).ToListAsync();

        // Get and sort candidates
        var candidates = voters
        .GroupBy(voters => voters.SelectedPresidentNationalId)
        .Select(candidate => new { votesFor = candidate.Key, voteCount = candidate.Count() })
        .OrderByDescending(candidate => candidate.voteCount);

        return Ok(candidates);
    }

    [HttpGet("get-state-votes/{state}")]
    public async Task<ActionResult<IEnumerable<string>>> GetStateVote(State state)
    {
        // Retrive voters state
        List<Voter> votersState = await _voterCollection.Find(v =>
        v.Address.State == state).ToListAsync();

        // Count votes by input state
        int voteCount = votersState.Count();

        // Return success
        return Ok($"{state}: {voteCount} votes");
    }

    [HttpGet("get-all-state-votes")]
    public async Task<ActionResult<IEnumerable<object>>> GetAllStatesVots()
    {
        // Retrive all voters
        List<Voter> voters = await _voterCollection.Find(new BsonDocument()).ToListAsync();

        // Select required content from voters list
        var stateVoteCounts = voters.GroupBy(v =>
        v.Address.State).Select(stateGroup =>
        new { State = $"{stateGroup.Key}", VoteCount = stateGroup.Count() }).ToList();

        // Return an object of selected content
        return Ok(stateVoteCounts);
    }

    [HttpGet("get-all-candidates-with-vote-percentage")]
    public async Task<ActionResult<IEnumerable<object>>> GetAllCandidatesWithVotePercentage()
    {
        // Retrive candidates
        List<PresidentCandidate> candidates = await _candidateCollection
        .Find(new BsonDocument()).ToListAsync();
        if (!candidates.Any())
        {
            return BadRequest("no candidate found");
        }

        // Retrive  total voters
        List<Voter> totalVoters = await _voterCollection.Find(new BsonDocument()).ToListAsync();
        if (!totalVoters.Any())
        {
            return BadRequest("no voter found");
        }

        // Count the vote for each candidate
        var voteCounts = new List<int>();
        var presidentCandidates = new List<PresidentCandidate>();
        foreach (var candidate in candidates)
        {
            var voters = await _voterCollection.Find(v =>
            v.SelectedPresidentNationalId == candidate.CandidateNationalId).ToListAsync();

            // Count votes
            int voteCount = voters.Count();

            // Add vote count to list
            voteCounts.Add(voteCount);

            // Add candidate to list
            presidentCandidates.Add(candidate);

        }

        // Count all votes
        int totalVotes = totalVoters.Count();

        // Calc each candidates votes to percentage
        var percentageResult = new List<float>();
        foreach (float vote in voteCounts)
        {
            float result = ((vote / totalVotes) * 100);
            percentageResult.Add(result);
        }

        return Ok(new { Candidates = presidentCandidates, Percentages = percentageResult });
    }
}
