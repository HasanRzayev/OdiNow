namespace OdiNow.Models;

public enum OrderStatus
{
    Pending = 0,
    Confirmed = 1,
    Ready = 2,
    Completed = 3,
    Cancelled = 4,
    Expired = 5
}

public enum OrderType
{
    Pickup = 0,
    DineIn = 1,
    Delivery = 2
}

public enum PaymentStatus
{
    Pending = 0,
    Succeeded = 1,
    Failed = 2,
    Refunded = 3
}

public enum PaymentType
{
    Deposit = 0,
    Remaining = 1,
    Full = 2
}

public enum NotificationType
{
    General = 0,
    Offer = 1,
    Order = 2,
    System = 3
}

public enum FavoriteType
{
    Restaurant = 0,
    MenuItem = 1,
    Offer = 2
}

public enum VerificationChannel
{
    Sms = 0,
    Call = 1,
    Email = 2
}

public enum TicketClaimStatus
{
    Claimed = 0,
    Redeemed = 1,
    Expired = 2,
    Cancelled = 3
}


