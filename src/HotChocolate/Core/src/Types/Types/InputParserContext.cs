#nullable enable
using HotChocolate.Execution;
using HotChocolate.Language;

namespace HotChocolate.Types;

public sealed class InputParserContext
{
    public InputParserContext(IVariableValueCollection? variables = null)
    {
        Variables = variables;
    }

    public IVariableValueCollection? Variables { get; }
}
