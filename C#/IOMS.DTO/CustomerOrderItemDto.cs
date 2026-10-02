namespace IOMS.DTO;

public record CustomerOrderItemDto(
    int Id,
    int CustomerOrderId,
    int ProductId,
    string ProductName,
    decimal ProductPrice,
    int Quantity);
