using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Cayleykit.Tests;

public class DocsTests
{
    private static readonly string[] ExpectedSourcePaths =
    [
        "src/Cayleykit/Core/CayleyException.cs",
        "src/Cayleykit/Core/Text.cs",
        "src/Cayleykit/Core/Ints.cs",
        "src/Cayleykit/Core/Options.cs",
        "src/Cayleykit/Core/Io.cs",
        "src/Cayleykit/Perms/Perm.cs",
        "src/Cayleykit/Perms/Notation.cs",
        "src/Cayleykit/Perms/Structure.cs",
        "src/Cayleykit/Groups/Orbits.cs",
        "src/Cayleykit/Groups/Chain.cs",
        "src/Cayleykit/Groups/Closure.cs",
        "src/Cayleykit/Groups/WordSearch.cs",
        "src/Cayleykit/Groups/Classes.cs",
        "src/Cayleykit/Commands/ShowCommand.cs",
        "src/Cayleykit/Commands/MulCommand.cs",
        "src/Cayleykit/Commands/PowCommand.cs",
        "src/Cayleykit/Commands/ConjCommand.cs",
        "src/Cayleykit/Commands/OrbitsCommand.cs",
        "src/Cayleykit/Commands/OrderCommand.cs",
        "src/Cayleykit/Commands/WordCommand.cs",
        "src/Cayleykit/Commands/ElementsCommand.cs",
        "src/Cayleykit/Commands/ClassesCommand.cs",
        "src/Cayleykit/Commands/CentralizerCommand.cs",
        "src/Cayleykit/Cli.cs",
        "src/Cayleykit/Program.cs"
    ];

    private static readonly string[] ExpectedTestPaths =
    [
        "tests/Cayleykit.Tests/Core/TextTests.cs",
        "tests/Cayleykit.Tests/Core/IntsTests.cs",
        "tests/Cayleykit.Tests/Core/OptionsTests.cs",
        "tests/Cayleykit.Tests/Core/IoTests.cs",
        "tests/Cayleykit.Tests/Perms/PermTests.cs",
        "tests/Cayleykit.Tests/Perms/NotationTests.cs",
        "tests/Cayleykit.Tests/Perms/StructureTests.cs",
        "tests/Cayleykit.Tests/Groups/OrbitsTests.cs",
        "tests/Cayleykit.Tests/Groups/ChainTests.cs",
        "tests/Cayleykit.Tests/Groups/ClosureTests.cs",
        "tests/Cayleykit.Tests/Groups/WordSearchTests.cs",
        "tests/Cayleykit.Tests/Groups/ClassesTests.cs",
        "tests/Cayleykit.Tests/Commands/ShowTests.cs",
        "tests/Cayleykit.Tests/Commands/MulTests.cs",
        "tests/Cayleykit.Tests/Commands/PowTests.cs",
        "tests/Cayleykit.Tests/Commands/ConjTests.cs",
        "tests/Cayleykit.Tests/Commands/OrbitsCommandTests.cs",
        "tests/Cayleykit.Tests/Commands/OrderTests.cs",
        "tests/Cayleykit.Tests/Commands/WordTests.cs",
        "tests/Cayleykit.Tests/Commands/ElementsTests.cs",
        "tests/Cayleykit.Tests/Commands/ClassesCommandTests.cs",
        "tests/Cayleykit.Tests/Commands/CentralizerTests.cs",
        "tests/Cayleykit.Tests/UnitTest1.cs",
        "tests/Cayleykit.Tests/DocsTests.cs"
    ];

    [Fact]
    public void Readme_documents_the_product_contract()
    {
        var readme = ReadReadme();

        foreach (var heading in new[]
        {
            "## Install", "## Usage", "## Notation", "## Commands",
            "## Exit codes", "## Layout", "## Development"
        })
        {
            Assert.Contains(heading, readme);
        }

        foreach (var requiredText in new[]
        {
            "offline command-line calculator for permutations and the groups they generate",
            "algebra students, puzzle and cube solvers",
            "without a computer algebra system",
            "Install the .NET 10 SDK",
            "dotnet build -c Release",
            "src/Cayleykit/bin/Release/net10.0/cayleykit",
            "the platform's executable suffix",
            "dotnet src/Cayleykit/bin/Release/net10.0/cayleykit.dll COMMAND",
            "The identity is \u0060()\u0060",
            "Points are from 1 through 64",
            "no spaces, signs, leading zeroes, or repetitions",
            "Single-point cycles and malformed cycles are invalid.",
            "Points omitted from the notation are fixed.",
            "degree is inferred from the largest point mentioned",
            "A product applies its permutations from left to right",
            "two to eight permutations",
            "one to eight generators",
            "reads generator permutations from standard input",
            "100000 elements",
            "500 elements",
            "720 elements",
            "Exit code 0",
            "Exit code 1 is used only by",
            "Exit code 2",
            "dotnet build",
            "dotnet format --verify-no-changes",
            "dotnet test"
        })
        {
            Assert.Contains(requiredText, readme);
        }

        foreach (var command in new[]
        {
            "show [--degree D] P",
            "mul [--degree D] P Q...",
            "pow [--degree D] P K",
            "conj [--degree D] P Q",
            "orbits [--degree D] G...",
            "order [--degree D] G...",
            "word [--degree D] P G...",
            "elements [--degree D] G...",
            "classes [--degree D] G..."
        })
        {
            Assert.Contains("\u0060" + command + "\u0060", readme);
        }
    }

