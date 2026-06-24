using TheSalemTrials;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem;
using Il2CppSystem.Runtime.Remoting.Messaging;
using MelonLoader;
using MelonLoader.Utils;
using System;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;
using UnityEngine;
using UnityEngine.Playables;
using static Il2Cpp.GameplayEvents;
using static Il2CppSystem.Array;
using static MelonLoader.Modules.MelonModule;

[assembly: MelonInfo(typeof(MainMod), "Windyways_TheSalemTrials", "1.1.0", "Windyways")]
[assembly: MelonGame("UmiArt", "Demon Bluff")]

namespace TheSalemTrials;
public class MainMod : MelonMod
{
    public override void OnInitializeMelon()
    {
        // Villagers
        ClassInjector.RegisterTypeInIl2Cpp<Sheriff>();
        ClassInjector.RegisterTypeInIl2Cpp<Admirer>();
        ClassInjector.RegisterTypeInIl2Cpp<Detective>();
        ClassInjector.RegisterTypeInIl2Cpp<Seer>();

        // Minions
        ClassInjector.RegisterTypeInIl2Cpp<Illusionist>();
        ClassInjector.RegisterTypeInIl2Cpp<Shroud>();
    }


    public MelonPreferences_Category configCategory = null!;
    public override void OnLateInitializeMelon()
    {
        GameObject content = GameObject.Find("Game/Gameplay/Content");
        NightPhase nightPhase = content.GetComponent<NightPhase>();
        Statics.GetStartingRoles();

        // Scrapped Characters BECAUSE I SUCK AT CODING

        // --- OUTCAST ---
        // Cursed Soul - I could be Good or Evil. I cannot be Disguised as.
        // Berserker - Pick 1 character. If not-Outcast, Execute it.

        // --- MINION ---
        // Pestilence (v3) - Corrupt a random Good Villager. I Lie and Disguise.
        // Conjurer - Game Start: Execute a random Good Villager. I Lie and Disguise.

        // --- DEMON ---
        // Serial Killer - At Night, i kill an unrevealed Good Villager if possible. Nighttime comes 2x faster. I do not Lie. I do not Disguise.
        // Cultist - Game Start: All Outcasts are replaced with 'Minion'. I Lie and Disguise.
        // Pestilence (v2) - Reveal: Deal 1 damage to you each time a card is revealed. If Executed, Corrupt remaining unrevealed Good Villagers. I do not lie and I do not Disguise.

        // --- VILLAGERS ---
        CharacterData sheriff = newCharacter("Sheriff", EAlignment.Good, ECharacterType.Villager, true, false, "\"The gun shows 'em who's boss. Don't lie to me.\"", "Bishop_58855542");
        sheriff.role = new Sheriff();
        sheriff.description = "Learn how many liars are adjacent to me.";
        sheriff.ifLies = $"I say '0 Liars' if there are adjacent Evils.\nI say '2 Liars' if there are no adjacent Evils.";
        sheriff.gender = EGender.Male;

        CharacterData admirer = newCharacter("Admirer", EAlignment.Good, ECharacterType.Villager, true, false, "\"This may sound creepy, but i watched her through the window.\"", "Bishop_58855542");
        admirer.role = new Admirer();
        admirer.description = "Pick 1 character:\nLearn if they are Corrupted.";
        admirer.ifLies = $"I say ‘Not Corrupted’ if the target is Lying.\nI say ‘Corrupted’ if the target is not Corrupted.";
        admirer.picking = true;
        admirer.abilityUsage = EAbilityUsage.Once;
        admirer.gender = EGender.Male;

        CharacterData detective = newCharacter("Detective", EAlignment.Good, ECharacterType.Villager, true, false, "\"The Investigator wonders why i’m better than him. I told him ‘because i just am’.\"", "Bishop_58855542");
        detective.role = new Detective();
        detective.description = "Learn how close an evil is to a random Good Villager.";
        detective.ifLies = $"I say 1 more or 1 less than the real distance.";
        detective.hints = "I cannot reference myself in my information.";
        detective.gender = EGender.Male;

        CharacterData seer = newCharacter("Seer", EAlignment.Good, ECharacterType.Villager, true, false, "\"I can look into who has the truest hearts. The Lover isn’t one of them.\"", "Bishop_58855542");
        seer.role = new Seer();
        seer.description = "Pick 2 characters:\nLearn 1 thing they have in common.";
        seer.ifLies = $"I say random false information."; // (duh)
        seer.hints = "I prioritize:\nCorrupted > Disguised > Evil > Good.";
        seer.picking = true;
        seer.abilityUsage = EAbilityUsage.Once;
        seer.gender = EGender.Male;

        // --- OUTCASTS ---

        // --- MINIONS ---
        CharacterData illusionist = newCharacter("Illusionist", EAlignment.Evil, ECharacterType.Minion, true, true, "\"I am the best at casting magic huh?\"", "Imp_58992273");
        illusionist.role = new Illusionist();
        illusionist.description = "I Lie and Disguise as a not-in-play Villager.";
        illusionist.gender = EGender.Male;

        CharacterData shroud = newCharacter("Shroud", EAlignment.Evil, ECharacterType.Minion, true, true, "\"I’m like, literally a ghost. That’s bluffing. Yeah.\"", "Imp_58992273");
        shroud.role = new Shroud();
        shroud.description = "I Lie and Disguise as an in-play Outcast or Villager.";
        shroud.gender = EGender.Male;

        // --- DEMONS ---


        AscensionsData advancedAscension = ProjectContext.Instance.gameData.advancedAscension;

        MelonLogger.Msg($"Adding roles to scripts");
        Il2CppSystem.Collections.Generic.List<string> displayedScripts = new Il2CppSystem.Collections.Generic.List<string>();
        foreach (CustomScriptData scriptData in advancedAscension.possibleScriptsData)
        {
            ScriptInfo script = scriptData.scriptInfo;
            if (!displayedScripts.Contains(scriptData.name))
            {
                MelonLogger.Msg($"Found a script! Name: {scriptData.name}. Compositions:");
                displayedScripts.Add(scriptData.name);
                Il2CppSystem.Collections.Generic.List<string> characterCounts = new Il2CppSystem.Collections.Generic.List<string>();
                foreach (CharactersCount characterCount in script.characterCounts)
                {
                    string charCount = $"{characterCount.town}/{characterCount.outs}/{characterCount.minion}/{characterCount.demon}";
                    if (!characterCounts.Contains(charCount)) MelonLogger.Msg(charCount);
                    characterCounts.Add(charCount);
                }
            }

            addRole(script.startingTownsfolks, sheriff);
            addRole(script.startingTownsfolks, admirer);
            addRole(script.startingTownsfolks, detective);
            addRole(script.startingTownsfolks, seer);

            addRole(script.startingMinions, illusionist);
            addRole(script.startingMinions, shroud);
        }

        for (int j = 0; j < advancedAscension.possibleScriptsData.Length; j++)
        {
            Debug.LogWarning(advancedAscension.possibleScriptsData[j].name);
            MelonLogger.Msg($"Script: {advancedAscension.possibleScriptsData[j].name.ToString()}");
        }
    }

