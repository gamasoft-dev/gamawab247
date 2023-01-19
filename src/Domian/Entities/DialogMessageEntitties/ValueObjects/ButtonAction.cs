using System;
using System.Collections.Generic;

namespace Domain.Entities.DialogMessageEntitties.ValueObjects;

    public class ButtonAction
    { 
        public List<Button> Buttons { get; set; }
    }

    /// <summary>
    /// This is the option entity for reply.
    /// </summary>
    public class Button
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public int NextMessagePosition { get; set; } = 2;
    }