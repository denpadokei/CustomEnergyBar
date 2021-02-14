using IPA.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
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
        private GameEnergyCounter gameEnergyCounter;
        
        private GameObject _energyPanelGO;
        private Material fullIconMaterial;
        private Image barImage;
        private Image fullIcon;
        private Color DefaltColor;
        [Inject]
        private void Constractor(GameplayCoreSceneSetupData gameplayCoreSceneSetupData, GameEnergyCounter gameEnergyCounter)
        {
            this.gameEnergyCounter = gameEnergyCounter;
            
            this.coreGameHUDController = Resources.FindObjectsOfTypeAll<CoreGameHUDController>().FirstOrDefault();
            this._energyPanelGO = this.coreGameHUDController.GetField<GameObject, CoreGameHUDController>("_energyPanelGO");
            foreach (var item in _energyPanelGO.GetComponentsInChildren<Image>().OrderBy(x => x.name)) {
                Plugin.Log.Debug($"{item}");
                if (item == null) {
                    continue;
                }
                if (item.name == "EnergyBar") {
                    this.barImage = item;
                }
                else if (item.name == "EnergyIconFull") {
                    this.fullIcon = item;
                    this.DefaltColor = item.color;
                }
                else if (item.name == "EnergyIconEmpty") {
                    item.color = Color.red;
                }
            }
            var difficultyBeatmap = gameplayCoreSceneSetupData.difficultyBeatmap;
            if (0 < difficultyBeatmap.beatmapData.spawnRotationEventsCount) {
                this.barImage.rectTransform.sizeDelta = new Vector2(0f, 0.7f);
            }
            this.fullIconMaterial = Instantiate(this.fullIcon.material);
            this.fullIcon.material = this.fullIconMaterial;
        }
        #region Monobehaviour Messages
        /// <summary>
        /// Only ever called once, mainly used to initialize variables.
        /// </summary>
        private void Awake()
        {
            Plugin.Log?.Debug($"{name}: Awake()");
        }

        private void Start()
        {
            if (this.gameEnergyCounter == null) {
                return;
            }
            this.gameEnergyCounter.gameEnergyDidChangeEvent += this.GameEnergyCounter_gameEnergyDidChangeEvent;
            this.GameEnergyCounter_gameEnergyDidChangeEvent(this.gameEnergyCounter.energy);
            // 光るシェーダー
            //GUI/Text Shader
            //Sprites / Default
            //TextMeshPro / Mobile / Bitmap
            //UI / Default
            this.fullIcon.material.shader = Resources.FindObjectsOfTypeAll<Shader>().FirstOrDefault(x => x.name == "GUI/Text Shader");
        }

        /// <summary>
        /// Called when the script is being destroyed.
        /// </summary>
        private void OnDestroy()
        {
            Plugin.Log?.Debug($"{name}: OnDestroy()");
            this.gameEnergyCounter.gameEnergyDidChangeEvent -= this.GameEnergyCounter_gameEnergyDidChangeEvent;
            Destroy(this.fullIconMaterial);
            this.fullIconMaterial = null;
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
                if (1 <= obj) {
                    this.fullIcon.color = Color.yellow;
                    
                }
                else if (this.fullIcon.color != this.DefaltColor) {
                    this.fullIcon.color = this.DefaltColor;
                    
                }
            }
#if DEBUG
            if (this.fullIcon.color != this.DefaltColor) {
                this.fullIcon.color = this.DefaltColor;
            }
            else {
                this.fullIcon.color = Color.yellow;
            }
#endif
        }
    }
}
