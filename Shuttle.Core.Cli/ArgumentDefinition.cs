using System;
using System.Collections.Generic;
using System.Linq;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Cli;

public class ArgumentDefinition
{
    private readonly List<string> _aliases = new();

    public ArgumentDefinition(string name, params string[] aliases)
    {
        Name = Guard.AgainstNullOrEmptyString(name, nameof(name));

        _aliases.AddRange(aliases.Where(item => !item.Equals(name, StringComparison.InvariantCultureIgnoreCase)).Distinct());
    }

    public IEnumerable<string> Aliases => _aliases.AsReadOnly();
    public bool IsRequired { get; private set; }

    public string Name { get; }

    public ArgumentDefinition AsRequired()
    {
        IsRequired = true;

        return this;
    }

    public bool IsSatisfiedBy(string name)
    {
        Guard.AgainstNullOrEmptyString(name, nameof(name));

        return Name.Equals(name, StringComparison.InvariantCultureIgnoreCase) || _aliases.Any(item => item.Equals(name, StringComparison.InvariantCultureIgnoreCase));
    }
}