using System;
using MemoryPack;
using TendedTarsier.Script.Modules.General.Services;
using UniRx;

namespace TendedTarsier
{
    public abstract class ProfileBase
    {
        protected readonly CompositeDisposable CompositeDisposable = new();

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

        public void Clear()
        {
            var newInstance = Activator.CreateInstance(GetType());
            TypeExtensions.PopulateObject(this, newInstance);
            _profileService.Save(this);
        }

        public void Terminate()
        {
            CompositeDisposable.Dispose();
            Dispose();
        }

        protected virtual void Dispose() { }
    }
}