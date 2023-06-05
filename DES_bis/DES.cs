using System;
using System.Collections;

namespace DES_bis
{
    public class DES
    {
         public static void PrintHex(BitArray bitArray)
         {
             int[] result = ConvertBitArrayToIntegers(ReverseBitArray(bitArray));
             Console.WriteLine();
             foreach (int value in result)
             {
                 Console.Write("0x" + value.ToString("X2") + " "); // Print as 0x**
             }
         }
         
         public static void PrintHexXReverse(BitArray bitArray)
         {
             int[] result = ConvertBitArrayToIntegers(bitArray);
             Console.WriteLine();
             foreach (int value in result)
             {
                 Console.Write("0x" + value.ToString("X2") + " "); // Print as 0x**
             }
         }
         public static BitArray ReverseBitArray(BitArray bitArray)
         {
             int length = bitArray.Length;
             BitArray reversedArray = new BitArray(length);

             for (int i = 0; i < length; i++)
             {
                 reversedArray[length - 1 - i] = bitArray[i];
             }

             return reversedArray;
         }

         public static int[] ConvertBitArrayToIntegers(BitArray bitArray)
         {
             if (bitArray.Length % 8 != 0)
             {
                 throw new ArgumentException("BitArray length must be a multiple of 8.");
             }

             int[] result = new int[bitArray.Length / 8];
             int currentIndex = 0;

             for (int i = 0; i < bitArray.Length; i += 8)
             {
                 int value = 0;
                 for (int j = 0; j < 8; j++)
                 {
                     if (bitArray[i + j])
                     {
                         value |= (1 << (7 - j));
                     }
                 }
                 result[currentIndex] = value;
                 currentIndex++;
             }

             return result;
         }

        public static BitArray Des(BitArray block_64, BitArray key_64)
         {
             BitArray block64Bits = new BitArray(64);
             BitArray key64Bits = new BitArray(64);

             for (int i = 0; i < 64; i++)
             {
                 key64Bits[i] = key_64[63 - i];
                 block64Bits[i] = block_64[63 - i];
             }

             block64Bits = InitialPermutation(block64Bits);

             int[] shift = new[] { 1, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1 };
             BitArray[] CD = key0CD(key64Bits);

             BitArray key48Bits = new BitArray(48);

             for (int i = 0; i < 16; i++)
             {
                 for (int j = 0; j < shift[i]; j++)
                 {
                     CD[0] = ShiftToLeft(CD[0]);
                     CD[1] = ShiftToLeft(CD[1]);
                 }

                 key48Bits = KeyFromCD(CD[1], CD[0]);
                 block64Bits = EncryptionDES(block64Bits, key48Bits);
             }
             BitArray left = GetLeft(block64Bits);
             BitArray right = GetRight(block64Bits);
             block64Bits = JoinLeftRight(right, left);

             block64Bits = FinalPermutation(block64Bits);

             return block64Bits;
         }

        private static BitArray GetLeft(BitArray block64)
         {
             BitArray resultLeft = new BitArray(32);
             for (int i = 0; i < 32; i++)
             {
                 resultLeft[i] = block64[i];
             }

             return resultLeft;
         }

        private static BitArray GetRight(BitArray block64)
         {
             BitArray resultRight = new BitArray(32);
             for (int i = 0; i < 32; i++)
             {
                 resultRight[i] = block64[i + 32];
             }

             return resultRight;
         }

        private static BitArray JoinLeftRight(BitArray left, BitArray right)
         {
             BitArray resultJoin = new BitArray(64);
             for (int i = 0; i < 32; i++)
             {
                 resultJoin[i + 32] = right[i];
                 resultJoin[i] = left[i];
             }

             return resultJoin;
         }

        public static BitArray EncryptionDES(BitArray block64, BitArray key48)
         {
             BitArray left = GetLeft(block64);
             BitArray right = GetRight(block64);

             BitArray F = Function(right, key48);

             for (int i = 0; i < 32; i++)
             {
                 left[i] = F[i] ^ left[i];
             }

             block64 = JoinLeftRight(right, left);

             return block64;
         }

