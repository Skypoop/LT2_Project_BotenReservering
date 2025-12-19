using System.Collections.ObjectModel;
using System.Text;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.App.Helpers;
public static class EmailTemplateHelper
{
    public static async Task<(string Subject, string Body)> RenderCompetitionConfirmationAsync(CompetitionEmailContext context, Client currentClient, int boatId)
    {
        string rawBody = await ResourceLoaderHelper.LoadEmbeddedResourceAsync("CompetitionConfirmation.html");

        if (string.IsNullOrEmpty(rawBody))
        {
            return (string.Empty, string.Empty);
        }

        string boatName = GetBoatName(boatId, context.CompetitionBoats);
        string teamName = GetTeamName(boatId, context.TeamNameByBoatId);
        string competitorsHtml = BuildCompetitorsHtml(boatId, context);
        string formattedStartTime = context.StartTimeWithPreparation.ToString("dd-MM-yyyy HH:mm");
        string formattedEndTime = context.EndDateTime.ToString("dd-MM-yyyy HH:mm");

        IEnumerable<Client> teamMembers = context.ClientsByBoatId[boatId];
        string teamMembersHtml = BuildTeamMembersHtml(teamMembers, currentClient.Id);

        string personalizedBody = FormatEmailBody(rawBody, currentClient.FullName, teamName, boatName, formattedStartTime, formattedEndTime, teamMembersHtml, competitorsHtml, context.CompetitionName);
        string subject = $"Wedstrijd Inschrijving: {context.CompetitionName}";

        return (subject, personalizedBody);
    }

    private static string GetBoatName(int boatId, IReadOnlyCollection<Boat> boats)
    {
        Boat? boat = boats.FirstOrDefault((Boat b) => b.Id == boatId);
        return boat?.Name ?? "Onbekende Boot";
    }

    private static string GetTeamName(int boatId, IReadOnlyDictionary<int, string> teamNames)
    {
        return teamNames.TryGetValue(boatId, out string? name) ? name : "Naamloos Team";
    }

    private static string BuildCompetitorsHtml(int excludedBoatId, CompetitionEmailContext context)
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

    private static string BuildTeamMembersHtml(IEnumerable<Client> teamMembers, int currentClientId)
    {
        StringBuilder myTeamSb = new StringBuilder();
        foreach (Client member in teamMembers)
        {
            string suffix = (member.Id == currentClientId) ? " <strong>(jij)</strong>" : "";
            myTeamSb.Append($"<li>{member.FullName}{suffix}</li>");
        }
        return myTeamSb.ToString();
    }

    private static string FormatEmailBody(string template, string clientName, string teamName, string boatName, string startTime, string endTime, string teamMembersHtml, string competitorsHtml, string competitionName)
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