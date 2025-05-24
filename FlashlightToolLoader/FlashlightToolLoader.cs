using BepInEx;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;
using BepInEx.Configuration;

namespace FlashlightToolLoader
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class FlashlightToolLoader : BaseUnityPlugin
    {
        public static FlashlightToolLoader Instance { get; private set; } = null!;
        internal new static ManualLogSource Logger { get; private set; } = null!;
        internal static Harmony? Harmony { get; set; }

        internal static FlashlightToolLoaderConfig BoundConfig { get; private set; } = null!;

        // The position and rotation of the vanilla light
        public static Vector3 vanillaLightPos = new Vector3(0.206999943f, -0.526000381f, 0.475000262f);
        public static Quaternion vanillaLightRot = new Quaternion(0f, -0.0208650213f, 0, 0.999782383f);

        private void Awake()
        {
            Logger = base.Logger;
            Instance = this;

            BoundConfig = new FlashlightToolLoaderConfig(Config);

            Patch();

            Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
        }

        internal static void Patch()
        {
            Harmony ??= new Harmony(MyPluginInfo.PLUGIN_GUID);

            Logger.LogDebug("Patching...");

            Harmony.PatchAll();

            Logger.LogDebug("Finished patching!");
        }

        internal static void Unpatch()
        {
            Logger.LogDebug("Unpatching...");

            Harmony?.UnpatchSelf();

            Logger.LogDebug("Finished unpatching!");
        }

    }

    class FlashlightToolLoaderConfig
    {
        public readonly ConfigEntry<string> Blacklist;

        public FlashlightToolLoaderConfig(ConfigFile config)
        {
            Blacklist = config.Bind("FlashlightToolLoader", "Blacklist", "Flashlight,ProFlashlight,FlashLaserPointer", "List of flashlights to ignore (Do not remove vanilla lights). Separated by commas, no spaces. Example: ItemName1,ItemName2,ItemName3");
        }
    }
}