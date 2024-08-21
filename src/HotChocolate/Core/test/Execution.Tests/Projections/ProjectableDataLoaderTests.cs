#if NET8_0_OR_GREATER
using CookieCrumble;
using GreenDonut;
using GreenDonut.Projections;
using HotChocolate.Execution.Processing;
using HotChocolate.Execution.TestContext;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Squadron;

namespace HotChocolate.Execution.Projections;

[Collection(PostgresCacheCollectionFixture.DefinitionName)]
public class ProjectableDataLoaderTests(PostgreSqlResource resource)
{
    public PostgreSqlResource Resource { get; } = resource;

    private string CreateConnectionString()
        => Resource.GetConnectionString($"db_{Guid.NewGuid():N}");

    [Fact]
    public async Task Brand_With_Name()
    {
        // Arrange
        var queries = new List<string>();
        var connectionString = CreateConnectionString();
        await CatalogContext.SeedAsync(connectionString);

        // Act
        var result = await new ServiceCollection()
            .AddScoped(_ => queries)
            .AddTransient(_ => new CatalogContext(connectionString))
            .AddGraphQL()
            .AddQueryType<Query>()
            .AddPagingArguments()
            .ModifyRequestOptions(o => o.IncludeExceptionDetails = true)
            .ExecuteRequestAsync(
                """
                {
                    brandById(id: 1) {
                        name
                    }
                }
                """);

        Snapshot.Create()
            .AddSql(queries)
            .AddResult(result)
            .MatchMarkdownSnapshot();
    }

    [Fact]
    public async Task Manual_Include_Of_Brand()
    {
        // Arrange
        var queries = new List<string>();
        var connectionString = CreateConnectionString();
        await CatalogContext.SeedAsync(connectionString);

        // Act
        var result = await new ServiceCollection()
            .AddScoped(_ => queries)
            .AddTransient(_ => new CatalogContext(connectionString))
            .AddGraphQL()
            .AddQueryType<Query>()
            .AddPagingArguments()
            .ModifyRequestOptions(o => o.IncludeExceptionDetails = true)
            .ExecuteRequestAsync(
                """
                {
                    productByIdWithBrand(id: 1) {
                        name
                    }
                }
                """);

        Snapshot.Create()
            .AddSql(queries)
            .AddResult(result)
            .MatchMarkdownSnapshot();
    }

    [Fact(Skip = "Include does not work")]
    public async Task Manual_Include_And_Observe_Brand()
    {
        // Arrange
        var queries = new List<string>();
        var connectionString = CreateConnectionString();
        await CatalogContext.SeedAsync(connectionString);

        // Act
        var result = await new ServiceCollection()
            .AddScoped(_ => queries)
            .AddTransient(_ => new CatalogContext(connectionString))
            .AddGraphQL()
            .AddQueryType<Query>()
            .AddTypeExtension<ProductExtensions>()
            .AddPagingArguments()
            .ModifyRequestOptions(o => o.IncludeExceptionDetails = true)
            .ExecuteRequestAsync(
                """
                {
                    productByIdWithBrandNoSelection(id: 1) {
                        name
                        brand {
                            name
                        }
                    }
                }
                """);

        Snapshot.Create()
            .AddSql(queries)
            .AddResult(result)
            .MatchMarkdownSnapshot();
    }

    [Fact]
    public async Task Do_Not_Project()
    {
        // Arrange
        var queries = new List<string>();
        var connectionString = CreateConnectionString();
        await CatalogContext.SeedAsync(connectionString);

        // Act
        var result = await new ServiceCollection()
            .AddScoped(_ => queries)
            .AddTransient(_ => new CatalogContext(connectionString))
            .AddGraphQL()
            .AddQueryType<Query>()
            .AddPagingArguments()
            .ModifyRequestOptions(o => o.IncludeExceptionDetails = true)
            .ExecuteRequestAsync(
                """
                {
                    brandByIdNoProjection(id: 1) {
                        name
                    }
                }
                """);

        Snapshot.Create()
            .AddSql(queries)
            .AddResult(result)
            .MatchMarkdownSnapshot();
    }

    [Fact]
    public async Task Project_And_Do_Not_Project()
    {
        // Arrange
        var queries = new List<string>();
        var connectionString = CreateConnectionString();
        await CatalogContext.SeedAsync(connectionString);

        // Act
        var result = await new ServiceCollection()
            .AddScoped(_ => queries)
            .AddTransient(_ => new CatalogContext(connectionString))
            .AddGraphQL()
            .AddQueryType<Query>()
            .AddPagingArguments()
            .ModifyRequestOptions(o => o.IncludeExceptionDetails = true)
            .ExecuteRequestAsync(
                """
                {
                    brandByIdNoProjection(id: 1) {
                        name
                    }
                    brandById(id: 1) {
                        name
                    }
                }
                """);

        Snapshot.Create()
            .AddSql(queries)
            .AddResult(result)
            .MatchMarkdownSnapshot();
    }