    // By the vanilla rule of one demon per village.
    public CharactersCount setCharacterCount(int Villagers, int Outcasts, int Minions, int Demons)
    {
        CharactersCount myCharacterCount = new CharactersCount(Villagers + Outcasts + Minions + Demons, Villagers, Demons, Outcasts, Minions);
        myCharacterCount.dOuts = Outcasts + 1;
        return myCharacterCount;
    }

    public Il2CppSystem.Collections.Generic.List<CharactersCount> addCharacterCount(CharactersCount characterCount, Il2CppSystem.Collections.Generic.List<CharactersCount> addList, int weight)
    {
        Il2CppSystem.Collections.Generic.List<CharactersCount> returnList = addList;
        for (int i = 0; i < weight; i++)
        {
            returnList.Add(characterCount);
        }
        return returnList;
    }

    public void w_addDemonRole(AscensionsData advancedAscension, CharacterData? data, string oldScriptName, string newScriptName, CustomScriptData w_NewScript, Il2CppSystem.Collections.Generic.List<CharacterData> jinxList, int configAmount)
    {
        if (data == null)
        {
            return;
        }
        if (configAmount == 0)
        {
            return;
        }
        foreach (CustomScriptData scriptData in advancedAscension.possibleScriptsData)
        {
            if (scriptData.name == oldScriptName)
            {
                CustomScriptData newScriptData = GameObject.Instantiate(scriptData);
                newScriptData.name = newScriptName;
                ScriptInfo newScript = new ScriptInfo();
                ScriptInfo script = w_NewScript.scriptInfo;
                newScriptData.scriptInfo = newScript;
                newScript.startingTownsfolks = script.startingTownsfolks;
                newScript.startingOutsiders = script.startingOutsiders;
                newScript.startingMinions = script.startingMinions;
                newScript.startingDemons = script.startingDemons;
                newScript.characterCounts = w_NewScript.scriptInfo.characterCounts;
                //newScript.startingDemons = new Il2CppSystem.Collections.Generic.List<CharacterData>();
                //newScript.startingDemons.Add(data);
                var newPSD = advancedAscension.possibleScriptsData.Append(newScriptData);
                for (int i = 0; i < configAmount; i++)
                {
                    newPSD = newPSD.Append(newScriptData);
                }
                advancedAscension.possibleScriptsData = newPSD.ToArray();
                return;
            }
        }
    }
    public void addCharacterDataToList(string ID, List<CharacterData> Characters)
    {
        foreach (CharacterData targetChar in Gameplay.Instance.GetAllAscensionCharacters())
        {
            if (targetChar.characterId == ID)
            {
                Characters.Append(targetChar);
            }
        }
    }
    public void replaceScriptChars(List<CharacterData> Characters, CustomScriptData w_TargetScript)
    {
        w_TargetScript.scriptInfo.startingTownsfolks.Clear();
        w_TargetScript.scriptInfo.startingOutsiders.Clear();
        w_TargetScript.scriptInfo.startingMinions.Clear();
        w_TargetScript.scriptInfo.startingDemons.Clear();
        foreach (CharacterData targetChar in Characters)
        {
            if (targetChar.type == ECharacterType.Villager)
            {
                w_TargetScript.scriptInfo.startingTownsfolks.Add(targetChar);
            }
            if (targetChar.type == ECharacterType.Outcast)
            {
                w_TargetScript.scriptInfo.startingOutsiders.Add(targetChar);
            }
            if (targetChar.type == ECharacterType.Minion)
            {
                w_TargetScript.scriptInfo.startingMinions.Add(targetChar);
            }
            if (targetChar.type == ECharacterType.Demon)
            {
                w_TargetScript.scriptInfo.startingDemons.Add(targetChar);
            }
        }
    }
    public void addRole(Il2CppSystem.Collections.Generic.List<CharacterData> list, CharacterData data)
    {
        if (list.Contains(data))
        {
            return;
        }
        list.Add(data);
    }
    public void addRoleEvenIfDupe(Il2CppSystem.Collections.Generic.List<CharacterData> list, CharacterData data)
    {
        list.Add(data);
    }
    public void addRoleIfNotJinxed(Il2CppSystem.Collections.Generic.List<CharacterData> list, CharacterData data, Il2CppSystem.Collections.Generic.List<CharacterData> jinxList, Il2CppSystem.Collections.Generic.List<CharacterData> jinxCheckList)
    {
        if (list.Contains(data))
        {
            return;
        }
        bool jinxed = false;
        foreach (CharacterData character in jinxList)
        {
            foreach (CharacterData character2 in jinxCheckList)
            {
                if (character2 == character)
                {
                    jinxed = true;
                }
            }
        }
        if (jinxed) return;
        list.Add(data);
    }
    public CharacterData[] allDatas = System.Array.Empty<CharacterData>();
    public override void OnUpdate()
    {
        if (allDatas.Length == 0)
        {
            var loadedCharList = Resources.FindObjectsOfTypeAll(Il2CppType.Of<CharacterData>());
            if (loadedCharList != null)
            {
                allDatas = new CharacterData[loadedCharList.Length];
                for (int i = 0; i < loadedCharList.Length; i++)
                {
                    allDatas[i] = loadedCharList[i]!.Cast<CharacterData>();
                }
            }
        }
        if (Statics.charactersArray.Length == 0)
        {
            var loadedCharList = Resources.FindObjectsOfTypeAll(Il2CppType.Of<CharacterData>());
            if (loadedCharList != null)
            {
                Statics.charactersArray = new CharacterData[loadedCharList.Length];
                for (int i = 0; i < loadedCharList.Length; i++)
                {
                    CharacterData data = loadedCharList[i]!.Cast<CharacterData>();
                    Statics.CheckAddRole(data);
                    Statics.charactersArray[i] = data;
                }
            }
            if (Statics.charactersArray.Length > 0)
            {
                this.OnFirstUpdate();
            }
        }
    }
    public CharacterData[] InsertAfterAct(string previous, CharacterData data)
    {
        MelonLogger.Msg($"Adding {data.name.ToString()} after {previous}");
        CharacterData[] actList = Characters.Instance.startGameActOrder;

        int actSize = actList.Length;
        CharacterData[] newActList = new CharacterData[actSize + 1];
        bool inserted = false;
        for (int i = 0; i < actSize; i++)
        {
            if (inserted)
            {
                newActList[i + 1] = actList[i];
            }
            else
            {
                if (actList[i] != null)
                {
                    newActList[i] = actList[i];
                    if (actList[i].name == previous)
                    {
                        newActList[i + 1] = data;
                        inserted = true;
                    }
                }
            }
        }
        if (!inserted)
        {
            LoggerInstance.Msg("");
        }
        return newActList;
    }
    public CharacterData[] InsertAtStartOfActOrder(CharacterData data)
    {
        MelonLogger.Msg($"Adding {data.name.ToString()} to start of act order");
        CharacterData[] actList = Characters.Instance.startGameActOrder;
        int actSize = actList.Length;
        CharacterData[] newActList = new CharacterData[actSize + 1];
        for (int i = 0; i < actSize; i++)
        {
            newActList[i + 1] = actList[i];
        }
        newActList[0] = data;
        return newActList;
    }
    public CharacterData[] InsertAtEndOfActOrder(CharacterData data)
    {
        MelonLogger.Msg($"Adding {data.name.ToString()} to end of act order");
        CharacterData[] actList = Characters.Instance.startGameActOrder;
        int actSize = actList.Length;
        CharacterData[] newActList = new CharacterData[actSize + 1];
        for (int i = 0; i < actSize; i++)
        {
            newActList[i] = actList[i];
        }
        newActList[actSize] = data;
        return newActList;
    }
    public CharacterData[] insertBeforeAct(string next, CharacterData data)
    {
        MelonLogger.Msg($"insertBeforeAct called adding {data.name.ToString()} before {next}");
        int actSize = Characters.Instance.startGameActOrder.Length;
        Il2CppSystem.Collections.Generic.List<CharacterData> newActList = new Il2CppSystem.Collections.Generic.List<CharacterData>();
        bool added = false;
        foreach (CharacterData character in Characters.Instance.startGameActOrder)
        {
            MelonLogger.Msg($"Attempting to add {character.name.ToString()} to act order");
            if (character.name.ToString() == next) MelonLogger.Msg($"Found target {character.name.ToString()}");
            if (character.name.ToString() == next && added == false)
            {
                MelonLogger.Msg($"Adding target {data.name.ToString()} to newActList");
                newActList.Add(data);
                MelonLogger.Msg($"Added {data.name.ToString()} to newActList");
            }
            MelonLogger.Msg($"Adding {character.name.ToString()} to newActList");
            newActList.Add(character);
        }
        CharacterData[] newActArray = new CharacterData[actSize + 1];
        int counter = 0;
        MelonLogger.Msg($"Beginning loop");
        foreach (CharacterData character in newActList)
        {
            Debug.Log(string.Format("Adding {0} to act order at array position {1}", character.name.ToString(), counter));
            newActArray[counter] = character;
            counter += 1;
        }
        return newActArray;
    }
    public static Il2CppSystem.Collections.Generic.List<CharacterData> JinxCharacter(Il2CppSystem.Collections.Generic.List<CharacterData> inputList, string ID)
    {
        Il2CppSystem.Collections.Generic.List<CharacterData> outputList = new Il2CppSystem.Collections.Generic.List<CharacterData>();
        foreach (CharacterData character in inputList)
        {
            if (character.characterId != ID)
            {
                outputList.Add(character);
            }
        }
        return outputList;
    }

