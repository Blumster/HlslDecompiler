﻿using System;
using System.IO;

namespace HlslDecompiler
{
    public class AsmWriter
    {
        ShaderModel shader;

        FileStream asmFile;
        StreamWriter asmWriter;

        public AsmWriter(ShaderModel shader)
        {
            this.shader = shader;
        }

        void WriteLine()
        {
            asmWriter.WriteLine();
        }

        void WriteLine(string value)
        {
            asmWriter.WriteLine(value);
        }

        void WriteLine(string format, params object[] args)
        {
            asmWriter.WriteLine(format, args);
        }

        static string ApplyModifier(SourceModifier modifier, string value)
        {
            switch (modifier)
            {
                case SourceModifier.None:
                    return value;
                case SourceModifier.Negate:
                    return string.Format("-{0}", value);
                case SourceModifier.Bias:
                    return string.Format("{0}_bias", value);
                case SourceModifier.BiasAndNegate:
                    return string.Format("-{0}_bias", value);
                case SourceModifier.Sign:
                    return string.Format("{0}_bx2", value);
                case SourceModifier.SignAndNegate:
                    return string.Format("-{0}_bx2", value);
                case SourceModifier.Complement:
                    throw new NotImplementedException();
                case SourceModifier.X2:
                    return string.Format("{0}_x2", value);
                case SourceModifier.X2AndNegate:
                    return string.Format("-{0}_x2", value);
                case SourceModifier.DivideByZ:
                    return string.Format("{0}_dz", value);
                case SourceModifier.DivideByW:
                    return string.Format("{0}_dw", value);
                case SourceModifier.Abs:
                    return string.Format("{0}_abs", value);
                case SourceModifier.AbsAndNegate:
                    return string.Format("-{0}_abs", value);
                case SourceModifier.Not:
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }
        }

        string GetDestinationName(Instruction instruction)
        {
            var resultModifier = instruction.GetDestinationResultModifier();
            if (resultModifier != ResultModifier.None)
            {
                Console.WriteLine(resultModifier);
                //throw new NotImplementedException();
            }

            int destIndex = instruction.GetDestinationParamIndex();

            string registerName = instruction.GetParamRegisterName(destIndex);
            const int registerLength = 4;
            string writeMaskName = instruction.GetDestinationWriteMaskName(registerLength, false);

            return string.Format("{0}{1}", registerName, writeMaskName);
        }

        string GetSourceName(Instruction instruction, int srcIndex)
        {
            string sourceRegisterName = instruction.GetParamRegisterName(srcIndex);
            sourceRegisterName += instruction.GetSourceSwizzleName(srcIndex);
            return ApplyModifier(instruction.GetSourceModifier(srcIndex), sourceRegisterName);
        }