    [Fact]
    public async Task Product_With_Name_And_Brand_With_Name()
    {
        // Arrange
        var queries = new List<string>();
        var connectionString = CreateConnectionString();
        await CatalogContext.SeedAsync(connectionString);

        // Act
        var result = await new ServiceCollection()
            .AddScoped(_ => queries)
            .AddTransient(_ => new CatalogContext(connectionString))
            .AddGraphQL()
            .AddQueryType<Query>()
            .AddPagingArguments()
            .ModifyRequestOptions(o => o.IncludeExceptionDetails = true)
            .ExecuteRequestAsync(
                """
                {
                    productById(id: 1) {
                        name
                        brand {
                            name
                        }
                    }
                }
                """);

        Snapshot.Create()
            .AddSql(queries)
            .AddResult(result)
            .MatchMarkdownSnapshot();
    }

    [Fact]
    public async Task Force_A_Branch()
    {
        // Arrange
        var queries = new List<string>();
        var connectionString = CreateConnectionString();
        await CatalogContext.SeedAsync(connectionString);

        // Act
        var result = await new ServiceCollection()
            .AddScoped(_ => queries)
            .AddTransient(_ => new CatalogContext(connectionString))
            .AddGraphQL()
            .AddQueryType<Query>()
            .AddPagingArguments()
            .ModifyRequestOptions(o => o.IncludeExceptionDetails = true)
            .ExecuteRequestAsync(
                """
                {
                    a: productById(id: 1) {
                        name
                        brand {
                            name
                        }
                    }
                    b: productById(id: 1) {
                        id
                        brand {
                            id
                        }
                    }
                }
                """);

        Snapshot.Create()
            .AddSql(queries)
            .AddResult(result)
            .MatchMarkdownSnapshot();
    }

    [Fact]
    public async Task Branches_Are_Merged()
    {
        // Arrange
        var queries = new List<string>();
        var connectionString = CreateConnectionString();
        await CatalogContext.SeedAsync(connectionString);

        // Act
        var result = await new ServiceCollection()
            .AddScoped(_ => queries)
            .AddTransient(_ => new CatalogContext(connectionString))
            .AddGraphQL()
            .AddQueryType<Query>()
            .AddPagingArguments()
            .ModifyRequestOptions(o => o.IncludeExceptionDetails = true)
            .ExecuteRequestAsync(
                """
                {
                    a: productById(id: 1) {
                        name
                        brand {
                            name
                        }
                    }
                    b: productById(id: 2) {
                        name
                        brand {
                            name
                        }
                    }
                }
                """);

        Snapshot.Create()
            .AddSql(queries)
            .AddResult(result)
            .MatchMarkdownSnapshot();
    }

    [Fact]
    public async Task Brand_Details_Country_Name()
    {
        // Arrange
        var queries = new List<string>();
        var connectionString = CreateConnectionString();
        await CatalogContext.SeedAsync(connectionString);

        // Act
        var result = await new ServiceCollection()
            .AddScoped(_ => queries)
            .AddTransient(_ => new CatalogContext(connectionString))
            .AddGraphQL()
            .AddQueryType<Query>()
            .AddPagingArguments()
            .ModifyRequestOptions(o => o.IncludeExceptionDetails = true)
            .ExecuteRequestAsync(
                """
                {
                    brandById(id: 1) {
                        name
                        details {
                            country {
                                name
                            }
                        }
                    }
                }
                """);

        Snapshot.Create()
            .AddSql(queries)
            .AddResult(result)
            .MatchMarkdownSnapshot();
    }

    [Fact]
    public async Task Brand_Details_Country_Name_With_Details_As_Custom_Resolver()
    {
        // Arrange
        var queries = new List<string>();
        var connectionString = CreateConnectionString();
        await CatalogContext.SeedAsync(connectionString);

        // Act
        var result = await new ServiceCollection()
            .AddScoped(_ => queries)
            .AddTransient(_ => new CatalogContext(connectionString))
            .AddGraphQL()
            .AddQueryType<Query>()
            .AddTypeExtension<BrandExtensions>()
            .AddPagingArguments()
            .ModifyRequestOptions(o => o.IncludeExceptionDetails = true)
            .ExecuteRequestAsync(
                """
                {
                    brandById(id: 1) {
                        name
                        details {
                            country {
                                name
                            }
                        }
                    }
                }
                """);

        Snapshot.Create()
            .AddSql(queries)
            .AddResult(result)
            .MatchMarkdownSnapshot();
    }

