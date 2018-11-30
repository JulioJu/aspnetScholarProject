# Teacher's instructions

Cahier des charges n°4 – Gestion vidéothèque

Sujet/contexte :

Le propriétaire d’une vidéothèque vous demande de lui réaliser une application
  pour gérer sa vidéothèque.

Il aimerait bien disposer des fonctionnalités suivantes :
1. Mettre à jours son stock par des nouveaux articles au fur et à mesure qu’ils
   arrivent ;
2. Rechercher les films par réalisateur, catégorie, date de sortie et par nom du
   film
3. Connaître tous les articles en cours de locations et leurs dates de retours
4. Les films loués par une personne
5. Les personnes qui ont loué un film
6. Faire la location de films aux personnes.
7. Éditer une facture à une personne
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
  3. https://github.com/OmniSharp/omnisharp-vim/issues/427
  4. https://github.com/OmniSharp/omnisharp-roslyn/issues/1341
  5. https://github.com/OmniSharp/omnisharp-roslyn/issues/129
  (do not forget to remove package `mono` installed by the Linux Distro).

* Warning, actually (21/11/2018) OmniSharp Vim doesn't work with dotnet versio
    500, but works well with version 403 :-).

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
    export PATH="$PATH:/home/user/.dotnet/tools"
    DOTNET_ROOT=/opt/dotnet
    dotnet dev-certs https
    ```
  *  See also https://github.com/dotnet/cli/issues/9114

* You could watch and run dotnet app simply with:
    ```sh
    export DOTNET_ROOT=/opt/dotnet \
      && export PATH="$PATH:/home/user/.dotnet/tools" \
      && rm -Rf bin/ obj/ && dotnet watch run
    ```
## Langage Version
* Set your c# Language Version in Aspnet.csproj (2.3 is )
* https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/
    c# 7.3 is used from `.NET Core 2.1 SDK 2.1.300 RC1`.
* In `ca.ruleset`
    "ToolsVersion (required): This attribute is required for backwards
    compatibility with other tools that use .ruleset files. In practice it is
    the version of Visual Studio that produced this .ruleset file."
    https://github.com/dotnet/roslyn/blob/master/docs/compilers/Rule%20Set%20Format.md
    * Note: Visual Studio 2017 corresponding to the version 15.

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
* **When `dotnet watch run` is launched, and if no lint warning are shown
    `$ rm -Rf obj/`**

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

* **For Razor:**
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

* **For complete info on Entity Framework (e.g. `Unique`, default value, etc. )**
    https://docs.microsoft.com/en-us/ef/core/

### Create, update (without keep old database) and generate database
* Reference: https://docs.microsoft.com/en-us/ef/core/get-started/aspnetcore/new-db?tabs=netcore-cli
  This is link of notion of "Code First", and "Database First". Thanks Adil ;-).
    ```sh
    sqlcmd -S localhost -U SA
    ```
    ```sql
    -- show databases thanks:
    SELECT name FROM master.sys.databases
    GO
    -- If database "videotheque" exists, drop database thanks:
    DROP database videotheque
    GO
        ```
    then run:
    ```sh
    # If there is Migrations/* files
    dotnet ef migrations remove

    dotnet ef migrations add InitialCreate
    # WARNING:
    # Check that files generated under folder `Migrations` are valid.
    # Actually, at the beginning of the file (line 3) an invalid line is
    # added:
    # `using ;`
    # Simply, delete it.

    dotnet ef database update
    ```

### Inheritance
* https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/inheritance?view=aspnetcore-2.0
    (tutorial not updated for aspnetcore-2.1).
    I choose `Table-per-Concrete Class (TPC) inheritance` in this program.
* TPC inheritance seems to not be implemented by EntityFramework
    https://github.com/aspnet/EntityFrameworkCore/issues/3170
    * **But in Code part, it works well**
        In our code, all fields from `./Entities/AbstractEntity.cs`
        or inserted in other inherited Entities ;-) !!

### Troubleshooting

* *«One of the trickiest problems I encountered when I was just starting ASP.NET
    web development was debugging issues with my web application connecting to
    SQL server, especially when connecting to a local instance of SQL Server.»*
    https://www.loganfranken.com/blog/1345/why-your-web-application-cant-connect-to-sql-server/

* To fix error
    1. `"Cannot open database “DatabaseName” requested by the login. The login failed.
    Login failed for user ‘DOMAIN\Username’.""`
    2. `Exception Details: System.Data.SqlClient.SqlException: Invalid object name 'dbo.BaseCs'`
    * --> READ section above "Create and generate database"

* If we have:
  ```
  $ dotnet ef database update
  Build failed.
  ```
  Then SEE SECTION ABOVE named `#Create, update and generate database`

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