    public void OnFirstUpdate()
    {
        PatchVanillaCharacterDescriptions();
    }

    public static class HiddenRoleStatus
    {
        public static ECharacterStatus hiddenRole = (ECharacterStatus)999;
    }

    public static CharacterData newCharacter(string name, EAlignment alignment, ECharacterType type, bool bluffable, bool usuallyDisguised, string flavour, string placeholderArtID)
    {
        Il2CppSystem.Collections.Generic.List<string> refIDs = new Il2CppSystem.Collections.Generic.List<string>();
        refIDs = GetRolePlaceholderArt(type, alignment);
        MelonLogger.Msg($"refIDs[0] = {refIDs[0]}");
        MelonLogger.Msg($"refIDs[1] = {refIDs[1]}");
        CharacterData backgroundRef = ProjectContext.Instance.gameData.GetCharacterDataOfId(refIDs[0]);
        CharacterData artRef = ProjectContext.Instance.gameData.GetCharacterDataOfId(placeholderArtID);
        if (artRef == null) artRef = ProjectContext.Instance.gameData.GetCharacterDataOfId(refIDs[1]);
        if (backgroundRef == null)
        {
            MelonLogger.Msg("backgroundRef is null! Resetting to Bishop...");
            backgroundRef = ProjectContext.Instance.gameData.GetCharacterDataOfId("Bishop_58855542");
        }
        if (artRef == null)
        {
            MelonLogger.Msg("artRef is null! Resetting to Bishop...");
            artRef = ProjectContext.Instance.gameData.GetCharacterDataOfId("Bishop_58855542");
        }
        MelonLogger.Msg($"backgroundRef = {backgroundRef.characterName}");
        MelonLogger.Msg($"artRef = {artRef.characterName}");
        



        CharacterData newCharacter = new CharacterData();
        //CharacterData bishopData = new CharacterData();
        //bishopData = ProjectContext.Instance.gameData.GetCharacterDataOfId("Bishop_58855542");
        //newCharacter.art = bishopData.art;
        //newCharacter.backgroundArt = bishopData.backgroundArt;
        //newCharacter.roguelikeInfo = bishopData.roguelikeInfo;

        MelonLogger.Msg("");
        MelonLogger.Msg($"Creating role {name} of type {type} and alignment {alignment}.");
        MelonLogger.Msg($"Name: {name}");
        newCharacter.name = name;
        newCharacter.characterName = name;
        MelonLogger.Msg($"Setting base desc...");
        newCharacter.description = "";
        MelonLogger.Msg($"Flavour: {flavour}");
        newCharacter.flavorText = flavour;
        newCharacter.hints = "";
        newCharacter.ifLies = "";
        newCharacter.picking = false;
        MelonLogger.Msg($"Alignment: {alignment.ToString()}");
        newCharacter.startingAlignment = alignment;
        MelonLogger.Msg($"Type: {type.ToString()}");
        newCharacter.type = type;
        MelonLogger.Msg($"Bluffable?: {bluffable.ToString()}");
        newCharacter.bluffable = bluffable;
        newCharacter.characterId = $"{name}_TST";
        newCharacter.artBgColor = getColour(type, alignment, "artBgColor");
        newCharacter.cardBgColor = getColour(type, alignment, "cardBgColor");
        newCharacter.cardBorderColor = getColour(type, alignment, "cardBorderColor");
        newCharacter.color = getColour(type, alignment, "color");
        MelonLogger.Msg($"Finished getting colours.");
        MelonLogger.Msg($"Usually Disguised?: {usuallyDisguised.ToString()}");
        newCharacter.usuallyDisguised = usuallyDisguised;
        newCharacter.additionalFlavorTexts = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStringArray(1);
        newCharacter.additionalFlavorTexts[0] = flavour;

        newCharacter.bundledCharacters = new Il2CppSystem.Collections.Generic.List<CharacterData>();
        newCharacter.additionalPossibleCharacters = new AddedCharacterTypes();

        newCharacter.art_cute = artRef.art_cute;
        newCharacter.backgroundArt = backgroundRef.backgroundArt;

        newCharacter.localization_key = $"TheSalemTrials_{name}";

        return newCharacter;
    }

