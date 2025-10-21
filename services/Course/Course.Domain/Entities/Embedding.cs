namespace Course.Domain.Entities
{
    public class Embedding
    {
        public Guid Id { get; set; }
        public Guid? DocumentId { get; set; }
        public Guid? QApairId { get; set; }
        public int?  ChunkIndex { get; set; }
        public string ChunkText { get; set; } = null!;
        public float[] Vector { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // navigation
        public Document? Document { get; set; }
    }
}