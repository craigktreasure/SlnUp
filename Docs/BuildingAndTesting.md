# Building and testing

## Requirements

### Using Visual Studio

* [Visual Studio 2022 17+](https://visualstudio.microsoft.com/downloads/)
  * Visual Studio 2022 comes with the .NET 6 SDK, you can also install another
    version from [here](https://dotnet.microsoft.com/download/dotnet/6.0).

### Visual Studio Code

* [Visual Studio Code](https://code.visualstudio.com/Download)
  * Install recommended extensions.
* [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)

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

## Managing a tool instalation from local build output

### Install the tool

These instructions assume you have previously [built](#build-the-tool) the tool.

To install the tool, run the following command:

``` shell
dotnet tool install -g VisualStudioSolutionUpdater --add-source ./__packages/NuGet/Release/ --version <version number>
```

### Update the tool

These instructions assume you have previously [built](#build-the-tool) and [installed](#install-the-tool) the tool.

For stable release versions, run the following command:

``` shell
dotnet tool update -g VisualStudioSolutionUpdater --add-source ./__packages/NuGet/Release/
```

For pre-release versions, there is currently no way to update to a pre-release version. See [here](https://github.com/dotnet/sdk/issues/2551) for updates on this issue. For the time being, you need to [uninstall](#uninstall-the-tool) the previous version of the tool and then [install](#install-the-tool) the tool again.

### Uninstall the tool

These instructions assume you have previously [built](#build-the-tool) and [installed](#install-the-tool) the tool.

To uninstall the tool, run the following command:

``` shell
dotnet tool uninstall -g VisualStudioSolutionUpdater
```
