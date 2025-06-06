public class Consultation
{
    public string Teacher { get; set; }
    public string Room { get; set; }
    public string Time { get; set; }
    public string Day { get; set; }
    public bool IsApproved { get; set; } = false; // Новое свойство
    public List<string> SignedUpStudents { get; set; } = new List<string>();
    public List<string> RequestedStudents { get; set; } = new List<string>();

    public override string ToString()
    {
        var signed = string.Join(",", SignedUpStudents);
        var requested = string.Join(",", RequestedStudents);
        return $"{Teacher}|{Room}|{Time}|{Day}|{signed}|{requested}|{IsApproved}";
    }

    public static Consultation FromString(string line)
    {
        var parts = line.Split('|');
        return new Consultation
        {
            Teacher = parts[0],
            Room = parts[1],
            Time = parts[2],
            Day = parts[3],
            SignedUpStudents = parts.Length > 4 && !string.IsNullOrEmpty(parts[4])
                ? parts[4].Split(',').ToList()
                : new List<string>(),
            RequestedStudents = parts.Length > 5 && !string.IsNullOrEmpty(parts[5])
                ? parts[5].Split(',').ToList()
                : new List<string>(),
            IsApproved = parts.Length > 6 && bool.TryParse(parts[6], out var approved)
                ? approved
                : false
        };
    }
}