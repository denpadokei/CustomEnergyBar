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
        private GameEnergyCounter gameEnergyCounter;
        private GameEnergyUIPanel gameEnergyUIPanel;
        private Image barImage;

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
        }

        /// <summary>
        /// Called when the script is being destroyed.
        /// </summary>
        private void OnDestroy()
        {
            Plugin.Log?.Debug($"{name}: OnDestroy()");
            this.gameEnergyCounter.gameEnergyDidChangeEvent -= this.GameEnergyCounter_gameEnergyDidChangeEvent;
            this.gameEnergyUIPanel = null;
        }
        #endregion

        [Inject]
        private void Constractor(DiContainer container)
        {
            this.gameEnergyCounter = container.TryResolve<GameEnergyCounter>();
            this.gameEnergyUIPanel = Resources.FindObjectsOfTypeAll<GameEnergyUIPanel>().FirstOrDefault();
            this.barImage = gameEnergyUIPanel?.GetField<Image, GameEnergyUIPanel>("_energyBar");
        }

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