    [Fact]
    public void Readme_usage_and_three_examples_match_the_cli()
    {
        var readme = ReadReadme();
        var textBlocks = ExtractFencedBlocks(readme, "text");
        Assert.Equal(4, textBlocks.Count);

        var usageOutput = textBlocks[0];
        Assert.Equal(Cli.Usage + "\n", usageOutput);
        AssertCli(["--help"], usageOutput);

        var exampleArguments = new[]
        {
            new[] { "show", "(1,2,3)(4,5)" },
            new[]
            {
                "order",
                "(2,23,14,10)(4,21,16,12)(5,7,8,6)",
                "(1,3,4,2)(7,11,19,23)(8,12,20,24)",
                "(1,7,16,18)(2,5,15,20)(9,11,12,10)"
            },
            new[] { "word", "(1,3)", "(1,2)", "(2,3)" }
        };

        var shellCommands = ExtractFencedBlocks(readme, "sh")
            .SelectMany(block => block.Split('\n'))
            .Where(line => line.StartsWith("cayleykit ", StringComparison.Ordinal))
            .ToArray();
        Assert.Equal(
            new[]
            {
                "cayleykit show \"(1,2,3)(4,5)\"",
                "cayleykit order \"(2,23,14,10)(4,21,16,12)(5,7,8,6)\" \"(1,3,4,2)(7,11,19,23)(8,12,20,24)\" \"(1,7,16,18)(2,5,15,20)(9,11,12,10)\"",
                "cayleykit word \"(1,3)\" \"(1,2)\" \"(2,3)\""
            },
            shellCommands);

        for (var index = 0; index < exampleArguments.Length; index++)
        {
            AssertCli(exampleArguments[index], textBlocks[index + 1]);
        }
    }

    [Fact]
    public void Readme_layout_lists_and_covers_every_project_source_file()
    {
        var root = FindRepositoryRoot();
        var readme = ReadReadme();
        var lines = readme.Split('\n');
        var layoutIndex = Array.IndexOf(lines, "## Layout");
        Assert.True(layoutIndex >= 0, "README must contain the ## Layout section.");

        var layoutLines = lines
            .Skip(layoutIndex + 1)
            .TakeWhile(line => !line.StartsWith("## ", StringComparison.Ordinal))
            .ToArray();

        Assert.Equal("### Source", layoutLines.FirstOrDefault(line => line.StartsWith("### ", StringComparison.Ordinal)));
        Assert.Equal(
            new[] { "### Source", "### Tests" },
            layoutLines.Where(line => line.StartsWith("### ", StringComparison.Ordinal)));

        foreach (var line in layoutLines)
        {
            Assert.True(
                line.Length == 0 ||
                line == "### Source" ||
                line == "### Tests" ||
                line.StartsWith("- \u0060", StringComparison.Ordinal),
                $"Unexpected content in Layout: {line}");
        }

        var sourceHeader = Array.IndexOf(layoutLines, "### Source");
        var testsHeader = Array.IndexOf(layoutLines, "### Tests");
        Assert.True(sourceHeader >= 0 && testsHeader > sourceHeader, "Layout must list Source before Tests.");

        var sourceEntries = ParseLayoutEntries(layoutLines[(sourceHeader + 1)..testsHeader]);
        var testEntries = ParseLayoutEntries(layoutLines[(testsHeader + 1)..]);
        Assert.Equal(25, sourceEntries.Count);
        Assert.Equal(24, testEntries.Count);
        Assert.Equal(ExpectedSourcePaths, sourceEntries.Select(entry => entry.Path));
        Assert.Equal(ExpectedTestPaths, testEntries.Select(entry => entry.Path));

        var entries = sourceEntries.Concat(testEntries).ToArray();
        Assert.Equal(49, entries.Length);
        Assert.Equal(entries.Length, entries.Select(entry => entry.Path).Distinct(StringComparer.Ordinal).Count());

        foreach (var entry in entries)
        {
            Assert.True(
                File.Exists(Path.Combine(root, entry.Path.Replace('/', Path.DirectorySeparatorChar))),
                $"Layout path does not exist: {entry.Path}");
        }

        var listedPaths = entries.Select(entry => entry.Path).ToHashSet(StringComparer.Ordinal);
        var discoveredPaths = new[]
            {
                Path.Combine(root, "src", "Cayleykit"),
                Path.Combine(root, "tests", "Cayleykit.Tests")
            }
            .SelectMany(directory => Directory.EnumerateFiles(directory, "*", SearchOption.AllDirectories))
            .Where(path => string.Equals(Path.GetExtension(path), ".cs", StringComparison.OrdinalIgnoreCase))
            .Where(path => !HasBuildDirectorySegment(Path.GetRelativePath(root, path)))
            .Select(path => NormalizeRelativePath(Path.GetRelativePath(root, path)))
            .ToHashSet(StringComparer.Ordinal);

        Assert.Equal(
            discoveredPaths.OrderBy(path => path, StringComparer.Ordinal),
            listedPaths.OrderBy(path => path, StringComparer.Ordinal));
    }

