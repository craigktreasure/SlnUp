namespace SlnUp;

using System;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Class SlnUpOptions.
/// Implements the <see cref="IEquatable{SlnUpOptions}" />
/// </summary>
/// <seealso cref="IEquatable{SlnUpOptions}" />
[ExcludeFromCodeCoverage]
internal record SlnUpOptions(string SolutionFilePath, Version Version, Version BuildVersion);
