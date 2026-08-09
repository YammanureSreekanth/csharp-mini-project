namespace Catalog.ConsoleApp.Domain.Interfaces
{
    public interface ISearchable
    {
        public bool MatchesKeyword(string keyword);
    }
}