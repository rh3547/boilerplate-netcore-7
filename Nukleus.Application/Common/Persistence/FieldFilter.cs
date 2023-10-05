namespace Nukleus.Application.Common.Persistence
{
    public class FieldFilter
    {
        public string? Operator { get; set; }
        public string? Comparator { get; set; }
        public bool? IsCaseSensitive { get; set; }
        public object Value { get; set; }
    }
}
