using System;

namespace Prawnbot.Core.Custom.Exceptions
{
    public class UnexpectedBananaException : Exception
    {
        public UnexpectedBananaException() : base (null, null)
        {

        }

        public UnexpectedBananaException(string message) : base (message)
        {

        }

        public UnexpectedBananaException(string message, Exception innerException) : base (message, innerException)
        {

        }

        private const string ConstantBanana = @"
                _
                //\
                V  \
                \  \_
                \,'.`-.
                |\ `. `.       
                ( \  `. `-.                        _,.-:\
                    \ \   `.  `-._             __..--' ,-';/
                    \ `.   `-.   `-..___..---'   _.--' ,'/
                    `. `.    `-._        __..--'    ,' /
                        `. `-_     ``--..''       _.-' ,'
                        `-_ `-.___        __,--'   ,'
                            `-.__  `----'''    __.-'
                                `--..____..--'

                ";

        public override string HelpLink
        {
            get => ConstantBanana; 
            set => value = ConstantBanana; 
        }

        public override string Message => "Its dangerous to code alone, take this: \n" + ConstantBanana;

        public override string ToString()
        {
            return ConstantBanana;
        }

        public override string StackTrace => ConstantBanana + ConstantBanana + ConstantBanana + ConstantBanana;
    }
}
