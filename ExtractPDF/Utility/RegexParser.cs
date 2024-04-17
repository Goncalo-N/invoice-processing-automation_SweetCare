using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace PDFDataExtraction.Utility
{
    public static class RegexParser
    {
        // Ensure your SupplierPattern class property names match the JSON field names exactly.
        public class SupplierPattern
        {
            [JsonProperty("nome_empresa")]
            public string NomeEmpresa { get; set; }

            [JsonProperty("padrao_regex_nome_fornecedor")]
            public string PadraoRegexNomeFornecedor { get; set; }

            [JsonProperty("padrao_regex_data_fatura")]
            public string PadraoRegexDataFatura { get; set; }

            [JsonProperty("padrao_regex_numero_encomenda")]
            public string PadraoRegexNumeroEncomenda { get; set; }

            [JsonProperty("padrao_regex_numero_fatura")]
            public string PadraoRegexNumeroFatura { get; set; }

            [JsonProperty("padrao_regex_data_vencimento_fatura")]
            public string PadraoRegexDataVencimentoFatura { get; set; }

            [JsonProperty("padrao_regex_total_sem_iva")]
            public string PadraoRegexTotalSemIva { get; set; }

            [JsonProperty("padrao_regex_totais_por_iva")]
            public string PadraoRegexTotaisPorIva { get; set; }

            [JsonProperty("padrao_regex_valor_iva")]
            public string PadraoRegexValorIva { get; set; }

            [JsonProperty("padrao_regex_desconto_pronto_pagamento")]
            public string PadraoRegexDescontoProntoPagamento { get; set; }

            [JsonProperty("padrao_regex_total_a_pagar")]
            public string PadraoRegexTotalAPagar { get; set; }

            [JsonProperty("padrao_regex_produto")]
            public string PadraoRegexProduto { get; set; }

            [JsonProperty("padrao_regex_taxa_iva")]
            public string PadraoRegexTaxaIva { get; set; }
        }

        public static List<SupplierPattern> LoadSupplierPatterns(string jsonFilePath)
        {
            var jsonContent = File.ReadAllText(jsonFilePath);
            return JsonConvert.DeserializeObject<List<SupplierPattern>>(jsonContent);
        }
    }
}
