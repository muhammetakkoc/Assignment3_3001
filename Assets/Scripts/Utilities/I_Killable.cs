/// <summary>
/// To be extended by anything that can receive damage or be destroyed by attacks
/// </summary>
public interface I_Killable
{
    public void DealDamage(int damage);
    public void Kill();
}
