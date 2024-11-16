public class Stamina : Resource
{
    public override void Regen()
    {
        if (!GetComponent<Health>().IsBlocking)
        {
            base.Regen();
        }
    }
}
