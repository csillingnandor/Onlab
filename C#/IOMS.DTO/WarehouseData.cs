namespace IOMS.DTO;

public class WarehouseData
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int Capacity { get; set; }

    // null, ha a raktárnak nincs megadva helye (ilyenkor a térképen nem jelenik meg)
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    // Van-e belső elrendezése (3D nézet: GET api/warehouses/{id}/layout)
    public bool HasLayout { get; set; }
}
