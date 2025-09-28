namespace Domain.Entities.Events;

public class InfoEvent : BaseEvent
{
    public InfoEvent(string Message) : base(Message, EventLevel.Info) { }
}

public class WarningEvent : BaseEvent
{
    public WarningEvent(string Message) : base(Message, EventLevel.Warning) { }

}

public class ErrorEvent : BaseEvent
{
    public ErrorEvent(string Message) : base(Message, EventLevel.Error) { }

}

public class CriticalEvent : BaseEvent
{
    public CriticalEvent(string Message) : base(Message, EventLevel.Critical) { }
}

public abstract class BaseEvent(string Message, EventLevel level) : EventArgs
{
    public string Message { get; } = Message;
    public EventLevel Level { get; } = level;
}