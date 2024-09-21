#if UNITY_EDITOR
using System.IO;
using TendedTarsier.Script.Modules.General.Services;
using UnityEditor;

namespace TendedTarsier.Script.Utilities
{
    public static class MenuUtilities
    {
        private const string RootMenu = "TendedTarsier/";
        private const string ProfileMenu = RootMenu + "User Profile/";

        [MenuItem(ProfileMenu + "Clean Profiles", false, 1)]
        private static void CleanProfiles()
        {
            if (Directory.Exists(ProfileService.ProfilesDirectory))
            {
                Directory.Delete(ProfileService.ProfilesDirectory, true);
            }
        }
    }
}
#endif