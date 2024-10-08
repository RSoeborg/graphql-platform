namespace HotChocolate.Types.Descriptors
{
#pragma warning disable 1591
    public class ClassWithInheritdoc(string foo) : BaseClass(foo)
    {
        /// <inheritdoc />
        public override string Foo { get; } = foo;

        /// <inheritdoc />
        public override void Bar(string baz) { }
    }
#pragma warning restore 1591
}
