namespace easyUiBackend.Domain.Entities
{
	public abstract class BaseEntity<T>
	{
		public T Id { get; set; } = default!;
		public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
		public string? CreatedBy { get; set; }

		public DateTime? UpdatedDate { get; set; }
		public string? UpdatedBy { get; set; }

		public bool IsActive { get; set; } = true;
	}
}
