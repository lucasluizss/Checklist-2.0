
-- Criar o Banco Teste
CREATE DATABASE Checklist;

-- Criar a tabela Tarefas
use Checklist;
CREATE TABLE Tasks
(
	IdTask INT PRIMARY KEY AUTO_INCREMENT,
    Description VARCHAR(1000),
    Estate BOOL
);

-- Para que rode no VS deve-se rodar a linha 3 e 4 do script
use Checklist;
   SET GLOBAL optimizer_switch='derived_merge=OFF';