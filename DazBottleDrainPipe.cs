using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using TUNING;
using UnityEngine;
using KMod;
using static Localization;
using STRINGS;
using KSerialization;
using Klei;


namespace DazDrains
{

    //copied from https://forums.kleientertainment.com/forums/topic/123339-guide-for-creating-translatable-mods/?tab=comments#comment-1390234 beflore major edits
    [HarmonyPatch(typeof(Localization), "Initialize")]
    public class Localization_Initialize_Patch
    {
        
        public static void Postfix()
        {
            // Basic intended way to register strings, keeps namespace
            RegisterForTranslation(typeof(DazDrains.STRINGS));
            ModFileLoc = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            ModFileLoc = ModFileLoc.Replace("file:///", "");
            ModFileLoc = ModFileLoc.Replace("DazBottleDrainPipe.dll", "");
            //generates C:/ Users / diazo / Documents / Klei / OxygenNotIncluded / mods / Dev / DazBottleDrainPipe /
   
            //LoadStrings();

            // Register strings without namespace, this is what the game actually uses
            LocString.CreateLocStringKeys(typeof(DazDrains.STRINGS), null);
            Debug.Log("Daz assem " + Strings.Get(DazDrains.STRINGS.BUILDINGS.PREFABS.DAZBOTTLEDRAINPIPE.EFFECT));
            Debug.Log("Daz not assem " + Strings.Get(STRINGS.BUILDINGS.PREFABS.DAZBOTTLEDRAINPIPE.EFFECT));
            Debug.Log("Daz not assem name" + Strings.Get(STRINGS.BUILDINGS.PREFABS.DAZBOTTLEDRAINPIPE.NAME));
            // Creates template for users to edit
            //GenerateStringsTemplate(typeof(DazDrains.STRINGS), Path.Combine(ModFileLoc, "strings_templates"));
            Debug.Log("Daz translated strings " + Path.Combine(ModFileLoc, "strings_templates/" + GetLocale()?.Code + ".po"));
            if (GetLocale()?.Code != null)
            {
                Debug.Log("Daz translated strings " + Path.Combine(ModFileLoc, "strings_templates/" + GetLocale()?.Code + ".po"));
                OverloadStrings(LoadStringsFile(Path.Combine(ModFileLoc, "strings_templates/" + GetLocale()?.Code + ".po"), false));
            }
            else
            {
                Debug.Log("Daz Lang code null");
            }
            Debug.Log("Daz assem 2" + Strings.Get(DazDrains.STRINGS.BUILDINGS.PREFABS.DAZBOTTLEDRAINPIPE.EFFECT));
            Debug.Log("Daz not assem 2" + Strings.Get(STRINGS.BUILDINGS.PREFABS.DAZBOTTLEDRAINPIPE.EFFECT));
            Debug.Log("Daz assem 3" + Strings.Get(DazDrains.STRINGS.BUILDINGS.PREFABS.DAZBOTTLEDRAINPIPE.NAME));
            Debug.Log("Daz not assem 3a" + Strings.Get("." +STRINGS.BUILDINGS.PREFABS.DAZBOTTLEDRAINPIPE.NAME));
            Strings.PrintTable();
            //Strings.PrintTable();
        }
        public static string ModFileLoc;

        //private static void LoadStrings()
        //{
        //    string path = Path.Combine(KMod.Mod.ModPath, "translations", GetLocale()?.Code + ".po");
        //    if (File.Exists(path))
        
              //OverloadStrings(LoadStringsFile(Path.Combine(ModFileLoc, "strings_templates/"+ GetLocale()?.Code + ".po"), false));
             
        //}

    }

    public class STRINGS
    {
        public class BUILDINGS
        {
            public class PREFABS
            {
                public class DAZBOTTLEDRAINPIPE
                {
                    public static LocString NAME = (LocString)UI.FormatAsLink("Bottle Drain to Pipe", nameof(DAZBOTTLEDRAINPIPE));
                    public static LocString DESC = (LocString)("Allows Dupes to drain bottles into Liquid Pipe.");
                    public static LocString EFFECT = (LocString)("Test Effect?");
                }
            }
        }
    }
    


