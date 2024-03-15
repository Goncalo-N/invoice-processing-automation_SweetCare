SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[regex](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[nome_empresa] [varchar](255) NOT NULL,
	[padrao_regex_nome_fornecedor] [varchar](255) NULL,
	[padrao_regex_data_fatura] [varchar](255) NULL,
	[padrao_regex_numero_encomenda] [varchar](255) NULL,
	[padrao_regex_numero_fatura] [varchar](255) NULL,
	[padrao_regex_data_vencimento_fatura] [varchar](255) NULL,
	[padrao_regex_total_sem_iva] [varchar](255) NULL,
	[padrao_regex_totais_por_iva] [varchar](255) NULL,
	[padrao_regex_valor_iva] [varchar](255) NULL,
	[padrao_regex_desconto_pronto_pagamento] [varchar](255) NULL,
	[padrao_regex_total_a_pagar] [varchar](255) NULL,
	[padrao_regex_produto] [varchar](255) NULL,
	[padrao_regex_taxa_iva] [varchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[regex] ADD  DEFAULT (NULL) FOR [padrao_regex_nome_fornecedor]
GO
ALTER TABLE [dbo].[regex] ADD  DEFAULT (NULL) FOR [padrao_regex_data_fatura]
GO
ALTER TABLE [dbo].[regex] ADD  DEFAULT (NULL) FOR [padrao_regex_numero_encomenda]
GO
ALTER TABLE [dbo].[regex] ADD  DEFAULT (NULL) FOR [padrao_regex_numero_fatura]
GO
ALTER TABLE [dbo].[regex] ADD  DEFAULT (NULL) FOR [padrao_regex_data_vencimento_fatura]
GO
ALTER TABLE [dbo].[regex] ADD  DEFAULT (NULL) FOR [padrao_regex_total_sem_iva]
GO
ALTER TABLE [dbo].[regex] ADD  DEFAULT (NULL) FOR [padrao_regex_totais_por_iva]
GO
ALTER TABLE [dbo].[regex] ADD  DEFAULT (NULL) FOR [padrao_regex_valor_iva]
GO
ALTER TABLE [dbo].[regex] ADD  DEFAULT (NULL) FOR [padrao_regex_desconto_pronto_pagamento]
GO
ALTER TABLE [dbo].[regex] ADD  DEFAULT (NULL) FOR [padrao_regex_total_a_pagar]
GO
ALTER TABLE [dbo].[regex] ADD  DEFAULT (NULL) FOR [padrao_regex_produto]
GO
ALTER TABLE [dbo].[regex] ADD  DEFAULT (NULL) FOR [padrao_regex_taxa_iva]
GO
