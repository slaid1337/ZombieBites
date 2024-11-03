using UnityEngine;

public class ZombieHumanChange : MonoBehaviour
{
    [SerializeField] private GameObject _zombie;
    [SerializeField] private GameObject _human;

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
        }
        else
        {
            _zombie.SetActive(false);
            _human.SetActive(true);

            _zombieScript.enabled = false;
            _humanScript.enabled = true;
        }
    }
}
