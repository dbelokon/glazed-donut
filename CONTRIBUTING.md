## Requirements

I develop `glazed-donut` in Visual Studio, so most of the installation instructions
that you are going to read here only explain on how to setup the environment for
Visual Studio.

The requirements are:

* Visual Studio 2019 Community Edition,
* .NET SDK, including .NET Core,
* [dotnet-format](https://github.com/dotnet/format)

## Development Installation
1. Clone this repository. You may use an HTTPS link, or the SSH link,
whichever you are most comfortable.
2. When you are done cloning, just open the Solution file, the one that
has the `.sln` extension.
3. Now you have the project open in Visual Studio!

## Building

In order to build, we need to invoke the C# compiler to compile all of our files.
This process is completely automated if you use Visual Studio. You may use
`Ctrl + Shift + B` to trigger the building process. If you want to start it up
to debug something, you can press `F5`, or `Ctrl + F5` if you want to start
`glazed-donut` without a debugging session on.

The executable should be found in `glazed-donut/bin/Debug/netcoreapp3.1`, if you
compiled for a Debug target.

## Formatting

All formatting rules are found in `.editorconfig`. To format your code automatically,
you can press the key combination `Ctrl + K` and then `Ctrl + D`. This will format
the whole file you have currently open.

If you want something more automated, such as formatting when you save your code,
you will have to install a Visual Studio Extension that formats your code when you
save.

However, do mind that the formatting performed by the shorcut or by the extension
is not the same as the formatting performed by `dotnet format`! 

## Linting

For linting the code, we use a combination between the `StyleCop` NuGet package
and the `dotnet-format`. `StyleCop` will provide feedback on the issues it finds,
and it will provide a way to fix it automatically. However, if you want to fix it
all at once, you just need to run the command

```
dotnet format glazed-donut.sln
```

at the root of the project. You may need to run it several times to fix all
possible warnings, because sometimes `dotnet-format` cannot fix all issues
in one sweep.
easily run a linting check process right before committing. 

## Unit Testing

This repository is using [xUnit](https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-dotnet-test) testing framework. 
All the unit tests are located in glazed-donut.Tests solution folder. 

To run unit tests, right click on the solution and select 'Run Tests'. 

When writing tests, please cover all the possible positive and nagative scenarios you can think of. 
Make sure all the tests are passed before you submit a PR. 