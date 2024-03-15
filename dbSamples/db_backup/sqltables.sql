USE [sweet]
GO
/****** Object:  Table [dbo].[regex]    Script Date: 15/03/2024 19:17:51 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[supplierOrderItems]    Script Date: 15/03/2024 19:17:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[supplierOrderItems](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[orderId] [int] NOT NULL,
	[ref] [char](8) NOT NULL,
	[subType] [char](2) NOT NULL,
	[qntOrder] [int] NOT NULL,
	[qntOrderBonus] [int] NOT NULL,
	[qntDelivery] [int] NOT NULL,
	[qntDeliveryBonus] [int] NOT NULL,
	[deliveryDate] [smalldatetime] NULL,
	[priceNoBonus] [money] NOT NULL,
	[priceWithBonus] [money] NOT NULL,
	[isStockUpdated] [bit] NOT NULL,
	[supplierInvoiceNumber] [varchar](15) NULL,
	[isFactUpdated] [bit] NOT NULL,
	[obs] [nvarchar](max) NULL,
 CONSTRAINT [PK_supplierOrderItems] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[supplierOrders]    Script Date: 15/03/2024 19:17:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[supplierOrders](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[orderDate] [smalldatetime] NOT NULL,
	[supplierId] [int] NOT NULL,
	[deliveryDate] [smalldatetime] NULL,
	[isDeliveryClosed] [bit] NOT NULL,
	[isClosed] [bit] NOT NULL,
	[orderType] [tinyint] NOT NULL,
	[obs] [nvarchar](max) NULL,
	[deliveryDateConfirmation] [smalldatetime] NULL,
	[firstAccessConfirmation] [smalldatetime] NULL,
	[dataSubmissionConfirmation] [smalldatetime] NULL,
	[supplierOrderNumber] [varchar](30) NULL,
 CONSTRAINT [PK_supplierOrders] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[suppliersName]    Script Date: 15/03/2024 19:17:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[suppliersName](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[supplierName] [varchar](25) NOT NULL,
	[supplierContactName] [nvarchar](200) NULL,
	[supplierContactEmail] [nvarchar](200) NULL,
	[supplierContactPhone] [varchar](50) NULL,
	[supplierContactLang] [char](2) NULL,
	[supplierLeavePending] [bit] NOT NULL,
	[supplierReturnsContactName] [nvarchar](200) NULL,
	[supplierReturnsContactEmail] [nvarchar](200) NULL,
	[workdaysToDeliver] [smallint] NULL,
	[discountPP] [decimal](3, 1) NOT NULL,
	[discountAffectIva] [bit] NOT NULL,
	[cobraIva] [bit] NOT NULL,
	[daysToPay] [int] NOT NULL,
	[contributeToLeadTime] [bit] NULL,
	[isActive] [bit] NOT NULL,
	[obsPrivate] [nvarchar](max) NULL,
	[obsShared] [nvarchar](max) NULL,
 CONSTRAINT [PK_suppliersName] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
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
ALTER TABLE [dbo].[supplierOrderItems] ADD  CONSTRAINT [DF_supplierOrderItems_qntOrderBonus]  DEFAULT ((0)) FOR [qntOrderBonus]
GO
ALTER TABLE [dbo].[supplierOrderItems] ADD  CONSTRAINT [DF_supplierOrderItems_quantityDelivery]  DEFAULT ((0)) FOR [qntDelivery]
GO
ALTER TABLE [dbo].[supplierOrderItems] ADD  CONSTRAINT [DF_supplierOrderItems_quantityBonus_1]  DEFAULT ((0)) FOR [qntDeliveryBonus]
GO
ALTER TABLE [dbo].[supplierOrderItems] ADD  CONSTRAINT [DF_supplierOrderItems_isStockUpdated]  DEFAULT ((-1)) FOR [isStockUpdated]
GO
ALTER TABLE [dbo].[supplierOrderItems] ADD  CONSTRAINT [DF_supplierOrderItems_isFactUpdated_1]  DEFAULT ((0)) FOR [isFactUpdated]
GO
ALTER TABLE [dbo].[supplierOrders] ADD  CONSTRAINT [DF_supplierOrders_isDeliveryClosed_1]  DEFAULT ((0)) FOR [isDeliveryClosed]
GO
ALTER TABLE [dbo].[supplierOrders] ADD  CONSTRAINT [DF_supplierOrders_isClosed_1]  DEFAULT ((0)) FOR [isClosed]
GO
ALTER TABLE [dbo].[supplierOrders] ADD  CONSTRAINT [DF_supplierOrders_orderType]  DEFAULT ((1)) FOR [orderType]
GO
ALTER TABLE [dbo].[suppliersName] ADD  CONSTRAINT [DF_suppliersName_workdaysToDeliver]  DEFAULT ((0)) FOR [workdaysToDeliver]
GO
ALTER TABLE [dbo].[suppliersName] ADD  CONSTRAINT [DF_suppliersName_discountPP]  DEFAULT ((0)) FOR [discountPP]
GO
ALTER TABLE [dbo].[suppliersName] ADD  CONSTRAINT [DF_suppliersName_discountAffectIva]  DEFAULT ((0)) FOR [discountAffectIva]
GO
ALTER TABLE [dbo].[suppliersName] ADD  CONSTRAINT [DF_suppliersName_defaultIVA]  DEFAULT ((0)) FOR [cobraIva]
GO
ALTER TABLE [dbo].[suppliersName] ADD  CONSTRAINT [DF_suppliersName_daysToPay]  DEFAULT ((0)) FOR [daysToPay]
GO
ALTER TABLE [dbo].[suppliersName] ADD  CONSTRAINT [DF_suppliersName_contributeToLeadTime]  DEFAULT ((1)) FOR [contributeToLeadTime]
GO
ALTER TABLE [dbo].[suppliersName] ADD  CONSTRAINT [DF_suppliersName_isActive]  DEFAULT ((1)) FOR [isActive]
GO
ALTER TABLE [dbo].[supplierOrderItems]  WITH CHECK ADD  CONSTRAINT [FK_supplierOrderItems_supplierOrders] FOREIGN KEY([orderId])
REFERENCES [dbo].[supplierOrders] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[supplierOrderItems] CHECK CONSTRAINT [FK_supplierOrderItems_supplierOrders]
GO
ALTER TABLE [dbo].[supplierOrders]  WITH CHECK ADD  CONSTRAINT [FK_supplierOrders_supplierOrders] FOREIGN KEY([supplierId])
REFERENCES [dbo].[suppliersName] ([ID])
GO
ALTER TABLE [dbo].[supplierOrders] CHECK CONSTRAINT [FK_supplierOrders_supplierOrders]
GO
