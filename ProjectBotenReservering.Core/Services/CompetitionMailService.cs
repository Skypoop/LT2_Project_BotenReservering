using System.Collections.ObjectModel;
using System.Text;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Services;

public class CompetitionMailService : ICompetitionMailService
{
    private readonly ISmtpMailService _smtpMailService;
    private readonly ResourceLoaderHelper _resourceHelper;

    public CompetitionMailService(ISmtpMailService smtpMailService, ResourceLoaderHelper resourceHelper)
    {
        _smtpMailService = smtpMailService;
        _resourceHelper = resourceHelper;
    }

    public async Task SendCompetitionConfirmationEmailsAsync(CompetitionEmailContext context)
    {
        string rawBody = await ResourceLoaderHelper.LoadEmbeddedResourceAsync("CompetitionConfirmation.html");
        if (string.IsNullOrEmpty(rawBody))
        {
            return;
        }

        foreach (KeyValuePair<int, ObservableCollection<Client>> entry in context.ClientsByBoatId)
        {
            await ProcessTeamEmailsAsync(entry.Key, entry.Value, rawBody, context);
        }
    }

    private async Task ProcessTeamEmailsAsync(int boatId, ObservableCollection<Client> teamMembers, string rawBody, CompetitionEmailContext context)
    {
        if (teamMembers.Count == 0) return;

        string boatName = GetBoatName(boatId, context.CompetitionBoats);
        string teamName = GetTeamName(boatId, context.TeamNameByBoatId);
        string competitorsHtml = BuildCompetitorsHtml(boatId, context);
        string formattedStartTime = context.StartTimeWithPreparation.ToString("dd-MM-yyyy HH:mm");
        string formattedEndTime = context.EndDateTime.ToString("dd-MM-yyyy HH:mm");

        foreach (Client client in teamMembers)
        {
            await PrepareAndSendEmailAsync(client, teamMembers, rawBody, teamName, boatName, formattedStartTime, formattedEndTime, competitorsHtml, context);
        }
    }

    private async Task PrepareAndSendEmailAsync(Client client, IEnumerable<Client> teamMembers, string rawBody, string teamName, string boatName, string startTime, string endTime, string competitorsHtml, CompetitionEmailContext context)
    {
        if (string.IsNullOrEmpty(client.Email)) return;

        string teamMembersHtml = BuildTeamMembersHtml(teamMembers, client.Id);
        string personalizedBody = FormatEmailBody(rawBody, client.FullName, teamName, boatName, startTime, endTime, teamMembersHtml, competitorsHtml, context.CompetitionName);

        await SendEmailToClientAsync(client.Email, personalizedBody, context.CompetitionName);
    }

    private async Task SendEmailToClientAsync(string emailAddress, string body, string competitionName)
    {
        List<string> receivers = new List<string> { emailAddress };
        string subject = $"Wedstrijd Inschrijving: {competitionName}";
        await _smtpMailService.SendMailAsync(receivers, subject, body);
    }

    private string GetBoatName(int boatId, IReadOnlyCollection<Boat> boats)
    {
        Boat? boat = boats.FirstOrDefault((Boat b) => b.Id == boatId);
        return boat?.Name ?? "Onbekende Boot";
    }

    private string GetTeamName(int boatId, IReadOnlyDictionary<int, string> teamNames)
    {
        return teamNames.TryGetValue(boatId, out string? name) ? name : "Naamloos Team";
    }

    private string BuildCompetitorsHtml(int excludedBoatId, CompetitionEmailContext context)
    {
        StringBuilder competitorsSb = new StringBuilder();
        bool hasCompetitors = false;

        foreach (KeyValuePair<int, ObservableCollection<Client>> otherEntry in context.ClientsByBoatId)
        {
            if (otherEntry.Key == excludedBoatId) continue;

            hasCompetitors = true;
            string otherTeamName = GetTeamName(otherEntry.Key, context.TeamNameByBoatId);

            IEnumerable<string> memberNames = otherEntry.Value.Select((Client c) => c.FullName);
            string memberString = string.Join(", ", memberNames);

            competitorsSb.Append($"<p style='margin: 5px 0;'><strong>{otherTeamName}</strong>: {memberString}</p>");
        }

        return hasCompetitors ? competitorsSb.ToString() : "<p>Geen tegenstanders.</p>";
    }

    private string BuildTeamMembersHtml(IEnumerable<Client> teamMembers, int currentClientId)
    {
        StringBuilder myTeamSb = new StringBuilder();
        foreach (Client member in teamMembers)
        {
            string suffix = (member.Id == currentClientId) ? " <strong>(jij)</strong>" : "";
            myTeamSb.Append($"<li>{member.FullName}{suffix}</li>");
        }
        return myTeamSb.ToString();
    }

    private string FormatEmailBody(string template, string clientName, string teamName, string boatName, string startTime, string endTime, string teamMembersHtml, string competitorsHtml, string competitionName)
    {
        return template
            .Replace("{Name}", clientName)
            .Replace("{CompetitionName}", competitionName)
            .Replace("{TeamName}", teamName)
            .Replace("{BoatName}", boatName)
            .Replace("{StartTime}", startTime)
            .Replace("{EndTime}", endTime)
            .Replace("{TeamMembers}", teamMembersHtml)
            .Replace("{Competitors}", competitorsHtml);
    }
}