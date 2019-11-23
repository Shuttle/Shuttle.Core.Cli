using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Cli
{
    public class Arguments
    {
        private readonly Dictionary<string, ArgumentDefinition> _argumentDefinitions =
            new Dictionary<string, ArgumentDefinition>();

        private readonly StringDictionary _arguments;

        private readonly Regex _remover = new Regex(@"^['""]?(.*?)['""]?$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private readonly Regex _splitter = new Regex(@"^-{1,2}|^/|=|:",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public Arguments(params string[] commandLine)
        {
            CommandLine = commandLine;

            _arguments = new StringDictionary();

            string parameter = null;

            foreach (var input in commandLine)
            {
                var parts = _splitter.Split(input, 3);

                switch (parts.Length)
                {
                    case 1:
                    {
                        if (parameter != null)
                        {
                            if (!_arguments.ContainsKey(parameter))
                            {
                                parts[0] = _remover.Replace(parts[0], "$1");

                                _arguments.Add(parameter.ToLower(), parts[0]);
                            }

                            parameter = null;
                        }

                        break;
                    }
                    case 2:
                    {
                        if (parameter != null)
                        {
                            if (!_arguments.ContainsKey(parameter))
                            {
                                _arguments.Add(parameter.ToLower(), "true");
                            }
                        }

                        parameter = parts[1];

                        break;
                    }
                    case 3:
                    {
                        if (parameter != null)
                        {
                            if (!_arguments.ContainsKey(parameter))
                            {
                                _arguments.Add(parameter.ToLower(), "true");
                            }
                        }

                        parameter = parts[1];

                        if (!_arguments.ContainsKey(parameter))
                        {
                            parts[2] = _remover.Replace(parts[2], "$1");

                            _arguments.Add(parameter.ToLower(), parts[2]);
                        }

                        parameter = null;

                        break;
                    }
                }
            }

            if (parameter == null)
            {
                return;
            }

            if (!_arguments.ContainsKey(parameter))
            {
                _arguments.Add(parameter.ToLower(), "true");
            }
        }

        public string[] CommandLine { get; }

        public string this[string name] => _arguments[name];

        public T Get<T>(string name)
        {
            var value = GetArgumentValue(name);

            if (string.IsNullOrEmpty(value))
            {
                throw new InvalidOperationException(
                    string.Format(Resources.MissingArgumentException, name));
            }

            return ChangeType<T>(value);
        }

        private string GetArgumentValue(string name)
        {
            var key = name.ToLower();

            if (_arguments.ContainsKey(key))
            {
                return _arguments[key];
            }

            if (!_argumentDefinitions.ContainsKey(key))
            {
                return string.Empty;
            }

            foreach (var alias in _argumentDefinitions[key].Aliases)
            {
                key = alias.ToLower();

                if (_arguments.ContainsKey(key))
                {
                    return _arguments[key];
                }
            }

            return string.Empty;
        }

        private static T ChangeType<T>(string value)
        {
            return (T) Convert.ChangeType(value, typeof(T));
        }

        public T Get<T>(string name, T @default)
        {
            var value = GetArgumentValue(name);

            return string.IsNullOrEmpty(value) ? @default : ChangeType<T>(value);
        }

        public bool Contains(string name)
        {
            return _arguments.ContainsKey(name.ToLower());
        }

        public Arguments Add(ArgumentDefinition definition)
        {
            Guard.AgainstNull(definition, nameof(definition));

            var key = definition.Name.ToLower();

            if (_argumentDefinitions.Any(existing =>
                existing.Value.IsSatisfiedBy(key) ||
                definition.Aliases.Any(alias => existing.Value.IsSatisfiedBy(alias))))
            {
                throw new InvalidOperationException(string.Format(Resources.DuplicateArgumentDefinitionException,
                    definition.Name));
            }

            _argumentDefinitions.Add(key, definition);

            return this;
        }
    }
}