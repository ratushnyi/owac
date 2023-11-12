using System;
using System.Collections.Generic;
using System.IO;
using MemoryPack;
using UnityEngine;

namespace TendedTarsier
{
    public class ProfileService
    {
        public static readonly string ProfilesDirectory = "Settings/Profiles";

        public ProfileService(List<ProfileBase> profiles)
        {
            RegisterFormatters();
            LoadSections(profiles);
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
            MemoryPackFormatterProvider.Register(new ReactiveCollectionFormatter<bool>());
            MemoryPackFormatterProvider.Register(new ReactiveCollectionFormatter<string>());
            MemoryPackFormatterProvider.Register(new ReactiveCollectionFormatter<int>());
            MemoryPackFormatterProvider.Register(new ReactiveCollectionFormatter<float>());
            MemoryPackFormatterProvider.Register(new ReactiveDictionaryFormatter<string, bool>());
            MemoryPackFormatterProvider.Register(new ReactiveDictionaryFormatter<string, string>());
            MemoryPackFormatterProvider.Register(new ReactiveDictionaryFormatter<string, int>());
            MemoryPackFormatterProvider.Register(new ReactiveDictionaryFormatter<string, float>());
        }

        private void LoadSections(List<ProfileBase> profiles)
        {
            foreach (var profile in profiles)
            {
                LoadSection(profile);
            }
        }

        private void LoadSection(ProfileBase profile)
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

        public void Save(ProfileBase profile)
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

        private void CreateSection(ProfileBase profile)
        {
            profile.OnSectionCreated();
            Save(profile);
        }

        private string GetSectionPath(string name)
        {
            var fileName = name + ".json";
            return Path.Combine(ProfilesDirectory, fileName);
        }
    }
}