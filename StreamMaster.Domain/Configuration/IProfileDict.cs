namespace StreamMaster.Domain.Configuration;

public interface IProfileDict<TProfile>
{
    Dictionary<string, TProfile> Profiles { get; }
    void AddProfile(string ProfileName, TProfile Profile);
    void AddProfiles(Dictionary<string, dynamic> profiles);
    void RemoveProfile(string ProfileName);
}
