using MarkdownInk.Mcp;
using Shouldly;

namespace MarkdownInk.Mcp.Tests;

/// <summary>
/// Unit tests for <see cref="MarkdownInkServerInfo.ResolveVersion"/> — the semver reported in the
/// MCP initialize handshake must be the real package version, not the MinVer-pinned AssemblyVersion.
/// </summary>
public class ServerInfoTests
{
    [Fact]
    public void ResolveVersion_Strips_Build_Metadata_Suffix()
    {
        MarkdownInkServerInfo.ResolveVersion("1.2.0+abc1234", new Version(1, 0, 0, 0))
            .ShouldBe("1.2.0");
    }

    [Fact]
    public void ResolveVersion_Keeps_Prerelease_Label()
    {
        MarkdownInkServerInfo.ResolveVersion("1.2.0-alpha.3", new Version(1, 0, 0, 0))
            .ShouldBe("1.2.0-alpha.3");
    }

    [Fact]
    public void ResolveVersion_Falls_Back_To_AssemblyVersion_When_No_Informational()
    {
        MarkdownInkServerInfo.ResolveVersion(null, new Version(1, 2, 3, 4))
            .ShouldBe("1.2.3.4");
    }

    [Fact]
    public void Create_Reports_The_Assembly_Name()
    {
        MarkdownInkServerInfo.Create().Name.ShouldBe("MarkdownInk.Mcp");
    }
}
