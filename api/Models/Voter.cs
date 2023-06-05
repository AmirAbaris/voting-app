using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models;

public record Voter(
  [property: BsonId, BsonRepresentation(BsonType.ObjectId)] string? VoterId,
  [RegularExpression(@"^\d{9}$", ErrorMessage = "VoterNationalId must be a 9-digit number")] string VoterNationalId,
  string SelectedPresidentById,
  [RegularExpression(@"^\d{9}$", ErrorMessage = "SelectedPresidentNationalId must be a 9-digit number")] string SelectedPresidentNationalId,
  [MinLength(3), MaxLength(20), RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "FirstName must only contain letters")] string FirstName,
  [MinLength(3), MaxLength(20), RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "LastName must only contain letters")] string LastName,
  Gender Gender,
  [Range(18, 120)] int Age,
  Address Address
);
