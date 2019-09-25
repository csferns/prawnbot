using System;
using System.Collections.Generic;
using System.Text;

namespace Prawnbot.Core.Exceptions
{
    public class UnexpectedBananaException : Exception
    {
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

        public override string Message => "Its dangerous to code alone, take this: \n" + Discord.Format.Code(ConstantBanana);

        public override string ToString()
        {
            return ConstantBanana;
        }

        public override string StackTrace => ConstantBanana + ConstantBanana + ConstantBanana + ConstantBanana;


    }
}
