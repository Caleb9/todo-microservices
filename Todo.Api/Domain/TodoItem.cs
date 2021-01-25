using System;
using JetBrains.Annotations;

namespace Todo.Api.Domain
{
    public sealed class TodoItem
    {
        public TodoItem(
            Guid id,
            string value,
            Guid todoListId)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));
            }

            Id = id;
            Value = value;
            TodoListId = todoListId;
        }

        [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
        private TodoItem(
            string id,
            string value,
            string todoListId)
            : this(Guid.Parse(id), value, Guid.Parse(todoListId))
        {
        }
        
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public Guid Id { get; private init; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string Value { get; private init; }
        
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public Guid TodoListId { get; private init; }

    }
}