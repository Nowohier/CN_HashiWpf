using FluentAssertions;
using Hashi.Rules.Extensions;
using Moq;
using NRules;
using NRules.Fluent;
using NRules.RuleModel;

namespace Hashi.Rules.Test;

/// <summary>
/// Unit tests for HashiRuleRepositoryExtensions class.
/// </summary>
public class HashiRuleRepositoryExtensionsTests
{
    private RuleRepository ruleRepository;

    [SetUp]
    public void SetUp()
    {
        ruleRepository = new RuleRepository();
    }

    [Test]
    public void CompileOne_WhenRuleDoesNotExist_ShouldThrowArgumentException()
    {
        // Arrange
        const string ruleName = "NonExistentRule";
        
        // Act & Assert
        var action = () => ruleRepository.CompileOne(ruleName);
        action.Should().Throw<ArgumentException>()
            .WithMessage($"Rule {ruleName} not found in the repository.");
    }

    [Test]
    public void CompileOne_WhenRepositoryIsEmpty_ShouldThrowArgumentException()
    {
        // Arrange
        const string ruleName = "TestRule";
        
        // Act & Assert
        var action = () => ruleRepository.CompileOne(ruleName);
        action.Should().Throw<ArgumentException>()
            .WithMessage($"Rule {ruleName} not found in the repository.");
    }

    [Test]
    public void CompileOne_WhenRuleNameIsNull_ShouldThrowArgumentException()
    {
        // Act & Assert
        var action = () => ruleRepository.CompileOne(null!);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Rule  not found in the repository.");
    }

    [Test]
    public void CompileOne_WhenRuleNameIsEmpty_ShouldThrowArgumentException()
    {
        // Arrange
        const string emptyRuleName = "";
        
        // Act & Assert
        var action = () => ruleRepository.CompileOne(emptyRuleName);
        action.Should().Throw<ArgumentException>()
            .WithMessage($"Rule {emptyRuleName} not found in the repository.");
    }
}