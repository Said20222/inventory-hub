namespace InventoryHub.Web.Models
{
    public class ItemFieldValue
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public Item Item { get; set; } = default!;

        public Guid FieldId { get; set; }
        public Field Field { get; set; } = default!;

        public string? StringValue { get; set; }
        public decimal? NumericValue { get; set; }
        public bool? BoolValue { get; set; }
        public string? UrlValue { get; set; }
    }
}
    