using System.Text.Json.Serialization;

namespace GP.APIs.DTOs
{
    public class ChatResponseDTO
    {
        public string bot_response { get; set; }
        public List<ConversationEntry> updated_history { get; set; }
    }
}
