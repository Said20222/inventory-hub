namespace InventoryHub.Web.Models.ViewModels
{
    public sealed class InventoryAccessVm
    {
        public Guid InventoryId { get; set; }
        public string InventoryTitle { get; set; } = "";
        public bool IsPublic { get; set; }

        public string? Query { get; set; }

        public List<UserRow> Writers { get; set; } = new();
        
        public sealed class UserRow
        {
            public string UserId { get; set; } = "";
            public string UserName { get; set; } = "";
            public string? Email { get; set; }
        }
    }
}