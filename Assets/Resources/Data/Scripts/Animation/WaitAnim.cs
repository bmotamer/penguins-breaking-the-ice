/// <summary>
/// Wait animation
/// </summary>
public class WaitAnim : Anim<float>
{
	
	public override void UpdateValue()
	{
		ValueRaw = ProgressRaw * Duration;
		Value    = Progress * Duration;
	}
	
}