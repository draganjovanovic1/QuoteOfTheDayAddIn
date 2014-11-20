using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace QuoteOfTheDayAddIn
{
    public class HttpQuoteFactory : IQuoteFactory
    {
        public HttpQuoteFactory(HttpClient httpClient)
        {
            if (httpClient == null)
                throw new ArgumentNullException("httpClient");

            _httpClient = httpClient;
        }

        public async Task<QuoteOfTheDay> Get()
        {
            var rawText = await _httpClient
                .GetStringAsync(Constants.QuotesServiceUrl);

            return ParseFromResult(rawText);
        }

        private QuoteOfTheDay ParseFromResult(string unparsedText)
        {
            var xdoc = XDocument.Parse(unparsedText);

            var quoteNode = xdoc.Descendants()
                .FirstOrDefault(d => d.Name.LocalName == "QuoteOfTheDay");

            var authorNode = xdoc.Descendants()
                .FirstOrDefault(d => d.Name.LocalName == "Author");

            if (quoteNode == null || authorNode == null)
                return null;

            return new QuoteOfTheDay
            {
                Quote = quoteNode.Value,
                Author = authorNode.Value
            };
        }

        private readonly HttpClient _httpClient;
    }
}