        private static BitArray[] key0CD(BitArray key_64)
         {
             BitArray key64 = change_8ths(key_64);

             BitArray C0 = new BitArray(28);
             BitArray D0 = new BitArray(28);

             int[] valuesOfC0 = new[] { 57, 49, 41, 33, 25, 17, 9, 1, 58, 50, 42, 34, 26, 18, 10, 2, 59, 51, 43, 35, 27, 19, 11, 3, 60, 52, 44, 36 };
             int[] valuesOfD0 = new[] { 63, 55, 47, 39, 31, 23, 15, 7, 62, 54, 46, 38, 30, 22, 14, 6, 61, 53, 45, 37, 29, 21, 13, 5, 28, 20, 12, 4 };

             for (int i = 0; i < 28; i++)
             {
                 C0[i] = key64[valuesOfC0[i] - 1];
                 D0[i] = key64[valuesOfD0[i] - 1];
             }
             return new BitArray[] { C0, D0 };
         }

        private static BitArray UniteCD(BitArray C, BitArray D)
         {
             BitArray result = new BitArray(56);

             for (int i = 0; i < 28; i++)
             {
                 result[i] = D[i];
                 result[i + 28] = C[i];
             }

             return result;
         }

        private static BitArray KeyFromCD(BitArray C, BitArray D)
         {
             BitArray key = new BitArray(48);
             BitArray CD = UniteCD(C, D);

             int[] keyTransformation = new[] {
                 14, 17, 11, 24, 1, 5, 3, 28,
                 15, 6, 21, 10, 23, 19, 12, 4,
                 26, 8, 16, 7, 27, 20, 13, 2,
                 41, 52, 31, 37, 47, 55, 30, 40,
                 51, 45, 33, 48, 44, 49, 39, 56,
                 34, 53, 46, 42, 50, 36, 29, 32};

             for (int i = 0; i < 48; i++)
             {
                 key[i] = CD[keyTransformation[i] - 1];
             }
             return key;
         }

        private static BitArray ShiftToLeft(BitArray bits)
         {
             BitArray shiftBits = new BitArray(28);
             for (int i = 0; i < 27; i++)
             {
                 shiftBits[i] = bits[i + 1];
             }

             shiftBits[27] = bits[0];
             return shiftBits;
         }

        private static BitArray change_8ths(BitArray key)
         {
             int count;
             for (int i = 0; i < 8; i++)
             {
                 count = 0;
                 for (int j = 0; j < 8; j++)
                 {
                     if (key[i * 8 + j])
                     {
                         count++;
                     }
                 }

                 if (count % 2 == 0)
                 {
                     key[(i + 1) * 8 - 1] = !key[(i + 1) * 8 - 1];
                 }
             }
             return key;
         }
        
        private static BitArray InitialPermutation(BitArray block)
         {
             int[] orderOfMixing =
             {
                 58, 50, 42, 34, 26, 18, 10, 2,
                 60, 52, 44, 36, 28, 20, 12, 4,
                 62, 54, 46, 38, 30, 22, 14, 6,
                 64, 56, 48, 40, 32, 24, 16, 8,
                 57, 49, 41, 33, 25, 17, 9, 1,
                 59, 51, 43, 35, 27, 19, 11, 3,
                 61, 53, 45, 37, 29, 21, 13, 5,
                 63, 55, 47, 39, 31, 23, 15, 7
             };

             BitArray result = new BitArray(64);

             for (int i = 0; i < 64; i++)
             {
                 result[i] = block[orderOfMixing[i] - 1];
             }
             return result;
         }
         
        private static BitArray FinalPermutation(BitArray block)
         {
             int[] orderOfMixing =
             {
                 40, 8, 48, 16, 56, 24, 64, 32,
                 39, 7, 47, 15, 55, 23, 63, 31,
                 38, 6, 46, 14, 54, 22, 62, 30,
                 37, 5, 45, 13, 53, 21, 61, 29,
                 36, 4, 44, 12, 52, 20, 60, 28,
                 35, 3, 43, 11, 51, 19, 59, 27,
                 34, 2, 42, 10, 50, 18, 58, 26,
                 33, 1, 41, 9, 49, 17, 57, 25
             };

             BitArray result = new BitArray(64);

             for (int i = 0; i < 64; i++)
             {
                 result[63 - i] = block[63 - (orderOfMixing[i] - 1)];
             }
             return result;
         }