    public static CharacterCount NewPossibleCharacterCount(ECharacterType type, int amount)
    {
        CharacterCount returnVal = new CharacterCount();
        returnVal.type = type;
        returnVal.count = amount;
        return returnVal;
    }

    public static Il2CppSystem.Collections.Generic.List<string> GetRolePlaceholderArt(ECharacterType type, EAlignment alignment) // First item of the list is the background, second is the art.
    {
        Il2CppSystem.Collections.Generic.List<string> returnList = new Il2CppSystem.Collections.Generic.List<string>();
        if (alignment == EAlignment.Good)
        {
            returnList.Add("Bishop_58855542");
        }
        else
        {
            returnList.Add("Minion_71804875");
        }
        if (type == ECharacterType.Villager)
        {
            if (alignment == EAlignment.Good)
            {
                returnList.Add("Knight_47970624"); // Good Villager: Knight
            }
            if (alignment == EAlignment.Evil)
            {
                returnList.Add("Gambler_42592744"); // Evil Villager: Slayer
            }
        }
        if (type == ECharacterType.Outcast)
        {
            if (alignment == EAlignment.Good)
            {
                returnList.Add("Wretch_80988916"); // Good Outcast: Wretch
            }
            if (alignment == EAlignment.Evil)
            {
                returnList.Add("Bombardier_79093372"); // Evil Outcast: Bombardier
            }
        }
        if (type == ECharacterType.Minion)
        {
            if (alignment == EAlignment.Good)
            {
                returnList.Add("Witch_25286521"); // Good Minion: Witch
            }
            if (alignment == EAlignment.Evil)
            {
                returnList.Add("Poisoner_64796285"); // Evil Minion: Poisoner
            }
        }
        if (type == ECharacterType.Demon)
        {
            if (alignment == EAlignment.Good)
            {
                returnList.Add("Confessor_18741708"); // Good Demon: Confessor
            }
            if (alignment == EAlignment.Evil)
            {
                returnList.Add("Lillith_90453844"); // Evil Demon: Lilis
            }
        }
        return returnList;
    }

    string roleColour(string type)
    {
        switch (type)
        {
            // Types
            case "Villager": return formattedKeyText("VillagerColour");
            case "Outcast": return formattedKeyText("OutcastColour");
            case "Minion": return formattedKeyText("MinionColour");
            case "Demon": return formattedKeyText("DemonColour");
            case "EvilVillager": return formattedKeyText("EvilVillagerColour");
            case "EvilOutcast": return formattedKeyText("EvilOutcastColour");
            case "GoodMinion": return formattedKeyText("GoodMinionColour");
            case "GoodDemon": return formattedKeyText("GoodDemonColour");
        }
        return formattedKeyText("");
    }

    public static Color getColour(ECharacterType type, EAlignment alignment, string field)
    {
        // Type = character type
        // Alignment = character alignment
        // Field = "color" for text colour, "cardBgColor" for card background colour, "cardBorderColor" for the border colour and "artBgColor" for the art background colour.
        // In summary, field = "color", "cardBgColor", "cardBorderColor" or "artBgColor".
        Color returnColour = new Color(0, 0, 0);
        if (field == "artBgColor")
        {
            return getColour(type, alignment, "cardBorderColor");
        }
        if (type == ECharacterType.Villager)
        {
            if (alignment == EAlignment.Good)
            {
                switch (field)
                {
                    // Types
                    case "color": return new Color(1f, 0.9333f, 0.7294f);
                    case "cardBgColor": return new Color(0.2588f, 0.1529f, 0.3411f);
                    case "cardBorderColor": return new Color(0.7137f, 0.3372f, 0.8666f);
                }
            }
            else
            {
                switch (field)
                {
                    // Types
                    case "color": return new Color(0.9098f, 0.7764f, 1f); // E8C6FF
                    case "cardBgColor": return new Color(0.1647f, 0.1058f, 0.2f); // 2A1B33
                    case "cardBorderColor": return new Color(0.6078f, 0.1843f, 0.6823f); // 9B2FAE
                }
            }
        }
        if (type == ECharacterType.Outcast)
        {
            if (alignment == EAlignment.Good)
            {
                switch (field)
                {
                    // Types
                    case "color": return new Color(0.9647f, 1, 0.447f);
                    case "cardBgColor": return new Color(0.1019f, 0.0666f, 0.0392f);
                    case "cardBorderColor": return new Color(0.7843f, 0.6470f, 0);
                }
            }
            else
            {
                switch (field)
                {
                    // Types
                    case "color": return new Color(1, 0.6666f, 0.9568f); // E8C6FF
                    case "cardBgColor": return new Color(0.2509f, 0, 0.2156f); // 2A1B33
                    case "cardBorderColor": return new Color(1, 0, 0.8666f); // FF00DD
                }
            }
        }
        if (type == ECharacterType.Minion)
        {
            if (alignment == EAlignment.Evil)
            {
                switch (field)
                {
                    // Types
                    case "color": return new Color(0.8509f, 0.4549f, 0);
                    case "cardBgColor": return new Color(0.094f, 0.0431f, 0.04313f);
                    case "cardBorderColor": return new Color(0.8196f, 0, 0.0235f);
                }
            }
            else
            {
                switch (field)
                {
                    // Types
                    case "color": return new Color(0.7882f, 1, 0.9490f); // E8C6FF
                    case "cardBgColor": return new Color(0.0588f, 0.1647f, 0.1647f); // 2A1B33
                    case "cardBorderColor": return new Color(0.2f, 0.8196f, 0.7764f); // 33D1C6
                }
            }
        }
        if (type == ECharacterType.Demon)
        {
            if (alignment == EAlignment.Evil)
            {
                switch (field)
                {
                    // Types
                    case "color": return new Color(1, 0.3803f, 0.3803f);
                    case "cardBgColor": return new Color(0.0941f, 0.0431f, 0.0431f);
                    case "cardBorderColor": return new Color(0.8196f, 0, 0.0235f);
                }
            }
            else
            {
                switch (field)
                {
                    // Types
                    case "color": return new Color(1f, 0.9607f, 0.8784f); // E8C6FF
                    case "cardBgColor": return new Color(0.1019f, 0.0588f, 0.1803f); // 2A1B33
                    case "cardBorderColor": return new Color(0.4784f, 0.3607f, 1f); // 7A5CFF
                }
            }
        }
        return returnColour;
    }

