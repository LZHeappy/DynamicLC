using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;

namespace SimpleLang
{
    // 标准库接口
    public interface ILibrary
    {
        void Init(Dictionary<string, object> vars);
        bool Call(string func, string[] args, Dictionary<string, object> vars);
    }

    // 标准库管理器
    public static class LibraryManager
    {
        private static readonly Dictionary<string, ILibrary> libs = new();

        public static void Register(string name, ILibrary lib) => libs[name] = lib;

        public static void Use(string libName, Dictionary<string, object> vars)
        {
            if (libs.TryGetValue(libName, out var lib))
            {
                lib.Init(vars);
                Console.WriteLine($"[lib] 已加载库: {libName}");
            }
            else
            {
                Console.WriteLine($"[lib] 未找到库: {libName}");
            }
        }

        public static bool Call(string libName, string func, string[] args, Dictionary<string, object> vars)
        {
            if (libs.TryGetValue(libName, out var lib))
                return lib.Call(func, args, vars);
            return false;
        }
    }

    // 标准IO库
    public class StdIOLibrary : ILibrary
    {
        public void Init(Dictionary<string, object> vars) { }
        public bool Call(string func, string[] args, Dictionary<string, object> vars)
        {
            if (func == "print")
            {
                Console.WriteLine(string.Join(" ", args));
                return true;
            }
            if (func == "input" && args.Length > 0)
            {
                Console.Write(args[0]);
                vars["last_input"] = Console.ReadLine();
                return true;
            }
            return false;
        }
    }

    // 数学库示例
    public class MathLibrary : ILibrary
    {
        public void Init(Dictionary<string, object> vars) { }
        public bool Call(string func, string[] args, Dictionary<string, object> vars)
        {
            if (func == "sqrt" && args.Length == 1 && double.TryParse(args[0], out double val))
            {
                Console.WriteLine(Math.Sqrt(val));
                return true;
            }
            return false;
        }
    }

    // 字符串库示例
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