### Date Update and Date Create
* **https://github.com/aspnet/EntityFrameworkCore/issues/10769**
    (code in AppDbContext.cs inspired from it)
* https://stackoverflow.com/questions/36798186/ef-changetracker-entries-where-not-recognized
    For code presented above, do not forget to `using System.Linq`.
* https://docs.microsoft.com/en-us/ef/core/modeling/generated-properties#data-annotations
    Explanations about `[DatabaseGenerated(DatabaseGeneratedOption.Identity)]`
    and `[DatabaseGenerated(DatabaseGeneratedOption.Computed)]`
* https://stackoverflow.com/questions/26001151/override-savechangesasync
    I use method `SaveChangesAsync()`, not `SaveChanges()` like in example presented
    in link above
* https://docs.microsoft.com/en-us/ef/core/api/microsoft.entityframeworkcore.dbcontext.savechangesasync#Microsoft_EntityFrameworkCore_DbContext_SaveChangesAsync_System_Boolean_System_Threading_CancellationToken_
    Explanation about `SaveChangeAsync()`
* https://forums.asp.net/t/2118687.aspx?Problem+with+generate+DateTime+with+Entity+Framework
    (For `[DatabaseGenerated(DatabaseGeneratedOption.Computed)]` otherwise we
    have Cannot insert the value NULL into column )
* https://github.com/aspnet/EntityFrameworkCore/issues/10417
    Default value for parent class (inheritence)

## Dotnet Watcher
```
export PATH="$PATH:/home/user/.dotnet/tools"
export DOTNET_ROOT=/opt/dotnet
dotnet watch run
```
# Other interesting doc
* Comments:
  * https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/xmldoc/recommended-tags-for-documentation-comments
  * https://docs.microsoft.com/en-us/dotnet/csharp/codedoc

* Get URL
    * In cs file (a little complicated)
      * https://forums.asp.net/t/2143295.aspx?Get+HttpContext+Current+in+a+library+project+in+ASP+NET+Core+
          (how get all url parts)
      * https://sites.google.com/site/netcorenote/asp-net-core/get-scheme-url-host
          (do not forget to inject the Service)
      * https://www.carlrippon.com/httpcontext-in-asp-net-core/
          (less interesting)
    * In Page file (very, very easy)
        * https://stackoverflow.com/questions/38437005/how-to-get-current-url-in-view-in-asp-net-core-1-0

# Page/View, Layout, Partial View, View Component

* Page/View, Layout, Partial View
    * https://cpratt.co/how-to-change-the-default-asp-net-mvc-themet/
        (explanation about the three)
    * ~~http://www.dotnet-stuff.com/tutorials/aspnet-mvc/how-to-render-different-layout-in-asp-net-mvc
        Old solution to use Layout only for some Pages
        (should be adapted for Razor Page)~~
        Bad solution, show notes below
    * Partial view:
      * "Unlike MVC view or page rendering, a partial view doesn't run
          `_ViewStart.cshtml`."
      * "In ASP.NET Core MVC, a controller's ViewResult is capable of returning
          either a view or a partial view. An analogous capability is planned for
          Razor Pages in ASP.NET Core 2.2. In Razor Pages, a PageModel can return
          a PartialViewResult."
      * "Partial view file names often begin with an underscore (_). This naming
          convention isn't required, but it helps to visually differentiate
          partial views from views and pages."
      * When the markup is changed in the partial view, it updates the rendered
          output of the markup files that use the partial view.
      * Source https://docs.microsoft.com/en-us/aspnet/core/mvc/views/partial?view=aspnetcore-2.1
    * View Component
        "Don't use a partial view where complex rendering logic or code
        execution is required to render the markup. Instead of a partial view,
        use a view component."
        e.g for Login panel, shopping cart.
    * Layout
        Partial views shouldn't be used to maintain common layout elements. Common
    layout elements should be specified in` _Layout.cshtml` files.

## Notes
* `_Layout.cshtml` can't have its own `_Layout.cshtml.cs` Model.
* Layout could be declared in top of page, not only in `_ViewStart.chtml`.
    As it, it is used only in the Page where is it declared.


# Issue created by me on GitHub

*The four firsts issues are also referenced above in section
  "How to for Linux Users"*
1. https://github.com/OmniSharp/omnisharp-vim/issues/423
2. https://github.com/OmniSharp/omnisharp-vim/issues/425 :
3. https://github.com/OmniSharp/omnisharp-vim/issues/427
4. https://github.com/OmniSharp/omnisharp-roslyn/issues/1341
5. https://github.com/OmniSharp/omnisharp-roslyn/issues/129
6. See https://github.com/aspnet/Docs/issues/9650
  ([razor-page overview] can't have Query String in HTTP POST request method)

# Credits

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
