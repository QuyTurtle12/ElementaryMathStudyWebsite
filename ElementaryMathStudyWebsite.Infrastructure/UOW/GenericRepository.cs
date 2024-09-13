﻿using ElementaryMathStudyWebsite.Core.Base;
using Microsoft.EntityFrameworkCore;
using ElementaryMathStudyWebsite.Infrastructure.Context;
using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using System.Linq.Expressions;

namespace ElementaryMathStudyWebsite.Infrastructure.UOW
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly DatabaseContext _context;
        protected readonly DbSet<T> _dbSet;
        public GenericRepository(DatabaseContext dbContext)
        {
            _context = dbContext;
            _dbSet = _context.Set<T>();
        }
        public IQueryable<T> Entities => _context.Set<T>();

        public void Delete(object id)
        {
            T entity = _dbSet.Find(id) ?? throw new Exception();
            _dbSet.Remove(entity);
        }

        public async Task DeleteAsync(object id)
        {
            T entity = await _dbSet.FindAsync(id) ?? throw new Exception();
            _dbSet.Remove(entity);
        }

        public IEnumerable<T> GetAll()
        {
            return _dbSet.AsEnumerable();
        }

        public async Task<IList<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public T? GetById(object id)
        {
            return _dbSet.Find(id);
        }

        public async Task<T?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<BasePaginatedList<T>> GetPagging(IQueryable<T> query, int index, int pageSize)
        {
            query = query.AsNoTracking();
            int count = await query.CountAsync();
            IReadOnlyCollection<T> items = await query.Skip((index - 1) * pageSize).Take(pageSize).ToListAsync();
            return new BasePaginatedList<T>(items, count, index, pageSize);
        }

        public async Task<BasePaginatedList<T>> GetPaggingDto(IEnumerable<T> items, int pageNumber, int pageSize)
        {
            // Convert the collection to a list (to ensure we can count and paginate it in-memory)
            var itemList = items.ToList();

            // Calculate total count of the items
            int count = itemList.Count;

            // Apply pagination
            var pagedItems = itemList
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Return a new paginated list (no async operation here since it's in-memory)
            return new BasePaginatedList<T>(pagedItems, count, pageNumber, pageSize);
        }


        public void Insert(T obj)
        {
            _dbSet.Add(obj);
        }

        public async Task InsertAsync(T obj)
        {
            await _dbSet.AddAsync(obj);
        }

        public void InsertRange(IList<T> obj)
        {
            _dbSet.AddRange(obj);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Update(T obj)
        {
            _dbSet.Entry(obj).State = EntityState.Modified;
        }

        public Task UpdateAsync(T obj)
        {
            return Task.FromResult(_dbSet.Update(obj));
        }

        // New method: FindByConditionAsync
        public async Task<T?> FindByConditionAsync(Expression<Func<T, bool>> expression)
        {
            return await _dbSet.FirstOrDefaultAsync(expression);
        }
        public async Task<T?> FindByConditionWithIncludesAsync(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            // Apply eager loading for all specified navigation properties
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            // Apply the specified condition
            return await query.FirstOrDefaultAsync(expression);
        }

        // New method: GetEntitiesWithCondition
        public IQueryable<T> GetEntitiesWithCondition(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            // Apply eager loading for all specified navigation properties
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            // Apply the specified condition
            return query.Where(expression);
        }

    }
}