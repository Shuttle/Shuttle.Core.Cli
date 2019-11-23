using System;
using System.Collections.Generic;
using System.Linq;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Cli
{
    public class ArgumentDefinition
    {
        private readonly List<string> _aliases = new List<string>();
        private string _description;
        private bool _isOptional;

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

        public ArgumentDefinition WithDescription(string description)
        {
            Guard.AgainstNullOrEmptyString(description, nameof(description));

            _description = description;

            return this;
        }

        public ArgumentDefinition AsOptional()
        {
            _isOptional = true;

            return this;
        }

        public bool IsSatisfiedBy(string name)
        {
            return Name.Equals(name, StringComparison.InvariantCultureIgnoreCase) ||
                   _aliases.Any(item => item.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        public string GetHelp()
        {
            return GetHelp(new HelpOptions());
        }

        public string GetHelp(HelpOptions options)
        {
            Guard.AgainstNull(options, nameof(options));

            return options.Template
                .Replace("{name}", Name)
                .Replace("{description}", _description ?? string.Empty)
                .Replace("{aliases}",
                    _aliases.Any()
                        ? $"{options.AliasSeparator}{string.Join(options.AliasSeparator, _aliases)}"
                        : string.Empty)
                .Replace("{optional-start-tag}", _isOptional ? options.OptionalStartTag : string.Empty)
                .Replace("{optional-end-tag}", _isOptional ? options.OptionalEndTag : string.Empty);
        }

        public class HelpOptions
        {
            public HelpOptions(string template)
            {
                Guard.AgainstNullOrEmptyString(template, nameof(template));

                Template = template;
            }

            public HelpOptions() : this(Resources.HelpTemplate)
            {
            }

            public string Template { get; }
            public string AliasSeparator { get; private set; } = "|";

            public string OptionalEndTag { get; private set; } = "]";

            public string OptionalStartTag { get; private set; } = "[";

            public HelpOptions WithAliasSeparator(string aliasSeparator)
            {
                AliasSeparator = string.IsNullOrEmpty(aliasSeparator) ? "|" : aliasSeparator;

                return this;
            }

            public HelpOptions WithOptionalTags(string startTag, string endTag)
            {
                Guard.AgainstNullOrEmptyString(startTag, nameof(startTag));
                Guard.AgainstNullOrEmptyString(endTag, nameof(endTag));

                OptionalStartTag = startTag;
                OptionalEndTag = endTag;

                return this;
            }
        }
    }
}