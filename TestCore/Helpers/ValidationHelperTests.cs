using NUnit.Framework;
using FluentAssertions;
using ProjectBotenReservering.Core.Helpers;

namespace TestCore.Helpers;

[TestFixture]
public class ValidationHelperTests
{
    [TestCase("Jan", false, TestName = "IsValidName_TooShort_ReturnsFalse")]
    [TestCase("Jan Jansen", true, TestName = "IsValidName_Valid_ReturnsTrue")]
    [TestCase("Jan-Willem de Vries", true, TestName = "IsValidName_HyphensAndSpaces_ReturnsTrue")]
    [TestCase("Jan Jansen2", false, TestName = "IsValidName_Numbers_ReturnsFalse")]
    [TestCase("", false, TestName = "IsValidName_Empty_ReturnsFalse")]
    [TestCase(null, false, TestName = "IsValidName_Null_ReturnsFalse")]
    [TestCase(" Jan Jansen ", true, TestName = "IsValidName_Untrimmed_ReturnsTrue")]
    [TestCase("René Één", true, TestName = "IsValidName_Accents_ReturnsTrue")]
    [TestCase("Jan Jansen!", false, TestName = "IsValidName_SpecialCharExclamation_ReturnsFalse")]
    public void IsValidName_Scenarios_ReturnExpected(string? input, bool expected)
    {
        bool result = ValidationHelper.IsValidName(input!);
        result.Should().Be(expected);
    }

    [TestCase("test@test.nl", true, TestName = "IsValidEmail_Valid_ReturnsTrue")]
    [TestCase("testtest.nl", false, TestName = "IsValidEmail_NoAtSign_ReturnsFalse")]
    [TestCase("", false, TestName = "IsValidEmail_Empty_ReturnsFalse")]
    [TestCase(null, false, TestName = "IsValidEmail_Null_ReturnsFalse")]
    [TestCase("test@domain", false, TestName = "IsValidEmail_NoExtension_ReturnsFalse")]
    [TestCase("test@.nl", false, TestName = "IsValidEmail_NoDomainName_ReturnsFalse")]
    [TestCase("test @test.nl", false, TestName = "IsValidEmail_SpaceInMail_ReturnsFalse")]
    public void IsValidEmail_Scenarios_ReturnExpected(string? input, bool expected)
    {
        bool result = ValidationHelper.IsValidEmail(input!);
        result.Should().Be(expected);
    }

    [TestCase("-1", false, TestName = "IsValidLevel_Negative_ReturnsFalse")]
    [TestCase("0", true, TestName = "IsValidLevel_MinBoundary_ReturnsTrue")]
    [TestCase("3", true, TestName = "IsValidLevel_MaxBoundary_ReturnsTrue")]
    [TestCase("4", false, TestName = "IsValidLevel_TooHigh_ReturnsFalse")]
    [TestCase("A", false, TestName = "IsValidLevel_NotInt_ReturnsFalse")]
    [TestCase("", true, TestName = "IsValidLevel_Empty_ReturnsTrue")]
    [TestCase(null, true, TestName = "IsValidLevel_Null_ReturnsTrue")]
    [TestCase(" 2 ", true, TestName = "IsValidLevel_UntrimmedNumber_ReturnsTrue")]
    public void IsValidLevel_Scenarios_ReturnExpected(string? input, bool expected)
    {
        bool result = ValidationHelper.IsValidLevel(input!);
        result.Should().Be(expected);
    }
}
