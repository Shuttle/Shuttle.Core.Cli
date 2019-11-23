﻿using NUnit.Framework;

namespace Shuttle.Core.Cli.Tests
{
    [TestFixture]
    public class ArgumentDefinitionFixture
    {
        [Test]
        public void Should_be_able_to_satisfy_relevant_names()
        {
            var definition= new ArgumentDefinition("arg1", "a1", "n1");

            Assert.That(definition.IsSatisfiedBy("Arg1"), Is.True);
            Assert.That(definition.IsSatisfiedBy("A1"), Is.True);
            Assert.That(definition.IsSatisfiedBy("N1"), Is.True);
            Assert.That(definition.IsSatisfiedBy("Arg2"), Is.False);
            Assert.That(definition.IsSatisfiedBy("A2"), Is.False);
            Assert.That(definition.IsSatisfiedBy("N2"), Is.False);
        }

        [Test]
        public void Should_be_able_to_exclude_duplicate_aliases()
        {
            var definition = new ArgumentDefinition("arg1", "a1", "arg1", "a1", "n1").WithDescription("argument one");

            Assert.That(definition.GetHelp(), Is.EqualTo("-arg1|a1|n1 : argument one"));
        }
    }
}