using System;

namespace VirtualMachine
{
    public interface IAssembler
    {
        public void LoadA(String symbol);
        public void assignA(String symbol);
        public void assignD(String symbol);
        public void assignAM(String symbol);
        public void assignMD(String symbol);
        public void assignAD(String symbol);
        public void assignAMD(String symbol);
    }
}