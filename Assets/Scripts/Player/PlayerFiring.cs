﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerFiring : MonoBehaviour {

    public int energyPoints;
    public float firingDelay = .1f;
    public float bulletSpeed = 50;
    public Slider energySlider;

    private float nextFiringTime;
    private StackPool bulletPool;

    private Weapon leftGun;
    private Weapon rightGun;
    private List<Weapon> weapons;

    private int currentRightWeaponIndex = 0;

	void Awake () {
        nextFiringTime = 0;
        bulletPool = GameObject.Find("BulletPool").GetComponent<StackPool>();
        SkillTree skillTree = GetComponent<SkillTree>();

        leftGun = new MachineGun(skillTree);
        rightGun = new EnergyGun(skillTree);
        weapons = new List<Weapon>();
	}
	
	// Update is called once per frame
	void Update () {
        // 0 = left, 1 = right, 2 = middle
        if (Input.GetMouseButton(0))
        {
            leftGun.Fire(transform);
        }

        if (Input.GetMouseButton(1))
        {
            if (energyPoints >= rightGun.GetEnergyCost() && rightGun.Fire(transform))
            {
                energyPoints -= rightGun.GetEnergyCost();
            }
        }

        energySlider.value = energyPoints;
	}

    public void AddWeapon(Weapon newWeapon)
    {
        weapons.Add(newWeapon);
    }

    public void ToggleRightWeapon()
    {
        currentRightWeaponIndex = currentRightWeaponIndex == weapons.Count - 1 ? 0 : currentRightWeaponIndex;

        rightGun = weapons[currentRightWeaponIndex];
    }
}