# How to for Linux users
* Create new project:
    ```
    dotnet new web
    dotnet new sln
    dotnet dotnet sln add aspnet.csproj
    ```

* OmniSharp Vim, see my issue at:
    1. https://github.com/OmniSharp/omnisharp-vim/issues/423
    2. https://github.com/OmniSharp/omnisharp-vim/issues/425 :
        (do not forget to remove package `mono` installed by the Linux Distro).

* To finish install of dotnet and avoid error message when you run `dotnet run` "
    ```
    System.InvalidOperationException: Unable to configure HTTPS endpoint.
    No server certificate was specified,
    and the default developer certificate could not be found.
    To generate a developer certificate run 'dotnet dev-certs https'.
    ```
    * The solution: >>> https://github.com/fsharp/FAKE/issues/2075 :
        ```
        dotnet tool install --global dotnet-dev-certs
        export PATH="$PATH:/home/julioprayer/.dotnet/tools"
        DOTNET_ROOT=/opt/dotnet
        dotnet dev-certs https
            ```
    *  See also https://github.com/dotnet/cli/issues/9114

## Code Analysis and StyleCop
* See https://carlos.mendible.com/2017/08/24/net-core-code-analysis-and-stylecop/
* See also https://en.wikipedia.org/wiki/FxCop
* and https://en.wikipedia.org/wiki/StyleCop

* List of rules and explanations at
    * https://github.com/DotNetAnalyzers/StyleCopAnalyzers/tree/master/documentation
    * https://docs.microsoft.com/en-us/visualstudio/code-quality
    * https://github.com/dotnet/roslyn-analyzers/blob/master/src/Microsoft.CodeQuality.Analyzers/Microsoft.CodeQuality.Analyzers.md
    * Disable warnings for file: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/preprocessor-directives/preprocessor-pragma-warning
    * All rules for Roslyn http://roslynanalyzersstatus.azurewebsites.net/


