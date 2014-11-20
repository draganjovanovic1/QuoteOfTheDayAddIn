using System.Threading.Tasks;

namespace QuoteOfTheDayAddIn
{
    public interface IQuoteFactory
    {
        Task<QuoteOfTheDay> Get();
    }
}