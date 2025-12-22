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
Veja a demonstração em vídeo abaixo. Caso o repositório esteja a ser visualizado localmente, o ficheiro de vídeo encontra-se em `APF_124877_124996/Demo.mp4`.

<!-- Se o visualizador suportar HTML5 local, o player abaixo tentará reproduzir o ficheiro -->
<video controls width="720">
	<source src="APF_124877_124996/Demo.mp4" type="video/mp4">
	O seu browser não suporta o elemento <code>video</code>. Abra o ficheiro [APF_124877_124996/Demo.mp4](APF_124877_124996/Demo.mp4) manualmente.
</video>

## Diagramas
Os diagramas finais (DER e ER) estão disponíveis na pasta do trabalho `APF_124877_124996´

<!-- Embeds: muitos visualizadores Markdown locais exibem PDFs quando embutidos; em plataformas que não suportem, os links abaixo funcionam como fallback. -->

<h3>DER (Diagrama Entidade-Relacionamento)</h3>
<embed src="APF_124877_124996/der.pdf" type="application/pdf" width="100%" height="720px" />


<h3>ER (Esquema Relacional)</h3>
<embed src="APF_124877_124996/er.pdf" type="application/pdf" width="100%" height="720px" />


