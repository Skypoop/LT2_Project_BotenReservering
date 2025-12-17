using ProjectBotenReservering.Core.Interfaces.Repositories;
﻿using ProjectBotenReservering.Core.Helpers;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;
using System.Diagnostics;

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
        _competitionBoatsList.Clear();

        if (_boatRepository.Get(boatId) == null)
        {
            throw new ArgumentException($"Boat with ID {boatId} does not exist.", nameof(boatId));
        }

        List<Boat> allBoatsFromName = GetAllCompititionBoatsFromName(boatId);

        if (allBoatsFromName.Count() < amount)
        {
            throw new InvalidOperationException($"Niet genoeg boten beschikbaar, Nodig: {amount}, Beschikbaar: {allBoatsFromName.Count()}");
        }

        for (int i = 0; i < amount; i++)
        {
            _competitionBoatsList.Add(allBoatsFromName[i]);
        }
    }

    private List<Boat> GetAllCompititionBoatsFromName(int id)
    {
        Boat boat = _boatRepository.Get(id);
        List<Boat> allBoatsFromName = _boatRepository.GetAllFromName(boat.Name);

        return allBoatsFromName;
    }

    public List<Boat> GetCompetitionBoats()
    {
        return _competitionBoatsList;
    }
    
    public (bool IsValid, string? ErrorMessage) ValidateCompetition(DateTime start, DateTime end, List<Boat> boats)
    {
        if (!CompetitionValidationHelper.IsCompetitionEndDateValid(start, end))
        {
            return (false, "De einddatum moet later zijn dan de begindatum.");
        }

        if (!CompetitionValidationHelper.IsCompetitionStartDateValid(start))
        {
            return (false, "De begindatum mag niet in het verleden liggen.");
        }

        if (!CompetitionValidationHelper.AreBoatsSelected(boats))
        {
            return (false, "Er zijn geen boten geselecteerd.");
        }

        return (true, null);
    }
}
