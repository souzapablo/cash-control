namespace CashControl.API.Abstractions.Messaging;

public interface ICommand;
public interface ICommand<TResponse> : IBaseCommand;

public interface IBaseCommand;