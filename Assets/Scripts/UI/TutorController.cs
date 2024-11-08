using UnityEngine;

public class TutorController : Singletone<TutorController>
{
    [SerializeField] GameObject[] _states;
    [SerializeField] private GameObject _closeBtn;

    private int _step;

    private void Start()
    {
        NextStep();
    }

    public void NextStep()
    {
        if (_step >= _states.Length) return;

        FadeBack.Instance.Open();

        LevelController.Instance.StopPlay();

        _states[_step].SetActive(true);
        _closeBtn.SetActive(true);

        _step++;
    }

    public void CloseTip()
    {
        _states[_step - 1].SetActive(false);
        _closeBtn.SetActive(false);

        FadeBack.Instance.Close();

        LevelController.Instance.ResumePlay();
    }
}
