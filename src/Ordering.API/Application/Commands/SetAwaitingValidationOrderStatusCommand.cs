namespace CollectibleDiecast.Ordering.API.Application.Commands;

public record SetAwaitingValidationOrderStatusCommand(int OrderNumber) : IRequest<bool>;
