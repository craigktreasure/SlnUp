namespace SlnUp;

using System;

internal record SlnUpOptions(string SolutionFilePath, Version Version, Version BuildVersion);
