using System;
using System.IO;

namespace ConsoleApp1
{
    class Ogrenci
    {
        public int OgrNo { get; set; }
        public char[] Ad { get; set; } = new char[20];
        public int Notu { get; set; }
    }

    class Program
    {
        static int Hash(int grade) => grade % 10;

        static void PrintHashTable(int[,] hash_tablo, Ogrenci[] veri_liste)
        {
            Console.WriteLine("Not\tNext");
            for (int i = 0; i < hash_tablo.Length / 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    Console.Write(hash_tablo[i, j] + "\t");
                }
                Console.WriteLine("\n");
            }

            Console.WriteLine("-------------------------\n\nNo\tİsmi\tNotu");
            foreach (var veri in veri_liste)
            {
                Console.WriteLine($@"{(veri != null ? veri.OgrNo : "null")}{"\t"}{(veri != null ? new string(veri?.Ad) : "null")}{"\t"}{(veri != null ? veri?.Notu : "null")}");
            }
        }

        static void FillHashTable(int[,] hash_tablo)
        {
            for (int i = 0; i < hash_tablo.Length / 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    hash_tablo[i, j] = -1;
                }
            }
        }

        static string[] ReadFileLines()
        {
            string[] lines;
            try
            {
                var count = File.ReadAllLines(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + @"\veri.txt").Length;
                lines = new string[count];
                using (var streamReader = new StreamReader(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + @"\veri.txt"))
                {
                    for (int i = 0; i < count; i++)
                    {
                        string line = streamReader.ReadLine();
                        lines[i] = line;
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("Dosya Bulunamadı:");
                Console.WriteLine(e.Message);
                return null;
            }

            return lines;
        }

        static bool Add(Ogrenci ogrenci, Ogrenci[] veri_liste, int[,] hash_tablo)
        {
            int index = Hash(ogrenci.Notu);
            bool isSuccess = false;
            if (hash_tablo[index, 0] == -1 && hash_tablo[index, 1] == -1)
            {
                hash_tablo[index, 0] = ogrenci.Notu;
                hash_tablo[index, 1] = -1;
                veri_liste[index] = new Ogrenci { OgrNo = ogrenci.OgrNo, Ad = ogrenci.Ad, Notu = ogrenci.Notu };
            }
            else if (hash_tablo[index, 0] != -1)
            {
                for (int j = 0; j < hash_tablo.Length / 2; j++)
                {
                    isSuccess = false;
                    if (hash_tablo[j, 0] == -1 && hash_tablo[j, 1] == -1)
                    {
                        hash_tablo[j, 0] = ogrenci.Notu;
                        hash_tablo[j, 1] = -1;
                        veri_liste[j] = new Ogrenci { OgrNo = ogrenci.OgrNo, Ad = ogrenci.Ad, Notu = ogrenci.Notu };

                        if (hash_tablo[index, 1] == -1)
                            hash_tablo[index, 1] = j;
                        else
                        {
                            var searchIndex = hash_tablo[index, 1];
                            for (int k = 0; k < hash_tablo.Length / 2; k++)
                            {
                                if (hash_tablo[searchIndex, 1] == -1)
                                {
                                    hash_tablo[searchIndex, 1] = j;
                                    break;
                                }
                                else
                                {
                                    searchIndex = hash_tablo[searchIndex, 1];
                                    continue;
                                }
                            }
                        }
                        isSuccess = true;
                        break;
                    }
                }
                if (!isSuccess)
                {
                    return false;
                }
            }
            return true;
        }

        static void FillFromFile(Ogrenci[] veri_liste, int[,] hash_tablo)
        {
            var lines = ReadFileLines();
            if (lines == null)
                return;

            foreach (var line in lines)
            {
                int studentNumber = Convert.ToInt32(line.Split(' ')[0]);
                string studentName = line.Split(' ')[1];
                int studentGrade = Convert.ToInt32(line.Split(' ')[2]);
                bool result = Add(new Ogrenci { Notu = studentGrade, Ad = studentName.ToCharArray(), OgrNo = studentNumber }, veri_liste, hash_tablo);
                if (!result)
                {
                    Console.WriteLine("Tablo Doldu!");
                    return;
                }

            }
        }

        static void FillFromInput(Ogrenci[] veri_liste, int[,] hash_tablo)
        {
            var student = new Ogrenci();
            try
            {
                Console.Write("Ögrenci Numarasını Girin: ");
                student.OgrNo = Convert.ToInt32(Console.ReadLine());
                Console.Write("Ögrenci Adını Girin: ");
                student.Ad = Console.ReadLine().ToCharArray();
                Console.Write("Ögrenci Notunu Girin: ");
                student.Notu = Convert.ToInt32(Console.ReadLine());
            }
            catch
            {
                Console.WriteLine("Hatalı giriş formatı.");
                return;
            }

            bool result = Add(student, veri_liste, hash_tablo);
            if (!result)
                Console.WriteLine("Tablo Dolduğu için ekleme işlemi başarısız oldu!");
            else
                Console.WriteLine("Ekleme işlemi başarılı.");
        }

        static void Search(int[,] hash_tablo)
        {
            int count = 0;
            Console.Write("Aramak istediğiniz notu girin: ");
            int searchGrade;
            try
            {
                searchGrade = Convert.ToInt32(Console.ReadLine());
            }
            catch
            {
                Console.WriteLine("Hatalı giriş formatı.");
                return;
            }

            int index = Hash(searchGrade);
            if (hash_tablo[index, 0] == searchGrade)
            {
                Console.WriteLine($"\nNot:{searchGrade}\tAdım Sayısı: 1");
            }
            else
            {
                var searchIndex = hash_tablo[index, 1];
                for (int k = 0; k < hash_tablo.Length / 2; k++)
                {
                    if (searchIndex == -1)
                    {
                        Console.WriteLine("Bulunamadı.");
                        return;
                    }
                    else if (hash_tablo[searchIndex, 0] == searchGrade)
                    {
                        count++;
                        Console.WriteLine($"\nNot:{searchGrade}\tAdım Sayısı: {count + 1}");
                        return;
                    }
                    else
                    {
                        count++;
                        searchIndex = hash_tablo[searchIndex, 1];
                        continue;
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            Ogrenci[] veri_liste = new Ogrenci[10];
            int[,] hash_tablo = new int[10, 2];
            FillHashTable(hash_tablo);

            ConsoleKey key;
            do
            {
                Console.Write("\nEkleme = 'E'\nVeri Dosyasından Ekleme = 'L'\nArama = 'A'\nListeyi Görüntüle = 'G'\nYapmak istediğiniz işlemin baş harfini girin. Çıkmak için 'C' tuşuna basın.\n");
                key = Console.ReadKey().Key;
                Console.Clear();
                switch (key)
                {
                    case ConsoleKey.A:
                        Search(hash_tablo);
                        break;
                    case ConsoleKey.C:
                        Console.WriteLine("Çıkış yapıldı.");
                        break;
                    case ConsoleKey.E:
                        FillFromInput(veri_liste, hash_tablo);
                        break;
                    case ConsoleKey.G:
                        PrintHashTable(hash_tablo, veri_liste);
                        break;
                    case ConsoleKey.L:
                        FillFromFile(veri_liste, hash_tablo);
                        Console.WriteLine("Veri dosyasından okuma işlemi tamamlandı.");
                        break;
                    default:
                        Console.WriteLine("Hatalı giriş. Program sonlandırıldı.");
                        key = ConsoleKey.C;
                        break;
                }
            } while (key != ConsoleKey.C);
        }
    }
}