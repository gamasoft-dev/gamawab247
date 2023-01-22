using System;
using System.Collections.Generic;

namespace Domain.Entities.DialogMessageEntitties.ValueObjects;

public class Row
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; }
    public string Description { get; set; }
    public int NextMessagePosition { get; set; }
}

public class Section
{
    public string Title { get; set; }
    public List<Row> Rows { get; set; }
}

public class ListAction
{
    public string Button { get; set; }
    public List<Section> Sections { get; set; }
}