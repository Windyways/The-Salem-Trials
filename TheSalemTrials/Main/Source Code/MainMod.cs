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

[assembly: MelonInfo(typeof(MainMod), "Windyways_TheSalemTrials", "1.2.0", "Windyways")]
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
        ClassInjector.RegisterTypeInIl2Cpp<Cleric>();
        ClassInjector.RegisterTypeInIl2Cpp<Psychic>();

        // Outcasts
        //ClassInjector.RegisterTypeInIl2Cpp<Starspawn>();

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
        // Trickster - I Lie and Disguise as a bluffing villager.

        // --- DEMON ---
        // Serial Killer - At Night, i kill an unrevealed Good Villager if possible. Nighttime comes 2x faster. I do not Lie. I do not Disguise.
        // Cultist - Game Start: All Outcasts are replaced with 'Minion'. I Lie and Disguise.
        // Pestilence (v2) - Reveal: Deal 1 damage to you each time a card is revealed. If Executed, Corrupt remaining unrevealed Good Villagers. I do not lie and I do not Disguise.
        // Vampire - Game Start:\nAll Minions become Vampires.\n\nYou take 1 damage when Executing me.\n\nI Lie and Disguise.

        // --- VILLAGERS ---
        CharacterData sheriff = newCharacter("Sheriff", EAlignment.Good, ECharacterType.Villager, true, false, "\"The gun shows 'em who's boss. Don't lie to me.\"", "Bishop_58855542");
        sheriff.role = new Sheriff();
        sheriff.description = "Learn how many liars are adjacent to me.";
        sheriff.ifLies = $"I say '0 Liars' if there are adjacent Evils.\nI say '2 Liars' if there are no adjacent Evils.";
        sheriff.gender = EGender.Male;

        CharacterData admirer = newCharacter("Admirer", EAlignment.Good, ECharacterType.Villager, true, false, "\"This may sound creepy, but i watched her through the window.\"", "Bishop_58855542");
        admirer.role = new Admirer();
        admirer.description = "Pick 1 character:\nLearn if they are Corrupted.";
        admirer.ifLies = $"I say the opposite from the Truth.";
        admirer.picking = true;
        admirer.abilityUsage = EAbilityUsage.Once;
        admirer.gender = EGender.Male;

        CharacterData detective = newCharacter("Detective", EAlignment.Good, ECharacterType.Villager, true, false, "\"The Investigator wonders why i’m better than him. I told him ‘because i just am’.\"", "Bishop_58855542");
        detective.role = new Detective();
        detective.description = "Learn how close an evil is to a random Good Villager.";
        detective.ifLies = $"I do not say the Truth.";
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

        CharacterData cleric = newCharacter("Cleric", EAlignment.Good, ECharacterType.Villager, true, false, "\"I am skilled in the art of purification. Unlike those people.\"", "Bishop_58855542");
        cleric.role = new Cleric();
        cleric.description = "Reveal:\nCleanse 1 Good Villager of Corruption (if possible). I Learn who I Cleansed.";
        cleric.ifLies = $"I say opposite from the Truth and I do not Cleanse.";
        cleric.hints = "If I am Revealed before I Cleanse my target, they will still Lie.";
        cleric.gender = EGender.Male;

        CharacterData psychic = newCharacter("Psychic", EAlignment.Good, ECharacterType.Villager, true, false, "\"The gun shows 'em who's boss. Don't lie to me.\"", "Bishop_58855542");
        psychic.role = new Psychic();
        psychic.description = "Learn how many Evils have been Revealed.";
        psychic.ifLies = $"I do not say the Truth.";
        psychic.gender = EGender.Male;

        // --- OUTCASTS ---
        /*CharacterData starspawn = newCharacter("Starspawn", EAlignment.Good, ECharacterType.Outcast, true, true, "\"I don’t like to reveal my identity. But i am talented at everything.\"", "Imp_58992273");
        starspawn.role = new Starspawn();
        starspawn.description = "I Disguise as an in-play Outcast (if possible).";
        starspawn.gender = EGender.Male; - I'll find a possible solution to make this different from Doppel.
        */

        // --- MINIONS ---
        CharacterData illusionist = newCharacter("Illusionist", EAlignment.Evil, ECharacterType.Minion, true, true, "\"I am the best at casting magic huh?\"", "Imp_58992273");
        illusionist.role = new Illusionist();
        illusionist.description = "I Lie and Disguise as a not-in-play Villager.";
        illusionist.gender = EGender.Male;

        CharacterData shroud = newCharacter("Shroud", EAlignment.Evil, ECharacterType.Minion, true, true, "\"I’m like, literally a ghost. That’s bluffing. Yeah.\"", "Imp_58992273");
        shroud.role = new Shroud();
        shroud.description = "I Lie and Disguise as an in-play Outcast or Villager (if possible).";
        shroud.hints = "If there are no valid characters to Disguise as, i will not Lie or Disguise.";
        shroud.gender = EGender.Male;

        // --- DEMONS ---


        // Vanilla order: Baa, Chancellor, Pooka, Poisoner, Witch, Puppeteer, Plague Doctor, Shaman, Alchemist, Puppet, Lilis
        //Characters.Instance.startGameActOrder = InsertAfterAct("Chancellor", vampire);

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
            addRole(script.startingTownsfolks, cleric);
            addRole(script.startingTownsfolks, psychic);

            //addRole(script.startingOutsiders, starspawn);

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
        //public static ECharacterStatus hiddenRole = (ECharacterStatus)1000;
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
            case "Cleanse": return "<color=#a0d1d0>Cleanse</color>";

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
        }
        return "Formatted key text invalid.";
    }


    public void PatchVanillaCharacterDescriptions()
    {
        for (int i = 0; i < allDatas.Count(); i++)
        {
            MelonLogger.Msg($"Description Patcher: Found {allDatas[i].name.ToString()}");
            /*if (allDatas[i].characterName == "Witness")
            {
                allDatas[i].hints += "\n- Character Corrupted by Pestilence" +
                                     "\n- Character transformed by Cultist";
                MelonLogger.Msg($"Patched Witness. New description: {allDatas[i].hints}");
            }*/
        }
    }




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


            int minEvilsKilled = 0;
            int maxEvilsKilled = 0;
            int AddedEvils = 0;
            //int AddedEvils1 = 0;
            //int AddedEvils2 = 0;

            foreach (var deadCharacter in deadCharacters)
            {
                if (deadCharacter.alignment == EAlignment.Evil/* || deadCharacter.statuses.Contains(HiddenRoleStatus.hiddenRole)*/)
                {
                    maxEvilsKilled++;
                    //if (!deadCharacter.statuses.Contains(HiddenRoleStatus.hiddenRole))
                    //{
                        minEvilsKilled++;
                    //}
                }
            }

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