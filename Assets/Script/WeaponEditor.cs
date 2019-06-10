using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Weapon))]
public class WeaponEditor : Editor {

    struct weaponInfo
    {
        //info
        [SerializeField]
        public string name;
        [SerializeField]
        public Weapon.eWeaponType type;

        //On Editor
        public int maxAmmo;
        public float cooldownTime;
        public float reloadCooldown;
        public float maxDeviation;
        public float deviationRate;
        public float deviationCooldown;
        public bool bReloading;
        public float shotgunBulletsInTime;
    }


    Weapon weapon;
    List<weaponInfo> list;



    void OnEnable()
    {
        weapon = (Weapon)target;
        //weapon = serializedObject.FindProperty("Weapon");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.LabelField("Add Bullet Type:");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Bullet"), true);
        EditorGUILayout.LabelField("List Weapon: ");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("ListWeapon"), true);

        serializedObject.ApplyModifiedProperties();
    }

    public void UpdateList()
    {
        
    }

}
