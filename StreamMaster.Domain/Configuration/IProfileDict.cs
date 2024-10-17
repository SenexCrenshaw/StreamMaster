namespace StreamMaster.Domain.Configuration;

public interface IProfileDict<TProfile>
{
    bool IsReadOnly(string ProfileName);

    Dictionary<string, TProfile> Profiles { get; }

    void AddProfile(string ProfileName, TProfile Profile);

    void AddProfiles(Dictionary<string, dynamic> profiles);

    void RemoveProfile(string ProfileName);
}