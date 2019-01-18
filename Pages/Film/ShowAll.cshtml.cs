namespace Videotheque.Pages.FilmPage
{
  using System;
  using System.Linq;
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.EntityFrameworkCore;
  using Videotheque.Data;
  using Videotheque.Pages.Abstract;

  public class ShowAll : ShowAllAbstract<Film>
  {
    public ShowAll(AppDbContext db)
      : base(db.Films)
    {
    }

    [BindProperty(SupportsGet = true)]
    public string TopN { get; set; }

    [BindProperty(SupportsGet = true)]
    public string SearchByTitle { get; set; }

    [BindProperty(SupportsGet = true)]
    public string SearchByDirectedBy { get; set; }

    [BindProperty(SupportsGet = true)]
    public string SearchByStyle { get; set; }

    [BindProperty(SupportsGet = true)]
    public string SearchReleaseAfter { get; set; }

    [BindProperty(SupportsGet = true)]
    public string SearchReleaseBefore { get; set; }

    protected private override IQueryable<Film> CompleteQueryable()
    {
      return base.CompleteQueryable()
        .Include(f => f.Articles);
    }

    /// <summary>If variable == -1 or a string, works</summary>
    public async Task<IActionResult> OnGetSearchAsync()
    {
      this.Message = "Ordered by ";
      int topNParsed = 1000;
      if (!string.IsNullOrEmpty(this.TopN))
      {
        if (!int.TryParse(this.TopN, out topNParsed))
        {
          return base
            .BadRequest("Error in URL, can't cast query param TopN to int.");
        }
        topNParsed = topNParsed > 0 ? topNParsed : 1000;
      }

      IQueryable<Film> filmIQ = this.CompleteQueryable();

      if (!string.IsNullOrEmpty(this.SearchByTitle))
      {
        // warn: Microsoft.EntityFrameworkCore.Query[20500] The LINQ expression
        // 'where [f].Title.Contains(__SearchByTitle_0,
        // InvariantCultureIgnoreCase)' could not be translated and will be
        // evaluated locally.
         // CA1307: The behavior of 'string.Contains(string)' could vary based on
        // the current user's locale settings.
        #pragma warning disable CA1307
        filmIQ = filmIQ.Where(f => f.Title.Contains(this.SearchByTitle))
          .OrderBy(f => f.Title);
          base.Message += "Title";
      }

      if (!string.IsNullOrEmpty(this.SearchByDirectedBy))
      {
        // warn: Microsoft.EntityFrameworkCore.Query[20500] The LINQ expression
        // 'where [f].Title.Contains(__SearchByTitle_0,
        // InvariantCultureIgnoreCase)' could not be translated and will be
        // evaluated locally.
         // CA1307: The behavior of 'string.Contains(string)' could vary based on
        // the current user's locale settings.
        #pragma warning disable CA1307
        filmIQ = filmIQ
          .Where(f => f.DirectedBy.Contains(this.SearchByDirectedBy))
          .OrderBy(f => f.DirectedBy);
        base.Message = base.Message.Equals("Ordered by ",
            System.StringComparison.InvariantCultureIgnoreCase)
          ? "DirectedBy"
          // SA1002: Semicolons must not be preceded by a space
          #pragma warning disable SA1002
          : "DirectedBy" + " then by " + this.Message ;
      }

      if (!string.IsNullOrEmpty(this.SearchByStyle))
      {
        GenreStyleEnum genreStyle;
        if (!GenreStyleEnum.TryParse(this.SearchByStyle, out genreStyle))
        {
          return base
            .BadRequest("Error in URL, can't cast param "
                + " Style to GenreStyleEnum.");
        }
        filmIQ = filmIQ.Where(f => f.GenreStyle.Equals(genreStyle))
          .OrderBy(f => f.GenreStyle);
        base.Message = base.Message.Equals("Ordered by ",
            System.StringComparison.InvariantCultureIgnoreCase)
          ? "style"
          // SA1002: Semicolons must not be preceded by a space
          #pragma warning disable SA1002
          : "style" + " then by " + this.Message ;
      }

      if (!string.IsNullOrEmpty(this.SearchReleaseAfter))
      {
        DateTime releaseAfter;
        if (!DateTime.TryParse(this.SearchReleaseAfter, out releaseAfter))
        {
          return base
            .BadRequest("Error in URL, can't cast param "
                + " SearchReleaseAfter to DateTime.");
        }
        filmIQ = filmIQ.Where(f => f.ReleaseDate >= releaseAfter)
          .OrderBy(f => f.GenreStyle);
        base.Message = base.Message.Equals("Ordered by ",
            System.StringComparison.InvariantCultureIgnoreCase)
          ? "release after (include) " + releaseAfter
          // SA1002: Semicolons must not be preceded by a space
          #pragma warning disable SA1002
          : "release after (include)"
            + releaseAfter + " then by " + this.Message ;
      }

      if (!string.IsNullOrEmpty(this.SearchReleaseBefore))
      {
        DateTime releaseBefore;
        if (!DateTime.TryParse(this.SearchReleaseBefore, out releaseBefore))
        {
          return base
            .BadRequest("Error in URL, can't cast param "
                + " SearchReleaseBefore to DateTime.");
        }
        filmIQ = filmIQ.Where(f => f.ReleaseDate <= releaseBefore)
          .OrderBy(f => f.GenreStyle);
        base.Message = base.Message.Equals("Ordered by ",
            System.StringComparison.InvariantCultureIgnoreCase)
          ? "release before (include) " + releaseBefore
          : "release before (include) "
            + releaseBefore + " then by " + this.Message ;
      }

      base.AbstractEntities = await filmIQ
        .Take(topNParsed)
        .ToListAsync();

      if (!string.IsNullOrEmpty(this.TopN))
      {
        base.AbstractEntities.Sort((x, y) =>
        {
          base.Message = "Ordered by CountBorrowing.";
          return y.CountBorrowing().CompareTo(x.CountBorrowing());
        });
      }

      return base.Page();
    }

  }
}

// vim:sw=2:ts=2:et:fileformat=dos
