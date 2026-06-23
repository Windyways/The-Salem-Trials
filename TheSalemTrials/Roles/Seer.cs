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
public class Seer : Role
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

        string info = $"";
        if (chars[0].IsCorrupted() && chars[1].IsCorrupted()) info = $"#{chars[0].id} and #{chars[1].id}\nare common\nwith Corruption!";
        else if (chars[0].bluff && chars[1].bluff) info = $"#{chars[0].id} and #{chars[1].id}\nare common\nwith Disguising!";
        else if (chars[0].alignment == EAlignment.Evil && chars[1].alignment == EAlignment.Evil) info = $"#{chars[0].id} and #{chars[1].id}\nare common\nwith Evil!";
        else if (chars[0].alignment == EAlignment.Good && chars[1].alignment == EAlignment.Good) info = $"#{chars[0].id} and #{chars[1].id}\nare common\nwith Good!";
        else info = $"#{chars[0].id} and #{chars[1].id}\nhave nothing in common!";

        onActed?.Invoke(new ActedInfo(info, chars));
        Debug.Log($"{info}");
    }

    private void CharacterPickedLiar()
    {
        CharacterPicker.OnCharactersPicked -= action1;
        CharacterPicker.OnStopPick -= action2;

        Il2CppSystem.Collections.Generic.List<Character> chars = new Il2CppSystem.Collections.Generic.List<Character>();
        chars.Add(CharacterPicker.PickedCharacters[0]);
        chars.Add(CharacterPicker.PickedCharacters[1]);

        string info = $"";
        int trueInfo = 0;
        if (chars[0].IsCorrupted() && chars[1].IsCorrupted()) trueInfo = 1;
        else if (chars[0].bluff && chars[1].bluff) trueInfo = 2;
        else if (chars[0].alignment == EAlignment.Evil && chars[1].alignment == EAlignment.Evil) trueInfo = 3;
        else if (chars[0].alignment == EAlignment.Good && chars[1].alignment == EAlignment.Good) trueInfo = 4;

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
        }

        onActed?.Invoke(new ActedInfo(info, chars));
        Debug.Log($"{info}");
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
}