using System.Text.Json.Serialization;

namespace GP.APIs.DTOs
{
    public class ChatRequestDTO
    {
        public string user_input { get; set; }
        public List<ConversationEntry> conversation_history { get; set; }
    }

   
}
