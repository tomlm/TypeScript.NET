namespace TypeScript.Syntax
{
    [NodeKindAttribute(NodeKind.NumberKeyword)]
    public class NumberKeyword : Node
    {
        #region Properties
        public override NodeKind Kind
        {
            get { return NodeKind.NumberKeyword; }
        }
        #endregion

        public override void AddChild(Node childNode)
        {
            base.AddChild(childNode);

            string nodeName = childNode.NodeName;
            switch (nodeName)
            {
                default:
                    this.ProcessUnknownNode(childNode);
                    break;
            }
        }
    }
}
