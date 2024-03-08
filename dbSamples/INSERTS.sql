-- Active: 1709306145245@@127.0.0.1@3306@invoicedb
INSERT INTO Regex (
    nome_empresa,
    padrao_regex_nome_fornecedor,
    padrao_regex_data_fatura,
    padrao_regex_numero_encomenda,
    padrao_regex_numero_fatura,
    padrao_regex_data_vencimento_fatura,
    padrao_regex_total_sem_iva,
    padrao_regex_totais_por_iva,
    padrao_regex_valor_iva,
    padrao_regex_desconto_pronto_pagamento,
    padrao_regex_total_a_pagar,
    padrao_regex_produto,
    padrao_regex_taxa_iva
) VALUES (
    'Roger & Gallet',
    '\\bRoger\\s*&\\s*Gallet\\b',
    'Data\\s+da\\s+Fatura\\s+(\\d{2}[/-]\d{2}[/-]\\d{4}|\d{4}[/-]\\d{2}[/-]\\d{2})',
    '\\b[A-Za-z]+\\d+CRM\\d+\\b',
    '\\b[A-Za-z]+\\d+FAC\\d+\\b',
    'Vencimento:\\s*\\b\\d{2}[\\/\\.-]\\d{2}[\\/\\.-]\\d{4}|\\d{4}[\\/\\.-]\\d{2}[\\/\\.-]\\d{2}\\b',
    'Total sem IVA\s*([\\d,]+)\\s*',
    'Padrão Regex para Totais por IVA',
    'IVA\\s*%?\\s*(\d+(?:\\.\\d+)?)\s+\\d+(?:\\.\\d+)?',
    'Padrão Regex para Desconto Pronto Pagamento',
    'Total com IVA\\s*([\\d,]+)\\s*EUR',
    '(?<Article>\\w+)\\s+(?<Barcode>\\d+)\\s+(?<Description>.+?)\\s+(?<Quantity>\\d+)\\s+(?<GrossPrice>[\\d,]+)\\s+(?<Discount>[\\d,]+)\\s+(?<PrecoSemIVA>[\\d,]+)\\s+(?<PrecoComIVA>[\\d,]+)',
    'Padrão Regex para Taxa de IVA'
);

INSERT INTO Regex (
    nome_empresa,
    padrao_regex_nome_fornecedor,
    padrao_regex_data_fatura,
    padrao_regex_numero_encomenda,
    padrao_regex_numero_fatura,
    padrao_regex_data_vencimento_fatura,
    padrao_regex_total_sem_iva,
    padrao_regex_totais_por_iva,
    padrao_regex_valor_iva,
    padrao_regex_desconto_pronto_pagamento,
    padrao_regex_total_a_pagar,
    padrao_regex_produto,
    padrao_regex_taxa_iva
) VALUES (
    'Moreno II',
    'MORENO\\s+II',
    'Data\\s+da\\s+Fatura\\s+(\\d{2}[/-]\d{2}[/-]\\d{4}|\d{4}[/-]\\d{2}[/-]\\d{2})',
    'Sweetcare\\s+Nº\\s*:\\s*(\\d+)',
    'Fatura\\s*Nº\\s*:\\s*(\\d+)',
    'Vencimento:\\s*\\b\\d{2}[\\/\\.-]\\d{2}[\\/\\.-]\\d{4}|\\d{4}[\\/\\.-]\\d{2}[\\/\\.-]\\d{2}\\b',
    'Total sem IVA\s*([\\d,]+)\\s*',
    'Padrão Regex para Totais por IVA',
    'IVA\\s*%?\\s*(\d+(?:\\.\\d+)?)\s+\\d+(?:\\.\\d+)?',
    'Padrão Regex para Desconto Pronto Pagamento',
    'Total com IVA\\s*([\\d,]+)\\s*EUR',
    '(?<CNP>\\S+)\\s+(?<Designation>.+?)\\s+(?<Lot>\\S+)\\s+(?<ExpiryDate>\\d{2}\\.\\d{2}\\.\\d{4})\\s+(?<Type>.+?)\\s+(?<Quantity>\\d+)\\s+(?<UnitPrice>[\\d,]+)\\s+(?<Discount1>[\\d,]+)\\s+(?<Discount2>[\\d,]+)\\s+(?<NetPrice>[\\d,]+)\\s+(?<IVA>\\d+)%\\s+(?<Total>[\\d,]+)',
    'Padrão Regex para Taxa de IVA'
);
-- insert for LEX Invoices
INSERT INTO Regex (
    nome_empresa,
    padrao_regex_nome_fornecedor,
    padrao_regex_data_fatura,
    padrao_regex_numero_encomenda,
    padrao_regex_numero_fatura,
    padrao_regex_data_vencimento_fatura,
    padrao_regex_total_sem_iva,
    padrao_regex_totais_por_iva,
    padrao_regex_valor_iva,
    padrao_regex_desconto_pronto_pagamento,
    padrao_regex_total_a_pagar,
    padrao_regex_produto,
    padrao_regex_taxa_iva
) VALUES (
    'LEX',
    'LEX',
    'Data\\s+da\\s+Fatura\\s+(\\d{2}[/-]\d{2}[/-]\\d{4}|\d{4}[/-]\\d{2}[/-]\\d{2})',
    'Sweetcare\\s+Nº\\s*:\\s*(\\d+)',
    'Fatura\\s*Nº\\s*:\\s*(\\d+)',
    'Vencimento:\\s*\\b\\d{2}[\\/\\.-]\\d{2}[\\/\\.-]\\d{4}|\\d{4}[\\/\\.-]\\d{2}[\\/\\.-]\\d{2}\\b',
    'Total sem IVA\s*([\\d,]+)\\s*',
    'Padrão Regex para Totais por IVA',
    'IVA\\s*%?\\s*(\d+(?:\\.\\d+)?)\s+\\d+(?:\\.\\d+)?',
    'Padrão Regex para Desconto Pronto Pagamento',
    'Total com IVA\\s*([\\d,]+)\\s*EUR',
    '(\w+)\s+([A-Z\s\d\-\+ML%]+)\s+(\d+)\s+([\w\-]+)\s+(\d+\sUN)\s+(\d+\.\d{2})\s+(\d+\.\d{2})\s+(\d+\.\d{2})\s+(\d)',
    'Padrão Regex para Taxa de IVA'
);