namespace Authentication.Domain.Entities
{
    public class Role : BaseEntity
    {
        public string RoleName { get; set; } = null!;

        public string? Description { get; set; }
        public virtual ICollection<Account> Users { get; set; } = new List<Account>();
    }
}
