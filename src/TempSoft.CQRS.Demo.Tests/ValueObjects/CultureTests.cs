using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using TempSoft.CQRS.Demo.ValueObjects;

namespace TempSoft.CQRS.Demo.Tests.ValueObjects
{
    [TestFixture]
    public class CultureTests
    {
        [Test]
        public void Should_support_creating_culture_from_country_and_language()
        {
            var culture = new Culture(Data.Country, Data.Language);

            culture.Country.Should().Be(Data.Country);
            culture.Language.Should().Be(Data.Language);
            culture.ToString().Should().Be(Data.Culture);
        }

        [Test]
        public void Should_support_creating_culture_from_culture_string()
        {
            var culture = new Culture(Data.Culture);

            culture.Country.Should().Be(Data.Country);
            culture.Language.Should().Be(Data.Language);
            culture.ToString().Should().Be(Data.Culture);
        }

        [Test]
        public void Should_support_creating_culture_from_just_a_country()
        {
            var culture = new Culture(Data.Country);

            culture.Country.Should().Be(Data.Country);
            culture.ToString().Should().Be(Data.Country);
        }

        [Test]
        public void Should_be_able_to_compare_two_cultures()
        {
            var culture1 = new Culture(Data.Country, Data.Language);
            var culture2 = new Culture(Data.Culture);

            (culture1 == culture2).Should().BeTrue();
        }

        [Test]
        public void Should_be_able_to_serialize_a_culture()
        {
            var culture1 = new Culture(Data.Country, Data.Language);
            var json = JsonConvert.SerializeObject(culture1);
            var culture2 = JsonConvert.DeserializeObject<Culture>(json);

            (culture1 == culture2).Should().BeTrue();
        }

        public static class Data
        {
            public const string Country = "SE";

            public const string Language = "sv";

            public static readonly string Culture = $"{Language}-{Country}";
        }


    }
}
