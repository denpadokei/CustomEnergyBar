using IPA.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CustomEnergyBar
{
    /// <summary>
    /// Monobehaviours (scripts) are added to GameObjects.
    /// For a full list of Messages a Monobehaviour can receive from the game, see https://docs.unity3d.com/ScriptReference/MonoBehaviour.html.
    /// </summary>
    public class CustomEnergyBarController : MonoBehaviour
    {
        // These methods are automatically called by Unity, you should remove any you aren't using.
        private CoreGameHUDController coreGameHUDController;
        [Inject]
        private GameEnergyCounter gameEnergyCounter;
        [Inject]
        private GameplayCoreSceneSetupData gameplayCoreSceneSetupData;
        private GameObject _energyPanelGO;
        private Image barImage;
        #region Monobehaviour Messages
        /// <summary>
        /// Only ever called once, mainly used to initialize variables.
        /// </summary>
        private void Awake()
        {
            Plugin.Log?.Debug($"{name}: Awake()");
            this.coreGameHUDController = Resources.FindObjectsOfTypeAll<CoreGameHUDController>().FirstOrDefault();
            this._energyPanelGO = this.coreGameHUDController.GetField<GameObject, CoreGameHUDController>("_energyPanelGO");

            foreach (var item in _energyPanelGO.GetComponentsInChildren<Image>()) {
                if (item == null) {
                    continue;
                }
                if (item.name == "EnergyBar") {
                    this.barImage = item;
                }
            }
            var beatmap = this.gameplayCoreSceneSetupData.difficultyBeatmap;
            if (0 < beatmap.beatmapData.spawnRotationEventsCount) {
                this.barImage.rectTransform.sizeDelta = new Vector2(0f, 0.7f);
            }
        }

        private void Start()
        {
            if (this.gameEnergyCounter == null) {
                return;
            }
            this.gameEnergyCounter.gameEnergyDidChangeEvent += this.GameEnergyCounter_gameEnergyDidChangeEvent;
            this.GameEnergyCounter_gameEnergyDidChangeEvent(this.gameEnergyCounter.energy);
        }

        /// <summary>
        /// Called when the script is being destroyed.
        /// </summary>
        private void OnDestroy()
        {
            Plugin.Log?.Debug($"{name}: OnDestroy()");
            this.gameEnergyCounter.gameEnergyDidChangeEvent -= this.GameEnergyCounter_gameEnergyDidChangeEvent;
        }
        #endregion
        private void GameEnergyCounter_gameEnergyDidChangeEvent(float obj)
        {
            if (barImage == null) {
                Plugin.Log.Debug("UIPanel is null!");
                return;
            }

            if (obj < 0.2f) {
                barImage.color = Color.red;
            }
            else if (obj < 0.5f) {
                barImage.color = Color.yellow;
            }
            else {
                barImage.color = Color.green;
            }
        }
    }
}
