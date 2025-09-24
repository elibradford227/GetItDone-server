namespace GetItDone.Tests;

using Xunit;
using GetItDone.Controllers;
using GetItDone.models;
using GetItDone.models.DTOs;
using GetItDone.repositories;
using GetItDone.services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using GetItDone.Utils;

public class TaskNormalizerTests
{
    [Theory]
    [InlineData("   Fix   bug   #123   ", "Fix bug #123")]
    [InlineData("\tHello\nWorld", "Hello World")]
    [InlineData("SingleWord", "SingleWord")]
    [InlineData(null, null)]
    public void NormalizeTitle_CollapsesWhiteSpace(string? input, string? expected)
    {
        string? result = TaskNormalizer.NormalizeTitle(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Fix\u200BBug", "FixBug")] 
    [InlineData("Bug\u0007", "Bug")]       
    public void NormalizeTitle_StripsInvisibleCharacters(string input, string expected)
    {
        string? result = TaskNormalizer.NormalizeTitle(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Single line description", "Single line description")]
    [InlineData("\tTabbed in text", "Tabbed in text")]
    [InlineData(null, null)]
    public void NormalizeDescription_CollapsesWhiteSpace(string? input, string? expected)
    {
        string? result = TaskNormalizer.NormalizeDescription(input);
        Assert.Equal(expected, result);
    }
}
