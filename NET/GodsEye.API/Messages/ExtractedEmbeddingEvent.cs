namespace GodsEye.API.Messages
{
    public class ExtractedEmbeddingEvent
    {
        public int CameraId { get; set; }
        public float[] Embedding { get; set; }
        public DateTime IdentifiedAt { get; set; }
    }
}
