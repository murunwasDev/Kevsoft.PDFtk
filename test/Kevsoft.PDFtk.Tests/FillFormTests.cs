﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Kevsoft.PDFtk.Tests
{
    public class FillFormTests
    {
        private readonly PDFtk _pdFtk = new();

        private static readonly Dictionary<string, string> FieldData = new()
        {
            ["Given Name Text Box"] = Guid.NewGuid().ToString(),
            ["Language 3 Check Box"] = "Yes",
        };

        [Fact]
        public async Task ShouldFillPdfForm_ForInputFileAsBytes()
        {
            var fileBytes = await File.ReadAllBytesAsync("TestFiles/Form.pdf");

            var result = await _pdFtk.FillForm(fileBytes, FieldData, false, false);

            result.Success.Should().BeTrue();
            var dumpDataFields = await _pdFtk.DumpDataFields(result.Result);
            dumpDataFields.Result.Where(x => FieldData.Keys.Contains(x.FieldName))
                .ToDictionary(x => x.FieldName!, field => field.FieldValue)
                .Should()
                .BeEquivalentTo(FieldData);
        }
        
        [Fact]
        public async Task ShouldFillPdfForm_ForInputFileAsStream()
        {
            var fileStream = File.OpenRead("TestFiles/Form.pdf");

            var result = await _pdFtk.FillForm(fileStream, FieldData, false, false);

            result.Success.Should().BeTrue();
            var dumpDataFields = await _pdFtk.DumpDataFields(result.Result);
            dumpDataFields.Result.Where(x => FieldData.Keys.Contains(x.FieldName))
                .ToDictionary(x => x.FieldName!, field => field.FieldValue)
                .Should()
                .BeEquivalentTo(FieldData);
        }
        
        [Fact]
        public async Task ShouldFillPdfForm_ForInputFileAsFilePath()
        {
            var result = await _pdFtk.FillForm("TestFiles/Form.pdf", FieldData, false, false);

            result.Success.Should().BeTrue();
            var dumpDataFields = await _pdFtk.DumpDataFields(result.Result);
            dumpDataFields.Result.Where(x => FieldData.Keys.Contains(x.FieldName))
                .ToDictionary(x => x.FieldName!, field => field.FieldValue)
                .Should()
                .BeEquivalentTo(FieldData);
        }

        [Fact]
        public async Task ShouldReturnUnsuccessfulAndEmptyResult_ForInvalidPdfFile()
        {
            var fileBytes = Guid.NewGuid().ToByteArray();

            var result = await _pdFtk.FillForm(fileBytes, new Dictionary<string, string>(), false, false);

            result.Success.Should().BeFalse();
            result.Result.Should().BeEmpty();
        }

    }
}