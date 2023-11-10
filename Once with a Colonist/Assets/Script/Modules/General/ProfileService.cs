using System;
using System.Collections.Generic;
using System.IO;
using MemoryPack;
using UnityEngine;
using Zenject;

namespace TendedTarsier
{
    public class ProfileService
    {
        private readonly string _profilesDirectory = "Settings/Profiles";

        [Inject]
        private void Construct(List<ProfileSection> profiles)
        {
            Debug.LogError(nameof(ProfileService));
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

        private void LoadSections(List<ProfileSection> profiles)
        {
            foreach (var profile in profiles)
            {
                LoadSection(profile);
            }
        }

        private void LoadSection(ProfileSection section)
        {
            var path = GetSectionPath(section.Name);
            if (File.Exists(path))
            {
                try
                {
                    var bytesData = File.ReadAllBytes(path);
                    MemoryPackExtensions.PopulateObject(section, bytesData);
                }
                catch
                {
                    CreateSection(section);
                }
            }
            else
            {
                CreateSection(section);
            }

            section.OnSectionLoaded();
            section.Init(this);
        }

        public void Save(ProfileSection section)
        {
            try
            {
                if (!Directory.Exists(_profilesDirectory))
                {
                    Directory.CreateDirectory(_profilesDirectory);
                }

                var file = GetSectionPath(section.Name);
                var byteData = MemoryPackSerializer.Serialize(section.GetType(), section);
                if (File.Exists(file))
                {
                    File.Delete(file);
                }

                File.WriteAllBytes(file, byteData);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed save section {section.Name}. Details ↓\n{e.Message} ");
            }
        }

        private void CreateSection(ProfileSection section)
        {
            section.OnSectionCreated();
            Save(section);
        }

        private string GetSectionPath(string name)
        {
            var fileName = name + ".json";
            return Path.Combine(_profilesDirectory, fileName);
        }
    }
}