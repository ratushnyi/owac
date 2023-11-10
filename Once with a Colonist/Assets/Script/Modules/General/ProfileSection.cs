using MemoryPack;

namespace TendedTarsier
{
    public abstract class ProfileSection
    {
        private ProfileService _profileService;

        [MemoryPackIgnore]
        public abstract string Name { get; }

        public void Init(ProfileService profileService)
        {
            _profileService = profileService;
        }

        public virtual void OnSectionCreated()
        {

        }

        public virtual void OnSectionLoaded()
        {

        }

        public void Save()
        {
            _profileService.Save(this);
        }
    }
}