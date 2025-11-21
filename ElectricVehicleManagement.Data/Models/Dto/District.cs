namespace ElectricVehicleManagement.Data.Models.Dto;

public class District
{
    public int Code { get; set; }
    public string Name { get; set; }
    public List<Ward>? Wards { get; set; }
}