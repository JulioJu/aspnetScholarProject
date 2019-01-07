# Table of Content
<!-- vim-markdown-toc GFM -->

* [Teacher's instructions](#teachers-instructions)
  * [Notes about my implementation](#notes-about-my-implementation)
* [How to for Linux users](#how-to-for-linux-users)
  * [Langage Version](#langage-version)
  * [Code Analysis and StyleCop](#code-analysis-and-stylecop)
  * [Sql Server](#sql-server)
    * [Create, update (without keep old database) and generate database](#create-update-without-keep-old-database-and-generate-database)
    * [Inheritance](#inheritance)
    * [Troubleshooting](#troubleshooting)
    * [Transact-SQL](#transact-sql)
      * [Create Schema](#create-schema)
  * [Dotnet Watcher](#dotnet-watcher)
  * [Nuget packages](#nuget-packages)
* [How To for the official Quick Start](#how-to-for-the-official-quick-start)
    * [0. Razor Overview](#0-razor-overview)
    * [1. Introduction Tutorial in ASP.NET Core documentation website](#1-introduction-tutorial-in-aspnet-core-documentation-website)
      * [Note about this tutorial](#note-about-this-tutorial)
        * [Integrate Scaffolding code into my very well factorized code](#integrate-scaffolding-code-into-my-very-well-factorized-code)
      * [In Linux, example downloaded](#in-linux-example-downloaded)
    * [1. bis Tutorial from the EntityFramework documentation website](#1-bis-tutorial-from-the-entityframework-documentation-website)
    * [2. Contonso University sample in ASP.NET Core documentation website](#2-contonso-university-sample-in-aspnet-core-documentation-website)
      * [Create the sample with dotnet cli](#create-the-sample-with-dotnet-cli)
      * [In Linux, example downloaded](#in-linux-example-downloaded-1)
    * [3. Asp.Net MVC](#3-aspnet-mvc)
* [Entity Framework, further resources and notes](#entity-framework-further-resources-and-notes)
  * [Model Validation](#model-validation)
  * [Relationship](#relationship)
  * [Date Update and Date Create](#date-update-and-date-create)
* [Page/View, Layout, Partial View, View Component](#pageview-layout-partial-view-view-component)
  * [Notes](#notes)
* [Other interesting doc](#other-interesting-doc)
* [Routing](#routing)
* [Inheritance](#inheritance-1)
* [Issue created by me on GitHub](#issue-created-by-me-on-github)
* [Credits](#credits)
* [TODO](#todo)

<!-- vim-markdown-toc -->

# Teacher's instructions

Cahier des charges n°4 – Gestion vidéothèque

Sujet/contexte :

Le propriétaire d’une vidéothèque vous demande de lui réaliser une application
  pour gérer sa vidéothèque.

Il aimerait bien disposer des fonctionnalités suivantes :
1. (Done) Mettre à jours son stock par des nouveaux articles au fur et à mesure
   qu’ils arrivent ;
2. Rechercher les films par réalisateur, catégorie, date de sortie et par nom du
   film
3. (Done) Connaître tous les articles en cours de locations et leurs dates de
   retours
4. (Done) Les films loués par une personne
5. (Done) Les personnes qui ont loué un film
6. (Done) Faire la location de films aux personnes.
7. Éditer une facture à une personne
8. Faire un top N des films loués

## Notes about my implementation

If it's a real app, the user should buy a printer of barecode. Barecode
  to print should be send to the printer software thanks API
  like  https://www.tec-it.com/en/software/label-printing/label-software/barcode-generator/Default.aspx
  (compatible with Linux).
  On Linux, if the software have not API, we could use `xdotool` to
  automate the process :-).

# How to for Linux users
* Create new project:
  ```sh
  dotnet new web
  dotnet new sln
  dotnet dotnet sln add aspnet.csproj
  ```
* Warning: this command don't generate the file `appsettings.Development.json`
    * I've found the example of this file with the command:
    `$ dotnet new webapp -o RazorPagesMovie`
    * presented in section below:
      * [Introduction Tutorial in ASP.NET Core documentation website](#1-introduction-tutorial-in-aspnet-core-documentation-website)
    * But I didn't see any new log in my console. Default log level
        seems to be already "Information".
        Do not put the `ConnectionStrings` in this file (seems not be read).

* To start well with OmniSharp Vim, see my issue at:
  1. https://github.com/OmniSharp/omnisharp-vim/issues/423
  2. https://github.com/OmniSharp/omnisharp-vim/issues/425 :
  3. https://github.com/OmniSharp/omnisharp-vim/issues/427
  6. https://github.com/OmniSharp/omnisharp-vim/issues/434
  (do not forget to remove package `mono` installed by the Linux Distro).

  * There are others issues about OmniSharp, but not important to start
      on OmniSharp
  * Warning OmniSharp needs 2.1.301 to works perfectly, and this project
      use 2.2.200, we can't use `global.json`. Therefore, actually do not
      forget to patch OmniSharp as explained in issue 434

* Use the following command line before compile or start the code:
    ```sh
    export DOTNET_ROOT=/opt/dotnet \
      && export PATH="$PATH:/home/user/.dotnet/tools" \
      && export ASPNETCORE_ENVIRONMENT=Development
      ```

* ~~Warning, actually (21/11/2018) OmniSharp Vim doesn't work with dotnet version
    500 and later, but works well with version 403 :-).~~
* ~~As version 2.2.100 has a documentation very very better (see section below)
    use old version of `dotnet` installed globally, and when
    you generate a code use version installed thanks
    https://dotnet.microsoft.com/download/thank-you/dotnet-sdk-2.2.100-linux-x64-binaries
    Set the new variable environnements like:
    ```sh
    $ export DOTNET_ROOT=$HOME/dotnet \
      && export PATH=$HOME/dotnet:$PATH \
      && export PATH="$PATH:/home/user/.dotnet/tools" \
      && export ASPNETCORE_ENVIRONMENT=Development
      ```~~

* To finish install of dotnet and avoid error message when you run `dotnet run` "
  ```
  System.InvalidOperationException: Unable to configure HTTPS endpoint.
  No server certificate was specified,
  and the default developer certificate could not be found.
  To generate a developer certificate run 'dotnet dev-certs https'.
  ```
  * The solution: >>> https://github.com/fsharp/FAKE/issues/2075 :
    ```sh
    dotnet tool install --global dotnet-dev-certs
    export PATH="$PATH:/home/user/.dotnet/tools"
    DOTNET_ROOT=/opt/dotnet
    dotnet dev-certs https
    ```
  *  See also https://github.com/dotnet/cli/issues/9114

* You could watch and run dotnet app simply with:
    ```sh
      rm -Rf bin/ obj/ && dotnet watch run
    ```
    (do not forget to export environnement variables presented above)

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

* There are two solutions to remove warnings in the source code.
    * The old `SuppressMessage`
        https://docs.microsoft.com/en-us/visualstudio/code-quality/in-source-suppression-overview?view=vs-2017
    * The new `#pragma warning disable CXXXX`
        https://docs.microsoft.com/en-us/cpp/preprocessor/warning?view=vs-2017
    * See also https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2080

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

* Connection string:
    https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-strings

* Above links are not complete, we have error
    `Configuration.GetSection always returns null`
    Therefore see https://stackoverflow.com/questions/46017593/configuration-getsection-always-returns-null

* LocalDB isn't implemented on Linux
    * https://github.com/aspnet/EntityFrameworkCore/issues/6336
    * Further doc for Windows only:
        https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-2016-express-localdb?view=sql-server-2017

* **For complete info on Entity Framework (e.g. `Unique`, default value, etc. )**
    https://docs.microsoft.com/en-us/ef/core/

* MappedDatatype
    https://docs.microsoft.com/en-us/ef/core/api/microsoft.entityframeworkcore.sqlserver.functionaltests.mappeddatatypes

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


## Dotnet Watcher
```
export PATH="$PATH:/home/user/.dotnet/tools"
export DOTNET_ROOT=/opt/dotnet
dotnet watch run
```
## Nuget packages
* If you manually add a package in `.csprog` file do not forget to run
     `$ dotnet restore && dotnet build`
* Prefer use  `$ dotnet add package`

# How To for the official Quick Start

**Read documentation presented at the section 1., then the section presented
at the section 2 very carefully.**

* Version 2.2 of the doc is more complete and understandable than version 2.1.
    Especially, the tutorial presented in the first section below doesn't exist
    in the version 2.1 of the tutorial, and it's very interesting to understand
    well how works CRUD in Razor Page.

* **Warning, even if this doc present well Razor, do not follow it. Especially
    it violates rule https://en.wikipedia.org/wiki/Don't_repeat_yourself**
    * I've implemented and used Partial Class
        https://en.wikipedia.org/wiki/Don't_repeat_yourself !
    * I've also used Abstract Classes, and Layouts, and Partial View. The code
        is well factorized !!! Can't be more factorized.

### 0. Razor Overview

* Before all you must read, understand and practice
    https://docs.microsoft.com/en-us/aspnet/core/razor-pages/?view=aspnetcore-2.2&tabs=visual-studio-code

### 1. Introduction Tutorial in ASP.NET Core documentation website

#### Note about this tutorial

* https://docs.microsoft.com/en-us/aspnet/core/razor-pages/?view=aspnetcore-2.2&tabs=netcore-cli
* In the left navigation panel, could be seen thanks:
  1. `Tutorials > Web apps >  Razor Pages`
  2. `Web apps > Razor Pages`
* Note: if you don't use `Visual Studio Code`, it works perfectly !  `Visual
    Studio Code tutorial` explain all with the tool `dotnet cli`
* Even if you use ASP.NET 2.1, read the tutorial of ASP.NET 2.2, more complete
    (more pages)
* For command `$ dotnet aspnet-codegenerator razorpage`
    presented at https://docs.microsoft.com/en-us/aspnet/core/tutorials/razor-pages/model?view=aspnetcore-2.2&tabs=visual-studio-code#scaffold-the-movie-model
    do not forget to install the tool, it it isn't already installed:
   `$ dotnet tool install --global dotnet-aspnet-codegenerator`
* Do not forget to read (thanks to say me that Luc)
    1. **https://docs.microsoft.com/en-us/aspnet/core/tutorials/razor-pages/model?view=aspnetcore-2.2&tabs=visual-studio-code**
      * do not use `$ dotnet add package Microsoft.EntityFrameworkCore.SQLite`
      * but `$ dotnet add package Microsoft.EntityFrameworkCore.SqlServer`
      * And `Startup.ConfigurationService` use SqlServer and not SQLite.
    2. **https://docs.microsoft.com/en-us/aspnet/core/tutorials/razor-pages/page?view=aspnetcore-2.2**
    * "Scaffolding" means in French "Génération  de modèle automatique".
    * See also this tutorial in French
      1. https://docs.microsoft.com/fr-fr/aspnet/core/tutorials/razor-pages/model?view=aspnetcore-2.2&tabs=visual-studio-code
      2. https://docs.microsoft.com/fr-fr/aspnet/core/tutorials/razor-pages/page?view=aspnetcore-2.2

##### Integrate Scaffolding code into my very well factorized code

* First of all, see doc above

* Actually, Scaffolding with the current code is broken. Simply
    move `Pages/*` outside the project, then generate the code.
    The error is:
    ```
    There was an error creating a DbContext :Pages/Abstract/DetailsAbstract.cs(14,45): error CS8107: Feature 'private protected' is not available in C# 7
    .0. Please use language version 7.2 or greater.
    ```

* My code is very well factorized, but code generated is not.

* Therefore use the code `./ScriptIntegrateScaffolding.sh NameOfNewEntity`
    ***Customized to fit my needs, with script launched under a
    NeoVim terminal, and with `neovim-remote`***

* Then, manually change `_DetailsParialView.cshtml`, `_FormPartialView.cshtml`,
    `_ShowAllTBody.cshtml`, `ShowAll.cshtml` according in what was Generated in
    `../GeneratedEntity`

* Warning, sometimes generated code isn't correct. For instance, today
     (12/20/2018) generate `Article` Entity generate nonexistent fields
     in `Details.cshtml`. So strange.

* Don't forget that
    `base.ViewData["ForeignKeyId"] = new SelectList(base._db.DependentEntity, …)`
    Isn't cool because it's not strongly typed.
    See https://docs.microsoft.com/en-us/aspnet/core/data/ef-rp/update-related-data?view=aspnetcore-2.2#customize-the-courses-pages

#### In Linux, example downloaded
* I've tested with
    https://github.com/aspnet/Docs/tree/master/aspnetcore/tutorials/razor-pages/razor-pages-start/2.2-stage-samples/RPmovieSearch
    (the more complete)
* Warning:
    https://github.com/aspnet/Docs/issues/9863
    « For Linux users RazorPagesMovie.Models.Movie.ID should be replaced by
      RazorPagesMovie.Models.Movie.Id »

      * An other solution is to add [key] on the line above like this:
      ```cs
      [Key]
      public int ID { get; set; }
      ```
      then:
      ```sh
      rm -R Migrations
      dotnet ef migrations add InitialCreate
      dotnet ef database update
      ```
* Not tested with SQLite, but tested with SQLServer.
* In `appsettings.json` you must replace the `ConnectionsSctrings` by
    ```json
    "ConnectionStrings": {
      "RazorPagesMovieContext": "Server=localhost; Database=RazorPageMovie; user id=SA; password=XXXXXX"
    }

    ```
* At the first line of Startup.cs, you must replace `#define UseSqlite` by
    `#define UseSqlServer`


###  1. bis Tutorial from the EntityFramework documentation website

* https://docs.microsoft.com/en-us/ef/core/get-started/aspnetcore/new-db?tabs=netcore-cli
* As Adil says me, the preceding link teach about "Code First" and not
    "Database First".  This notions has disappeared in the EntityFramework
    Core Documentation, but stay in the EntityFramework 6 Documentation
    (see https://docs.microsoft.com/en-us/ef/ef6/)

### 2. Contonso University sample in ASP.NET Core documentation website

* Warning 1: Contoso University example for MVC is more updated.
    * https://docs.microsoft.com/en-us/aspnet/core/data/ef-rp/update-related-data?view=aspnetcore-2.2#update-the-instructor-pages
    * https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/update-related-data?view=aspnetcore-2.0#update-the-delete-page
    * Ref https://github.com/aspnet/Docs/issues/9237 )
* Warning 2: `many` counterpart of a `many-to-one` relationships are not presented.
    * Following is *not* very interesting for us:
        https://github.com/aspnet/Docs/blob/68928585f451253769edd476b4a915843183bb5e/aspnetcore/data/ef-rp/intro/samples/cu/Pages/Instructors/InstructorCoursesPageModel.cshtml.cs#L33,L71
    * **Don't forget to read the UML diagram at:**
    https://docs.microsoft.com/en-us/aspnet/core/data/ef-rp/complex-data-model?view=aspnetcore-2.2&tabs=visual-studio
    * Solution for one-to-many is presented bellow, in section Relationship.

#### Create the sample with dotnet cli
* https://docs.microsoft.com/en-us/aspnet/core/data/ef-rp/?view=aspnetcore-2.2
* In the left navigation panel, could be seen thanks:
  1. `Tutorials > Data access >  EF Core with Razor Pages`
  2. `Data access > EF Core with Razor Pages`
* To understand command lines used in this tutorial, see tutorial
    presented in the section above.

#### In Linux, example downloaded
https://github.com/aspnet/Docs/tree/master/aspnetcore/data/ef-rp/intro/samples/cu
  (the more complete, referenced in the doc)
  * Note than https://github.com/aspnet/Docs/tree/master/aspnetcore/data/ef-rp/intro/samples/cu21 doesn't work

* Download it thanks:
    `$ svn co https://github.com/aspnet/Docs/trunk/aspnetcore/data/ef-rp/intro/samples/cu`
* You could remove some not interesting files in it.
    `$ rm appsettings1.json appsettings2.json ContosoUniversity1_csproj`
* As LocalDB isn't available on Linux Platforms:
    in appsettings.json change the `ConnectionString` for example like this
    `"DefaultConnection":"Server=localhost; Database=sampleMSUniversity; user id=SA; password=XXXXXX"`
    (see explanations above)
* Run following command to create the Database:
    `$ dotnet ef database update`
    (see above for further explanations)
* Actually (12/5/2018), in `ContosoUniversity.csproj`, line 6 should be changed
    * from : `<PackageReference Include="Microsoft.AspNetCore.All" Version="2.0.9" />`
    * to: `<PackageReference Include="Microsoft.AspNetCore.All" Version="2.1.0" />`
    * See also https://stackoverflow.com/questions/50825242/the-type-or-namespace-name-hosting-does-not-exist-in-the-namespace-microsoft
    * Otherwise we have error: `The type or namespace name 'Hosting' does not
        exist in the namespace 'Microsoft.AspNetCore.Razor' (are you missing an
        assembly reference?)`
* Run the app:
    `$ dotnet run`

* If Migrations files are deleted, then generated again thanks command
    `dotnet ef update`
    I've seen than id has defined in uppercase (`ID`). Maybe see
    https://github.com/aspnet/Docs/issues/9863

### 3. Asp.Net MVC

* https://docs.microsoft.com/en-us/aspnet/core/mvc/overview?view=aspnetcore-2.2
  * As “ Razor Pages is a new **aspect of ASP.NET Core MVC** that makes coding
  page-focused scenarios easier and more productive. ” **don't** forget to read
  parts of
  This **doc is very important** to undertstand.
  * But Razor Page differs from “ *Model-View-Controller approach* ”
  * To understand what is a model, there is a ref to
      https://deviq.com/kinds-of-models/
  * Citations above from
      https://docs.microsoft.com/en-us/aspnet/core/razor-pages/?view=aspnetcore-2.2&tabs=visual-studio-code

* More explanations at:
  1. https://hackernoon.com/asp-net-core-razor-pages-vs-mvc-which-will-create-better-web-apps-in-2018-bd137ae0acaa
  2. https://stackify.com/asp-net-razor-pages-vs-mvc/
      (first sentences are almost the same, but not the end)
  * Razor Page is MVVM
  * To understand the sentence “ *The key difference between Razor pages and MVC
      is that the model and controller code is also included within the Razor
      Page itself.* ”
      don't forget to read also link above

# Entity Framework, further resources and notes

## Model Validation
* https://docs.microsoft.com/en-us/aspnet/core/mvc/models/validation?view=aspnetcore-2.2
* https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations?view=netframework-4.7.2

## Relationship
* in EntityFramework doc
    https://docs.microsoft.com/en-us/ef/core/modeling/relationships
* in Aspnet Core doc
    See Contonso University sample in ASP.NET Core documentation website.
* https://docs.microsoft.com/en-us/ef/core/saving/related-data
    Very synthetic !
* For `many-to-one` relationship, for example to add Articles to Customer
    * For retrieve the Article:
        ```
      Article articleToAdd = await base._db.Articles
          .FindAsync(idParsed);
        ```
    * For `edit`
      in the function:
      `public async Task<IActionResult> OnPostEditAsync(string[] currentlyBorrowed)`
      * To add:
        ```
        base.AbstractEntity.CurrentlyBorrowed.Add(articleToAdd);
        base._db.Attach(articleToAdd).State = EntityState.Modified;
        ```
      * To delete:
          ```
          articleToRemove.BorrowerId = null;
          base._db.Attach(articleToRemove).State = EntityState.Modified;
          ```
          * note doesn't work:
          `base.AbstractEntity.CurrentlyBorrowed.Remove(articleToRemove);`
          or
          `articleToRemove.Borrower = null;`
      * I've found the solution alone, nothing found in StackOverflow.
  * For create an Entity with `many` counterpart simply in the function
      `public async Task<IActionResult> OnPostCreateAsync(string[] currentlyBorrowed)`
      add:
      ```
      base.AbstractEntity.CurrentlyBorrowed.Add(articleToAdd);
      ```
  * For build an array with `input name="nameOfArray"`,
      can't use `type=checkbox` because the array `nameOfArray`
      will contain only inputs with `value="true"` and not inputs with
      `value=false`. Length of `nameOfArray` depends
      of number of `<inputs type="checkbox" name="nameOfArray" value="true" />`
      * e.g. at ./Pages/Customer/_FormPartialView.cshtml
  * See also:
      * https://docs.microsoft.com/en-us/aspnet/core/data/ef-rp/update-related-data?view=aspnetcore-2.2#add-course-assignments-to-the-instructor-edit-page
          (explanation is far of the solution, but could help).
      * https://docs.microsoft.com/en-us/ef/core/saving/related-data#adding-a-graph-of-new-entities
      * “ *FirstOrDefaultAsync is more efficient than SingleOrDefaultAsync at
        fetching one entity:
        In much of the scaffolded code, FindAsync can be used in place of
        FirstOrDefaultAsync.  But if you want to Include other entities, then
        FindAsync is no longer appropriate.* ”
        https://docs.microsoft.com/en-us/aspnet/core/data/ef-rp/crud?view=aspnetcore-2.2

* Note:
    ```
    [Required]
    public int FilmId { get; set; }

    // should not be required
    public Film Film { get; set; }
    ```


## Date Update and Date Create
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
* `_ViewImport.cshtml` could be easy nested, contrary to `ViewStart.cshtml`.
    In front of nested `Layout.cshtml` we must include parent `_Layout.cshtml`
    * See also https://www.mikesdotnetting.com/article/164/nested-layout-pages-with-razor
* In a Layout, we can't use a `@RenderSection` in a `@foreach()` instruction.
    because of https://github.com/aspnet/Razor/issues/702
    (not allow to render multiple time the same section)
    * In the past, there was a solution described https://stackoverflow.com/questions/7931104/call-rendersection-twice .
        * But actually can't works, beacause there is some async call, and the
        result of calling `@Html.Raw(aVariable)` is that the variable is written
        above the `@foreach`.
          ```cshtml
        @{var result = "truc";}
        @foreach (var contact in Model.AbstractEntities)
        {
        <td>@contact.Id</td>
        @Html.Raw(result);
        }
          ```
        ==> "truc" written above @contact.Id several times.
          * And I've found
              https://github.com/aspnet/Mvc/issues/3813#issuecomment-167597645
        * Maybe there is a solution
        https://stackoverflow.com/questions/13858203/add-rendersection-to-html-helper
          But not tested.
* We can't pass a variable between View and Layout, use `Partial View` for it.
    See https://stackoverflow.com/questions/10552502/passing-data-to-a-layout-page

* Note copy and past from `./Pages/PartialView/_ShowAll.cshtml`
    ```cshtml

        @** Following use "_ShowAllTBody.cshtml" of the folder*
         *  where the Page that use the currant Layout is.
         *@
         @await Html.PartialAsync("_ShowAllTbody",
            new ViewDataDictionary(ViewData) { { "field", field } })

        @*
        *  The type or namespace name 'Model' could not be found (are you missing a
        *  using directive or an assembly reference?)
        *  [We can't use @model in the Layout because at this time we can't to
        *    the type]
              @{await Html.PartialAsync("_ShowAllTbody",
                (Model.AbstractEntities)field);}
        *@

        @** Following use "_ShowAllTbody" of the folder
         *  where the current Layout is. We don't want that.
         *  @await Html.PartialAsync("./_ShowAllTbody.cshtml")
         *@
    ```

* Casting ViewData in asp.net MVC
    An example at `./Pages/Customer/_ShowAllTbody.cshtml`
    See https://stackoverflow.com/questions/37050968/casting-viewdata-in-asp-net-mvc

* In Edit.cshtml Razor Page, following doesn't work:
    ```cshtml
    @{
    int myId = 1;
    }
    <input asp-for="@myId" />
    ```
  * The error is:
   "InvalidOperationException: The property 'Id' on entity type 'Customer' has
      a temporary value while attempting to change the entity's state to
      'Modified'.  Either set a permanent value explicitly or ensure that the
      database is configured to generate "
  * Maybe see
      https://docs.microsoft.com/en-us/ef/core/saving/explicit-values-generated-properties
  * Therefore, we don't use a `PartialView` for the Layout
      `./Pages/Layout/_Edit.cshtml` like that:
      ```cshtml
      @await Html.PartialAsync("./_EditPartial.cshtml",
        new ViewDataDictionary(ViewData) { { "id", Model.AbstractEntity.Id } })
      ```
      And like that in `./Pages/Layout/_EditPartial.cshtml`
      ```cshtml
      @{
        int id = (int) ViewData["id"];
      }
      <input asp-for="@id" />
      ```
  * Therefore, in each `Edit.cshtml` Page we must have:
      ```cshtml
      <input asp-for="AbstractEntity.Id" type="hidden" />
      ```

# Other interesting doc
* **API reference:**
    https://docs.microsoft.com/en-us/dotnet/api/?view=aspnetcore-2.1

* Comments:
  * https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/xmldoc/recommended-tags-for-documentation-comments
  * https://docs.microsoft.com/en-us/dotnet/csharp/codedoc

* Get URL
    * ~~In cs file (a little complicated)~~
      * ~~https://forums.asp.net/t/2143295.aspx?Get+HttpContext+Current+in+a+library+project+in+ASP+NET+Core+
          (how get all url parts)~~
      * ~~https://sites.google.com/site/netcorenote/asp-net-core/get-scheme-url-host
          (do not forget to inject the Service)~~
      * ~~https://www.carlrippon.com/httpcontext-in-asp-net-core/
          (less interesting)~~
    * ~~In Page file (very, very easy)
        * https://stackoverflow.com/questions/38437005/how-to-get-current-url-in-view-in-asp-net-core-1-0~~
    * The solution is: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-context?view=aspnetcore-2.2
        Notes
        1. You should use `base.HttpContext.Request.Path`
        2. Doesn't work in constructor.

* Migration  from old dotnet version to a newer
    * See https://docs.microsoft.com/en-us/aspnet/core/migration/
    * Do not forget to update `$ dotnet tool`. See `$ dotnet tool list` to
        see the tools to update.
    * Do not forget to update also tag the `LangVersion` in the `csprog` file.
    * Do not forget to update also the  attribute `ToolsVersion` of the root tag
        `RuleSet` in the `ruleset` file.

* tag helper `form` can't handle HTTP PUT and DELETE method, because
    PUT and DELETE are not valid on HTML forms
    https://softwareengineering.stackexchange.com/questions/114156/why-are-there-are-no-put-and-delete-methods-on-html-forms/264735
    * If you want consume PUT or DELETE Restful API, send POST to back end,
        then send another PUT or DLETe API. Otherwise, use JavaScript.
        Probably, there are workaroynd with `HTML Helper`, but not so
        interesting.
        Seems to be the same for all non JavaScript Client Framework, except
        maybe for Thymeleaf

# Routing

* To get the URL
    * In a Razor Page use `@Context.WhatEver`
    * In a Razor PageModel, you must inject the context in the constructor
        named `IHttpContextAccessor`
    * See examples in the current code.
    * For MVC, maybe see also
        https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/routing?view=aspnetcore-2.2

* Razor Pages - Understanding Handler Methods:
    https://www.mikesdotnetting.com/article/308/razor-pages-understanding-handler-methods

* Razor Pages route and app conventions in ASP.NET Core
    https://docs.microsoft.com/en-us/aspnet/core/razor-pages/razor-pages-conventions?view=aspnetcore-2.2

* We could customize routes thanks https://stackoverflow.com/questions/52317610/localized-page-names-with-asp-net-core-2-1
    See also https://www.mikesdotnetting.com/article/327/customising-routing-conventions-in-razor-pages

* To redirect pages, see https://github.com/aspnet/Mvc/issues/6436

* 404 Not found:
    * https://www.learnrazorpages.com/configuration/custom-errors
    * https://www.reddit.com/r/dotnet/comments/8wtd9r/custom_404_page_in_aspnet_core_21_razor_pages/

# Inheritance

* This code use all powerful c# inheritance.
    See for instance ./Pages/Abstract/DetailsAbstract.cshtml.cs and
    ./Pages/Customer/Details.cshtml.cs

* There are examples about `delegate`, `virtual`, genericity, `Task<T>`, async/await

* I have noticed than:
    * Could not have two OnGetAsync in a same function, even with
        parametric overload
    * PerformSearchInDatabase could not be nullable, because
        we are in a Generic function.
    * PerformSearchInDatabase could not be assign to a default
        callback, default parameter should be evaluated at compiled time.
    * (see ./Pages/Abstract/DetailsAbstract.cshtml.cs for further examples)

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
7. https://github.com/OmniSharp/omnisharp-vim/issues/434
8. https://github.com/aspnet/Docs/issues/9863
  "For Linux users RazorPagesMovie.Models.Movie.ID should be replaced by
9. https://github.com/OmniSharp/omnisharp-roslyn/issues/1358
  RazorPagesMovie.Models.Movie.Id
10. https://github.com/OmniSharp/omnisharp-roslyn/pull/1361
11. https://github.com/OmniSharp/omnisharp-vim/issues/437
12. https://github.com/aspnet/AspNetCore/issues/6329
  [Suggestion] Accessibility for input fields required


# Credits

* Strongly inspired from
    https://docs.microsoft.com/en-us/aspnet/core/razor-pages/?view=aspnetcore-2.1&tabs=netcore-cli
* Base created thanks `dotnet cli 2.1.403`
* Title icon from https://commons.wikimedia.org/wiki/File:Video-film.svg

# TODO
* Understand https://medium.com/bynder-tech/c-why-you-should-use-configureawait-false-in-your-library-code-d7837dce3d7f and the warning `CA2007`
    ```
    <Rules AnalyzerId="AsyncUsageAnalyzers" RuleNamespace="AsyncUsageAnalyzers">
      <Rule Id="UseConfigureAwait" Action="Warning" />
    </Rules>
    ```
* ~~Todo: read and maybe apply
    https://docs.microsoft.com/en-us/aspnet/core/tutorials/razor-pages/page?view=aspnetcore-2.1~~ (Done)

* Add in our code some interesting code of :
    1. https://github.com/aspnet/Docs/tree/master/aspnetcore/tutorials/razor-pages/razor-pages-start/2.2-stage-samples/RPmovieSQLiteNewField
    especially:
      * From `Startup.cs`
    2.  https://github.com/aspnet/Docs/tree/master/aspnetcore/data/ef-rp/intro/samples/cu

* TODO
  * https://docs.microsoft.com/en-us/aspnet/core/data/ef-rp/read-related-data?view=aspnetcore-2.2&tabs=visual-studio#create-an-instructors-page-that-shows-courses-and-enrollments
  * https://docs.microsoft.com/en-us/aspnet/core/data/ef-rp/update-related-data?view=aspnetcore-2.2#customize-the-courses-pages

* ~~TODO (it's an emergency)
  MERGE ALL CRUD OPERATIONS, OTHERWISE TO MUCH VIOLATION OF DRY
  https://en.wikipedia.org/wiki/Don't_repeat_yourself~~

* See TODO in the code
  To simply see all TODO, simply in ./ca.ruleset comment the rule `S1135` .

* Add client-side validation scaffolded.

* Our goal is simply to make a simple app deployable in an intranet,
    for an updated PC browser (Chrome, Firefox, Edge, Safari, not IE).
    We assume that the user will not try to change the client side app
    thanks Developers tools of the Browser. Therefore, if in a client
    side we have `<input type="number" name="xxx" />`, we will not `try` `catch`
    in server side `int.Parse(xxx)`. Like that, all fields
    `<input type="hidden">`are not tested.
    Could be done in a future, therefore this kind of TODO as marked `TODO LOW`

* ~~Todo: change `base.ViewData` to be strong typed (better when we scaffold
    with my script)~~
    not useful. When we have more than 10 films, not cool.

* Check my issue https://github.com/aspnet/AspNetCore/issues/6329
  [Suggestion] Accessibility for input fields required

<!-- vim:sw=2:ts=2:et:fileformat=dos
-->
