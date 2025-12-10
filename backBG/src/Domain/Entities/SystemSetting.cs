using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class SystemSetting
{
    public int Id { get; set; }

    public string SettingKey { get; set; } = string.Empty;

    public string SettingValue { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string DataType { get; set; } = "string";

    public bool IsSystem { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
