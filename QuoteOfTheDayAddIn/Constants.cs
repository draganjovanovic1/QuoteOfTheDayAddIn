using System;

namespace QuoteOfTheDayAddIn
{
    public static class Constants
    {
        public static readonly DateTime DefaultDateTime = new DateTime(4501, 1, 1);
        public const string QuotesServiceUrl = "http://www.swanandmokashi.com/Homepage/Webservices/QuoteOfTheDay.asmx/GetQuote";
    }
}
