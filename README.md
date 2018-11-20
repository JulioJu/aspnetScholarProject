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
## Sql Server
* ***DO NOT FORGET TO CHANGE PASSWORD IN*** ./appsettings.json !!!
  * DO NOT FORGET TO NOT PUSH PASSWORD, `.gitignore` locally
  `git update-index --assume-unchanged ./appsettings.json`

* Do not forget than this docs are also available for `dotnet cli`, not only
    for Visual Studio.
  * https://docs.microsoft.com/en-us/aspnet
  * https://docs.microsoft.com/en-us/ef

* How to install and configure `mssql-server` on Linux
    https://computingforgeeks.com/how-to-install-ms-sql-on-ubuntu-18-04-lts/
    For Arch Linux, there is similar packages.
    * See also https://docs.microsoft.com/en-us/sql/linux/sql-server-linux-configure-mssql-conf?view=sql-server-2017
    * https://docs.microsoft.com/en-us/aspnet/web-pages/overview/data/5-working-with-data

* For Razor:
  * (very interesting — very synthetic)
      https://docs.microsoft.com/en-us/aspnet/core/razor-pages/?view=aspnetcore-2.1&tabs=netcore-cli
  * (essential if you don't use Visual Studio.
      As Adil says me, it's for "Code First" and not "Database First")
      https://docs.microsoft.com/en-us/ef/core/get-started/aspnetcore/new-db?tabs=netcore-cli
  * (Very interesting — very synthetic, for Visual Studio)
      https://docs.microsoft.com/en-us/aspnet/core/tutorials/razor-pages/sql?view=aspnetcore-2.1
  * (Very complete, with a sample)
      https://docs.microsoft.com/en-us/aspnet/core/data/ef-rp/?view=aspnetcore-2.1

* Connection string:
    https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-strings

* Above links are not complete, we have error
    `Configuration.GetSection always returns null`
    Therefore see https://stackoverflow.com/questions/46017593/configuration-getsection-always-returns-null

* For LocalDB see:
    See https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-2016-express-localdb?view=sql-server-2017

### Troubleshooting

* *«One of the trickiest problems I encountered when I was just starting ASP.NET
    web development was debugging issues with my web application connecting to
    SQL server, especially when connecting to a local instance of SQL Server.»*
    https://www.loganfranken.com/blog/1345/why-your-web-application-cant-connect-to-sql-server/

* To fix error
    1. `"Cannot open database “DatabaseName” requested by the login. The login failed.
    Login failed for user ‘DOMAIN\Username’.""`
    2. `Exception Details: System.Data.SqlClient.SqlException: Invalid object name 'dbo.BaseCs'`
    * READ and apply https://docs.microsoft.com/en-us/ef/core/get-started/aspnetcore/new-db?tabs=netcore-cli
        This is link of notion of "Code First", and "Database First". Thanks
        Adil ;-).


### Transact-SQL
* https://www.w3schools.com/sql/
* https://docs.microsoft.com/en-us/sql/t-sql/lesson-1-creating-database-objects?view=sql-server-linux-2017


* To show tables:
    ``
    Select Table_name as "Table name"
    From Information_schema.Tables
    Where Table_type = 'BASE TABLE' and Objectproperty
    (Object_id(Table_name), 'IsMsShipped') = 0
    ``
    Source: https://stackoverflow.com/questions/124205/how-can-i-do-the-equivalent-of-show-tables-in-t-sql


#### Create Schema

Tu use this file, connect sqlcmd:
```
$ sqlcmd -S localhost -U SA
```
Then execute:
```
:r BuildSchema.sql
```
https://stackoverflow.com/questions/8831651/is-it-possible-to-reference-other-scripts-in-a-tsql-script-file
https://docs.microsoft.com/en-us/sql/relational-databases/scripting/edit-sqlcmd-scripts-with-query-editor?view=sql-server-linux-2017

## Dotnet Watcher

```
export PATH="$PATH:/home/user/.dotnet/tools"
export DOTNET_ROOT=/opt/dotnet
dotnet watch run
```
## Issue on GitHub
* See https://github.com/aspnet/Docs/issues/9650

## Credits

* Strongly inspired from
    https://docs.microsoft.com/en-us/aspnet/core/razor-pages/?view=aspnetcore-2.1&tabs=netcore-cli
* Base created thanks `dotnet cli 2.1.403`

# TODO
* Understand https://medium.com/bynder-tech/c-why-you-should-use-configureawait-false-in-your-library-code-d7837dce3d7f and the warning `CA2007`
    ```
    <Rules AnalyzerId="AsyncUsageAnalyzers" RuleNamespace="AsyncUsageAnalyzers">
      <Rule Id="UseConfigureAwait" Action="Warning" />
    </Rules>
    ```

<!-- vim:sw=2:ts=2:et:fileformat=dos
-->
