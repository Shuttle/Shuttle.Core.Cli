using System;
using NUnit.Framework;

namespace Shuttle.Core.Cli.Tests
{
    [TestFixture]
    public class ArgumentsFixture
    {
        [Test]
        public void Should_not_be_able_to_add_a_duplicate_argument_definition_or_alias()
        {
            var arguments = new Arguments();

            arguments.Add(new ArgumentDefinition("arg1", "a1"));

            Assert.That(() => arguments.Add(new ArgumentDefinition("arg1")), Throws.TypeOf<InvalidOperationException>());
            Assert.That(() => arguments.Add(new ArgumentDefinition("arg2", "a1")), Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void Should_be_able_to_get_the_argument_using_an_alias()
        {
            var arguments = new Arguments("-a1:one", "/arg2=two");

            arguments.Add(new ArgumentDefinition("arg1", "a1"));
            arguments.Add(new ArgumentDefinition("arg2", "a2"));

            Assert.That(arguments.Contains("arg1"), Is.True);
            Assert.That(arguments.Contains("a2"), Is.True);

            Assert.That(arguments.Get<string>("arg1"), Is.EqualTo("one"));
            Assert.That(arguments.Get<string>("a2"), Is.EqualTo("two"));
        }

        [Test]
        public void Should_be_able_to_parse_simple_arguments()
        {
            var arguments = new Arguments("-arg1:arg1value", "/arg2", "arg2value", "--enabled", "/threads=5");

            Assert.That(arguments["arg1"], Is.EqualTo("arg1value"));
            Assert.That(arguments["arg2"], Is.EqualTo("arg2value"));

            Assert.That(arguments.Get("enabled", false), Is.True);
            Assert.That(arguments.Get("disabled", true), Is.True);

            Assert.That(arguments.Get("threads", 5), Is.EqualTo(5));
            Assert.That(arguments["threads"], Is.EqualTo("5"));

            Assert.That(arguments["bogus"], Is.Null);
            Assert.Throws<InvalidOperationException>(() => arguments.Get<int>("bogus"));
        }

        [Test]
        public void Should_be_able_to_determine_whether_all_required_fields_are_present()
        {
            var arguments = new Arguments();

            Assert.That(arguments.HasMissingValues(), Is.False);

            var definition = new ArgumentDefinition("arg1");

            arguments.Add(definition);

            Assert.That(arguments.HasMissingValues(), Is.False);

            definition.AsRequired();

            Assert.That(arguments.HasMissingValues(), Is.True);

            arguments.Add("arg1");

            Assert.That(arguments.HasMissingValues(), Is.False);

            arguments.Add(new ArgumentDefinition("arg2", "a2").AsRequired());

            Assert.That(arguments.HasMissingValues(), Is.True);

            arguments.Add("a2");

            Assert.That(arguments.HasMissingValues(), Is.False);
        }
    }
}