namespace EasyUiBackend.Domain.Entities
{
	public abstract class BaseEntity
	{
		public Guid Id { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public string? CreatedBy { get; set; }

		public DateTime? UpdatedAt { get; set; }
		public string? UpdatedBy { get; set; }

		public bool IsActive { get; set; } = true;
	}
}
