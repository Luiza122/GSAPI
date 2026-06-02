# Evidências de execução - AgroOrbit API

Este arquivo indica quais evidências devem ser apresentadas na entrega da disciplina.

## 1. Como rodar a API

No terminal, execute:

```bash
git clone https://github.com/Luiza122/GSC-.git
cd GSC-/AgroOrbit.Api/AgroOrbit.Api
dotnet restore
dotnet run
```

Depois, abra o Swagger:

```text
http://localhost:5000/swagger
```

Se o terminal mostrar outra porta, use a porta indicada pelo `dotnet run`.

## 2. Prints obrigatórios recomendados

### Print 1 - Terminal com a API rodando

Mostrar:

- comando `dotnet run`;
- mensagem de aplicação iniciada;
- URL local da API.

### Print 2 - Swagger aberto

Mostrar:

- URL `/swagger`;
- lista de endpoints da API.

### Print 3 - GET /api/fazendas

Endpoint:

```http
GET /api/fazendas
```

Objetivo:

- provar que o banco SQLite foi criado;
- mostrar a fazenda inicial criada pelo seed.

### Print 4 - GET /api/equipamentos

Endpoint:

```http
GET /api/equipamentos
```

Objetivo:

- mostrar satélite, drone e sensor IoT cadastrados.

### Print 5 - POST /api/monitoramento/leituras-satelite

Endpoint:

```http
POST /api/monitoramento/leituras-satelite
```

JSON para enviar:

```json
{
  "talhaoId": 1,
  "sateliteId": 1,
  "indiceSaude": 0.38,
  "umidadeEstimada": 22,
  "capturadoEmUtc": "2026-06-08T10:00:00Z"
}
```

Objetivo:

- provar que uma leitura de satélite foi registrada;
- mostrar que a API gerou alertas automaticamente.

### Print 6 - GET /api/alertas

Endpoint:

```http
GET /api/alertas
```

Objetivo:

- mostrar os alertas criados pela leitura de satélite.

### Print 7 - GET /api/dashboard/1

Endpoint:

```http
GET /api/dashboard/1
```

Objetivo:

- mostrar o dashboard central da fazenda;
- exibir status da lavoura, quantidade de alertas e média de saúde.

### Print 8 - POST /api/relatorios/semanal/1

Endpoint:

```http
POST /api/relatorios/semanal/1
```

Objetivo:

- mostrar o relatório semanal gerado automaticamente.

## 3. Ordem recomendada para apresentação

1. Abrir o Swagger.
2. Executar `GET /api/fazendas`.
3. Executar `GET /api/equipamentos`.
4. Executar `POST /api/monitoramento/leituras-satelite`.
5. Executar `GET /api/alertas`.
6. Executar `GET /api/dashboard/1`.
7. Executar `POST /api/relatorios/semanal/1`.

Essa ordem demonstra o fluxo completo da solução: monitoramento por satélite, geração de alerta, visualização no dashboard e criação de relatório semanal.

## 4. Observação para a entrega

As imagens dos prints podem ser anexadas diretamente no Teams ou colocadas em uma pasta chamada `docs/prints` no repositório.
