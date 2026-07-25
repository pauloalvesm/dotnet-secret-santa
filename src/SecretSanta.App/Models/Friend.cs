namespace SecretSanta.App.Models;

public class Friend
{
    private string name;
    private string email;

    public string Name { get => name; set => name = value; }
    public string Email { get => email; set => email = value; }

    public Friend(string name, string email)
    {
        Name = name;
        Email = email;
    }

    public override string ToString()
    {
        return $"Name: {name}, Email {email}";
    }

    public override bool Equals(object obj)
    {
        if (obj is null || ReferenceEquals(this, obj))
        {
            return false;
        }

        if (obj is Friend friend)
        {
            return Email.Equals(friend.Email, StringComparison.OrdinalIgnoreCase);
        }

        return false;
    }

    public override int GetHashCode()
    {
        return Email.GetHashCode(StringComparison.OrdinalIgnoreCase);
    }
}
