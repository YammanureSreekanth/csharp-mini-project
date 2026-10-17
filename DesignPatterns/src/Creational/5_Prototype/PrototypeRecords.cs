using System.Collections.Immutable;

namespace Creational.Prototype;

public record ListingRecord(string Name, decimal Price, ImmutableList<string> Tags);