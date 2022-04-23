# WARNING

This framework is WIP, I won't bother adding proper docs and version tags at this point.

# Razensoft.Ports

Ports & Adapters framework for Unity

Here's a list of useful stuff to check if you REALLY want to figure what this is about:

- https://github.com/Razenpok/Razensoft.Mediator - this framework is based on that one

- https://github.com/jbogard/MediatR - hey Jimmy, I stole your code

- Read about Hexagonal and Ports & Adapters architectures

- This framework has integration hooks with Zenject and VContainer - go check the code

- MonoInputAdapter are the entry points into your RequestHandlers (use cases). MonoOutputAdapter are the exit points of your RequestHandlers (use cases). Apparently, with this approach you can actually unit test stuff without losing your sanity.

- MonoInputAdapter has a Connect button in editor to help you hook it with UnityEvents

Here's some code

```csharp
public class ClaimBonusClick : MonoInputAdapter<ClaimBonusClick.Command>
{
    public class Command : IRequest { }

    public class CommandHandler : RequestHandler<Command>
    {
        private readonly IOutputPort _output;
        private readonly IAnalyticsProvider _analytics;

        public CommandHandler(IOutputPort output, IAnalyticsProvider analytics)
        {
            _output = output;
            _analytics = analytics;
        }

        protected override void Handle(Command command)
        {
            _analytics.Send("bonus_claimed");
            _output.Send(new ClosePopup());
        }
    }
}

public class ClosePopupAdapter : MonoOutputAdapter<ClosePopup>
{
    [SerializeField] private PopupManager _popupManager;

    protected override void Handle(ClosePopup request)
    {
        _popupManager.CloseActive();
    }
}
```
