using System.Collections.Generic;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace FlashlightToolLoader.Patches
{
    //patching StartOfRound instead of PlayerControllerB awake like originally so that LethalLib has time to load its items
    [HarmonyPatch(typeof(StartOfRound))]
    public class StartOfRoundPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("Start")]
        public static void AfterStartOfRound()
        {
            FlashlightToolLoader.Logger.LogDebug("StartOfRound.Start postfix: Initializing helmet lights for all players.");

            // Loop through all players and call InitiateHelmetLight
            foreach (PlayerControllerB player in StartOfRound.Instance.allPlayerScripts)
            {
                HelmetLightPatch.InitiateHelmetLights(player);
            }
            

            HelmetLightPatch.FixShipItems(); //fix flashlighs on ship to be the same as their prefabs
        }
    }

    
    public class HelmetLightPatch
    {
        // Dictionary to store the flashlight objects and their corresponding IDs
        public static Dictionary<GameObject, int> LightToID = new Dictionary<GameObject, int>();

        public static void InitiateHelmetLights(PlayerControllerB __instance)
        {
            FlashlightToolLoader.Logger.LogDebug("InitiateHelmetLight called on player: " + __instance);

            List<Item> flashlights= GetValidItems();

            //debug log print all items in the list
            foreach (Item light in flashlights)
            {
                FlashlightToolLoader.Logger.LogDebug("Flashlight to be made: " + light.name);
            }
            //get vanilla helmet light array size
            int vanillaHelmetLightCount = __instance.allHelmetLights.Length;
            FlashlightToolLoader.Logger.LogDebug("Vanilla Helmet Light Count: " + vanillaHelmetLightCount);
            // Create an array of light object the same size as the flashlights list
            Light[] newLightObjects = (Light[])(object)new Light[flashlights.Count];

            LightToID.Clear(); //clear the dictionary to avoid duplicates
            for (int i=0; i<flashlights.Count; i++)
            {
                newLightObjects[i] = CreateLight(flashlights[i]);
                flashlights[i].spawnPrefab.GetComponent<FlashlightItem>().flashlightTypeID = i+vanillaHelmetLightCount; //set the flashlight type ID to the index of the light in the array + the size of the vanilla helmet light array to avoid overlaps
                FlashlightToolLoader.Logger.LogDebug("Flashlight Type ID: " + flashlights[i].spawnPrefab.GetComponent<FlashlightItem>().flashlightTypeID);
                //makes the new light a child of the HelmetLights object
                try
                {
                    newLightObjects[i].transform.SetParent(__instance.helmetLight.transform.parent.transform, false);
                    newLightObjects[i].transform.localPosition = FlashlightToolLoader.vanillaLightPos;
                    newLightObjects[i].transform.localRotation = FlashlightToolLoader.vanillaLightRot;
                    FlashlightToolLoader.Logger.LogDebug("Set parent of light: " + newLightObjects[i].name + " to: " + __instance.helmetLight.transform.parent.name);
                }
                catch (System.Exception e)
                {
                    FlashlightToolLoader.Logger.LogError("Failed to set parent of light: " + e.Message);
                }

                //add the light to the dictionary
                LightToID[flashlights[i].spawnPrefab] = i + vanillaHelmetLightCount;
                FlashlightToolLoader.Logger.LogDebug("Added light: " + newLightObjects[i].name + " to dictionary with ID: " + flashlights[i].spawnPrefab.GetComponent<FlashlightItem>().flashlightTypeID);
            }

            //adds the new lights to the allHelmetLights list
            __instance.allHelmetLights=__instance.allHelmetLights.AddRangeToArray(newLightObjects);
            
        }

        public static List<Item> GetValidItems()
        {
            //list of flashlights to ignore
            List<string> IgnoreLights = new List<string>(FlashlightToolLoader.BoundConfig.Blacklist.Value
        .Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries));

            List<Item> flashlights = new List<Item>();
            foreach (Item item in StartOfRound.Instance.allItemsList.itemsList) //for all items in the game
            {
                FlashlightToolLoader.Logger.LogDebug("Item: " + item.name); //debug log all items
                if (IsFlashlight(item) && !IgnoreLights.Contains(item.name)) //if they are a flashlight and not in the ignore list
                {
                    flashlights.Add(item);
                }
            }
            return flashlights;
        }

        public static bool IsFlashlight(Item item)
        {
            // Check if the item has FlashlightItem component
            if (item.spawnPrefab.GetComponent<FlashlightItem>()!=null)
            {
                FlashlightToolLoader.Logger.LogDebug("Item is a flashlight: " + item);
                return true;
            }
            else
            {
                return false;
            }

        }

        public static Light CreateLight(Item item)
        {
            // Find the child named "Light" (case-insensitive)
            Transform lightTransform = null;
            foreach (Transform child in item.spawnPrefab.GetComponentsInChildren<Transform>(true))
            {
                if (string.Equals(child.name, "Light", System.StringComparison.OrdinalIgnoreCase))
                {
                    lightTransform = child;
                    break;
                }
            }

            if (lightTransform != null)
            {
                // Instantiate a new copy so each player gets their own light
                GameObject newLightObj = Object.Instantiate(lightTransform.gameObject);
                Light lightComponent = newLightObj.GetComponent<Light>();
                lightComponent.name = item.spawnPrefab.name + " Light"; // Set a unique name for the light object
                FlashlightToolLoader.Logger.LogDebug("Instantiated Light: "+newLightObj.name+" of item: " + item);
                return lightComponent;
            }
            else
            {
                FlashlightToolLoader.Logger.LogDebug("No Light found");
                return null;
            }
        }




        public static void FixShipItems() {
            List<GrabbableObject> shipItems = new List<GrabbableObject>(GameObject.FindObjectsOfType<GrabbableObject>());
            
            FlashlightToolLoader.Logger.LogDebug("GrabbableObjects found: " + shipItems.Count);

            foreach (GrabbableObject item in shipItems) //for all items in the ship
            {
                FlashlightToolLoader.Logger.LogDebug("Item in ship: " + item.name); //debug log all items

                // Check if the item has FlashlightItem component and if its prefab is in the dictionary, if so then the flashlight in the ship is given that ID
                if (item.GetComponent<FlashlightItem>() != null && LightToID.TryGetValue(item.itemProperties.spawnPrefab, out int newId))
                {
                    item.GetComponent<FlashlightItem>().flashlightTypeID = newId;
                    FlashlightToolLoader.Logger.LogDebug($"Updated flashlightTypeID for {item.name} to {newId}");
                }
            }
        }
    }

}
