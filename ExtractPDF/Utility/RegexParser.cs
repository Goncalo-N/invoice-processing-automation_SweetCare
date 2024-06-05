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
            public required string NomeEmpresa { get; set; }

            [JsonProperty("padrao_regex_nome_fornecedor")]
            public required string PadraoRegexNomeFornecedor { get; set; }

            [JsonProperty("padrao_regex_data_fatura")]
            public required string PadraoRegexDataFatura { get; set; }

            [JsonProperty("padrao_regex_numero_encomenda")]
            public required string PadraoRegexNumeroEncomenda { get; set; }

            [JsonProperty("padrao_regex_numero_fatura")]
            public required string PadraoRegexNumeroFatura { get; set; }

            [JsonProperty("padrao_regex_data_vencimento_fatura")]
            public required string PadraoRegexDataVencimentoFatura { get; set; }

            [JsonProperty("padrao_regex_total_sem_iva")]
            public required string PadraoRegexTotalSemIva { get; set; }

            [JsonProperty("padrao_regex_totais_por_iva")]
            public required string PadraoRegexTotaisPorIva { get; set; }

            [JsonProperty("padrao_regex_valor_iva")]
            public required string PadraoRegexValorIva { get; set; }

            [JsonProperty("padrao_regex_desconto_pronto_pagamento")]
            public required string PadraoRegexDescontoProntoPagamento { get; set; }

            [JsonProperty("padrao_regex_total_a_pagar")]
            public required string PadraoRegexTotalAPagar { get; set; }

            [JsonProperty("padrao_regex_produto")]
            public required string PadraoRegexProduto { get; set; }

            [JsonProperty("padrao_regex_taxa_iva")]
            public required string PadraoRegexTaxaIva { get; set; }
        }

        public static List<SupplierPattern> LoadSupplierPatterns(string jsonFilePath)
        {
            // Load the JSON file and deserialize it into a list of SupplierPattern objects.
            var jsonContent = File.ReadAllText(jsonFilePath);
            return JsonConvert.DeserializeObject<List<SupplierPattern>>(jsonContent) ?? new List<SupplierPattern>();
        }
    }
}