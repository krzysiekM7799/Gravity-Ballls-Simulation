public class ElapsedTimeChecker
{
    private float timeThatMustPass;
    private float lastTimeTriggered;

    public ElapsedTimeChecker(float timeThatMustPass)
    {
        this.timeThatMustPass = timeThatMustPass;
        lastTimeTriggered = -timeThatMustPass;
    }

    public void SetTimeThatMustPass(float timeThatMustPass)
    {
        this.timeThatMustPass = timeThatMustPass;
    }

    public bool CheckElapsedTime()
    {
        if (UnityEngine.Time.time - lastTimeTriggered > timeThatMustPass)
            return true;
        return false;
    }
    public void StartCountTime()
    {
        lastTimeTriggered = UnityEngine.Time.time;
    }

}