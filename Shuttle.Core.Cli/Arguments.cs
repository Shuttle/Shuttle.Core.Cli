using System;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace Shuttle.Core.Cli
{
    public class Arguments
    {
        private readonly StringDictionary _parameters;

        private readonly Regex _remover = new Regex(@"^['""]?(.*?)['""]?$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private readonly Regex _spliter = new Regex(@"^-{1,2}|^/|=|:",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public Arguments(params string[] commandLine)
        {
            CommandLine = commandLine;

            _parameters = new StringDictionary();

            string parameter = null;

            foreach (var input in commandLine)
            {
                var parts = _spliter.Split(input, 3);

                switch (parts.Length)
                {
                    case 1:
                    {
                        if (parameter != null)
                        {
                            if (!_parameters.ContainsKey(parameter))
                            {
                                parts[0] = _remover.Replace(parts[0], "$1");

                                _parameters.Add(parameter.ToLower(), parts[0]);
                            }

                            parameter = null;
                        }

                        break;
                    }
                    case 2:
                    {
                        if (parameter != null)
                        {
                            if (!_parameters.ContainsKey(parameter))
                            {
                                _parameters.Add(parameter.ToLower(), "true");
                            }
                        }

                        parameter = parts[1];

                        break;
                    }
                    case 3:
                    {
                        if (parameter != null)
                        {
                            if (!_parameters.ContainsKey(parameter))
                            {
                                _parameters.Add(parameter.ToLower(), "true");
                            }
                        }

                        parameter = parts[1];

                        if (!_parameters.ContainsKey(parameter))
                        {
                            parts[2] = _remover.Replace(parts[2], "$1");

                            _parameters.Add(parameter.ToLower(), parts[2]);
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

            if (!_parameters.ContainsKey(parameter))
            {
                _parameters.Add(parameter.ToLower(), "true");
            }
        }

        public string[] CommandLine { get; }

        public string this[string name] => _parameters[name];

        public T Get<T>(string name)
        {
            if (!_parameters.ContainsKey(name.ToLower()))
            {
                throw new InvalidOperationException(
                    string.Format(Resources.MissingArgumentException, name));
            }

            return ChangeType<T>(name);
        }

        private T ChangeType<T>(string name)
        {
            return (T) Convert.ChangeType(_parameters[name.ToLower()], typeof(T));
        }

        public T Get<T>(string name, T @default)
        {
            if (!_parameters.ContainsKey(name.ToLower()))
            {
                return @default;
            }

            return ChangeType<T>(name);
        }

        public bool Contains(string name)
        {
            return _parameters.ContainsKey(name.ToLower());
        }
    }
}