        public void Write(string asmFilename)
        {
            asmFile = new FileStream(asmFilename, FileMode.Create, FileAccess.Write);
            asmWriter = new StreamWriter(asmFile);

            WriteLine("{0}s_{1}_{2}", (shader.Type == ShaderType.Vertex) ? "v" : "p", shader.MajorVersion, shader.MinorVersion);

            foreach (Instruction instruction in shader.Instructions)
            {
                switch (instruction.Opcode)
                {
                    case Opcode.Abs:
                        WriteLine("abs {0}, {1}", GetDestinationName(instruction),
                            GetSourceName(instruction, 1));
                        break;
                    case Opcode.Add:
                        WriteLine("add {0}, {1}, {2}", GetDestinationName(instruction),
                            GetSourceName(instruction, 1), GetSourceName(instruction, 2));
                        break;
                    case Opcode.Cmp:
                        WriteLine("cmp {0}, {1}, {2}, {3}", GetDestinationName(instruction),
                            GetSourceName(instruction, 1), GetSourceName(instruction, 2), GetSourceName(instruction, 3));
                        break;
                    case Opcode.Dcl:
                        string dclInstruction = "dcl";
                        if (instruction.GetParamRegisterType(1) != RegisterType.MiscType)
                        {
                            dclInstruction += "_" + instruction.GetDeclSemantic().ToLower();
                        }
                        WriteLine("{0} {1}", dclInstruction, GetDestinationName(instruction));
                        break;
                    case Opcode.Def:
                        {
                            string constRegisterName = instruction.GetParamRegisterName(0);
                            string constValue0 = instruction.GetParamSingle(1).ToString(System.Globalization.CultureInfo.InvariantCulture);
                            string constValue1 = instruction.GetParamSingle(2).ToString(System.Globalization.CultureInfo.InvariantCulture);
                            string constValue2 = instruction.GetParamSingle(3).ToString(System.Globalization.CultureInfo.InvariantCulture);
                            string constValue3 = instruction.GetParamSingle(4).ToString(System.Globalization.CultureInfo.InvariantCulture);
                            WriteLine("def {0}, {1}, {2}, {3}, {4}", constRegisterName, constValue0, constValue1, constValue2, constValue3);
                        }
                        break;
                    case Opcode.DefI:
                        {
                            string constRegisterName = instruction.GetParamRegisterName(0);
                            WriteLine("defi {0}, {1}, {2}, {3}, {4}", constRegisterName,
                                instruction.Params[1], instruction.Params[2], instruction.Params[3], instruction.Params[4]);
                        }
                        break;
                    case Opcode.DP2Add:
                        WriteLine("dp2add {0}, {1}, {2}, {3}", GetDestinationName(instruction),
                            GetSourceName(instruction, 1), GetSourceName(instruction, 2), GetSourceName(instruction, 3));
                        break;
                    case Opcode.Dp3:
                        WriteLine("dp3 {0}, {1}, {2}", GetDestinationName(instruction),
                            GetSourceName(instruction, 1), GetSourceName(instruction, 2));
                        break;
                    case Opcode.Dp4:
                        WriteLine("dp4 {0}, {1}, {2}", GetDestinationName(instruction),
                            GetSourceName(instruction, 1), GetSourceName(instruction, 2));
                        break;
                    case Opcode.Else:
                        WriteLine("else");
                        break;
                    case Opcode.Endif:
                        WriteLine("endif");
                        break;
                    case Opcode.Exp:
                        WriteLine("exp {0}, {1}", GetDestinationName(instruction),
                            GetSourceName(instruction, 1));
                        break;
                    case Opcode.Frc:
                        WriteLine("frac {0}, {1}", GetDestinationName(instruction), GetSourceName(instruction, 1));
                        break;
                    case Opcode.If:
                        WriteLine("if {0}", GetSourceName(instruction, 0));
                        break;
                    case Opcode.IfC:
                        WriteLine("if_{0} {1}, {2}",
                            ((IfComparison)instruction.Modifier).ToString().ToLower(),
                            GetSourceName(instruction, 0), GetSourceName(instruction, 1));
                        break;
                    case Opcode.Log:
                        WriteLine("log {0}, {1}", GetDestinationName(instruction),
                            GetSourceName(instruction, 1));
                        break;
                    case Opcode.Lrp:
                        WriteLine("lrp {0}, {1}, {2}, {3}", GetDestinationName(instruction),
                            GetSourceName(instruction, 1), GetSourceName(instruction, 2), GetSourceName(instruction, 3));
                        break;
                    case Opcode.Mad:
                        WriteLine("mad {0}, {1}, {2}, {3}", GetDestinationName(instruction),
                            GetSourceName(instruction, 1), GetSourceName(instruction, 2), GetSourceName(instruction, 3));
                        break;
                    case Opcode.Max:
                        WriteLine("max {0}, {1}, {2}", GetDestinationName(instruction),
                            GetSourceName(instruction, 1), GetSourceName(instruction, 2));
                        break;
                    case Opcode.Min:
                        WriteLine("min {0}, {1}, {2}", GetDestinationName(instruction),
                            GetSourceName(instruction, 1), GetSourceName(instruction, 2));
                        break;
                    case Opcode.Mov:
                        WriteLine("mov {0}, {1}", GetDestinationName(instruction), GetSourceName(instruction, 1));
                        break;
                    case Opcode.MovA:
                        WriteLine("mova {0}, {1}", GetDestinationName(instruction), GetSourceName(instruction, 1));
                        break;
                    case Opcode.Mul:
                        WriteLine("mul {0}, {1}, {2}", GetDestinationName(instruction),
                            GetSourceName(instruction, 1), GetSourceName(instruction, 2));
                        break;
                    case Opcode.Nop:
                        WriteLine("nop");
                        break;
                    case Opcode.Nrm:
                        WriteLine("nrm {0}, {1}", GetDestinationName(instruction), GetSourceName(instruction, 1));
                        break;
                    case Opcode.Pow:
                        WriteLine("pow {0}, {1}, {2}", GetDestinationName(instruction),
                            GetSourceName(instruction, 1), GetSourceName(instruction, 2));
                        break;
                    case Opcode.Rcp:
                        WriteLine("rcp {0}, {1}", GetDestinationName(instruction), GetSourceName(instruction, 1));
                        break;
                    case Opcode.Rsq:
                        WriteLine("rsq {0}, {1}", GetDestinationName(instruction), GetSourceName(instruction, 1));
                        break;
                    case Opcode.Sge:
                        WriteLine("sge {0}, {1}, {2}", GetDestinationName(instruction),
                            GetSourceName(instruction, 1), GetSourceName(instruction, 2));
                        break;
                    case Opcode.Slt:
                        WriteLine("slt {0}, {1}, {2}", GetDestinationName(instruction),
                            GetSourceName(instruction, 1), GetSourceName(instruction, 2));
                        break;
                    case Opcode.SinCos:
                        if (shader.MajorVersion >= 3)
                        {
                            WriteLine("sincos {0}, {1}", GetDestinationName(instruction),
                                GetSourceName(instruction, 1));
                        }
                        else
                        {
                            WriteLine("sincos {0}, {1}, {2}, {3}", GetDestinationName(instruction),
                                GetSourceName(instruction, 1), GetSourceName(instruction, 2), GetSourceName(instruction, 3));
                        }
                        break;
                    case Opcode.Sub:
                        WriteLine("sub {0}, {1}, {2}", GetDestinationName(instruction),
                            GetSourceName(instruction, 1), GetSourceName(instruction, 2));
                        break;
                    case Opcode.Tex:
                        if ((shader.MajorVersion == 1 && shader.MinorVersion >= 4) || (shader.MajorVersion > 1))
                        {
                            WriteLine("texld {0}, {1}, {2}", GetDestinationName(instruction),
                                GetSourceName(instruction, 1), GetSourceName(instruction, 2));
                        }
                        else
                        {
                            WriteLine("tex {0}", GetDestinationName(instruction));
                        }
                        break;
                    case Opcode.TexLDL:
                        WriteLine("texldl {0}, {1}, {2}", GetDestinationName(instruction),
                            GetSourceName(instruction, 1), GetSourceName(instruction, 2));
                        break;
                    case Opcode.Comment:
                    case Opcode.End:
                        break;
                    default:
                        WriteLine(instruction.Opcode.ToString());
                        Console.WriteLine(instruction.Opcode);
                        //throw new NotImplementedException();
                        break;
                }
            }

            asmWriter.Dispose();
            asmFile.Dispose();
        }
    }
}
