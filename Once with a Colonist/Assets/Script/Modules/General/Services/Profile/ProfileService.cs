using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using MemoryPack;
using UniRx;
using UnityEngine;
using TendedTarsier.Script.Modules.General.Profile;
using TendedTarsier.Script.Modules.Gameplay.Configs.Stats;
using TendedTarsier.Script.Modules.Gameplay.Field;
using TendedTarsier.Script.Modules.General.Profiles.Stats;
using TendedTarsier.Script.Utilities.MemoryPack.FormatterProviders;

namespace TendedTarsier.Script.Modules.General.Services.Profile
{
    [UsedImplicitly]
    public class ProfileService : ServiceBase
    {
        public static readonly string ProfilesDirectory = Path.Combine(Application.persistentDataPath, "Profiles");

        private readonly List<IProfile> _profiles;

        public ProfileService(List<IProfile> profiles)
        {
            _profiles = profiles;
        }

        protected override void Initialize()
        {
            base.Initialize();

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
                if (!Directory.Exists(ProfilesDirectory))
                {
                    Directory.CreateDirectory(ProfilesDirectory);
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

        private string GetSectionPath(string name)
        {
            var fileName = name + ".json";
            return Path.Combine(ProfilesDirectory, fileName);
        }

        public override void Dispose()
        {
            base.Dispose();
            SaveAll();
        }
    }
}