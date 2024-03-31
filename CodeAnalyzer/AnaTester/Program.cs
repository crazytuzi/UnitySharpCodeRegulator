namespace AnaTester
{
    /// <summary>
    /// 启动程序sss
    /// </summary>
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine(0);
            }

            else if (args.Length == 2)
            {
                if (true)
                {
                    Console.WriteLine(22);
                }
                else
                {
                    Console.WriteLine(23);
                }
            }
            else
            {
                Console.WriteLine(2);
            }
            Console.WriteLine(3);
        }
    }
}