# RouteMerger.Gpx

RouteMerger.Gpx is a .NET library for merging GPX files. It provides utilities to combine multiple GPX tracks, routes, and waypoints into a single GPX file, making it easier to manage and process GPS data.

## Features

- Merge multiple GPX files into one
- Preserve tracks, routes, and waypoints
- Support for GPX 1.1 schema
- Easy integration with .NET projects

## Installation

Add a reference to the `RouteMerger.Gpx` package in your solution:
```bash
dotnet add package RouteMerger.Gpx
```

## Usage

```csharp
using RouteMerger.Gpx;

// Example: Merge multiple GPX files
var merger = new GpxMerger();
var mergedGpx = merger.Merge(new[] { "file1.gpx", "file2.gpx" });

// Save merged GPX to file
File.WriteAllText("merged.gpx", mergedGpx);
```

## Requirements
.NET 9.0 or later

## Author
Guus Joppe