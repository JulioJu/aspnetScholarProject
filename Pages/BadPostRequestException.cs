namespace Videotheque.Pages
{
  using System;
  using System.Runtime.Serialization;

  // See https://stackoverflow.com/questions/94488/what-is-the-correct-way-to-make-a-custom-net-exception-serializable
  [Serializable]
  // Important: This attribute is NOT inherited from Exception, and MUST be
  // specified otherwise serialization will fail with a SerializationException
  // stating that "Type X in Assembly Y is not marked as serializable."
  public sealed class BadPostRequestException : Exception
  {
    private BadPostRequestException()
    {
    }

    public BadPostRequestException(string message)
      : base("ERROR in POST: " + message)
    {
    }

    public BadPostRequestException(string message, Exception inner)
      : base("ERROR in POST: " + message, inner)
    {
    }

    // Without this constructor, deserialization will fail
    private BadPostRequestException(SerializationInfo info,
        StreamingContext context)
      : base(info, context)
    {
    }
  }
}

// vim:sw=2:ts=2:et:fileformat=dos
