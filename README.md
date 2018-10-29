# Teacher's instructions

Cahier des charges n°4 – Gestion vidéothèque
Sujet/contexte :
Le propriétaire d’une vidéothèque vous demande de lui réaliser une application pour gérer sa vidéothèque.
Il aimerait bien disposer des fonctionnalités suivantes :
1. Mettre à jours son stock par des nouveaux articles au fur et à mesure qu’ils arrivent ;
2. Rechercher les films par réalisateur, catégorie, date de sortie et par nom du film
3. Connaitre tous les articles en cours de locations et leurs dates de retours
4. Les films loués par une personne
5. Les personnes qui ont loué un film
6. Faire la location de films aux personnes.
7. Editer une facture à une personne
8. Faire un top N des films loués

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
* See before all: https://carlos.mendible.com/2017/08/24/net-core-code-analysis-and-stylecop/
  ```
  dotnet add package Microsoft.CodeAnalysis.FxCopAnalyzers
  dotnet add package StyleCop.Analyzers
  ```
  In `*.csproj` inside a Property Group add:
  ``<CodeAnalysisRuleSet>ca.ruleset</CodeAnalysisRuleSet>``
  * Note from JulioJu: do not forget to declare rules it in `cs.ruleset`
    (see the file).
* See also https://en.wikipedia.org/wiki/FxCop
* and https://en.wikipedia.org/wiki/StyleCop

* List of rules and explanations at
  * https://github.com/DotNetAnalyzers/StyleCopAnalyzers/tree/master/documentation
  * https://docs.microsoft.com/en-us/visualstudio/code-quality
  * https://github.com/dotnet/roslyn-analyzers/blob/master/src/Microsoft.CodeQuality.Analyzers/Microsoft.CodeQuality.Analyzers.md
  * Disable warnings for file: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/preprocessor-directives/preprocessor-pragma-warning
  * All rules for Roslyn http://roslynanalyzersstatus.azurewebsites.net/

* To also add sonar-chsarp:
  * `$ dotnet add package SonarAnalyzer.CSharp`
  * Then in file `*.ruleset` :
    ```
    <Rules AnalyzerId="SonarAnalyser.CSharp" RuleNamespace="SonarAnalyser.CSharp">
    </Rules>
      ```

## Dotnet Watcher

```
export PATH="$PATH:/home/user/.dotnet/tools"
export DOTNET_ROOT=/opt/dotnet
dotnet watch run
```

<!-- vim: sw=2 ts=2 et: set ++fileformat=dos -->