    string formattedKeyText(string target)
    {
        switch (target)
        {
            // Keywords
            case "Honest": return "<color=#7AC6FF>Honest</color>";
            case "Pure": return "<color=#7AFBFF>Pure</color>";
            case "Cure": return "<color=#7AFBFF>Cure</color>";
            case "Cured": return "<color=#7AFBFF>Cured</color>";
            case "Heal": return "<color=#2EFF43>Heal</color>";
            case "Max Health": return "<color=#7AFBFF>Max Health</color>";
            case "Health": return "<color=#7AFBFF>Health</color>";
            case "Damage": return "<color=#C72424>Damage</color>";
            case "True Role": return "<color=#57E69C>True Role</color>";
            case "Truthful": return "<color=#3A95D6>Truthful</color>";
            case "Truth": return "<color=#3A95D6>Truth</color>";
            case "Reveal": return "<color=#A1E6E2>Reveal</color>";
            case "Reveals": return "<color=#A1E6E2>Reveals</color>";
            case "Revealed": return "<color=#A1E6E2>Revealed</color>";
            case "Hidden": return "<color=#697D91>Hidden</color>";
            case "Unrevealed": return "<color=#697D91>Unrevealed</color>";
            case "Bluff": return "<color=#D96EDB>Bluff</color>";
            case "Bluffing": return "<color=#D96EDB>Bluffing</color>";
            case "Attack": return "<color=#FF0037>Attack</color>";
            case "Kill": return "<color=#FF0037>Kill</color>";
            case "Killed": return "<color=#FF0037>Killed</color>";
            case "Killing": return "<color=#FF0037>Killing</color>";
            case "Dead": return "<color=#B36979>Dead</color>";
            case "Die": return "<color=#B36979>Die</color>";
            case "Alive": return "<color=#A4EDB7>Alive</color>";
            case "Living": return "<color=#A4EDB7>Living</color>";
            case "Deck": return "<color=#789AF0>Deck</color>";
            case "Lose": return "<color=#FF0000>Lose</color>";
            // Cycle is gonna be a long one because of the fancy gradient I'm doing
            case "Cycle": return "<color=#99ff99>C</color><color=#99e6b3>y</color><color=#99cccc>c</color><color=#99b3e6>l</color><color=#9999ff>e</color>";
            case "Cycle 1": return "<color=#99ff99>C</color><color=#99e6b3>y</color><color=#99cccc>c</color><color=#99b3e6>l</color><color=#9999ff>e 1</color>";
            case "Cycle 2": return "<color=#99ff99>C</color><color=#99e6b3>y</color><color=#99cccc>c</color><color=#99b3e6>l</color><color=#9999ff>e 2</color>";
            case "Cycle 3": return "<color=#99ff99>C</color><color=#99e6b3>y</color><color=#99cccc>c</color><color=#99b3e6>l</color><color=#9999ff>e 3</color>";
            case "Cycle 4": return "<color=#99ff99>C</color><color=#99e6b3>y</color><color=#99cccc>c</color><color=#99b3e6>l</color><color=#9999ff>e 4</color>";
            case "Cycle 5": return "<color=#99ff99>C</color><color=#99e6b3>y</color><color=#99cccc>c</color><color=#99b3e6>l</color><color=#9999ff>e 5</color>";
            case "Cycle 6": return "<color=#99ff99>C</color><color=#99e6b3>y</color><color=#99cccc>c</color><color=#99b3e6>l</color><color=#9999ff>e 6</color>"; // Cycles beyond 6 are pointless

            // Custom role keywords
            case "Poison": return "<color=#3F8538>Poison</color>"; // For unused Toxomancer role.
            case "Poisoned": return "<color=#3F8538>Poisoned</color>";
            case "Trick": return "<color=#70E8FF>Trick</color>"; // Used by Faerie.
            case "Tricked": return "<color=#70E8FF>Tricked</color>";
            case "Bewildered": return "<color=#70E8FF>Bewil</color><color=#FF00DD>dered</color>"; // Also used by Faerie.
            case "Misled": return "<color=#FF00AE>Misled</color>"; // Used by Venelum and Vidiyon.


            // Devs
            case "Normandia": return "<color=#CE1119>Normandia</color>";
            case "Uzabi": return "<color=#CE1119>Uzabi</color>";

            // Modders
            case "@tstidon": return "<color=#7289DA>@</color><color=#C080FF>tstidon</color>";
            case "TSTidon": return "<color=#C080FF>TSTidon</color>";
            case "WWW": return "<color=#3BA55C>WWW is not taken</color>";
            case "@WWW": return "<color=#7289DA>@</color><color=#3BA55C>wwwisnottaken</color>";
            case "Carlz": return "<color=#5FC4F9>Carlz</color>";
            case "@Carlz": return "<color=#7289DA>@</color><color=#5FC4F9>carlz54339</color>";

            // Art credits
            case "Blue Cheesed": return "<color=#D8D8D8>Blue Cheesed</color>"; // Arithmetician
            case "@Blue Cheesed": return "<color=#7289DA>@</color><color=#D8D8D8>hydethefish</color>";
            case "WeekendWolf": return "<color=#5476ff>WeekendWolf</color>"; // Forager, Sentinel, Lunatic
            case "@weekendwolf": return "<color=#7289DA>@</color><color=#5476ff>hellzalley</color>";
            case "Astery": return "<color=#d506c7>Astery</color>"; // Gemcrafter
            case "@astery": return "<color=#7289DA>@</color><color=#d506c7>astery__</color>";
            case "LostIllustrator": return "<color=#45e0f8>Lost Illustrator</color>"; // Scavenger
            case "@lostillustrator": return "<color=#7289DA>@</color><color=#45e0f8>lostillustrator</color>";
            case "Hiraeth": return "<color=#4b53d5>Hiraeth</color>"; // Warden
            case "@hiraeth": return "<color=#7289DA>@</color><color=#4b53d5>lullabiesmourn</color>";
            case "Panda": return "<color=#cadee6>Panda</color>"; // Spy
            case "@Panda": return "<color=#7289DA>@</color><color=#cadee6>@pandacharly</color>";
            case "Derpy_Feesh": return "<color=#7948d7>Derpy_Feesh</color>"; // Leviathan
            case "@derpy_feesh": return "<color=#7289DA>@</color><color=#7948d7>derpy_feesh</color>"; // Leviathan

            // Special thanks
            case "NoLucksGiven": return "<color=#FFC07B>NoLucksGiven</color>"; // Played mod on YouTube, brought attention to it.
            case "D_NoLucksGiven": return "<color=#7289DA>@</color><color=#FFC07B>nolucksgiven</color>";
            case "Y_NoLucksGiven": return $"<color=#FFC07B>https://www.{formattedKeyText("YouTube")}.com/c/NoLucksGiven</color>";
            case "Fi": return "<color=#96EAFF>Fi the Dragonfly</color>"; // Faerie character is literally Fi lmao
            case "@fithedragonfly": return "<color=#96EAFF>@fithedragonfly</color>";

            // Colours
            case "VillagerColour": return "<color=#B656DD>";
            case "VillagerAltColour": return "<color=#C080FF>";
            case "OutcastColour": return "<color=#F6FF72>";
            case "OutcastAltColour": return "<color=#C8A500>";
            case "MinionColour": return "<color=#D97400>";
            case "DemonColour": return "<color=#FF6161>";

            // Colours, Alignment Flip
            case "EvilVillagerColour": return "<color=#9B2FAE>";
            case "EvilOutcastColour": return "<color=#FF00DD>";
            case "GoodMinionColour": return "<color=#33D1C6>";
            case "GoodDemonColour": return "<color=#7A5CFF>";

            // Platforms
            case "Discord": return "<color=#7289DA>Discord</color>";
            case "Tumblr": return "<color=#36465D>Tumblr</color>";
            case "YouTube": return "<color=#FE0000>YouTube</color>";
            case "Youtube": return "<color=#FE0000>YouTube</color>";
        }
        return "Formatted key text invalid, please report this to TSTidon.";
    }


