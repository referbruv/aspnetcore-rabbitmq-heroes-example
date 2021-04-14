using System.Collections.Generic;

namespace RabbitHeroes.Contracts
{
    public interface IRepository<T>
    {
        IEnumerable<T> All(int page, int maxRecords);
        T Create(T hero);
        void Delete(long id);
        IEnumerable<T> SearchByName(string heroName);
        T Single(long id);
        T Update(long id, T model);
    }

    public interface IHeroesRepository : IRepository<Hero> { }
}
