namespace Domain.Enums;
public enum ReservationAction
{
    ReservationCreated = 1,
    ConvertedToNotified = 2,
    ConvertedToFulfilled = 3,
    Expired = 4,
    Canceled = 5
}