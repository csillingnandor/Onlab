
namespace IOMS.DTO;

public record CustomerOrderDto(
    int Id,
    int CustomerId,
    string CustomerName,
    DateTime OrderDate,
    OrderStatus Status,
    decimal TotalAmount,
    List<CustomerOrderItemDto> Items);
