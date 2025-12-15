using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Services;

public class CompetitionService : ICompetitionService
{
    private readonly IBoatRepository _boatRepository;

    private List<Boat> _competitionBoatsList = new List<Boat>();

    private int _selectedBoatId;
    private int _amountBoats;
    public int SelectedBoatId
    {
        get { return _selectedBoatId; }
        set
        {
            AddBoatsToCompetition(value, AmountBoats);
        }
    }

    public int AmountBoats
    {
        get { return _amountBoats; }
        set
        {
            _amountBoats = value;
        }
    }

    public CompetitionService(IBoatRepository boatRepository)
    {
        _boatRepository = boatRepository;
    }

    private void AddBoatsToCompetition(int boatId, int amount)
    {
        _competitionBoatsList.Clear();

        for (int i = 0; i < amount; i++)
        {
            _competitionBoatsList.Add(_boatRepository.Get(boatId));
        }
    }

    public List<Boat> GetCompetitionBoats()
    {
        return _competitionBoatsList;
    }
}