    // 优化器
    public static class Optimizer
    {
        public static List<string> Optimize(List<string> lines)
        {
            var result = new List<string>();
            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                // 支持以 $ 开头作为注释
                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("//") || trimmed.StartsWith("$"))
                    continue;
                result.Add(trimmed);
            }
            return result;
        }
    }

    // 分析器
    public static class Analyzer
    {
        public static void Analyze(List<string> lines)
        {
            var vars = new HashSet<string>();
            foreach (var line in lines)
            {
                var assign = Regex.Match(line, @"(\w+)\s*=");
                if (assign.Success)
                    vars.Add(assign.Groups[1].Value);
            }
            Console.WriteLine("[analyzer] 变量: " + string.Join(", ", vars));
        }
    }

    // 简单调试器
    public class Debugger
    {
        public bool Enabled { get; set; }
        public HashSet<int> Breakpoints { get; } = new();

        public void Check(int line, Dictionary<string, object> vars)
        {
            if (!Enabled) return;
            if (Breakpoints.Contains(line))
            {
                Console.WriteLine($"[debug] 断点在第{line + 1}行，变量: {string.Join(", ", vars)}");
                Console.WriteLine("[debug] 按回车继续...");
                Console.ReadLine();
            }
        }
    }

    public class Interpreter
    {
        static Debugger debugger = new();

        public static void Main(string[] args)
        {
            // 注册标准库和扩展库
            LibraryManager.Register("io", new StdIOLibrary());
            LibraryManager.Register("math", new MathLibrary());
            LibraryManager.Register("str", new StringLibrary());
            // 其它库请放在单独文件并注册，如 ListLibrary、DequeLibrary 等
            LibraryManager.Register("list", new ListLibrary());
            LibraryManager.Register("deque", new DequeLibrary());
            LibraryManager.Register("vector", new VectorLibrary());
            LibraryManager.Register("stringbuilder", new StringBuilderLibrary());
            LibraryManager.Register("process", new ProcessLibrary());
            LibraryManager.Register("thread", new ThreadLibrary());
            LibraryManager.Register("socket", new SocketLibrary());
            LibraryManager.Register("gui", new GUILibrary());
            LibraryManager.Register("registry", new RegistryLibrary());
            LibraryManager.Register("db", new DatabaseLibrary());

            if (args.Length == 0)
            {
                Console.WriteLine("用法: Interpreter.exe <脚本文件名.slc> [debug]");
                return;
            }
            string filename = args[0];
            if (!filename.EndsWith(".slc", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("只支持 .slc 文件");
                return;
            }
            if (args.Length > 1 && args[1].ToLower() == "debug") debugger.Enabled = true;

            if (!File.Exists(filename))
            {
                Console.WriteLine($"文件不存在: {filename}");
                return;
            }

            var lines = new List<string>(File.ReadAllLines(filename));
            lines = Optimizer.Optimize(lines); // 优化
            Analyzer.Analyze(lines);           // 分析

            int pc = 0;
            var labels = new Dictionary<string, int>();
            var vars = new Dictionary<string, object>();
            var stack = new List<object>(); // runtime stack
            var classes = new Dictionary<string, List<string>>();

            // 预处理标签
            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                if (line.EndsWith(":"))
                    labels[line.Substring(0, line.Length - 1)] = i;
            }

            // 主解释循环
            while (pc < lines.Count)
            {
                string line = lines[pc];
                debugger.Check(pc, vars);

                // use 语句
                if (line.StartsWith("use "))
                {
                    var libName = line.Substring(4).Trim(';', ' ');
                    LibraryManager.Use(libName, vars);
                    pc++;
                    continue;
                }

                // 调试指令：以 ';' 开头的命令
                if (line.StartsWith(";"))
                {
                    var cmd = line.Substring(1).Trim();
                    if (cmd == "stack")
                    {
                        Console.WriteLine("-- Stack --");
                        foreach (var item in stack.AsEnumerable().Reverse())
                            Console.WriteLine(item?.ToString() ?? "null");
                        Console.WriteLine("-- End Stack --");
                    }
                    else
                    {
                        Console.WriteLine($"[debug] 未知调试命令: {cmd}");
                    }
                    pc++;
                    continue;
                }

                // 标准库调用: io.print "Hello"
                var libCall = Regex.Match(line, @"(\w+)\.(\w+)\s*(.*)");
                if (libCall.Success)
                {
                    var left = libCall.Groups[1].Value;
                    var method = libCall.Groups[2].Value;
                    var argTokens = libCall.Groups[3].Value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    // 先尝试作为库调用
                    if (LibraryManager.Call(left, method, argTokens, vars))
                    {
                        pc++;
                        continue;
                    }
                    // 再尝试作为对象方法调用： variable.method
                    if (vars.TryGetValue(left, out var instObj) && instObj is Dictionary<string, object> inst && inst.TryGetValue("__class", out var clsNameObj) && clsNameObj is string clsName)
                    {
                        if (classes.TryGetValue(clsName, out var classBody))
                        {
                            // 查找 method 定义： method <name>
                            for (int ci = 0; ci < classBody.Count; ci++)
                            {
                                var l = classBody[ci].Trim();
                                if (l.StartsWith("method "))
                                {
                                    var m = l.Substring(7).Trim();
                                    if (m == method)
                                    {
                                        // 执行方法体直到 end
                                        // 暂存 this
                                        vars["this"] = inst;
                                        int mi = ci + 1;
                                        while (mi < classBody.Count && classBody[mi].Trim() != "end")
                                        {
                                            ExecuteLine(classBody[mi], vars, labels, stack);
                                            mi++;
                                        }
                                        vars.Remove("this");
                                        break;
                                    }
                                }
                            }
                        }
                        pc++;
                        continue;
                    }
                }


                // class 语句（类理念，暂只收集类体）
                if (line.StartsWith("class "))
                {
                    var className = line.Substring(6).Trim().Trim('{', ' ', ';');
                    var classBody = new List<string>();
                    pc++;
                    while (pc < lines.Count && !lines[pc].Trim().StartsWith("}"))
                    {
                        classBody.Add(lines[pc]);
                        pc++;
                    }
                    classes[className] = classBody;
                    pc++; // 跳过 }
                    continue;
                }

                // 输出
                if (line.StartsWith("out "))
                {
                    var match = Regex.Match(line, @"out\s+""(.*?)""(,.*)?");
                    if (match.Success)
                    {
                        Console.Write(match.Groups[1].Value);
                        if (match.Groups[2].Success)
                        {
                            string[] codes = match.Groups[2].Value.Split(',');
                            foreach (var code in codes)
                            {
                                string c = code.Trim(' ', ',');
                                if (c.StartsWith("0x"))
                                    Console.Write((char)Convert.ToInt32(c, 16));
                                else if (int.TryParse(c, out int val))
                                    Console.Write((char)val);
                            }
                        }
                    }
                    pc++;
                    continue;
                }

                // 输入，支持格式化 in varName, "提示"
                if (line.StartsWith("in "))
                {
                    var match = Regex.Match(line, @"in\s+(\w+)(?:,\s*""(.*?)"")?");
                    if (match.Success)
                    {
                        string varName = match.Groups[1].Value;
                        string prompt = match.Groups[2].Success ? match.Groups[2].Value : $"请输入{varName}: ";
                        Console.Write(prompt);
                        vars[varName] = Console.ReadLine();
                    }
                    pc++;
                    continue;
                }

                // goto
                if (line.StartsWith("goto "))
                {
                    var label = line.Substring(5).Trim();
                    if (labels.ContainsKey(label))
                        pc = labels[label];
                    else
                        pc++;
                    continue;
                }

                // for
                if (line.StartsWith("for "))
                {
                    var match = Regex.Match(line, @"for\s+(\w+)\s*=\s*(\d+)\s*to\s*(\d+)");
                    if (match.Success)
                    {
                        string varName = match.Groups[1].Value;
                        int start = int.Parse(match.Groups[2].Value);
                        int end = int.Parse(match.Groups[3].Value);
                        for (int i = start; i <= end; i++)
                        {
                            vars[varName] = i;
                            pc++;
                            if (pc < lines.Count)
                                ExecuteLine(lines[pc], vars, labels, stack);
                        }
                        pc++;
                        continue;
                    }
                }

                // while
                if (line.StartsWith("while "))
                {
                    var match = Regex.Match(line, @"while\s+(\w+)\s*<\s*(\d+)");
                    if (match.Success)
                    {
                        string varName = match.Groups[1].Value;
                        int end = int.Parse(match.Groups[2].Value);
                        int startLine = pc + 1;
                        while (vars.ContainsKey(varName) && Convert.ToInt32(vars[varName]) < end)
                        {
                            if (startLine < lines.Count)
                                ExecuteLine(lines[startLine], vars, labels, stack);
                            vars[varName] = Convert.ToInt32(vars[varName]) + 1;
                        }
                        pc = startLine + 1;
                        continue;
                    }
                }

                // do-while
                if (line.StartsWith("do "))
                {
                    int startLine = pc + 1;
                    pc++;
                    while (pc < lines.Count && !lines[pc].Trim().StartsWith("while "))
                    {
                        ExecuteLine(lines[pc], vars, labels, stack);
                        pc++;
                    }
                    if (pc < lines.Count)
                    {
                        var match = Regex.Match(lines[pc], @"while\s+(\w+)\s*<\s*(\d+)");
                        if (match.Success)
                        {
                            string varName = match.Groups[1].Value;
                            int end = int.Parse(match.Groups[2].Value);
                            while (vars.ContainsKey(varName) && Convert.ToInt32(vars[varName]) < end)
                            {
                                for (int i = startLine; i < pc; i++)
                                    ExecuteLine(lines[i], vars, labels, stack);
                                vars[varName] = Convert.ToInt32(vars[varName]) + 1;
                            }
                        }
                        pc++;
                    }
                    continue;
                }

                // 变量赋值
                var assign = Regex.Match(line, @"(\w+)\s*=\s*(\d+)");
                if (assign.Success)
                {
                    vars[assign.Groups[1].Value] = int.Parse(assign.Groups[2].Value);
                    pc++;
                    continue;
                }

                pc++;
            }
        }

        static void ExecuteLine(string line, Dictionary<string, object> vars, Dictionary<string, int> labels, List<object>? stack = null)
        {
            var trimmed = line.Trim();

            // stack operations
            if (trimmed.StartsWith("push "))
            {
                if (stack != null)
                {
                    var token = trimmed.Substring(5).Trim();
                    if (vars.TryGetValue(token, out var v))
                        stack.Add(v!);
                    else if (int.TryParse(token, out var ival))
                        stack.Add(ival);
                    else
                        stack.Add(token);
                }
                return;
            }
            if (trimmed == "pop")
            {
                if (stack != null && stack.Count > 0)
                {
                    var v = stack[stack.Count - 1];
                    stack.RemoveAt(stack.Count - 1);
                    Console.WriteLine(v?.ToString() ?? "null");
                }
                return;
            }

            if (trimmed.StartsWith("out "))
            {
                var match = Regex.Match(line, @"out\s+""(.*?)""(,.*)?");
                if (match.Success)
                {
                    Console.Write(match.Groups[1].Value);
                    if (match.Groups[2].Success)
                    {
                        string[] codes = match.Groups[2].Value.Split(',');
                        foreach (var code in codes)
                        {
                            string c = code.Trim(' ', ',');
                            if (c.StartsWith("0x"))
                                Console.Write((char)Convert.ToInt32(c, 16));
                            else if (int.TryParse(c, out int val))
                                Console.Write((char)val);
                        }
                    }
                }
            }
        }
    }
}