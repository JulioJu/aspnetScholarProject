using System;

namespace Videotheque.Pages
{
  public class BadPostRequestException : Exception
  {

  public BadPostRequestException()
  {
    throw new NotImplementedException();
  }

  public BadPostRequestException(string message)
    : base("ERROR in POST: " + message)
  {

  }

  public BadPostRequestException(string message, Exception inner)
    : base("ERROR in POST: " + message, inner)
  {
  }

  }
}

// vim:sw=2:ts=2:et:fileformat=dos
