using UnityEngine;
using System.Collections;
using UnityEditor;

public class BattleSimulator : EditorWindow
{
	[MenuItem("Window/Battle Simulator")]
    static void ShowWindow () {
        EditorWindow.GetWindow (typeof(BattleSimulator));
    }

	float m_Damage = 0;
  	float m_AttackerHealth = 100.0f;
  	int m_Attack = 0;
  	int m_Defence = 0;
  	int m_DefStars = 0;
  	float m_AttackModifier = 1.0f;
  	float m_DefenceModifier = 1.0f;
	
	void OnGUI ()
	{
		EditorGUILayout.TextField("Damage: " + m_Damage*100.0f + "%");
		m_AttackerHealth = EditorGUILayout.FloatField("Attacker Health(0-100): ",m_AttackerHealth);
		
		m_Attack = EditorGUILayout.IntField("Attack: ",m_Attack);
		m_Defence = EditorGUILayout.IntField("Defence: ",m_Defence);
		m_DefStars = EditorGUILayout.IntField("Def Stars (+.05def/star): ",m_DefStars);
		
		m_AttackModifier = EditorGUILayout.FloatField("Att Modifier(0-x): ",m_AttackModifier);
		m_DefenceModifier = EditorGUILayout.FloatField("Def Modifier(0-x): ",m_DefenceModifier);
		
		if(GUILayout.Button("Simulate"))
		{
			Simulate(out m_Damage
				  	,m_AttackerHealth
				  	,m_Attack
				  	,m_Defence
				  	,m_DefStars
				  	,m_AttackModifier
				  	,m_DefenceModifier);
		}
	}
	
	void Simulate(out float damage
	              , float attackerHealth
	              , int attack
	              , int defence
	              , int defStars
	              , float attackModifier
	              , float defenceModifier )
	{
		attackModifier = Mathf.Max(0,attackModifier);
		defenceModifier = Mathf.Max(0,defenceModifier);
			
		float realAttack = (float)attack * attackModifier * (attackerHealth/100.0f);
		float defStarPower = 0.05f;
		float realDef = (float)defence * defenceModifier * (1.0f + defStars*defStarPower );
		
		damage = .5f + (realAttack-realDef)*.05f;
		damage = Mathf.Max(0,damage);
	}
}
