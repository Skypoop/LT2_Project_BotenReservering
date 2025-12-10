namespace ProjectBotenReservering.Core.Models;

public class Reservation
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int ClientId { get; set; }
    public int BoatId { get; set; }
    public bool Approved { get; set; }
    public bool Active { get; set; }

    public Reservation(DateTime createdAt, DateTime startTime, DateTime endTime, int clientId, int boatId, bool approved, int id = 0, bool active = true)
    {
        Id = id;
        CreatedAt = createdAt;
        StartTime = startTime;
        EndTime = endTime;
        ClientId = clientId;
        BoatId = boatId;
        Approved = approved;
        Active = active;
    }
}
