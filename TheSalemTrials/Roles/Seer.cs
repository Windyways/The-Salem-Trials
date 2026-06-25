using Il2Cpp;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppSystem;
using MelonLoader;
using System;
using System.ComponentModel.Design;
using UnityEngine;

namespace TheSalemTrials;

[RegisterTypeInIl2Cpp]
public class Seer : Role // Code base from Arbiter from Demon Bluff Expansion Pack!
{
    Character chRef;
    private Il2CppSystem.Action action1;
    private Il2CppSystem.Action action2;
    private Il2CppSystem.Action action3;
    public override ActedInfo GetInfo(Character charRef)
    {
        return new ActedInfo("", null);
    }

    public override ActedInfo GetBluffInfo(Character charRef)
    {
        return new ActedInfo("", null);
    }

    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        chRef = charRef;
        CharacterPicker.Instance.StartPickCharacters(2, charRef);
        CharacterPicker.OnCharactersPicked += action1;
        CharacterPicker.OnStopPick += action2;
    }

    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        chRef = charRef;
        CharacterPicker.Instance.StartPickCharacters(2, charRef);
        CharacterPicker.OnCharactersPicked += action3;
        CharacterPicker.OnStopPick += action2;
    }

    private void CharacterPicked()
    {
        CharacterPicker.OnCharactersPicked -= action1;
        CharacterPicker.OnStopPick -= action2;

        Il2CppSystem.Collections.Generic.List<Character> chars = new Il2CppSystem.Collections.Generic.List<Character>();
        chars.Add(CharacterPicker.PickedCharacters[0]);
        chars.Add(CharacterPicker.PickedCharacters[1]);

        int index = 0;
        if (chars[0].IsCorrupted() && chars[1].IsCorrupted()) index = 1;
        else if (chars[0].IsDisguising() && chars[1].IsDisguising()) index = 2;
        else if (chars[0].alignment == chars[1].alignment) index = 3;

        string info = InfoFromIndex(chars[0], chars[1], index);
        onActed?.Invoke(new ActedInfo(info, chars));
    }

    private void CharacterPickedLiar() // Pain in the butt to comprehend.
    {
        CharacterPicker.OnCharactersPicked -= action1;
        CharacterPicker.OnStopPick -= action2;

        Il2CppSystem.Collections.Generic.List<Character> chars = new Il2CppSystem.Collections.Generic.List<Character>();
        chars.Add(CharacterPicker.PickedCharacters[0]);
        chars.Add(CharacterPicker.PickedCharacters[1]);

        /*string info = $"";

        if (trueInfo == 1)
        {
            int diceRoll = Calculator.RollDice(4);
            if (diceRoll == 1) info = $"#{chars[0].id} and #{chars[1].id}\nare common\nwith Disguising!";
            if (diceRoll == 2) info = $"#{chars[0].id} and #{chars[1].id}\nare common\nwith Evil!";
            if (diceRoll == 3) info = $"#{chars[0].id} and #{chars[1].id}\nare common\nwith Good!";
            if (diceRoll == 4) info = $"#{chars[0].id} and #{chars[1].id}\nhave nothing in common!";
        }
        else if (trueInfo == 2)
        {
            int diceRoll = Calculator.RollDice(4);
            if (diceRoll == 1) info = $"#{chars[0].id} and #{chars[1].id}\nare common\nwith Corruption!";
            if (diceRoll == 2) info = $"#{chars[0].id} and #{chars[1].id}\nare common\nwith Evil!";
            if (diceRoll == 3) info = $"#{chars[0].id} and #{chars[1].id}\nare common\nwith Good!";
            if (diceRoll == 4) info = $"#{chars[0].id} and #{chars[1].id}\nhave nothing in common!";
        }
        else if (trueInfo == 3)
        {
            int diceRoll = Calculator.RollDice(4);
            if (diceRoll == 1) info = $"#{chars[0].id} and #{chars[1].id}\nare common\nwith Corruption!";
            if (diceRoll == 2) info = $"#{chars[0].id} and #{chars[1].id}\nare common\nwith Disguising!";
            if (diceRoll == 3) info = $"#{chars[0].id} and #{chars[1].id}\nare common\nwith Good!";
            if (diceRoll == 4) info = $"#{chars[0].id} and #{chars[1].id}\nhave nothing in common!";
        }
        else if (trueInfo == 4)
        {
            int diceRoll = Calculator.RollDice(4);
            if (diceRoll == 1) info = $"#{chars[0].id} and #{chars[1].id}\nare common\nwith Corruption!";
            if (diceRoll == 2) info = $"#{chars[0].id} and #{chars[1].id}\nare common\nwith Evil!";
            if (diceRoll == 3) info = $"#{chars[0].id} and #{chars[1].id}\nare common\nwith Disguising!";
            if (diceRoll == 4) info = $"#{chars[0].id} and #{chars[1].id}\nhave nothing in common!";
        }
        else // if (trueInfo == 0)
        {
            int diceRoll = Calculator.RollDice(4);
            if (diceRoll == 1) info = $"#{chars[0].id} and #{chars[1].id}\nare common\nwith Corruption!";
            if (diceRoll == 2) info = $"#{chars[0].id} and #{chars[1].id}\nare common\nwith Evil!";
            if (diceRoll == 3) info = $"#{chars[0].id} and #{chars[1].id}\nare common\nwith Good!";
            if (diceRoll == 4) info = $"#{chars[0].id} and #{chars[1].id}\nare common\nwith Disguising!";
        }*/

        int index = 0;
        if (chars[0].IsCorrupted() && chars[1].IsCorrupted()) index = 1;
        else if (chars[0].IsDisguising() && chars[1].IsDisguising()) index = 2;
        else if (chars[0].alignment == chars[1].alignment) index = 3;

        string info = InfoFromIndex(chars[0], chars[1], Ext.MakeNumberWrong(index, 2, 0));
        onActed?.Invoke(new ActedInfo(info, chars));
    }

    private void StopPick()
    {
        CharacterPicker.OnCharactersPicked -= action1;
        CharacterPicker.OnStopPick -= action2;
        CharacterPicker.OnCharactersPicked -= action3;
    }

    public Seer() : base(ClassInjector.DerivedConstructorPointer<Seer>())
    {
        ClassInjector.DerivedConstructorBody((Il2CppObjectBase)this);
        action1 = new System.Action(CharacterPicked);
        action2 = new System.Action(StopPick);
        action3 = new System.Action(CharacterPickedLiar);
    }

    public Seer(System.IntPtr ptr) : base(ptr)
    {
        action1 = new System.Action(CharacterPicked);
        action2 = new System.Action(StopPick);
        action3 = new System.Action(CharacterPickedLiar);
    }

    private string InfoFromIndex(Character c1, Character c2, int index)
    {
        if (index == 1) return $"#{c1.id} and #{c2.id}\nare both\nCorrupted!";
        if (index == 2) return $"#{c1.id} and #{c2.id}\nare both\nDisguising!";
        if (index == 3) return $"#{c1.id} and #{c2.id}\nhave the same alignment!";
        return $"#{c1.id} and #{c2.id}\nhave nothing in common!";
    }
}