        private static BitArray Expansion48(BitArray block)
         {
             int[] orderOfMixing =
             {
                 32, 1, 2, 3, 4, 5, 4, 5,
                 6, 7, 8, 9, 8, 9, 10, 11,
                 12, 13, 12, 13, 14, 15, 16, 17,
                 16, 17, 18, 19, 20, 21, 20, 21,
                 22, 23, 24, 25, 24, 25, 26, 27,
                 28, 29, 28, 29, 30, 31, 32, 1
             };

             BitArray result = new BitArray(48);

             for (int i = 0; i < 48; i++)
             {
                 result[i] = block[orderOfMixing[i] - 1];
             }
             return result;
         }

        private static BitArray Function(BitArray block_32, BitArray key_48)
        {
            BitArray block48Bits = Expansion48(block_32);

            for (int i = 0; i < 48; i++)
            {
                block48Bits[i] ^= key_48[i];
            }

            BitArray byte1 = new BitArray(6);
            byte1[0] = block48Bits[0];
            byte1[1] = block48Bits[1];
            byte1[2] = block48Bits[2];
            byte1[3] = block48Bits[3];
            byte1[4] = block48Bits[4];
            byte1[5] = block48Bits[5];

            BitArray byte2 = new BitArray(6);
            byte2[0] = block48Bits[6];
            byte2[1] = block48Bits[7];
            byte2[2] = block48Bits[8];
            byte2[3] = block48Bits[9];
            byte2[4] = block48Bits[10];
            byte2[5] = block48Bits[11];

            BitArray byte3 = new BitArray(6);
            byte3[0] = block48Bits[12];
            byte3[1] = block48Bits[13];
            byte3[2] = block48Bits[14];
            byte3[3] = block48Bits[15];
            byte3[4] = block48Bits[16];
            byte3[5] = block48Bits[17];

            BitArray byte4 = new BitArray(6);
            byte4[0] = block48Bits[18];
            byte4[1] = block48Bits[19];
            byte4[2] = block48Bits[20];
            byte4[3] = block48Bits[21];
            byte4[4] = block48Bits[22];
            byte4[5] = block48Bits[23];

            BitArray byte5 = new BitArray(6);
            byte5[0] = block48Bits[24];
            byte5[1] = block48Bits[25];
            byte5[2] = block48Bits[26];
            byte5[3] = block48Bits[27];
            byte5[4] = block48Bits[28];
            byte5[5] = block48Bits[29];

            BitArray byte6 = new BitArray(6);
            byte6[0] = block48Bits[30];
            byte6[1] = block48Bits[31];
            byte6[2] = block48Bits[32];
            byte6[3] = block48Bits[33];
            byte6[4] = block48Bits[34];
            byte6[5] = block48Bits[35];

            BitArray byte7 = new BitArray(6);
            byte7[0] = block48Bits[36];
            byte7[1] = block48Bits[37];
            byte7[2] = block48Bits[38];
            byte7[3] = block48Bits[39];
            byte7[4] = block48Bits[40];
            byte7[5] = block48Bits[41];

            BitArray byte8 = new BitArray(6);
            byte8[0] = block48Bits[42];
            byte8[1] = block48Bits[43];
            byte8[2] = block48Bits[44];
            byte8[3] = block48Bits[45];
            byte8[4] = block48Bits[46];
            byte8[5] = block48Bits[47];

            BitArray byte_1 = S1(byte1);
            BitArray byte_2 = S2(byte2);
            BitArray byte_3 = S3(byte3);
            BitArray byte_4 = S4(byte4);
            BitArray byte_5 = S5(byte5);
            BitArray byte_6 = S6(byte6);
            BitArray byte_7 = S7(byte7);
            BitArray byte_8 = S8(byte8);

            BitArray block32Bits = new BitArray(32);

            block32Bits[0] = byte_1[3];
            block32Bits[1] = byte_1[2];
            block32Bits[2] = byte_1[1];
            block32Bits[3] = byte_1[0];

            block32Bits[4] = byte_2[3];
            block32Bits[5] = byte_2[2];
            block32Bits[6] = byte_2[1];
            block32Bits[7] = byte_2[0];

            block32Bits[8] = byte_3[3];
            block32Bits[9] = byte_3[2];
            block32Bits[10] = byte_3[1];
            block32Bits[11] = byte_3[0];

            block32Bits[12] = byte_4[3];
            block32Bits[13] = byte_4[2];
            block32Bits[14] = byte_4[1];
            block32Bits[15] = byte_4[0];

            block32Bits[16] = byte_5[3];
            block32Bits[17] = byte_5[2];
            block32Bits[18] = byte_5[1];
            block32Bits[19] = byte_5[0];

            block32Bits[20] = byte_6[3];
            block32Bits[21] = byte_6[2];
            block32Bits[22] = byte_6[1];
            block32Bits[23] = byte_6[0];

            block32Bits[24] = byte_7[3];
            block32Bits[25] = byte_7[2];
            block32Bits[26] = byte_7[1];
            block32Bits[27] = byte_7[0];

            block32Bits[28] = byte_8[3];
            block32Bits[29] = byte_8[2];
            block32Bits[30] = byte_8[1];
            block32Bits[31] = byte_8[0];

            block32Bits = FeistelFunction(block32Bits);

            return block32Bits;
            
            
        }

