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
    [Fact]
    public void NormalizeTaskTitle()
    {
        string input = "  Fix   bug   #123  ";
        string? result = TaskNormalizer.NormalizeTitle(input);
        Assert.Equal("Fix bug #123", result);
    }
}
