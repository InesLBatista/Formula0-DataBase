# Fórmula 0
# Autores
- Inês Batista, 124877
- Maria Quinteiro, 124996

## Descrição do Trabalho
Este projeto consiste na conceção e implementação de uma base de dados relacional completa para a gestão de um campeonato de corridas automóveis inspirado na Fórmula 1. O objetivo principal foi modelar, estruturar e validar um esquema que permita suportar todas as entidades e regras necessárias para registar temporadas, corridas, sessões (treinos, qualificação, corrida), circuitos, equipas, pilotos, membros do staff, contratos, penalizações, pitstops e resultados detalhados por sessão.

O trabalho inclui: (i) a modelagem conceptual com um Diagrama Entidade-Relacionamento (DER) que traduz as entidades essenciais e as respetivas cardinalidades; (ii) a transformação do modelo conceptual em um esquema relacional normalizado com chaves primárias e estrangeiras (ER / esquema relacional); (iii) a criação dos scripts DDL para gerar as tabelas, índices e constraints; (iv) procedimentos armazenados e triggers para lógica de negócio (e.g., atualização automática de classificações e validação de integridade transacional); e (v) scripts de inicialização com dados de exemplo e conjuntos de testes para validação funcional.

Principais características e decisões de projeto:
- Modelagem das relações entre `Season`, `GrandPrix`, `Session`, `Circuit`, `Driver`, `Team`, `Result`, e `Contract`, de modo a permitir consultas históricas e agregações por temporada, por piloto e por equipa.
- Normalização até 3FN para reduzir duplicação and garantir consistência; índices selecionados para acelerar consultas típicas (classificações, resultados por corrida, histórico de um piloto).
- Procedimentos armazenados para operações complexas (por exemplo, computar a tabela de pontuação após uma corrida) e triggers para manter campos derivados atualizados.
- Utilização de transações para operações de escrita críticas (registo de resultados, aplicação de penalizações) para manter a integridade dos dados.

## Demo
Caso o repositório esteja a ser visualizado localmente, o ficheiro de vídeo encontra-se em `APF_124877_124996/Demo.mp4`.



<p align="center">
	<a href="APF_124877_124996/Demo.mp4">
		<img src="APF_124877_124996/demo.gif" alt="Demo GIF" width="720" />
	</a>
</p>




