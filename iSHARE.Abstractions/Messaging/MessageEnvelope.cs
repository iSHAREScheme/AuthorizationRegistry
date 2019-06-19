namespace iSHARE.Abstractions.Messaging
{
    public class MessageEnvelope<TMessage>
    {
        public MessageReference Reference { get; set; }
        public TMessage Message { get; set; }
    }
}