        private static BitArray FeistelFunction(BitArray block)
         {
             int[] orderOfMixing =
             {
                 16, 7, 20, 21, 29, 12, 28, 17,
                 1, 15, 23, 26, 5, 18, 31, 10,
                 2, 8, 24, 14, 32, 27, 3, 9,
                 19, 13, 30, 6, 22, 11, 4, 25
             };

             BitArray result = new BitArray(32);

             for (int i = 0; i < 32; i++)
             {
                 result[i] = block[orderOfMixing[i] - 1];
             }
             return result;
         }

        private static BitArray S1(BitArray block1)
        {
            int[][] S1_transformation =
            {
                new int[] {14, 4, 13, 1, 2, 15, 11, 8, 3, 10, 6, 12, 5, 9, 0, 7},
                new int[] {0, 15, 7, 4, 14, 2, 13, 1, 10, 6, 12, 11, 9, 5, 3, 8},
                new int[] {4, 1, 14, 8, 13, 6, 2, 11, 15, 12, 9, 7, 3, 10, 5, 0},
                new int[] {15, 12, 8, 2, 4, 9, 1, 7, 5, 11, 3, 14, 10, 0, 6, 13}
            };

            BitArray a = new BitArray(2);
            BitArray b = new BitArray(4);

            a[1] = block1[0];
            a[0] = block1[5];

            b[3] = block1[1];
            b[2] = block1[2];
            b[1] = block1[3];
            b[0] = block1[4];

            int int_a = BitArrayToInt(a);
            int int_b = BitArrayToInt(b);

            int int_result = S1_transformation[int_a][int_b];


            BitArray result = new BitArray(4);

            result = IntToBitArray(int_result, 4);
            return result;
        }

        private static BitArray S2(BitArray block6)
        {
            int[][] S2_transformation =
            {
                new int[] {15, 1, 8, 14, 6, 11, 3, 4, 9, 7, 2, 13, 12, 0, 5, 10},
                new int[] {3, 13, 4, 7, 15, 2, 8, 14, 12, 0, 1, 10, 6, 9, 11, 5},
                new int[] {0, 14, 7, 11, 10, 4, 13, 1, 5, 8, 12, 6, 9, 3, 2, 15},
                new int[] {13, 8, 10, 1, 3, 15, 4, 2, 11, 6, 7, 12, 0, 5, 14, 9}
            };

            BitArray a = new BitArray(2);
            BitArray b = new BitArray(4);

            a[1] = block6[0];
            a[0] = block6[5];

            b[3] = block6[1];
            b[2] = block6[2];
            b[1] = block6[3];
            b[0] = block6[4];

            int int_a = BitArrayToInt(a);
            int int_b = BitArrayToInt(b);

            int int_result = S2_transformation[int_a][int_b];


            BitArray result = new BitArray(4);

            result = IntToBitArray(int_result, 4);
            return result;
        }

