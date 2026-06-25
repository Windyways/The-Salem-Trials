using Il2Cpp;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes;

namespace TheSalemTrials
{
    public class Illusionist : Role
    {
        public Illusionist() : base(ClassInjector.DerivedConstructorPointer<Illusionist>())
        {
            ClassInjector.DerivedConstructorBody((Il2CppObjectBase)this);
        }
        public Illusionist(System.IntPtr ptr) : base(ptr)
        {

        }

        public override CharacterData GetBluffIfAble(Character charRef)
        {
            // Become a new character
            CharacterData bluff = Characters.Instance.GetRandomUniqueVillagerBluff();
            Gameplay.Instance.AddScriptCharacterIfAble(bluff.type, bluff);
            return bluff;
        }
    }
}
