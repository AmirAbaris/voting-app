using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

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
        var candidate = await _candidateCollection.Find<PresidentCandidate>(c =>
        c.CandidateNationalId == candidateNationalId.Trim()).FirstOrDefaultAsync();

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
    public async Task<ActionResult<IEnumerable<object>>> GetWinnerCandidate()
    {
        // Retrive candidates
        List<PresidentCandidate> candidates = await _candidateCollection.Find(new BsonDocument()).ToListAsync();

        if (!candidates.Any())
        {
            return BadRequest("no candidate found");
        }

        // Count the vote for each candidate
        Dictionary<string, int> voteCounts = new Dictionary<string, int>();

        foreach (var candidate in candidates)
        {
            List<Voter> voters = await _voterCollection.Find(v =>
            v.SelectedPresidentNationalId == candidate.CandidateNationalId).ToListAsync();

            int voteCount = voters.Count();

            voteCounts.Add(candidate.CandidateNationalId, voteCount);
        }

        // Find candidate with the most votes
        KeyValuePair<string, int> winner = voteCounts.OrderByDescending(kv =>
        kv.Value).First();

        // Retrive the winner's information
        var winnerCandidate = await _candidateCollection.Find<PresidentCandidate>(c =>
        c.CandidateNationalId == winner.Key).FirstOrDefaultAsync();

        // Return the winner's information and vote count
        return Ok(new { Winner = winnerCandidate, voteCounts = winner.Value });
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

        var stateVoteCounts = voters.GroupBy(v =>
        v.Address.State).Select(stateGroup =>
        new { State = $"{stateGroup.Key}", VoteCount = stateGroup.Count() }).ToList();

        return stateVoteCounts;
    }
}
