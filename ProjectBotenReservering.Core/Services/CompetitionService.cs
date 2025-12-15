using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Services;

public class CompetitionService : ICompetitionService
{
    private readonly IBoatRepository _boatRepository;
    private readonly List<Boat> _competitionBoatsList = [];

    private int _selectedBoatId;
    private int _amountBoats;
    public int SelectedBoatId
    {
        get { return _selectedBoatId; }
        set
        {
            AddBoatsToCompetition(value, AmountBoats);

            _selectedBoatId = value;
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
        if (_boatRepository.Get(boatId) == null)
        {
            throw new ArgumentException($"Boat with ID {boatId} does not exist.", nameof(boatId));
        }

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
