
namespace SimpleLang
{
    // List库
    public class ListLibrary : ILibrary
    {
        private Dictionary<string, List<object>> lists = new();
        public void Init(Dictionary<string, object> vars) { }
        public bool Call(string func, string[] args, Dictionary<string, object> vars)
        {
            if (func == "create" && args.Length == 1)
            {
                lists[args[0]] = new List<object>();
                return true;
            }
            if (func == "add" && args.Length >= 2 && lists.ContainsKey(args[0]))
            {
                lists[args[0]].Add(string.Join(" ", args, 1, args.Length - 1));
                return true;
            }
            if (func == "get" && args.Length == 2 && lists.ContainsKey(args[0]) && int.TryParse(args[1], out int idx))
            {
                Console.WriteLine(lists[args[0]][idx]);
                return true;
            }
            return false;
        }
    }

    // Deque库
    public class DequeLibrary : ILibrary
    {
        private Dictionary<string, LinkedList<object>> deques = new();
        public void Init(Dictionary<string, object> vars) { }
        public bool Call(string func, string[] args, Dictionary<string, object> vars)
        {
            if (func == "create" && args.Length == 1)
            {
                deques[args[0]] = new LinkedList<object>();
                return true;
            }
            if (func == "add_first" && args.Length >= 2 && deques.ContainsKey(args[0]))
            {
                deques[args[0]].AddFirst(string.Join(" ", args, 1, args.Length - 1));
                return true;
            }
            if (func == "add_last" && args.Length >= 2 && deques.ContainsKey(args[0]))
            {
                deques[args[0]].AddLast(string.Join(" ", args, 1, args.Length - 1));
                return true;
            }
            if (func == "remove_first" && args.Length == 1 && deques.ContainsKey(args[0]))
            {
                if (deques[args[0]].Count > 0)
                    deques[args[0]].RemoveFirst();
                return true;
            }
            if (func == "remove_last" && args.Length == 1 && deques.ContainsKey(args[0]))
            {
                if (deques[args[0]].Count > 0)
                    deques[args[0]].RemoveLast();
                return true;
            }
            return false;
        }
    }

    // Vector库（用List实现）
    public class VectorLibrary : ILibrary
    {
        private Dictionary<string, List<object>> vectors = new();
        public void Init(Dictionary<string, object> vars) { }
        public bool Call(string func, string[] args, Dictionary<string, object> vars)
        {
            if (func == "create" && args.Length == 1)
            {
                vectors[args[0]] = new List<object>();
                return true;
            }
            if (func == "add" && args.Length >= 2 && vectors.ContainsKey(args[0]))
            {
                vectors[args[0]].Add(string.Join(" ", args, 1, args.Length - 1));
                return true;
            }
            if (func == "get" && args.Length == 2 && vectors.ContainsKey(args[0]) && int.TryParse(args[1], out int idx))
            {
                Console.WriteLine(vectors[args[0]][idx]);
                return true;
            }
            return false;
        }
    }

    // String库
    /*
    public class StringLibrary : ILibrary
    {
        public void Init(Dictionary<string, object> vars) { }
        public bool Call(string func, string[] args, Dictionary<string, object> vars)
        {
            if (func == "upper" && args.Length == 1)
            {
                Console.WriteLine(args[0].ToUpper());
                return true;
            }
            if (func == "lower" && args.Length == 1)
            {
                Console.WriteLine(args[0].ToLower());
                return true;
            }
            return false;
        }
    }
    */

    // StringBuilder库
    public class StringBuilderLibrary : ILibrary
    {
        private Dictionary<string, System.Text.StringBuilder> builders = new();
        public void Init(Dictionary<string, object> vars) { }
        public bool Call(string func, string[] args, Dictionary<string, object> vars)
        {
            if (func == "create" && args.Length == 1)
            {
                builders[args[0]] = new System.Text.StringBuilder();
                return true;
            }
            if (func == "append" && args.Length >= 2 && builders.ContainsKey(args[0]))
            {
                builders[args[0]].Append(string.Join(" ", args, 1, args.Length - 1));
                return true;
            }
            if (func == "to_string" && args.Length == 1 && builders.ContainsKey(args[0]))
            {
                Console.WriteLine(builders[args[0]].ToString());
                return true;
            }
            return false;
        }
    }

    // Process库
    public class ProcessLibrary : ILibrary
    {
        public void Init(Dictionary<string, object> vars) { }
        public bool Call(string func, string[] args, Dictionary<string, object> vars)
        {
            if (func == "start" && args.Length >= 1)
            {
                System.Diagnostics.Process.Start(string.Join(" ", args));
                return true;
            }
            return false;
        }
    }

    // Thread库
    public class ThreadLibrary : ILibrary
    {
        private Dictionary<string, System.Threading.Thread> threads = new();
        public void Init(Dictionary<string, object> vars) { }
        public bool Call(string func, string[] args, Dictionary<string, object> vars)
        {
            if (func == "start" && args.Length == 2)
            {
                string name = args[0];
                string msg = args[1];
                var t = new System.Threading.Thread(() => Console.WriteLine(msg));
                threads[name] = t;
                t.Start();
                return true;
            }
            return false;
        }
    }

    // Socket库（仅演示，实际需完善）
    public class SocketLibrary : ILibrary
    {
        public void Init(Dictionary<string, object> vars) { }
        public bool Call(string func, string[] args, Dictionary<string, object> vars)
        {
            // 这里只做演示，实际网络通信需详细实现
            if (func == "info")
            {
                Console.WriteLine("Socket功能已加载（需完善）");
                return true;
            }
            return false;
        }
    }

    // GUI库（仅演示，实际需完善）
    public class GUILibrary : ILibrary
    {
        public void Init(Dictionary<string, object> vars) { }
        public bool Call(string func, string[] args, Dictionary<string, object> vars)
        {
            if (func == "show_message" && args.Length > 0)
            {
                // Avoid requiring WinForms reference in the project.
                // Print GUI message to console as fallback.
                Console.WriteLine("[gui] " + string.Join(" ", args));
                return true;
            }
            return false;
        }
    }

    // 注册表库（仅演示，实际需完善）
    public class RegistryLibrary : ILibrary
    {
        public void Init(Dictionary<string, object> vars) { }
        public bool Call(string func, string[] args, Dictionary<string, object> vars)
        {
            // 这里只做演示，实际注册表操作需详细实现
            if (func == "info")
            {
                Console.WriteLine("Registry功能已加载（需完善）");
                return true;
            }
            return false;
        }
    }

    // 数据库库（仅演示，实际需完善）
    public class DatabaseLibrary : ILibrary
    {
        public void Init(Dictionary<string, object> vars) { }
        public bool Call(string func, string[] args, Dictionary<string, object> vars)
        {
            // 这里只做演示，实际数据库操作需详细实现
            if (func == "info")
            {
                Console.WriteLine("Database功能已加载（需完善）");
                return true;
            }
            return false;
        }
    }
}