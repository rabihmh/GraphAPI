using Microsoft.Graph.Models;

namespace NetwaysPoc.GraphServices;

public class EventService
{
    public Event CreateEvent(string subject, DateTimeOffset start, DateTimeOffset end, List<string> participants)
    {
        var newEvent = new Event
        {
            Subject = subject,
            Start = new DateTimeTimeZone
            {
                DateTime = start.ToString("yyyy-MM-ddTHH:mm:ss"),
                TimeZone = "UTC"
            },
            End = new DateTimeTimeZone
            {
                DateTime = end.ToString("yyyy-MM-ddTHH:mm:ss"),
                TimeZone = "UTC"
            },
            Attendees = participants.Select(participant => new Attendee
            {
                EmailAddress = new EmailAddress
                {
                    Address = participant
                },
                Type = AttendeeType.Required
            }).ToList(),
            IsOnlineMeeting = true
        };

        return newEvent;
    }
}