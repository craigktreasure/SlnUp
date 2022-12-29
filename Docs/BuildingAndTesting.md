# Building and testing

## Requirements

### Using Visual Studio

* [Visual Studio 2022 17.4+][download-vs]
  * You'll also need the [.NET 6 SDK][download-dotnet-6-sdk] and [.NET 7 SDK][download-dotnet-7-sdk].

### Visual Studio Code

* [Visual Studio Code][download-vs-code]
  * Install recommended extensions.
* [.NET 6 SDK][download-dotnet-6-sdk]
* [.NET 7 SDK][download-dotnet-7-sdk]

## Build the tool

To build the tool, run the following command:

``` shell
dotnet build -c Release
```

The tool will be packed into a `nupkg` file at `./__packages/NuGet/Release/`.

## Run the tests

To run all the tests, simply run the following command:

``` shell
dotnet test
```

## Managing a tool installation from local build output

### Install the tool

These instructions assume you have previously [built](#build-the-tool) the tool.

To install the tool, run the following command:

``` shell
dotnet tool install -g SlnUp --add-source ./__packages/NuGet/Release/ --version <version number>
```

### Update the tool

These instructions assume you have previously [built](#build-the-tool) and [installed](#install-the-tool) the tool.

For stable release versions, run the following command:

``` shell
dotnet tool update -g SlnUp --add-source ./__packages/NuGet/Release/
```

For pre-release versions, you need to specify the `--prerelase` argument.

### Uninstall the tool

These instructions assume you have previously [built](#build-the-tool) and [installed](#install-the-tool) the tool.

To uninstall the tool, run the following command:

``` shell
dotnet tool uninstall -g SlnUp
```

## Updating Visual Studio versions

The VisualStudio.VersionScraper tool is used to retrieve and update the Visual Studio versions bundled with SlnUp.

Simply run the following PowerShell script:

```powershell
./scripts/UpdateVSVersions.ps1
```

[download-dotnet-6-sdk]: https://dotnet.microsoft.com/download/dotnet/6.0 "Download .NET 6.0"
[download-dotnet-7-sdk]: https://dotnet.microsoft.com/download/dotnet/7.0 "Download .NET 7.0"
[download-vs]: https://visualstudio.microsoft.com/downloads/ "Download Visual Studio"
[download-vs-code]: https://code.visualstudio.com/Download "Download Visual Studio Code"
