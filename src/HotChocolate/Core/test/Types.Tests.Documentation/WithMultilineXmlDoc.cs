namespace HotChocolate.Types.Descriptors
{
#pragma warning disable 1591
    public class WithMultilineXmlDoc(string foo)
    {
        /// <summary>
        /// Query and manages users.
        ///
        /// Please note:
        /// * Users ...
        /// * Users ...
        ///     * Users ...
        ///     * Users ...
        ///
        /// You need one of the following role: Owner,
        /// Editor, use XYZ to manage permissions.
        /// </summary>
        public string Foo { get; set; } = foo;
    }
#pragma warning restore 1591
}
