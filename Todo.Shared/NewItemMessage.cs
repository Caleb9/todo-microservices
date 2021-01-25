using System;

namespace Todo.Shared
{
    public sealed record NewItemMessage(Guid ListId, Guid ItemId);
}