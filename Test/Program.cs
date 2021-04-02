using System;
using System.Threading.Tasks;
//using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;

namespace Test
{
    class Program
    {
        static int num = 0;
        //ダブルクォートをつける
        static string AddDoubleQuote(string path)
        {
            return "\"" + path + "\"";
        }

        //ダブルクォートを外す
        static string DeleteDoubleQuote(string path)
        {
            path = path.Replace("\"", "");
            path = path.Replace("\"", "");

            return path;
        }


        //ディレクトリ内部を表示
        static List<string> GetDirectory(string path)
        {
            List<string> name = new List<string>();
            string[] list = Directory.GetFiles(path, "*", SearchOption.AllDirectories);

            foreach (string na in list)
            {
                name.Add(na);
            }

            return name;
        }

        //コマンドを取得
        static string GetCommand(string in_path,string out_path)
        {
            in_path = "\"" + in_path + "\"";

            string ext = Path.GetExtension(in_path);
            ext = ext.Replace("\"", "");
            //Console.WriteLine(ext);
            if (ext == ".flac")
            {
//                Console.WriteLine("ああああ " + ext );
                out_path = "\""+ out_path.Replace(ext, ".m4a");
                

                string command = " -i " + in_path + " -threads 0 -c:v copy -metadata comment=\"\" -acodec alac" + " " + out_path + "\"";
                return command;
            }
            else
            {
                return null;
            }

        }
        
        //ファイルをエンコードする
        public static async Task<int> ConvertFile(string in_path,string out_path,int max)
        {
            return await Task.Run(() =>
            {
                Process pro = new Process();

                //エンコードをかける
                string st = GetCommand(in_path,out_path);
                if (st != null)
                {
                    //Interlocked(num,1);
                    pro.StartInfo.FileName = "ffmpeg.exe";
                    pro.StartInfo.Arguments = st;  //引数
                    pro.StartInfo.CreateNoWindow = true;    //コンソール画面を表示しない。
                    pro.Start();
                    pro.WaitForExit();  //処理を待機
                    num++;
                    //Console.WriteLine("exit code: " + pro.ExitCode);
                    Console.WriteLine("[ " +num +" / " +max+ " ]: "  + Path.GetFileName(st));

                    int e = pro.ExitCode;
                    pro.Close();

                    return e;
                    //return pro.ExitCode;
                }
                else
                {
            //        Console.WriteLine("Error");
                    return -1;
                }
            });
        
        }

        



        static void Main(string[] args)
        {

            //string str = args[0];
            string str = Console.ReadLine();                    //ファイル名をD&D
            str = DeleteDoubleQuote(str);                       //ダブルクォートを外す

            List<string> filePath = GetDirectory(str);          //ディレクトリを取得
            List<string> destPath = new List<string>();         //保存先のパス
            List<Task<int>> tasklist = new List<Task<int>>();   //タスクリスト

            int num = 0;
            foreach (string n in filePath)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(n.Replace(Directory.GetParent(str).FullName, Directory.GetCurrentDirectory())));   //フォルダ作成 
                tasklist.Add(ConvertFile(n, n.Replace(Directory.GetParent(str).FullName, Directory.GetCurrentDirectory()),filePath.Count));
            }

            Task.WaitAll(tasklist.ToArray());   //待機
           

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("変換終了");

            Console.ReadKey();
        }
    }
}