    [HarmonyPatch(typeof(Db))]
    [HarmonyPatch("Initialize")]
    public static class Db_Initialize_Patch
    {
        public static void Postfix()
        {
            //add to tech tree, note it uses a sized array, so have to make a new array 1 bigger and add building ID
            int TempCount = Database.Techs.TECH_GROUPING["LiquidPiping"].Count() + 1;
            string[] TempArray = new string[TempCount];
            Database.Techs.TECH_GROUPING["LiquidPiping"].CopyTo(TempArray, 0);
            //TempArray.Add() failed, left a blank entry, unsure why so workaround this way
            TempArray[TempCount - 1] = "DazBottleDrainPipe";
            Database.Techs.TECH_GROUPING["LiquidPiping"] = TempArray;
        }
    }


    [HarmonyPatch(typeof(GeneratedBuildings))]
    [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
    public class GeneratedBuildings_LoadGeneratedBuildings_Patch
    {
        public static void Prefix()
        {
            ModUtil.AddBuildingToPlanScreen("Plumbing", DazBottleDrainPipe.ID);
                        
        }
        //public static void Postfix()
        //{
        //    BuildingDef DazBottleTemp = Assets.GetBuildingDef("DazBottleDrainPipe");
        //    Debug.Log("Daz name check: " + DazBottleTemp.Name + DazBottleTemp.Desc + DazBottleTemp.Effect);
        //}
    }


    public class DazBottleDrainPipe : IBuildingConfig
    {
        public const string ID = "DazBottleDrainPipe";

        public override BuildingDef CreateBuildingDef()
        {
            Debug.Log("Daz making build def");
            float[] tieR2 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
            string[] plumbable = MATERIALS.PLUMBABLE;
            EffectorValues none1 = NOISE_POLLUTION.NONE;
            EffectorValues none2 = TUNING.BUILDINGS.DECOR.NONE;
            EffectorValues noise = none1;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 1, "DazBottleDrainPipe_kanim", 10, 3f, tieR2, plumbable, 1600f, BuildLocationRule.Anywhere, none2, noise);
            buildingDef.Floodable = false;
            buildingDef.Overheatable = false;
            buildingDef.Entombable = true;
            buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
            buildingDef.ObjectLayer = ObjectLayer.LiquidConduitConnection; 
            buildingDef.AudioCategory = "Metal";
            buildingDef.AudioSize = "small";
            buildingDef.BaseTimeUntilRepair = -1f;
            buildingDef.OutputConduitType = ConduitType.Liquid;
            buildingDef.UtilityInputOffset = new CellOffset(0, 0);
            buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
            buildingDef.SceneLayer = Grid.SceneLayer.LiquidConduitBridges;
            buildingDef.DragBuild = false;
            GeneratedBuildings.RegisterWithOverlay(OverlayScreen.LiquidVentIDs, ID);
            //Strings.Add("STRINGS.BUILDINGS.PREFABS." + ID.ToUpperInvariant() + ".NAME", "NamePlaceholder");
            //Strings.Add("STRINGS.BUILDINGS.PREFABS." + ID.ToUpperInvariant() + ".DESC", "DescPlaceholer");
            //Strings.Add("STRINGS.BUILDINGS.PREFABS." + ID.ToUpperInvariant() + ".EFFECT", "Effectplaceholder");
                       
            return buildingDef;
        }

        //required by IBuildingConfig, copied from LiquidConduitConfig for starting base
        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            Prioritizable.AddRef(go);
            Storage storage = go.AddOrGet<Storage>();
            storage.storageFilters = STORAGEFILTERS.LIQUIDS;
            storage.showInUI = true;
            storage.showDescriptor = true;
            storage.capacityKg = 107f;
            storage.allowItemRemoval = false;
            go.AddOrGet<TreeFilterable>();
            ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
            conduitDispenser.conduitType = ConduitType.Liquid;
            conduitDispenser.alwaysDispense = true;
            go.AddOrGet<DazBottleEmptier>();

        }



