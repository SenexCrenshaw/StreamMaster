namespace StreamMaster.SchedulesDirectAPI.Data;

public partial class SchedulesDirectData
{
    private readonly Dictionary<string, MxfPerson> _people =[];
    public MxfPerson FindOrCreatePerson(string name)
    {
        if (_people.TryGetValue(name, out var person)) return person;
        People.Add(person = new MxfPerson(People.Count + 1, name));
        _people.Add(name, person);
        return person;
    }
}
