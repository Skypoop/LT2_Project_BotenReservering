using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.App.Helpers;

public static class EmailTemplateHelper
{
    private static readonly Regex _placeholderPattern = new Regex(@"\{(\w+)\}", RegexOptions.Compiled);
    private static readonly ConditionalWeakTable<CompetitionEmailContext, ContextCacheData> _contextCache = new();
    private static string? _cachedEmailTemplate;

    private class ContextCacheData
    {
        public Dictionary<int, string> CompetitorHtmls { get; set; } = new();
        public Dictionary<int, string> BoatNames { get; set; } = new();
        public string FormattedStartTime { get; set; } = string.Empty;
        public string FormattedEndTime { get; set; } = string.Empty;
    }

    public static Task<(string Subject, string Body)> RenderCompetitionConfirmationAsync(CompetitionEmailContext context, Client currentClient, int boatId)
    {
        if (_cachedEmailTemplate == null)
        {
            return LoadTemplateAndRenderAsync(context, currentClient, boatId);
        }

        return Task.FromResult(RenderSynchronous(context, currentClient, boatId));
    }

    private static async Task<(string Subject, string Body)> LoadTemplateAndRenderAsync(CompetitionEmailContext context, Client currentClient, int boatId)
    {
        _cachedEmailTemplate = await ResourceLoaderHelper.LoadEmbeddedResourceAsync("CompetitionConfirmation.html");

        if (string.IsNullOrEmpty(_cachedEmailTemplate))
        {
            return (string.Empty, string.Empty);
        }

        return RenderSynchronous(context, currentClient, boatId);
    }

    private static (string Subject, string Body) RenderSynchronous(CompetitionEmailContext context, Client currentClient, int boatId)
    {
        ContextCacheData cacheData = _contextCache.GetValue(context, GenerateCacheForContext);

        string boatName = cacheData.BoatNames.TryGetValue(boatId, out string? bName) ? bName : "Onbekende Boot";
        string competitorsListHtml = cacheData.CompetitorHtmls.TryGetValue(boatId, out string? cHtml) ? cHtml : "<p>Geen tegenstanders.</p>";
        string teamName = GetTeamName(boatId, context.TeamNameByBoatId);

        IEnumerable<Client> teamMembers = context.ClientsByBoatId[boatId];
        string teamMembersListHtml = BuildTeamMembersHtml(teamMembers, currentClient.Id);

        Dictionary<string, string> templateValues = new Dictionary<string, string>
        {
            { "Name", currentClient.FullName },
            { "CompetitionName", context.CompetitionName },
            { "TeamName", teamName },
            { "BoatName", boatName },
            { "StartTime", cacheData.FormattedStartTime },
            { "EndTime", cacheData.FormattedEndTime },
            { "TeamMembers", teamMembersListHtml },
            { "Competitors", competitorsListHtml }
        };

        string personalizedBody = ReplacePlaceholders(_cachedEmailTemplate!, templateValues);
        string subject = $"Wedstrijd Inschrijving: {context.CompetitionName}";

        return (subject, personalizedBody);
    }

    private static ContextCacheData GenerateCacheForContext(CompetitionEmailContext context)
    {
        ContextCacheData data = new ContextCacheData
        {
            FormattedStartTime = context.StartTimeWithPreparation.ToString("dd-MM-yyyy HH:mm"),
            FormattedEndTime = context.EndDateTime.ToString("dd-MM-yyyy HH:mm")
        };

        foreach (Boat boat in context.CompetitionBoats)
        {
            data.BoatNames[boat.Id] = boat.Name;
        }

        Dictionary<int, string> boatHtmlFragments = new Dictionary<int, string>();
        foreach (KeyValuePair<int, ObservableCollection<Client>> entry in context.ClientsByBoatId)
        {
            boatHtmlFragments[entry.Key] = BuildSingleTeamFragment(entry.Key, entry.Value, context);
        }

        foreach (int boatId in context.ClientsByBoatId.Keys)
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<int, string> fragment in boatHtmlFragments)
            {
                if (fragment.Key == boatId) continue;
                sb.Append(fragment.Value);
            }
            string combined = sb.ToString();
            data.CompetitorHtmls[boatId] = string.IsNullOrEmpty(combined) ? "<p>Geen tegenstanders.</p>" : combined;
        }

        return data;
    }

    private static string BuildSingleTeamFragment(int boatId, ObservableCollection<Client> members, CompetitionEmailContext context)
    {
        string teamName = GetTeamName(boatId, context.TeamNameByBoatId);
        IEnumerable<string> memberNames = members.Select(c => c.FullName);
        string memberString = string.Join(", ", memberNames);

        return $"<p style='margin: 5px 0;'><strong>{teamName}</strong>: {memberString}</p>";
    }

    private static string ReplacePlaceholders(string template, Dictionary<string, string> values)
    {
        return _placeholderPattern.Replace(template, match =>
        {
            string key = match.Groups[1].Value;
            return values.TryGetValue(key, out string? value) ? value : match.Value;
        });
    }

    private static string GetTeamName(int boatId, IReadOnlyDictionary<int, string> teamNames)
    {
        return teamNames.TryGetValue(boatId, out string? name) ? name : "Naamloos Team";
    }

    private static string BuildTeamMembersHtml(IEnumerable<Client> teamMembers, int currentClientId)
    {
        StringBuilder teamMembersStringBuilder = new StringBuilder();
        foreach (Client member in teamMembers)
        {
            string suffix = (member.Id == currentClientId) ? " <strong>(jij)</strong>" : "";
            teamMembersStringBuilder.Append($"<li>{member.FullName}{suffix}</li>");
        }
        return teamMembersStringBuilder.ToString();
    }
}