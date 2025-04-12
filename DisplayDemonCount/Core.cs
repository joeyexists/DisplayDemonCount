using MelonLoader;
using HarmonyLib;
using System.Reflection;

[assembly: MelonInfo(typeof(DisplayDemonCount.Core), "DisplayDemonCount", "1.0.0", "joeyexists", null)]
[assembly: MelonGame("Little Flag Software, LLC", "Neon White")]

namespace DisplayDemonCount
{
    public class Core : MelonMod
    {

        public override void OnLateInitializeMelon()
        {
            var harmony = new HarmonyLib.Harmony("com.joeyexists.displaydemoncount");
            harmony.PatchAll();

            Settings.Register();
        }

        public static class Settings
        {
            public static MelonPreferences_Entry<bool> AlwaysDisplayDemonCount;

            public static void Register()
            {
                var category = MelonPreferences.CreateCategory("DisplayDemonCount");

                AlwaysDisplayDemonCount = category.CreateEntry("Always Display Demon Count", true,
                    description: "Displays the demon count on every level, including Chapter 11 and sidequest levels.");
            }
        }

        [HarmonyPatch(typeof(PlayerUI), "UpdateDemonCounter")]
        public static class AlwaysDisplayDemonCountPatch
        {
            static FieldInfo _lastDemonCount = typeof(PlayerUI).GetField("_lastDemonCount", BindingFlags.NonPublic | BindingFlags.Instance);

            static bool Prefix(PlayerUI __instance, int count)
            {
                if (!Settings.AlwaysDisplayDemonCount.Value)
                    return true;

                // Always show demon counter
                __instance.demonCounterHolder.SetActive(count > 0);

                // Update the demon counter as normal
                if (__instance.demonCounterHolder.activeInHierarchy)
                {
                    int lastCount = (int)_lastDemonCount.GetValue(__instance);

                    if (lastCount != count)
                    {
                        __instance.demonCounterSpring.CurrentPos += __instance.demonCounterSpringForce;
                    }

                    __instance.demonCounterNumberText.text = count.ToString();
                }

                _lastDemonCount.SetValue(__instance, count);
                return false;
            }
        }
    }
}