using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models;

public record PresidentCandidate(
  [property: BsonId, BsonRepresentation(BsonType.ObjectId)] string? CandidateId,
  [RegularExpression(@"^\d{9}$", ErrorMessage = "CandidateNationalId must be a 9-digit number")] string CandidateNationalId,
  [MinLength(3), MaxLength(20), RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "FirstName must only contain letters")] string FirstName,
  [MinLength(3), MaxLength(20), RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "LastName must only contain letters")] string LastName,
  [Range(35, 120)] int Age,
  Gender Gender,
  Party Party,
  Address Address
);