        //required by IBuildingConfig, copied from LiquidConduitConfig for starting base
        public override void DoPostConfigureComplete(GameObject go)
        {
            //Debug.Log("Daz not assem 3" + Strings.Get(STRINGS.BUILDINGS.PREFABS.DAZBOTTLEDRAINPIPE.NAME));
            //Debug.Log("Daz mod post config");
            //Database.Techs.TECH_GROUPING["LiquidPiping"].Add("DazBottleDrainPipe");
            //foreach (string str in Database.Techs.TECH_GROUPING["LiquidPiping"])
            //{
            //    Debug.Log("daz check " + str);
            //}
        }
        //public virtual void DoPostConfigurePreview(BuildingDef def, GameObject go)
        //{
        //}

        //public virtual void DoPostConfigureUnderConstruction(GameObject go)
        //{
        //}

        //public virtual void ConfigurePost(BuildingDef def)
        //{
        //}
    }



    //99% of DazBottleEmptier is a direct copy of BottleEmptier, renamed state machine and changed Emit method.
    //nuke OnStorageChanged since i can't do bottle full animations and squeez animation on free spriter
    [SerializationConfig(MemberSerialization.OptIn)]
    public class DazBottleEmptier :
      StateMachineComponent<DazBottleEmptier.StatesInstance>,
      IGameObjectEffectDescriptor
    {
        //public float emptyRate = 10f; emptying rate now in LiquidDispense GameObject
        [Serialize]
        public bool allowManualPumpingStationFetching;
        public bool isGasEmptier;
        [SerializeField]
        public Color noFilterTint = (Color)FilteredStorage.NO_FILTER_TINT;
        [SerializeField]
        public Color filterTint = (Color)FilteredStorage.FILTER_TINT;
        private static readonly EventSystem.IntraObjectHandler<DazBottleEmptier> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<DazBottleEmptier>((System.Action<DazBottleEmptier, object>)((component, data) => component.OnRefreshUserMenu(data)));
        private static readonly EventSystem.IntraObjectHandler<DazBottleEmptier> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<DazBottleEmptier>((System.Action<DazBottleEmptier, object>)((component, data) => component.OnCopySettings(data)));

        protected override void OnSpawn()
        {
            base.OnSpawn();
            this.smi.StartSM();
            this.Subscribe<DazBottleEmptier>(493375141, DazBottleEmptier.OnRefreshUserMenuDelegate);
            this.Subscribe<DazBottleEmptier>(-905833192, DazBottleEmptier.OnCopySettingsDelegate);
        }

        public List<Descriptor> GetDescriptors(GameObject go) => (List<Descriptor>)null;

        private void OnChangeAllowManualPumpingStationFetching()
        {
            this.allowManualPumpingStationFetching = !this.allowManualPumpingStationFetching;
            this.smi.RefreshChore();
        }

        private void OnRefreshUserMenu(object data) => Game.Instance.userMenu.AddButton(this.gameObject, this.allowManualPumpingStationFetching ? new KIconButtonMenu.ButtonInfo("action_bottler_delivery", (string)UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.DENIED.NAME, new System.Action(this.OnChangeAllowManualPumpingStationFetching), tooltipText: ((string)UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.DENIED.TOOLTIP)) : new KIconButtonMenu.ButtonInfo("action_bottler_delivery", (string)UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.ALLOWED.NAME, new System.Action(this.OnChangeAllowManualPumpingStationFetching), tooltipText: ((string)UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.ALLOWED.TOOLTIP)), 0.4f);

        private void OnCopySettings(object data)
        {
            this.allowManualPumpingStationFetching = ((GameObject)data).GetComponent<DazBottleEmptier>().allowManualPumpingStationFetching;
            this.smi.RefreshChore();
        }

        public class StatesInstance :
          GameStateMachine<DazBottleEmptier.States, DazBottleEmptier.StatesInstance, DazBottleEmptier, object>.GameInstance
        {
            private FetchChore chore;

            public MeterController meter { get; private set; }

            public StatesInstance(DazBottleEmptier smi)
              : base(smi)
            {
                this.master.GetComponent<TreeFilterable>().OnFilterChanged += new System.Action<Tag[]>(this.OnFilterChanged);
                //can't do sub-animations in free spriter, so no meter on bottle level
        //        this.meter = new MeterController((KAnimControllerBase)this.GetComponent<KBatchedAnimController>(), "meter_target", nameof(meter), Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[3]
        //        {
        //"meter_target",
        //"meter_arrow",
        //"meter_scale"
        //        });
               // this.Subscribe(-1697596308, new System.Action<object>(this.OnStorageChange));
                this.Subscribe(644822890, new System.Action<object>(this.OnOnlyFetchMarkedItemsSettingChanged));
            }

            public void CreateChore()
            {
                KBatchedAnimController component1 = this.GetComponent<KBatchedAnimController>();
                Tag[] tags = this.GetComponent<TreeFilterable>().GetTags();
                if (tags == null || tags.Length == 0)
                {
                    component1.TintColour = (Color32)this.master.noFilterTint;
                }
                else
                {
                    component1.TintColour = (Color32)this.master.filterTint;
                    Tag[] forbidden_tags;
                    if (!this.master.allowManualPumpingStationFetching)
                        forbidden_tags = new Tag[1]
                        {
            GameTags.LiquidSource
                        };
                    else
                        forbidden_tags = new Tag[0];
                    Storage component2 = this.GetComponent<Storage>();
                    this.chore = new FetchChore(Db.Get().ChoreTypes.StorageFetch, component2, component2.Capacity(), this.GetComponent<TreeFilterable>().GetTags(), forbidden_tags: forbidden_tags);
                }
            }

            public void CancelChore()
            {
                if (this.chore == null)
                    return;
                this.chore.Cancel("Storage Changed");
                this.chore = (FetchChore)null;
            }

            public void RefreshChore() => this.GoTo((StateMachine.BaseState)this.sm.unoperational);

            private void OnFilterChanged(Tag[] tags) => this.RefreshChore();

            //private void OnStorageChange(object data)
            //{
            //   //would like to put an emptying bottle animation back in eventually, so leave method for now
            //    //Storage component = this.GetComponent<Storage>();
            //    //this.meter.SetPositionPercent(Mathf.Clamp01(component.RemainingCapacity() / component.capacityKg));
            //}

            private void OnOnlyFetchMarkedItemsSettingChanged(object data) => this.RefreshChore();

            public void StartMeter()
            {
                PrimaryElement firstPrimaryElement = this.GetFirstPrimaryElement();
                if ((UnityEngine.Object)firstPrimaryElement == (UnityEngine.Object)null)
                    return;
                this.GetComponent<KBatchedAnimController>().SetSymbolTint(new KAnimHashedString("meter_target"), (Color)firstPrimaryElement.Element.substance.colour);
            }

            private PrimaryElement GetFirstPrimaryElement()
            {
                Storage component1 = this.GetComponent<Storage>();
                for (int idx = 0; idx < component1.Count; ++idx)
                {
                    GameObject gameObject = component1[idx];
                    if (!((UnityEngine.Object)gameObject == (UnityEngine.Object)null))
                    {
                        PrimaryElement component2 = gameObject.GetComponent<PrimaryElement>();
                        if (!((UnityEngine.Object)component2 == (UnityEngine.Object)null))
                            return component2;
                    }
                }
                return (PrimaryElement)null;
            }
            //totally remove all Emit code from BottleEmptier, everything goes out LiquidDispenser now, but still need this method in State Machine to triger reset on empty.
            public void Emit(float dt)
            {
                //Oddness going on where liquids can be zero amount in storage, but still exist in list with zero mass, this causes IsEmpty to false negative so check mass in storage for zero            
                Storage component = this.GetComponent<Storage>();
                if (component.MassStored() <= 0.00f)
                {
                    //Destroy zero mass item, note we remove it from storage first so we don't leave a 'null' in storage
                    GameObject go = component.items[0];
                    component.items.Remove(go);
                    Util.KDestroyGameObject(go);
                    
                    //reset state machine for next load of liquid
                    this.smi.RefreshChore();
                }
            }
        }

        public class States :
          GameStateMachine<DazBottleEmptier.States, DazBottleEmptier.StatesInstance, DazBottleEmptier>
        {
            private StatusItem statusItem;
            public GameStateMachine<DazBottleEmptier.States, DazBottleEmptier.StatesInstance, DazBottleEmptier, object>.State unoperational;
            public GameStateMachine<DazBottleEmptier.States, DazBottleEmptier.StatesInstance, DazBottleEmptier, object>.State waitingfordelivery;
            public GameStateMachine<DazBottleEmptier.States, DazBottleEmptier.StatesInstance, DazBottleEmptier, object>.State emptying;

            public override void InitializeStates(out StateMachine.BaseState default_state)
            {
                default_state = (StateMachine.BaseState)this.waitingfordelivery;
                this.statusItem = new StatusItem(nameof(DazBottleEmptier), "", "", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
                this.statusItem.resolveStringCallback = (Func<string, object, string>)((str, data) =>
                {
                    DazBottleEmptier bottleEmptier = (DazBottleEmptier)data;
                    if ((UnityEngine.Object)bottleEmptier == (UnityEngine.Object)null)
                        return str;
                    return bottleEmptier.allowManualPumpingStationFetching ? (string)(bottleEmptier.isGasEmptier ? BUILDING.STATUSITEMS.CANISTER_EMPTIER.ALLOWED.NAME : BUILDING.STATUSITEMS.BOTTLE_EMPTIER.ALLOWED.NAME) : (string)(bottleEmptier.isGasEmptier ? BUILDING.STATUSITEMS.CANISTER_EMPTIER.DENIED.NAME : BUILDING.STATUSITEMS.BOTTLE_EMPTIER.DENIED.NAME);
                });
                this.statusItem.resolveTooltipCallback = (Func<string, object, string>)((str, data) =>
                {
                    DazBottleEmptier bottleEmptier = (DazBottleEmptier)data;
                    if ((UnityEngine.Object)bottleEmptier == (UnityEngine.Object)null)
                        return str;
                    return bottleEmptier.allowManualPumpingStationFetching ? (bottleEmptier.isGasEmptier ? (string)BUILDING.STATUSITEMS.CANISTER_EMPTIER.ALLOWED.TOOLTIP : (string)BUILDING.STATUSITEMS.BOTTLE_EMPTIER.ALLOWED.TOOLTIP) : (bottleEmptier.isGasEmptier ? (string)BUILDING.STATUSITEMS.CANISTER_EMPTIER.DENIED.TOOLTIP : (string)BUILDING.STATUSITEMS.BOTTLE_EMPTIER.DENIED.TOOLTIP);
                });
                this.root.ToggleStatusItem(this.statusItem, (Func<DazBottleEmptier.StatesInstance, object>)(smi => (object)smi.master));
                this.unoperational.TagTransition(GameTags.Operational, this.waitingfordelivery).PlayAnim("off");
                this.waitingfordelivery.TagTransition(GameTags.Operational, this.unoperational, true).EventTransition(GameHashes.OnStorageChange, this.emptying, (StateMachine<DazBottleEmptier.States, DazBottleEmptier.StatesInstance, DazBottleEmptier, object>.Transition.ConditionCallback)(smi => !smi.GetComponent<Storage>().IsEmpty())).Enter("CreateChore", (StateMachine<DazBottleEmptier.States, DazBottleEmptier.StatesInstance, DazBottleEmptier, object>.State.Callback)(smi => smi.CreateChore())).Exit("CancelChore", (StateMachine<DazBottleEmptier.States, DazBottleEmptier.StatesInstance, DazBottleEmptier, object>.State.Callback)(smi => smi.CancelChore())).PlayAnim("on");
                this.emptying.TagTransition(GameTags.Operational, this.unoperational, true).EventTransition(GameHashes.OnStorageChange, this.waitingfordelivery, (StateMachine<DazBottleEmptier.States, DazBottleEmptier.StatesInstance, DazBottleEmptier, object>.Transition.ConditionCallback)(smi => smi.GetComponent<Storage>().IsEmpty())).Enter("StartMeter", (StateMachine<DazBottleEmptier.States, DazBottleEmptier.StatesInstance, DazBottleEmptier, object>.State.Callback)(smi => smi.StartMeter())).Update("Emit", (System.Action<DazBottleEmptier.StatesInstance, float>)((smi, dt) => smi.Emit(dt))).PlayAnim("working_loop", KAnim.PlayMode.Loop);
            }
        }
    }

}