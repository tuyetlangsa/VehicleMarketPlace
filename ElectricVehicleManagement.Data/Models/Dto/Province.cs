namespace ElectricVehicleManagement.Data.Models.Dto;

public class Province
{
    public int Code { get; set; }
    public string Name { get; set; }
    public List<District>? Districts { get; set; }
}