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
        try
        {
            bool ok = CriarArquivos(); //Retorno true se o arquivo de entrada existir e o de saida for criado

            if (ok)
            {
                using StreamReader reader = new(inputFile);
                using StreamWriter writer = new(outFile);
                string ModeloEntrada = reader.ReadLine();

                if (ModeloEntrada == ";S")
                {
                    ProcessaMaquinaTuring(reader, writer);
                }
                else if (ModeloEntrada == ";I")
                {
                    SimulaMTDIEmMT(reader, writer);
                }
                else
                {
                    throw new Exception("Não foi possível identificar o modelo da máquina de entreda.");
                }         
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    private static void ProcessaMaquinaTuring(StreamReader reader, StreamWriter writer)
    {
        string line;

        List<string> novasConfiguracoes = new()
        {
            $"0 0 0 {MovimentoEsquerda} 0",
            $"0 1 1 {MovimentoEsquerda} 0",
            $"0 * # {MovimentoDireita} {EstadoAuxiliar}"
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

    static private void SimulaMTDIEmMT(StreamReader reader, StreamWriter writer)
    {
        string line;

        List<string> novasConfiguracoes = new()
        {
            $"0 0 0 {MovimentoEsquerda} 0",
            $"0 1 1 {MovimentoEsquerda} 0",
            $"0 _ # {MovimentoDireita} achaFinalFita",
            $"achaFinalFita * * {MovimentoDireita} achaFinalFita",
            $"achaFinalFita _ @ {MovimentoEsquerda} voltarinicio",
            $"voltarinicio * * {MovimentoEsquerda} voltarinicio",
            $"voltarinicio # * {MovimentoDireita} {EstadoAuxiliar}"
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

                    string novaConfiguracao = $"{currentState} {currentSymbol} {newSymbol} {direction} {"acharFinalFita" + newState}";

                    //Verifica se a lista de novas configurações ja possui a configuração
                    if (!novasConfiguracoes.Contains(novaConfiguracao))
                    {
                        novasConfiguracoes.Add(novaConfiguracao);
                        novasConfiguracoes.AddRange(AdicionarEstadosSimuladoresMTDI(newState));
                    }
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

    static List<string> AdicionarEstadosSimuladoresMTDI(string estadoOriginal)
    {
        //Para nao ocorrer indeterminismo teremos que criar cada configuração seguinte para cada estado que recebe um mov p esquerda
        //Novas Estados:
        string acharFinalFita = "acharFinalFita" + estadoOriginal;
        string moverMarcadorFinalFita = "moverMarcadorFinalFita" + estadoOriginal;
        string preparaDeslocamentoSimbolos = "preparaDeslocamentoSimbolos" + estadoOriginal;
        string deslocamentoSimbolos = "deslocamentoSimbolos" + estadoOriginal;
        string escreve1 = "escreve1" + estadoOriginal;
        string escreve0 = "escreve0" + estadoOriginal;
        string fimDeslocamento = "fimDeslocamento" + estadoOriginal;

        List<string> novasConfiguracoes = new() {
            //Estado para encontrar o final dos dados
            $"{acharFinalFita} * * {MovimentoDireita} {acharFinalFita}",
            $"{acharFinalFita} @ * {MovimentoDireita} {moverMarcadorFinalFita}",
            //Deslocar o marcador @ para a direita
            $"{moverMarcadorFinalFita} * * {MovimentoDireita} {moverMarcadorFinalFita}", // Pula sobre os símbolos até o espaço em branco
            $"{moverMarcadorFinalFita} _ @ {MovimentoEsquerda} {preparaDeslocamentoSimbolos}", //Move @ para a direita e inicia o deslocamento de símbolos
            $"{preparaDeslocamentoSimbolos} * * {MovimentoEsquerda} {preparaDeslocamentoSimbolos}",
            $"{preparaDeslocamentoSimbolos} @ _ {MovimentoEsquerda} {deslocamentoSimbolos}",
            //Deslocar símbolos para a direita
            $"{deslocamentoSimbolos} 0 _ {MovimentoDireita} {escreve0}", //Lê 0, move para a direita para escrever 0
            $"{deslocamentoSimbolos} 1 _ {MovimentoDireita} {escreve1}", //Lê 1, move para a direita para escrever 1
            $"{deslocamentoSimbolos} _ * {MovimentoEsquerda} {deslocamentoSimbolos}",
            $"{deslocamentoSimbolos} # * {MovimentoDireita} {fimDeslocamento}",
            
            //
            $"{escreve0} * 0 {MovimentoEsquerda} {deslocamentoSimbolos}", //Escreve 0 e retorna para ler o próximo símbolo
            $"{escreve1} * 1 {MovimentoEsquerda} {deslocamentoSimbolos}", //Escreve 1 e retorna para ler o próximo símbolo

            //Após o deslocamento, volta para o estado original com o cabeçote no novo espaço em branco
            $"{fimDeslocamento} _ * * {estadoOriginal}"
        };

        return novasConfiguracoes;
    }
}