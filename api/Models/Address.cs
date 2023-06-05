using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models;

public record Address(
  string City,
  State State,
  string Street,
  [RegularExpression(@"^\d{5}$", ErrorMessage = "ZipCode must be a 5-digit number")] string ZipCode
);
