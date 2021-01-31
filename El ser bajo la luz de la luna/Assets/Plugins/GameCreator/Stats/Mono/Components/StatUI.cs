namespace GameCreator.Stats
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using GameCreator.Core;

    [AddComponentMenu("UI/Game Creator/Stat UI", 0)]
    public class StatUI : MonoBehaviour
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        public TargetGameObject target = new TargetGameObject(TargetGameObject.Target.Player);

        [StatSelector]
        public StatAsset stat;

        public Image icon;
        public Graphic color;
        public Text title;
        public Text description;
        public Text shortName;

        public Text value;
        public Image imageFill;

        private bool exitingApplication = false;

        // INITIALIZERS: --------------------------------------------------------------------------

        private void Start()
        {
            Stats stats = this.GetStatsTarget();
            if (stats == null) return;

            stats.AddOnChangeStat(this.UpdateStatUI);
            this.UpdateStatUI(null);
        }

        private void OnDestroy()
        {
            if (this.exitingApplication) return;

            Stats stats = this.GetStatsTarget();
            if (stats == null) return;

            stats.RemoveOnChangeStat(this.UpdateStatUI);
        }

        private void OnApplicationQuit()
        {
            this.exitingApplication = true;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private Stats GetStatsTarget()
        {
            if (this.stat == null) return null;

            GameObject targetGO = this.target.GetGameObject(gameObject);
            if (targetGO == null) return null;

            Stats stats = targetGO.GetComponentInChildren<Stats>(true);
            if (stats == null) return null;

            return stats;
        }

        private void UpdateStatUI(Stats.EventArgs args)
        {
            Stats stats = this.GetStatsTarget();
            if (stats == null) return;

            string statID = this.stat.stat.uniqueName;

            if (this.icon != null) this.icon.overrideSprite = stats.GetStatIcon(statID);
            if (this.color != null) this.color.color = stats.GetStatColor(statID);
            if (this.title != null) this.title.text = stats.GetStatTitle(statID);
            if (this.description != null) this.description.text = stats.GetStatDescription(statID);
            if (this.shortName != null) this.shortName.text = stats.GetStatShortName(statID);

            if (this.value != null) this.value.text = stats.GetStat(statID, null).ToString();
            if (this.imageFill != null) this.imageFill.fillAmount = stats.GetStat(statID, null);
        }
    }
}