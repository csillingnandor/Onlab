using System.ComponentModel.DataAnnotations;

namespace IOMS.DTO;


public record CreateCustomerOrderDto(
    [Range(1, int.MaxValue)] int CustomerId,
    [MinLength(1)] List<CreateCustomerOrderItemDto> Items);

public record CreateCustomerOrderItemDto(
    [Range(1, int.MaxValue)] int ProductId,
    [Range(1, 10_000)] int Quantity);
