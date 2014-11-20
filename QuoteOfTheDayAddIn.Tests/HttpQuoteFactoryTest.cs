using System;
using System.Net;
using System.Net.Http;
using System.Text;
using TestStack.BDDfy;
using Xunit;
using Xunit.Should;

namespace QuoteOfTheDayAddIn.Tests
{
    public class HttpQuoteFactoryTest
    {
        [Fact]
        public async void ShouldReturnNullWhenResponseCannotBeParsed()
        {
            var fakeResponseHandler = new FakeResponseHandler();

            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("", Encoding.UTF8)
            };

            fakeResponseHandler
                .AddFakeResponse(new Uri(Constants.QuotesServiceUrl), responseMessage);

            var httpClient = new HttpClient(fakeResponseHandler);

            var quoteFactory = new HttpQuoteFactory(httpClient);

            var result = await quoteFactory.Get();

            result.ShouldBeNull();
        }

        [Fact]
        public async void ShouldParseValidResponseAndReturnQuoteOfTheDay()
        {
            var fakeResponseHandler = new FakeResponseHandler();

            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("<Quotes xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns=\"http://swanandmokashi.com\"><QuoteOfTheDay>Testing is great</QuoteOfTheDay><Author>Dragan</Author></Quotes>", Encoding.UTF8)
            };

            fakeResponseHandler
                .AddFakeResponse(new Uri(Constants.QuotesServiceUrl), responseMessage);

            var httpClient = new HttpClient(fakeResponseHandler);

            var quoteFactory = new HttpQuoteFactory(httpClient);

            var result = await quoteFactory.Get();

            result.ShouldBe(new QuoteOfTheDay { Quote = "Testing is great", Author = "Dragan" });
        }
    }
}