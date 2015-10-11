namespace GMechanics.FlowchartControl.ItemElements
{
    public interface IItemElement
    {
        ItemElementType ElementType { get; }
        object UserObject { get; set; }
    }
}
