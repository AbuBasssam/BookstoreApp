namespace Domain.Enums;

public enum BookActionType : byte
{
    OpenReservation = 1,
    CloseReservation = 2,
    AddCopy = 3,
    DeleteCopy = 4,
    UpdateBookInfo = 5
}