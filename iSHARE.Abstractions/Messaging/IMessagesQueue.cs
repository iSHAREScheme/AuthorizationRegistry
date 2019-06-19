using System.Threading.Tasks;

namespace iSHARE.Abstractions.Messaging
{
    public interface IMessagesQueue
    {
        Task Enqueue(object message);
        Task<MessageEnvelope<TMessage>> Peek<TMessage>();
        Task DeleteMessage(MessageReference reference);
    }
}
