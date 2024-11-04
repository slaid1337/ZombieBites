using UnityEngine;

public class ZombieHumanChange : MonoBehaviour
{
    [SerializeField] private GameObject _zombie;
    [SerializeField] private GameObject _human;

    [SerializeField] private Collider _zombieCollider;
    [SerializeField] private Collider _humanCollider;

    private ZombieBehaviour _zombieScript;
    private HumanBehaviour _humanScript;

    private void Awake()
    {
        _zombieScript = GetComponent<ZombieBehaviour>();
        _humanScript = GetComponent<HumanBehaviour>();
    }

    public void SwitchState(bool isZombie)
    {
        if (isZombie)
        {
            _zombie.SetActive(true);
            _human.SetActive(false);

            _zombieScript.enabled = true;
            _humanScript.enabled = false;

            _zombieCollider.enabled = true;
            _humanCollider.enabled = false;

            HumanPool.Instance.RemoveHuman(_humanScript);
        }
        else
        {
            _zombie.SetActive(false);
            _human.SetActive(true);

            _zombieScript.enabled = false;
            _humanScript.enabled = true;

            _zombieCollider.enabled = false;
            _humanCollider.enabled = true;

            HumanPool.Instance.AddHuman(_humanScript);
        }
    }
}
