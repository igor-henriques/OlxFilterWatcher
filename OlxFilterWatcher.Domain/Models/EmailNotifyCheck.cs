namespace OlxFilterWatcher.Domain.DTOs;

public record EmailNotifyCheck
{
    public EmailNotifyCheck(string email, bool isNotified)
    {
        Email = email;
        IsNotified = isNotified;
    }

    public string Email { get; init; }
    public bool IsNotified { get; set; }
}
