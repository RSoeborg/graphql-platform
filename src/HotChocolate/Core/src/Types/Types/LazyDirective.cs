#nullable enable
using HotChocolate.Language;
using HotChocolate.Resolvers;

namespace HotChocolate.Types;

internal sealed class LazyDirective
{
    private readonly DirectiveType _type;
    private readonly DirectiveNode _node;
    private readonly Func<IMiddlewareContext, object> _valueFactory;
    private Directive? _resolvedDirective;

    public LazyDirective(
        DirectiveType type,
        DirectiveNode node,
        Func<IMiddlewareContext, object> valueFactory)
    {
        _type = type;
        _node = node;
        _valueFactory = valueFactory;
    }

    public ValueTask<Directive> ResolveAsync(IMiddlewareContext context)
    {
        if (_resolvedDirective is null)
        {
            var value = _valueFactory(context);
            _resolvedDirective = new Directive(_type, _node, value);
        }
        return new ValueTask<Directive>(_resolvedDirective);
    }
}
