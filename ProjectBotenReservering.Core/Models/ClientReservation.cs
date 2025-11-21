namespace ProjectBotenReservering.Core.Models;

public class ClientReservation
{
    public int ClientId { get; set; }
    public int ReservationId { get; set; }
    public bool Approved { get; set; }

    public ClientReservation(int clientId, int reservationId, bool approved)
    {
        ClientId = clientId;
        ReservationId = reservationId;
        Approved = approved;
    }
}

