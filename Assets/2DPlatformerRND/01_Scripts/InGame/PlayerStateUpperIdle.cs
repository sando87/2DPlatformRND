using UnityEngine;


namespace PahlBit
{
    public class PlayerStateUpperIdle : PlayerStateBase
    {
        public override void InitState()
        {
            base.InitState();

            Base.AnimHelper.AddEventEnter(AnimStateNameHash.UpperIdle, () =>
            {
                ChangeStateToIdle();
            });
        }

    }
}
