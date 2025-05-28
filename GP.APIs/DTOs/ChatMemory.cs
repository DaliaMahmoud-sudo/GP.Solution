namespace GP.APIs.DTOs
{
    public static class ChatMemory
    {
        public static Dictionary<string, List<ConversationEntry>> UserHistories { get; set; } = new();
    }
}
