namespace ConfigTesting.Models
{
    public record ConfigChangeResponse
    {
        /// <summary>
        /// The value before the IConfiguration changed
        /// </summary>
        public string OldValue { get; init; } = string.Empty;

        /// <summary>
        /// The value after the IConfiguration changed
        /// </summary>
        public string NewValue { get; init; } = string.Empty;

        /// <summary>
        /// The value of the binded property before the IConfiguration changed
        /// </summary>
        public string BindBeforeChange { get; init; } = string.Empty;

        /// <summary>
        /// The value of the binded property after the IConfiguration changed
        /// </summary>
        public string BindAfterChange { get; init; } = string.Empty;
    }
}