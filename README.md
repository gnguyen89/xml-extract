# XML-Extract

This project was created with .NET Core 2.0 (via VSCode).

There are two projects.


## XmlExtract

The main project. First restore it with `dotnet restore` and then run with `dotnet run`. The debug settings for VSCode is also included.

`https://localhost:5001/api/extract` is the api route. It accepts raw text format.


## XmlExtract.Tests

The test project. 
`dotnet restore` to install missing dependencies.
`dotnet test` to run all tests.