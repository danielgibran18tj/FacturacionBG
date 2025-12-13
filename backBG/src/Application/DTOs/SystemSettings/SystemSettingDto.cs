namespace Application.DTOs.SystemSettings
{
    public class SystemSettingDto
    {
        public int Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string DataType { get; set; } = "string";
        public bool IsSystem { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

}
