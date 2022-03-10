using System;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;


namespace DLCEnabler
{
    [BepInPlugin(pluginGUID, pluginName, pluginVersion)]
    public class DLCEnabler : BaseUnityPlugin
    {
        const string pluginGUID = "Valheim.DLC";
        const string pluginName = "Valheim_DLC_Enabler";
        public const string pluginVersion = "0.0.1";
        public static ConfigEntry<bool> modEnabled;
        Harmony harmony = new Harmony(pluginGUID);

        void Awake()
        {
            DLCEnabler.modEnabled = base.Config.Bind<bool>("General", "Enabled", true, "Unlock DLC Items");
            if (DLCEnabler.modEnabled.Value)
            {
                 harmony.PatchAll();
            }
        }
        void OnDestroy()
        {
            harmony.UnpatchSelf();

        }
    }

    [HarmonyPatch(typeof(DLCMan))]
    [HarmonyPatch(nameof(DLCMan.IsDLCInstalled))]
    [HarmonyPatch(new Type[] { typeof(DLCMan.DLCInfo) })]
    public class IsDLCInstalled_Patch
    {
        public static bool Prefix(ref DLCMan __instance, ref bool __result)
        {
            if (__instance == null)
            {
                return false;
            }
            __result = true; /* SERIOUSLY ? you just locked DLC items behind a boolean ? */
            return false;
        }
    }

    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch(nameof(Player.Awake))]
    public class Player_Awake_Patch
    {
        public static bool Prefix(ref Player __instance)
        {
            foreach (Recipe recipe in ObjectDB.instance.m_recipes)
            {
                if (recipe.m_item != null && !recipe.m_enabled)
                {
                    recipe.m_enabled = true;
                }
            }
            return true;
        }
    }

}
