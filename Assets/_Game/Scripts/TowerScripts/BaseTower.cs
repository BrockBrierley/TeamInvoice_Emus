using UnityEngine;

public class BaseTower : MonoBehaviour
{

    [Header("Tower Components")]
    [SerializeField] private TargetingComponent targeting;
    [SerializeField] private RotationComponent rotation;
    [SerializeField] private ShootingComponent shooting;

    

    // Update is called once per frame
    void Update()
    {
        //updates the targeting logic
        targeting.UpdateTarget();

        //if theres a valid target, run the rotate/shoot scripts
        if(targeting.CurrentTarget != null)
        {
            rotation.RotateTowards(targeting.CurrentTarget);
            shooting.HandleShooting(targeting.CurrentTarget);
        }

    }
}
