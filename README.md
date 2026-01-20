# 🤖 IRibeiro For Hire - Chat API (Gemini RAG)

Backend de alta performance desenvolvido em **.NET 10** que utiliza Inteligência Artificial Generativa para responder perguntas sobre a trajetória profissional de **Itamar Ribeiro**. 

O projeto implementa uma arquitetura de **RAG (Retrieval-Augmented Generation)**, garantindo que a IA forneça respostas precisas baseadas em dados reais extraídos do currículo via busca vetorial.

---

##  Diferenciais Técnicos

### 1. Arquitetura RAG com pgvector
Diferente de um chat comum, esta API utiliza busca semântica. Quando uma pergunta é feita:
* Geramos um **Embedding** de 768 dimensões via Google Gemini API.
* Realizamos uma busca por **Similaridade de Cosseno** no PostgreSQL utilizando a extensão `pgvector`.
* O contexto recuperado é enviado junto com a pergunta para a IA, eliminando alucinações.

### 2. Pipeline de Filtros (Clean Code)
Utilizamos **Action Filters** customizados para manter os Controllers limpos e focados no negócio:
* **ValidationFilter:** Validação automática de contratos DTO.
* **RateLimitFilter:** Controle de fluxo persistente no banco de dados para evitar abusos e custos excessivos.

### 3. Rastreamento e Segurança
* **Identificação Anônima:** Uso de Cookies seguros (`HttpOnly`, `Secure`, `SameSite=None`) com hash SHA256 para identificar visitantes de forma privada.
* **Auditoria:** Registro completo de IP e histórico de interações para análise e controle de limites.

---

## 🛠️ Stack Tecnológica

* **Framework:** .NET 10 (C#)
* **IA Generativa:** Google Gemini 2.5 Flash
* **Embeddings:** Google Text-Embedding-004
* **Banco de Dados:** PostgreSQL + pgvector
* **Cloud & Deploy:** Koyeb (Backend) | Vercel (Frontend)

---

## ⚙️ Configuração Local

1. Clone o repositório:
```
git clone [https://github.com/itamar-ribeiro/iribeiro-api.git](https://github.com/itamar-ribeiro/iribeiro-api.git)
```

2. Configure as variáveis de ambiente no seu arquivo .env, a definição é encontrada no .env.dev:
	```
	GEMINI_API_KEY=sua_api_key
	DB_CONNECTION_STRING=seu_postgres_connection_string
	AI_SYSTEM_PROMPT="Instruções para a IA..."
	```

3. Rode a aplicação:
	```bash 
	$ dotnet run --project IRibeiroForHireAPI
	```

Autor:
Itamar Ribeiro 
Desenvolvedor Fullstack .NET

[Conecte-se comigo no LinkedIn](https://www.linkedin.com/in/itamar-ribeiro/)
