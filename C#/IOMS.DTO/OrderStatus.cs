namespace IOMS.DTO;

// Az API-n kiadott státusz; szándékosan független a DAL entitás enumjától.
public enum OrderStatus
{
    Pending,
    Shipped,
    Delivered,
    Cancelled
}