    public void PatchVanillaCharacterDescriptions()
    {
        for (int i = 0; i < allDatas.Count(); i++)
        {
            MelonLogger.Msg($"Description Patcher: Found {allDatas[i].name.ToString()}");
            if (allDatas[i].characterName == "Witness")
            {
                allDatas[i].hints += "\n- Character Corrupted by Pestilence" +
                                     "\n- Character transformed by Cultist";
                MelonLogger.Msg($"Patched Witness. New description: {allDatas[i].hints}");
            }
        }
    }
    //int toxomancerPoisonTimer = 0;
    //int toxomancerDeathTimer = 0;


    /*private void OnCharacterRevealed(Character revealed)
    {
        toxomancerPoisonTimer -= 1;
        toxomancerDeathTimer -= 1;
        CharacterData charData = revealed.dataRef;
        Il2CppSystem.Collections.Generic.List<Character> allChars = new Il2CppSystem.Collections.Generic.List<Character>(Gameplay.CurrentCharacters.Pointer);

        int revealCount = 0;
        for (int i = 0; i < allChars.Count; i++)
        {
            if (allChars[i].revealed == true)
            {
                revealCount++;
            }
        }
        if (revealCount == 1)
        {
            toxomancerPoisonTimer = 2;
            toxomancerDeathTimer = 4;
        }

        bool toxomancerInPlay = false;
        Character toxomancer = new Character();
        for (int i = 0; i < allChars.Count; i++)
        {
            if (allChars[i].dataRef.characterId == "Toxomancer_TST" && allChars[i].state != ECharacterState.Dead)
            {
                toxomancerInPlay = true;
                break;
            }
            if (allChars[i].dataRef.characterId == "Toxomancer_TST")
            {
                toxomancer = allChars[i];
            }
        }
        if (toxomancerInPlay)
        {
            if (toxomancerPoisonTimer == 0)
            {
                Il2CppSystem.Collections.Generic.List<Character> possiblePoisonTargets = new Il2CppSystem.Collections.Generic.List<Character>();
                foreach (Character character in allChars)
                {
                    if (character.GetRegisterAs().type == ECharacterType.Villager && character.GetRegisterAlignment() == EAlignment.Good && character.state != ECharacterState.Dead)
                    {
                        possiblePoisonTargets.Add(character);
                    }
                }
                Character poisonTarget = possiblePoisonTargets[UnityEngine.Random.RandomRangeInt(0, possiblePoisonTargets.Count)];
                poisonTarget.statuses.AddStatus(ECharacterStatus.Corrupted, toxomancer);
                poisonTarget.statuses.AddStatus(w_Toxomancer.ToxomancerPoison.toxomancerPoison, toxomancer);
                toxomancerPoisonTimer = 3;
                toxomancerDeathTimer = 2;
            }
        }
        if (toxomancerDeathTimer == 0)
        {
            foreach (Character character in allChars)
            {
                if (character.statuses.Contains(w_Toxomancer.ToxomancerPoison.toxomancerPoison))
                {
                    PlayerController.PlayerInfo.health.Damage(1);
                    character.RevealAllReal();
                    character.KillByDemon(toxomancer);
                }
            }
        }

    }*/

























    /*
    [HarmonyPatch(typeof(Gossip), nameof(Gossip.Act))]
    private static class GetPoetTrueInfo
    {
        private static bool Prefix(Gossip __instance, ETriggerPhase trigger, Character charRef)
        {
            if (trigger != ETriggerPhase.Day) return true;
            if (charRef.bluff)
            {
                if (charRef.bluff.characterId != "Gossip_85354100")
                {
                    return true;
                }
            }
            else if (charRef.dataRef.characterId != "Gossip_85354100")
            {
                return true;
            }
            Il2CppSystem.Collections.Generic.List<Role> infoRoles = new Il2CppSystem.Collections.Generic.List<Role>();
            infoRoles.Add(new Empath());
            infoRoles.Add(new Scout());
            infoRoles.Add(new Investigator());
            infoRoles.Add(new BountyHunter());
            infoRoles.Add(new Lookout());
            infoRoles.Add(new Knitter());
            infoRoles.Add(new Tracker());
            infoRoles.Add(new Shugenja());
            infoRoles.Add(new Noble());
            infoRoles.Add(new Bishop());
            infoRoles.Add(new Archivist());
            infoRoles.Add(new Acrobat2());
            infoRoles.Add(new w_Arithmetician());
            infoRoles.Add(new w_Chiromancer());
            infoRoles.Add(new w_Clairvoyant());
            infoRoles.Add(new w_Detective());
            infoRoles.Add(new w_Introvert());
            infoRoles.Add(new w_Jewelsmith());
            infoRoles.Add(new w_Lamb());
            infoRoles.Add(new w_Prince());
            infoRoles.Add(new w_Ranger());
            infoRoles.Add(new w_Sentinel());
            infoRoles.Add(new w_Spy());
            ActedInfo myInfo = infoRoles[UnityEngine.Random.RandomRangeInt(0, infoRoles.Count)].GetInfo(charRef);
            __instance.onActed?.Invoke(myInfo);
            return false;
        }
    }


    [HarmonyPatch(typeof(Gossip), nameof(Gossip.BluffAct))]
    private static class GetPoetFalseInfo
    {
        private static bool Prefix(Gossip __instance, ETriggerPhase trigger, Character charRef)
        {
            if (trigger != ETriggerPhase.Day) return true;
            if (charRef.bluff)
            {
                if (charRef.bluff.characterId != "Gossip_85354100")
                {
                    return true;
                }
            }
            else if (charRef.dataRef.characterId != "Gossip_85354100")
            {
                return true;
            }
            Il2CppSystem.Collections.Generic.List<Role> infoRoles = new Il2CppSystem.Collections.Generic.List<Role>();
            infoRoles.Add(new Empath());
            infoRoles.Add(new Scout());
            infoRoles.Add(new Investigator());
            infoRoles.Add(new BountyHunter());
            infoRoles.Add(new Lookout());
            infoRoles.Add(new Knitter());
            infoRoles.Add(new Tracker());
            infoRoles.Add(new Shugenja());
            infoRoles.Add(new Noble());
            infoRoles.Add(new Bishop());
            infoRoles.Add(new Archivist());
            infoRoles.Add(new Acrobat2());
            infoRoles.Add(new w_Arithmetician());
            infoRoles.Add(new w_Chiromancer());
            infoRoles.Add(new w_Clairvoyant());
            infoRoles.Add(new w_Detective());
            infoRoles.Add(new w_Introvert());
            infoRoles.Add(new w_Jewelsmith());
            infoRoles.Add(new w_Lamb());
            infoRoles.Add(new w_Prince());
            infoRoles.Add(new w_Ranger());
            infoRoles.Add(new w_Sentinel());
            infoRoles.Add(new w_Spy());
            ActedInfo myInfo = infoRoles[UnityEngine.Random.RandomRangeInt(0, infoRoles.Count)].GetBluffInfo(charRef);
            __instance.onActed?.Invoke(myInfo);
            return false;
        }
    }
    */


