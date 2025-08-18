using System;

namespace ELMAR.DevHtmlHelper.Models.Extensions
{
    public static class DateTimeExtension
    {
        public static string ConvertToPortalUpdated(this DateTime ultAtualizacao)
        {
            return $"Atualizado em: {(ultAtualizacao.Year == 1 ? " - " : ultAtualizacao.ToString("dd/MM/yyyy - HH:mm"))}";
        }
    }
}