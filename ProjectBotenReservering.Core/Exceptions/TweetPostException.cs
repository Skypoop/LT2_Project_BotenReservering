namespace ProjectBotenReservering.Core.Exceptions;

public class TweetPostException : Exception
{
    public TweetPostException(string message) : base(message) { }
    public TweetPostException(string message, Exception inner) : base(message, inner) { }
}