    private static string ReadReadme()
    {
        var root = FindRepositoryRoot();
        return File.ReadAllText(Path.Combine(root, "README.md"))
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace('\r', '\n');
    }

    private static string FindRepositoryRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null && !File.Exists(Path.Combine(current.FullName, "Cayleykit.slnx")))
        {
            current = current.Parent;
        }

        Assert.True(current is not null, "Could not find Cayleykit.slnx above AppContext.BaseDirectory.");
        return current!.FullName;
    }

    private static List<string> ExtractFencedBlocks(string markdown, string language)
    {
        var fence = new string((char)96, 3);
        var marker = fence + language;
        var blocks = new List<string>();
        var lines = markdown.Split('\n');
        for (var index = 0; index < lines.Length; index++)
        {
            if (lines[index] != marker)
            {
                continue;
            }

            var content = new List<string>();
            index++;
            while (index < lines.Length && lines[index] != fence)
            {
                content.Add(lines[index]);
                index++;
            }

            Assert.True(index < lines.Length, $"Unclosed {language} code block.");
            blocks.Add(string.Join("\n", content) + "\n");
        }

        return blocks;
    }

    private static List<(string Path, string Description)> ParseLayoutEntries(IEnumerable<string> lines)
    {
        var entries = new List<(string Path, string Description)>();
        foreach (var line in lines.Where(line => line.StartsWith("- \u0060", StringComparison.Ordinal)))
        {
            var delimiter = line.IndexOf("\u0060 - ", StringComparison.Ordinal);
            Assert.True(delimiter > 2, $"Malformed Layout bullet: {line}");

            var path = line[3..delimiter];
            var description = line[(delimiter + 4)..];
            Assert.DoesNotContain('\\', path);
            Assert.False(string.IsNullOrWhiteSpace(description));
            Assert.EndsWith(".", description);
            Assert.Equal(1, description.Count(character => character == '.'));
            entries.Add((path, description));
        }

        return entries;
    }

    private static bool HasBuildDirectorySegment(string path)
    {
        return path
            .Split([Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar], StringSplitOptions.RemoveEmptyEntries)
            .Any(segment =>
                string.Equals(segment, "bin", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(segment, "obj", StringComparison.OrdinalIgnoreCase));
    }

    private static string NormalizeRelativePath(string path)
    {
        return path.Replace(Path.DirectorySeparatorChar, '/').Replace(Path.AltDirectorySeparatorChar, '/');
    }

    private static void AssertCli(string[] arguments, string expectedOutput)
    {
        var io = new FakeIo();

        var exitCode = Cli.Run(arguments, io);

        Assert.Equal(0, exitCode);
        Assert.Equal(expectedOutput, string.Concat(io.Output));
        Assert.Empty(io.Errors);
        Assert.Equal(0, io.ReadCount);
    }

    private sealed class FakeIo : IIo
    {
        public List<string> Output { get; } = [];

        public List<string> Errors { get; } = [];

        public int ReadCount { get; private set; }

        public byte[] ReadStdin()
        {
            ReadCount++;
            return [];
        }

        public void Out(string text) => Output.Add(text);

        public void Err(string text) => Errors.Add(text);
    }
}
