public class VarDeclNode : Node
{
    public VarDeclNode(string value, NodeType type):base(value,type)
    {
        
    }
    public override void Accept(IVisitor visitor){
        visitor.Visit(this);
    }
}