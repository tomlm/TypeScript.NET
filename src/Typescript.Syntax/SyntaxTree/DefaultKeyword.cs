﻿namespace TypeScript.Syntax
{
    [NodeKindAttribute(NodeKind.DefaultKeyword)]
    public class DefaultKeyword : Node
    {
        #region Properties
        public override NodeKind Kind
        {
            get { return NodeKind.DefaultKeyword; }
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