    public class Query
    {
        public async Task<Brand?> GetBrandByIdAsync(
            int id,
            ISelection selection,
            BrandByIdDataLoader brandById,
            CancellationToken cancellationToken)
            => await brandById.Select(selection).LoadAsync(id, cancellationToken);

        public async Task<Brand?> GetBrandByIdNoProjectionAsync(
            int id,
            ISelection selection,
            BrandByIdDataLoader brandById,
            CancellationToken cancellationToken)
            => await brandById.LoadAsync(id, cancellationToken);

        public async Task<Product?> GetProductByIdAsync(
            int id,
            ISelection selection,
            ProductByIdDataLoader productById,
            CancellationToken cancellationToken)
            => await productById.Select(selection).LoadAsync(id, cancellationToken);

        public async Task<Product?> GetProductByIdWithBrandAsync(
            int id,
            ISelection selection,
            ProductByIdDataLoader productById,
            CancellationToken cancellationToken)
            => await productById.Select(selection).Include(c => c.Brand).LoadAsync(id, cancellationToken);

        /*
        public async Task<Product?> GetProductByIdWithBrandNoSelectionAsync(
            int id,
            ISelection selection,
            ProductByIdDataLoader productById,
            CancellationToken cancellationToken)
            => await productById.Include(c => c.Brand).LoadAsync(id, cancellationToken);
            */
    }

    [ExtendObjectType<Brand>]
    public class BrandExtensions
    {
        [BindMember(nameof(Brand.Details))]
        public BrandDetails GetDetails(
            [Parent] Brand brand)
            => new() { Country = new Country { Name = "Germany" } };
    }

    [ExtendObjectType<Product>]
    public class ProductExtensions
    {
        [BindMember(nameof(Product.Brand))]
        public Task<Brand> GetBrandAsync(
            [Parent] Product product,
            BrandByIdDataLoader brandById)
            => brandById.LoadRequiredAsync(product.BrandId);
    }

    public class BrandByIdDataLoader : StatefulBatchDataLoader<int, Brand>
    {
        private readonly IServiceProvider _services;
        private readonly List<string> _queries;

        public BrandByIdDataLoader(IServiceProvider services,
            List<string> queries,
            IBatchScheduler batchScheduler,
            DataLoaderOptions options) : base(batchScheduler, options)
        {
            _services = services;
            _queries = queries;

            PromiseCacheObserver
                .Create<int, Brand, Product>(
                    p =>
                    {
                        if (p.Brand is not null)
                        {
                            return new KeyValuePair<int, Brand>(p.Brand.Id, p.Brand);
                        }

                        return null;
                    },
                    this)
                .Accept(this);
        }

        protected override async Task<IReadOnlyDictionary<int, Brand>> LoadBatchAsync(
            IReadOnlyList<int> keys,
            DataLoaderFetchContext<Brand> context,
            CancellationToken cancellationToken)
        {
            var catalogContext = _services.GetRequiredService<CatalogContext>();

            var query = catalogContext.Brands
                .Where(t => keys.Contains(t.Id))
                .Select(context.GetSelector())
                .SelectKey(b => b.Id);

            lock (_queries)
            {
                _queries.Add(query.ToQueryString());
            }

            var x = await query.ToDictionaryAsync(t => t.Id, cancellationToken);

            return x;
        }
    }

    public class ProductByIdDataLoader(
        IServiceProvider services,
        List<string> queries,
        IBatchScheduler batchScheduler,
        DataLoaderOptions options)
        : StatefulBatchDataLoader<int, Product>(batchScheduler, options)
    {
        protected override async Task<IReadOnlyDictionary<int, Product>> LoadBatchAsync(
            IReadOnlyList<int> keys,
            DataLoaderFetchContext<Product> context,
            CancellationToken cancellationToken)
        {
            var catalogContext = services.GetRequiredService<CatalogContext>();

            var query = catalogContext.Products
                .Where(t => keys.Contains(t.Id))
                .Select(context.GetSelector())
                .SelectKey(b => b.Id);

            lock (queries)
            {
                queries.Add(query.ToQueryString());
            }

            var x = await query.ToDictionaryAsync(t => t.Id, cancellationToken);

            return x;
        }
    }
}

file static class Extensions
{
    public static Snapshot AddSql(this Snapshot snapshot, List<string> queries)
    {
        snapshot.Add(string.Join("\n", queries), "SQL");
        return snapshot;
    }

    public static Snapshot AddResult(this Snapshot snapshot, IExecutionResult result)
    {
        snapshot.Add(result, "Result");
        return snapshot;
    }
}
#endif
