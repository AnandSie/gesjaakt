
namespace Domain.Entities.Events;

public class InfoEvent : BaseEvent
{
    public InfoEvent(string Message) : base(Message) { }
}

public class WarningEvent : BaseEvent
{
    public WarningEvent(string Message) : base(Message) { }

}
public class CriticalEvent : BaseEvent
{
    public CriticalEvent(string Message) : base(Message) { }
}

public abstract class BaseEvent(string Message) : EventArgs
{
    public string Message { get; } = Message;
}