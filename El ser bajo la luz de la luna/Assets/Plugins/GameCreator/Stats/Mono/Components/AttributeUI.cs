namespace GameCreator.Stats
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using GameCreator.Core;

    [AddComponentMenu("UI/Game Creator/Attribute UI", 0)]
	public class AttributeUI : MonoBehaviour
    {
        public TargetGameObject target = new TargetGameObject(TargetGameObject.Target.Player);

        [AttributeSelector]
        public AttrAsset attribute;

        public Image icon;
        public Graphic color;
        public Text title;
        public Text description;
        public Text shortName;

        [Tooltip("{0}: The current value of the Attribute\n{1}: The maximum value or stat value")]
        public string valueFormat = "{0}/{1}";
        public Text value;

        public Image valueFillImage;
        public RectTransform valueScaleX;
        public RectTransform valueScaleY;

        private bool exitingApplication = false;

        // INITIALIZERS: --------------------------------------------------------------------------

        private void Start()
        {
            Stats stats = this.GetStatsTarget();
            if (stats == null) return;

            stats.AddOnChangeAttr(this.UpdateAttrUI);
            this.UpdateAttrUI(null);
        }

        private void OnDestroy()
        {
            if (this.exitingApplication) return;

            Stats stats = this.GetStatsTarget();
            if (stats == null) return;

            stats.RemoveOnChangeAttr(this.UpdateAttrUI);
        }

        private void OnApplicationQuit()
        {
            this.exitingApplication = true;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private Stats GetStatsTarget()
        {
            if (this.attribute == null) return null;

            GameObject targetGO = this.target.GetGameObject(gameObject);
            if (targetGO == null) return null;

            Stats stats = targetGO.GetComponentInChildren<Stats>(true);
            if (stats == null) return null;

            return stats;
        }

        public void UpdateAttrUI()
        {
            this.UpdateAttrUI(null);
        }

        private void UpdateAttrUI(Stats.EventArgs args)
        {
            Stats stats = this.GetStatsTarget();
            if (stats == null) return;

            string attrID = this.attribute.attribute.uniqueName;

            if (this.icon != null) this.icon.overrideSprite = stats.GetAttrIcon(attrID);
            if (this.color != null) this.color.color = stats.GetAttrColor(attrID);
            if (this.title != null) this.title.text = stats.GetAttrDescription(attrID);
            if (this.description != null) this.description.text = stats.GetAttrDescription(attrID);
            if (this.shortName != null) this.shortName.text = stats.GetAttrShortName(attrID);

            float curAttr = stats.GetAttrValue(attrID);
            float maxAttr = stats.GetAttrMaxValue(attrID);

            if (this.value != null) this.value.text = string.Format(
                this.valueFormat,
                curAttr,
                maxAttr
            );

            if (this.valueFillImage != null) this.valueFillImage.fillAmount = (curAttr / maxAttr);

            if (this.valueScaleX != null) this.valueScaleX.localScale = new Vector3(
                (curAttr / maxAttr),
                this.valueScaleX.localScale.y,
                this.valueScaleX.localScale.z
            );

            if (this.valueScaleY != null) this.valueScaleY.localScale = new Vector3(
                this.valueScaleY.localScale.x,
                (curAttr / maxAttr),
                this.valueScaleY.localScale.z
            );
        }
    }
}
