using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Shooter : MonoBehaviour
{
    [SerializeField] private Projectile projectilePrefab = null;
    [SerializeField] private Transform gun = null;
    [SerializeField] private AttackerSpawner myLaneSpawner = null;
    [SerializeField] private Animator animator = null;
    private const string PROJECTILE_PARENT_NAME = "Projectiles";
    private static GameObject projectileParent = null;
    private static bool projectileParentCreated = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        SetLane();
    }

    public void Fire()
    {
        Projectile projectile = Instantiate(projectilePrefab, gun.position, Quaternion.identity, transform) as Projectile;
        HookUpProjectileParent(projectile);
    }

    private void HookUpProjectileParent(Projectile projectile)
    {
        CreateProjectileParent();
        projectile.transform.parent = projectileParent.transform;
    }

    private void CreateProjectileParent()
    {
        if (!projectileParentCreated || !projectileParent)
        {
            projectileParentCreated = true;
            projectileParent = new GameObject(PROJECTILE_PARENT_NAME);
        }
    }

    private void SetLane()
    {
        AttackerSpawner[] spawners = FindObjectsOfType<AttackerSpawner>();
        for(int spawnerID = 0; spawnerID < spawners.Length; spawnerID++)
        {
            if (spawners[spawnerID].transform.position.y == transform.position.y)
            {
                myLaneSpawner = spawners[spawnerID];
                return;
            }
        }
    }

    private bool IsAttackerInLane()
    {
        return myLaneSpawner.transform.childCount <= 0 ? false : true;
    }

    private void Update()
    {
        SetAnimator();
    }

    private void SetAnimator()
    {
        if (IsAttackerInLane())
        {
            animator.SetBool("isShooting", true);
        }
        else
        {
            animator.SetBool("isShooting", false);
        }
    }
}
