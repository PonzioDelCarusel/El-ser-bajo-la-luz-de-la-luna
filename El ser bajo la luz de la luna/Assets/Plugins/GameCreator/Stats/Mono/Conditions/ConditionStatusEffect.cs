namespace GameCreator.Stats
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using GameCreator.Core;
    using GameCreator.Variables;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
    public class ConditionStatusEffect : ICondition
	{
        public TargetGameObject target = new TargetGameObject(TargetGameObject.Target.Player);

        [StatusEffectSelector]
        public StatusEffectAsset statusEffect;

		// EXECUTABLE: ----------------------------------------------------------------------------

		public override bool Check(GameObject target)
		{
            GameObject targetGO = this.target.GetGameObject(target);
            if (targetGO == null)
            {
                Debug.LogError("Condition Status Effect: No target defined");
                return false;
            }

            Stats stats = targetGO.GetComponentInChildren<Stats>();
            if (stats == null)
            {
                Debug.LogError("Condition Status Effect: Could not get Stats component in target");
                return false;
            }

            if (stats.HasStatusEffect(this.statusEffect))
            {
                return true;
            }

            return false;
		}

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

        public const string CUSTOM_ICON_PATH = "Assets/Plugins/GameCreator/Stats/Icons/Conditions/";

		public static new string NAME = "Stats/Status Effect";
        private const string NODE_TITLE = "Has {0} status effect {1}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spTarget;
        private SerializedProperty spStef;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
            string statName = (this.statusEffect == null 
                ? "(none)" 
                : this.statusEffect.statusEffect.uniqueName
            );

            return string.Format(
                NODE_TITLE,
                this.target.ToString(),
                statName
            );
		}

		protected override void OnEnableEditorChild ()
		{
            this.spTarget = this.serializedObject.FindProperty("target");
            this.spStef = this.serializedObject.FindProperty("statusEffect");
		}

		protected override void OnDisableEditorChild ()
		{
            this.spTarget = null;
            this.spStef = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spTarget);
            EditorGUILayout.PropertyField(this.spStef);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}