        private static BitArray S3(BitArray block6)
        {
            int[][] S3_transformation =
            {
                new int[] {10, 0, 9, 14, 6, 3, 15, 5, 1, 13, 12, 7, 11, 4, 2, 8},
                new int[] {13, 7, 0, 9, 3, 4, 6, 10, 2, 8, 5, 14, 12, 11, 15, 1},
                new int[] {13, 6, 4, 9, 8, 15, 3, 0, 11, 1, 2, 12, 5, 10, 14, 7},
                new int[] {1, 10, 13, 0, 6, 9, 8, 7, 4, 15, 14, 3, 11, 5, 2, 12}
            };

            BitArray a = new BitArray(2);
            BitArray b = new BitArray(4);

            a[1] = block6[0];
            a[0] = block6[5];

            b[3] = block6[1];
            b[2] = block6[2];
            b[1] = block6[3];
            b[0] = block6[4];

            int int_a = BitArrayToInt(a);
            int int_b = BitArrayToInt(b);

            int int_result = S3_transformation[int_a][int_b];


            BitArray result = new BitArray(4);

            result = IntToBitArray(int_result, 4);
            return result;
        }

        private static BitArray S4(BitArray block6)
        {
            int[][] S4_transformation =
            {
                new int[] {7, 13, 14, 3, 0, 6, 9, 10, 1, 2, 8, 5, 11, 12, 4, 15},
                new int[] {13, 8, 11, 5, 6, 15, 0, 3, 4, 7, 2, 12, 1, 10, 14, 9},
                new int[] {10, 6, 9, 0, 12, 11, 7, 13, 15, 1, 3, 14, 5, 2, 8, 4},
                new int[] {3, 15, 0, 6, 10, 1, 13, 8, 9, 4, 5, 11, 12, 7, 2, 14}
            };

            BitArray a = new BitArray(2);
            BitArray b = new BitArray(4);

            a[1] = block6[0];
            a[0] = block6[5];

            b[3] = block6[1];
            b[2] = block6[2];
            b[1] = block6[3];
            b[0] = block6[4];

            int int_a = BitArrayToInt(a);
            int int_b = BitArrayToInt(b);

            int int_result = S4_transformation[int_a][int_b];


            BitArray result = new BitArray(4);

            result = IntToBitArray(int_result, 4);
            return result;
        }

        private static BitArray S5(BitArray block6)
        {
            int[][] S5_transformation =
            {
                new int[] {2, 12, 4, 1, 7, 10, 11, 6, 8, 5, 3, 15, 13, 0, 14, 9},
                new int[] {14, 11, 2, 12, 4, 7, 13, 1, 5, 0, 15, 10, 3, 9, 8, 6},
                new int[] {4, 2, 1, 11, 10, 13, 7, 8, 15, 9, 12, 5, 6, 3, 0, 14},
                new int[] {11, 8, 12, 7, 1, 14, 2, 13, 6, 15, 0, 9, 10, 4, 5, 3}
            };

            BitArray a = new BitArray(2);
            BitArray b = new BitArray(4);

            a[1] = block6[0];
            a[0] = block6[5];

            b[3] = block6[1];
            b[2] = block6[2];
            b[1] = block6[3];
            b[0] = block6[4];

            int int_a = BitArrayToInt(a);
            int int_b = BitArrayToInt(b);

            int int_result = S5_transformation[int_a][int_b];


            BitArray result = new BitArray(4);

            result = IntToBitArray(int_result, 4);
            return result;
        }

