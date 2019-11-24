using System;
using System.Collections.Generic;
using System.Linq;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Cli
{
    public class ArgumentDefinition
    {
        private readonly List<string> _aliases = new List<string>();

        public ArgumentDefinition(string name, params string[] aliases)
        {
            Guard.AgainstNullOrEmptyString(name, nameof(name));

            Name = name;

            if (aliases != null)
            {
                _aliases.AddRange(aliases.Where(item => !item.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    .Distinct());
            }
        }

        public string Name { get; }
        public IEnumerable<string> Aliases => _aliases.AsReadOnly();

        public bool IsSatisfiedBy(string name)
        {
            return Name.Equals(name, StringComparison.InvariantCultureIgnoreCase) ||
                   _aliases.Any(item => item.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}