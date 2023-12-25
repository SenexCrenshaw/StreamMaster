namespace StreamMaster.SchedulesDirectAPI.Data;

public partial class SchedulesDirectData
{
    private Dictionary<string, MxfPerson> _people = [];
    public MxfPerson FindOrCreatePerson(string name, int? epgId = null)
    {
        if (_people.TryGetValue(name, out MxfPerson? person))
        {
            return person;
        }

        person = new MxfPerson(People.Count + 1, name);

        if (epgId != null)
        {
            person.extras.Add("epgid", epgId);
        }

        People.Add(person);
        _people.Add(name, person);
        return person;
    }
}
