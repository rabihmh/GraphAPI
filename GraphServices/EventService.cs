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
                TimeZone =TimeZoneInfo.Local.Id.ToString()
            },
            End = new DateTimeTimeZone
            {
                DateTime = end.ToString("yyyy-MM-ddTHH:mm:ss"),
                TimeZone = TimeZoneInfo.Local.Id.ToString()
            },
            Attendees = participants.Select(participant => new Attendee
            {
                EmailAddress = new EmailAddress
                {
                    Address = participant
                },
                Type = AttendeeType.Required
            }).ToList(),
            IsOnlineMeeting = true,
            Body = new ItemBody
            {
                ContentType = BodyType.Html,
                Content = @"
                            <h1>Meeting details</h1>
                            <p>Test meeting</p>
                            <ul>
                                <li>Test 1</li>
                                <li>Test 2</li>
                            </ul>
                            "
            }
        };

        return newEvent;
    }
}
