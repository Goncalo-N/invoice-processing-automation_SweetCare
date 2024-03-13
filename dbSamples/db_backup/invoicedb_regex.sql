-- MySQL dump 10.13  Distrib 8.0.36, for Win64 (x86_64)
--
-- Host: localhost    Database: invoicedb
-- ------------------------------------------------------
-- Server version	8.0.36

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `regex`
--

DROP TABLE IF EXISTS `regex`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `regex` (
  `id` int NOT NULL AUTO_INCREMENT,
  `nome_empresa` varchar(255) NOT NULL,
  `padrao_regex_nome_fornecedor` varchar(255) DEFAULT NULL,
  `padrao_regex_data_fatura` varchar(255) DEFAULT NULL,
  `padrao_regex_numero_encomenda` varchar(255) DEFAULT NULL,
  `padrao_regex_numero_fatura` varchar(255) DEFAULT NULL,
  `padrao_regex_data_vencimento_fatura` varchar(255) DEFAULT NULL,
  `padrao_regex_total_sem_iva` varchar(255) DEFAULT NULL,
  `padrao_regex_totais_por_iva` varchar(255) DEFAULT NULL,
  `padrao_regex_valor_iva` varchar(255) DEFAULT NULL,
  `padrao_regex_desconto_pronto_pagamento` varchar(255) DEFAULT NULL,
  `padrao_regex_total_a_pagar` varchar(255) DEFAULT NULL,
  `padrao_regex_produto` varchar(255) DEFAULT NULL,
  `padrao_regex_taxa_iva` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `regex`
--

LOCK TABLES `regex` WRITE;
/*!40000 ALTER TABLE `regex` DISABLE KEYS */;
INSERT INTO `regex` VALUES (2,'Roger & Gallet','\\bRoger\\s*&\\s*Gallet\\b','\\b\\d{2}[/-]\\d{2}[/-]\\d{4}|\\d{4}[/-]\\d{2}[/-]\\d{2}\\b','\\b[A-Za-z]+\\d+CRM\\d+\\b','\\b[A-Za-z]+\\d+FAC\\d+\\b','\\b\\d{2}[/-]\\d{2}[/-]\\d{4}|\\d{4}[/-]\\d{2}[/-]\\d{2}\\b','Total sem IVA\\s*([\\d,]+)\\s*','Padrão Regex para Totais por IVA','IVA\\s*%?\\s*(d+(?:\\.\\d+)?)s+\\d+(?:\\.\\d+)?','Padrão Regex para Desconto Pronto Pagamento','Total com IVA\\s*([\\d,]+)\\s*EUR','(?<Article>\\w+)\\s+(?<Barcode>\\d+)\\s+(?<Description>.+?)\\s+(?<Quantity>\\d+)\\s+(?<GrossPrice>[\\d,]+)\\s+(?<Discount>[\\d,]+)\\s+(?<PrecoSemIVA>[\\d,]+)\\s+(?<PrecoComIVA>[\\d,]+)','(\\d+,\\d{2})\\s+%\\s+(\\d+,\\d{2})\\s+€'),(3,'MORENO II','\\bMORENO\\s*+II\\b','\\b\\d{2}[.-]\\d{2}[.-]\\d{4}|\\d{4}[/-]\\d{2}[/-]\\d{2}\\b','Sweetcare\\s+Nº\\s*:\\s*(\\d+)','(?<=Fatura Nº\\s)\\d+','Vencimento:\\s*\\b\\d{2}[\\/\\.-]\\d{2}[\\/\\.-]\\d{4}|\\d{4}[\\/\\.-]\\d{2}[\\/\\.-]\\d{2}\\b','(?<=Base de Incidência:\\s*)([\\d,]+)','Padrão Regex para Totais por IVA','IVA\\s*%?\\s*(d+(?:\\.\\d+)?)s+\\d+(?:\\.\\d+)?','Padrão Regex para Desconto Pronto Pagamento','([\\d,]+)\\s*TOTAL','(?<Article>\\w+)\\s+(?<Barcode>\\d+)\\s+(?<Description>.+?)\\s+(?<Quantity>\\d+)\\s+(?<GrossPrice>[\\d,]+)\\s+(?<Discount>[\\d,]+)\\s+(?<PrecoSemIVA>[\\d,]+)\\s+(?<PrecoComIVA>[\\d,]+)','Padrão Regex para Taxa de IVA'),(4,'LABORATORIOS EXPANSCIENCE','LABORATORIOS EXPANSCIENCE','\\b\\d{2}[.-]\\d{2}[.-]\\d{4}|\\d{4}[/-]\\d{2}[/-]\\d{2}','(?<=N\\s+encomen\\s+\\w+\\s+\\w+\\s+\\w+\\s+\\w+\\s+\\w+\\s+\\w+\\s)(\\d+)','(?<=FT S/)(\\d+)','Vencimento:\\s*\\b\\d{2}[\\/\\.-]\\d{2}[\\/\\.-]\\d{4}|\\d{4}[\\/\\.-]\\d{2}[\\/\\.-]\\d{2}\\b','Total líquido\\s+([\\d\\.,]+)','Padrão Regex para Totais por IVA','IVA\\s*%?\\s*(d+(?:\\.\\d+)?)s+\\d+(?:\\.\\d+)?','Padrão Regex para Desconto Pronto Pagamento','Total fatura\\s+([\\d\\.,]+) EUR','(\\bPT?\\d+|\\b\\d+)\\s+(.+?)\\s+(\\d+)\\s*([-\\w]*)\\s+(\\d+\\sUN)\\s+(\\d+\\.\\d{2})\\s+(\\d+\\.\\d{2})\\s+(\\d+\\.\\d{2})\\s+(\\d+)','\\b(0\\.00|6\\.00|13\\.00|23\\.00)\\s+(\\d+\\.\\d{2})');
/*!40000 ALTER TABLE `regex` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2024-03-13 17:18:52
