namespace Domain.Interfaces
{
    public interface ISearchable
    {
        public bool MatchesKeyword(string keyword);
    }
}