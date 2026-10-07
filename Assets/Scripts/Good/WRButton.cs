
public class WRButton : GorillaPressableButton
{
    public WrappedRec wrappedRec;
    public override void ButtonActivation()
    {
        wrappedRec.StartWrappRec();
    }
}
