namespace Domain.Enums;
public enum ReservationAction
{
    ReservationCreated = 1,
    ConvertedToFulfilled = 2,
    ConvertedToNotified = 3,
    Expired = 4,
    Canceled = 5
}