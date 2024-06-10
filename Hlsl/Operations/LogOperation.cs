namespace HlslDecompiler.Hlsl
{
    public class LogOperation : Operation
    {
        public LogOperation(HlslTreeNode value)
        {
            AddInput(value);
        }

        public override string Mnemonic => "log";
    }
}
