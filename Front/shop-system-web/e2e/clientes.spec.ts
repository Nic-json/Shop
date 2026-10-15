
import { test, expect } from '@playwright/test';

test.describe('ShopSys - Clientes', () => {
  test('deve cadastrar, consultar e editar um cliente', async ({ page }) => {
    // Dados exclusivos para esta execução
    const identificador = `${Date.now()}-${Math.random()
      .toString(36)
      .slice(2, 8)}`;

    const nomeOriginal = `Cliente E2E ${identificador}`;
    const nomeAtualizado = `Cliente Editado ${identificador}`;
    const email = `e2e-${identificador}@teste.com`;

    // 1. Acessa a página de clientes
    await page.goto('/clientes');

    await expect(
      page.getByRole('heading', { name: 'Clientes', exact: true })
    ).toBeVisible();

    // 2. Abre o modal de cadastro
    await page.getByRole('button', {
      name: 'Novo cliente',
    }).click();

    // 3. Preenche o formulário
    await page.locator('#nome').fill(nomeOriginal);
    await page.locator('#email').fill(email);
    await page.locator('#telefone').fill('51999999999');

    // CPF/CNPJ é opcional; deixamos vazio para evitar
    // depender de um documento específico no teste.

    // 4. Salva o cliente
    const respostaCadastro = page.waitForResponse(
      (response) =>
        response.url().includes('/api/clientes') &&
        response.request().method() === 'POST'
    );

    await page.getByRole('button', {
      name: 'Salvar cliente',
    }).click();

    const cadastro = await respostaCadastro;
    expect(cadastro.status()).toBe(201);

    // 5. Procura o cliente na listagem
    await page.getByRole('searchbox', {
      name: 'Buscar clientes',
    }).fill(email);

    await expect(
      page.getByText(nomeOriginal, { exact: true })
    ).toBeVisible();

    // 6. Abre os detalhes do cliente pela tabela
    const linha = page.locator('tr').filter({
      hasText: email,
    });

    await linha.getByRole('button', {
      name: 'Abrir',
    }).click();

    // 7. Confirma que os detalhes foram carregados
    await expect(
      page.getByRole('heading', {
        name: nomeOriginal,
        exact: true,
      })
    ).toBeVisible();

    // 8. Abre o modal de edição
    await page.getByRole('button', {
      name: 'Editar cliente',
    }).click();

    await expect(page.locator('#nome')).toHaveValue(nomeOriginal);

    // 9. Atualiza o nome
    await page.locator('#nome').fill(nomeAtualizado);

    const respostaAtualizacao = page.waitForResponse(
      (response) =>
        response.url().includes('/api/clientes/') &&
        response.request().method() === 'PUT'
    );

    await page.getByRole('button', {
      name: 'Salvar alterações',
    }).click();

    const atualizacao = await respostaAtualizacao;
    expect(atualizacao.status()).toBe(204);

    // 10. Confirma a atualização nos detalhes
    await expect(
      page.getByRole('heading', {
        name: nomeAtualizado,
        exact: true,
      })
    ).toBeVisible();
  });
});
