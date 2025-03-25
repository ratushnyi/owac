using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using MemoryPack;
using TendedTarsier.Script.Modules.General.Configs.Stats;
using UniRx;
using UnityEngine;
using TendedTarsier.Script.Modules.General.Profiles.Stats;
using TendedTarsier.Script.Utilities.MemoryPack.FormatterProviders;
using Zenject;

namespace TendedTarsier.Script.Modules.General.Services.Profile
{
    [UsedImplicitly]
    public class ProfileService : ServiceBase
    {
        private List<IProfile> _profiles;

        [Inject]
        private void Construct(List<IProfile> profiles)
        {
            _profiles = profiles;
        }

        public override void Initialize()
        {
            RegisterFormatters();
            LoadSections();
        }

        private void RegisterFormatters()
        {
            MemoryPackFormatterProvider.Register(new ReactivePropertyFormatter<DateTime?>());
            MemoryPackFormatterProvider.Register(new ReactivePropertyFormatter<DateTime>());
            MemoryPackFormatterProvider.Register(new ReactivePropertyFormatter<TimeSpan>());
            MemoryPackFormatterProvider.Register(new ReactivePropertyFormatter<List<int>>());
            MemoryPackFormatterProvider.Register(new ReactivePropertyFormatter<bool>());
            MemoryPackFormatterProvider.Register(new ReactivePropertyFormatter<string>());
            MemoryPackFormatterProvider.Register(new ReactivePropertyFormatter<int>());
            MemoryPackFormatterProvider.Register(new ReactivePropertyFormatter<float>());
            MemoryPackFormatterProvider.Register(new ReactivePropertyFormatter<StatProfileElement>());
            MemoryPackFormatterProvider.Register(new ReactiveCollectionFormatter<bool>());
            MemoryPackFormatterProvider.Register(new ReactiveCollectionFormatter<string>());
            MemoryPackFormatterProvider.Register(new ReactiveCollectionFormatter<int>());
            MemoryPackFormatterProvider.Register(new ReactiveCollectionFormatter<float>());
            MemoryPackFormatterProvider.Register(new ReactiveDictionaryFormatter<string, bool>());
            MemoryPackFormatterProvider.Register(new ReactiveDictionaryFormatter<string, string>());
            MemoryPackFormatterProvider.Register(new ReactiveDictionaryFormatter<string, int>());
            MemoryPackFormatterProvider.Register(new ReactiveDictionaryFormatter<string, float>());
            MemoryPackFormatterProvider.Register(new ReactiveDictionaryFormatter<string, ReactiveProperty<int>>());
            MemoryPackFormatterProvider.Register(new ReactiveDictionaryFormatter<StatType, ReactiveProperty<StatProfileElement>>());
        }

        private void LoadSections()
        {
            foreach (var profile in _profiles)
            {
                LoadSection(profile);
            }
        }

        private void LoadSection(IProfile profile)
        {
            var path = GetSectionPath(profile.Name);
            if (File.Exists(path))
            {
                try
                {
                    var bytesData = File.ReadAllBytes(path);
                    var referenceObject = MemoryPackSerializer.Deserialize(profile.GetType(), bytesData);

                    TypeExtensions.PopulateObject(profile, referenceObject);
                }
                catch
                {
                    CreateSection(profile);
                }
            }
            else
            {
                CreateSection(profile);
            }

            profile.OnSectionLoaded();
            profile.Init(this);
        }

        public void SaveAll()
        {
            foreach (var profile in _profiles)
            {
                Save(profile);
            }
        }

        public void Save(IProfile profile)
        {
            try
            {
                if (!Directory.Exists(GeneralConstants.ProfilesPath))
                {
                    Directory.CreateDirectory(GeneralConstants.ProfilesPath);
                }

                var file = GetSectionPath(profile.Name);
                var byteData = MemoryPackSerializer.Serialize(profile.GetType(), profile);
                if (File.Exists(file))
                {
                    File.Delete(file);
                }

                File.WriteAllBytes(file, byteData);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed save section {profile.Name}. Details ↓\n{e.Message} ");
            }
        }

        private void CreateSection(IProfile profile)
        {
            profile.OnSectionCreated();
            Save(profile);
        }

        private string GetSectionPath(string sectionName)
        {
            var fileName = sectionName + ".json";
            return Path.Combine(GeneralConstants.ProfilesPath, fileName);
        }

        public override void Dispose()
        {
            SaveAll();
        }
    }
}