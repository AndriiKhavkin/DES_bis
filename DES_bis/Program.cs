 using System;
 using System.Collections;
 using System.Linq;
 using System.Text;

 namespace DES_bis
 {
     class Program
     {
        static void Main()
        { 
            int[] text1 = new[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF };
            int[] key1 = new[] { 0xFE, 0xFE, 0xFE, 0xFE, 0xFE, 0xFE, 0xFE, 0xFE };
            int[] code1 = new[] { 0x6D, 0xCE, 0x0D, 0xC9, 0x00, 0x65, 0x56, 0xA3 };

            int[] text2 = new[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            int[] key2 = new[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            int[] code2 = new[] { 0x8C, 0xA6, 0x4D, 0xE9, 0xC1, 0xB1, 0x23, 0xA7 };

            int[] text3 = new[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF };
            int[] key3 = new[] { 0xFE, 0xDC, 0xBA, 0x98, 0x76, 0x54, 0x32, 0x10 };
            int[] code3 = new[] { 0xED, 0x39, 0xD9, 0x50, 0xFA, 0x74, 0xBC, 0xC4 };
            
            BitArray block64_1 = DES.IntToBitArray(text1, 64);
            BitArray key64_1 = DES.IntToBitArray(key1, 64);
            BitArray test64_1 = DES.IntToBitArray(code1, 64);
            BitArray result1 = DES.Des(block64_1, key64_1);

            BitArray block64_2 = DES.IntToBitArray(text2, 64);
            BitArray key64_2 = DES.IntToBitArray(key2, 64);
            BitArray test64_2 = DES.IntToBitArray(code2, 64);
            BitArray result2 = DES.Des(block64_2, key64_2);

            BitArray block64_3 = DES.IntToBitArray(text3, 64);
            BitArray key64_3 = DES.IntToBitArray(key3, 64);
            BitArray test64_3 = DES.IntToBitArray(code3, 64);
            BitArray result3 = DES.Des(block64_3, key64_3);

            for (int i = 0; i < 64; i++)
            {
                test64_1[i] = test64_1[63 - i];
                test64_2[i] = test64_2[63 - i];
                test64_3[i] = test64_3[63 - i];
            }
            
            Console.WriteLine("Test 1");
            Console.Write("Text:");
            DES.PrintHex(block64_1);
            Console.Write("\nKey:");
            DES.PrintHex(key64_1);
            Console.Write("\nResult:");
            DES.PrintHexXReverse(result1);
            Console.Write("\nExpected:");
            DES.PrintHexXReverse(test64_1);

            Console.WriteLine("\n\nTest 2");
            Console.Write("Text:");
            DES.PrintHex(block64_2);
            Console.Write("\nKey:");
            DES.PrintHex(key64_2);
            Console.Write("\nResult:");
            DES.PrintHexXReverse(result2);
            Console.Write("\nExpected:");
            DES.PrintHexXReverse(test64_2);
            
            Console.WriteLine("\n\nTest 3");
            Console.Write("Text:");
            DES.PrintHex(block64_3);
            Console.Write("\nKey:");
            DES.PrintHex(key64_3);
            Console.Write("\nResult:");
            DES.PrintHexXReverse(result3);
            Console.Write("\nExpected:");
            DES.PrintHexXReverse(test64_3);

            char[] password = new[] { 'p', 'a', 's', 's', 'w', 'o', 'r', 'd' };
            char[] name = new[] { 'K', 'h', 'a', 'v', 'k', 'i', 'n', 'A' };
            int[] intPassword = new int[8];
            int[] intName = new int[8];

            for (int i = 0; i < 8; i++)
            {
                intPassword[i] = Convert.ToInt32(password[i]);
                intName[i] = Convert.ToInt32(name[i]);
            }

            BitArray bitOfPassword = DES.IntToBitArray(intPassword, 64);
            BitArray bitOfName = DES.IntToBitArray(intName, 64);
            BitArray result = DES.EncryptionDES(bitOfName, bitOfPassword);

            Console.WriteLine("\n\nTest 4");
            Console.Write("Name:");
            DES.PrintHex(bitOfName);
            Console.Write("\nPassword:");
            DES.PrintHex(bitOfPassword);
            Console.Write("\nResult:");
            DES.PrintHexXReverse(result);
            Console.WriteLine("\nPress enter to exit...");
            Console.ReadLine();
        }
        
       
     }
 }
 