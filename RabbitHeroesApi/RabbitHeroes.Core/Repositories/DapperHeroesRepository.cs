using Dapper;
using RabbitHeroes.Contracts;
using RabbitHeroes.Core.Dapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RabbitHeroes.Core.Repositories
{
    public class DapperHeroesRepository : IHeroesRepository
    {
        private readonly IDapperr _dapperr;
        private readonly ICacheService _cache;

        public DapperHeroesRepository(IDapperr dapperr, ICacheService cache)
        {
            _dapperr = dapperr;
            _cache = cache;
        }

        public IEnumerable<Hero> All(int page, int maxRecords)
        {
            var sql = @"
                SELECT h.Id, h.Category, h.Created, h.HasCape, h.IsAlive, h.Name, h.Powers
                FROM Heroes AS h
                ORDER BY h.Created DESC
                LIMIT @maxRecords OFFSET @page";
            var dp = new DynamicParameters(new { @maxRecords = maxRecords, @page = page });
            return _dapperr.Query<Hero>(sql, dp);
        }


        public Hero Create(Hero hero)
        {
            var sql = @"
            INSERT INTO Heroes (
                Name,
                Category,
                HasCape,
                IsAlive,
                Powers,
                Created)
            VALUES
            (
                @name, @category, @hasCape, @isAlive, @powers, @created
            );
            SELECT h.* FROM Heroes h WHERE h.Id = last_insert_rowid();
        ";
            var dp = new DynamicParameters(new
            {
                @name = hero.Name,
                @category = hero.Category,
                @hasCape = hero.HasCape,
                @isAlive = hero.IsAlive,
                @powers = hero.Powers,
                @created = DateTime.Now
            });

            return _dapperr.Query<Hero>(sql, dp).FirstOrDefault();
        }

        public void Delete(long id)
        {
            var sql = @"DELETE FROM Heroes WHERE Id = @id";
            _dapperr.Execute(sql, new DynamicParameters(new { id }));
        }

        public IEnumerable<Hero> SearchByName(string heroName)
        {
            var sql = @"SELECT h.* FROM Heroes h WHERE h.NAME LIKE %@heroName%";
            var dp = new DynamicParameters(new { heroName });
            return _dapperr.Query<Hero>(sql, dp);
        }

        public Hero Single(long id)
        {
            // TryGet Hero from Cache
            // If not Available pull from DB
            var cached = _cache.Get<Hero>(id.ToString());

            if (cached != null) return cached;
            else
            {
                var sql = @"SELECT h.* FROM Heroes h WHERE h.Id == @id";
                
                var dp = new DynamicParameters(new { id });
                
                var result = _dapperr.Query<Hero>(sql, dp).FirstOrDefault();

                return _cache.Set<Hero>(id.ToString(), result);
            }
        }

        public Hero Update(long id, Hero hero)
        {
            var sql = @"
                UPDATE Heroes SET Name = @name, Category = @category, HasCape = @hasCape, IsAlive = @isAlive, Powers = @powers;
                SELECT h.* FROM Heroes h WHERE h.Id = @id";

            var dp = new DynamicParameters(new
            {
                @id = id,
                @name = hero.Name,
                @category = hero.Category,
                @hasCape = hero.HasCape,
                @isAlive = hero.IsAlive,
                @powers = hero.Powers
            });

            return _dapperr.Query<Hero>(sql, dp).FirstOrDefault();
        }
    }
}
