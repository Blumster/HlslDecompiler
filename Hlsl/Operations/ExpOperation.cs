namespace HlslDecompiler.Hlsl
{
    public class ExpOperation : Operation
    {
        public ExpOperation(HlslTreeNode value)
        {
            AddInput(value);
        }

        public override string Mnemonic => "exp";
    }
}
