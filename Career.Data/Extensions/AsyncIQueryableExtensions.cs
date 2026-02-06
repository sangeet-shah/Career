using Career.Data.Repository;
using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Career.Data.Extensions;

public static class AsyncIQueryableExtensions
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="source">source</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="getOnlyTotalCount">A value in indicating whether you want to load only total number of records. Set to "true" if you don't want to load data from database</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public static async Task<IPagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, int pageIndex, int pageSize, bool getOnlyTotalCount = false)
    {
        if (source == null)
            return new PagedList<T>(new List<T>(), pageIndex, pageSize);

        //min allowed page size is 1
        pageSize = Math.Max(pageSize, 1);

        var count = await source.CountAsync();

        var data = new List<T>();

        if (!getOnlyTotalCount)
            data.AddRange(await source.Skip(pageIndex * pageSize).Take(pageSize).ToListAsync());

        return new PagedList<T>(data, pageIndex, pageSize, count);
    }

    /// <summary>
    /// Asynchronously loads data from query to a dictionary
    /// </summary>
    /// <typeparam name="TSource">Query element type</typeparam>
    /// <typeparam name="TKey">Dictionary key type</typeparam>
    /// <typeparam name="TElement">Dictionary element type</typeparam>
    /// <param name="source">Source query</param>
    /// <param name="keySelector">Source element key selector</param>
    /// <param name="elementSelector">Dictionary element selector</param>
    /// <param name="comparer">Dictionary key comparer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the dictionary with query results
    /// </returns>
    public static Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(
        this IQueryable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector,
        IEqualityComparer<TKey> comparer = null) where TKey : notnull
    {
        return comparer == null
            ? AsyncExtensions.ToDictionaryAsync(source, keySelector, elementSelector)
            : AsyncExtensions.ToDictionaryAsync(source, keySelector, elementSelector, comparer);
    }

    /// <summary>
    /// Returns the first element of a sequence, or a default value if the sequence contains no elements
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source</typeparam>
    /// <param name="source">Source</param>
    /// <param name="predicate">Predicate</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the default(TSource) if source is empty; otherwise, the first element in source
    /// </returns>
    public static Task<TSource> FirstOrDefaultAsync<TSource>(this IQueryable<TSource> source,
        Expression<Func<TSource, bool>> predicate = null)
    {
        return predicate == null ? AsyncExtensions.FirstOrDefaultAsync(source) : AsyncExtensions.FirstOrDefaultAsync(source, predicate);
    }

    /// <summary>
    /// Asynchronously loads data from query to a list
    /// </summary>
    /// <typeparam name="TSource">Query element type</typeparam>
    /// <param name="source">Source query</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list with query results
    /// </returns>
    public static Task<List<TSource>> ToListAsync<TSource>(this IQueryable<TSource> source)
    {
        return AsyncExtensions.ToListAsync(source);
    }

    /// <summary>
    /// Determines whether any element of a sequence satisfies a condition
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source</typeparam>
    /// <param name="source">A sequence whose elements to test for a condition</param>
    /// <param name="predicate">A function to test each element for a condition</param>
    /// <returns>
    /// true if any elements in the source sequence pass the test in the specified predicate;
    /// otherwise, false
    /// </returns>
    /// <returns>A task that represents the asynchronous operation</returns>
    public static Task<bool> AnyAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate = null)
    {
        return predicate == null ? AsyncExtensions.AnyAsync(source) : AsyncExtensions.AnyAsync(source, predicate);
    }
}
