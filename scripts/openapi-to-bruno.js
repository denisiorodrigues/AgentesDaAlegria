'use strict';

const fs = require('fs');
const path = require('path');

const ROOT = path.resolve(__dirname, '..');
const OPENAPI_FILE = path.join(ROOT, 'openapi', 'AgentesDaAlegria.API.json');
const BRUNO_DIR = path.join(ROOT, 'bruno');

if (!fs.existsSync(OPENAPI_FILE)) {
  console.error(`Arquivo não encontrado: ${OPENAPI_FILE}`);
  console.error('Execute primeiro: dotnet build ./backend/AgentesDaAlegria.API.csproj');
  process.exit(1);
}

const spec = JSON.parse(fs.readFileSync(OPENAPI_FILE, 'utf8'));
const schemas = spec.components?.schemas ?? {};

// Gera um valor de exemplo a partir de um JSON Schema
function exemplo(schema, depth = 0) {
  if (!schema || depth > 5) return null;

  if (schema.$ref) {
    const name = schema.$ref.replace('#/components/schemas/', '');
    return exemplo(schemas[name], depth + 1);
  }

  if (schema.example !== undefined) return schema.example;
  if (schema.allOf) return exemplo(schema.allOf[0], depth + 1);
  if (schema.anyOf) return exemplo(schema.anyOf[0], depth + 1);

  switch (schema.type) {
    case 'object': {
      const obj = {};
      for (const [k, v] of Object.entries(schema.properties ?? {})) {
        obj[k] = exemplo(v, depth + 1);
      }
      return obj;
    }
    case 'array':
      return [exemplo(schema.items, depth + 1)];
    case 'string':
      if (schema.format === 'date') return '2000-01-15';
      if (schema.format === 'date-time') return '2000-01-15T00:00:00Z';
      if (schema.format === 'email') return 'usuario@exemplo.com';
      if (schema.format === 'uuid') return '00000000-0000-0000-0000-000000000000';
      if (schema.format === 'password') return 'Senha@123';
      return '';
    case 'integer':
    case 'number':
      return 0;
    case 'boolean':
      return false;
    default:
      return null;
  }
}

// Determina a pasta a partir do path da API: /api/auth/login → auth
function pastaDoPath(apiPath) {
  const segmentos = apiPath.replace(/^\//, '').split('/');
  return segmentos.find(s => s && s !== 'api' && !s.startsWith('{')) ?? 'root';
}

// Converte PascalCase/camelCase para texto legível: "EsqueciSenha" → "Esqueci Senha"
function humanizar(str) {
  return str.replace(/([A-Z])/g, ' $1').trim();
}

// Remove caracteres inválidos em nomes de arquivo
function nomeSeguro(str) {
  return str.replace(/[<>:"/\\|?*]/g, '').trim();
}

// Monta o conteúdo de um arquivo .bru
function gerarBru(method, apiPath, operation, seq) {
  const nome = operation.summary ?? humanizar(operation.operationId ?? `${method} ${apiPath}`);
  const temBody = !!operation.requestBody?.content?.['application/json'];
  const temAuth = (operation.security?.length ?? 0) > 0;
  const tipoBody = temBody ? 'json' : 'none';
  const tipoAuth = temAuth ? 'bearer' : 'none';

  const linhas = [
    'meta {',
    `  name: ${nome}`,
    '  type: http',
    `  seq: ${seq}`,
    '}',
    '',
    `${method} {`,
    `  url: {{baseUrl}}${apiPath}`,
    `  body: ${tipoBody}`,
    `  auth: ${tipoAuth}`,
    '}',
    '',
  ];

  if (temBody) {
    linhas.push('headers {');
    linhas.push('  Content-Type: application/json');
    linhas.push('}');
    linhas.push('');

    const schema = operation.requestBody.content['application/json'].schema;
    const corpo = exemplo(schema);
    linhas.push('body:json {');
    linhas.push(JSON.stringify(corpo, null, 2));
    linhas.push('}');
    linhas.push('');
  }

  if (temAuth) {
    linhas.push('auth:bearer {');
    linhas.push('  token: {{accessToken}}');
    linhas.push('}');
    linhas.push('');
  }

  return linhas.join('\n');
}

// ── Main ──────────────────────────────────────────────────────────────────────

fs.mkdirSync(BRUNO_DIR, { recursive: true });

// bruno.json (sempre regera)
fs.writeFileSync(
  path.join(BRUNO_DIR, 'bruno.json'),
  JSON.stringify({ version: '1', name: 'AgentesDaAlegria', type: 'collection', ignore: [] }, null, 2) + '\n'
);

// environments/local.bru (preserva edições manuais — cria só se não existir)
const envDir = path.join(BRUNO_DIR, 'environments');
fs.mkdirSync(envDir, { recursive: true });
const localBru = path.join(envDir, 'local.bru');
if (!fs.existsSync(localBru)) {
  fs.writeFileSync(localBru, [
    'vars {',
    '  baseUrl: http://localhost:5000',
    '  accessToken: ',
    '}',
    '',
  ].join('\n'));
  console.log('  + environments/local.bru');
}

// Remove pastas antigas geradas (preserva environments)
for (const entry of fs.readdirSync(BRUNO_DIR, { withFileTypes: true })) {
  if (entry.isDirectory() && entry.name !== 'environments') {
    fs.rmSync(path.join(BRUNO_DIR, entry.name), { recursive: true, force: true });
  }
}

// Gera arquivos .bru a partir dos paths do spec
const seqPorPasta = {};
let total = 0;

for (const [apiPath, pathItem] of Object.entries(spec.paths ?? {})) {
  for (const method of ['get', 'post', 'put', 'patch', 'delete']) {
    const op = pathItem[method];
    if (!op) continue;

    const pasta = pastaDoPath(apiPath);
    seqPorPasta[pasta] = (seqPorPasta[pasta] ?? 0) + 1;
    const seq = seqPorPasta[pasta];

    const pastaDir = path.join(BRUNO_DIR, pasta);
    fs.mkdirSync(pastaDir, { recursive: true });

    const nome = op.summary ?? humanizar(op.operationId ?? `${method} ${apiPath}`);
    const arquivo = `${String(seq).padStart(2, '0')} ${nomeSeguro(nome)}.bru`;

    fs.writeFileSync(path.join(pastaDir, arquivo), gerarBru(method, apiPath, op, seq));
    console.log(`  ✓ ${pasta}/${arquivo}`);
    total++;
  }
}

console.log(`\n${total} requisição(ões) gerada(s) em bruno/`);