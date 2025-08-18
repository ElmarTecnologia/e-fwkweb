using System.Collections.Generic;

namespace ELMAR.DevHtmlHelper.Helpers
{
    public static class TurmalinaTags
    {
        public static Dictionary<string, Dictionary<string, string>> dicItemProps =
        new Dictionary<string, Dictionary<string, string>>()
        {
            { //Módulo Despesa Extra Orçamentária 
                "DespesaExtraOramentria",
                new Dictionary<string, string>(){
                    { "DespesaExtraOramentria", "https://turmalina.tcepb.tc.br/documentation/ExtraBudgetExpenditure" },
                    { "Cod. Órgão", "managementUnitID" }, 
                    { "Órgão", "managementUnitName" },
                    { "Data", "moveDate" },
                    { "Código", "extraBudgetExpenditureID" },
                    { "Nº Guia", "tabID" }, 
                    { "Guia Data", "tabDate" }, 
                    { "Fornecedor", "creditorName" }, 
                    { "CPF|CNPJ", "identificationNumber" }, 
                    { "Descrição", "extraBudgetExpenditureDescription" },
                    { "Nomenclatura", "extraBudgetExpenditureNomenclature" },
                    { "Histórico", "tabHistory" },                     
                    { "Valor", "paymentAmount" }
                }
            },
            {  //Módulo Empenhos (Despesa Orçamentária)
                "DespesaEmpenhos",
                new Dictionary<string, string>(){
                    { "DespesaEmpenhos", "https://turmalina.tcepb.tc.br/documentation/BudgetExpenditure" },
                    { "Cod. Órgão", "managementUnitID" }, 
                    { "Órgão", "managementUnitName" },
                    { "Função", "budgetExpenditureFunction" },
                    { "Sub-Função", "budgetExpenditureSubfunction" },
                    { "Programa", "budgetExpenditureProgram" }, 
                    { "Ação", "budgetExpenditureAction" }, 
                    { "Empenho", "comittedExpenditureID" }, 
                    { "Data", "comittedExpenditureDate" },
                    { "Classificação", "economicCategory" },
                    { "Natureza", "budgetNature" }, 
                    { "Modalidade", "budgetExpenditureModality" }, 
                    { "Elemento", "budgetExpenditureElement" }, 
                    { "CPF|CNPJ", "identificationNumber" },
                    { "Fornecedor", "creditorName" },
                    { "Licitação", "bidID" },
                    { "Modalidade Licitação", "bidModality" }, 
                    { "F.Recurso", "" },
                    { "Valor", "comittedValue" },
                    { "Valor Orçado", "fixedAmount" }, 
                    { "Valor Pagamento", "paymentAmount" },
                    { "Histórico do Empenho", "comittedExpenditureHistory" },
                    { "Especificação", "comittedExpenditureHistory" }
                }
            },
            {  //Módulo Empenhos (Despesa Orçamentária)
                "DespesaSubEmpenhos",
                new Dictionary<string, string>(){
                    { "DespesaSubEmpenhos", "https://turmalina.tcepb.tc.br/documentation/BudgetExpenditure" },
                    { "Órgão", "managementUnitName" },
                    { "Empenho", "comittedExpenditureID" },
                    { "Data", "comittedExpenditureDate" },
                    { "Classificação", "economicCategory" },
                    { "CPF|CNPJ", "identificationNumber" },
                    { "Fornecedor", "creditorName" },
                    { "Licitação", "bidID" },
                    { "Modalidade", "bidModality" },
                    { "F.Recurso", "" },
                    { "Valor", "comittedValue" },
                    { "Histórico do Empenho", "comittedExpenditureHistory" },
                    { "Especificação", "comittedExpenditureHistory" }
                }
            },            
            {  //Módulo Despesa Pagamento
                "DespesasPagamentos",
                new Dictionary<string, string>(){
                    { "DespesasPagamentos", "https://turmalina.tcepb.tc.br/documentation/PaymentDocument" },
                    { "Cod. Órgão", "managementUnitID" }, 
                    { "Órgão", "managementUnitName" }, 
                    { "Empenho", "comittedExpenditureID" },
                    { "Data", "paymentDate" },
                    { "Fonte Recurso", "fundingSource" },
                    { "Histórico", "paymentHistory" },
                    { "Parcela", "" },
                    { "Licitação", "bidID" },
                    { "Modalidade", "bidModality" },
                    { "Fornecedor", "creditorName" },
                    { "Conta", "bankAccountNumber" },
                    { "Tipo Transação", "bankOperationID" },
                    { "Número da Transação", "identificationNumber" },
                    { "Valor", "paymentAmount" },
                    { "Retenção", "" },
                    { "Valor Líquido", "" },
                    { "Elemento Despesa", "" }
                }
            },            
            {  //Módulo Convênios
                "Convnios",
                new Dictionary<string, string>(){
                    { "Convnios", "https://turmalina.tcepb.tc.br/documentation/Agreement" },
                    { "Cod. Órgão", "managementUnitID" },
                    { "Órgão", "managementUnitName" },
                    { "Código do Convênio", "agreementID" },
                    { "Dígito", "" },
                    { "Publicação", "celebrationDate" },
                    { "Início Vigência", "publicationDate" },
                    { "Final Vigência", "validityDate" },
                    { "Descrição", "object" },
                    { "Concedente", "grantorName" },
                    { "CNPJ", "identificationNumber" },
                    { "Origem", "contractorName" },
                    { "Total", "agreementAmount" },
                    { "Valor Concedente", "" },
                    { "Valor Convedente", "counterpartAmount" },
                    { "Saldo do Exercício Anterior", "" },
                    { "Receita do Exercício Anterior", "" },
                    { "Empenhado no Exercício Anterior", "" },
                    { "Pago no Exercício Anterior", "" }
                }
            },
            {  //Módulo Licitação
                "Licitaes",
                new Dictionary<string, string>(){
                    { "Licitaes", "https://turmalina.tcepb.tc.br/documentation/Bidding" },
                    { "Cod. Órgão", "managementUnitID" }, 
                    { "Órgão", "managementUnitName" },
                    { "Docs", "" },
                    { "Edital", "notice" },
                    { "Nº Licitação", "bidID" },
                    { "Data", "publicationDate" },
                    { "Modalidade", "bidModality" },
                    { "Valor Estimado", "bidderProposalAmount" },
                    { "Valor", "paymentAmount" },
                    { "Homologação", "realizationDate" },
                    { "Participantes", "bidderName" }, 
                    { "CPF|CNPJ", "identificationNumber" }, 
                    { "Objeto", "object" },
                    { "Nº Processo", "" },
                    { "Situação", "" },
                    { "Tipo Objeto", "" },
                    { "Nº Propostas", "" }
                }
            },            
            {  //Módulo Licitação Contratos
                "LicitaesContratos",
                new Dictionary<string, string>(){
                    { "LicitaesContratos", "https://turmalina.tcepb.tc.br/documentation/Contract" },
                    { "Cod. Órgão", "managementUnitID" }, 
                    { "Órgão", "managementUnitName" },
                    { "Nº Contrato", "contractID" },
                    { "Objeto", "object" },
                    { "CPF|CNPJ", "identificationNumber" },
                    { "Fornecedor", "contractorName" },
                    { "Publicação", "publicationDate" },
                    { "Assinatura", "validityDate" },
                    { "Vigencia", "validityDate" },
                    { "Valor", "contractAmount" },
                    { "Nome do Fiscal", "" },
                    { "Função do Fiscal", "" },
                    { "Observações", "" }
                }
            },
            { //Módulo Receita Extra Orçamentária 
                "ReceitaExtraOramentria",
                new Dictionary<string, string>(){
                    { "ReceitaExtraOramentria", "https://turmalina.tcepb.tc.br/documentation/ExtraBudgetRevenue" },
                    { "Cod. Órgão", "managementUnitID" }, 
                    { "Órgão", "managementUnitName" },
                    { "Data", "moveDate" },
                    { "Código", "extraBudgetRevenueID" },
                    { "Classificação", "extraBudgetRevenueSource" }, 
                    { "Descrição", "extraBudgetRevenueDescription" },
                    { "Nomenclatura", "nomenclature" }, 
                    { "Histórico", "extraBudgetRevenueHistory" }, 
                    { "Valor", "realizedAmount" }
                }
            },
            { //Módulo Receita Prevista
                "ReceitaPrevista",
                new Dictionary<string, string>(){
                    { "ReceitaPrevista", "https://turmalina.tcepb.tc.br/documentation/BudgetRevenue" },
                    { "Cod. Órgão", "managementUnitID" }, 
                    { "Órgão", "managementUnitName" },
                    { "Cód.Receita", "budgetRevenueSource" },
                    { "Descrição", "budgetRevenueDescription" },
                    { "Receita Prevista", "predictedAmount " },
                    { "Realizada no Mês", "collectionAmount" },
                    { "Realizada Até o Mês", "" },
                    { "Diferença para Mais", "" },
                    { "Diferença para Menos", "" }
                }
            },
            {  //Módulo Pessoal (Folha)
                "FolhadePagamento",
                new Dictionary<string, string>(){
                    { "FolhadePagamento", "https://turmalina.tcepb.tc.br/documentation/EmployeeInformation" },
                    { "Matrícula", "" },
                    { "Nome", "employeeName" },
                    { "CPF", "identificationNumber" },
                    { "Cargo", "employeePosition" },
                    { "Secretaria", "" },
                    { "Regime", "employmentContractType" },
                    { "Dt. Admissão", "" },
                    { "Vantagens", "employeeSalary" },
                    { "Descontos", "" },
                    { "Líquido", "employeeSalary" },
                }
            },
            {  //Módulo Pessoal (Quadro Funcional)
                "QuadroFuncional",
                new Dictionary<string, string>(){
                    { "QuadroFuncional", "https://turmalina.tcepb.tc.br/documentation/EmployeeInformation" },
                    { "Matrícula", "" },
                    { "Nome", "employeeName" },
                    { "CPF", "identificationNumber" },
                    { "Cargo", "employeePosition" },
                    { "Secretaria", "" },
                    { "Regime", "employmentContractType" },
                    { "Dt. Admissão", "" },
                    { "Vantagens", "employeeSalary" },
                    { "Descontos", "" },
                    { "Líquido", "employeeSalary" },
                }
            },
            {  //Módulo Pessoal (Serv. Temporários)
                "ServidoresTemporrios",
                new Dictionary<string, string>(){
                    { "ServidoresTemporrios", "https://turmalina.tcepb.tc.br/documentation/EmployeeInformation" },
                    { "Matrícula", "" },
                    { "Nome", "employeeName" },
                    { "CPF", "identificationNumber" },
                    { "Cargo", "employeePosition" },
                    { "Secretaria", "" },
                    { "Regime", "employmentContractType" },
                    { "Dt. Admissão", "" },
                    { "Vantagens", "employeeSalary" },
                    { "Descontos", "" },
                    { "Líquido", "employeeSalary" },
                }
            },
            {  //Módulo Pessoal (Serv. Cedidos)
                "ServidoresCedidos",
                new Dictionary<string, string>(){
                    { "ServidoresCedidos", "https://turmalina.tcepb.tc.br/documentation/EmployeeInformation" },
                    { "Matrícula", "" },
                    { "Nome", "employeeName" },
                    { "CPF", "identificationNumber" },
                    { "Cargo", "employeePosition" },
                    { "Secretaria", "" },
                    { "Regime", "employmentContractType" },
                    { "Dt. Admissão", "" },
                    { "Vantagens", "employeeSalary" },
                    { "Descontos", "" },
                    { "Líquido", "employeeSalary" },
                }
            },
            {  //Módulo Pessoal (Serv. Cedidos)
                "InstrumentosdePlanejamento",
                new Dictionary<string, string>(){
                    { "InstrumentosdePlanejamento", "https://turmalina.tcepb.tc.br/documentation/PlanningInstrument" },
                    { "Lei de Diretrizes Orçamentárias", "budgetGuidelinesLaw" },
                    { "Lei Orçamentária Anual", "annualBudgetLaw" },
                    { "Plano Pluri Anual", "multiyearPlan" }
                } //TODO: Adicionar leitura das tags nos headers
            },
            {  //Módulo Pessoal (Serv. Cedidos)
                "e-sic",
                new Dictionary<string, string>(){
                    { "e-sic", "tm-sic" }
                }
            }
        };
    }
}