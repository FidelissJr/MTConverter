// See https://aka.ms/new-console-template for more information

//Console.WriteLine("Hello, World!");

using System.Reflection;

class MTConverter
{
    private static readonly string directory = GetCurrentDirectory();
    private static readonly string inputFile = $"{directory}\\entrada.txt";
    private static readonly string outFile = $"{directory}\\saida.txt";

    private static readonly string EstadoAuxiliar = "aux0";
    private static readonly string EstadoInicial = "0";
    private static readonly string delimitador = "#";
    private static readonly string EstadoAceitacao = "accept";
    private static readonly string EstadoRejeicao = "reject";
    private static readonly string MovimentoDireita = "r";
    private static readonly string MovimentoEsquerda = "l";
    private static readonly string MovimentoEstacioario = "*";

    static void Main()
    {
        bool ok = CriarArquivos(); //Retorno true se o arquivo de entrada existir e o de saida for criado
        if (ok)
            ProcessaMaquinaTuring();
    }

    private static void ProcessaMaquinaTuring()
    {
        using StreamReader reader = new(inputFile);
        using StreamWriter writer = new(outFile);
        string line;

        List<string> novasConfiguracoes = new()
        {
            "0 0 0 l 0",
            "0 1 1 l 0",
            $"0 * # r {EstadoAuxiliar}"
        };

        while ((line = reader.ReadLine()) != null)
        {
            if (IgnorarLinha(line))
            {
                //Caso a linha seja um comentario, somente escrever ela na saida
                writer.WriteLine(line);
                continue;
            }

            //Caso o comentário esteja após a configuração, na mesma linha, a validação ocorre somente na configuração
            string[] parts = line.Contains(';') ? line.Split(';')[0].Trim().Split() : line.Trim().Split();

            if (parts.Length == 5)
            {
                string currentState = parts[0];
                string currentSymbol = parts[1];
                string newSymbol = parts[2];
                string direction = parts[3];
                string newState = parts[4];

                if (newState == EstadoInicial || currentState == EstadoInicial)
                    line = SubstituirEstado(line, EstadoInicial, EstadoAuxiliar);

                if (direction == MovimentoEsquerda && (newState != EstadoRejeicao && newState != EstadoAceitacao))
                {
                    //Faz a tratativa deste movimento p esquerda
                    currentState = newState;
                    currentSymbol = delimitador;
                    newSymbol = delimitador;
                    direction = MovimentoDireita;

                    string novaConfiguracao = $"{currentState} {currentSymbol} {newSymbol} {direction} {newState}";

                    //Verifica se a lista de novas configurações ja possui a configuração
                    if (!novasConfiguracoes.Contains(novaConfiguracao))
                        novasConfiguracoes.Add(novaConfiguracao);
                }

                writer.WriteLine(line);

            }
            else
            {
                Console.WriteLine("Erro na formatação da função sigma. Linha: " + line);
                break;
            }
        }

        AdicionarNovasConfiguracoes(writer, novasConfiguracoes);
    }

    private static bool CriarArquivos()
    {
        // Cria o arquivo de saída
        if (!File.Exists(outFile))
            File.CreateText(outFile);

        return (File.Exists(inputFile) || File.Exists(outFile));
    }

    static bool IgnorarLinha(string line)
    {
        //Ignora a linha (retorn true) caso a linha esteja em branco ou inicie com ";"
        return line.FirstOrDefault().ToString() == "\0" || line.StartsWith(";");
    }

    static string SubstituirEstado(string source, string find, string replace)
    {
        int position = source.IndexOf(find);
        if (position < 0)
        {
            return source; // Nada foi encontrado para substituir, retorna a string original
        }

        // Realiza a substituição da primeira ocorrência
        return source.Substring(0, position) + replace + source.Substring(position + find.Length);
    }

    static void AdicionarNovasConfiguracoes(StreamWriter writer, List<string> novasConfiguracoes)
    {
        writer.WriteLine("; Novas configurações geradas");

        foreach (var item in novasConfiguracoes)
            writer.WriteLine(item);
    }

    public static string GetCurrentDirectory()
    {
        // Obtém o caminho completo do arquivo em execução
        string executablePath = Assembly.GetExecutingAssembly().Location.Split("bin").First();     

        // Retorna o diretório do arquivo
        return Path.GetDirectoryName(executablePath);
    }

    static private void SimulaMTDIEmMT()
    {
        using StreamReader reader = new(inputFile);
        using StreamWriter writer = new(outFile);
        string line;

        List<string> novasConfiguracoes = new()
        {
            "0 0 0 l 0",
            "0 1 1 l 0",
            $"0 * # r {EstadoAuxiliar}"
        };

        while ((line = reader.ReadLine()) != null)
        {
            if (IgnorarLinha(line))
            {
                //Caso a linha seja um comentario, somente escrever ela na saida
                writer.WriteLine(line);
                continue;
            }

            //Caso o comentário esteja após a configuração, na mesma linha, a validação ocorre somente na configuração
            string[] parts = line.Contains(';') ? line.Split(';')[0].Trim().Split() : line.Trim().Split();

            if (parts.Length == 5)
            {
                string currentState = parts[0];
                string currentSymbol = parts[1];
                string newSymbol = parts[2];
                string direction = parts[3];
                string newState = parts[4];

                if (newState == EstadoInicial || currentState == EstadoInicial)
                    line = SubstituirEstado(line, EstadoInicial, EstadoAuxiliar);

                if (direction == MovimentoEsquerda && (newState != EstadoRejeicao && newState != EstadoAceitacao))
                {
                    //Faz a tratativa deste movimento p esquerda
                    currentState = newState;
                    currentSymbol = delimitador;
                    newSymbol = delimitador;
                    direction = MovimentoDireita;

                    string novaConfiguracao = $"{currentState} {currentSymbol} {newSymbol} {direction} {newState}";

                    //Verifica se a lista de novas configurações ja possui a configuração
                    if (!novasConfiguracoes.Contains(novaConfiguracao))
                        novasConfiguracoes.Add(novaConfiguracao);
                }

                writer.WriteLine(line);

            }
            else
            {
                Console.WriteLine("Erro na formatação da função sigma. Linha: " + line);
                break;
            }
        }

        AdicionarNovasConfiguracoes(writer, novasConfiguracoes);
    }
}