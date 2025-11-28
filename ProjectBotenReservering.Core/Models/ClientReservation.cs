namespace ProjectBotenReservering.Core.Models;

public class ClientReservation
{
    public int ClientId { get; set; }
    public int ReservationId { get; set; }

    public ClientReservation(int clientId, int reservationId)
    {
        ClientId = clientId;
        ReservationId = reservationId;
    }
}

