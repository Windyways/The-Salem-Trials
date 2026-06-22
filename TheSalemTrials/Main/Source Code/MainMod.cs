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

[assembly: MelonInfo(typeof(MainMod), "Windyways_TheSalemTrials", "1.0.0", "Windyways")]
[assembly: MelonGame("UmiArt", "Demon Bluff")]

namespace TheSalemTrials;
public class MainMod : MelonMod
{
    public override void OnInitializeMelon()
    {
        // Villagers
        ClassInjector.RegisterTypeInIl2Cpp<Sheriff>();
        ClassInjector.RegisterTypeInIl2Cpp<Admirer>();

        // Minions
        ClassInjector.RegisterTypeInIl2Cpp<Illusionist>();
        ClassInjector.RegisterTypeInIl2Cpp<Pestilence>();

        // Demons
        ClassInjector.RegisterTypeInIl2Cpp<Cultist>();
    }


    public MelonPreferences_Category configCategory = null!;
    public override void OnLateInitializeMelon()
    {
        GameObject content = GameObject.Find("Game/Gameplay/Content");
        NightPhase nightPhase = content.GetComponent<NightPhase>();
        Statics.GetStartingRoles();


        configCategory = MelonPreferences.CreateCategory("TSTModSettings");
        configCategory.CreateEntry("Agmeres_Weight", 2, description: "How likely Agmeres is to be in-play.");
        configCategory.CreateEntry("Veni-Vidi-Vici_Weight", 2, description: "How likely the Hellspawn Triplets are to be in-play.");
        configCategory.CreateEntry("Caedoccidere_Weight", 2, description: "How likely Caedoccidere is to be in-play.");
        configCategory.CreateEntry("Praesect_Weight", 2, description: "How likely Praesect is to be in-play.");
        configCategory.CreateEntry("Mendaverte_Weight", 2, description: "How likely Mendaverte is to be in-play.");
        configCategory.CreateEntry("Venelum_Weight", 2, description: "How likely Venelum is to be in-play.");
        configCategory.CreateEntry("Sanguitaurus_Weight", 2, description: "How likely Sanguitaurus is to be in-play.");
        configCategory.CreateEntry("Tenecaligo_Weight", 2, description: "How likely Tenecaligo is to be in-play.");
        configCategory.CreateEntry("Leviathan_Weight", 2, description: "How likely Leviathan is to be in-play.");
        configCategory.CreateEntry("Iris_Weight", 2, description: "How likely Iris is to be in-play.");
        configCategory.CreateEntry("Carni_Weight", 2, description: "How likely Carnicarius is to be in-play.");
        configCategory.CreateEntry("Misc_Weight", 2, description: "How likely any Demon I've forgotten is to be in-play.");
        configCategory.SetFilePath(Path.Combine(MelonEnvironment.UserDataDirectory, "TSTModConfig.cfg"));
        configCategory.SaveToFile();





        // To potentially add later:
        // Auditor - Learn how many characters fit a particular descriptor (e.g. Good, Evil, Villager, Outcast, Minion, Demon, Corruption, etc)
        // Bounty Hunter - 1 random Good Villager is Evil & Corrupted. Learn 1 Evil character.
        // Investigator - Learn how many Unrevealed characters are Disguised.
        // Paperboy - Learn how long my chain of Good characters is.
        // Partisan - Good characters adjacent to me can't die.
        // Saint - I am always Good.
        // Writer - Learn a Villager role and its distance to its nearest Outcast.
        // ??? (Visionary?) - Every 4 Reveals, Learn that a random character is either a particular Good role or a particular Evil role.

        // Occultist - Has a Minion ability (Make them become a Good Minion that Disguises as Occultist?)
        // Provocateur - Kills a Villager each night, lose if all Villagers die.

        // Follower - Created by a Demon. Deals damage when executed if the Demon still lives.
        // Goblin - Each night, swaps the roles of 2 unrevealed characters if possible.
        // Hypnotist - Forces its neighbours to Disguise, but they don't Lie unless they already would.
        // Prosecutor - Makes a Villager Register as the Prosecutor. When Executed, Learns the role that it prosecuted.
        // Warlock - I have the Demon's ability.
        // ??? - Lies and Disguises as or like a fake Outcast added to the Deck.

        // Aries - Creates an Evil Minion out of a Good Villager. Buff to something better, maybe? Feels a little weak, esp. compared to Praesect. Maybe add fake Minions to the Deck too? Disguise unusually?
        // Clavinus - Name derived from Latin "Pampinus" meaning "Tendril" and "Clavicula" also meaning "Tendril". Sits adjacent to only Villagers & Minions, and Corrupts adjacent Villagers. Tells the truth.
        // Poenitte - Name derived from Latin "Poenitere" meaning "Repent" and "Dimitte" meaning "Forgive". Good Demon. Adds a second Demon - Pick 1: if a Demon picked, execute it. Might also call it Vivian?
        // Sect Leader - Creates a second Demon adjacent to an Evil character. Need to think of a proper name for this. Perhaps could also become a different Demon similar to Magnere?
        // Tempriam - Dead Minions appear as Tempriams. Need to think about how I'd implement something like this. Could overwrite the CharacterData of Minions with Tempriam when their true role is revealed, maybe.

        // --- VILLAGERS ---
        CharacterData sheriff = newCharacter("Sheriff", EAlignment.Good, ECharacterType.Villager, true, false, "\"The gun shows 'em who's boss. Don't lie to me.\"", "Bishop_58855542");
        sheriff.role = new Sheriff();
        sheriff.description = "Learn how many liars are adjacent to you.";
        sheriff.ifLies = $"I say '0 Liars' if there are adjacent Evils.\nI say '2 Liars' if there are no adjacent Evils.";
        sheriff.gender = EGender.Male;

        CharacterData admirer = newCharacter("Admirer", EAlignment.Good, ECharacterType.Villager, true, false, "\"This may sound creepy, but i watched her through the window.\"", "Bishop_58855542");
        admirer.role = new Admirer();
        admirer.description = "Pick 1 character:\nLearn if they are Corrupted.";
        admirer.ifLies = $"I say ‘Not Corrupted’ if the target is Lying.\nI say ‘Corrupted’ if the target is not Corrupted.";
        admirer.gender = EGender.Male;

        // --- MINIONS ---
        CharacterData pestilence = newCharacter("Pestilence", EAlignment.Evil, ECharacterType.Minion, true, false, "\"Mister, talk to the horse.\"", "Imp_58992273");
        pestilence.role = new Pestilence();
        pestilence.description = "Game Start:\nCorrupt a random Good Villager.\n\nI Lie and Disguise.";
        pestilence.gender = EGender.Male;

        CharacterData illusionist = newCharacter("Illusionist", EAlignment.Evil, ECharacterType.Minion, true, true, "\"I am the best at casting magic huh?\"", "Imp_58992273");
        illusionist.role = new Illusionist();
        illusionist.description = "I Lie and Disguise as a not-in-play Villager.";
        illusionist.gender = EGender.Male;

        // --- DEMONS ---
        CharacterData cultist = newCharacter("Cultist", EAlignment.Evil, ECharacterType.Demon, false, true, "\"They weren’t evil on purpose. I made them to.\"", "Imp_58992273");
        cultist.role = new Cultist();
        cultist.description = "Game Start:\nTransform a random Good Villager into an existing Minion.\n\nI Lie and Disguise.\n";
        cultist.gender = EGender.Male;




        MelonLogger.Msg($"Doing act order");


        // Vanilla order: Baa, Chancellor, Pooka, Poisoner, Witch, Puppeteer, Plague Doctor, Shaman, Alchemist, Puppet, Lilis

        //Characters.Instance.startGameActOrder = InsertAtStartOfActOrder(w_pandemonium);

        // Characters.Instance.startGameActOrder = insertAfterAct("Chancellor", w_twindemontriplet); // No longer needs to act on start after rework

        //Characters.Instance.startGameActOrder = insertBeforeAct("Pooka", w_politician);
        //Characters.Instance.startGameActOrder = insertBeforeAct("Pooka", w_twindemon);
        //Characters.Instance.startGameActOrder = insertBeforeAct("Pooka", w_saboteur);

        //Characters.Instance.startGameActOrder = InsertAtEndOfActOrder(w_illusionist);
        //Characters.Instance.startGameActOrder = InsertAtEndOfActOrder(w_shard);
        //Characters.Instance.startGameActOrder = InsertAtEndOfActOrder(w_pilgrim);
        //Characters.Instance.startGameActOrder = InsertAtEndOfActOrder(w_devout);


        MelonLogger.Msg($"Act order done.");

        foreach (CharacterData character in Characters.Instance.startGameActOrder)
        {
            MelonLogger.Msg($"Act Order: {character.characterName}");
        }



        // New act order: Pandemonium, Tenecaligo, Baa, Sanguitaurus, Chancellor, Mutant, Praesect, Undying, Good Swarm, Marionette, Politician, Veniyon, Saboteur, Pooka, Poisoner, Witch, Puppeteer, Plague Doctor, Shaman, Agmeres, Zealot, Fanatic, Acolyte, Vidiyon, Venelum, Copycat, Emenverax, Specularus, Pilgrim, Mendaverte, Devout.







        /*
        foreach (CharacterData character in Characters.Instance.startGameActOrder)
        {
            MelonLogger.Msg($"Game Start order: {character.name.ToString()}");
        }
        */

















        MelonLogger.Msg($"Preparing scripts");


        Il2CppSystem.Collections.Generic.List<CharacterData> emptyCharacterDataList = new Il2CppSystem.Collections.Generic.List<CharacterData>();

        CustomScriptData legionScriptData = new CustomScriptData();
        legionScriptData.name = "Legion_1";
        ScriptInfo legionScript = new ScriptInfo();
        Il2CppSystem.Collections.Generic.List<CharacterData> legionList = new Il2CppSystem.Collections.Generic.List<CharacterData>();
        legionScript.mustInclude = legionList;
        legionScript.startingDemons = legionList;
        legionScript.startingTownsfolks = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingTownsfolks;
        legionScript.startingOutsiders = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingOutsiders;
        legionScript.startingMinions = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingMinions;
        CharactersCount legionCounter1 = new CharactersCount(5, 1, 1, 1, 2);
        legionCounter1.dTown = legionCounter1.town + 3;
        legionCounter1.dOuts = legionCounter1.outs + 1;
        CharactersCount legionCounter2 = new CharactersCount(6, 1, 1, 1, 3);
        legionCounter1.dTown = legionCounter2.town + 3;
        legionCounter2.dOuts = legionCounter2.outs + 1;
        CharactersCount legionCounter3 = new CharactersCount(7, 2, 1, 1, 3);
        legionCounter1.dTown = legionCounter3.town + 4;
        legionCounter3.dOuts = legionCounter3.outs + 1;
        CharactersCount legionCounter4 = new CharactersCount(8, 2, 1, 1, 4);
        legionCounter1.dTown = legionCounter4.town + 4;
        legionCounter4.dOuts = legionCounter4.outs + 1;
        CharactersCount legionCounter5 = new CharactersCount(9, 2, 1, 1, 5);
        legionCounter1.dTown = legionCounter5.town + 5;
        legionCounter4.dOuts = legionCounter5.outs + 1;
        CharactersCount legionCounter6 = new CharactersCount(10, 2, 1, 1, 6);
        legionCounter1.dTown = legionCounter6.town + 5;
        legionCounter4.dOuts = legionCounter6.outs + 1;
        Il2CppSystem.Collections.Generic.List<CharactersCount> legionCounterList = new Il2CppSystem.Collections.Generic.List<CharactersCount>();
        legionCounterList.Add(legionCounter1);
        legionCounterList.Add(legionCounter2);
        legionCounterList.Add(legionCounter2);
        legionCounterList.Add(legionCounter3);
        legionCounterList.Add(legionCounter3);
        legionCounterList.Add(legionCounter3);
        legionCounterList.Add(legionCounter4);
        legionCounterList.Add(legionCounter4);
        legionCounterList.Add(legionCounter4);
        legionCounterList.Add(legionCounter4);
        legionCounterList.Add(legionCounter5);
        legionCounterList.Add(legionCounter5);
        legionCounterList.Add(legionCounter5);
        legionCounterList.Add(legionCounter5);
        legionCounterList.Add(legionCounter5);
        legionCounterList.Add(legionCounter6);
        legionCounterList.Add(legionCounter6);
        legionCounterList.Add(legionCounter6);
        legionCounterList.Add(legionCounter6);
        legionCounterList.Add(legionCounter6);
        legionCounterList.Add(legionCounter6);
        legionScript.characterCounts = legionCounterList;
        legionScriptData.scriptInfo = legionScript;



        /*
        CustomScriptData twinDemonScriptData = new CustomScriptData();
        twinDemonScriptData.name = "TwinDemon_1";
        ScriptInfo twinDemonScript = new ScriptInfo();
        Il2CppSystem.Collections.Generic.List<CharacterData> twinDemonList = new Il2CppSystem.Collections.Generic.List<CharacterData>();
        twinDemonList.Add(w_twindemon);
        twinDemonScript.mustInclude = twinDemonList;
        twinDemonScript.startingDemons = twinDemonList;
        twinDemonScript.startingTownsfolks = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingTownsfolks;
        twinDemonScript.startingOutsiders = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingOutsiders;
        twinDemonScript.startingMinions = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingMinions;
        CharactersCount twinDemonCounter1 = new CharactersCount(7, 4, 1, 1, 1);
        twinDemonCounter1.dOuts = twinDemonCounter1.outs + 1;
        CharactersCount twinDemonCounter2 = new CharactersCount(8, 4, 1, 2, 1);
        twinDemonCounter2.dOuts = twinDemonCounter2.outs + 1;
        CharactersCount twinDemonCounter3 = new CharactersCount(9, 5, 1, 1, 2);
        twinDemonCounter3.dOuts = twinDemonCounter3.outs + 1;
        CharactersCount twinDemonCounter4 = new CharactersCount(10, 5, 1, 2, 2);
        twinDemonCounter4.dOuts = twinDemonCounter4.outs + 1;
        Il2CppSystem.Collections.Generic.List<CharactersCount> twinDemonCounterList = new Il2CppSystem.Collections.Generic.List<CharactersCount>();
        twinDemonCounterList.Add(twinDemonCounter1);
        twinDemonCounterList.Add(twinDemonCounter2);
        twinDemonCounterList.Add(twinDemonCounter3);
        twinDemonCounterList.Add(twinDemonCounter4);
        twinDemonScript.characterCounts = twinDemonCounterList;
        twinDemonScriptData.scriptInfo = twinDemonScript;
        */



        /*
        Il2CppSystem.Collections.Generic.List<CharacterData> illusionistJinxes = new Il2CppSystem.Collections.Generic.List<CharacterData>();
        CustomScriptData illusionistScriptData = new CustomScriptData();
        illusionistScriptData.name = "Illusionist_1";
        ScriptInfo illusionistScript = new ScriptInfo();
        Il2CppSystem.Collections.Generic.List<CharacterData> illusionistList = new Il2CppSystem.Collections.Generic.List<CharacterData>();
        illusionistList.Add(w_illusionist);
        illusionistScript.mustInclude = illusionistList;
        illusionistScript.startingDemons = illusionistList;
        illusionistScript.startingTownsfolks = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingTownsfolks;
        illusionistScript.startingOutsiders = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingOutsiders;
        illusionistScript.startingMinions = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingMinions;
        CharactersCount illusionistCounter2 = new CharactersCount(8, 4, 1, 2, 1);
        illusionistCounter2.dOuts = illusionistCounter2.outs + 1;
        CharactersCount illusionistCounter3 = new CharactersCount(9, 5, 1, 1, 2);
        illusionistCounter3.dOuts = illusionistCounter3.outs + 1;
        CharactersCount illusionistCounter4 = new CharactersCount(10, 5, 1, 2, 2);
        illusionistCounter4.dOuts = illusionistCounter4.outs + 1;
        Il2CppSystem.Collections.Generic.List<CharactersCount> illusionistCounterList = new Il2CppSystem.Collections.Generic.List<CharactersCount>();
        illusionistCounterList.Add(illusionistCounter2);
        illusionistCounterList.Add(illusionistCounter3);
        illusionistCounterList.Add(illusionistCounter4);
        illusionistScript.characterCounts = illusionistCounterList;
        illusionistScriptData.scriptInfo = illusionistScript;
        */


        /*
        Il2CppSystem.Collections.Generic.List<CharacterData> tripleDemonJinxes = new Il2CppSystem.Collections.Generic.List<CharacterData>();
        CustomScriptData tripleDemonScriptData = new CustomScriptData();
        tripleDemonScriptData.name = "TwinDemon_2";
        ScriptInfo tripleDemonScript = new ScriptInfo();
        Il2CppSystem.Collections.Generic.List<CharacterData> tripleDemonList = new Il2CppSystem.Collections.Generic.List<CharacterData>();
        tripleDemonList.Add(w_twindemon);
        tripleDemonList.Add(w_twindemontriplet);
        tripleDemonScript.mustInclude = tripleDemonList;
        tripleDemonScript.startingDemons = tripleDemonList;
        tripleDemonScript.startingTownsfolks = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingTownsfolks;
        tripleDemonScript.startingOutsiders = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingOutsiders;
        tripleDemonScript.startingMinions = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingMinions;
        JinxCharacter(legionScript.startingTownsfolks, "Bishop_58855542");
        JinxCharacter(legionScript.startingTownsfolks, "Empress_13782227");
        JinxCharacter(legionScript.startingTownsfolks, "Oracle_07039445");
        JinxCharacter(legionScript.startingTownsfolks, "Prince_TST");
        JinxCharacter(legionScript.startingMinions, "Swarm_Good_TST");
        CharactersCount tripleDemonCounter1 = new CharactersCount(7, 4, 2, 0, 1);
        tripleDemonCounter1.dOuts = tripleDemonCounter1.outs + 1;
        CharactersCount tripleDemonCounter2 = new CharactersCount(8, 4, 2, 1, 1);
        tripleDemonCounter2.dOuts = tripleDemonCounter2.outs + 1;
        CharactersCount tripleDemonCounter3 = new CharactersCount(9, 5, 2, 1, 1);
        tripleDemonCounter3.dOuts = tripleDemonCounter3.outs + 1;
        CharactersCount tripleDemonCounter4 = new CharactersCount(10, 5, 2, 1, 2);
        tripleDemonCounter4.dOuts = tripleDemonCounter4.outs + 1;
        Il2CppSystem.Collections.Generic.List<CharactersCount> tripleDemonCounterList = new Il2CppSystem.Collections.Generic.List<CharactersCount>();
        tripleDemonCounterList.Add(tripleDemonCounter1);
        tripleDemonCounterList.Add(tripleDemonCounter2);
        tripleDemonCounterList.Add(tripleDemonCounter3);
        tripleDemonCounterList.Add(tripleDemonCounter4);
        tripleDemonScript.characterCounts = tripleDemonCounterList;
        tripleDemonScriptData.scriptInfo = tripleDemonScript;
        */




        CustomScriptData caedoccidereScriptData = new CustomScriptData();
        caedoccidereScriptData.name = "Caedoccidere_1";
        ScriptInfo caedoccidereScript = new ScriptInfo();
        Il2CppSystem.Collections.Generic.List<CharacterData> caedoccidereList = new Il2CppSystem.Collections.Generic.List<CharacterData>();
        caedoccidereScript.mustInclude = caedoccidereList;
        caedoccidereScript.startingDemons = caedoccidereList;
        caedoccidereScript.startingTownsfolks = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingTownsfolks;
        caedoccidereScript.startingOutsiders = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingOutsiders;
        caedoccidereScript.startingMinions = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingMinions;
        CharactersCount caedoccidereCounter01 = new CharactersCount(7, 4, 1, 1, 1); // 4/1/1/1
        caedoccidereCounter01.dOuts = caedoccidereCounter01.outs + 1;
        CharactersCount caedoccidereCounter02 = new CharactersCount(7, 4, 1, 2, 0); // 4/2/0/1
        caedoccidereCounter02.dOuts = caedoccidereCounter02.outs + 1;
        CharactersCount caedoccidereCounter03 = new CharactersCount(8, 4, 1, 3, 0); // 4/3/0/1
        caedoccidereCounter03.dOuts = caedoccidereCounter03.outs + 1;
        CharactersCount caedoccidereCounter04 = new CharactersCount(8, 4, 1, 2, 1); // 4/2/1/1
        caedoccidereCounter04.dOuts = caedoccidereCounter04.outs + 1;
        CharactersCount caedoccidereCounter05 = new CharactersCount(8, 4, 1, 1, 2); // 4/1/2/1
        caedoccidereCounter05.dOuts = caedoccidereCounter05.outs + 1;
        CharactersCount caedoccidereCounter06 = new CharactersCount(9, 5, 1, 3, 0); // 5/3/0/1
        caedoccidereCounter06.dOuts = caedoccidereCounter06.outs + 1;
        CharactersCount caedoccidereCounter07 = new CharactersCount(9, 5, 1, 2, 1); // 5/2/1/1
        caedoccidereCounter07.dOuts = caedoccidereCounter07.outs + 1;
        CharactersCount caedoccidereCounter08 = new CharactersCount(9, 5, 1, 1, 2); // 5/1/2/1
        caedoccidereCounter08.dOuts = caedoccidereCounter08.outs + 1;
        CharactersCount caedoccidereCounter09 = new CharactersCount(10, 5, 1, 1, 3); // 5/1/3/1
        caedoccidereCounter09.dOuts = caedoccidereCounter09.outs + 1;
        CharactersCount caedoccidereCounter10 = new CharactersCount(10, 5, 1, 2, 2); // 5/2/2/1
        caedoccidereCounter10.dOuts = caedoccidereCounter10.outs + 1;
        CharactersCount caedoccidereCounter11 = new CharactersCount(10, 5, 1, 3, 1); // 5/3/1/1
        caedoccidereCounter11.dOuts = caedoccidereCounter11.outs + 1;
        CharactersCount caedoccidereCounter12 = new CharactersCount(10, 5, 1, 4, 0); // 5/4/0/1
        caedoccidereCounter12.dOuts = caedoccidereCounter12.outs + 1;
        Il2CppSystem.Collections.Generic.List<CharactersCount> caedoccidereCounterList = new Il2CppSystem.Collections.Generic.List<CharactersCount>();
        // 7 cards
        caedoccidereCounterList.Add(caedoccidereCounter01); // 4/1/1/1
        caedoccidereCounterList.Add(caedoccidereCounter01); // 4/1/1/1
        caedoccidereCounterList.Add(caedoccidereCounter02); // 4/2/0/1

        // 8 cards
        caedoccidereCounterList.Add(caedoccidereCounter03); // 4/3/0/1
        caedoccidereCounterList.Add(caedoccidereCounter04); // 4/2/1/1
        caedoccidereCounterList.Add(caedoccidereCounter04); // 4/2/1/1
        caedoccidereCounterList.Add(caedoccidereCounter05); // 4/1/2/1
        caedoccidereCounterList.Add(caedoccidereCounter05); // 4/1/2/1
        caedoccidereCounterList.Add(caedoccidereCounter05); // 4/1/2/1

        // 9 cards
        caedoccidereCounterList.Add(caedoccidereCounter06); // 5/3/0/1
        caedoccidereCounterList.Add(caedoccidereCounter07); // 5/2/1/1
        caedoccidereCounterList.Add(caedoccidereCounter07); // 5/2/1/1
        caedoccidereCounterList.Add(caedoccidereCounter08); // 5/1/2/1
        caedoccidereCounterList.Add(caedoccidereCounter08); // 5/1/2/1
        caedoccidereCounterList.Add(caedoccidereCounter08); // 5/1/2/1

        // 10 cards
        caedoccidereCounterList.Add(caedoccidereCounter09); // 5/1/3/1
        caedoccidereCounterList.Add(caedoccidereCounter09); // 5/1/3/1
        caedoccidereCounterList.Add(caedoccidereCounter09); // 5/1/3/1
        caedoccidereCounterList.Add(caedoccidereCounter09); // 5/1/3/1
        caedoccidereCounterList.Add(caedoccidereCounter10); // 5/2/2/1
        caedoccidereCounterList.Add(caedoccidereCounter10); // 5/2/2/1
        caedoccidereCounterList.Add(caedoccidereCounter10); // 5/2/2/1
        caedoccidereCounterList.Add(caedoccidereCounter11); // 5/3/1/1
        caedoccidereCounterList.Add(caedoccidereCounter11); // 5/3/1/1
        caedoccidereCounterList.Add(caedoccidereCounter12); // 5/4/0/1
        caedoccidereScript.characterCounts = caedoccidereCounterList;
        caedoccidereScriptData.scriptInfo = caedoccidereScript;




        CustomScriptData praesectScriptData = new CustomScriptData();
        praesectScriptData.name = "Praesect_1";
        ScriptInfo praesectScript = new ScriptInfo();
        Il2CppSystem.Collections.Generic.List<CharacterData> praesectList = new Il2CppSystem.Collections.Generic.List<CharacterData>();
        praesectScript.mustInclude = praesectList;
        praesectScript.startingDemons = praesectList;
        praesectScript.startingTownsfolks = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingTownsfolks;
        praesectScript.startingOutsiders = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingOutsiders;
        praesectScript.startingMinions = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingMinions;
        JinxCharacter(legionScript.startingTownsfolks, "Spy_TST");
        JinxCharacter(legionScript.startingMinions, "Swarm_Good_TST");
        CharactersCount praesectCounter2 = new CharactersCount(8, 5, 1, 2, 0);
        praesectCounter2.dOuts = praesectCounter2.outs + 1;
        CharactersCount praesectCounter3 = new CharactersCount(9, 6, 1, 1, 1);
        praesectCounter3.dOuts = praesectCounter3.outs + 1;
        CharactersCount praesectCounter4 = new CharactersCount(10, 6, 1, 2, 1);
        praesectCounter4.dOuts = praesectCounter4.outs + 1;
        Il2CppSystem.Collections.Generic.List<CharactersCount> praesectCounterList = new Il2CppSystem.Collections.Generic.List<CharactersCount>();
        praesectCounterList.Add(praesectCounter2);
        praesectCounterList.Add(praesectCounter3);
        praesectCounterList.Add(praesectCounter4);
        praesectScript.characterCounts = praesectCounterList;
        praesectScriptData.scriptInfo = praesectScript;




        CustomScriptData mendaverteScriptData = new CustomScriptData();
        mendaverteScriptData.name = "Mendaverte_1";
        ScriptInfo mendaverteScript = new ScriptInfo();
        Il2CppSystem.Collections.Generic.List<CharacterData> mendaverteList = new Il2CppSystem.Collections.Generic.List<CharacterData>();
        mendaverteScript.mustInclude = mendaverteList;
        mendaverteScript.startingDemons = mendaverteList;
        mendaverteScript.startingTownsfolks = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingTownsfolks;
        mendaverteScript.startingOutsiders = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingOutsiders;
        mendaverteScript.startingMinions = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingMinions;
        JinxCharacter(legionScript.startingTownsfolks, "Alchemist_94446803");
        JinxCharacter(legionScript.startingMinions, "Turncoat_TST");
        CharactersCount mendaverteCounter2 = new CharactersCount(8, 5, 1, 1, 1);
        mendaverteCounter2.dOuts = mendaverteCounter2.outs + 1;
        CharactersCount mendaverteCounter3 = new CharactersCount(9, 5, 1, 2, 1);
        mendaverteCounter3.dOuts = mendaverteCounter3.outs + 1;
        CharactersCount mendaverteCounter4 = new CharactersCount(10, 6, 1, 1, 2);
        mendaverteCounter4.dOuts = mendaverteCounter4.outs + 1;
        CharactersCount mendaverteCounter5 = new CharactersCount(10, 5, 1, 2, 2);
        mendaverteCounter5.dOuts = mendaverteCounter5.outs + 1;
        Il2CppSystem.Collections.Generic.List<CharactersCount> mendaverteCounterList = new Il2CppSystem.Collections.Generic.List<CharactersCount>();
        mendaverteCounterList.Add(mendaverteCounter2);
        mendaverteCounterList.Add(mendaverteCounter3);
        mendaverteCounterList.Add(mendaverteCounter4);
        mendaverteCounterList.Add(mendaverteCounter5);
        mendaverteScript.characterCounts = mendaverteCounterList;
        mendaverteScriptData.scriptInfo = mendaverteScript;




        CustomScriptData venelumScriptData = new CustomScriptData();
        venelumScriptData.name = "Venelum_1";
        ScriptInfo venelumScript = new ScriptInfo();
        Il2CppSystem.Collections.Generic.List<CharacterData> venelumList = new Il2CppSystem.Collections.Generic.List<CharacterData>();
        venelumScript.mustInclude = venelumList;
        venelumScript.startingDemons = venelumList;
        venelumScript.startingTownsfolks = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingTownsfolks;
        venelumScript.startingOutsiders = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingOutsiders;
        venelumScript.startingMinions = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingMinions;
        CharactersCount venelumCounter1 = new CharactersCount(8, 5, 1, 1, 1);
        venelumCounter1.dOuts = venelumCounter1.outs + 1;
        CharactersCount venelumCounter2 = new CharactersCount(9, 5, 1, 2, 1);
        venelumCounter2.dOuts = venelumCounter2.outs + 1;
        CharactersCount venelumCounter3 = new CharactersCount(9, 5, 1, 1, 2);
        venelumCounter3.dOuts = venelumCounter3.outs + 1;
        CharactersCount venelumCounter4 = new CharactersCount(10, 6, 1, 1, 2);
        venelumCounter4.dOuts = venelumCounter4.outs + 1;
        CharactersCount venelumCounter5 = new CharactersCount(10, 5, 1, 2, 2);
        venelumCounter5.dOuts = venelumCounter5.outs + 1;
        Il2CppSystem.Collections.Generic.List<CharactersCount> venelumCounterList = new Il2CppSystem.Collections.Generic.List<CharactersCount>();
        venelumCounterList.Add(venelumCounter1);
        venelumCounterList.Add(venelumCounter2);
        venelumCounterList.Add(venelumCounter3);
        venelumCounterList.Add(venelumCounter4);
        venelumCounterList.Add(venelumCounter5);
        venelumScript.characterCounts = venelumCounterList;
        venelumScriptData.scriptInfo = venelumScript;



        /*
        CustomScriptData shardScriptData = new CustomScriptData();
        shardScriptData.name = "Shard_1";
        ScriptInfo shardScript = new ScriptInfo();
        Il2CppSystem.Collections.Generic.List<CharacterData> shardList = new Il2CppSystem.Collections.Generic.List<CharacterData>();
        shardList.Add(w_shard);
        shardScript.mustInclude = shardList;
        shardScript.startingDemons = shardList;
        shardScript.startingTownsfolks = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingTownsfolks;
        shardScript.startingTownsfolks.Remove(w_prince);
        shardScript.startingOutsiders = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingOutsiders;
        shardScript.startingMinions = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingMinions;
        shardScript.startingMinions.Remove(w_undying);
        CharactersCount shardCounter1 = new CharactersCount(7, 5, 1, 0, 1);
        shardCounter1.dTown = 10;
        shardCounter1.dOuts = 4;
        CharactersCount shardCounter2 = new CharactersCount(7, 4, 1, 1, 1);
        shardCounter2.dTown = 10;
        shardCounter2.dOuts = 4;
        CharactersCount shardCounter3 = new CharactersCount(8, 5, 1, 1, 1);
        shardCounter3.dTown = 11;
        shardCounter3.dOuts = 5;
        CharactersCount shardCounter4 = new CharactersCount(8, 4, 1, 2, 1);
        shardCounter4.dTown = 11;
        shardCounter4.dOuts = 5;
        CharactersCount shardCounter5 = new CharactersCount(8, 4, 1, 1, 2);
        shardCounter5.dTown = 11;
        shardCounter5.dOuts = 5;
        CharactersCount shardCounter6 = new CharactersCount(8, 5, 1, 0, 2);
        shardCounter6.dTown = 11;
        shardCounter6.dOuts = 5;
        CharactersCount shardCounter7 = new CharactersCount(9, 5, 1, 1, 2);
        shardCounter7.dTown = 12;
        shardCounter7.dOuts = 6;
        CharactersCount shardCounter8 = new CharactersCount(9, 5, 1, 2, 1);
        shardCounter8.dTown = 12;
        shardCounter8.dOuts = 6;
        CharactersCount shardCounter9 = new CharactersCount(10, 5, 1, 2, 2);
        shardCounter9.dTown = 13;
        shardCounter9.dOuts = 7;
        CharactersCount shardCounter10 = new CharactersCount(10, 6, 1, 0, 3);
        shardCounter10.dTown = 13;
        shardCounter10.dOuts = 7;
        Il2CppSystem.Collections.Generic.List<CharactersCount> shardCounterList = new Il2CppSystem.Collections.Generic.List<CharactersCount>();
        shardCounterList.Add(shardCounter1);
        shardCounterList.Add(shardCounter2);
        shardCounterList.Add(shardCounter3);
        shardCounterList.Add(shardCounter4);
        shardCounterList.Add(shardCounter5);
        shardCounterList.Add(shardCounter6);
        shardCounterList.Add(shardCounter7);
        shardCounterList.Add(shardCounter8);
        shardCounterList.Add(shardCounter9);
        shardCounterList.Add(shardCounter10);
        shardScript.characterCounts = shardCounterList;
        shardScriptData.scriptInfo = shardScript
        */



        CustomScriptData minosScriptData = new CustomScriptData();
        minosScriptData.name = "Minos_1";
        ScriptInfo minosScript = new ScriptInfo();
        Il2CppSystem.Collections.Generic.List<CharacterData> minosList = new Il2CppSystem.Collections.Generic.List<CharacterData>();
        minosScript.mustInclude = minosList;
        minosScript.startingDemons = minosList;
        minosScript.startingTownsfolks = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingTownsfolks;
        minosScript.startingOutsiders = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingOutsiders;
        minosScript.startingMinions = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingMinions;
        // 7 characters
        CharactersCount minosCounter01 = setCharacterCount(3, 3, 0, 1);
        CharactersCount minosCounter02 = setCharacterCount(3, 2, 1, 1);
        CharactersCount minosCounter03 = setCharacterCount(4, 1, 1, 1);
        CharactersCount minosCounter04 = setCharacterCount(4, 0, 2, 1);
        // 8 characters
        CharactersCount minosCounter05 = setCharacterCount(3, 4, 0, 1);
        CharactersCount minosCounter06 = setCharacterCount(3, 3, 1, 1);
        CharactersCount minosCounter07 = setCharacterCount(4, 2, 1, 1);
        CharactersCount minosCounter08 = setCharacterCount(4, 1, 2, 1);
        CharactersCount minosCounter09 = setCharacterCount(4, 0, 3, 1);
        // 9 characters
        CharactersCount minosCounter10 = setCharacterCount(4, 4, 0, 1);
        CharactersCount minosCounter11 = setCharacterCount(4, 3, 1, 1);
        CharactersCount minosCounter12 = setCharacterCount(4, 2, 2, 1);
        CharactersCount minosCounter13 = setCharacterCount(4, 1, 3, 1);
        // 10 characters
        CharactersCount minosCounter14 = setCharacterCount(4, 5, 0, 1);
        CharactersCount minosCounter15 = setCharacterCount(4, 4, 1, 1);
        CharactersCount minosCounter16 = setCharacterCount(4, 3, 2, 1);
        CharactersCount minosCounter17 = setCharacterCount(5, 2, 2, 1);
        CharactersCount minosCounter18 = setCharacterCount(5, 1, 3, 1);
        Il2CppSystem.Collections.Generic.List<CharactersCount> minosCounterList = new Il2CppSystem.Collections.Generic.List<CharactersCount>();


        // 7 characters
        minosCounterList.Add(minosCounter01); // 3/3/0/1

        minosCounterList.Add(minosCounter02); // 3/2/1/1
        minosCounterList.Add(minosCounter02); // 3/2/1/1
        minosCounterList.Add(minosCounter02); // 3/2/1/1

        minosCounterList.Add(minosCounter03); // 4/1/1/1
        minosCounterList.Add(minosCounter03); // 4/1/1/1
        minosCounterList.Add(minosCounter03); // 4/1/1/1
        minosCounterList.Add(minosCounter03); // 4/1/1/1
        minosCounterList.Add(minosCounter03); // 4/1/1/1

        minosCounterList.Add(minosCounter04); // 4/0/2/1
        minosCounterList.Add(minosCounter04); // 4/0/2/1
        minosCounterList.Add(minosCounter04); // 4/0/2/1
        minosCounterList.Add(minosCounter04); // 4/0/2/1
        minosCounterList.Add(minosCounter04); // 4/0/2/1
        minosCounterList.Add(minosCounter04); // 4/0/2/1
        minosCounterList.Add(minosCounter04); // 4/0/2/1


        // 8 characters
        minosCounterList.Add(minosCounter05); // 3/4/0/1

        minosCounterList.Add(minosCounter06); // 3/3/1/1
        minosCounterList.Add(minosCounter06); // 3/3/1/1

        minosCounterList.Add(minosCounter07); // 4/2/1/1
        minosCounterList.Add(minosCounter07); // 4/2/1/1
        minosCounterList.Add(minosCounter07); // 4/2/1/1

        minosCounterList.Add(minosCounter08); // 4/1/2/1
        minosCounterList.Add(minosCounter08); // 4/1/2/1
        minosCounterList.Add(minosCounter08); // 4/1/2/1
        minosCounterList.Add(minosCounter08); // 4/1/2/1

        minosCounterList.Add(minosCounter09); // 4/0/3/1
        minosCounterList.Add(minosCounter09); // 4/0/3/1
        minosCounterList.Add(minosCounter09); // 4/0/3/1
        minosCounterList.Add(minosCounter09); // 4/0/3/1
        minosCounterList.Add(minosCounter09); // 4/0/3/1


        // 9 characters
        minosCounterList.Add(minosCounter10); // 4/4/0/1

        minosCounterList.Add(minosCounter11); // 4/3/1/1
        minosCounterList.Add(minosCounter11); // 4/3/1/1
        minosCounterList.Add(minosCounter11); // 4/3/1/1
        minosCounterList.Add(minosCounter11); // 4/3/1/1

        minosCounterList.Add(minosCounter12); // 4/2/2/1
        minosCounterList.Add(minosCounter12); // 4/2/2/1
        minosCounterList.Add(minosCounter12); // 4/2/2/1
        minosCounterList.Add(minosCounter12); // 4/2/2/1
        minosCounterList.Add(minosCounter12); // 4/2/2/1
        minosCounterList.Add(minosCounter12); // 4/2/2/1
        minosCounterList.Add(minosCounter12); // 4/2/2/1

        minosCounterList.Add(minosCounter13); // 4/1/3/1
        minosCounterList.Add(minosCounter13); // 4/1/3/1
        minosCounterList.Add(minosCounter13); // 4/1/3/1
        minosCounterList.Add(minosCounter13); // 4/1/3/1
        minosCounterList.Add(minosCounter13); // 4/1/3/1


        // 10 characters
        minosCounterList.Add(minosCounter14); // 4/5/0/1

        minosCounterList.Add(minosCounter15); // 4/4/1/1
        minosCounterList.Add(minosCounter15); // 4/4/1/1
        minosCounterList.Add(minosCounter15); // 4/4/1/1
        minosCounterList.Add(minosCounter15); // 4/4/1/1
        minosCounterList.Add(minosCounter15); // 4/4/1/1

        minosCounterList.Add(minosCounter16); // 4/3/2/1
        minosCounterList.Add(minosCounter16); // 4/3/2/1
        minosCounterList.Add(minosCounter16); // 4/3/2/1
        minosCounterList.Add(minosCounter16); // 4/3/2/1
        minosCounterList.Add(minosCounter16); // 4/3/2/1
        minosCounterList.Add(minosCounter16); // 4/3/2/1
        minosCounterList.Add(minosCounter16); // 4/3/2/1
        minosCounterList.Add(minosCounter16); // 4/3/2/1
        minosCounterList.Add(minosCounter16); // 4/3/2/1
        minosCounterList.Add(minosCounter16); // 4/3/2/1
        minosCounterList.Add(minosCounter16); // 4/3/2/1

        minosCounterList.Add(minosCounter17); // 5/2/2/1
        minosCounterList.Add(minosCounter17); // 5/2/2/1
        minosCounterList.Add(minosCounter17); // 5/2/2/1
        minosCounterList.Add(minosCounter17); // 5/2/2/1
        minosCounterList.Add(minosCounter17); // 5/2/2/1
        minosCounterList.Add(minosCounter17); // 5/2/2/1
        minosCounterList.Add(minosCounter17); // 5/2/2/1
        minosCounterList.Add(minosCounter17); // 5/2/2/1
        minosCounterList.Add(minosCounter17); // 5/2/2/1
        minosCounterList.Add(minosCounter17); // 5/2/2/1
        minosCounterList.Add(minosCounter17); // 5/2/2/1
        minosCounterList.Add(minosCounter17); // 5/2/2/1
        minosCounterList.Add(minosCounter17); // 5/2/2/1
        minosCounterList.Add(minosCounter17); // 5/2/2/1
        minosCounterList.Add(minosCounter17); // 5/2/2/1
        minosCounterList.Add(minosCounter17); // 5/2/2/1

        minosCounterList.Add(minosCounter18); // 5/1/3/1
        minosCounterList.Add(minosCounter18); // 5/1/3/1
        minosCounterList.Add(minosCounter18); // 5/1/3/1
        minosCounterList.Add(minosCounter18); // 5/1/3/1
        minosCounterList.Add(minosCounter18); // 5/1/3/1
        minosCounterList.Add(minosCounter18); // 5/1/3/1
        minosCounterList.Add(minosCounter18); // 5/1/3/1
        minosCounterList.Add(minosCounter18); // 5/1/3/1
        minosCounterList.Add(minosCounter18); // 5/1/3/1
        minosCounterList.Add(minosCounter18); // 5/1/3/1
        minosCounterList.Add(minosCounter18); // 5/1/3/1
        minosCounterList.Add(minosCounter18); // 5/1/3/1



        /*
        Layouts:
        3/3/0/1: 1%
        3/2/1/1: 4%
        4/1/1/1: 6%
        4/0/2/1: 6%
        3/4/0/1: 1%
        3/3/1/1: 2%
        4/2/1/1: 4%
        4/1/2/1: 5%
        4/0/3/1: 4%
        4/4/0/1: 1%
        4/3/1/1: 5%
        4/2/2/1: 9%
        4/1/3/1: 4%
        4/5/0/1: 1%
        4/4/1/1: 6%
        4/3/2/1: 13%
        5/2/2/1: 20%
        5/1/3/1: 9%
        */


        minosScript.characterCounts = minosCounterList;
        minosScriptData.scriptInfo = minosScript;





        /*
        CustomScriptData pandemoniumScriptData = new CustomScriptData();
        pandemoniumScriptData.name = "Pandemonium_1";
        ScriptInfo pandemoniumScript = new ScriptInfo();
        Il2CppSystem.Collections.Generic.List<CharacterData> pandemoniumList = new Il2CppSystem.Collections.Generic.List<CharacterData>();
        pandemoniumList.Add(w_pandemonium);
        pandemoniumScript.mustInclude = pandemoniumList;
        pandemoniumScript.startingDemons = pandemoniumList;
        pandemoniumScript.startingTownsfolks = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingTownsfolks;
        pandemoniumScript.startingOutsiders = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingOutsiders;
        pandemoniumScript.startingMinions = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingMinions;
        CharactersCount pandemoniumCounter01 = setCharacterCount(7, 1, 2, 1); // 11
        CharactersCount pandemoniumCounter02 = setCharacterCount(7, 2, 2, 1); // 12
        CharactersCount pandemoniumCounter03 = setCharacterCount(9, 0, 3, 1); // 13
        CharactersCount pandemoniumCounter04 = setCharacterCount(9, 1, 3, 1); // 14
        CharactersCount pandemoniumCounter05 = setCharacterCount(9, 2, 3, 1); // 15
        Il2CppSystem.Collections.Generic.List<CharactersCount> pandemoniumCounterList = new Il2CppSystem.Collections.Generic.List<CharactersCount>();


        pandemoniumCounterList.Add(pandemoniumCounter01);
        pandemoniumCounterList.Add(pandemoniumCounter02);
        pandemoniumCounterList.Add(pandemoniumCounter02);
        pandemoniumCounterList.Add(pandemoniumCounter03);
        pandemoniumCounterList.Add(pandemoniumCounter03);
        pandemoniumCounterList.Add(pandemoniumCounter03);
        pandemoniumCounterList.Add(pandemoniumCounter04);
        pandemoniumCounterList.Add(pandemoniumCounter04);
        pandemoniumCounterList.Add(pandemoniumCounter04);
        pandemoniumCounterList.Add(pandemoniumCounter04);
        pandemoniumCounterList.Add(pandemoniumCounter05);
        pandemoniumCounterList.Add(pandemoniumCounter05);
        pandemoniumCounterList.Add(pandemoniumCounter05);
        pandemoniumCounterList.Add(pandemoniumCounter05);
        pandemoniumCounterList.Add(pandemoniumCounter05);


        pandemoniumScript.characterCounts = pandemoniumCounterList;
        pandemoniumScriptData.scriptInfo = pandemoniumScript;
        */





        CustomScriptData tenecaligoScriptData = new CustomScriptData();
        tenecaligoScriptData.name = "Tenecaligo_1";
        ScriptInfo tenecaligoScript = new ScriptInfo();
        Il2CppSystem.Collections.Generic.List<CharacterData> tenecaligoList = new Il2CppSystem.Collections.Generic.List<CharacterData>();
        tenecaligoScript.mustInclude = tenecaligoList;
        tenecaligoScript.startingDemons = tenecaligoList;
        tenecaligoScript.startingTownsfolks = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingTownsfolks;
        tenecaligoScript.startingOutsiders = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingOutsiders;
        tenecaligoScript.startingMinions = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingMinions;
        CharactersCount tenecaligoCounter01 = setCharacterCount(9, 0, 0, 1); // Always have 10 characters. Final ratio = 5 Villagers, 0-4 Outcasts, 0-4 Minions, 1 Demon.
        tenecaligoCounter01.dOuts = 0;
        Il2CppSystem.Collections.Generic.List<CharactersCount> tenecaligoCounterList = new Il2CppSystem.Collections.Generic.List<CharactersCount>();


        tenecaligoCounterList.Add(tenecaligoCounter01);


        tenecaligoScript.characterCounts = tenecaligoCounterList;
        tenecaligoScriptData.scriptInfo = tenecaligoScript;





        CustomScriptData leviathanScriptData = new CustomScriptData();
        leviathanScriptData.name = "Leviathan_1";
        ScriptInfo leviathanScript = new ScriptInfo();
        Il2CppSystem.Collections.Generic.List<CharacterData> leviathanList = new Il2CppSystem.Collections.Generic.List<CharacterData>();
        leviathanScript.mustInclude = leviathanList;
        leviathanScript.startingDemons = leviathanList;
        leviathanScript.startingTownsfolks = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingTownsfolks;
        leviathanScript.startingOutsiders = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingOutsiders;
        leviathanScript.startingMinions = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingMinions;
        CharactersCount leviathanCounter01 = setCharacterCount(1, 4, 1, 1); // 7
        leviathanCounter01.dOuts = 6;
        leviathanCounter01.dMinion = 3;
        CharactersCount leviathanCounter02 = setCharacterCount(2, 4, 1, 1); // 8
        leviathanCounter02.dOuts = 6;
        leviathanCounter02.dMinion = 3;
        CharactersCount leviathanCounter03 = setCharacterCount(2, 4, 2, 1); // 9
        leviathanCounter03.dOuts = 6;
        leviathanCounter03.dMinion = 4;
        CharactersCount leviathanCounter04 = setCharacterCount(2, 5, 2, 1); // 10
        leviathanCounter04.dOuts = 7;
        leviathanCounter04.dMinion = 4;
        CharactersCount leviathanCounter05 = setCharacterCount(3, 5, 1, 1); // 10
        leviathanCounter05.dOuts = 7;
        leviathanCounter05.dMinion = 3;
        Il2CppSystem.Collections.Generic.List<CharactersCount> leviathanCounterList = new Il2CppSystem.Collections.Generic.List<CharactersCount>();


        leviathanCounterList.Add(leviathanCounter01);
        for (int i = 0; i < 3; i++)
        {
            leviathanCounterList.Add(leviathanCounter02);
        }
        for (int i = 0; i < 9; i++)
        {
            leviathanCounterList.Add(leviathanCounter03);
        }
        for (int i = 0; i < (27 / 2); i++)
        {
            leviathanCounterList.Add(leviathanCounter04);
            leviathanCounterList.Add(leviathanCounter05);
        }


        leviathanScript.characterCounts = leviathanCounterList;
        leviathanScriptData.scriptInfo = leviathanScript;






        CustomScriptData twinDemonScriptData = new CustomScriptData();
        twinDemonScriptData.name = "TwinDemon_1";
        ScriptInfo twinDemonScript = new ScriptInfo();
        Il2CppSystem.Collections.Generic.List<CharacterData> twinDemonList = new Il2CppSystem.Collections.Generic.List<CharacterData>();
        twinDemonScript.startingDemons = twinDemonList;
        twinDemonScript.startingTownsfolks = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingTownsfolks;
        twinDemonScript.startingOutsiders = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingOutsiders;
        twinDemonScript.startingMinions = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingMinions;
        // 6 characters
        CharactersCount twinDemonCounter_06_4101 = setCharacterCount(4, 1, 0, 1);
        CharactersCount twinDemonCounter_06_4011 = setCharacterCount(4, 0, 1, 1);
        CharactersCount twinDemonCounter_06_4002 = setCharacterCount(4, 0, 0, 2);

        CharactersCount twinDemonCounter_07_4111 = setCharacterCount(4, 1, 1, 1);
        CharactersCount twinDemonCounter_07_4201 = setCharacterCount(4, 2, 0, 1);
        CharactersCount twinDemonCounter_07_4021 = setCharacterCount(4, 0, 2, 1);
        CharactersCount twinDemonCounter_07_4102 = setCharacterCount(4, 1, 0, 2);
        CharactersCount twinDemonCounter_07_4012 = setCharacterCount(4, 0, 1, 2);

        CharactersCount twinDemonCounter_08_4202 = setCharacterCount(4, 2, 0, 2);
        CharactersCount twinDemonCounter_08_4112 = setCharacterCount(4, 1, 1, 2);
        CharactersCount twinDemonCounter_08_4103 = setCharacterCount(4, 1, 0, 3);

        CharactersCount twinDemonCounter_09_4212 = setCharacterCount(4, 2, 1, 2);
        CharactersCount twinDemonCounter_09_5022 = setCharacterCount(5, 0, 2, 2);
        CharactersCount twinDemonCounter_09_4203 = setCharacterCount(4, 2, 0, 3);
        CharactersCount twinDemonCounter_09_5013 = setCharacterCount(5, 0, 1, 3);

        CharactersCount twinDemonCounter_10_4312 = setCharacterCount(4, 3, 1, 2);
        CharactersCount twinDemonCounter_10_5122 = setCharacterCount(5, 1, 2, 2);
        CharactersCount twinDemonCounter_10_5113 = setCharacterCount(5, 1, 1, 3);

        twinDemonCounter_06_4101.dDemon = 3;
        twinDemonCounter_06_4011.dDemon = 3;
        twinDemonCounter_06_4002.dDemon = 3;
        twinDemonCounter_07_4111.dDemon = 3;
        twinDemonCounter_07_4201.dDemon = 3;
        twinDemonCounter_07_4021.dDemon = 3;
        twinDemonCounter_07_4102.dDemon = 3;
        twinDemonCounter_07_4012.dDemon = 3;
        twinDemonCounter_08_4202.dDemon = 3;
        twinDemonCounter_08_4112.dDemon = 3;
        twinDemonCounter_08_4103.dDemon = 3;
        twinDemonCounter_09_4212.dDemon = 3;
        twinDemonCounter_09_5022.dDemon = 3;
        twinDemonCounter_09_4203.dDemon = 3;
        twinDemonCounter_09_5013.dDemon = 3;
        twinDemonCounter_10_4312.dDemon = 3;
        twinDemonCounter_10_5122.dDemon = 3;
        twinDemonCounter_10_5113.dDemon = 3;



        Il2CppSystem.Collections.Generic.List<CharactersCount> twinDemonCounterList = new Il2CppSystem.Collections.Generic.List<CharactersCount>();

        // 6-character villages will be fairly rare - 5 lots total, 7% chance.
        twinDemonCounterList = addCharacterCount(twinDemonCounter_06_4101, twinDemonCounterList, 1); // 20%
        twinDemonCounterList = addCharacterCount(twinDemonCounter_06_4011, twinDemonCounterList, 1); // 20%
        twinDemonCounterList = addCharacterCount(twinDemonCounter_06_4002, twinDemonCounterList, 3); // 60%

        // 7-characters will be fairly uncommon, 10 lots total for a 13% chance.
        twinDemonCounterList = addCharacterCount(twinDemonCounter_07_4111, twinDemonCounterList, 2); // 20%
        twinDemonCounterList = addCharacterCount(twinDemonCounter_07_4201, twinDemonCounterList, 1); // 10%
        twinDemonCounterList = addCharacterCount(twinDemonCounter_07_4021, twinDemonCounterList, 1); // 10%
        twinDemonCounterList = addCharacterCount(twinDemonCounter_07_4102, twinDemonCounterList, 3); // 30%
        twinDemonCounterList = addCharacterCount(twinDemonCounter_07_4012, twinDemonCounterList, 3); // 30%

        // 8-characters will have a 20% chance by having 15 lots.
        twinDemonCounterList = addCharacterCount(twinDemonCounter_08_4202, twinDemonCounterList, 5); // 33%
        twinDemonCounterList = addCharacterCount(twinDemonCounter_08_4112, twinDemonCounterList, 7); // 47%
        twinDemonCounterList = addCharacterCount(twinDemonCounter_08_4103, twinDemonCounterList, 3); // 20%

        // 9-characters will have a 27% chance by having 20 lots
        twinDemonCounterList = addCharacterCount(twinDemonCounter_09_4212, twinDemonCounterList, 7); // 35%
        twinDemonCounterList = addCharacterCount(twinDemonCounter_09_5022, twinDemonCounterList, 7); // 35%
        twinDemonCounterList = addCharacterCount(twinDemonCounter_09_4203, twinDemonCounterList, 3); // 15%
        twinDemonCounterList = addCharacterCount(twinDemonCounter_09_5013, twinDemonCounterList, 3); // 15%

        // 10-characters will have a 33% chance by having 25 lots.
        twinDemonCounterList = addCharacterCount(twinDemonCounter_10_4312, twinDemonCounterList, 10); // 40%
        twinDemonCounterList = addCharacterCount(twinDemonCounter_10_5122, twinDemonCounterList, 10); // 40%
        twinDemonCounterList = addCharacterCount(twinDemonCounter_10_5113, twinDemonCounterList, 5);  // 20%

        twinDemonScript.characterCounts = twinDemonCounterList;
        twinDemonScriptData.scriptInfo = twinDemonScript;



        CustomScriptData irisScriptData = new CustomScriptData();
        irisScriptData.name = "Iris_1";
        ScriptInfo irisScript = new ScriptInfo();
        Il2CppSystem.Collections.Generic.List<CharacterData> irisList = new Il2CppSystem.Collections.Generic.List<CharacterData>();
        irisScript.mustInclude = irisList;
        irisScript.startingDemons = irisList;
        irisScript.startingTownsfolks = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingTownsfolks;
        irisScript.startingOutsiders = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingOutsiders;
        irisScript.startingMinions = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingMinions;
        CharactersCount irisCounter01 = setCharacterCount(5, 2, 1, 1); // 9 characters
        CharactersCount irisCounter02 = setCharacterCount(4, 1, 1, 1); // 7 characters
        CharactersCount irisCounter03 = setCharacterCount(6, 0, 2, 1); // 9 characters
        CharactersCount irisCounter04 = setCharacterCount(4, 1, 0, 1); // 6 characters
        CharactersCount irisCounter05 = setCharacterCount(4, 2, 1, 1); // 8 characters
        CharactersCount irisCounter06 = setCharacterCount(6, 1, 2, 1); // 10 characters
        Il2CppSystem.Collections.Generic.List<CharactersCount> irisCounterList = new Il2CppSystem.Collections.Generic.List<CharactersCount>();


        irisCounterList.Add(irisCounter01);
        irisCounterList.Add(irisCounter02);
        irisCounterList.Add(irisCounter02);
        irisCounterList.Add(irisCounter03);
        irisCounterList.Add(irisCounter04);
        irisCounterList.Add(irisCounter04);
        irisCounterList.Add(irisCounter05);
        irisCounterList.Add(irisCounter05);
        irisCounterList.Add(irisCounter06);
        irisCounterList.Add(irisCounter06);


        irisScript.characterCounts = irisCounterList;
        irisScriptData.scriptInfo = irisScript;



        CustomScriptData carniScriptData = new CustomScriptData();
        carniScriptData.name = "Carnicarius_1";
        ScriptInfo carniScript = new ScriptInfo();
        Il2CppSystem.Collections.Generic.List<CharacterData> carniList = new Il2CppSystem.Collections.Generic.List<CharacterData>();
        carniScript.mustInclude = carniList;
        carniScript.startingDemons = carniList;
        carniScript.startingTownsfolks = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingTownsfolks;
        carniScript.startingOutsiders = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingOutsiders;
        carniScript.startingMinions = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingMinions;
        CharactersCount carniCounter01 = setCharacterCount(4, 2, 1, 1); // 8 characters
        CharactersCount carniCounter02 = setCharacterCount(4, 1, 2, 1); // 8 characters
        CharactersCount carniCounter03 = setCharacterCount(5, 0, 3, 1); // 9 characters
        CharactersCount carniCounter04 = setCharacterCount(5, 1, 2, 1); // 9 characters
        CharactersCount carniCounter05 = setCharacterCount(5, 2, 1, 1); // 9 characters
        CharactersCount carniCounter06 = setCharacterCount(5, 1, 3, 1); // 10 characters
        CharactersCount carniCounter07 = setCharacterCount(5, 2, 2, 1); // 10 characters
        CharactersCount carniCounter08 = setCharacterCount(5, 3, 1, 1); // 10 characters
        CharactersCount carniCounter09 = setCharacterCount(5, 4, 1, 1); // 11 characters
        CharactersCount carniCounter10 = setCharacterCount(6, 2, 2, 1); // 11 characters
        CharactersCount carniCounter11 = setCharacterCount(6, 1, 3, 1); // 11 characters
        CharactersCount carniCounter12 = setCharacterCount(6, 0, 4, 1); // 11 characters
        Il2CppSystem.Collections.Generic.List<CharactersCount> carniCounterList = new Il2CppSystem.Collections.Generic.List<CharactersCount>();


        // 8 characters
        carniCounterList.Add(carniCounter01);
        carniCounterList.Add(carniCounter01);
        carniCounterList.Add(carniCounter01);
        carniCounterList.Add(carniCounter01);
        carniCounterList.Add(carniCounter02);
        carniCounterList.Add(carniCounter02);
        carniCounterList.Add(carniCounter02);
        carniCounterList.Add(carniCounter02);

        // 9 characters
        carniCounterList.Add(carniCounter03);
        carniCounterList.Add(carniCounter03);
        carniCounterList.Add(carniCounter04);
        carniCounterList.Add(carniCounter04);
        carniCounterList.Add(carniCounter04);
        carniCounterList.Add(carniCounter04);
        carniCounterList.Add(carniCounter04);
        carniCounterList.Add(carniCounter04);
        carniCounterList.Add(carniCounter05);
        carniCounterList.Add(carniCounter05);
        carniCounterList.Add(carniCounter05);
        carniCounterList.Add(carniCounter05);

        // 10 characters
        carniCounterList.Add(carniCounter06);
        carniCounterList.Add(carniCounter06);
        carniCounterList.Add(carniCounter07);
        carniCounterList.Add(carniCounter07);
        carniCounterList.Add(carniCounter07);
        carniCounterList.Add(carniCounter07);
        carniCounterList.Add(carniCounter07);
        carniCounterList.Add(carniCounter07);
        carniCounterList.Add(carniCounter07);
        carniCounterList.Add(carniCounter07);
        carniCounterList.Add(carniCounter08);
        carniCounterList.Add(carniCounter08);

        // 11 characters
        carniCounterList.Add(carniCounter09);
        carniCounterList.Add(carniCounter10);
        carniCounterList.Add(carniCounter10);
        carniCounterList.Add(carniCounter11);
        carniCounterList.Add(carniCounter11);
        carniCounterList.Add(carniCounter12);


        carniScript.characterCounts = carniCounterList;
        carniScriptData.scriptInfo = carniScript;




        //CustomScriptData tenecaligoScriptData = new CustomScriptData();
        //tenecaligoScriptData.name = "Tenecaligo_1";
        //ScriptInfo tenecaligoScript = new ScriptInfo();
        //Il2CppSystem.Collections.Generic.List<CharacterData> tenecaligoList = new Il2CppSystem.Collections.Generic.List<CharacterData>();
        //tenecaligoList.Add(w_fogDemon);
        //tenecaligoScript.mustInclude = tenecaligoList;
        //tenecaligoScript.startingDemons = tenecaligoList;
        //tenecaligoScript.startingTownsfolks = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingTownsfolks;
        //tenecaligoScript.startingOutsiders = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingOutsiders;
        //tenecaligoScript.startingMinions = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingMinions;
        //CharactersCount tenecaligoCounter2 = new CharactersCount(10, 9, 1, 0, 0);
        //tenecaligoCounter2.dOuts = 4;
        //Il2CppSystem.Collections.Generic.List<CharactersCount> tenecaligoCounterList = new Il2CppSystem.Collections.Generic.List<CharactersCount>();
        //tenecaligoCounterList.Add(tenecaligoCounter2);
        //tenecaligoScript.characterCounts = tenecaligoCounterList;
        //tenecaligoScriptData.scriptInfo = tenecaligoScript;




        //CustomScriptData furtamuScriptData = new CustomScriptData();
        //furtamuScriptData.name = "Furtamu_1";
        //ScriptInfo furtamuScript = new ScriptInfo();
        //Il2CppSystem.Collections.Generic.List<CharacterData> furtamuList = new Il2CppSystem.Collections.Generic.List<CharacterData>();
        //furtamuList.Add(w_switchDemon);
        //furtamuScript.mustInclude = furtamuList;
        //furtamuScript.startingDemons = furtamuList;
        //furtamuScript.startingTownsfolks = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingTownsfolks;
        //furtamuScript.startingOutsiders = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingOutsiders;
        //furtamuScript.startingMinions = ProjectContext.Instance.gameData.advancedAscension.possibleScriptsData[0].scriptInfo.startingMinions;
        //CharactersCount furtamuCounter1 = new CharactersCount(8, 5, 1, 1, 1);
        //furtamuCounter1.dOuts = furtamuCounter1.outs + 1;
        //CharactersCount furtamuCounter2 = new CharactersCount(9, 5, 1, 2, 1);
        //furtamuCounter2.dOuts = furtamuCounter2.outs + 1;
        //CharactersCount furtamuCounter3 = new CharactersCount(10, 6, 1, 1, 2);
        //furtamuCounter3.dOuts = furtamuCounter3.outs + 1;
        //Il2CppSystem.Collections.Generic.List<CharactersCount> furtamuCounterList = new Il2CppSystem.Collections.Generic.List<CharactersCount>();
        //furtamuCounterList.Add(furtamuCounter1);
        //furtamuCounterList.Add(furtamuCounter2);
        //furtamuCounterList.Add(furtamuCounter3);
        //furtamuScript.characterCounts = furtamuCounterList;
        //furtamuScriptData.scriptInfo = furtamuScript;

        //List<CharacterData> strangeDisguisesChars = new List<CharacterData>();
        //addCharacterDataToList("Clairvoyant_TST", strangeDisguisesChars);
        //addCharacterDataToList("Cleric_EP", strangeDisguisesChars);
        //addCharacterDataToList("Confessor_18741708", strangeDisguisesChars);
        //addCharacterDataToList("Detective_VP", strangeDisguisesChars);
        //addCharacterDataToList("Dreamer_32014895", strangeDisguisesChars);
        //addCharacterDataToList("Druid_89845092", strangeDisguisesChars);
        //addCharacterDataToList("Empress_13782227", strangeDisguisesChars);
        //addCharacterDataToList("Forager_TST", strangeDisguisesChars);
        //addCharacterDataToList("Lookout_41018246", strangeDisguisesChars);
        //addCharacterDataToList("Oracle_07039445", strangeDisguisesChars);
        //addCharacterDataToList("Prince_TST", strangeDisguisesChars);
        //addCharacterDataToList("Scout_88081716", strangeDisguisesChars);
        //addCharacterDataToList("Sentinel_TST", strangeDisguisesChars);
        //addCharacterDataToList("Village Idiot_VP", strangeDisguisesChars);
        //addCharacterDataToList("Witness_25155076", strangeDisguisesChars);
        //addCharacterDataToList("Doppleganger_52694042", strangeDisguisesChars);
        //addCharacterDataToList("Drunk_15369527", strangeDisguisesChars);
        //addCharacterDataToList("GoodTwin_VP", strangeDisguisesChars);
        //addCharacterDataToList("Lunatic_TST", strangeDisguisesChars);
        //addCharacterDataToList("Mayor_VP", strangeDisguisesChars);
        //addCharacterDataToList("Lycaon_VP", strangeDisguisesChars);
        //addCharacterDataToList("Shaman_26945607", strangeDisguisesChars);
        //addCharacterDataToList("Swarm_Good_TST", strangeDisguisesChars);
        //addCharacterDataToList("Turncoat_TST", strangeDisguisesChars);
        //addCharacterDataToList("Belias_EP", strangeDisguisesChars);
        //addCharacterDataToList("Illusionist_TST", strangeDisguisesChars);
        //addCharacterDataToList("TwinDemon_TST", strangeDisguisesChars);
        //replaceScriptChars(strangeDisguisesChars, strangeDisguisesScriptData);
        //Il2CppReferenceArray<CharacterData> advancedAscensionDemons = new Il2CppReferenceArray<CharacterData>(advancedAscension.demons.Length + 2);
        //advancedAscensionDemons = advancedAscension.demons;
        //advancedAscensionDemons[advancedAscensionDemons.Length - 2] = w_legion;
        //advancedAscensionDemons[advancedAscensionDemons.Length - 1] = w_twindemon;
        //advancedAscension.demons = advancedAscensionDemons;
        //Il2CppReferenceArray<CharacterData> advancedAscensionStartingDemons = new Il2CppReferenceArray<CharacterData>(advancedAscension.startingDemons.Length + 2);
        //advancedAscensionStartingDemons = advancedAscension.startingDemons;
        //advancedAscensionStartingDemons[advancedAscensionStartingDemons.Length - 2] = w_legion;
        //advancedAscensionStartingDemons[advancedAscensionStartingDemons.Length - 1] = w_twindemon;
        //advancedAscension.startingDemons = advancedAscensionStartingDemons;
        //Il2CppReferenceArray<CustomScriptData> advancedAscensionScriptsData = new Il2CppReferenceArray<CustomScriptData>(advancedAscension.possibleScriptsData.Length + 2);
        //advancedAscensionScriptsData = advancedAscension.possibleScriptsData;
        //advancedAscensionScriptsData[advancedAscensionScriptsData.Length - 2] = legionScriptData;
        //advancedAscensionScriptsData[advancedAscensionScriptsData.Length - 1] = twinDemonScriptData;
        //advancedAscension.possibleScriptsData = advancedAscensionScriptsData;
        MelonLogger.Msg($"Adding scripts");
        AscensionsData advancedAscension = ProjectContext.Instance.gameData.advancedAscension;



        for (int i = 0; i < 100; i++)
        {
            //w_addDemonRole(advancedAscension, w_pandemonium, "Baa_Difficult", "Pandemonium_1", pandemoniumScriptData, emptyCharacterDataList);
        }


        Il2CppSystem.Collections.Generic.List<CharacterData> undyingJinxes = new Il2CppSystem.Collections.Generic.List<CharacterData>();
        //undyingJinxes.Add(w_shard);

        Il2CppSystem.Collections.Generic.List<CharacterData> turncoatJinxes = new Il2CppSystem.Collections.Generic.List<CharacterData>();
        //turncoatJinxes.Add(w_shard);

        Il2CppSystem.Collections.Generic.List<CharacterData> pilgrimJinxes = new Il2CppSystem.Collections.Generic.List<CharacterData>();
        //pilgrimJinxes.Add(w_shard);


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

            addRole(script.startingMinions, pestilence);
            addRole(script.startingMinions, illusionist);

            addRole(script.startingDemons, cultist);

            for (int i = 0; i < 100; i++)
            {
                //addRoleEvenIfDupe(script.startingTownsfolks, w_knave);
                //addRoleEvenIfDupe(script.startingOutsiders, w_tergiversator);
                //addRoleEvenIfDupe(script.startingMinions, w_ritualist);
                //addRoleEvenIfDupe(script.startingMinions, w_snakeCharmer);
            }
            for (int i = 0; i < allDatas.Length; i++)
            {
                //if (allDatas[i].characterId == "Gambler_42592744")
                //{
                //    script.startingTownsfolks.Remove(allDatas[i]);
                //}
            }
        }
        /*MelonLogger.Msg($"Trying to jinx roles...");
        foreach (CustomScriptData scriptData in advancedAscension.possibleScriptsData)
        {
            Il2CppSystem.Collections.Generic.List<string> JinxList = new Il2CppSystem.Collections.Generic.List<string>();
            JinxList.Add("Bishop_58855542");
            JinxList.Add("Empress_13782227");
            JinxList.Add("Oracle_07039445");
            JinxList.Add("Prince_TST");
            JinxList.Add("Doppleganger_52694042");
            JinxList.Add("Baron_04539999");
            JinxList.Add("Mezepheles_09511163");
            JinxList.Add("Shaman_26945607");
            JinxList.Add("Swarm_Good_TST");
            JinxList.Add("Toxomancer_TST");
            if (scriptData.name.ToString() == "Legion_1")
            {
                for (int i = 0; i < legionScript.startingTownsfolks.Count; i++)
                {
                    if (JinxList.Contains(legionScript.startingTownsfolks[i].characterId))
                    {
                        i -= 1;
                        legionScript.startingTownsfolks.RemoveAt(i);
                    }
                }
                for (int i = 0; i < legionScript.startingOutsiders.Count; i++)
                {
                    if (JinxList.Contains(legionScript.startingOutsiders[i].characterId))
                    {
                        i -= 1;
                        legionScript.startingOutsiders.RemoveAt(i);
                    }
                }
                for (int i = 0; i < legionScript.startingMinions.Count; i++)
                {
                    if (JinxList.Contains(legionScript.startingMinions[i].characterId))
                    {
                        i -= 1;
                        legionScript.startingMinions.RemoveAt(i);
                    }
                }
            }
        }
        */


        for (int j = 0; j < advancedAscension.possibleScriptsData.Length; j++)
        {
            Debug.LogWarning(advancedAscension.possibleScriptsData[j].name);
            MelonLogger.Msg($"Script: {advancedAscension.possibleScriptsData[j].name.ToString()}");
        }
        //w_addDemonRole(advancedAscension, w_switchDemon, "Baa_Difficult", "Furtamu_1", furtamuScriptData);
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