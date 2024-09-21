using TendedTarsier.Script.Modules.General.Services.Profile;

namespace TendedTarsier.Script.Modules.General.Profile
{
    public interface IProfile
    {
        string Name { get; }
        void Init(ProfileService profileService);
        void OnSectionLoaded();
        void OnSectionCreated();
    }
}