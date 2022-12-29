# Visual Studio solution updater

This is a [.NET tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools)
that allows you to easily change the Visual Studio version information in a
solution file using a Visual Studio version number.

![CI Build](https://github.com/craigktreasure/SlnUp/workflows/SlnUp-CI/badge.svg?branch=main)
[![codecov](https://codecov.io/gh/craigktreasure/SlnUp/branch/main/graph/badge.svg?token=SV8DFLV2H4)](https://codecov.io/gh/craigktreasure/SlnUp)
[![NuGet](https://img.shields.io/nuget/v/SlnUp)](https://www.nuget.org/packages/SlnUp/)
[![NuGet](https://img.shields.io/nuget/dt/SlnUp)](https://www.nuget.org/packages/SlnUp/)

- [Visual Studio solution updater](#visual-studio-solution-updater)
  - [Why?](#why)
  - [How to use it](#how-to-use-it)
    - [Install the tool](#install-the-tool)
    - [Update the tool](#update-the-tool)
    - [Run the tool](#run-the-tool)
  - [How does it work?](#how-does-it-work)
  - [What is supported?](#what-is-supported)

## Why?

The Visual Studio Version Selector tool, which determines which version of
Visual Studio to open when you double-click a .sln file, uses the solution
[file header][vs-sln-file-header] to determine which version of Visual Studio
to open.

Updating a solution file with correct version information requires you to
understand the solution file header and the Visual Studio version information.

## How to use it

### Install the tool

```shell
dotnet tool install -g SlnUp
```

### Update the tool

```shell
dotnet tool update -g SlnUp
```

### Run the tool

To view all available options, you can run:

```shell
slnup --help
```

The simplest form is to run the tool (`slnup`) from a directory containing a
single solution (`.sln`) file. This will cause the tool to discover a solution
file in the current directory and update it with the latest version.

```shell
slnup
```

You can also pass a Visual Studio product year (2017, 2019, or 2022) to update
to the latest version for the specified product year.

```shell
slnup 2022
```

You can also specify a specific version of Visual Studio using a two-part
(ex. 17.0) or three-part (17.1.0) version number:

```shell
slnup 17.0
```

A path to a solution file may also be specified using the `-p` or `--path`
parameters. This will be necessary if there is more than one solution file
in the current directory.

```shell
slnup 2022 --path ./path/to/solution.sln
```

If you want to specify the exact version information to be put into the solution
file header, you can specify the version information like so:

```shell
slnup 17.0 --build-version 17.0.31903.59
```

## How does it work?

Visual Studio solution files have a well known [file header][vs-sln-file-header].
The tool knows how to parse and update the solution file header and save the
file. The tool also knows version information for builds of Visual Studio from
Visual Studio 2017 to 2022. The version information specified, is used to update
the file header.

Consider a solution file with the following file header:

```text
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version 16
VisualStudioVersion = 16.0.28701.123
MinimumVisualStudioVersion = 10.0.40219.1
```

This solution file header was last updated using Visual Studio 16 (a.k.a.
Visual Studio 2019) with build number `16.0.28701.123`.

When you double click on a solution file like this, the Visual Studio Version
Selector tool will attempt to locate an installed version of Visual Studio that
most closely matches the version information. If Visual Studio 2019 is installed,
that will likely be used to open the solution file.

Now, let's say you want to update the solution file to be opened with Visual
Studio 2022. You could run the following to update the solution file with
version information for Visual Studio 2022:

```shell
slnup 2022
```

In the case of `2022`, the tool will use the latest known version information
it knows for that version. So, this would cause the tool to update the solution
file header to the following:

```text
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version 17
VisualStudioVersion = 17.0.31903.59
MinimumVisualStudioVersion = 10.0.40219.1
```

Now, when you double click on the updated solution file, the Visual Studio
Version Selector tool will attempt to locate and open the solution file using
Visual Studio 2022.

## What is supported?

- The tool supports updating solution files with file format version `12.00`.
  - If the solution file does not contain a file format version, you will
    receive an error.
  - If the solution file header does not contain version information such as
    `# Visual Studio Version 17`, `VisualStudioVersion = 17.0.31903.59`, or
    `MinimumVisualStudioVersion = 10.0.40219.1`, then those values will be added
    to the file header.
- The tool supports Visual Studio 2017, 2019, and 2022.

[vs-sln-file-header]: https://docs.microsoft.com/visualstudio/extensibility/internals/solution-dot-sln-file?view=vs-2022#file-header "File header"