        private static BitArray S6(BitArray block6)
        {
            int[][] S6_transformation =
            {
                new int[] {12, 1, 10, 15, 9, 2, 6, 8, 0, 13, 3, 4, 14, 7, 5, 11},
                new int[] {10, 15, 4, 2, 7, 12, 9, 5, 6, 1, 13, 14, 0, 11, 3, 8},
                new int[] {9, 14, 15, 5, 2, 8, 12, 3, 7, 0, 4, 10, 1, 13, 11, 6},
                new int[] {4, 3, 2, 12, 9, 5, 15, 10, 11, 14, 1, 7, 6, 0, 8, 13}
            };

            BitArray a = new BitArray(2);
            BitArray b = new BitArray(4);

            a[1] = block6[0];
            a[0] = block6[5];

            b[3] = block6[1];
            b[2] = block6[2];
            b[1] = block6[3];
            b[0] = block6[4];

            int int_a = BitArrayToInt(a);
            int int_b = BitArrayToInt(b);

            int int_result = S6_transformation[int_a][int_b];


            BitArray result = new BitArray(4);

            result = IntToBitArray(int_result, 4);
            return result;
        }

        private static BitArray S7(BitArray block6)
        {
            int[][] S7_transformation =
            {
                new int[] {4, 11, 2, 14, 15, 0, 8, 13, 3, 12, 9, 7, 5, 10, 6, 1},
                new int[] {13, 0, 11, 7, 4, 9, 1, 10, 14, 3, 5, 12, 2, 15, 8, 6},
                new int[] {1, 4, 11, 13, 12, 3, 7, 14, 10, 15, 6, 8, 0, 5, 9, 2},
                new int[] {6, 11, 13, 8, 1, 4, 10, 7, 9, 5, 0, 15, 14, 2, 3, 12}
            };

            BitArray a = new BitArray(2);
            BitArray b = new BitArray(4);

            a[1] = block6[0];
            a[0] = block6[5];

            b[3] = block6[1];
            b[2] = block6[2];
            b[1] = block6[3];
            b[0] = block6[4];

            int int_a = BitArrayToInt(a);
            int int_b = BitArrayToInt(b);

            int int_result = S7_transformation[int_a][int_b];


            BitArray result = new BitArray(4);

            result = IntToBitArray(int_result, 4);
            return result;
        }

        private static BitArray S8(BitArray block6)
        {
            int[][] S8_transformation =
            {
                new int[] {13, 2, 8, 4, 6, 15, 11, 1, 10, 9, 3, 14, 5, 0, 12, 7},
                new int[] {1, 15, 13, 8, 10, 3, 7, 4, 12, 5, 6, 11, 0, 14, 9, 2},
                new int[] {7, 11, 4, 1, 9, 12, 14, 2, 0, 6, 10, 13, 15, 3, 5, 8},
                new int[] {2, 1, 14, 7, 4, 10, 8, 13, 15, 12, 9, 0, 3, 5, 6, 11}
            };

            BitArray a = new BitArray(2);
            BitArray b = new BitArray(4);

            a[1] = block6[0];
            a[0] = block6[5];

            b[3] = block6[1];
            b[2] = block6[2];
            b[1] = block6[3];
            b[0] = block6[4];

            int int_a = BitArrayToInt(a);
            int int_b = BitArrayToInt(b);

            int int_result = S8_transformation[int_a][int_b];


            BitArray result = new BitArray(4);

            result = IntToBitArray(int_result, 4);
            return result;
        }
        
        static int BitArrayToInt(BitArray bits)
         {
             int result = 0;
         
             for (int i = bits.Count - 1; i >= 0; i--)
             {
                 if (bits[i])
                     result += Convert.ToInt32(Math.Pow(2, i));
             }
         
             return result;
         }

        private static BitArray IntToBitArray(int num, int length)
         {
             BitArray result = new BitArray(length);
             BitArray b = new BitArray(new int[] { num });

             for (int i = 0; i < length; i++)
             {
                 result[i] = b[i];
             }
             return result;
         }

        public static BitArray IntToBitArray(int[] num, int length)
         {
             BitArray result = new BitArray(length);
             for (int n = 0; n < num.Length; n++)
             {
                 BitArray b = new BitArray(new int[] { num[num.Length - n - 1] });

                 for (int i = 0; i < 8; i++)
                 {
                     result[i + n * 8] = b[i];
                 }
             }
             return result;
         }

    }
}