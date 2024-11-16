public class ItemSlot
{
    public Item Item => m_Item;
    public int Stack;
    public int Index;

    private Item m_Item;

    public ItemSlot(Item item, int stack, int index)
    {
        m_Item = item;
        Stack = stack;
        Index = index;
    }
}
