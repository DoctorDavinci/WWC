using BDArmory;
using System.Collections.Generic;
using UnityEngine;
using HullBreach;
using System.Collections;


namespace WetWeaponCheck
{
    public class ModuleVesselAlarms: PartModule
    {
        private bool targeted = false;
        private bool hullBreached = false;
        private bool hullCritical = false;
        private bool targetedAlert = false;
        private bool breachAlert = false;
        private bool criticalAlert = false;
        private bool pauseRoutine = false;
        private bool hbPause = false;

        private AudioSource soundBreach;
        private AudioSource soundCritical;
        private AudioSource soundTargeted;

        [KSPField(isPersistant = true, guiActiveEditor = true, guiActive = true, guiName = "Vessel Alarms"),
         UI_Toggle(controlEnabled = false, scene = UI_Scene.All, disabledText = "Off", enabledText = "On")]
        public bool vesselAlarms = true;

        private MissileFire wmPart;
        private MissileFire wmCheck()
        {
            MissileFire _wmPart = null;

            _wmPart = part.FindModuleImplementing<MissileFire>();

            return _wmPart;
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            if (HighLogic.LoadedSceneIsFlight)
            {
                GetSounds();
            }
        }

        public void Update()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (vesselAlarms)
                {
                    if (!hbPause)
                    {
                        HullBreachCheck();
                    }

                    if (!pauseRoutine)
                    {
                        UnderFirecheck();
                    }

                    if (targeted && !targetedAlert)
                    {
                        if (vessel.isActiveVessel)
                        {
                            soundTargeted.Play();

                            ScreenMsg("ALARM: VESSEL UNDER ATTACK");
                        }
                        targetedAlert = true;
                    }

                    if (hullBreached && !breachAlert)
                    {
                        if (vessel.isActiveVessel)
                        {
                            soundBreach.Play();
                            ScreenMsg("ALARM: HULL BREACHED");
                        }
                        breachAlert = true;
                    }

                    if (hullCritical && !criticalAlert)
                    {
                        if (vessel.isActiveVessel)
                        {
                            soundCritical.Play();
                            ScreenMsg("ALARM: HULL CRITICAL");
                        }
                        criticalAlert = true;
                    }
                }
            }
        }

        IEnumerator HBPauseRoutine()
        {
            hbPause = true;
            yield return new WaitForSeconds(3);
            hbPause = false;
        }

        IEnumerator PauseRoutine()
        {
            pauseRoutine = true;
            yield return new WaitForSeconds(3);
            pauseRoutine = false;
        }



        private void ScreenMsg(string msg)
        {
            ScreenMessages.PostScreenMessage(new ScreenMessage(msg, 4, ScreenMessageStyle.UPPER_CENTER));
        }

        private void UnderFirecheck()
        {
            List<MissileFire> wmParts = new List<MissileFire>(200);
            foreach (Part p in vessel.Parts)
            {
                wmParts.AddRange(p.FindModulesImplementing<MissileFire>());
            }
            foreach (MissileFire wmPart in wmParts)
            {
                if (wmPart.underAttack)
                {
                    targeted = true;
                }
                else
                {
                    targeted = false;
                    targetedAlert = false;
                }
            }
        }

        private void HullBreachCheck()
        {
            List<ModuleHullBreach> hpParts = new List<ModuleHullBreach>(200);
            foreach (Part p in vessel.Parts)
            {
                hpParts.AddRange(p.FindModulesImplementing<ModuleHullBreach>());
            }
            foreach (ModuleHullBreach hpPart in hpParts)
            {
                if (hpPart.isHullBreached)
                {
                    hullBreached = true;
                }

                if (hpPart.DamageState == "Critical")
                {
                    hullCritical = true;
                }
            }

            StartCoroutine(HBPauseRoutine());
        }


        private void GetSounds()
        {
            soundBreach = gameObject.AddComponent<AudioSource>();
            soundBreach.clip = GameDatabase.Instance.GetAudioClip("WWC/Sounds/sound_Klaxon");
            soundCritical = gameObject.AddComponent<AudioSource>();
            soundCritical.clip = GameDatabase.Instance.GetAudioClip("WWC/Sounds/sound_Klaxon_Critical");
            soundTargeted = gameObject.AddComponent<AudioSource>();
            soundTargeted.clip = GameDatabase.Instance.GetAudioClip("WWC/Sounds/sound_missileWarning");

            soundBreach.loop = false;
            soundBreach.volume = GameSettings.AMBIENCE_VOLUME;
            soundBreach.dopplerLevel = 0f;
            soundBreach.rolloffMode = AudioRolloffMode.Logarithmic;
            soundBreach.minDistance = 0.5f;
            soundBreach.maxDistance = 1f;

            soundCritical.loop = false;
            soundCritical.volume = GameSettings.AMBIENCE_VOLUME;
            soundCritical.dopplerLevel = 0f;
            soundCritical.rolloffMode = AudioRolloffMode.Logarithmic;
            soundCritical.minDistance = 0.5f;
            soundCritical.maxDistance = 1f;

            soundTargeted.loop = false;
            soundTargeted.volume = GameSettings.AMBIENCE_VOLUME;
            soundTargeted.dopplerLevel = 0f;
            soundTargeted.rolloffMode = AudioRolloffMode.Logarithmic;
            soundTargeted.minDistance = 0.5f;
            soundTargeted.maxDistance = 1f;
        }
    }
}
