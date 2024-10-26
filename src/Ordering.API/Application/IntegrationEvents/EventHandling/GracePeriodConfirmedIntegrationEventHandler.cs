namespace CollectibleDiecast.Ordering.API.Application.IntegrationEvents.EventHandling;

public class GracePeriodConfirmedIntegrationEventHandler: IIntegrationEventHandler<GracePeriodConfirmedIntegrationEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<GracePeriodConfirmedIntegrationEventHandler> _logger;

    public GracePeriodConfirmedIntegrationEventHandler(
        IMediator mediator,
        ILogger<GracePeriodConfirmedIntegrationEventHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Event handler which confirms that the grace period has been completed and order will not initially be cancelled.
    /// Therefore, the order process continues for validation. 
    /// </summary>
    public async Task Handle(GracePeriodConfirmedIntegrationEvent @event)
    {
        _logger.LogInformation("Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})", @event.Id, @event);

        var command = new SetAwaitingValidationOrderStatusCommand(@event.OrderId);

        _logger.LogInformation(
            "Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
            command.GetGenericTypeName(),
            nameof(command.OrderNumber),
            command.OrderNumber,
            command);

        await _mediator.Send(command);
    }
}
