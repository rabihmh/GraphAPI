using Microsoft.Graph.Models;

namespace NetwaysPoc.GraphServices;

public class EmailService
{
    private readonly List<Attachment> _attachments = new();

    public Message CreateStandardEmail(string recipient, string header, string body)
    {
        var message = new Message
        {
            Subject = header,
            Body = new ItemBody
            {
                ContentType = BodyType.Text,
                Content = body
            },
            ToRecipients = new List<Recipient>()
            {
                new Recipient
                {
                    EmailAddress = new EmailAddress
                    {
                        Address = recipient
                    }
                }
            },
            Attachments = _attachments
        };

        return message;
    }

    public Message CreateHtmlEmail(string recipient, string header, string body)
    {
        var message = new Message
        {
            Subject = header,
            Body = new ItemBody
            {
                ContentType = BodyType.Html,
                Content = body
            },
            ToRecipients = new List<Recipient>()
            {
                new Recipient
                {
                    EmailAddress = new EmailAddress
                    {
                        Address = recipient
                    }
                }
            },
            Attachments = _attachments
        };

        return message;
    }

    public void AddAttachment(byte[] rawData, string filePath)
    {
        _attachments.Add(new FileAttachment
        {
            Name = Path.GetFileName(filePath),
            ContentBytes = EncodeToBase64Bytes(rawData)
        });
    }

    public void ClearAttachments()
    {
        _attachments.Clear();
    }

    private static byte[] EncodeToBase64Bytes(byte[] rawData)
    {
        var base64String = Convert.ToBase64String(rawData);
        var returnValue = Convert.FromBase64String(base64String);
        return returnValue;
    }
}
