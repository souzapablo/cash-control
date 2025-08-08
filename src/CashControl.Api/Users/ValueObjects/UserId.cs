using CashControl.Api.Abstractions;

namespace CashControl.Api.Users.ValueObjects;

public sealed class UserId(Guid value) : EntityId<UserId>(value) { }