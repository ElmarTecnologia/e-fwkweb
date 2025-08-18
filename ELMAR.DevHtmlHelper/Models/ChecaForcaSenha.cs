using System;
using System.Text.RegularExpressions;
using System.Web;

namespace ELMAR.DevHtmlHelper.Models
{
    public class ChecaForcaSenha
    {
        public string InfoMessage { get; set; }
        public string DicaSenha { get; set; }
        public ForcaDaSenha? ForcaSenha { get; set; }

        public ChecaForcaSenha() { }

        public ChecaForcaSenha(ForcaDaSenha forcaSenha)
        {
            this.ForcaSenha = forcaSenha;
        }

        public bool VerificaForcaSenha(string senha, HttpContextBase httpContext)
        {
            string nvlForcaSenha;
            int forca;

            if (ForcaSenha == null)
            {
                nvlForcaSenha = FwkConfig.GetSettingValue("nvlForcaSenha", Core.GetSetCTX(httpContext)).Equals(string.Empty) ? "Fraca" : FwkConfig.GetSettingValue("nvlForcaSenha", Core.GetSetCTX(httpContext));
                forca = (int)(ForcaDaSenha)Enum.Parse(typeof(ForcaDaSenha), nvlForcaSenha);
            }
            else
            {
                nvlForcaSenha = ForcaSenha.ToString();
                forca = (int)ForcaSenha;
            }

            if (this.GeraPontosSenha(senha) < forca)
            {
                InfoMessage = "Nível de segurança da senha: " + this.GetForcaDaSenha(senha).ToString() + ". Por favor, informe uma senha mais segura. "
                              + this.GetDicaSenha(httpContext, (ForcaDaSenha)Enum.Parse(typeof(ForcaDaSenha), nvlForcaSenha));
                return false;
            }            

            return true;
        }

        public int GeraPontosSenha(string senha)
        {
            if (senha == null) return 0;
            int pontosPorTamanho = GetPontoPorTamanho(senha);
            int pontosPorMinusculas = GetPontoPorMinusculas(senha);
            int pontosPorMaiusculas = GetPontoPorMaiusculas(senha);
            int pontosPorDigitos = GetPontoPorDigitos(senha);
            int pontosPorSimbolos = GetPontoPorSimbolos(senha);
            int pontosPorRepeticao = GetPontoPorRepeticao(senha);
            return pontosPorTamanho + pontosPorMinusculas + pontosPorMaiusculas + pontosPorDigitos + pontosPorSimbolos - pontosPorRepeticao;
        }

        private int GetPontoPorTamanho(string senha)
        {
            return Math.Min(10, senha.Length) * 3;
        }

        private int GetPontoPorMinusculas(string senha)
        {
            int rawplacar = senha.Length - Regex.Replace(senha, "[a-z]", "").Length;
            return Math.Min(2, rawplacar) * 9;
        }

        private int GetPontoPorMaiusculas(string senha)
        {
            int rawplacar = senha.Length - Regex.Replace(senha, "[A-Z]", "").Length;
            return Math.Min(2, rawplacar) * 9;
        }

        private int GetPontoPorDigitos(string senha)
        {
            int rawplacar = senha.Length - Regex.Replace(senha, "[0-9]", "").Length;
            return Math.Min(2, rawplacar) * 9;
        }

        private int GetPontoPorSimbolos(string senha)
        {
            int rawplacar = Regex.Replace(senha, "[a-zA-Z0-9]", "").Length;
            return Math.Min(2, rawplacar) * 10;
        }

        private int GetPontoPorRepeticao(string senha)
        {
            Regex regex = new Regex(@"(\w)*.*\1");
            bool repete = regex.IsMatch(senha);
            if (repete)
            {
                return 9;
            }
            else
            {
                return 0;
            }
        }

        public ForcaDaSenha GetForcaDaSenha(string senha)
        {
            int placar = GeraPontosSenha(senha);

            if (placar < 30)
                return ForcaDaSenha.Inaceitavel;
            else if (placar < 50)
                return ForcaDaSenha.Fraca;
            else if (placar < 70)
                return ForcaDaSenha.Aceitavel;
            else if (placar < 90)
                return ForcaDaSenha.Forte;
            else
                return ForcaDaSenha.Segura;
        }

        public string GetDicaSenha(HttpContextBase httpContext, ForcaDaSenha? forcaSenha = null)
        {
            if (forcaSenha == null)
            {
                string nvlForcaSenha = FwkConfig.GetSettingValue("nvlForcaSenha", Core.GetSetCTX(httpContext)).Equals(string.Empty) ? "Fraca" : FwkConfig.GetSettingValue("nvlForcaSenha", Core.GetSetCTX(httpContext));
                forcaSenha = (ForcaDaSenha)Enum.Parse(typeof(ForcaDaSenha), nvlForcaSenha);
            }

            switch (forcaSenha)
            {
                case ForcaDaSenha.Inaceitavel:
                    DicaSenha = "Sugestão: Informar uma senha com pelo menos 6 caracteres.";
                    break;
                case ForcaDaSenha.Fraca:
                    DicaSenha = "Sugestão: Informar uma senha com pelo menos 6 caracteres, contendo letras e números.";
                    break;
                case ForcaDaSenha.Aceitavel:
                    DicaSenha = "Sugestão: Informar uma senha com pelo menos 6 caracteres, contendo letras miúsculas e minúsculas e números.";
                    break;
                case ForcaDaSenha.Forte:
                    DicaSenha = "Sugestão: Informar uma senha com pelo menos 8 caracteres, contendo letras miúsculas e minúsculas, números e pelo menos um caracter especial.";
                    break;
                case ForcaDaSenha.Segura:
                    DicaSenha = "Sugestão: Informar uma senha com pelo menos 10 caracteres, contendo letras miúsculas e minúsculas, números e pelo menos dois caracteres especiais.";
                    break;
                default:
                    DicaSenha = "Nível de Senha não definido.";
                    break;
            }

            return DicaSenha;
        }
    }

    public enum ForcaDaSenha
    {
        Inaceitavel=30,
        Fraca=50,
        Aceitavel=70,
        Forte=90,
        Segura=110
    }
}