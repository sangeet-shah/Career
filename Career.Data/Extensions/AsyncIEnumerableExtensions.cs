using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Career.Data.Extensions;

public static class AsyncIEnumerableExtensions
{
    /// <summary>
    /// Projects each element of an async-enumerable sequence into a new form by applying
    /// an asynchronous selector function to each member of the source sequence and awaiting
    /// the result.
    /// </summary>
    /// <typeparam name="TSource"> The type of the elements in the source sequence</typeparam>
    /// <typeparam name="TResult">
    /// The type of the elements in the result sequence, obtained by running the selector
    /// function for each element in the source sequence and awaiting the result.
    /// </typeparam>
    /// <param name="source">A sequence of elements to invoke a transform function on</param>
    /// <param name="predicate">An asynchronous transform function to apply to each source element</param>
    /// <returns>
    /// An async-enumerable sequence whose elements are the result of invoking the transform
    /// function on each element of the source sequence and awaiting the result
    /// </returns>
    public static IAsyncEnumerable<TResult> SelectAwait<TSource, TResult>(this IEnumerable<TSource> source,
        Func<TSource, ValueTask<TResult>> predicate)
    {
        return source.ToAsyncEnumerable().SelectAwait(predicate);
    }

    /// <summary>
    /// Filters the elements of an async-enumerable sequence based on an asynchronous
    /// predicate
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source">An async-enumerable sequence whose elements to filter</param>
    /// <param name="predicate">An asynchronous predicate to test each source element for a condition</param>
    /// <returns>
    /// An async-enumerable sequence that contains elements from the input sequence that
    /// satisfy the condition
    /// </returns>
    public static IAsyncEnumerable<TSource> WhereAwait<TSource>(this IEnumerable<TSource> source,
        Func<TSource, ValueTask<bool>> predicate)
    {
        return source.ToAsyncEnumerable().WhereAwait(predicate);
    }

    /// <summary>
    /// Determines whether any element in an async-enumerable sequence satisfies a condition
    /// </summary>
    /// <typeparam name="TSource">The type of element in the sequence</typeparam>
    /// <param name="source">An async-enumerable sequence whose elements to apply the predicate to</param>
    /// <param name="predicate">An asynchronous predicate to apply to each element of the source sequence</param>
    /// <returns>
    /// A Task containing a value indicating whether any elements in the source
    /// sequence pass the test in the specified predicate
    /// </returns>
    /// <returns>A task that represents the asynchronous operation</returns>
    public static Task<bool> AnyAwaitAsync<TSource>(this IEnumerable<TSource> source,
        Func<TSource, ValueTask<bool>> predicate)
    {
        return source.ToAsyncEnumerable().AnyAwaitAsync(predicate).AsTask();
    }
}
