namespace ProjectBotenReservering.Core.Models;

public class ManagementTask
{
    public int Id { get; set; }
    public string Name { get; set; }

    public ManagementTask(int id, string name)
    {
        Id = id;
        Name = name;
    }
}

