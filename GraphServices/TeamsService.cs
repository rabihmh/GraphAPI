using Microsoft.Graph.Models;

namespace NetwaysPoc.GraphServices;

public abstract class TeamsService
{
    public static OnlineMeeting CreateTeamsMeeting(
        string meeting,
        DateTimeOffset begin,
        DateTimeOffset end)
    {

        var onlineMeeting = new OnlineMeeting
        {
            StartDateTime = begin,
            EndDateTime = end,
            Subject = meeting,
            LobbyBypassSettings = new LobbyBypassSettings
            {
                Scope = LobbyBypassScope.Everyone
            }
        };

        return onlineMeeting;
    }

    public static OnlineMeeting AddMeetingParticipants(OnlineMeeting onlineMeeting, List<string> attendees)
    {
        var meetingAttendees = new List<MeetingParticipantInfo>();
        foreach (var attendee in attendees)
        {
            if (!string.IsNullOrEmpty(attendee))
            {
                meetingAttendees.Add(new MeetingParticipantInfo
                {
                    Upn = attendee.Trim()
                });
            }
        }

        onlineMeeting.Participants ??= new MeetingParticipants();;
        onlineMeeting.Participants.Attendees = meetingAttendees;

        return onlineMeeting;
    }
}
