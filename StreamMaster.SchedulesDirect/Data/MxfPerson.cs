namespace StreamMaster.SchedulesDirect.Data;

public partial class SchedulesDirectData
{
    private Dictionary<string, MxfPerson> _people = [];
    public MxfPerson FindOrCreatePerson(string name)
    {
        if (_people.TryGetValue(name, out MxfPerson? person))
        {
            return person;
        }

        person = new MxfPerson(People.Count + 1, name);


        People.Add(person);
        _people.Add(name, person);
        return person;
    }
}
