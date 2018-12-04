namespace Videotheque.Data
{
  // From https://en.wikipedia.org/wiki/Film_genre
  // S2344: Rename this enumeration to remove the 'Enum' suffix.
  #pragma warning disable S2344
  public enum GenreStyleEnum
  {
    Action,
    Adventure,
    Art,
    Biographical,
    Christian,
    Comedy,
    Documentary,
    Drama,
    Erotic,
    Educational,
    Social,
    Epic,
    Experimental,
    Exploitation,
    Fantasy,
    // CA1707: Remove the underscores from member name
    #pragma warning disable CA1707
    Film_noir,
    Gothic,
    Horror,
    Mumblecore,
    Musical,
    Mystery,
    Pornographic,
    Propaganda,
    Reality,
    Romantic,
    // CA1707: Remove the underscores from member name
    #pragma warning disable CA1707
    Science_fiction,
    Thriller,
    Transgressive,
    Trick
  }
}

// vim:sw=2:ts=2:et:fileformat=dos