    /* Was causing crashes.
    [HarmonyPatch(typeof(Investigator), nameof(Investigator.BluffAct))]
    private static class GetOracleFalseInfo // Practically identical, save for the fact that it can't see Good Minions. Should fix problems with Good Swarm.
    {
        private static bool Prefix(Gossip __instance, ETriggerPhase trigger, Character charRef)
        {
            if (trigger != ETriggerPhase.Day) return true;
            if (charRef.bluff)
            {
                if (charRef.bluff.characterId != "Oracle_07039445")
                {
                    return true;
                }
            }
            else if (charRef.dataRef.characterId != "Oracle_07039445")
            {
                return true;
            }
            Il2CppSystem.Collections.Generic.List<Character> possibleInfoTargets = new Il2CppSystem.Collections.Generic.List<Character>();
            Il2CppSystem.Collections.Generic.List<Character> infoTargets = new Il2CppSystem.Collections.Generic.List<Character>();
            Il2CppSystem.Collections.Generic.List<CharacterData> deckMinions = Gameplay.Instance.GetScriptCharactersOfType(ECharacterType.Minion);
            CharacterData chosenMinion = new CharacterData();
            if (deckMinions.Count == 0)
            {
                foreach (CharacterData character in Gameplay.Instance.GetAllAscensionCharacters())
                {
                    if (character.type == ECharacterType.Minion)
                    {
                        deckMinions.Add(character);
                    }
                }
            }
            chosenMinion = deckMinions[UnityEngine.Random.RandomRangeInt(0, deckMinions.Count)];
            foreach (Character character in Gameplay.CurrentCharacters)
            {
                if (character.GetRegisterAlignment() == EAlignment.Good && character.GetCharacterType() != ECharacterType.Minion)
                {
                    possibleInfoTargets.Add(character);
                }
            }
            string actInfo = "";
            if (possibleInfoTargets.Count < 2)
            {
                actInfo = "This village confuses me.";
            }
            infoTargets.Add(possibleInfoTargets[UnityEngine.Random.RandomRangeInt(0, possibleInfoTargets.Count)]);
            possibleInfoTargets.Remove(infoTargets[0]);
            infoTargets.Add(possibleInfoTargets[UnityEngine.Random.RandomRangeInt(0, possibleInfoTargets.Count)]);

            if (infoTargets[0].id < infoTargets[1].id)
            {
                actInfo = string.Format("#{0} or #{1} is a {2}", infoTargets[0].id, infoTargets[1].id, chosenMinion.name.ToString());
            }
            else
            {
                actInfo = string.Format("#{0} or #{1} is a {2}", infoTargets[1].id, infoTargets[0].id, chosenMinion.name.ToString());
            }
            ActedInfo myInfo = new ActedInfo(actInfo, infoTargets);
            __instance.onActed?.Invoke(myInfo);
            return false;
        }
    }
    */


