
# FlashlightToolLoader
A mod designed to be an improved version of hu_luo_bo_ya's [Lightutility](https://thunderstore.io/c/lethal-company/p/hu_luo_bo_ya/Lightutility/) mod. This mod dynamically assists in the creation of custom flashlight items as opposed to the hardcoding solution done within that mod and vanilla, allowing you to pocket custom flashlights and keep their unique light shining from the player as opposed to it reverting to a vanilla one. ([Discord link](https://discord.com/channels/1168655651455639582/1245084720614604873))


<details>
<summary>For Players</summary>

This mod should be ready to use as is. If you encounter a custom flashlight item that this mod does not work well with, you may blacklist the item in the config. The name that goes there is the name you should see printed in the logs. While this mod is compatible with Lightutility, it is recommended to blacklist the flashlights that it creates as this could lead to unexpected errors.<br>

</details>

<details>
<summary>For Developers</summary>

If you wish to create a compatible flashlight, you shouldn't need to do much. You just have to follow the vanilla format some. Your flashlight needs the flashlight script and the primary light needs to be a child object named "Light". It is recommended that you leave the flashlight's FlashlightTypeID field one of the vanilla values (0-2) incase there is an issue dynamically changing it through this mod as invalid values break the inventory.
![Imgur](https://imgur.com/x1U5kSy.png)

</details>


## Credits

- The developers of this mod's dependencies as it literally could not exist without them.
- [Audio Knight](https://www.youtube.com/@knightofaudio) on YouTube for a handy starting tutorial.
- [Nomnomab's project patcher](https://github.com/nomnomab/lc-project-patcher) to access vanilla LC through Unity.
- Those that helped provide information on the modding Discord, mainly through the [Dev-general channel on Discord](https://discord.com/channels/1168655651455639582/1168656318345777313).
- Debugging tools like [Imperium](https://thunderstore.io/c/lethal-company/p/giosuel/Imperium/) and [UnityExplorer](https://thunderstore.io/c/lethal-company/p/LethalCompanyModding/Yukieji_UnityExplorer/).
- Thunderstore for hosting this mod as I wouldn't know how to distribute without it.
- hu_luo_bo_ya's [Lightutility](https://thunderstore.io/c/lethal-company/p/hu_luo_bo_ya/Lightutility/) as the primary inspiration for this project.