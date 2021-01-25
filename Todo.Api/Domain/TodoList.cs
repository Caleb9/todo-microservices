using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Todo.Api.Domain
{
    public sealed class TodoList
    {
        public TodoList(
            Guid id,
            string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            }

            Id = id;
            Name = name;
            Items = new List<TodoItem>();
        }

        [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
        private TodoList(
            string id,
            string name)
            : this(Guid.Parse(id), name)
        {
        }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)] 
        public Guid Id { get; private init; }
        
        [UsedImplicitly(ImplicitUseKindFlags.Assign)] 
        public string Name { get; private init; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)] 
        public List<TodoItem> Items { get; private init; }
    }
}