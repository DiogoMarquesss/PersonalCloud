namespace PersonalCloud.Common.Configuration;

public static class DotEnv
{
    public static void Load()
    {
        var directory = new DirectoryInfo(Directory.GetCurrentDirectory());

        while (directory is not null)
        {
            var candidates = new[]
            {
                Path.Combine(directory.FullName, ".env"),
                Path.Combine(directory.FullName, "src", ".env")
            };

            foreach (var path in candidates)
            {
                if (!File.Exists(path))
                    continue;

                foreach (var rawLine in File.ReadLines(path))
                {
                    var line = rawLine.Trim();
                    if (line.Length == 0 || line.StartsWith('#'))
                        continue;

                    if (line.StartsWith("export ", StringComparison.Ordinal))
                        line = line[7..].TrimStart();

                    var separator = line.IndexOf('=');
                    if (separator <= 0)
                        continue;

                    var key = line[..separator].Trim();
                    var value = line[(separator + 1)..].Trim();
                    if (value.Length >= 2 &&
                        ((value[0] == '"' && value[^1] == '"') ||
                         (value[0] == '\'' && value[^1] == '\'')))
                    {
                        value = value[1..^1];
                    }

                    if (Environment.GetEnvironmentVariable(key) is null)
                        Environment.SetEnvironmentVariable(key, value);
                }

                return;
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException("Could not find a .env file in the current directory or its parents.");
    }
}
