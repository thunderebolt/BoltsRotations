﻿using FFXIVClientStructs.FFXIV.Client.Game;
using RotationSolver.Basic.Data;
using static FFXIVClientStructs.FFXIV.Client.UI.Misc.ConfigModule;

namespace BoltsRotations.Ranged
{
    [LinkDescription("$Your link description here, it is better to link to a png! this attribute can be multiple! $")]
    [SourceCode("$If your rotation is open source, please write the link to this attribute! $")]
    [RotationDesc(ActionID.Wildfire)]

    // Change this base class to your job's base class. It is named like XXX_Base.
    internal class MCH_64 : MCH_Base
    {
        //Change this to the game version right now.
        public override string GameVersion => "6.41";

        public override string RotationName => "Bolt's Machinist";

        public override string Description => "Delayed Opener \nUses the first pot automatically (if enabled)  \nPot yourself for best deeps \n\nWill dump everything when the boss is dying. Set the dying HP cutoff in settings.  \n\nMade for a Level 90 MCH!";

        protected override IAction CountDownAction(float remainTime)
        {
            if (remainTime < Service.Config.CountDownAhead)
            {
                if (SplitShot.CanUse(out var act1, CanUseOption.MustUse)) return act1;
            }
            if (remainTime < 2 && UseBurstMedicine(out var act)) return act;
            return base.CountDownAction(remainTime);
        }

        //GCD actions here.
        protected override bool EmergencyGCD(out IAction act)
        {

            //Overheated
            if (AutoCrossbow.CanUse(out act)) return true;
            if (HeatBlast.CanUse(out act)) return true;

            if (BioBlaster.CanUse(out act)) return true;
            if (!AirAnchor.EnoughLevel && HotShot.CanUse(out act)) return true;

            if (CombatElapsedLess(12))
            {
                if (!CombatElapsedLess(1) && Drill.CanUse(out act, CanUseOption.MustUse)) return true;

                if (Drill.ElapsedAfterGCD(2) && AirAnchor.CanUse(out act, CanUseOption.MustUse)) return true;

                if (IsLastGCD(true, AirAnchor) && ChainSaw.CanUse(out act, CanUseOption.MustUse)) return true;
            }
            if (!CombatElapsedLessGCD(1) && Drill.CanUse(out act, CanUseOption.MustUse)) return true;
            if (!CombatElapsedLessGCD(4) && AirAnchor.CanUse(out act, CanUseOption.MustUse)) return true;
            if (!CombatElapsedLessGCD(5) && ChainSaw.CanUse(out act, CanUseOption.MustUse)) return true;

           

            //Aoe
            if (ChainSaw.CanUse(out act)) return true;
            if (SpreadShot.CanUse(out act)) return true;

            if (CleanShot.CanUse(out act)) return true;
            if (SlugShot.CanUse(out act)) return true;
            if (SplitShot.CanUse(out act)) return true;
            return false;
        }

        protected override bool EmergencyAbility(IAction nextGCD, out IAction act)
        {
            
            if (IsTargetDying)
            {
                
                if (Player.HasStatus(false, StatusID.Reassemble))
                {
                    if (Reassemble.CanUse(out act, CanUseOption.EmptyOrSkipCombo)) return true;
                }

                if (GaussRound.CanUse(out act, CanUseOption.EmptyOrSkipCombo)) return true;
                if (Ricochet.CanUse(out act, CanUseOption.EmptyOrSkipCombo)) return true;
                if (AirAnchor.CanUse(out act)) return true;
                if (ChainSaw.CanUse(out act)) return true;
                if (Drill.CanUse(out act)) return true;
                if (RookAutoturret.CanUse(out act)) return true;
            }

            // Queen during opener - 50 battery
            if (Battery >= 50 && CombatElapsedLessGCD(18))
            {
                if (RookAutoturret.CanUse(out act)) return true;
            }

            // The second Queen at one minute 
            if (CombatElapsedLess(61) && !CombatElapsedLess(31) && Battery >= 50)
            {
                if (RookAutoturret.CanUse(out act)) return true;
            }

            // Even Batteries
            if (Wildfire.ElapsedAfter(115) && Battery == 100)
            {
                if (RookAutoturret.CanUse(out act)) return true;
            }

            // Odd Batteries
            if (Battery >= 80 && !Wildfire.ElapsedAfter(80))
            {
                if (RookAutoturret.CanUse(out act)) return true;
            }

            if (nextGCD.IsTheSameTo(true, AirAnchor))
            {
                if (Reassemble.CanUse(out act, CanUseOption.MustUse)) return true;
            }

            if (nextGCD.IsTheSameTo(true, ChainSaw))
            {
                if (Reassemble.CanUse(out act, CanUseOption.EmptyOrSkipCombo)) return true;
            }

            if (InBurst && IsTargetBoss)
            {
                if (Heat >= 50 && !CombatElapsedLess(10)
                    && Wildfire.CanUse(out act, CanUseOption.MustUse)) return true;
            }

            if (IsLastGCD(true, Drill) && CombatElapsedLess(10))
            {
                if (BarrelStabilizer.CanUse(out act)) return true;
            }

            if (Wildfire.ElapsedAfter(100))
            {
                if (BarrelStabilizer.CanUse(out act)) return true;
            }

            if (CanUseHypercharge(out act))
            {
                if (Hypercharge.CanUse(out act)) return true;
            }

            return base.EmergencyAbility(nextGCD, out act);
        }

        protected override bool GeneralGCD(out IAction act)
        {
            if (SplitShot.CanUse(out act)) return true;

            return false;
        }

        #region 0GCD actions
        protected override bool AttackAbility( out IAction act)
        {
            if (Ricochet.CanUse(out act, CanUseOption.MustUseEmpty)) return true;
            if (GaussRound.CanUse(out act, CanUseOption.MustUseEmpty)) return true;
            return false;
        }

        private bool CanUseHypercharge(out IAction act)
        {
            act = null; 
            
            if (Player.HasStatus(true, StatusID.Reassemble)) return false;

            if (Drill.EnoughLevel && Drill.WillHaveOneChargeGCD(2)) return false;
            if (AirAnchor.EnoughLevel && AirAnchor.WillHaveOneCharge(8)) return false;
            if (ChainSaw.EnoughLevel && ChainSaw.WillHaveOneCharge(8)) return false;


            if (AutoCrossbow.CanUse(out _))
            {
                return true;
            }

            if (Heat >= 50 && !Wildfire.ElapsedAfter(90) && Wildfire.IsCoolingDown) return true;
            if (Player.HasStatus(true, StatusID.Wildfire)) return true;

            return Hypercharge.CanUse(out act);
        }

        //Some 0gcds that don't need to a hostile target in attack range.
        protected override bool GeneralAbility(out IAction act)
        {
            return base.GeneralAbility(out act);
        }
        #endregion

        #region Extra
      
        //Extra configurations you want to show on your rotation config.
        protected override IRotationConfigSet CreateConfiguration()
        {
            return base.CreateConfiguration();
        }

        //This is the method to update all field you wrote, it is used first during one frame.
        protected override void UpdateInfo()
        {
            
        }

        //This method is used when player change the terriroty, such as go into one duty, you can use it to set the field.
        public override void OnTerritoryChanged()
        {
            base.OnTerritoryChanged();
        }

        //This method is used to debug. If you want to show some information in Debug panel, show something here.
        public override void DisplayStatus()
        {
            base.DisplayStatus();
        }
        #endregion
    }
}