    [HarmonyPatch(typeof(ObjectivesUI), nameof(ObjectivesUI.UpdateObjectives))]
    public static class ChangeCounter
    {
        public static void Postfix(ObjectivesUI __instance)
        {
            //bool LilisInPlay = false;
            int minions = Gameplay.CurrentScript.minion;
            int demons = Gameplay.CurrentScript.demon;
            int MaxEvils = minions + demons;
            var deadCharacters = Gameplay.DeadCharacters;
            Il2CppSystem.Collections.Generic.List<Character> allCurrentCharacters = new Il2CppSystem.Collections.Generic.List<Character>(Gameplay.CurrentCharacters.Pointer);
            Il2CppSystem.Collections.Generic.List<CharacterData> allCurrentCharactersData = new Il2CppSystem.Collections.Generic.List<CharacterData>(Gameplay.Instance.GetScriptCharacters().Pointer);
            Il2CppSystem.Collections.Generic.List<string> Evils = new();
            //Il2CppSystem.Collections.Generic.List<string> allCurrentCharactersNames;
            //Il2CppSystem.Collections.Generic.List<string> allCurrentCharactersDataNames;

            //allCurrentCharactersNames = sortByName(allCurrentCharacters);
            //allCurrentCharactersDataNames = sortByName(allCurrentCharactersData);






            int minEvilsKilled = 0;
            int maxEvilsKilled = 0;
            int AddedEvils = 0;
            //int AddedEvils1 = 0;
            //int AddedEvils2 = 0;

            foreach (var deadCharacter in deadCharacters)
            {
                if (deadCharacter.alignment == EAlignment.Evil || deadCharacter.statuses.Contains(HiddenRoleStatus.hiddenRole))
                {
                    maxEvilsKilled++;
                    if (!deadCharacter.statuses.Contains(HiddenRoleStatus.hiddenRole))
                    {
                        minEvilsKilled++;
                    }
                }
            }


            //foreach (var character in allCurrentCharacters)
            //{

            //string characterData = allCurrentCharactersData[i].name.ToString();
            //string character;

            /*if (i <= allCurrentCharacters.Count - 1)
            {
               character = allCurrentCharactersNames[i];
            }
            else
            {
                character = "";
            }*/
            //MelonLogger.Msg("Character: " + character.dataRef.name.ToString());

            /*if (character == "Belias" || character == "Mayor" || character == "Good Twin" || character == "Puppeteer" || character == "Hypnotist" || character == "Executioner")
            {

                AddedEvils1++;
            }*/


            //if (character.dataRef.name == "Belias" || character.dataRef.name == "Mayor" || character.dataRef.name == "Good Twin" || character.dataRef.name == "Puppeteer" || character.dataRef.name == "Hypnotist" || character.dataRef.name == "Executioner")
            //{
            //if (Evils.Contains(character.dataRef.name.ToString()))
            //{
            //    AddedEvils++;
            //}

            //else
            //{
            //   Evils.Add(character.dataRef.name.ToString());
            //    AddedEvils++;
            //}

            //}

            //}

            //foreach (var characterData in allCurrentCharactersData)
            //{
            //   if (characterData.name.ToString() == "Hellspawn")
            //        MaxEvils++;
            //    if (characterData.name == "Belias" || characterData.name == "Mayor" || characterData.name == "Good Twin" || characterData.name == "Puppeteer" || characterData.name == "Hypnotist" || characterData.name == "Executioner")
            //    {

            //        if (!Evils.Contains(characterData.name.ToString()))
            //        {
            //            Evils.Add(characterData.name.ToString());
            //            AddedEvils++;
            //        }
            //    }

            //}

            /*if(AddedEvils2 > AddedEvils1)
            {
                AddedEvils = AddedEvils2;
            }

            else
            {
                AddedEvils = AddedEvils1;
            }*/

            //string EvilsKilledText = EvilsKilled.ToString();
            //string MaxEvilsAmount = AddedEvils.ToString();

            //if (MaxEvils < minions + demons)
            // MaxEvils++;
            if (minEvilsKilled == maxEvilsKilled)
            {
                __instance.evilsKilled.text = System.String.Format("<color=grey>Evils killed:</color> <color=red>{0}", minEvilsKilled);
            }
            else
            {
                __instance.evilsKilled.text = System.String.Format("<color=grey>Evils killed:</color> <color=red>{0}-{1}", minEvilsKilled, maxEvilsKilled);
            }


            /* else if(MaxEvils < minions + demons)
             {
                 MaxEvilsText = System.String.Format("<color=red>{0}-{1}", MaxEvils, minions + demons);
             }*/

            //if(LilisInPlay)
            // {
            //    EvilsKilledText = "?";
            // }

            // LilisInPlay = false;

            string minionCountText = "Minions";
            if (minions == 1)
            {
                minionCountText = "Minion";
            }
            string demonCountText = "Demons";
            if (demons == 1)
            {
                demonCountText = "Demon";
            }
            __instance.objective.text = System.String.Format("Find and Execute all Evil Characters<br><color=grey><size=18>(<color=orange>{0}+ {2}</color> and <color=red>{1}+ {3} </color>)", minions, demons, minionCountText, demonCountText);

        }
    }


    public static class Statics
    {
        public static Dictionary<string, CharacterData> roles = new Dictionary<string, CharacterData>();
        public static CharacterData[] charactersArray = Il2CppSystem.Array.Empty<CharacterData>();
        /*
        public static void checkCreateCircle(Transform parent, int size)
        {
            string name = "Circle_" + size;
            Transform t = parent.FindChild(name);
            if (t != null)
            {
                MelonLogger.Msg("Object Already exists!: " + name);
                return;
            }
            //createCircle(parent, size, name);
        }
        public static GameObject createCircle(int size) // I'm just gonna wait for WWW to figure this out
        {
            GameObject circle = new GameObject();
            circle.name = "Circle_" + size;
            circle.transform.SetParent(Characters.Instance.gameObject.transform);
            RectTransform rect = circle.AddComponent<RectTransform>();
            CharactersPool circPool = circle.AddComponent<CharactersPool>();
            GameObject circ6 = Characters.Instance.gameObject.transform.Find("Circle_6").gameObject;
            CharactersPool circ6Pool = circ6.GetComponent<CharactersPool>();
            circPool.characterPrefab = circ6Pool.characterPrefab;
            circPool.characters = new Character[0];
            circPool.cardPlaceHolders = new CardPlaceholder[size];
            for (int i = 0; i < size; i++)
            {
                GameObject cardHolder = new GameObject();
                cardHolder.transform.SetParent(circle.transform);
                string name = "CardPlaceholder";
                if (i > 0)
                {
                    name += " (" + i + ")";
                }
                cardHolder.name = name;
                RectTransform cardRect = cardHolder.AddComponent<RectTransform>();
                cardRect.anchoredPosition3D = new Vector3(0f, 0f, 0f);
                CardPlaceholder placeholder = cardHolder.AddComponent<CardPlaceholder>();
                int angle = i * 360 / size;
                if (angle <= 30)
                {
                    placeholder.actedSide = EActedSide.Down;
                }
                else if (angle <= 149)
                {
                    placeholder.actedSide = EActedSide.Left;
                }
                else if (angle <= 210)
                {
                    placeholder.actedSide = EActedSide.Up;
                }
                else if (angle <= 329)
                {
                    placeholder.actedSide = EActedSide.Right;
                }
                else
                {
                    placeholder.actedSide = EActedSide.Down;
                }
                circPool.cardPlaceHolders[i] = placeholder;
            }
            circle.transform.position = new UnityEngine.Vector3(0f, 1f, 85.9444f);
            circle.transform.localScale = new UnityEngine.Vector3(1f, 1f, 1f);
            circle.SetActive(false);
            addToCharsPool(circPool);
            return circle;
        }
        */
        public static void addToCharsPool(CharactersPool pool)
        {
            CharactersPool[] pools = Characters.Instance.characterPool;
            CharactersPool[] newPools = new CharactersPool[pools.Length + 1];
            for (int i = 0; i < pools.Length; i++)
            {
                newPools[i] = pools[i];
            }
            newPools[pools.Length] = pool;
            Characters.Instance.characterPool = newPools;
        }

        public static void GetStartingRoles()
        {
            AscensionsData allCharactersAscension = ProjectContext.Instance.gameData.allCharactersAscension;
            foreach (CharacterData data in allCharactersAscension.startingTownsfolks)
            {
                CheckAddRole(data);
            }
            foreach (CharacterData data in allCharactersAscension.startingOutsiders)
            {
                CheckAddRole(data);
            }
            foreach (CharacterData data in allCharactersAscension.startingMinions)
            {
                CheckAddRole(data);
            }
            foreach (CharacterData data in allCharactersAscension.startingDemons)
            {
                CheckAddRole(data);
            }
        }
        public static void CheckAddRole(CharacterData data)
        {
            string name = data.name;
            if (!roles.ContainsKey(name))
            {
                roles.Add(name, data);
            }
        }

    }
}