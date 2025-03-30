namespace EasyUiBackend.Domain.Entities
{
	public abstract class BaseEntity
	{
		public Guid Id { get; set; }
		
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public Guid? CreatedBy { get; set; }

		public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }

		public bool IsActive { get; set; } = true;
	}
}
