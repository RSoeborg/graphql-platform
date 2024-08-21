using GreenDonut;
using GreenDonut.Projections;

namespace HotChocolate.Types;

public static class DataLoaders
{
    [DataLoader(Lookups = [nameof(CreateLookupKey)])]
    public static async Task<IDictionary<int, string>> GetSomeInfoById(
        IReadOnlyList<int> keys)
        => await Task.FromResult(keys.ToDictionary(k => k, k => k + " - some info"));

    public static int CreateLookupKey(string key)
        => default!;

    public static int CreateLookupKey(Guid key)
        => default!;

    [DataLoader]
    public static Task<ILookup<int, string>> GetSomeInfoGroupedById(
        IReadOnlyList<int> keys)
        => default!;

    [DataLoader]
    public static Task<string> GetSomeInfoCacheById(
        int key)
        => default!;

    [DataLoader]
    public static Task<string> GetSomeInfoCacheWithServiceById(
        int key,
        ChapterRepository repository,
        CancellationToken ct)
        => default!;

    [DataLoader]
    public static async Task<IDictionary<int, string>> GetSomeInfoWithServiceById(
        IReadOnlyList<int> keys,
        ChapterRepository repository,
        CancellationToken ct)
        => await Task.FromResult(keys.ToDictionary(k => k, k => k + " - some info"));

    [DataLoader]
    public static async Task<IDictionary<int, string>> GetSomeInfoWithServiceAndRequiredStateById(
        IReadOnlyList<int> keys,
        ChapterRepository repository,
        [DataLoaderState("key")] string state,
        CancellationToken ct)
        => await Task.FromResult(keys.ToDictionary(k => k, k => k + " - some info"));

    [DataLoader]
    public static async Task<IDictionary<int, string>> GetSomeInfoWithServiceAndStateById(
        IReadOnlyList<int> keys,
        ChapterRepository repository,
        [DataLoaderState("key")] string? state,
        CancellationToken ct)
        => await Task.FromResult(keys.ToDictionary(k => k, k => k + " - some info"));

    [DataLoader]
    public static async Task<IDictionary<int, string>> GetSomeInfoWithServiceAndStateWithDefaultById(
        IReadOnlyList<int> keys,
        ChapterRepository repository,
        [DataLoaderState("key")] string state = "123",
        CancellationToken ct = default)
        => await Task.FromResult(keys.ToDictionary(k => k, k => k + " - some info"));

#if NET8_0_OR_GREATER
    [DataLoader]
    public static async Task<IDictionary<int, Author>> GetAuthorById(
        IReadOnlyList<int> keys,
        IQueryable<Author> query,
        ISelectorBuilder selector,
        CancellationToken ct)
        => await Task.FromResult(query.Select(selector).SelectKey(t => t.Id).ToDictionary(t => t.Id));
#endif
}
